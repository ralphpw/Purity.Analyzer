using Purity.Analyzer.Tests.Verifiers;

namespace Purity.Analyzer.Tests;

/// <summary>
/// Tests for PUR003: Pure method performs I/O operation.
/// </summary>
public class PUR003_IoDetectionTests
{
    [Fact]
    public async Task PUR003_ConsoleWriteLine_ReportsDiagnostic()
    {
        var code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad()
                {
                    {|#0:Console.WriteLine("Hello")|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR003)
            .WithLocation(0)
            .WithArguments("Bad", "Console.WriteLine(string?)");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR003_ConsoleReadLine_ReportsDiagnostic()
    {
        var code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public string Bad()
                {
                    return {|#0:Console.ReadLine()|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR003)
            .WithLocation(0)
            .WithArguments("Bad", "Console.ReadLine()");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR003_FileReadAllText_ReportsDiagnostic()
    {
        var code = """
            using System.IO;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public string Bad()
                {
                    return {|#0:File.ReadAllText("path.txt")|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR003)
            .WithLocation(0)
            .WithArguments("Bad", "File.ReadAllText(string)");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR003_FileWriteAllText_ReportsDiagnostic()
    {
        var code = """
            using System.IO;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad()
                {
                    {|#0:File.WriteAllText("path.txt", "content")|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR003)
            .WithLocation(0)
            .WithArguments("Bad", "File.WriteAllText(string, string?)");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR003_DirectoryExists_ReportsDiagnostic()
    {
        var code = """
            using System.IO;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public bool Bad()
                {
                    return {|#0:Directory.Exists("path")|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR003)
            .WithLocation(0)
            .WithArguments("Bad", "Directory.Exists(string?)");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR003_NewStreamReader_ReportsDiagnostic()
    {
        var code = """
            using System.IO;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad()
                {
                    using var reader = {|#0:new StreamReader("file.txt")|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR003)
            .WithLocation(0)
            .WithArguments("Bad", "new StreamReader()");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR003_NewStreamWriter_ReportsDiagnostic()
    {
        var code = """
            using System.IO;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad()
                {
                    using var writer = {|#0:new StreamWriter("file.txt")|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR003)
            .WithLocation(0)
            .WithArguments("Bad", "new StreamWriter()");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR003_StreamReaderMethod_ReportsDiagnostic()
    {
        var code = """
            using System.IO;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public string Bad(StreamReader reader)
                {
                    return {|#0:reader.ReadToEnd()|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR003)
            .WithLocation(0)
            .WithArguments("Bad", "StreamReader.ReadToEnd()");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR003_ClassLevelAttribute_ReportsDiagnostic()
    {
        var code = """
            using System;
            using Purity.Contracts;

            [EnforcedPure]
            public class Test
            {
                public void Bad()
                {
                    {|#0:Console.WriteLine("Hello")|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR003)
            .WithLocation(0)
            .WithArguments("Bad", "Console.WriteLine(string?)");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR003_BclPureAttribute_ReportsDiagnostic()
    {
        var code = """
            using System;
            using System.Diagnostics.Contracts;

            public class Test
            {
                [Pure]
                public void Bad()
                {
                    {|#0:Console.WriteLine("Hello")|};
                }
            }
            """;

        var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR003)
            .WithLocation(0)
            .WithArguments("Bad", "Console.WriteLine(string?)");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
    }

    [Fact]
    public async Task PUR003_PureComputation_NoDiagnostic()
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
    public async Task PUR003_StringOperations_NoDiagnostic()
    {
        var code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public string Good(string s)
                {
                    return s.ToUpper();
                }
            }
            """;

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task PUR003_MathOperations_NoDiagnostic()
    {
        var code = """
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

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task PUR003_MethodWithoutAttribute_NoDiagnostic()
    {
        var code = """
            using System;

            public class Test
            {
                public void NotPure()
                {
                    Console.WriteLine("Hello");
                }
            }
            """;

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task PUR003_MultipleIoOperations_ReportsMultipleDiagnostics()
    {
        var code = """
            using System;
            using System.IO;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad()
                {
                    {|#0:Console.WriteLine("Hello")|};
                    var content = {|#1:File.ReadAllText("path.txt")|};
                }
            }
            """;

        var expected1 = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR003)
            .WithLocation(0)
            .WithArguments("Bad", "Console.WriteLine(string?)");

        var expected2 = CSharpAnalyzerVerifier<PurityAnalyzer>
            .Diagnostic(DiagnosticDescriptors.PUR003)
            .WithLocation(1)
            .WithArguments("Bad", "File.ReadAllText(string)");

        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected1, expected2);
    }
}
