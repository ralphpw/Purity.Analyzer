# LLM Style Guideline

**Project:** Purity.Analyzer  
**Effective Date:** 2025-12-08  
**Status:** Authoritative

---

## 1. Code Exemplars

When writing code for Purity.Analyzer, channel these established libraries:

**Primary Exemplar: [Dapper](https://github.com/DapperLib/Dapper)**
- External API: Zero ceremony, pure human intent
- Internal: Domain-focused naming (`reader`, `command`, `parameters` not `r`, `cmd`, `p`)
- Functional style with `?.` and `??`
- Minimal abstraction layers
- **The Dapper Principle:** If you can't explain what this line does to a human in 3 seconds, rewrite it

**Secondary Exemplar: [Meziantou.Analyzer](https://github.com/meziantou/Meziantou.Analyzer)**
- Well-maintained Roslyn analyzer reference implementation
- Clear diagnostic messages with actionable fixes
- Comprehensive test coverage patterns

**Execution Mantra:**
```
INTERNAL:      Write like Dapper (domain names, zero ceremony, one responsibility)
EXTERNAL:      Design like Minimal APIs (minimal surface, obvious defaults)  
COMPOSABILITY: Flow like LINQ (pipe intent, avoid stateful noise)
NAMING:        Empathy like Stripe (zero ambiguity for domain experts)
FAILURE:       Explicit like Rust (no hidden nulls or implicit fallbacks)
```

---

## 2. Scope

This document governs all AI-assisted contributions to Purity.Analyzer, including code generation, documentation, issue triage, and architectural proposals.

---

## 3. Writing Standards

### 3.1 Documentation

- **Precision over brevity.** Every claim must be verifiable. Avoid hedging ("might", "could", "probably").
- **No marketing language.** Documentation serves engineers, not end users. Eliminate subjective qualifiers ("elegant", "powerful", "simple").
- **Concrete examples required.** Abstract descriptions must be accompanied by compilable code samples.
- **Comment philosophy: MINIMIZE COGNITIVE LOAD.** Actively prune comments that duplicate what code already says through naming and structure.
  - Bad: `// Loop through methods` above `foreach (var method in methods)`
  - Good: `// Use exponential backoff to handle rate limiting` above complex algorithm
- **Comments explain WHY, not WHAT.** If naming and structure make the code self-explanatory, omit the comment.
- **Formal tone.** Use academic prose conventions: passive voice where appropriate, third person for specifications.

### 3.2 Code Comments

- **Explain invariants, not mechanics.** Good: `// Ensures transitive purity: all callees must be [EnforcedPure] or whitelisted`. Bad: `// Loop through methods`.
- **Document non-obvious decisions.** Why this approach over alternatives? What assumptions hold?
- **No redundant comments.** Do not restate what is syntactically obvious.
- **Link to sources:** Framework bugs link to bug reports. Non-obvious patterns cite references (Roslyn docs, established analyzer patterns).

### 3.3 Error Messages

- **Actionable diagnostics.** Every error must indicate (1) what failed, (2) why it failed, (3) how to fix it.
- **Consistent structure:** `{DiagnosticID}: {What} {Why} {Where}`
  - Example: `PUR001: Method 'Calculate' marked as [EnforcedPure] mutates field '_cache' at line 42`

---

## 4. Code Standards

### 4.1 Naming Conventions

**Private Fields:** Use `_camelCase` prefix.
- Rationale: Instant visual distinction (field `_cache` vs parameter `cache`), grep-friendly, discourages accidental access widening.

**Private Methods:** Use `PascalCase` (standard .NET convention).
- Rationale: Methods are methods regardless of visibility. Underscore prefix on methods is unconventional and confuses contributors.

```csharp
// Analyzer class example
public class PurityAnalyzer : DiagnosticAnalyzer
{
    private readonly ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;
    private readonly PurityWhitelist _whitelist;
    
    private bool IsMethodPure(IMethodSymbol method) { ... }
    private void AnalyzeMethodBody(SyntaxNodeAnalysisContext context) { ... }
}
```

**Public API:** Use PascalCase. Names must be domain-focused, not abstract.
- Bad: `public void Analyze(SyntaxNode node, DataSource source)`
- Good: `public void AnalyzeMethodPurity(MethodDeclarationSyntax method, SemanticModel semanticModel)`

**Avoid generic names** like `dataSource`, `config`, `context` when domain-specific names are available.

### 4.2 Architecture

- **Single Responsibility Principle.** Analyzers detect violations only. Code fixes perform remediation only. No mixing.
- **Immutability by default.** Prefer `readonly` fields, `ImmutableArray<T>`, `record` types.
- **Avoid premature abstraction.** Ship concrete implementations before extracting interfaces. Every abstraction must buy reuse, invariants, or test isolation. If removing a layer improves clarity, remove it.

### 4.3 Functional Style

**Prefer functional patterns over imperative:**
```csharp
// Bad: Imperative noise
var pureMethodSymbols = new List<IMethodSymbol>();
foreach (var method in methods)
{
    if (method.GetAttributes().Any(a => a.AttributeClass?.Name == "EnforcedPureAttribute"))
        pureMethodSymbols.Add(method);
}

// Good: Functional flow
var pureMethodSymbols = methods
    .Where(m => m.GetAttributes().Any(a => a.AttributeClass?.Name == "EnforcedPureAttribute"))
    .ToImmutableArray();
```

**Explicit error handling:** All failure modes must be obvious at call site.
```csharp
// Bad: Hidden null
var method = GetMethodSymbol(node);

// Good: Explicit failure
var method = GetMethodSymbol(node) ?? throw new InvalidOperationException($"Method symbol not found for {node}");
```

**Switch expressions must throw on default:**
```csharp
// Required pattern
return syntaxKind switch
{
    SyntaxKind.FieldDeclaration => AnalyzeFieldMutation(node),
    SyntaxKind.InvocationExpression => AnalyzeMethodCall(node),
    _ => throw new InvalidOperationException($"Unexpected syntax kind: {syntaxKind}")
};

// Forbidden: Silent failure
_ => null  // Never return null/empty on unhandled cases
```

**Fail Early:** Validate at method entry, not deep in logic.
```csharp
public void AnalyzePurity(IMethodSymbol method)
{
    var methodSymbol = method ?? throw new ArgumentNullException(nameof(method));
    if (!HasPurityAttribute(methodSymbol))
        return; // Early exit for non-pure methods
    
    // Main logic follows only after validation
    var violations = DetectViolations(methodSymbol);
    ReportDiagnostics(violations);
}
```

### 4.4 Modern C# Syntax

**Prefer modern collection and initialization syntax:**
```csharp
// Collection expressions
var diagnosticIds = ["PUR001", "PUR002", "PUR003"];
var emptyViolations = ImmutableArray<PurityViolation>.Empty;

// Target-typed new
Dictionary<string, bool> purityCache = new();
List<IMethodSymbol> pureMethods = new();

// Null-coalescing with empty collection fallback
var methods = type.GetMembers().OfType<IMethodSymbol>().ToArray() ?? [];
var attributes = symbol.GetAttributes().ToImmutableArray();
```

### 4.5 Record Types for Simple Data

Prefer `record` for diagnostic results and simple data containers. Inline them in the analyzer file when they're only used locally.

```csharp
// Inline record for purity analysis results
public class PurityAnalyzer : DiagnosticAnalyzer
{
    private record PurityViolation(Location Location, string ViolationType, string Details);
    
    private ImmutableArray<PurityViolation> DetectViolations(IMethodSymbol method) { ... }
}
```

### 4.6 Roslyn Conventions

- **Use semantic model where available.** Prefer `ISymbol` analysis over syntax trees for type resolution.
- **Cache compilation units.** Do not re-parse the same syntax trees.
- **Report diagnostics at precise locations.** Use `Location.Create()` with exact syntax spans, not entire method bodies.
- **Lazy evaluation.** Defer expensive analysis until a purity attribute is detected.
- **Memoization.** Cache transitive purity results across method invocations.

### 4.7 Performance

- Target: < 5% build time overhead on large codebases
- Profile and optimize call-graph analysis (most expensive operation)
- Use incremental analysis patterns where applicable (Phase 2+)

---

## 5. Testing Standards

### 5.1 Coverage Requirements

**Every diagnostic must have:**
- At least 3 positive test cases (code that violates the rule)
- At least 3 negative test cases (code that complies)
- At least 1 edge case (e.g., nested lambdas, generic methods)

### 5.2 Test Naming

**Pattern:** `{DiagnosticID}_{Scenario}_{ExpectedOutcome}`
- Example: `PUR001_FieldMutation_ReportsError`
- Example: `PUR002_CallsPureMethod_NoError`
- Example: `PUR005_ReturnsImmutableList_NoError`

### 5.3 Assertions

- **Precise verification.** Test exact diagnostic message, not just presence.
- **Test all locations.** Verify `Location` spans match expected syntax nodes.
- **Use Roslyn test helpers:** `Microsoft.CodeAnalysis.Testing` framework.

---

## 6. Git Commit Standards

### 6.1 Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:** `feat`, `fix`, `docs`, `test`, `refactor`, `perf`, `chore`

**Scope:** `analyzer`, `attributes`, `tests`, `docs`

**Example:**
```
feat(analyzer): implement PUR001 field mutation detection

Adds diagnostic for methods marked [EnforcedPure] that mutate instance
or static fields. Includes semantic analysis to distinguish field
assignments from local variable mutations.

Closes #12
```

### 6.2 Commit Hygiene

- **Atomic commits.** Each commit compiles and passes tests.
- **No "WIP" commits.** Squash before merge.
- **Separate concerns.** Do not mix refactoring with feature additions.

---

## 7. Pull Request Standards

### 7.1 Required Sections

Every PR must include:

1. **Summary:** What changes, why.
2. **Testing:** How was this verified?
3. **Breaking Changes:** List any API surface changes.
4. **Checklist:**
   - [ ] Tests pass locally
   - [ ] Documentation updated
   - [ ] Changelog entry added (if user-facing)

### 7.2 Review Expectations

- **Self-review first.** Read your own diff before requesting review.
- **Address all feedback.** Do not mark threads as resolved without explanation.
- **No merge without approval.** Maintainers must approve all PRs.

---

## 8. Documentation Maintenance

### 8.1 Synchronization

- **Single source of truth.** Functional specification is authoritative. README and inline docs derive from it.
- **Update atomically.** Code changes that alter behavior require corresponding doc updates in the same PR.

### 8.2 Examples

- **All examples must compile.** Include full `using` statements and class wrappers where necessary.
- **Test examples.** Add example code to integration tests to prevent documentation drift.

---

## 9. Prohibited Patterns

### 9.1 Code

- **No `dynamic` type** (defeats static analysis).
- **No reflection in analyzers** (performance, maintainability).
- **No `#pragma` disables** in production code (indicates design flaw).
- **No wrapper methods around single framework calls** (adds indirection without value).
- **No `async void`** except for true event handlers (unobservable exceptions).
- **No `.Result` or `.Wait()`** on tasks (deadlock risk, use `await`).

### 9.2 Documentation

- **No TODO comments in shipped docs.** Move to GitHub issues.
- **No unverified performance claims.** Cite benchmarks or omit.
- **No placeholder content.** Empty sections indicate incomplete work.

---

## 10. Versioning

### 10.1 Semantic Versioning

- **Major (X.0.0):** Breaking changes to analyzer rules or attribute APIs.
- **Minor (0.X.0):** New diagnostics, new code fixes.
- **Patch (0.0.X):** Bug fixes, documentation corrections.

### 10.2 Compatibility

- **Analyzer backward compatibility.** New rules must be opt-in via severity configuration.
- **Attribute forward compatibility.** Old attributes must work with new analyzers.

---

## 11. LLM-Specific Guidelines

### 11.1 Code Generation

- **Always provide rationale.** Explain why this implementation was chosen.
- **Cite Roslyn patterns.** Link to existing analyzers (Meziantou.Analyzer, Microsoft.CodeAnalysis.NetAnalyzers) that use similar techniques.
- **Acknowledge limitations.** If a proposed solution has edge cases, document them explicitly.

### 11.2 Reflection Questions

Use these during code review (not rapid drafting):
- Does reading this file top-to-bottom feel like narrating a story, not deciphering a machine?
- Would the diff of this change be trivial to understand 6 months from now?
- Is every abstraction buying reuse, invariants, or test isolation? If not, collapse it.
- Are all failure modes explicit at the call site (throw / null / default / recovery)?
- Could removing one layer improve clarity? If yes, do it.
- Does naming eliminate the need for comments in 90%+ of the code?

### 11.3 Issue Triage

- **Categorize precisely.** Use labels: `bug`, `enhancement`, `documentation`, `question`.
- **Propose solutions, not just acknowledgments.** Every triaged bug should include a hypothesized root cause.

### 11.4 Architecture Proposals

- **Alternatives analysis required.** Present at least two approaches with trade-offs.
- **Migration path.** Breaking changes must include upgrade instructions.

---

## 12. References

- [.NET Compiler Platform SDK](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/)
- [Dapper](https://github.com/DapperLib/Dapper) (code exemplar)
- [Meziantou.Analyzer](https://github.com/meziantou/Meziantou.Analyzer) (analyzer exemplar)
- [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Semantic Versioning 2.0.0](https://semver.org/)

---

**End of Document**
