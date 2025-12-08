# Purity.Analyzer Functional Specification

**Version:** 1.0  
**Status:** Active

---

## Overview

Purity.Analyzer is a Roslyn-based static analyzer that enforces functional purity in C# codebases. It provides compile-time verification that methods, classes, and structs marked with purity attributes are free from side effects, mutation, and non-deterministic behavior.

---

## Attributes

### `[EnforcedPure]`

**Namespace:** `Purity.Contracts`  
**Package:** `Purity.Analyzer.Attributes`

**Targets:** `Method`, `Class`, `Struct`

**Semantics:**
- Applied to a **method**: that method must be pure
- Applied to a **class/struct**: all instance and static methods in that type must be pure

```csharp
namespace Purity.Contracts
{
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct,
        Inherited = false)]
    public sealed class EnforcedPureAttribute : Attribute { }
}
```

### `[Pure]` (BCL)

**Namespace:** `System.Diagnostics.Contracts`

The BCL's existing `PureAttribute` is recognized but treated differently:
- Claims purity as documentation intent
- Analyzer verifies but does **not trust** without analysis
- Same purity rules apply during verification

**Rationale:**
- Many codebases already use `[Pure]` for documentation
- Supporting it provides immediate value and eases migration
- Creates continuity with Code Contracts heritage

---

## Two-Tier Verification System

The analyzer uses different trust models for each attribute. Trust behavior is configurable via `.purity/config.json`.

### `[EnforcedPure]` — Verified at Compile Time

| Context | Standard Mode | Strict Mode | Zero-Trust Mode |
|---------|---------------|-------------|-----------------|
| **Current compilation** | Verify | Verify | Verify |
| **Referenced assemblies** | **Trust** | **Verify** | ❌ PUR002 |

**Performance benefit:** In standard mode, large call graphs skip deep transitive analysis for trusted methods.

**Trust assumption:** Code marked `[EnforcedPure]` in a referenced assembly was verified when originally compiled with Purity.Analyzer.

### `[Pure]` — Claimed (Always Verified in Standard/Strict)

| Context | Standard Mode | Strict Mode | Zero-Trust Mode |
|---------|---------------|-------------|-----------------|
| **Current compilation** | Verify | Verify | Verify |
| **Referenced assemblies** | **Verify** | **Verify** | ❌ PUR002 |

**Rationale:** Third-party libraries may have `[Pure]` on methods that aren't actually pure. The analyzer cannot trust unenforced claims.

### Trust Modes

See [Whitelist.md](Whitelist.md) for full configuration details.

| Mode | Use Case |
|------|----------|
| **standard** | Default—trust verified sources, suitable for most projects |
| **strict** | Re-verify external `[EnforcedPure]`—when you don't control dependencies |
| **zero-trust** | Only user whitelist trusted—for LLM-assisted review workflows |

### Example

```csharp
// Library A (compiled WITH Purity.Analyzer)
[EnforcedPure]
public int Add(int a, int b) => a + b;  // Verified at compile time

// Library B (references Library A)
[EnforcedPure]
public int Calculate(int x)
{
    return Add(x, 5);  // ✅ Trusts Add() in standard mode
                       // ⚠️ Re-verifies Add() in strict mode
                       // ❌ PUR002 in zero-trust mode (unless whitelisted)
}

// Third-party library (NOT compiled with Purity.Analyzer)
[Pure]  // Just a claim, not verified
public int ThirdPartyMethod() => /* ... */;

// Your code
[EnforcedPure]
public int UseThirdParty()
{
    return ThirdPartyMethod();  // ⚠️ Analyzer verifies in standard/strict
                                // ❌ PUR002 in zero-trust (unless whitelisted)
}
```

### Preventing False Trust

**Problem:** What if someone adds `[EnforcedPure]` to impure code without running the analyzer?

**Mitigation:**
- The analyzer **always verifies** `[EnforcedPure]` methods in the current compilation
- In standard mode, it trusts `[EnforcedPure]` on referenced assemblies
- Use **strict mode** if you don't control referenced assemblies
- Use **zero-trust mode** for security-sensitive code requiring explicit review

**Best Practice:**
- Use `[EnforcedPure]` on code you compile with Purity.Analyzer
- Use **strict mode** when depending on code you don't control
- Use **zero-trust mode** for auditable purity verification

---

## Diagnostics

Each violation emits a **compile-time error** (except PUR010 and PUR011 which are warnings).

| ID | Severity | Description |
|----|----------|-------------|
| **PUR001** | Error | Method mutates a field |
| **PUR002** | Error | Method calls a non-pure method |
| **PUR003** | Error | Method performs I/O |
| **PUR004** | Error | Method uses non-deterministic API |
| **PUR005** | Error | Method returns a mutable type |
| **PUR006** | Error | Method mutates a parameter |
| **PUR007** | Error | Method has ref/out parameter |
| **PUR008** | Error | Method uses unsafe code |
| **PUR009** | Error | Method uses reflection |
| **PUR010** | Warning | Method uses exception-based control flow |
| **PUR011** | Info | Method calls a method marked for review |

See [Rules.md](Rules.md) for detailed explanations and examples.

---

## Architecture

