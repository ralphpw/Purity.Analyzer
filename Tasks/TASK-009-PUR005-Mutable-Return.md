# TASK-009: Implement PUR005 (Mutable Return Types)

**Priority:** P0  
**Estimate:** 4 hours  
**Status:** Not Started  
**Depends On:** TASK-003, TASK-004

---

## Description

Detect methods returning mutable collection types within `[EnforcedPure]` methods and emit PUR005 error.

---

## Diagnostic

| Property | Value |
|----------|-------|
| ID | PUR005 |
| Severity | Error |
| Message | Method '{0}' marked as pure returns mutable type '{1}'. Consider using '{2}' instead. |

---

## Implementation

### Mutable Types to Detect

**Collections:**
- `System.Collections.Generic.List<T>`
- `System.Collections.Generic.Dictionary<TKey, TValue>`
- `System.Collections.Generic.HashSet<T>`
- `System.Collections.Generic.Queue<T>`
- `System.Collections.Generic.Stack<T>`
- `System.Collections.Generic.LinkedList<T>`
- `System.Collections.Generic.SortedList<TKey, TValue>`
- `System.Collections.Generic.SortedDictionary<TKey, TValue>`
- `System.Collections.Generic.SortedSet<T>`

**Arrays:**
- `T[]` (any array type)

**Non-Generic:**
- `System.Collections.ArrayList`
- `System.Collections.Hashtable`

### Immutable Alternatives (for message)

| Mutable Type | Suggested Alternative |
|--------------|----------------------|
| `List<T>` | `ImmutableList<T>` |
| `T[]` | `ImmutableArray<T>` |
| `Dictionary<K,V>` | `ImmutableDictionary<K,V>` |
| `HashSet<T>` | `ImmutableHashSet<T>` |
| `Queue<T>` | `ImmutableQueue<T>` |
| `Stack<T>` | `ImmutableStack<T>` |
| `SortedSet<T>` | `ImmutableSortedSet<T>` |
| `SortedDictionary<K,V>` | `ImmutableSortedDictionary<K,V>` |

### Detection Strategy

Check the return type of the method declaration:

```csharp
private static readonly Dictionary<string, string> MutableToImmutable = new()
{
    ["System.Collections.Generic.List"] = "ImmutableList",
    ["System.Collections.Generic.Dictionary"] = "ImmutableDictionary",
    ["System.Collections.Generic.HashSet"] = "ImmutableHashSet",
    ["System.Collections.Generic.Queue"] = "ImmutableQueue",
    ["System.Collections.Generic.Stack"] = "ImmutableStack",
    ["System.Collections.Generic.SortedSet"] = "ImmutableSortedSet",
    ["System.Collections.Generic.SortedDictionary"] = "ImmutableSortedDictionary",
    ["System.Collections.Generic.LinkedList"] = "ImmutableList",
    ["System.Collections.Generic.SortedList"] = "ImmutableSortedDictionary",
    ["System.Collections.ArrayList"] = "ImmutableList",
    ["System.Collections.Hashtable"] = "ImmutableDictionary",
};

private void CheckReturnType(
    SyntaxNodeAnalysisContext context,
    MethodDeclarationSyntax method,
    IMethodSymbol methodSymbol)
{
    var returnType = methodSymbol.ReturnType;
    
    if (returnType is IArrayTypeSymbol)
    {
        ReportPUR005(context, method.ReturnType.GetLocation(),
            methodSymbol.Name, returnType.ToDisplayString(), "ImmutableArray<T>");
        return;
    }
    
    if (returnType is INamedTypeSymbol namedType)
    {
        var originalDef = namedType.OriginalDefinition.ToDisplayString();
        
        // Remove generic arity suffix for lookup
        var lookupKey = originalDef.Split('<')[0];
        
        if (MutableToImmutable.TryGetValue(lookupKey, out var suggestion))
        {
            ReportPUR005(context, method.ReturnType.GetLocation(),
                methodSymbol.Name, returnType.ToDisplayString(), suggestion + "<T>");
        }
    }
}
```

---

## Test Cases

### Should Report PUR005

```csharp
// List<T>
[EnforcedPure]
public List<int> Bad()
{
    return new List<int> { 1, 2, 3 };  // ❌ PUR005
}

// Array
[EnforcedPure]
public int[] Bad2()
{
    return new[] { 1, 2, 3 };  // ❌ PUR005
}

// Dictionary
[EnforcedPure]
public Dictionary<string, int> Bad3()
{
    return new Dictionary<string, int>();  // ❌ PUR005
}

// HashSet
[EnforcedPure]
public HashSet<int> Bad4()
{
    return new HashSet<int>();  // ❌ PUR005
}

// Multidimensional array
[EnforcedPure]
public int[,] Bad5()
{
    return new int[2, 2];  // ❌ PUR005
}
```

### Should NOT Report

```csharp
// ImmutableList<T>
[EnforcedPure]
public ImmutableList<int> Good()
{
    return ImmutableList.Create(1, 2, 3);  // ✅ OK
}

// ImmutableArray<T>
[EnforcedPure]
public ImmutableArray<int> Good2()
{
    return ImmutableArray.Create(1, 2, 3);  // ✅ OK
}

// Value types
[EnforcedPure]
public int Good3()
{
    return 42;  // ✅ OK
}

// String (immutable)
[EnforcedPure]
public string Good4()
{
    return "hello";  // ✅ OK
}

// Structs (value semantics)
[EnforcedPure]
public DateTime Good5()
{
    return new DateTime(2024, 1, 1);  // ✅ OK
}

// IEnumerable<T> (interface, not necessarily mutable)
[EnforcedPure]
public IEnumerable<int> Good6()
{
    return ImmutableList.Create(1, 2, 3);  // ✅ OK
}
```

---

## Acceptance Criteria

- [ ] Detects `List<T>` return type
- [ ] Detects array return types (`T[]`, `T[,]`, etc.)
- [ ] Detects `Dictionary<K,V>` return type
- [ ] Detects `HashSet<T>` return type
- [ ] Detects other mutable collection types
- [ ] Does NOT flag immutable collections
- [ ] Does NOT flag value types
- [ ] Does NOT flag strings
- [ ] Diagnostic suggests immutable alternative
- [ ] All test cases pass

---

## Edge Cases

- `IList<T>` interface (tricky—could wrap immutable)
- `ICollection<T>` interface
- Custom mutable types (out of scope for Phase 1)
- Generic method returning `T` where `T` could be mutable

---

## Design Decisions

**Why not check `IList<T>`, `ICollection<T>`, etc.?**

These interfaces can wrap immutable implementations. Flagging them would cause false positives:
```csharp
public IList<int> Valid() => ImmutableList.Create(1, 2, 3); // ImmutableList implements IList
```

We only flag concrete mutable types.

---

## Dependencies

- TASK-003 (analyzer skeleton)
- TASK-004 (test infrastructure)

---

## Blocks

None (independent diagnostic)
