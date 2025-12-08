using Purity.Analyzer.Tests.Verifiers;

namespace Purity.Analyzer.Tests;

/// <summary>
/// Tests for PUR004: Pure method uses non-deterministic API.
/// </summary>
public class PUR004_NonDeterministicTests
{
    [Fact]
    public async Task PUR004_DateTimeNow_ReportsDiagnostic()
    {
        var code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public DateTime Bad()
                {
                    return {|#0:DateTime.Now|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR004)
            .WithLocation(0)
            .WithArguments("Bad", "System.DateTime.Now");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR004_DateTimeUtcNow_ReportsDiagnostic()
    {
        var code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public DateTime Bad()
                {
                    return {|#0:DateTime.UtcNow|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR004)
            .WithLocation(0)
            .WithArguments("Bad", "System.DateTime.UtcNow");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR004_DateTimeToday_ReportsDiagnostic()
    {
        var code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public DateTime Bad()
                {
                    return {|#0:DateTime.Today|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR004)
            .WithLocation(0)
            .WithArguments("Bad", "System.DateTime.Today");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR004_DateTimeOffsetNow_ReportsDiagnostic()
    {
        var code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public DateTimeOffset Bad()
                {
                    return {|#0:DateTimeOffset.Now|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR004)
            .WithLocation(0)
            .WithArguments("Bad", "System.DateTimeOffset.Now");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR004_GuidNewGuid_ReportsDiagnostic()
    {
        var code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public Guid Bad()
                {
                    return {|#0:Guid.NewGuid|}();
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR004)
            .WithLocation(0)
            .WithArguments("Bad", "System.Guid.NewGuid");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR004_NewRandom_ReportsDiagnostic()
    {
        var code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Bad()
                {
                    var rng = {|#0:new Random()|};
                    return 0;
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR004)
            .WithLocation(0)
            .WithArguments("Bad", "new Random()");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR004_RandomNext_ReportsDiagnostic()
    {
        var code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Bad(Random rng)
                {
                    return {|#0:rng.Next()|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR004)
            .WithLocation(0)
            .WithArguments("Bad", "Random.Next()");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR004_EnvironmentTickCount_ReportsDiagnostic()
    {
        var code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Bad()
                {
                    return {|#0:Environment.TickCount|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR004)
            .WithLocation(0)
            .WithArguments("Bad", "System.Environment.TickCount");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR004_EnvironmentTickCount64_ReportsDiagnostic()
    {
        var code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public long Bad()
                {
                    return {|#0:Environment.TickCount64|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR004)
            .WithLocation(0)
            .WithArguments("Bad", "System.Environment.TickCount64");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR004_ClassLevelAttribute_ReportsDiagnostic()
    {
        var code = """
            using System;
            using Purity.Contracts;

            [EnforcedPure]
            public class Test
            {
                public DateTime Bad()
                {
                    return {|#0:DateTime.Now|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR004)
            .WithLocation(0)
            .WithArguments("Bad", "System.DateTime.Now");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR004_BclPureAttribute_ReportsDiagnostic()
    {
        var code = """
            using System;
            using System.Diagnostics.Contracts;

            public class Test
            {
                [Pure]
                public DateTime Bad()
                {
                    return {|#0:DateTime.Now|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR004)
            .WithLocation(0)
            .WithArguments("Bad", "System.DateTime.Now");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR004_DateTimeConstructor_NoDiagnostic()
    {
        var code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public DateTime Good(int year, int month, int day)
                {
                    return new DateTime(year, month, day);
                }
            }
            """;

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task PUR004_GuidParse_NoDiagnostic()
    {
        var code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public Guid Good(string s)
                {
                    return Guid.Parse(s);
                }
            }
            """;

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task PUR004_GuidEmpty_NoDiagnostic()
    {
        var code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public Guid Good()
                {
                    return Guid.Empty;
                }
            }
            """;

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task PUR004_PureComputation_NoDiagnostic()
    {
        var code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Good(int x)
                {
                    return x * 2;
                }
            }
            """;

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task PUR004_MethodWithoutAttribute_NoDiagnostic()
    {
        var code = """
            using System;

            public class Test
            {
                public DateTime NotPure()
                {
                    return DateTime.Now;
                }
            }
            """;

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task PUR004_MultipleNonDeterministicCalls_ReportsMultipleDiagnostics()
    {
        var code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public string Bad()
                {
                    var now = {|#0:DateTime.Now|};
                    var id = {|#1:Guid.NewGuid|}();
                    return id.ToString();
                }
            }
            """;

        var expected1 = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR004)
            .WithLocation(0)
            .WithArguments("Bad", "System.DateTime.Now");

        var expected2 = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR004)
            .WithLocation(1)
            .WithArguments("Bad", "System.Guid.NewGuid");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected1, expected2);
    }
}