```
Purity.Analyzer/
├── Purity.Analyzer/                    # Roslyn analyzer (DiagnosticAnalyzer)
│   ├── Analyzers/
│   │   ├── PurityAnalyzer.cs           # Main analyzer logic
│   │   ├── CallGraphAnalyzer.cs        # Transitive call analysis
│   │   └── WhitelistManager.cs         # BCL whitelist management
│   ├── Diagnostics/
│   │   └── DiagnosticDescriptors.cs    # PUR001-PUR010 definitions
│   └── Resources/
│       └── bcl-whitelist.json          # Embedded whitelist
├── Purity.Analyzer.Attributes/         # Attribute definitions (separate package)
│   └── EnforcedPureAttribute.cs
├── Purity.Analyzer.Tests/              # Unit tests
│   └── *.cs
└── Purity.Analyzer.sln
```

### Implementation Strategy

1. **Attribute Detection**
   - Scan for `[EnforcedPure]` or `[Pure]` on methods, classes, structs
   - If `[EnforcedPure]` in referenced assembly → trust it, skip analysis
   - If `[EnforcedPure]` in current compilation → verify it
   - If `[Pure]` anywhere → always verify it

2. **Method Body Analysis**
   - Walk the syntax tree for each pure method
   - Check assignments → PUR001, PUR006
   - Check invocations → PUR002
   - Check object creation → PUR005
   - Check member access → PUR004
   - Check unsafe/reflection → PUR008, PUR009

3. **Transitive Analysis (Call Graph)**
   - Build call graph for each pure method
   - For each called method:
     - `[EnforcedPure]` in referenced assembly → trust
     - `[EnforcedPure]` in current compilation → verify recursively
     - `[Pure]` anywhere → verify recursively
     - In BCL whitelist → trust
     - Otherwise → emit PUR002
   - Cache results to avoid re-analysis

4. **Whitelist Lookup**
   - Check fully-qualified method name against embedded whitelist
   - See [Whitelist.md](Whitelist.md) for details

5. **Diagnostic Reporting**
   - Emit errors with precise location (syntax node)
   - Include helpful message with method and violation details

---

## Configuration

### EditorConfig

```ini
[*.cs]
# Adjust severity per-rule
dotnet_diagnostic.PUR001.severity = error
dotnet_diagnostic.PUR005.severity = warning
dotnet_diagnostic.PUR010.severity = none

# Custom whitelist path (Phase 2)
purity_analyzer.whitelist_path = purity-whitelist.json
```

### Custom Whitelist (Phase 2)

Users can provide a `purity.json` file:

```json
{
  "whitelist": [
    "MyCompany.Utils.SafeHelper.ComputeHash",
    "MyCompany.Math.*"
  ]
}
```

---

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.CodeAnalysis.CSharp` | >= 4.0 | Roslyn APIs |
| `Microsoft.CodeAnalysis.Analyzers` | >= 3.3 | Analyzer development |
| `Microsoft.CodeAnalysis.Testing.NUnit` | latest | Unit testing |

### Target Frameworks

| Project | Framework |
|---------|-----------|
| Purity.Analyzer | netstandard2.0 |
| Purity.Analyzer.Attributes | netstandard2.0 |
| Purity.Analyzer.Tests | net8.0 |

**Rationale:** `netstandard2.0` ensures compatibility with all .NET SDK versions.

---

## Code Fixes (Phase 2+)

| Diagnostic | Suggested Fix |
|------------|---------------|
| **PUR005** | Replace `List<T>` with `ImmutableList<T>` |
| **PUR002** | Mark called method `[EnforcedPure]` (if owned) |
| **PUR007** | Change `ref`/`out` to `in` or return tuple |

---

## Phases

### Phase 1: MVP (v0.1.0)
- `[EnforcedPure]` attribute package
- Diagnostics PUR001–PUR005
- Support for `[Pure]` attribute
- Small BCL whitelist (Math, String basics)
- Unit tests for each diagnostic
- NuGet package

### Phase 2: Call Graph + Whitelist (v0.2.0)
- Full call-graph analysis (PUR002 fully enforced)
- Expanded BCL whitelist (LINQ, immutable collections)
- User-configurable whitelist
- Performance benchmarks

### Phase 3: Code Fixes + Polish (v1.0.0)
- Code fixes for common violations
- Detailed documentation
- Sample projects
- v1.0.0 stable release

### Future (v2.0+)
- Effect tracking: `[Pure]` vs `[Deterministic]` vs `[Total]`
- Custom effect types: `Task<T>` as pure if computation is pure
- Whole-assembly enforcement: `[assembly: EnforcedPure]`

---

## Open Questions

1. **Async pure methods?**
   - Proposal: Allow `Task<T>` if computation is pure (no I/O)
   - Defer to Phase 2

2. **Lambdas and local functions?**
   - Proposal: Analyze as methods; enforce purity if closing over pure context
   - Phase 1: Disallow closure over mutable state

3. **Allow throw for unrecoverable errors?**
   - Yes, but emit PUR010 warning suggesting `Result<T>` pattern
   - Phase 1: Allow throws, warn in Phase 2

4. **Whitelist governance?**
   - Core team approves additions
   - Community PRs with proof (property-based tests)
