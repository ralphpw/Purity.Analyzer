# BCL Purity Whitelist

This document describes the architecture and governance of the BCL (Base Class Library) purity whitelist used by Purity.Analyzer.

---

## Overview

The whitelist contains methods from the .NET BCL that have been verified as pure:
- No side effects (PUR001)
- No I/O operations (PUR003, PUR004)
- Deterministic output (PUR005)
- No mutation of parameters (PUR006)

Methods in the whitelist are trusted by the analyzer and do not require `[Pure]` or `[EnforcedPure]` attributes.

---

## Architecture

The whitelist system uses a **two-artifact design**:

### Source Files (Human-Readable)

Located in `/whitelist/`, organized by namespace:

```
/whitelist/
  /bcl/
    System.Math.md
    System.String.md
    System.Linq.Enumerable.md
    System.Collections.Immutable.md
    ...
  /third-party/           # Future: community-maintained
    ...
```

Each `.md` file contains:
- Markdown tables with full method signatures
- Review provenance (reviewer, date, .NET version)
- Human-readable notes and caveats

### Compiled Artifact (Runtime)

At build time, source files are compiled to:

```
/Purity.Analyzer/Resources/whitelist.csv
```

This CSV is embedded as a resource and loaded into a `HashSet<string>` at runtime for O(1) lookup.

**Why CSV?** One line = one signature = one `HashSet.Add()`. No parsing, no dependencies, trivially refactorable if needs change.

**Format:** One fully-qualified method signature per line, no headers, no metadata.

```csv
System.Math.Abs(System.Int32)
System.Math.Abs(System.Int64)
System.Math.Max(System.Int32,System.Int32)
System.String.Substring(System.Int32)
```

---

## Source File Format

Each whitelist source file uses the following Markdown table format:

| Signature | NoMutation | NoIO | Deterministic | NoExceptions | Reviewer | ReviewDate | DotNetVersion |
|-----------|------------|------|---------------|--------------|----------|------------|---------------|
| `Math.Abs(System.Int32)` | yes | yes | yes | yes | @ralphpw | 2024-12-08 | 8.0 |
| `Math.Abs(System.Int64)` | yes | yes | yes | yes | @ralphpw | 2024-12-08 | 8.0 |

### Column Definitions

| Column | Values | Description |
|--------|--------|-------------|
| **Signature** | | Fully-qualified method signature in CLR format |
| **NoMutation** | `yes`/`no` | Method does not mutate parameters or `this` |
| **NoIO** | `yes`/`no` | Method performs no I/O (file, network, console) |
| **Deterministic** | `yes`/`no` | Same inputs always produce same outputs |
| **NoExceptions** | `yes`/`warn`/`no` | `yes` = never throws, `warn` = throws on invalid input, `no` = throws normally |
| **Reviewer** | | GitHub handle or AI model identifier (e.g., `claude-opus-4-20250514`) |
| **ReviewDate** | | ISO 8601 date of review |
| **DotNetVersion** | | .NET version reviewed against |

### Notes Section

Each file may include a `## Notes` section for caveats:

```markdown
## Notes

- `Math.DivRem` can throw `DivideByZeroException` when divisor is 0
- Generic methods use backtick notation: `Enumerable.Where``1`
```

---

## Signature Format

Signatures use **CLR fully-qualified format**:

```
Namespace.Type.Method(ParamType1,ParamType2)
```

### Examples

