# Motivation: Why Purity.Analyzer?

---

## The Problem

C# is a multi-paradigm language that supports functional programming patterns:
- LINQ for declarative data transformation
- Immutable collections (`ImmutableList<T>`, `ImmutableArray<T>`)
- Records for immutable data types
- Pattern matching for expressive control flow
- Init-only properties for controlled initialization

Yet despite these features, **C# provides no compile-time guarantee of functional purity**.

### The Current State

**`[Pure]` is just documentation:**

```csharp
[Pure]  // This is a lie—nothing enforces it
public int Add(int a, int b)
{
    Console.WriteLine("Adding...");  // Side effect!
    _callCount++;                     // Mutation!
    return a + b;
}
```

The BCL's `System.Diagnostics.Contracts.PureAttribute` exists, but:
- The compiler ignores it completely
- Code Contracts (which could check it) is deprecated and removed from modern .NET
- IDE tools like ReSharper warn when pure method results are ignored, but don't verify actual purity

**The result:** Developers must rely on discipline and code review to maintain functional enclaves. This doesn't scale.

---

## Why Purity Matters

### Testability

Pure functions need no mocks, no setup, no teardown—just inputs and expected outputs:

```csharp
[EnforcedPure]
public int Add(int a, int b) => a + b;

[Test]
public void Add_ReturnsSum() => Assert.AreEqual(5, Add(2, 3));
```

### Parallelization

Pure code is inherently thread-safe:

```csharp
// Safe to parallelize
[EnforcedPure]
public ImmutableList<int> Transform(ImmutableList<int> items)
    => items.Select(x => x * 2).ToImmutableList();

// Can safely run in parallel
var results = data.AsParallel().Select(Transform).ToList();
```

No locks, no race conditions, no shared mutable state. Pure functions can be distributed across threads, cores, or even machines.

### Reasoning and Refactoring

Pure functions have **referential transparency**—the compiler can safely inline, memoize, reorder, or eliminate calls without changing behavior.

### Domain Modeling

Many domains benefit from purity guarantees:

**Finance:** Calculations must be auditable and reproducible
```csharp
[EnforcedPure]
public decimal CalculateInterest(decimal principal, decimal rate, int periods)
    => principal * (decimal)Math.Pow((double)(1 + rate), periods) - principal;
```

**Healthcare:** Diagnostic algorithms must be deterministic
```csharp
[EnforcedPure]
public RiskLevel AssessCardiacRisk(PatientData data)
    => /* pure calculation based on input data */;
```

**Scientific Computing:** Simulations must be reproducible
```csharp
[EnforcedPure]
public SimulationState Step(SimulationState current, SimulationParams params)
    => /* pure state transition */;
```

---

## Why Doesn't This Already Exist?

No production-ready purity analyzer exists for C#. The technical complexity is significant:
- **Transitive checking:** Must verify entire call graphs
- **BCL knowledge:** Must know which BCL methods are pure
- **Performance:** Must analyze without slowing builds
- **False positives:** Must be accurate enough to be useful

Microsoft's implicit stance is "use F# for functional programming"—but many organizations have large C# codebases and need functional guarantees in specific subsystems, not entire applications.

---

## Why Now?

### The Ecosystem is Ready

**Roslyn analyzers are mature:**
- Nullable reference types proved that strict analyzers work at scale
- The analyzer ecosystem is well-understood
- Tooling (CSharpSyntaxWalker, SemanticModel) is battle-tested

**C# has the building blocks:**
- `record` types for immutable data (C# 9+)
- `readonly struct` for stack-allocated immutability
- `System.Collections.Immutable` for immutable collections
- `init` accessors for controlled initialization
- Pattern matching for functional-style conditionals

**Community demand is growing:**
- Discussions on Reddit, StackOverflow, GitHub
- Interest in functional C# libraries (LanguageExt, CSharpFunctionalExtensions)
- Adoption of Result/Option patterns

### The Gap

Purity.Analyzer fills the gap between:
- **"Disciplined FP in C#"** — relying on conventions and code review
- **"Enforced FP in F#"** — requiring a different language entirely

With Purity.Analyzer, teams can:
- Mark critical code paths as `[EnforcedPure]`
- Get compile-time verification of purity contracts
- Gradually adopt functional patterns without rewriting in F#
- Trust that pure code stays pure as the codebase evolves

---

## The Vision

Purity.Analyzer enables **functional enclaves** within C# applications:

```
┌─────────────────────────────────────────────────┐
│                  Application                     │
│                                                  │
│  ┌──────────────┐    ┌──────────────┐           │
│  │   Impure     │    │   Impure     │           │
│  │  (I/O, UI,   │◄──►│  (Database,  │           │
│  │   Config)    │    │   Network)   │           │
│  └──────┬───────┘    └──────┬───────┘           │
│         │                   │                    │
│         ▼                   ▼                    │
│  ┌─────────────────────────────────────┐        │
│  │         Pure Core Domain            │        │
│  │    [EnforcedPure] - Verified        │        │
│  │                                     │        │
│  │  • Business logic                   │        │
│  │  • Calculations                     │        │
│  │  • Transformations                  │        │
│  │  • Validations                      │        │
│  └─────────────────────────────────────┘        │
│                                                  │
└─────────────────────────────────────────────────┘
```

The impure shell handles I/O and external communication. The pure core contains business logic that is:
- Easy to test
- Safe to parallelize
- Easy to reason about
- Guaranteed to remain pure

---

## Summary

| Without Purity.Analyzer | With Purity.Analyzer |
|------------------------|----------------------|
| `[Pure]` is documentation only | `[EnforcedPure]` is verified |
| Trust relies on code review | Trust is automated |
| Purity violations slip through | Violations are compile errors |
| Refactoring is risky | Pure code can be freely refactored |
| Testing requires mocks | Pure functions test trivially |
| Parallelization needs careful analysis | Pure code is inherently parallel-safe |

**Purity.Analyzer brings compile-time purity verification to C#, enabling safer, more testable, and more maintainable codebases.**
