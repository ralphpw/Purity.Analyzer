using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Purity.Contracts;

namespace BasicUsage;

/// <summary>
/// Examples that VIOLATE purity rules, with fixes shown in comments.
/// This file is excluded from compilation by default (see BasicUsage.csproj).
/// To see the violations, remove the Compile Remove in the .csproj and rebuild.
/// </summary>
public class ViolationDemos
{
    private int _counter;

    // ❌ PUR001: Field mutation
    [EnforcedPure]
    public int IncrementCounter()
    {
        _counter++;  // Error: mutates field
        return _counter;
    }
    // ✅ FIX: Return computed value instead of mutating field
    // [EnforcedPure]
    // public int IncrementCounter(int currentValue) => currentValue + 1;

    // ❌ PUR003: I/O operations
    [EnforcedPure]
    public void LogMessage(string message)
    {
        Console.WriteLine(message);  // Error: performs I/O
    }
    // ✅ FIX: Return the message, let caller handle I/O
    // [EnforcedPure]
    // public string FormatMessage(string message) => $"[LOG] {message}";

    // ❌ PUR004: Non-deterministic API
    [EnforcedPure]
    public DateTime GetCurrentTime()
    {
        return DateTime.Now;  // Error: non-deterministic
    }
    // ✅ FIX: Accept time as parameter (dependency injection pattern)
    // [EnforcedPure]
    // public string FormatTime(DateTime time) => time.ToString("HH:mm:ss");

    // ❌ PUR005: Mutable return type
    [EnforcedPure]
    public List<int> CreateList()
    {
        return new List<int> { 1, 2, 3 };  // Error: List<T> is mutable
    }
    // ✅ FIX: Return immutable collection
    // [EnforcedPure]
    // public ImmutableArray<int> CreateList() => ImmutableArray.Create(1, 2, 3);

    // ❌ PUR006: Parameter mutation
    [EnforcedPure]
    public void ModifyArray(int[] numbers)
    {
        numbers[0] = 42;  // Error: mutates parameter
    }
    // ✅ FIX: Return new array instead of mutating
    // [EnforcedPure]
    // public int[] SetFirstElement(int[] numbers, int value)
    // {
    //     var result = new int[numbers.Length];
    //     Array.Copy(numbers, result, numbers.Length);
    //     result[0] = value;
    //     return result;
    // }

    // ❌ PUR007: ref/out parameters
    [EnforcedPure]
    public bool TryParseNumber(string input, out int result)
    {
        return int.TryParse(input, out result);  // Error: has 'out' parameter
    }
    // ✅ FIX: Return tuple or custom result type
    // [EnforcedPure]
    // public (bool success, int value) TryParseNumber(string input)
    // {
    //     if (int.TryParse(input, out var value))
    //         return (true, value);
    //     return (false, 0);
    // }

    // ❌ PUR009: Reflection
    [EnforcedPure]
    public object CreateInstance(string typeName)
    {
        var type = Type.GetType(typeName);  // Error: uses reflection
        return Activator.CreateInstance(type!);
    }
    // ✅ FIX: Use generics or factory pattern
    // [EnforcedPure]
    // public T CreateInstance<T>() where T : new() => new T();

    // ⚠️ PUR010: Exception control flow (Warning, not Error)
    [EnforcedPure]
    public int ParseNumber(string input)
    {
        return int.Parse(input);  // Warning: may throw for control flow
    }
    // ✅ FIX: Use TryParse pattern
    // [EnforcedPure]
    // public int? ParseNumber(string input)
    // {
    //     return int.TryParse(input, out var result) ? result : null;
    // }
}
