using Xunit;
using VerifyCS = Purity.Analyzer.Tests.Verifiers.CSharpAnalyzerVerifier<Purity.Analyzer.PurityAnalyzer>;

namespace Purity.Analyzer.Tests;

/// <summary>
/// Tests for PUR008: Pure method contains unsafe code.
/// </summary>
public sealed class PUR008_UnsafeCodeTests
{
    [Fact]
    public async Task UnsafeModifier_ReportsDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public unsafe int Bad(int* ptr) => *ptr;
            }
            """;

        // The unsafe keyword on the method triggers PUR008
        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR008)
                .WithSpan(6, 23, 6, 26)
                .WithArguments("Bad"));
    }

    [Fact]
    public async Task UnsafeBlock_ReportsDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Bad()
                {
                    int x = 42;
                    unsafe
                    {
                        int* ptr = &x;
                        return *ptr;
                    }
                }
            }
            """;

        // Unsafe block and pointer type each trigger PUR008
        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR008)
                .WithSpan(9, 9, 9, 15)
                .WithArguments("Bad"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR008)
                .WithSpan(11, 13, 11, 17)
                .WithArguments("Bad"));
    }

    [Fact]
    public async Task FixedStatement_ReportsDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Bad(int[] arr)
                {
                    unsafe
                    {
                        fixed (int* ptr = arr)
                        {
                            return *ptr;
                        }
                    }
                }
            }
            """;

        // Unsafe block, fixed statement, and pointer type each trigger PUR008
        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR008)
                .WithSpan(8, 9, 8, 15)
                .WithArguments("Bad"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR008)
                .WithSpan(10, 13, 10, 18)
                .WithArguments("Bad"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR008)
                .WithSpan(10, 20, 10, 24)
                .WithArguments("Bad"));
    }

    [Fact]
    public async Task PointerType_ReportsDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public unsafe int Bad()
                {
                    int x = 42;
                    int* ptr = &x;
                    return *ptr;
                }
            }
            """;

        // unsafe modifier on method triggers PUR008
        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR008)
                .WithSpan(6, 23, 6, 26)
                .WithArguments("Bad"));
    }

    [Fact]
    public async Task NoUnsafeCode_NoDiagnostic()
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
    public async Task UnsafeInNonPureMethod_NoDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                public unsafe int NotPure(int* ptr) => *ptr;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task UnsafeWithBclPure_ReportsDiagnostic()
    {
        const string code = """
            using System.Diagnostics.Contracts;

            public class Test
            {
                [Pure]
                public unsafe int Bad(int* ptr) => *ptr;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR008)
                .WithSpan(6, 23, 6, 26)
                .WithArguments("Bad"));
    }
}
