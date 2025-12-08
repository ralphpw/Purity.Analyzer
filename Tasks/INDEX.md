# Task Index

Implementation tasks for Purity.Analyzer.

---

## v0.1.0 — Core MVP (First Usable Release)

**Goal:** Catch the most common purity violations with minimal whitelist.

| Task | Estimate | Status | Suggested LLM | Description |
|------|----------|--------|---------------|-------------|
| [TASK-001](TASK-001-Initialize-Repository.md) | 1h | ✅ Done | Sonnet | Initialize repository and CI |
| [TASK-002](TASK-002-Create-EnforcedPureAttribute.md) | 30m | ✅ Done | Sonnet | Create `[EnforcedPure]` attribute |
| [TASK-003](TASK-003-Analyzer-Skeleton.md) | 2h | ✅ Done | **Opus** | Set up Roslyn analyzer skeleton |
| [TASK-004](TASK-004-Test-Infrastructure.md) | 2h | ✅ Done | **Opus** | Set up unit test infrastructure |
| [TASK-005](TASK-005-PUR001-Field-Mutation.md) | 4h | ✅ Done | Sonnet | Implement PUR001 (field mutation) |
| [TASK-007](TASK-007-PUR003-IO-Detection.md) | 3h | ✅ Done | Sonnet | Implement PUR003 (I/O detection) |
| [TASK-008](TASK-008-PUR004-Non-Deterministic.md) | 3h | ✅ Done | Sonnet | Implement PUR004 (non-determinism) |
| [TASK-017](TASK-017-Whitelist-Compilation.md) | 3h | ✅ Done | **Opus** | Whitelist compilation pipeline |
| [TASK-006](TASK-006-PUR002-Non-Pure-Calls.md) | 6h | ✅ Done | **Opus** | Implement PUR002 (non-pure calls) |
| [TASK-015](TASK-015-NuGet-Publish.md) | 2h | Not Started | Sonnet | Package and publish to NuGet |
| TASK-012 | 2h | ✅ Done | Sonnet | Write README.md |
| [TASK-013](TASK-013-CONTRIBUTING.md) | 1h | ✅ Done | Sonnet | Write CONTRIBUTING.md |
| [TASK-014](TASK-014-Documentation.md) | 3h | ✅ Done | Sonnet | Create docs/ folder |

**Total:** 10 tasks, ~0h remaining  
**Opus tasks:** 4 (infrastructure, complex logic) - All done  
**Sonnet tasks:** 6 (straightforward diagnostics, docs) - 1 remaining (NuGet)

---

## v0.2.0 — Full Verification (Production-Ready)

**Goal:** Complete diagnostic coverage, user config, and trust modes.

| Task | Estimate | Status | Suggested LLM | Description |
|------|----------|--------|---------------|-------------|
| [TASK-009](TASK-009-PUR005-Mutable-Return.md) | 4h | ✅ Done | Sonnet | Implement PUR005 (mutable returns) |
| [TASK-010](TASK-010-PUR006-Parameter-Mutation.md) | 3h | ✅ Done | Sonnet | Implement PUR006 (param mutation) |
| [TASK-011](TASK-011-PUR007-Ref-Out-Params.md) | 2h | ✅ Done | Sonnet | Implement PUR007 (ref/out params) |
| [TASK-018](TASK-018-User-Configuration.md) | 6h | ✅ Done | **Opus** | User config & trust modes |
| [TASK-016](TASK-016-Announce.md) | 2h | Not Started | Sonnet | Announce v0.2.0 |

**Total:** 5 tasks, ~0h remaining  
**Opus tasks:** 1 (complex config system) - Done  
**Sonnet tasks:** 4 (pattern-matching diagnostics) - 3 done, 1 remaining (announce)

---

## LLM Selection Rationale

**Use Opus (3x cost) for:**
- Architecture/infrastructure setup (TASK-003, TASK-004)
- Complex algorithms requiring deep reasoning (TASK-006 call-graph analysis, TASK-017 compilation)
- Multi-system integration (TASK-018 config system)

**Use Sonnet for:**
- Pattern-matching diagnostics (PUR001, 003, 004, 005, 006, 007, 009, 010, 011)
- Documentation/boilerplate (TASK-001, 002, 012, 013, 014, 015, 016)
- Well-defined transformations with clear examples

---

## Implementation Order (v0.1.0)

Dependencies require this sequence:

1. **TASK-001** — Initialize repository (foundation)
2. **TASK-002** — Create attribute (API surface)
3. **TASK-003** — Analyzer skeleton (registration, infrastructure)
4. **TASK-004** — Test infrastructure (validate as we build)
5. **TASK-005** — PUR001 (field mutation) — simplest diagnostic, proves the pipeline works
6. **TASK-007** — PUR003 (I/O detection) — high-impact, independent
7. **TASK-008** — PUR004 (non-determinism) — easy win, independent
8. **TASK-017** — Whitelist compilation — required before PUR002
9. **TASK-006** — PUR002 (non-pure calls) — depends on whitelist, enables transitive checking
10. **TASK-015** — NuGet publish — ship it

---

## Dependency Graph

```
v0.1.0 (Core MVP)
┌─────────────────────────────────────────────────────────────┐
│ TASK-001 (repo)                                             │
│    ├── TASK-002 (attribute)                                 │
│    │      └── TASK-003 (analyzer) ─┬─ TASK-005 (PUR001)    │
│    │                                ├─ TASK-007 (PUR003)    │
│    │                                └─ TASK-008 (PUR004)    │
│    ├── TASK-004 (tests) ───────────┘                        │
│    └── TASK-017 (whitelist) ──► TASK-006 (PUR002)          │
│                                                              │
│ All above ──► TASK-015 (NuGet)                              │
└─────────────────────────────────────────────────────────────┘

v0.2.0 (Full Verification)
┌─────────────────────────────────────────────────────────────┐
│ TASK-009 (PUR005) ─┬                                        │
│ TASK-010 (PUR006) ─┼─► TASK-016 (Announce)                 │
│ TASK-011 (PUR007) ─┤                                        │
│ TASK-018 (config) ─┘                                        │
└─────────────────────────────────────────────────────────────┘
```
