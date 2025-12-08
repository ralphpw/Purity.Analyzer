# Task Index

Implementation tasks for Purity.Analyzer v0.1.0 (MVP).

---

## Priority Legend

- **P0** — MVP-critical, must complete
- **P1** — Nice-to-have for MVP, can defer

---

## Task Summary

| Task | Priority | Estimate | Status | Description |
|------|----------|----------|--------|-------------|
| [TASK-001](TASK-001-Initialize-Repository.md) | P0 | 1h | Not Started | Initialize repository and CI |
| [TASK-002](TASK-002-Create-EnforcedPureAttribute.md) | P0 | 30m | Not Started | Create `[EnforcedPure]` attribute |
| [TASK-003](TASK-003-Analyzer-Skeleton.md) | P0 | 2h | Not Started | Set up Roslyn analyzer skeleton |
| [TASK-004](TASK-004-Test-Infrastructure.md) | P0 | 2h | Not Started | Set up unit test infrastructure |
| [TASK-005](TASK-005-PUR001-Field-Mutation.md) | P0 | 4h | Not Started | Implement PUR001 (field mutation) |
| [TASK-006](TASK-006-PUR002-Non-Pure-Calls.md) | P0 | 6h | Not Started | Implement PUR002 (non-pure calls) |
| [TASK-007](TASK-007-PUR003-IO-Detection.md) | P0 | 3h | Not Started | Implement PUR003 (I/O detection) |
| [TASK-008](TASK-008-PUR004-Non-Deterministic.md) | P0 | 3h | Not Started | Implement PUR004 (non-determinism) |
| [TASK-009](TASK-009-PUR005-Mutable-Return.md) | P0 | 4h | Not Started | Implement PUR005 (mutable returns) |
| [TASK-010](TASK-010-PUR006-Parameter-Mutation.md) | P1 | 3h | Not Started | Implement PUR006 (param mutation) |
| [TASK-011](TASK-011-PUR007-Ref-Out-Params.md) | P1 | 2h | Not Started | Implement PUR007 (ref/out params) |
| TASK-012 | P0 | 2h | ✅ Done | Write README.md |
| [TASK-013](TASK-013-CONTRIBUTING.md) | P1 | 1h | ✅ Done | Write CONTRIBUTING.md |
| [TASK-014](TASK-014-Documentation.md) | P1 | 3h | ✅ Done | Create docs/ folder |
| [TASK-015](TASK-015-NuGet-Publish.md) | P0 | 2h | Not Started | Package and publish to NuGet |
| [TASK-016](TASK-016-Announce.md) | P1 | 2h | Not Started | Announce v0.1.0 |
| [TASK-017](TASK-017-Whitelist-Compilation.md) | P0 | 3h | Not Started | Whitelist compilation pipeline |
| [TASK-018](TASK-018-User-Configuration.md) | P1 | 6h | Not Started | User config & trust modes |

---

## Effort Summary

| Priority | Tasks | Estimated Hours |
|----------|-------|-----------------|
| P0 | 12 tasks | ~31h |
| P1 | 6 tasks | ~17h |
| **Total** | 18 tasks | ~48h |

---

## Dependency Graph

```
TASK-001 (repo)
    ├── TASK-002 (attribute)
    │       └── TASK-003 (analyzer) ─┬─ TASK-005 (PUR001)
    │                                ├─ TASK-006 (PUR002) ──► TASK-018 (config)
    │                                ├─ TASK-007 (PUR003)
    │                                ├─ TASK-008 (PUR004)
    │                                ├─ TASK-009 (PUR005)
    │                                ├─ TASK-010 (PUR006)
    │                                └─ TASK-011 (PUR007)
    │
    ├── TASK-004 (tests) ────────────┘
    │
    └── TASK-017 (whitelist compile)

TASK-012 (README) ✅
TASK-013 (CONTRIBUTING) ✅
TASK-014 (docs/) ✅

All P0 tasks ──► TASK-015 (NuGet) ──► TASK-016 (Announce)
```

---

## Suggested Order

1. **TASK-001** — Initialize repository
2. **TASK-002** — Create attribute
3. **TASK-003** — Analyzer skeleton
4. **TASK-004** — Test infrastructure
5. **TASK-017** — Whitelist compilation pipeline
6. **TASK-005** — PUR001 (field mutation)
7. **TASK-006** — PUR002 (non-pure calls)
8. **TASK-007** — PUR003 (I/O)
9. **TASK-008** — PUR004 (non-determinism)
10. **TASK-009** — PUR005 (mutable returns)
11. **TASK-010** — PUR006 (param mutation) [P1]
12. **TASK-011** — PUR007 (ref/out) [P1]
13. **TASK-018** — User config & trust modes [P1]
14. **TASK-015** — NuGet publish
15. **TASK-016** — Announce [P1]
