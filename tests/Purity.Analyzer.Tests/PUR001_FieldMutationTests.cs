using Purity.Analyzer.Tests.Verifiers;

namespace Purity.Analyzer.Tests;

/// <summary>
/// Tests for PUR001: Pure method mutates field.
/// </summary>
public class PUR001_FieldMutationTests
{
    [Fact]
    public async Task PUR001_DirectFieldAssignment_ReportsDiagnostic()
    {
        var code = """
            using Purity.Contracts;

            public class Test
            {
                private int _counter;

                [EnforcedPure]
                public int Bad()
                {
                    {|#0:_counter|} = 42;
                    return _counter;
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR001)
            .WithLocation(0)
            .WithArguments("Bad", "_counter");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR001_StaticFieldAssignment_ReportsDiagnostic()
    {
        var code = """
            using Purity.Contracts;

            public class Test
            {
                private static int _global;

                [EnforcedPure]
                public void Bad()
                {
                    {|#0:_global|} = 100;
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR001)
            .WithLocation(0)
            .WithArguments("Bad", "_global");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR001_PostIncrement_ReportsDiagnostic()
    {
        var code = """
            using Purity.Contracts;

            public class Test
            {
                private int _counter;

                [EnforcedPure]
                public int Bad()
                {
                    {|#0:_counter|}++;
                    return _counter;
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR001)
            .WithLocation(0)
            .WithArguments("Bad", "_counter");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR001_PreIncrement_ReportsDiagnostic()
    {
        var code = """
            using Purity.Contracts;

            public class Test
            {
                private int _counter;

                [EnforcedPure]
                public int Bad()
                {
                    ++{|#0:_counter|};
                    return _counter;
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR001)
            .WithLocation(0)
            .WithArguments("Bad", "_counter");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR001_PostDecrement_ReportsDiagnostic()
    {
        var code = """
            using Purity.Contracts;

            public class Test
            {
                private int _counter;

                [EnforcedPure]
                public int Bad()
                {
                    {|#0:_counter|}--;
                    return _counter;
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR001)
            .WithLocation(0)
            .WithArguments("Bad", "_counter");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR001_CompoundAssignment_ReportsDiagnostic()
    {
        var code = """
            using Purity.Contracts;

            public class Test
            {
                private int _counter;

                [EnforcedPure]
                public int Bad()
                {
                    {|#0:_counter|} += 10;
                    return _counter;
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR001)
            .WithLocation(0)
            .WithArguments("Bad", "_counter");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR001_FieldViaThis_ReportsDiagnostic()
    {
        var code = """
            using Purity.Contracts;

            public class Test
            {
                private int _counter;

                [EnforcedPure]
                public int Bad()
                {
                    {|#0:this._counter|} = 42;
                    return _counter;
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR001)
            .WithLocation(0)
            .WithArguments("Bad", "_counter");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR001_ClassLevelAttribute_ReportsDiagnostic()
    {
        var code = """
            using Purity.Contracts;

            [EnforcedPure]
            public class Test
            {
                private int _counter;

                public int Bad()
                {
                    {|#0:_counter|} = 42;
                    return _counter;
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR001)
            .WithLocation(0)
            .WithArguments("Bad", "_counter");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR001_BclPureAttribute_ReportsDiagnostic()
    {
        var code = """
            using System.Diagnostics.Contracts;

            public class Test
            {
                private int _counter;

                [Pure]
                public int Bad()
                {
                    {|#0:_counter|} = 42;
                    return _counter;
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR001)
            .WithLocation(0)
            .WithArguments("Bad", "_counter");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR001_LocalVariableMutation_NoDiagnostic()
    {
        var code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Good(int x)
                {
                    int local = x;
                    local++;
                    return local;
                }
            }
            """;

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task PUR001_ParameterReassignment_NoDiagnostic()
    {
        var code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Good(int x)
                {
                    x = 42;
                    return x;
                }
            }
            """;

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task PUR001_ReadingField_NoDiagnostic()
    {
        var code = """
            using Purity.Contracts;

            public class Test
            {
                private int _counter;

                [EnforcedPure]
                public int Good()
                {
                    return _counter;
                }
            }
            """;

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task PUR001_MethodWithoutAttribute_NoDiagnostic()
    {
        var code = """
            public class Test
            {
                private int _counter;

                public int NotPure()
                {
                    _counter = 42;
                    return _counter;
                }
            }
            """;

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task PUR001_MultipleMutations_ReportsMultipleDiagnostics()
    {
        var code = """
            using Purity.Contracts;

            public class Test
            {
                private int _a;
                private int _b;

                [EnforcedPure]
                public void Bad()
                {
                    {|#0:_a|} = 1;
                    {|#1:_b|} = 2;
                }
            }
            """;

        var expected1 = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR001)
            .WithLocation(0)
            .WithArguments("Bad", "_a");

        var expected2 = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR001)
            .WithLocation(1)
            .WithArguments("Bad", "_b");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected1, expected2);
    }
}
