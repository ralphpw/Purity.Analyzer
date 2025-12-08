# TASK-005: Implement PUR001 (Field Mutation)

**Priority:** P0  
**Estimate:** 4 hours  
**Status:** Not Started  
**Depends On:** TASK-003, TASK-004

---

## Description

Detect assignment to instance or static fields within `[EnforcedPure]` methods and emit PUR001 error.

---

## Diagnostic

| Property | Value |
|----------|-------|
| ID | PUR001 |
| Severity | Error |
| Message | Method '{0}' marked as pure mutates field '{1}' |

---

## Implementation

### Detection Logic

1. For each method marked as pure:
2. Walk the method body looking for:
   - `AssignmentExpressionSyntax` where target is a field
   - `PrefixUnaryExpressionSyntax` (`++field`, `--field`)
   - `PostfixUnaryExpressionSyntax` (`field++`, `field--`)
   - Compound assignments (`field += x`, etc.)
3. Use semantic model to determine if target is a field symbol
4. Report diagnostic with field name

### Code Sketch

```csharp
private void CheckForFieldMutation(
    SyntaxNodeAnalysisContext context,
    MethodDeclarationSyntax method,
    IMethodSymbol methodSymbol)
{
    foreach (var descendant in method.DescendantNodes())
    {
        switch (descendant)
        {
            case AssignmentExpressionSyntax assignment:
                CheckAssignmentTarget(context, assignment.Left, methodSymbol);
                break;
                
            case PrefixUnaryExpressionSyntax prefix 
                when prefix.IsKind(SyntaxKind.PreIncrementExpression) 
                  || prefix.IsKind(SyntaxKind.PreDecrementExpression):
                CheckAssignmentTarget(context, prefix.Operand, methodSymbol);
                break;
                
            case PostfixUnaryExpressionSyntax postfix
                when postfix.IsKind(SyntaxKind.PostIncrementExpression)
                  || postfix.IsKind(SyntaxKind.PostDecrementExpression):
                CheckAssignmentTarget(context, postfix.Operand, methodSymbol);
                break;
        }
    }
}

private void CheckAssignmentTarget(
    SyntaxNodeAnalysisContext context,
    ExpressionSyntax target,
    IMethodSymbol methodSymbol)
{
    var symbol = context.SemanticModel.GetSymbolInfo(target).Symbol;
    
    if (symbol is IFieldSymbol fieldSymbol)
    {
        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.PUR001,
            target.GetLocation(),
            methodSymbol.Name,
            fieldSymbol.Name);
        context.ReportDiagnostic(diagnostic);
    }
}
```

---

## Test Cases

### Should Report PUR001

```csharp
// Instance field assignment
private int _counter;

[EnforcedPure]
public int Bad()
{
    _counter = 42;  // ❌ PUR001
    return _counter;
}

// Static field assignment
private static int _global;

[EnforcedPure]
public void Bad2()
{
    _global = 100;  // ❌ PUR001
}

// Increment/decrement
[EnforcedPure]
public int Bad3()
{
    _counter++;     // ❌ PUR001
    return _counter;
}

// Compound assignment
[EnforcedPure]
public int Bad4()
{
    _counter += 10; // ❌ PUR001
    return _counter;
}
```

### Should NOT Report

```csharp
// Local variable mutation is OK
[EnforcedPure]
public int Good(int x)
{
    int local = x;
    local++;        // ✅ OK - local variable
    return local;
}

// Reading fields is OK
[EnforcedPure]
public int Good2()
{
    return _counter; // ✅ OK - reading, not writing
}

// Parameter reassignment is OK (covered by PUR006 for ref types)
[EnforcedPure]
public int Good3(int x)
{
    x = 42;         // ✅ OK - value type parameter
    return x;
}
```

---

## Acceptance Criteria

- [ ] Detects direct field assignment (`_field = x`)
- [ ] Detects increment/decrement (`_field++`, `++_field`)
- [ ] Detects compound assignment (`_field += x`)
- [ ] Detects static field mutation
- [ ] Does NOT flag local variable mutation
- [ ] Does NOT flag reading fields
- [ ] Diagnostic shows correct method and field names
- [ ] All test cases pass

---

## Edge Cases

- Field accessed via `this.` prefix
- Field in base class
- Field in nested class
- Auto-property backing field (via `set`)

---

## Dependencies

- TASK-003 (analyzer skeleton)
- TASK-004 (test infrastructure)

---

## Blocks

None (independent diagnostic)
