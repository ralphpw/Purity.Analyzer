# Purity Rules Reference

This document provides detailed explanations of each diagnostic rule enforced by Purity.Analyzer.

---

## PUR001: Field Mutation

**Severity:** Error  
**Message:** `Method '{0}' marked as pure mutates field '{1}'`

### What It Detects

Assignment to instance or static fields from within a pure method.

### Why It Matters

Pure functions must not modify external state. Field mutation creates side effects that break referential transparency—calling the function twice with the same arguments may produce different results.

### Violations

```csharp
private int _counter;
private static int _globalCount;

[EnforcedPure]
public int Bad()
{
    _counter++;           // ❌ PUR001: mutates instance field
    return _counter;
}

[EnforcedPure]
public void AlsoBad()
{
    _globalCount = 42;    // ❌ PUR001: mutates static field
}
```

### Allowed

```csharp
[EnforcedPure]
public int Good(int x)
{
    int local = x * 2;    // ✅ Local variables may be mutated
    local += 1;
    return local;
}
```

### How to Fix

- Return computed values instead of storing in fields
- Use immutable data structures
- Pass state as parameters and return new state

---

## PUR002: Calls Non-Pure Method

**Severity:** Error  
**Message:** `Method '{0}' marked as pure calls non-pure method '{1}'`

### What It Detects

Invocation of methods that are not known to be pure.

### Why It Matters

Purity is transitive. If a pure method calls an impure method, the entire call chain becomes impure. The analyzer must verify that all called methods are also pure.

### Violations

```csharp
public int Impure() => DateTime.Now.Hour;  // Not marked pure

[EnforcedPure]
public int Bad()
{
    return Impure();      // ❌ PUR002: calls non-pure method
}

[EnforcedPure]
public void AlsoBad()
{
    Console.WriteLine("Hi");  // ❌ PUR002: Console.WriteLine is not pure
}
```

### Allowed

```csharp
[EnforcedPure]
public int Add(int a, int b) => a + b;

[EnforcedPure]
public int Calculate(int x)
{
    return Add(x, 5);     // ✅ Add is marked [EnforcedPure]
}

[EnforcedPure]
public double Compute(double x)
{
    return Math.Sqrt(x);  // ✅ Math.Sqrt is whitelisted
}
```

### How to Fix

- Mark the called method `[EnforcedPure]` (if you own it)
- Use whitelisted BCL methods
- Refactor to avoid calling impure methods

---

## PUR003: I/O Operations

**Severity:** Error  
**Message:** `Method '{0}' marked as pure performs I/O via '{1}'`

### What It Detects

- Console I/O (`Console.WriteLine`, `Console.Read`, etc.)
- File I/O (`File.ReadAllText`, `StreamWriter`, `StreamReader`, etc.)
- Network I/O (`HttpClient`, `WebRequest`, etc.)
- Database access

### Why It Matters

I/O operations are inherently impure—they interact with the outside world and produce different results depending on external state (file contents, network availability, etc.).

### Violations

```csharp
[EnforcedPure]
public string Bad()
{
    return File.ReadAllText("config.txt");  // ❌ PUR003: file I/O
}

[EnforcedPure]
public void AlsoBad()
{
    Console.WriteLine("Hello");  // ❌ PUR003: console I/O
}

[EnforcedPure]
public async Task<string> WorseBad()
{
    var client = new HttpClient();
    return await client.GetStringAsync("https://example.com");  // ❌ PUR003: network I/O
}
```

### Allowed

```csharp
[EnforcedPure]
public string Good(string input)
{
    return input.ToUpper();  // ✅ String manipulation, no I/O
}
```

### How to Fix

- Accept data as parameters instead of reading from external sources
- Move I/O to the boundary of your application
- Use dependency injection to pass pre-loaded data

---

## PUR004: Non-Deterministic APIs

**Severity:** Error  
**Message:** `Method '{0}' marked as pure uses non-deterministic API '{1}'`

### What It Detects

- `DateTime.Now`, `DateTime.UtcNow`
- `DateTimeOffset.Now`, `DateTimeOffset.UtcNow`
- `Guid.NewGuid()`
- `Random` constructor and methods
- `Environment.TickCount`

### Why It Matters

Pure functions must be deterministic—given the same inputs, they must always produce the same output. Non-deterministic APIs return different values each time they're called.

### Violations

```csharp
[EnforcedPure]
public DateTime Bad()
{
    return DateTime.Now;        // ❌ PUR004: non-deterministic
}

[EnforcedPure]
public Guid AlsoBad()
{
    return Guid.NewGuid();      // ❌ PUR004: non-deterministic
}

[EnforcedPure]
public int RandomBad()
{
    var rng = new Random();
    return rng.Next();          // ❌ PUR004: non-deterministic
}
```

### Allowed

```csharp
[EnforcedPure]
public DateTime Good(int year, int month, int day)
{
    return new DateTime(year, month, day);  // ✅ Deterministic constructor
}

[EnforcedPure]
public Guid ParseGuid(string input)
{
    return Guid.Parse(input);   // ✅ Deterministic parsing
}
```

