using Purity.Analyzer.Tests.Verifiers;

namespace Purity.Analyzer.Tests;

/// <summary>
/// Smoke tests to verify the analyzer infrastructure works correctly.
/// These tests confirm the analyzer loads and detects purity attributes.
/// </summary>
public class SmokeTests
{
    [Fact]
    public async Task Analyzer_LoadsSuccessfully_NoDiagnosticsForPureMethod()
    {
        var code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Add(int a, int b) => a + b;
            }
            """;

        // No diagnostics expected for valid pure code
        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task Analyzer_DetectsEnforcedPureAttribute_OnMethod()
    {
        var code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Method() => 42;
            }
            """;

        // Analyzer should load and process without crashing
        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task Analyzer_DetectsBclPureAttribute_OnMethod()
    {
        var code = """
            using System.Diagnostics.Contracts;

            public class Test
            {
                [Pure]
                public int Method() => 42;
            }
            """;

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task Analyzer_DetectsEnforcedPureAttribute_OnClass()
    {
        var code = """
            using Purity.Contracts;

            [EnforcedPure]
            public class Test
            {
                public int Method() => 42;
            }
            """;

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task Analyzer_DetectsBclPureAttribute_OnClass()
    {
        var code = """
            using System.Diagnostics.Contracts;

            [Pure]
            public class Calculator
            {
                public int Add(int a, int b) => a + b;
                public int Multiply(int a, int b) => a * b;
            }
            """;

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task Analyzer_IgnoresMethodsWithoutPurityAttribute()
    {
        var code = """
            public class Test
            {
                private int _counter;

                // This method mutates state but has no purity attribute,
                // so analyzer should not report anything
                public void Increment() => _counter++;
            }
            """;

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task Analyzer_DetectsEnforcedPureAttribute_OnStruct()
    {
        var code = """
            using Purity.Contracts;

            [EnforcedPure]
            public struct Point
            {
                public int X { get; }
                public int Y { get; }

                public Point(int x, int y)
                {
                    X = x;
                    Y = y;
                }

                public int DistanceSquared() => X * X + Y * Y;
            }
            """;

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }
}
