using Xunit;
using VerifyCS = Purity.Analyzer.Tests.Verifiers.CSharpAnalyzerVerifier<Purity.Analyzer.PurityAnalyzer>;

namespace Purity.Analyzer.Tests;

/// <summary>
/// Tests for PUR010: Pure method uses exception-throwing API.
/// Note: PUR010 is a Warning, not an Error. It suggests using TryParse patterns.
/// Parse methods also trigger PUR002 since they're not whitelisted.
/// </summary>
public sealed class PUR010_ExceptionControlFlowTests
{
    [Fact]
    public async Task IntParse_ReportsWarning()
    {
        const string code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Bad(string s) => int.Parse(s);
            }
            """;

        // Triggers both PUR002 (non-pure) and PUR010 (exception control flow)
        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(7, 33, 7, 45)
                .WithArguments("Bad", "int.Parse(string)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR010)
                .WithSpan(7, 33, 7, 45)
                .WithArguments("Bad", "int.Parse(string)"));
    }

    [Fact]
    public async Task DoubleParse_ReportsWarning()
    {
        const string code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public double Bad(string s) => double.Parse(s);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(7, 36, 7, 51)
                .WithArguments("Bad", "double.Parse(string)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR010)
                .WithSpan(7, 36, 7, 51)
                .WithArguments("Bad", "double.Parse(string)"));
    }

    [Fact]
    public async Task DateTimeParse_ReportsWarning()
    {
        const string code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public DateTime Bad(string s) => DateTime.Parse(s);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(7, 38, 7, 55)
                .WithArguments("Bad", "DateTime.Parse(string)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR010)
                .WithSpan(7, 38, 7, 55)
                .WithArguments("Bad", "DateTime.Parse(string)"));
    }

    [Fact]
    public async Task GuidParse_ReportsWarning()
    {
        const string code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public Guid Bad(string s) => Guid.Parse(s);
            }
            """;

        // Guid.Parse is pure (whitelisted) but throws on invalid input
        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR010)
                .WithSpan(7, 34, 7, 47)
                .WithArguments("Bad", "Guid.Parse(string)"));
    }

    [Fact]
    public async Task EnumParse_ReportsWarning()
    {
        const string code = """
            using System;
            using Purity.Contracts;

            public enum Color { Red, Green, Blue }

            public class Test
            {
                [EnforcedPure]
                public Color Bad(string s) => (Color)Enum.Parse(typeof(Color), s);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(9, 42, 9, 70)
                .WithArguments("Bad", "Enum.Parse(Type, string)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR010)
                .WithSpan(9, 42, 9, 70)
                .WithArguments("Bad", "Enum.Parse(Type, string)"));
    }

    [Fact]
    public async Task TryParse_NoDiagnostic()
    {
        const string code = """
            #nullable enable
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Good(string s)
                {
                    return int.TryParse(s, out var result) ? result : 0;
                }
            }
            """;

        // TryParse triggers PUR002 (not whitelisted) but not PUR010 (doesn't throw)
        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(10, 16, 10, 47)
                .WithArguments("Good", "int.TryParse(string?, out int)"));
    }

    [Fact]
    public async Task ParseInNonPureMethod_NoDiagnostic()
    {
        const string code = """
            using System;

            public class Test
            {
                public int NotPure(string s) => int.Parse(s);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task ParseWithBclPure_ReportsWarning()
    {
        const string code = """
            using System;
            using System.Diagnostics.Contracts;

            public class Test
            {
                [Pure]
                public int Bad(string s) => int.Parse(s);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(7, 33, 7, 45)
                .WithArguments("Bad", "int.Parse(string)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR010)
                .WithSpan(7, 33, 7, 45)
                .WithArguments("Bad", "int.Parse(string)"));
    }

    [Fact]
    public async Task MultipleParseCalls_ReportsMultipleWarnings()
    {
        const string code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public (int, double) Bad(string a, string b)
                    => (int.Parse(a), double.Parse(b));
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(8, 13, 8, 25)
                .WithArguments("Bad", "int.Parse(string)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR010)
                .WithSpan(8, 13, 8, 25)
                .WithArguments("Bad", "int.Parse(string)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(8, 27, 8, 42)
                .WithArguments("Bad", "double.Parse(string)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR010)
                .WithSpan(8, 27, 8, 42)
                .WithArguments("Bad", "double.Parse(string)"));
    }

    [Fact]
    public async Task ConvertMethods_ReportsBothDiagnostics()
    {
        const string code = """
            #nullable enable
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Bad(string s) => Convert.ToInt32(s);
            }
            """;

        // Convert.ToInt32 triggers both PUR002 and PUR010
        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(8, 33, 8, 51)
                .WithArguments("Bad", "Convert.ToInt32(string?)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR010)
                .WithSpan(8, 33, 8, 51)
                .WithArguments("Bad", "Convert.ToInt32(string?)"));
    }
}