### How to Fix

- Pass time/random values as parameters
- Use dependency injection for clock/random services
- Accept pre-generated values from callers

---

## PUR005: Mutable Return Type

**Severity:** Error  
**Message:** `Method '{0}' marked as pure returns mutable type '{1}'`

### What It Detects

Return types that are mutable collections:
- `List<T>`
- `Dictionary<TKey, TValue>`
- `HashSet<T>`
- `T[]` (arrays)
- Other mutable collection types

### Why It Matters

Returning mutable collections allows callers to modify the returned data, potentially affecting other code that holds references to the same collection. This breaks the expectation that pure functions don't create side effects.

### Violations

```csharp
[EnforcedPure]
public List<int> Bad()
{
    return new List<int> { 1, 2, 3 };  // ❌ PUR005: mutable return type
}

[EnforcedPure]
public int[] AlsoBad()
{
    return new[] { 1, 2, 3 };          // ❌ PUR005: array is mutable
}
```

### Allowed

```csharp
[EnforcedPure]
public ImmutableList<int> Good()
{
    return ImmutableList.Create(1, 2, 3);  // ✅ Immutable collection
}

[EnforcedPure]
public ImmutableArray<int> AlsoGood()
{
    return ImmutableArray.Create(1, 2, 3);  // ✅ Immutable array
}

[EnforcedPure]
public int SimpleGood()
{
    return 42;  // ✅ Value types are inherently immutable
}

[EnforcedPure]
public string StringGood()
{
    return "hello";  // ✅ Strings are immutable
}
```

### How to Fix

- Use `System.Collections.Immutable` types
- Return value types or strings
- Return read-only interfaces (with caution—underlying collection may still be mutable)

---

## PUR006: Parameter Mutation

**Severity:** Error  
**Message:** `Method '{0}' marked as pure mutates parameter '{1}'`

### What It Detects

- Calling mutating methods on parameters (e.g., `list.Add()`)
- Assigning to parameter properties (e.g., `param.Value = x`)
- Modifying parameter fields

### Why It Matters

Pure functions must not modify their inputs. Mutating parameters creates side effects visible to the caller.

### Violations

```csharp
[EnforcedPure]
public void Bad(List<int> list)
{
    list.Add(42);              // ❌ PUR006: mutates parameter
}

[EnforcedPure]
public void AlsoBad(Person person)
{
    person.Name = "Changed";   // ❌ PUR006: mutates parameter property
}
```

### Allowed

```csharp
[EnforcedPure]
public int Good(List<int> list)
{
    return list.Count;         // ✅ Reading is allowed
}

[EnforcedPure]
public string GetName(Person person)
{
    return person.Name;        // ✅ Reading properties is allowed
}
```

### How to Fix

