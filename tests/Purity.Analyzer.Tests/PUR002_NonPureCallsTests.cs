using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Purity.Analyzer.Tests.Verifiers.CSharpAnalyzerVerifier<Purity.Analyzer.PurityAnalyzer>;

namespace Purity.Analyzer.Tests;

/// <summary>
/// Tests for PUR002: Calls to non-pure methods within pure methods.
/// </summary>
public sealed class PUR002_NonPureCallsTests
{
    #region Should Report PUR002

    [Fact]
    public async Task CallToUnmarkedMethod_ReportsDiagnostic()
    {
        const string source = """
            using Purity.Contracts;

            public class Test
            {
                public int Helper() => 42;

                [EnforcedPure]
                public int Bad()
                {
                    return {|#0:Helper()|};
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(
            source,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithLocation(0)
                .WithArguments("Bad", "Test.Helper()"));
    }

    [Fact]
    public async Task CallToUnmarkedStaticMethod_ReportsDiagnostic()
    {
        const string source = """
            using Purity.Contracts;

            public static class Helpers
            {
                public static int Calculate() => 42;
            }

            public class Test
            {
                [EnforcedPure]
                public int Bad()
                {
                    return {|#0:Helpers.Calculate()|};
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(
            source,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithLocation(0)
                .WithArguments("Bad", "Helpers.Calculate()"));
    }

    [Fact]
    public async Task CallToUnmarkedMethodWithParameters_ReportsDiagnostic()
    {
        const string source = """
            using Purity.Contracts;

            public class Test
            {
                public int Add(int a, int b) => a + b;

                [EnforcedPure]
                public int Bad(int x, int y)
                {
                    return {|#0:Add(x, y)|};
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(
            source,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithLocation(0)
                .WithArguments("Bad", "Test.Add(int, int)"));
    }

    [Fact]
    public async Task MultipleNonPureCalls_ReportsMultipleDiagnostics()
    {
        const string source = """
            using Purity.Contracts;

            public class Test
            {
                public int Helper1() => 1;
                public int Helper2() => 2;

                [EnforcedPure]
                public int Bad()
                {
                    var a = {|#0:Helper1()|};
                    var b = {|#1:Helper2()|};
                    return a + b;
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(
            source,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithLocation(0)
                .WithArguments("Bad", "Test.Helper1()"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithLocation(1)
                .WithArguments("Bad", "Test.Helper2()"));
    }

    [Fact]
    public async Task CallToUnmarkedMethodInExternalClass_ReportsDiagnostic()
    {
        const string source = """
            using Purity.Contracts;

            public class External
            {
                public int GetValue() => 42;
            }

            public class Test
            {
                [EnforcedPure]
                public int Bad()
                {
                    var ext = new External();
                    return {|#0:ext.GetValue()|};
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(
            source,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithLocation(0)
                .WithArguments("Bad", "External.GetValue()"));
    }

    [Fact]
    public async Task ChainedNonPureCalls_ReportsMultipleDiagnostics()
    {
        const string source = """
            using Purity.Contracts;

            public class Builder
            {
                public Builder Step1() => this;
                public Builder Step2() => this;
                public int Build() => 42;
            }

            public class Test
            {
                [EnforcedPure]
                public int Bad()
                {
                    var b = new Builder();
                    return {|#0:{|#1:{|#2:b.Step1()|}.Step2()|}.Build()|};
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(
            source,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithLocation(0)
                .WithArguments("Bad", "Builder.Build()"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithLocation(1)
                .WithArguments("Bad", "Builder.Step2()"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithLocation(2)
                .WithArguments("Bad", "Builder.Step1()"));
    }

    #endregion

    #region Should NOT Report (Purity Attributes)

    [Fact]
    public async Task CallToEnforcedPureMethod_NoDiagnostic()
    {
        const string source = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Add(int a, int b) => a + b;

                [EnforcedPure]
                public int Good(int x)
                {
                    return Add(x, 5);
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task CallToBclPureMethod_NoDiagnostic()
    {
        const string source = """
            using System.Diagnostics.Contracts;
            using Purity.Contracts;

            public class Test
            {
                [Pure]
                public int Multiply(int a, int b) => a * b;

                [EnforcedPure]
                public int Good(int x)
                {
                    return Multiply(x, 2);
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task CallToMethodOnPureClass_NoDiagnostic()
    {
        const string source = """
            using Purity.Contracts;

            [EnforcedPure]
            public class PureCalculator
            {
                public int Add(int a, int b) => a + b;
                public int Subtract(int a, int b) => a - b;
            }

            public class Test
            {
                [EnforcedPure]
                public int Good(int x)
                {
                    var calc = new PureCalculator();
                    return calc.Add(x, calc.Subtract(10, 3));
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    #endregion

    #region Should NOT Report (BCL Whitelist)

    [Fact]
    public async Task CallToMathAbs_NoDiagnostic()
    {
        const string source = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Good(int x)
                {
                    return Math.Abs(x);
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task CallToMathSqrt_NoDiagnostic()
    {
        const string source = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public double Good(double x)
                {
                    return Math.Sqrt(x);
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task CallToMathMaxMin_NoDiagnostic()
    {
        const string source = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Good(int a, int b)
                {
                    return Math.Max(a, Math.Min(b, 100));
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task CallToStringIsNullOrEmpty_NoDiagnostic()
    {
        const string source = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public bool Good(string s)
                {
                    return string.IsNullOrEmpty(s);
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task CallToStringInstanceMethods_NoDiagnostic()
    {
        const string source = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public string Good(string s)
                {
                    return s.ToUpper().Trim();
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task CallToStringContains_NoDiagnostic()
    {
        const string source = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public bool Good(string s)
                {
                    return s.Contains("test");
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task CallToStringSubstring_NoDiagnostic()
    {
        const string source = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public string Good(string s)
                {
                    return s.Substring(0, 5);
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task CallInExpressionBody_ReportsDiagnostic()
    {
        const string source = """
            using Purity.Contracts;

            public class Test
            {
                public int Helper() => 42;

                [EnforcedPure]
                public int Bad() => {|#0:Helper()|};
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(
            source,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithLocation(0)
                .WithArguments("Bad", "Test.Helper()"));
    }

    [Fact]
    public async Task CallInLambdaParameter_LinqExtensionMethods()
    {
        // LINQ extension methods require special handling since they're defined on System.Linq.Enumerable
        // but are called on IEnumerable<T>. This test documents the current behavior.
        // Future enhancement: resolve extension method to its defining type for whitelist lookup.
        const string source = """
            using System.Linq;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int NotYetSupported(int[] numbers)
                {
                    return {|#0:{|#1:numbers.Where(x => x > 0)|}.Count()|};
                }
            }
            """;

        // Currently reports PUR002 because extension method lookup isn't implemented yet
        await VerifyCS.VerifyAnalyzerAsync(
            source,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithLocation(0)
                .WithArguments("NotYetSupported", "IEnumerable<int>.Count<int>()"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithLocation(1)
                .WithArguments("NotYetSupported", "IEnumerable<int>.Where<int>(Func<int, bool>)"));
    }

    [Fact]
    public async Task CallToOverloadedMethod_ChecksCorrectOverload()
    {
        const string source = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public double Good(double x)
                {
                    // Math.Abs(double) is whitelisted
                    return Math.Abs(x);
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task UnmarkedMethodDoesNotTriggerAnalysis()
    {
        const string source = """
            using System;

            public class Test
            {
                public int Helper() => 42;

                // Not marked as pure - should not be analyzed
                public int Unmarked()
                {
                    return Helper() + Console.Read();
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task RecursiveCallToSamePureMethod_NoDiagnostic()
    {
        const string source = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Factorial(int n)
                {
                    return n <= 1 ? 1 : n * Factorial(n - 1);
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task MutuallyRecursivePureMethods_NoDiagnostic()
    {
        const string source = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public bool IsEven(int n)
                {
                    return n == 0 || IsOdd(n - 1);
                }

                [EnforcedPure]
                public bool IsOdd(int n)
                {
                    return n != 0 && IsEven(n - 1);
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    #endregion
}
