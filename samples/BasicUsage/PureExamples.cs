using System;
using System.Linq;
using Purity.Contracts;

namespace BasicUsage;

/// <summary>
/// Examples of pure calculations that compile successfully.
/// </summary>
public static class PureExamples
{
    [EnforcedPure]
    public static int Add(int a, int b) => a + b;

    [EnforcedPure]
    public static int Multiply(int a, int b) => a * b;

    [EnforcedPure]
    public static double CalculateCircleArea(double radius)
    {
        return Math.PI * radius * radius;
    }

    [EnforcedPure]
    public static string FormatName(string first, string last)
    {
        return $"{last}, {first}".ToUpper();
    }

    [EnforcedPure]
    public static string CombineStrings(string a, string b, string separator)
    {
        // String concatenation is pure
        return a + separator + b;
    }

    [EnforcedPure]
    public static int CountMatches(string[] items, string prefix)
    {
        // Manual loop is pure (no mutation)
        int count = 0;
        foreach (var item in items)
        {
            if (item.StartsWith(prefix))
                count++;
        }
        return count;
    }

    [EnforcedPure]
    public static int Factorial(int n)
    {
        // Pure recursion
        return n <= 1 ? 1 : n * Factorial(n - 1);
    }
}
