using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;
using Purity.Analyzer.Configuration;
using Xunit;

namespace Purity.Analyzer.Tests;

/// <summary>
/// Tests for the ConfigurationLoader class.
/// </summary>
public sealed class ConfigurationLoaderTests
{
    [Fact]
    public void Load_NoConfigFiles_ReturnsDefault()
    {
        var additionalFiles = ImmutableArray<Microsoft.CodeAnalysis.AdditionalText>.Empty;
        var config = ConfigurationLoader.Load(additionalFiles);

        Assert.Equal(TrustMode.Standard, config.TrustMode);
        Assert.Empty(config.Whitelist.Include);
        Assert.Empty(config.Whitelist.Exclude);
        Assert.Empty(config.ReviewRequired);
        Assert.False(config.OutputPendingReview);
    }

    [Fact]
    public void Load_WithConfigFile_ParsesCorrectly()
    {
        var json = """
            {
                "trustMode": "strict",
                "whitelist": {
                    "include": ["MyApp.Pure.*", "ThirdParty.Lib.Calculate"],
                    "exclude": ["System.Console.*"]
                },
                "reviewRequired": ["ThirdParty.Lib.Calculate"],
                "outputPendingReview": true
            }
            """;

        var additionalFiles = ImmutableArray.Create<Microsoft.CodeAnalysis.AdditionalText>(
            new TestAdditionalText(".purity/config.json", json));

        var config = ConfigurationLoader.Load(additionalFiles);

        Assert.Equal(TrustMode.Strict, config.TrustMode);
        Assert.Contains("MyApp.Pure.*", config.Whitelist.Include);
        Assert.Contains("ThirdParty.Lib.Calculate", config.Whitelist.Include);
        Assert.Contains("System.Console.*", config.Whitelist.Exclude);
        Assert.Contains("ThirdParty.Lib.Calculate", config.ReviewRequired);
        Assert.True(config.OutputPendingReview);
    }

    [Theory]
    [InlineData("standard", TrustMode.Standard)]
    [InlineData("Standard", TrustMode.Standard)]
    [InlineData("strict", TrustMode.Strict)]
    [InlineData("Strict", TrustMode.Strict)]
    [InlineData("zero-trust", TrustMode.ZeroTrust)]
    [InlineData("Zero-Trust", TrustMode.ZeroTrust)]
    [InlineData("zerotrust", TrustMode.ZeroTrust)]
    [InlineData("invalid", TrustMode.Standard)] // Falls back to standard
    public void Load_TrustMode_ParsesCorrectly(string trustModeValue, TrustMode expected)
    {
        var json = $$"""
            {
                "trustMode": "{{trustModeValue}}"
            }
            """;

        var additionalFiles = ImmutableArray.Create<Microsoft.CodeAnalysis.AdditionalText>(
            new TestAdditionalText(".purity/config.json", json));

        var config = ConfigurationLoader.Load(additionalFiles);
        Assert.Equal(expected, config.TrustMode);
    }

    [Fact]
    public void Load_MalformedJson_ReturnsDefault()
    {
        var json = "{ invalid json }";

        var additionalFiles = ImmutableArray.Create<Microsoft.CodeAnalysis.AdditionalText>(
            new TestAdditionalText(".purity/config.json", json));

        var config = ConfigurationLoader.Load(additionalFiles);
        Assert.Equal(TrustMode.Standard, config.TrustMode);
    }

    [Fact]
    public void Load_EmptyFile_ReturnsDefault()
    {
        var additionalFiles = ImmutableArray.Create<Microsoft.CodeAnalysis.AdditionalText>(
            new TestAdditionalText(".purity/config.json", ""));

        var config = ConfigurationLoader.Load(additionalFiles);
        Assert.Equal(TrustMode.Standard, config.TrustMode);
    }

    [Fact]
    public void Load_JsonWithComments_ParsesCorrectly()
    {
        var json = """
            {
                // This is a comment
                "trustMode": "strict"
            }
            """;

        var additionalFiles = ImmutableArray.Create<Microsoft.CodeAnalysis.AdditionalText>(
            new TestAdditionalText(".purity/config.json", json));

        var config = ConfigurationLoader.Load(additionalFiles);
        Assert.Equal(TrustMode.Strict, config.TrustMode);
    }

    [Fact]
    public void Load_MultipleConfigs_MergesCorrectly()
    {
        var solutionConfig = """
            {
                "trustMode": "standard",
                "whitelist": {
                    "include": ["SharedLib.*"]
                }
            }
            """;

        var projectConfig = """
            {
                "trustMode": "strict",
                "whitelist": {
                    "include": ["ProjectLib.*"]
                }
            }
            """;

        var additionalFiles = ImmutableArray.Create<Microsoft.CodeAnalysis.AdditionalText>(
            new TestAdditionalText("solution/.purity/config.json", solutionConfig),
            new TestAdditionalText("solution/project/.purity/config.json", projectConfig));

        var config = ConfigurationLoader.Load(additionalFiles);

        // Later (project) trust mode wins
        Assert.Equal(TrustMode.Strict, config.TrustMode);
        // Whitelists are merged
        Assert.Contains("SharedLib.*", config.Whitelist.Include);
        Assert.Contains("ProjectLib.*", config.Whitelist.Include);
    }

    /// <summary>
    /// Test implementation of AdditionalText for unit testing.
    /// </summary>
    private sealed class TestAdditionalText : Microsoft.CodeAnalysis.AdditionalText
    {
        private readonly string _path;
        private readonly string _content;

        public TestAdditionalText(string path, string content)
        {
            _path = path;
            _content = content;
        }

        public override string Path => _path;

        public override SourceText? GetText(System.Threading.CancellationToken cancellationToken = default) =>
            SourceText.From(_content);
    }
}
