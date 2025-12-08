# Purity.Analyzer

**Compile-time enforcement of functional purity in C#**

A Roslyn analyzer that verifies methods marked with `[EnforcedPure]` are genuinely pure—no side effects, no I/O, no non-determinism. Catch impurity at compile time, not runtime.

[![NuGet](https://img.shields.io/nuget/v/Purity.Analyzer.svg)](https://www.nuget.org/packages/Purity.Analyzer)
[![License](https://img.shields.io/github/license/ralphpw/Purity.Analyzer)](LICENSE)

## Why?

C# lacks built-in support for enforcing functional purity. `System.Diagnostics.Contracts.PureAttribute` is advisory only—nothing stops "pure" methods from writing to disk or mutating state.

Purity.Analyzer is a **pragmatic heuristic tool** that statically verifies purity constraints at compile time. It won't catch everything (reflection, unsafe code, some async patterns), but it catches the common violations that matter:

- **Fearless refactoring**: Pure methods can be reordered, parallelized, or memoized safely
- **Easier testing**: Pure methods need no mocks—just inputs and expected outputs
- **Better architecture**: Purity boundaries make code easier to reason about

Not a formal proof system—a useful guardrail. If 80% of purity bugs are caught in CI instead of production, that's a win

## Quick Start

```bash
dotnet add package Purity.Analyzer
```

```csharp
using Purity.Contracts;

public class Calculator
{
    [EnforcedPure]
    public int Add(int a, int b) => a + b; // ✅ Compiles

    [EnforcedPure]
    public int AddWithLogging(int a, int b)
    {
        Console.WriteLine($"Adding {a} + {b}"); // ❌ PUR003: Performs I/O
        return a + b;
    }
}
```

## Supported Rules

| Diagnostic | Description |
|------------|-------------|
| **PUR001** | Method mutates a field |
| **PUR002** | Method calls a non-pure method |
| **PUR003** | Method performs I/O |
| **PUR004** | Method uses non-deterministic API (DateTime.Now, Guid.NewGuid, Random) |
| **PUR005** | Method returns a mutable type |
| **PUR006** | Method mutates a parameter |
| **PUR007** | Method has ref/out parameters |
| **PUR008** | Method uses unsafe code or pointers |
| **PUR009** | Method uses reflection |
| **PUR010** | Method uses exception-based control flow |

See [docs/Rules.md](docs/Rules.md) for detailed explanations and examples.

## Two-Tier Verification

Purity.Analyzer uses two attributes with different trust models:

### `[EnforcedPure]` — Verified at Compile Time

Methods marked `[EnforcedPure]` are fully analyzed. When calling other `[EnforcedPure]` methods (including from dependencies), the analyzer trusts them without re-analysis—they were already verified when compiled.

```csharp
[EnforcedPure]
public int Double(int x) => Multiply(x, 2); // ✅ Trusts Multiply

[EnforcedPure]
public int Multiply(int a, int b) => a * b;
```

### `[Pure]` — BCL Compatibility (Always Verified)

The BCL's `System.Diagnostics.Contracts.PureAttribute` is recognized but not trusted. Methods marked with `[Pure]` are always re-analyzed, because `[Pure]` is advisory-only and may not actually be pure.

```csharp
[EnforcedPure]
public int Calculate(int x)
{
    return Math.Abs(x); // ✅ Math.Abs is whitelisted
}
```

## BCL Whitelist

Common BCL methods known to be pure are whitelisted:

- `System.Math.*`
- `System.String` (most methods)
- `System.Linq.Enumerable.*`
- `System.Collections.Immutable.*`

See [docs/Whitelist.md](docs/Whitelist.md) for the full list and how to extend it.

## Configuration

```
.purity/
├── config.json              # Trust mode, excludes
└── whitelist/               # Your verified methods (same schema as built-in)
    └── ThirdParty.Lib.md
```

User whitelists use the same markdown format as the built-in whitelist, enabling auditable reviews and upstream contribution.

Templates: [config.template.json](.purity/config.template.json), [whitelist/\_template.md](.purity/whitelist/_template.md)

### Trust Modes

| Mode | Description |
|------|-------------|
| **standard** | Trust BCL whitelist, `[EnforcedPure]` in refs, and `.purity/whitelist/` |
| **strict** | Re-verify `[EnforcedPure]` in referenced assemblies |
| **zero-trust** | Only `.purity/whitelist/` trusted—for LLM-assisted review |

See [docs/Whitelist.md](docs/Whitelist.md#trust-modes) for details.

## Limitations

Purity.Analyzer is a **pragmatic heuristic tool**, not a formal proof system. It catches common violations but has known gaps:

**What it does well:**
- Detects direct field mutation, I/O calls, non-deterministic APIs
- Prevents most accidental purity violations
- Fails loudly (compile error) when unsure

**What it can't verify:**
- Purity of methods without source (relies on whitelist)
- Reflection-based mutations or dynamic invocations
- Unsafe code, span manipulation, or unmanaged interop
- Async/await state machines (partial support)
- Transitive exceptions from whitelisted methods

**Philosophy:** If 80% of purity violations are caught at compile time, that's a massive improvement over 0%. The whitelist + suppressions give you escape hatches when the analyzer is wrong.

This is not an academic proof of correctness—it's a **useful guardrail** that makes functional C# safer and more maintainable.

## Roadmap

- **v0.1.0** — Core diagnostics (PUR001–PUR005), basic whitelist, NuGet package
- **v0.2.0** — Full call-graph analysis, expanded whitelist, user configuration
- **v1.0.0** — Code fixes, detailed documentation, production-ready

See [GitHub Issues](https://github.com/ralphpw/Purity.Analyzer/issues) for current status.

## Contributing

We welcome contributions! See [CONTRIBUTING.md](CONTRIBUTING.md) for:

- How to build locally
- How to run tests
- How to add a new diagnostic
- Code style guidelines

## License

MIT License. See [LICENSE](LICENSE) for details.

---

*Built with ❤️ for the functional C# community*
