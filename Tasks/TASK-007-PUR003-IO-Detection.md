# TASK-007: Implement PUR003 (I/O Detection)

**Priority:** P0  
**Estimate:** 3 hours  
**Status:** Not Started  
**Depends On:** TASK-003, TASK-004

---

## Description

Detect I/O operations within `[EnforcedPure]` methods and emit PUR003 error.

---

## Diagnostic

| Property | Value |
|----------|-------|
| ID | PUR003 |
| Severity | Error |
| Message | Method '{0}' marked as pure performs I/O via '{1}' |

---

## Implementation

### I/O APIs to Detect

**Console:**
- `System.Console.*` (all members)

**File System:**
- `System.IO.File.*`
- `System.IO.Directory.*`
- `System.IO.Path.*` (some methods are pure, but exclude for simplicity)
- `System.IO.StreamReader` (constructor and methods)
- `System.IO.StreamWriter` (constructor and methods)
- `System.IO.FileStream` (constructor and methods)

**Network:**
- `System.Net.Http.HttpClient.*`
- `System.Net.WebRequest.*`
- `System.Net.WebClient.*`

**Database (common patterns):**
- `*.ExecuteReader`
- `*.ExecuteNonQuery`
- `*.ExecuteScalar`

### Detection Strategy

Create a set of I/O type prefixes and check if called methods belong to these types:

```csharp
private static readonly HashSet<string> IoTypePatterns = new()
{
    "System.Console",
    "System.IO.File",
    "System.IO.Directory",
    "System.IO.StreamReader",
    "System.IO.StreamWriter",
    "System.IO.FileStream",
    "System.IO.BinaryReader",
    "System.IO.BinaryWriter",
    "System.Net.Http.HttpClient",
    "System.Net.WebRequest",
    "System.Net.WebClient",
};

private bool IsIoOperation(IMethodSymbol method)
{
    var containingType = method.ContainingType?.ToDisplayString();
    if (containingType is null)
        return false;
    
    return IoTypePatterns.Any(pattern => containingType.StartsWith(pattern));
}
```

### Code Sketch

```csharp
private void CheckForIoOperations(
    SyntaxNodeAnalysisContext context,
    MethodDeclarationSyntax method,
    IMethodSymbol methodSymbol)
{
    foreach (var invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>())
    {
        var calledSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        
        if (calledSymbol is null)
            continue;
        
        if (!IsIoOperation(calledSymbol))
            continue;
        
        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.PUR003,
            invocation.GetLocation(),
            methodSymbol.Name,
            calledSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat));
        context.ReportDiagnostic(diagnostic);
    }
}
```

---

## Test Cases

### Should Report PUR003

```csharp
// Console output
[EnforcedPure]
public void Bad()
{
    Console.WriteLine("Hello");  // ❌ PUR003
}

// Console input
[EnforcedPure]
public string Bad2()
{
    return Console.ReadLine();   // ❌ PUR003
}

// File read
[EnforcedPure]
public string Bad3()
{
    return File.ReadAllText("path.txt");  // ❌ PUR003
}

// File write
[EnforcedPure]
public void Bad4()
{
    File.WriteAllText("path.txt", "content");  // ❌ PUR003
}

// StreamReader
[EnforcedPure]
public string Bad5()
{
    using var reader = new StreamReader("file.txt");  // ❌ PUR003
    return reader.ReadToEnd();
}

// HttpClient
[EnforcedPure]
public async Task<string> Bad6()
{
    var client = new HttpClient();
    return await client.GetStringAsync("https://example.com");  // ❌ PUR003
}
```

### Should NOT Report

```csharp
// Pure computation
[EnforcedPure]
public int Good(int x)
{
    return x * 2;  // ✅ OK
}

// String operations (not I/O)
[EnforcedPure]
public string Good2(string s)
{
    return s.ToUpper();  // ✅ OK
}

// Math operations
[EnforcedPure]
public double Good3(double x)
{
    return Math.Sqrt(x);  // ✅ OK
}
```

---

## Acceptance Criteria

- [ ] Detects `Console.*` calls
- [ ] Detects `File.*` calls
- [ ] Detects `Directory.*` calls
- [ ] Detects `StreamReader`/`StreamWriter` usage
- [ ] Detects `HttpClient` calls
- [ ] Diagnostic shows I/O operation name
- [ ] All test cases pass

---

## Edge Cases

- `Path.Combine` and `Path.GetExtension` are actually pure—consider whitelist
- Async I/O operations
- I/O in lambda expressions (defer to Phase 2)

---

## Note on PUR002 vs PUR003

PUR003 is a more specific diagnostic than PUR002. When I/O is detected:
- Report PUR003 (specific: "performs I/O")
- Do NOT also report PUR002 (generic: "calls non-pure")

This provides more actionable error messages.

---

## Dependencies

- TASK-003 (analyzer skeleton)
- TASK-004 (test infrastructure)

---

## Blocks

None (independent diagnostic)
