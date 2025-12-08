# Task Index

## v0.1.0 — Core MVP ✅

| Task | Status | Description |
|------|--------|-------------|
| TASK-001 | ✅ | Initialize repository and CI |
| TASK-002 | ✅ | Create `[EnforcedPure]` attribute |
| TASK-003 | ✅ | Roslyn analyzer skeleton |
| TASK-004 | ✅ | Test infrastructure |
| TASK-005 | ✅ | PUR001 (field mutation) |
| TASK-006 | ✅ | PUR002 (non-pure calls) |
| TASK-007 | ✅ | PUR003 (I/O detection) |
| TASK-008 | ✅ | PUR004 (non-determinism) |
| TASK-012 | ✅ | README.md |
| TASK-013 | ✅ | CONTRIBUTING.md |
| TASK-014 | ✅ | docs/ folder |
| TASK-015 | ✅ | NuGet packaging |
| TASK-017 | ✅ | Whitelist compilation |

---

## v0.2.0 — Full Verification ✅

| Task | Status | Description |
|------|--------|-------------|
| TASK-009 | ✅ | PUR005 (mutable returns) |
| TASK-010 | ✅ | PUR006 (param mutation) |
| TASK-011 | ✅ | PUR007 (ref/out params) |
| TASK-019 | ✅ | PUR008 (unsafe code) |
| TASK-020 | ✅ | PUR009 (reflection) |
| TASK-021 | ✅ | PUR010 (exception control flow) |
| TASK-018 | ✅ | User config & trust modes |
| TASK-016 | Not Started | Announce v0.2.0 |

---

## Future Features

| Task | Priority | Description |
|------|----------|-------------|
| TASK-022 | Medium | Demo/Sample Project - Standalone project consuming NuGet package |
| TASK-023 | High | Code Fixes - Auto-fix violations (add [EnforcedPure], convert Parse to TryParse, inject IClock) |
| TASK-024 | Medium | Deep Data Flow Analysis - Track parameter mutations through method calls |
| TASK-025 | Low | Auto-generate BCL Whitelist - Scrape .NET API docs to maintain whitelist |
| TASK-026 | Low | Async Purity Model - Define purity semantics for Task<T> and async methods |

