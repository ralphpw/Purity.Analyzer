# TASK-006: Implement PUR002 (Calls Non-Pure Method) - Basic

**Priority:** P0  
**Estimate:** 6 hours  
**Status:** Not Started  
**Depends On:** TASK-003, TASK-004

---

## Description

Detect method invocations within `[EnforcedPure]` methods that call non-pure methods, emitting PUR002 error.

This is the **basic** implementation for Phase 1. Full transitive call-graph analysis is deferred to Phase 2.

---

## Diagnostic

| Property | Value |
|----------|-------|
| ID | PUR002 |
| Severity | Error |
| Message | Method '{0}' marked as pure calls non-pure method '{1}' |

---

## Implementation

### Phase 1 Scope

A called method is considered pure if:
1. Marked with `[EnforcedPure]` attribute
2. Marked with `[Pure]` attribute
3. In the hardcoded BCL whitelist (initial set of ~20 methods)

### Detection Logic

1. For each method marked as pure:
2. Find all `InvocationExpressionSyntax` nodes
3. Resolve the called method symbol
4. Check if the method is pure (attribute or whitelist)
5. If not pure, report PUR002

### Code Sketch

```csharp
private void CheckForImpureCalls(
    SyntaxNodeAnalysisContext context,
    MethodDeclarationSyntax method,
    IMethodSymbol methodSymbol)
{
    foreach (var invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>())
    {
        var calledSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        
        if (calledSymbol is null)
            continue;
        
        if (IsMethodPure(calledSymbol))
            continue;
        
        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.PUR002,
            invocation.GetLocation(),
            methodSymbol.Name,
            calledSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat));
        context.ReportDiagnostic(diagnostic);
    }
}

private bool IsMethodPure(IMethodSymbol method)
{
    // Check for purity attributes
    if (HasPurityAttribute(method))
        return true;
    
    // Check BCL whitelist
    var fullName = method.ContainingType?.ToDisplayString() + "." + method.Name;
    if (BclWhitelist.Contains(fullName))
        return true;
    
    return false;
}
```

### Initial BCL Whitelist

```csharp
private static readonly HashSet<string> BclWhitelist = new()
{
    // Math
    "System.Math.Abs",
    "System.Math.Sqrt",
    "System.Math.Pow",
    "System.Math.Min",
    "System.Math.Max",
    "System.Math.Round",
    "System.Math.Floor",
    "System.Math.Ceiling",
    
    // String
    "System.String.Concat",
    "System.String.Join",
    "System.String.IsNullOrEmpty",
    "System.String.IsNullOrWhiteSpace",
    "System.String.Substring",
    "System.String.ToLower",
    "System.String.ToUpper",
    "System.String.Trim",
    "System.String.Contains",
    "System.String.StartsWith",
    "System.String.EndsWith",
};
```

---

## Test Cases

### Should Report PUR002

```csharp
// Unmarked method
public int Helper() => 42;

[EnforcedPure]
public int Bad()
{
    return Helper();  // ❌ PUR002: Helper is not marked pure
}

// Console.WriteLine
[EnforcedPure]
public void Bad2()
{
    Console.WriteLine("Hi");  // ❌ PUR002: not in whitelist
}

// DateTime.Now (also PUR004, but PUR002 applies)
[EnforcedPure]
public DateTime Bad3()
{
    return DateTime.Now;  // ❌ PUR002: not in whitelist
}
```

### Should NOT Report

```csharp
// Call to [EnforcedPure] method
[EnforcedPure]
public int Add(int a, int b) => a + b;

[EnforcedPure]
public int Good(int x)
{
    return Add(x, 5);  // ✅ OK: Add is [EnforcedPure]
}

// Call to [Pure] method
[Pure]
public int Multiply(int a, int b) => a * b;

[EnforcedPure]
public int Good2(int x)
{
    return Multiply(x, 2);  // ✅ OK: Multiply is [Pure]
}

// Call to whitelisted BCL method
[EnforcedPure]
public double Good3(double x)
{
    return Math.Sqrt(x);  // ✅ OK: Math.Sqrt is whitelisted
}

// String methods
[EnforcedPure]
public string Good4(string s)
{
    return s.ToUpper().Trim();  // ✅ OK: both are whitelisted
}
```

---

## Acceptance Criteria

- [ ] Detects calls to unmarked methods
- [ ] Allows calls to `[EnforcedPure]` methods
- [ ] Allows calls to `[Pure]` methods
- [ ] Allows calls to whitelisted BCL methods
- [ ] Diagnostic shows correct method names
- [ ] All test cases pass

---

## Edge Cases

- Overloaded methods (verify correct overload is checked)
- Extension methods
- Generic methods
- Operators (should these be checked?)
- Property getters (treated as method calls)
- Delegate invocations
- Lambda expressions (defer to Phase 2)

---

## Phase 2 Enhancements

- Full transitive call-graph analysis
- Verify `[Pure]` methods are actually pure
- Expanded BCL whitelist
- User-configurable whitelist

---

## Dependencies

- TASK-003 (analyzer skeleton)
- TASK-004 (test infrastructure)

---

## Blocks

None (independent diagnostic, though whitelist will be expanded later)
