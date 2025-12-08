using System.Collections.Immutable;
using Purity.Analyzer.Configuration;
using Xunit;

namespace Purity.Analyzer.Tests;

/// <summary>
/// Tests for the PatternMatcher class.
/// </summary>
public sealed class PatternMatcherTests
{
    [Theory]
    [InlineData("System.Math.Abs(System.Int32)", "System.Math.Abs(System.Int32)", true)]
    [InlineData("System.Math.Abs(System.Int32)", "System.Math.Abs(System.Int64)", false)]
    [InlineData("System.Math.Abs(System.Int32)", "System.String.Concat(System.String)", false)]
    public void Matches_ExactMatch_ReturnsExpected(string signature, string pattern, bool expected)
    {
        var result = PatternMatcher.Matches(signature, pattern);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("System.Math.Abs(System.Int32)", "System.Math.*", true)]
    [InlineData("System.Math.Max(System.Int32,System.Int32)", "System.Math.*", true)]
    [InlineData("System.Math.PI", "System.Math.*", true)] // Properties also match type wildcards
    [InlineData("System.MathF.Abs(System.Single)", "System.Math.*", false)] // Different type
    [InlineData("System.String.Concat(System.String)", "System.Math.*", false)]
    public void Matches_TypeWildcard_ReturnsExpected(string signature, string pattern, bool expected)
    {
        var result = PatternMatcher.Matches(signature, pattern);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("MyApp.Services.Calculator.Add(System.Int32,System.Int32)", "MyApp.Services.*", true)]
    [InlineData("MyApp.Services.Calculator.Add(System.Int32,System.Int32)", "MyApp.*", true)]
    [InlineData("MyApp.Services.Calculator.Add(System.Int32,System.Int32)", "OtherApp.*", false)]
    public void Matches_NamespaceWildcard_ReturnsExpected(string signature, string pattern, bool expected)
    {
        var result = PatternMatcher.Matches(signature, pattern);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void MatchesAny_EmptyPatterns_ReturnsFalse()
    {
        var patterns = ImmutableHashSet<string>.Empty;
        var result = PatternMatcher.MatchesAny("System.Math.Abs(System.Int32)", patterns);
        Assert.False(result);
    }

    [Fact]
    public void MatchesAny_MultiplePatterns_ReturnsTrueIfAnyMatches()
    {
        var patterns = ImmutableHashSet.Create(
            "System.String.*",
            "System.Math.*");
        
        Assert.True(PatternMatcher.MatchesAny("System.Math.Abs(System.Int32)", patterns));
        Assert.True(PatternMatcher.MatchesAny("System.String.Concat(System.String)", patterns));
        Assert.False(PatternMatcher.MatchesAny("System.Console.WriteLine(System.String)", patterns));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Matches_NullOrEmptyPattern_ReturnsFalse(string? pattern)
    {
        var result = PatternMatcher.Matches("System.Math.Abs(System.Int32)", pattern!);
        Assert.False(result);
    }
}
