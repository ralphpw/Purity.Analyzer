# TASK-011: Implement PUR007 (ref/out Parameters)

**Priority:** P1  
**Estimate:** 2 hours  
**Status:** Not Started  
**Depends On:** TASK-003, TASK-004

---

## Description

Detect `ref` and `out` parameters in `[EnforcedPure]` methods and emit PUR007 error.

---

## Diagnostic

| Property | Value |
|----------|-------|
| ID | PUR007 |
| Severity | Error |
| Message | Method '{0}' marked as pure has ref/out parameter '{1}' |

---

## What to Detect

- `ref` parameters
- `out` parameters

## What to Allow

- `in` parameters (read-only reference)
- Regular parameters (value or reference)

---

## Test Cases

```csharp
// ❌ Should Report
[EnforcedPure]
public void Bad(ref int x) => x = 42;

[EnforcedPure]
public bool Bad2(out int result) { result = 0; return true; }

// ✅ Should NOT Report
[EnforcedPure]
public int Good(in int x) => x * 2;

[EnforcedPure]
public int Good2(int x) => x * 2;
```

---

## Implementation

Check parameter modifiers in method declaration:

```csharp
foreach (var param in methodSymbol.Parameters)
{
    if (param.RefKind is RefKind.Ref or RefKind.Out)
    {
        ReportPUR007(context, param.Locations[0], methodSymbol.Name, param.Name);
    }
}
```

---

## Dependencies

- TASK-003, TASK-004
