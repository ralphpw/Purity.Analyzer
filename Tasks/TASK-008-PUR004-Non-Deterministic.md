# TASK-008: Implement PUR004 (Non-Deterministic APIs)

**Priority:** P0  
**Estimate:** 3 hours  
**Status:** Not Started  
**Depends On:** TASK-003, TASK-004

---

## Description

Detect usage of non-deterministic APIs within `[EnforcedPure]` methods and emit PUR004 error.

---

## Diagnostic

| Property | Value |
|----------|-------|
| ID | PUR004 |
| Severity | Error |
| Message | Method '{0}' marked as pure uses non-deterministic API '{1}' |

---

## Implementation

### Non-Deterministic APIs to Detect

**DateTime/DateTimeOffset:**
- `DateTime.Now`
- `DateTime.UtcNow`
- `DateTime.Today`
- `DateTimeOffset.Now`
- `DateTimeOffset.UtcNow`

**Guid:**
- `Guid.NewGuid()`

**Random:**
- `System.Random` constructor
- `Random.Next()`, `NextDouble()`, `NextBytes()`, etc.

**Environment:**
- `Environment.TickCount`
- `Environment.TickCount64`
- `Environment.ProcessId` (varies per process)

### Detection Strategy

Two types of access to detect:
1. **Property access** (e.g., `DateTime.Now`)
2. **Method calls** (e.g., `Guid.NewGuid()`)

```csharp
private static readonly HashSet<string> NonDeterministicMembers = new()
{
    // DateTime
    "System.DateTime.Now",
    "System.DateTime.UtcNow",
    "System.DateTime.Today",
    
    // DateTimeOffset
    "System.DateTimeOffset.Now",
    "System.DateTimeOffset.UtcNow",
    
    // Guid
    "System.Guid.NewGuid",
    
    // Environment
    "System.Environment.TickCount",
    "System.Environment.TickCount64",
    "System.Environment.ProcessId",
};

private static readonly HashSet<string> NonDeterministicTypes = new()
{
    "System.Random",
};
```

### Code Sketch

```csharp
private void CheckForNonDeterministicApis(
    SyntaxNodeAnalysisContext context,
    MethodDeclarationSyntax method,
    IMethodSymbol methodSymbol)
{
    foreach (var node in method.DescendantNodes())
    {
        switch (node)
        {
            case MemberAccessExpressionSyntax memberAccess:
                CheckMemberAccess(context, memberAccess, methodSymbol);
                break;
                
            case ObjectCreationExpressionSyntax creation:
                CheckObjectCreation(context, creation, methodSymbol);
                break;
        }
    }
}

private void CheckMemberAccess(
    SyntaxNodeAnalysisContext context,
    MemberAccessExpressionSyntax memberAccess,
    IMethodSymbol methodSymbol)
{
    var symbol = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol;
    if (symbol is null)
        return;
    
    var fullName = symbol.ContainingType?.ToDisplayString() + "." + symbol.Name;
    
    if (NonDeterministicMembers.Contains(fullName))
    {
        ReportPUR004(context, memberAccess.GetLocation(), methodSymbol.Name, fullName);
    }
}

private void CheckObjectCreation(
    SyntaxNodeAnalysisContext context,
    ObjectCreationExpressionSyntax creation,
    IMethodSymbol methodSymbol)
{
    var typeSymbol = context.SemanticModel.GetTypeInfo(creation).Type;
    if (typeSymbol is null)
        return;
    
    var typeName = typeSymbol.ToDisplayString();
    
    if (NonDeterministicTypes.Contains(typeName))
    {
        ReportPUR004(context, creation.GetLocation(), methodSymbol.Name, $"new {typeName}()");
    }
}
```

---

## Test Cases

### Should Report PUR004

```csharp
// DateTime.Now
[EnforcedPure]
public DateTime Bad()
{
    return DateTime.Now;  // ❌ PUR004
}

// DateTime.UtcNow
[EnforcedPure]
public DateTime Bad2()
{
    return DateTime.UtcNow;  // ❌ PUR004
}

// Guid.NewGuid
[EnforcedPure]
public Guid Bad3()
{
    return Guid.NewGuid();  // ❌ PUR004
}

// Random constructor
[EnforcedPure]
public int Bad4()
{
    var rng = new Random();  // ❌ PUR004
    return rng.Next();
}

// Random with methods
[EnforcedPure]
public int Bad5(Random rng)
{
    return rng.Next();  // ❌ PUR004 (Random.Next is non-deterministic)
}

// Environment.TickCount
[EnforcedPure]
public int Bad6()
{
    return Environment.TickCount;  // ❌ PUR004
}
```

### Should NOT Report

```csharp
// DateTime constructor (deterministic)
[EnforcedPure]
public DateTime Good(int year, int month, int day)
{
    return new DateTime(year, month, day);  // ✅ OK
}

// Guid.Parse (deterministic)
[EnforcedPure]
public Guid Good2(string input)
{
    return Guid.Parse(input);  // ✅ OK
}

// DateTime arithmetic (deterministic)
[EnforcedPure]
public DateTime Good3(DateTime dt, TimeSpan offset)
{
    return dt.Add(offset);  // ✅ OK
}

// Guid.Empty (constant)
[EnforcedPure]
public Guid Good4()
{
    return Guid.Empty;  // ✅ OK
}
```

---

## Acceptance Criteria

- [ ] Detects `DateTime.Now` and `DateTime.UtcNow`
- [ ] Detects `DateTimeOffset.Now` and `DateTimeOffset.UtcNow`
- [ ] Detects `Guid.NewGuid()`
- [ ] Detects `new Random()` and `Random.*` methods
- [ ] Detects `Environment.TickCount`
- [ ] Does NOT flag `DateTime` constructor
- [ ] Does NOT flag `Guid.Parse`
- [ ] Diagnostic shows API name
- [ ] All test cases pass

---

## Edge Cases

- `DateTime.Now` accessed via variable: `var dt = DateTime.Now;`
- Random passed as parameter (should we flag `Random.Next()` on a parameter?)
- `Task.Delay` (uses time internally but is more about async)

---

## Dependencies

- TASK-003 (analyzer skeleton)
- TASK-004 (test infrastructure)

---

## Blocks

None (independent diagnostic)