- Return new objects instead of mutating inputs
- Use immutable parameter types
- Create copies before modification (but don't return modified copies from pure methods)

---

## PUR007: ref/out Parameters

**Severity:** Error  
**Message:** `Method '{0}' marked as pure has ref/out parameter '{1}'`

### What It Detects

- `ref` parameters in **user-defined** pure methods
- `out` parameters in **user-defined** pure methods

### Why It Matters

`ref` and `out` parameters are designed for mutation—they allow the method to modify the caller's variable. This is fundamentally incompatible with purity.

### BCL Whitelist Exception

Some BCL methods with `out` parameters (e.g., `Math.BigMul(Int64, Int64, out Int64)`) are whitelisted because:
1. The `out` parameter is the **only** way to return multiple values in older APIs
2. The mutation is **contained**—it only affects the caller's local variable, not shared state
3. The method remains **deterministic**—same inputs always produce same outputs

**User-defined methods should prefer tuples:** `(long high, long low) BigMul(long a, long b)`. The analyzer enforces this for your code while allowing legacy BCL patterns.

### Violations

```csharp
[EnforcedPure]
public void Bad(ref int x)    // ❌ PUR007: ref parameter
{
    x = 42;
}

[EnforcedPure]
public bool AlsoBad(out int result)  // ❌ PUR007: out parameter
{
    result = 123;
    return true;
}
```

### Allowed

```csharp
[EnforcedPure]
public int Good(in int x)     // ✅ 'in' is read-only
{
    return x * 2;
}

[EnforcedPure]
public (bool success, int value) TryParse(string input)  // ✅ Return tuple instead
{
    if (int.TryParse(input, out var result))
        return (true, result);
    return (false, 0);
}
```

### How to Fix

- Use `in` for read-only reference parameters
- Return tuples or custom result types instead of `out` parameters
- Return nullable types for "try" patterns

---

## PUR008: Unsafe Code

**Severity:** Error  
**Message:** `Method '{0}' marked as pure uses unsafe code`

### What It Detects

- `unsafe` blocks
- Pointer declarations and operations
- `fixed` statements

### Why It Matters

Unsafe code can bypass type safety and perform arbitrary memory manipulation. This makes it impossible to verify purity statically.

### Violations

```csharp
[EnforcedPure]
public unsafe int Bad(int* ptr)  // ❌ PUR008: unsafe parameter
{
    return *ptr;
}

[EnforcedPure]
public int AlsoBad()
{
    unsafe                       // ❌ PUR008: unsafe block
    {
        int x = 42;
        int* p = &x;
        return *p;
    }
}
```

### Allowed

```csharp
[EnforcedPure]
public int Good(int x)
{
    return x * 2;  // ✅ Safe code only
}
```

### How to Fix

- Use safe alternatives (Span<T>, Memory<T>)
- Move unsafe code to separate non-pure methods
- Reconsider if the algorithm truly requires unsafe code

---

## PUR009: Reflection

**Severity:** Error  
**Message:** `Method '{0}' marked as pure uses reflection`

### What It Detects

- `Type.GetType()`
- `Assembly.Load()`, `Assembly.LoadFrom()`
- `MethodInfo.Invoke()`
- `Activator.CreateInstance()`
- Other reflection APIs that can perform side effects

### Why It Matters

Reflection can bypass compile-time checks and perform arbitrary operations including I/O, mutation, and object creation. Its dynamic nature makes static purity verification impossible.

### Violations

```csharp
[EnforcedPure]
public object Bad(string typeName)
{
    var type = Type.GetType(typeName);   // ❌ PUR009: reflection
    return Activator.CreateInstance(type);
}

[EnforcedPure]
public void AlsoBad(object obj, string methodName)
{
    var method = obj.GetType().GetMethod(methodName);
    method.Invoke(obj, null);            // ❌ PUR009: reflection
}
```

### Allowed

```csharp
[EnforcedPure]
public string GetTypeName<T>()
{
    return typeof(T).Name;  // ✅ typeof() is compile-time, not reflection
}
```

### How to Fix

- Use generics instead of reflection
- Use compile-time type information (`typeof()`)
- Move reflection to separate non-pure methods

---

## PUR010: Exception Control Flow

**Severity:** Warning  
**Message:** `Method '{0}' marked as pure may throw exception '{1}' for control flow`

### What It Detects

Use of exceptions for expected control flow rather than exceptional conditions.

### Why It Matters

While throwing exceptions for truly exceptional conditions (e.g., `ArgumentNullException`) is acceptable, using exceptions for normal control flow (e.g., parsing failures) breaks referential transparency and makes code harder to reason about.

### Flagged (Warning)

```csharp
[EnforcedPure]
public int Parse(string input)
{
    return int.Parse(input);  // ⚠️ PUR010: may throw FormatException
}
```

### Preferred Pattern

```csharp
[EnforcedPure]
public Result<int, string> ParseSafe(string input)
{
    if (int.TryParse(input, out var result))
        return Result.Ok(result);
    return Result.Error($"Cannot parse '{input}' as integer");
}
```

### Note

PUR010 is a **warning**, not an error. Throwing exceptions for unrecoverable errors (e.g., `ArgumentNullException` for null guards) is still acceptable in pure methods.

### How to Fix

- Use `TryParse` patterns
- Return `Result<T, E>` or `Option<T>` types
- Reserve exceptions for truly exceptional conditions

---

## PUR011: Review Required

**Severity:** Info  
**Message:** `Method '{0}' calls '{1}' which is marked for review`

### What It Detects

Calls to methods in the user's `reviewRequired` list in `.purity/config.json`.

### Why It Matters

In **zero-trust mode** or during gradual adoption, you may want to whitelist methods to unblock the build while flagging them for future review. PUR011 provides visibility into these deferred reviews.

### Configuration

```json
{
  "whitelist": {
    "include": ["ThirdParty.Lib.Calculate"]
  },
  "reviewRequired": ["ThirdParty.Lib.Calculate"]
}
```

### Example

```csharp
[EnforcedPure]
public int Calculate(int x)
{
    return ThirdParty.Lib.Calculate(x);  // ℹ️ PUR011: marked for review
}
```

### How to Address

1. Review the method's source code for purity
2. If pure: remove from `reviewRequired`, keep in `whitelist.include`
3. If impure: remove from both lists, refactor code

See [Whitelist.md](Whitelist.md#review-required-methods) for the LLM-assisted review workflow.

---

## Summary Table

| Rule | Severity | Category | Quick Description |
|------|----------|----------|-------------------|
| PUR001 | Error | Mutation | No field mutation |
| PUR002 | Error | Calls | Only call pure methods |
| PUR003 | Error | I/O | No I/O operations |
| PUR004 | Error | Determinism | No non-deterministic APIs |
| PUR005 | Error | Return | No mutable return types |
| PUR006 | Error | Mutation | No parameter mutation |
| PUR007 | Error | Parameters | No ref/out parameters |
| PUR008 | Error | Safety | No unsafe code |
| PUR009 | Error | Reflection | No reflection |
| PUR010 | Warning | Control Flow | Avoid exception control flow |
| PUR011 | Info | Review | Method pending purity review |
