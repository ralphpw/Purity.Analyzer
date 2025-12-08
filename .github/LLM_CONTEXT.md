# Purity.Analyzer — LLM Context

**Last Updated:** 2025-12-08  
**For:** AI agents implementing or extending this project

---

## What This Is

A Roslyn analyzer that **verifies functional purity at compile time** for C# methods marked with `[EnforcedPure]`. Unlike the BCL's advisory `[Pure]` attribute, violations produce compile errors.

## What This Is NOT

- A runtime purity checker
- A replacement for F#
- A full functional programming framework

---

## Current State

| Component | Status |
|-----------|--------|
| Design & Documentation | ✅ Complete |
| `[EnforcedPure]` attribute | 🔲 Not started |
| Analyzer skeleton | 🔲 Not started |
| PUR001–PUR005 diagnostics | 🔲 Not started |
| BCL whitelist | 🔲 Partial (Math, String, Enumerable source files exist) |
| User configuration | 🔲 Not started |

---

## Key Documents

| Document | Purpose | Read When |
|----------|---------|-----------|
| [docs/FunctionalSpec.md](docs/FunctionalSpec.md) | **Authoritative** specification | Before any implementation |
| [docs/Rules.md](docs/Rules.md) | Diagnostic definitions with examples | Implementing PUR001–PUR010 |
| [docs/Whitelist.md](docs/Whitelist.md) | Whitelist architecture & trust model | Working on PUR002 or whitelist |
| [.github/LLMStyleGuideline.md](.github/LLMStyleGuideline.md) | Code style requirements | Before writing any code |
| [Tasks/INDEX.md](Tasks/INDEX.md) | Implementation task index | Planning work |

---

## Implementation Order

```
1. TASK-001 → Repository setup, CI
2. TASK-002 → [EnforcedPure] attribute
3. TASK-003 → Analyzer skeleton
4. TASK-004 → Test infrastructure
5. TASK-005 → PUR001 (field mutation)
6. TASK-006 → PUR002 (non-pure calls) ← Most complex
7. TASK-007–009 → PUR003–PUR005
```

---

## Trust Model Summary

Trust behavior is configurable via `.purity/config.json`:

| Mode | BCL Whitelist | `[EnforcedPure]` in Refs | User Whitelist |
|------|---------------|-------------------------|----------------|
| **standard** | ✅ Trusted | ✅ Trusted | ✅ Trusted |
| **strict** | ✅ Trusted | ⚠️ Verify | ✅ Trusted |
| **zero-trust** | ❌ Not trusted | ❌ Not trusted | ✅ Trusted |

**Zero-trust mode** enables LLM-assisted review workflows—see [Whitelist.md](docs/Whitelist.md#llm-assisted-review-workflow).

---

## Common Mistakes to Avoid

1. **Don't use reflection in analyzer code** — Breaks performance, use Roslyn symbols
2. **Don't trust `[Pure]` from external assemblies** — It's advisory only
3. **Don't forget transitive analysis** — If A calls B calls C, all must be pure
4. **Don't block on I/O** — Analyzers must be fast
5. **Don't return mutable collections from analysis** — Use `ImmutableArray<T>`

---

## Style Quick Reference

```
INTERNAL:      Write like Dapper (domain names, zero ceremony)
EXTERNAL:      Design like Minimal APIs (minimal surface)
COMPOSABILITY: Flow like LINQ (pipe intent)
NAMING:        Empathy like Stripe (zero ambiguity)
FAILURE:       Explicit like Rust (no hidden nulls)
```

- Private fields: `_camelCase`
- LINQ over foreach
- Switch expressions must throw on default
- No `dynamic`, no `async void`, no `.Result`

---

## Next Steps

If you're an LLM implementing this project, start with:

1. Read [TASK-001](Tasks/TASK-001-Initialize-Repository.md)
2. Create the solution structure
3. Proceed through tasks in order

Each task file contains implementation sketches and test cases.
