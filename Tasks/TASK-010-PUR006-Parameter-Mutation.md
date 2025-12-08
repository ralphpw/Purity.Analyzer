# TASK-010: Implement PUR006 (Parameter Mutation)

**Priority:** P1  
**Estimate:** 3 hours  
**Status:** Not Started  
**Depends On:** TASK-003, TASK-004

---

## Description

Detect mutations to parameters within `[EnforcedPure]` methods and emit PUR006 error.

---

## Diagnostic

| Property | Value |
|----------|-------|
| ID | PUR006 |
| Severity | Error |
| Message | Method '{0}' marked as pure mutates parameter '{1}' |

---

## What to Detect

- Calling mutating methods on parameters: `list.Add(x)`, `dict.Clear()`
- Assigning to parameter properties: `param.Value = x`
- Assigning to parameter fields: `param.field = x`

## What NOT to Flag

- Reading parameter properties: `param.Count`
- Calling non-mutating methods: `param.Contains(x)`
- Value type parameter reassignment: `x = 5` (doesn't affect caller)

---

## Test Cases

```csharp
// ❌ Should Report
[EnforcedPure]
public void Bad(List<int> list) => list.Add(42);

[EnforcedPure]
public void Bad2(Person p) => p.Name = "Changed";

// ✅ Should NOT Report
[EnforcedPure]
public int Good(List<int> list) => list.Count;

[EnforcedPure]
public bool Good2(List<int> list) => list.Contains(5);
```

---

## Implementation Notes

Detect mutating methods by checking:
1. Method name patterns: `Add`, `Remove`, `Clear`, `Set*`, `Insert`, `Push`, `Pop`, `Enqueue`, `Dequeue`
2. Property setters on parameters
3. Field assignments on parameters

---

## Dependencies

- TASK-003, TASK-004
