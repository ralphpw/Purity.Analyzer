---
description: Code style and contribution guidelines for Purity.Analyzer
applyTo: '**'
---

# Purity.Analyzer Style Guidelines

Read and follow the guidelines in `.github/LLMStyleGuideline.md` for all code generation, documentation, and architectural decisions.

## Quick Reference

**Code Exemplar:** Write like [Dapper](https://github.com/DapperLib/Dapper) — domain names, zero ceremony, functional style.

**Execution Mantra:**
- INTERNAL: Write like Dapper (domain names, zero ceremony, one responsibility)
- EXTERNAL: Design like Minimal APIs (minimal surface, obvious defaults)
- COMPOSABILITY: Flow like LINQ (pipe intent, avoid stateful noise)
- NAMING: Empathy like Stripe (zero ambiguity for domain experts)
- FAILURE: Explicit like Rust (no hidden nulls or implicit fallbacks)

**Naming:**
- Private fields: `_camelCase`
- Private methods: `PascalCase`
- Public API: `PascalCase`, domain-focused names

**Required Patterns:**
- Functional style over imperative (LINQ > foreach)
- Fail early with explicit exceptions
- Switch expressions must throw on default
- Modern C# syntax (collection expressions, target-typed new)

**Prohibited:**
- No `dynamic`, no reflection in analyzers
- No `async void`, no `.Result` or `.Wait()`
- No wrapper methods around single framework calls

**Commit Format:** `<type>(<scope>): <subject>` (feat, fix, docs, test, refactor, perf, chore)

For complete guidelines, see `.github/LLMStyleGuideline.md`.
