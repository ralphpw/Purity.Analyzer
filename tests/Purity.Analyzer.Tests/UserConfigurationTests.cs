using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Purity.Analyzer.Tests;

/// <summary>
/// Tests for user configuration features: trust modes, user whitelist, PUR011.
/// </summary>
public sealed class UserConfigurationTests
{
    private const string AttributeSource = """
        namespace Purity.Contracts
        {
            [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Class | System.AttributeTargets.Struct)]
            public sealed class EnforcedPureAttribute : System.Attribute { }
        }
        """;

    [Fact]
    public async Task UserWhitelist_Include_TrustsMethod()
    {
        var source = """
            using Purity.Contracts;

            public static class Calculator
            {
                [EnforcedPure]
                public static int Calculate(int x)
                {
                    // This would normally be flagged as non-pure
                    return ThirdParty.Lib.Compute(x);
                }
            }

            namespace ThirdParty
            {
                public static class Lib
                {
                    public static int Compute(int x) => x * 2;
                }
            }
            """;

        var config = """
            {
                "whitelist": {
                    "include": ["ThirdParty.Lib.Compute(System.Int32)"]
                }
            }
            """;

        // With the whitelist, no diagnostic should be reported
        await VerifyWithConfigAsync(source, config);
    }

    [Fact]
    public async Task UserWhitelist_Exclude_BlocksBclMethod()
    {
        var source = """
            using Purity.Contracts;

            public static class Calculator
            {
                [EnforcedPure]
                public static int Calculate(int x)
                {
                    return {|#0:System.Math.Abs(x)|};
                }
            }
            """;

        var config = """
            {
                "whitelist": {
                    "exclude": ["System.Math.Abs(System.Int32)"]
                }
            }
            """;

        // With the exclude, Math.Abs should be flagged
        var expected = new DiagnosticResult(DiagnosticDescriptors.PUR002)
            .WithLocation(0)
            .WithArguments("Calculate", "Math.Abs(int)");

        await VerifyWithConfigAsync(source, config, expected);
    }

    [Fact]
    public async Task UserWhitelist_WildcardInclude_TrustsAllMethods()
    {
        var source = """
            using Purity.Contracts;

            public static class Calculator
            {
                [EnforcedPure]
                public static int Calculate(int x, int y)
                {
                    var a = ThirdParty.Lib.Add(x, y);
                    var b = ThirdParty.Lib.Multiply(x, y);
                    return a + b;
                }
            }

            namespace ThirdParty
            {
                public static class Lib
                {
                    public static int Add(int a, int b) => a + b;
                    public static int Multiply(int a, int b) => a * b;
                }
            }
            """;

        var config = """
            {
                "whitelist": {
                    "include": ["ThirdParty.Lib.*"]
                }
            }
            """;

        // Wildcard should trust all methods in ThirdParty.Lib
        await VerifyWithConfigAsync(source, config);
    }

    [Fact]
    public async Task ReviewRequired_EmitsPUR011()
    {
        var source = """
            using Purity.Contracts;

            public static class Calculator
            {
                [EnforcedPure]
                public static int Calculate(int x)
                {
                    return {|#0:ThirdParty.Lib.Compute(x)|};
                }
            }

            namespace ThirdParty
            {
                public static class Lib
                {
                    public static int Compute(int x) => x * 2;
                }
            }
            """;

        var config = """
            {
                "whitelist": {
                    "include": ["ThirdParty.Lib.Compute(System.Int32)"]
                },
                "reviewRequired": ["ThirdParty.Lib.Compute(System.Int32)"]
            }
            """;

        // Should emit PUR011 (Info) for review-required method
        var expected = new DiagnosticResult(DiagnosticDescriptors.PUR011)
            .WithLocation(0)
            .WithArguments("Calculate", "Lib.Compute(int)");

        await VerifyWithConfigAsync(source, config, expected);
    }

    [Fact]
    public async Task ZeroTrustMode_RequiresExplicitWhitelist()
    {
        var source = """
            using Purity.Contracts;

            public static class Calculator
            {
                [EnforcedPure]
                public static int Calculate(int x)
                {
                    // Math.Abs is normally whitelisted, but not in zero-trust
                    return {|#0:System.Math.Abs(x)|};
                }
            }
            """;

        var config = """
            {
                "trustMode": "zero-trust"
            }
            """;

        // In zero-trust mode, even BCL whitelist is not trusted
        var expected = new DiagnosticResult(DiagnosticDescriptors.PUR002)
            .WithLocation(0)
            .WithArguments("Calculate", "Math.Abs(int)");

        await VerifyWithConfigAsync(source, config, expected);
    }

    [Fact]
    public async Task ZeroTrustMode_WithUserWhitelist_Works()
    {
        var source = """
            using Purity.Contracts;

            public static class Calculator
            {
                [EnforcedPure]
                public static int Calculate(int x)
                {
                    return System.Math.Abs(x);
                }
            }
            """;

        var config = """
            {
                "trustMode": "zero-trust",
                "whitelist": {
                    "include": ["System.Math.Abs(System.Int32)"]
                }
            }
            """;

        // User whitelist overrides zero-trust for specified methods
        await VerifyWithConfigAsync(source, config);
    }

    /// <summary>
    /// Helper to verify analyzer with config file.
    /// </summary>
    private static async Task VerifyWithConfigAsync(
        string source,
        string config,
        params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<PurityAnalyzer, DefaultVerifier>
        {
            TestState =
            {
                Sources = { source, AttributeSource },
                AdditionalFiles = { (".purity/config.json", config) }
            }
        };

        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync();
    }
}
