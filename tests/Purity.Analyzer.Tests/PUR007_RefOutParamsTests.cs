using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Purity.Analyzer.Tests.Verifiers.CSharpAnalyzerVerifier<Purity.Analyzer.PurityAnalyzer>;

namespace Purity.Analyzer.Tests;

/// <summary>
/// Tests for PUR007: Pure method has ref/out parameter.
/// </summary>
public sealed class PUR007_RefOutParamsTests
{
    [Fact]
    public async Task RefParameter_ReportsDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad(ref int x) => x = 42;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR007)
                .WithSpan(6, 29, 6, 30)
                .WithArguments("Bad", "ref", "x"));
    }

    [Fact]
    public async Task OutParameter_ReportsDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public bool TryParse(out int result)
                {
                    result = 0;
                    return true;
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR007)
                .WithSpan(6, 34, 6, 40)
                .WithArguments("TryParse", "out", "result"));
    }

    [Fact]
    public async Task MultipleRefOutParameters_ReportsMultipleDiagnostics()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad(ref int x, out int y)
                {
                    x = 1;
                    y = 2;
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR007)
                .WithSpan(6, 29, 6, 30)
                .WithArguments("Bad", "ref", "x"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR007)
                .WithSpan(6, 40, 6, 41)
                .WithArguments("Bad", "out", "y"));
    }

    [Fact]
    public async Task InParameter_NoDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Good(in int x) => x * 2;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task RegularParameter_NoDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Good(int x) => x * 2;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task MixedParameters_ReportsOnlyRefOut()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Mixed(int a, in int b, ref int c, out int d)
                {
                    c = a + b;
                    d = a - b;
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR007)
                .WithSpan(6, 48, 6, 49)
                .WithArguments("Mixed", "ref", "c"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR007)
                .WithSpan(6, 59, 6, 60)
                .WithArguments("Mixed", "out", "d"));
    }

    [Fact]
    public async Task RefStructParameter_ReportsDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public struct Point { public int X, Y; }

            public class Test
            {
                [EnforcedPure]
                public void Bad(ref Point p) { }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR007)
                .WithSpan(8, 31, 8, 32)
                .WithArguments("Bad", "ref", "p"));
    }

    [Fact]
    public async Task NonPureMethod_NoDiagnostic()
    {
        const string code = """
            public class Test
            {
                public void NotPure(ref int x) => x = 42;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }
}