| Method | Signature |
|--------|-----------|
| `Math.Abs(int)` | `System.Math.Abs(System.Int32)` |
| `string.Concat(string, string)` | `System.String.Concat(System.String,System.String)` |
| `ImmutableArray<T>.Add(T)` | `System.Collections.Immutable.ImmutableArray`1.Add(`0)` |
| `Enumerable.Where<T>(...)` | `System.Linq.Enumerable.Where``1(System.Collections.Generic.IEnumerable``1,System.Func``2)` |

### Generic Type Parameters

- Open generic types use backtick notation: `` `1 ``, `` `2 ``
- Generic parameters use positional notation: `` `0 ``, `` `1 ``

---

## Runtime Loading

```csharp
private static readonly HashSet<string> WhitelistedMethods = LoadWhitelist();

private static HashSet<string> LoadWhitelist()
{
    using var stream = typeof(PurityAnalyzer).Assembly
        .GetManifestResourceStream("Purity.Analyzer.Resources.whitelist.csv");
    using var reader = new StreamReader(stream!);
    
    var methods = new HashSet<string>(StringComparer.Ordinal);
    while (reader.ReadLine() is { } line)
        methods.Add(line);
    return methods;
}
```

---

## Review Governance

### Review Triggers

A method's whitelist entry should be re-reviewed when:
- A new major .NET version is released
- The method's implementation is known to have changed
- A bug report suggests the method may not be pure

### Review Policy

| Policy | Value |
|--------|-------|
| **Required Reviewers** | 1 (maintainer or verified AI) |
| **Staleness Warning** | None (manual review cycles) |
| **Evidence Required** | Source inspection or documentation |

### Reviewer Identification

- **Human:** GitHub handle (e.g., `@ralphpw`)
- **AI:** Model identifier with date (e.g., `claude-3.5-sonnet-2024-12`)

---

## Methods NOT Whitelisted

The following categories are explicitly **not** in the whitelist:

### Non-Deterministic (PUR005)
- `DateTime.Now`, `DateTime.UtcNow`
- `DateTimeOffset.Now`, `DateTimeOffset.UtcNow`
- `Guid.NewGuid()`
- `Random.*`
- `Environment.TickCount`, `Environment.ProcessId`

### I/O (PUR003, PUR004)
- `Console.*`
- `File.*`, `Directory.*`
- `Stream.*`
- `HttpClient.*`
- `Process.*`

### Mutation (PUR001)
- `List<T>.Add`, `Remove`, `Clear`, etc.
- `Dictionary<TKey, TValue>.Add`, `Remove`, etc.
- `StringBuilder.*` (mutates internal state)
- `Array.Copy`, `Array.Sort` (mutates target)

### Reflection (PUR007)
- `Type.GetType(string)`
- `Assembly.Load*`
- `MethodInfo.Invoke`
- `Activator.CreateInstance`

---

## User Configuration

### Overview

Users need control over the whitelist for several reasons:
- **Third-party libraries** — Methods not in the BCL whitelist
- **Internal libraries** — Company code compiled without Purity.Analyzer
- **Zero-trust mode** — Security-sensitive contexts requiring explicit review
- **Gradual adoption** — Incremental purity enforcement

### Whitelist vs Warning Suppression

Standard .NET warning suppression (`.editorconfig`, `#pragma`) silences diagnostics but **does not affect purity analysis**. A suppressed method remains "impure" for transitive analysis.

The whitelist is different: it marks methods as **genuinely pure**, allowing callers to also be pure.

```csharp
// Suppression: Calculate compiles, but Wrapper fails (Calculate isn't "pure")
[EnforcedPure]
public int Calculate(int x)
{
    #pragma warning disable PUR002
    return ThirdParty.Compute(x);
    #pragma warning restore PUR002
}

[EnforcedPure]
public int Wrapper(int x) => Calculate(x);  // ❌ PUR002: Calculate calls non-pure method
```

```csharp
// Whitelist: Both compile (ThirdParty.Compute is trusted as pure)

[EnforcedPure]
public int Calculate(int x) => ThirdParty.Compute(x);  // ✅

[EnforcedPure] 
public int Wrapper(int x) => Calculate(x);  // ✅
```

### User Whitelist Directory: `.purity/whitelist/`

User whitelists use **the same markdown format as the built-in whitelist**. This ensures:
- Reviews are auditable (who verified, when, against what .NET version)
- LLM-assisted review outputs can be used directly
- User contributions can be submitted upstream via PR

```
.purity/
├── config.json              # Trust mode, excludes, settings
└── whitelist/               # User-verified methods (same schema as /whitelist/)
    ├── ThirdParty.Lib.md
    ├── MyCompany.Utils.md
    └── ...
```

Example `.purity/whitelist/ThirdParty.Lib.md`:

```markdown
# ThirdParty.Lib Purity Whitelist

## Methods

| Signature | NoMutation | NoIO | Deterministic | NoExceptions | Reviewer | ReviewDate | DotNetVersion |
|-----------|------------|------|---------------|--------------|----------|------------|---------------|
| `ThirdParty.Lib.Calculate(System.Int32)` | yes | yes | yes | yes | @yourname | 2025-12-08 | 8.0 |
| `ThirdParty.Lib.Transform(System.String)` | yes | yes | yes | warn | claude-opus-4-20250514 | 2025-12-08 | 8.0 |

## Notes

- `Transform` may throw `ArgumentNullException` on null input
```

### Configuration File: `.purity/config.json`

Controls trust mode and exclusions (not inclusions—those go in whitelist files):

```json
{
  "trustMode": "standard",
  "exclude": [
    "System.Math.DivRem"
  ],
  "reviewRequired": [
    "ThirdParty.Lib.Transform"
  ]
}
```

Templates: [`.purity/config.template.json`](../.purity/config.template.json), [`.purity/whitelist/_template.md`](../.purity/whitelist/_template.md)

### Trust Modes

| Mode | BCL Whitelist | `[EnforcedPure]` in Refs | User Whitelist | Behavior |
|------|---------------|-------------------------|----------------|----------|
| **standard** | ✅ Trusted | ✅ Trusted | ✅ Trusted | Default—trust verified sources |
| **strict** | ✅ Trusted | ⚠️ Verify | ✅ Trusted | Re-verify `[EnforcedPure]` in dependencies |
| **zero-trust** | ❌ Not trusted | ❌ Not trusted | ✅ Trusted | Only user whitelist trusted |

#### Standard Mode (Default)

Trust BCL whitelist, trust `[EnforcedPure]` in referenced assemblies.

```json
{ "trustMode": "standard" }
```

#### Strict Mode

Re-analyze methods marked `[EnforcedPure]` in referenced assemblies. Useful when you don't control the referenced code.

```json
{ "trustMode": "strict" }
```

#### Zero-Trust Mode

**Nothing is trusted by default.** Every method call must be either:
1. In the user's `.purity/whitelist/*.md` files
2. Marked `[EnforcedPure]` in the current compilation

```json
{ "trustMode": "zero-trust" }
```

Whitelisted methods go in `.purity/whitelist/*.md` (same format as built-in):

```
.purity/
├── config.json           # { "trustMode": "zero-trust" }
└── whitelist/
    └── reviewed.md       # Your reviewed methods
```

This mode is designed for **LLM-assisted review workflows**:

1. Analyzer emits PUR002 for every untrusted method
2. User (or LLM) reviews the methods
3. Approved methods are added to `.purity/whitelist/*.md`
4. Build succeeds once all methods are reviewed

### Global vs Project Configuration

| File | Scope | Precedence |
|------|-------|------------|
| `~/.purity/config.json` | User-global | Lowest |
| `$(SolutionDir)/.purity/config.json` | Solution-wide | Medium |
| `$(ProjectDir)/.purity/config.json` | Project-specific | Highest |

Configurations **merge** with higher precedence winning for conflicts.

### Review-Required Methods

For methods you want to call but haven't fully verified, use `reviewRequired`:

```json
{
  "whitelist": {
    "include": ["ThirdParty.Lib.Calculate"]
  },
  "reviewRequired": ["ThirdParty.Lib.Calculate"]
}
```

This allows the build to pass but emits **PUR011** (informational):

```
PUR011: Method 'Calculate' calls 'ThirdParty.Lib.Calculate' which is marked for review
```

---

## LLM-Assisted Review Workflow

### The Problem

In zero-trust mode, every BCL method triggers PUR002. Manually reviewing hundreds of methods is impractical.

### The Solution

Integrate with LLM tooling to automate review:

```
┌─────────────────────────────────────────────────────────────┐
│                    Build Process                             │
│                                                              │
│  1. Analyzer runs in zero-trust mode                        │
│  2. Collects all PUR002 violations                          │
│  3. Outputs review manifest: .purity/pending-review.json    │
│                                                              │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                    LLM Review Agent                          │
│                                                              │
│  1. Reads pending-review.json                               │
│  2. For each method:                                        │
│     - Fetches .NET runtime source (if BCL)                  │
│     - Analyzes for purity violations                        │
│     - Records verdict + reasoning                           │
│  3. Outputs: .purity/reviewed.json                          │
│                                                              │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                    Merge to Config                           │
│                                                              │
│  1. Approved methods → .purity/whitelist/*.md               │
│  2. Rejected methods → document why, refactor code          │
│  3. Uncertain methods → reviewRequired                      │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Review Manifest Format

Generated by analyzer when `--output-pending-review` is specified:

```json
{
  "generated": "2025-12-08T10:30:00Z",
  "trustMode": "zero-trust",
  "pendingReview": [
    {
      "signature": "System.Math.Abs(System.Int32)",
      "calledFrom": [
        { "file": "Calculator.cs", "line": 42, "method": "Calculate" }
      ],
      "runtimeSourceUrl": "https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Math.cs"
    }
  ]
}
```

### Reviewed Output Format

```json
{
  "reviewed": "2025-12-08T11:00:00Z",
  "reviewer": "claude-opus-4-20250514",
  "methods": [
    {
      "signature": "System.Math.Abs(System.Int32)",
      "verdict": "pure",
      "confidence": "high",
      "reasoning": "Single CPU instruction, no side effects, deterministic",
      "sourceReviewed": "dotnet/runtime@abc123"
    }
  ]
}
```

---

## Extending the Whitelist

### Contributing to the Default Whitelist

To propose additions to the default BCL whitelist:

1. Open an issue describing the method(s) to add
2. Provide evidence the method is pure:
   - Link to .NET runtime source code
   - Property-based test demonstrating determinism
   - No observable side effects
3. Submit a PR adding the method to the appropriate `/whitelist/bcl/*.md` file

**Criteria for inclusion:**
- Method must be deterministic
- Method must have no side effects
- Method must not perform I/O
- Method must not mutate parameters
- Method must be from the BCL (not third-party)

---

## Whitelist Compilation

A build-time tool compiles `/whitelist/**/*.md` → `whitelist.csv`:

1. Recursively find all `.md` files in `/whitelist/`
2. Parse Markdown tables, extract `Signature` column
3. Validate signature format
4. Write one signature per line to `whitelist.csv`
5. Embed as assembly resource

**Build integration:** See [TASK-017](../Tasks/TASK-017-Whitelist-Compilation.md) for implementation details.

---

## File Index

See `/whitelist/` for the source files:

- [`/whitelist/bcl/System.Math.md`](../whitelist/bcl/System.Math.md)
- [`/whitelist/bcl/System.String.md`](../whitelist/bcl/System.String.md)
- [`/whitelist/bcl/System.Linq.Enumerable.md`](../whitelist/bcl/System.Linq.Enumerable.md)
- [`/whitelist/bcl/System.Collections.Immutable.md`](../whitelist/bcl/System.Collections.Immutable.md) *(planned)*
- ... (more to come)
