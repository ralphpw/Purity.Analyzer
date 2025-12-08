using Xunit;

namespace Purity.Analyzer.Tests;

/// <summary>
/// Tests for the PurityWhitelist embedded resource and lookup functionality.
/// </summary>
public sealed class WhitelistTests
{
    [Fact]
    public void Whitelist_ContainsExpectedSignatureCount()
    {
        // Should have a reasonable number of BCL method signatures
        Assert.True(PurityWhitelist.Count > 100, $"Expected >100 signatures, got {PurityWhitelist.Count}");
    }

    [Theory]
    [InlineData("System.Math.Abs(System.Int32)")]
    [InlineData("System.Math.Abs(System.Double)")]
    [InlineData("System.Math.Max(System.Int32,System.Int32)")]
    [InlineData("System.Math.Min(System.Int32,System.Int32)")]
    [InlineData("System.Math.Sqrt(System.Double)")]
    [InlineData("System.Math.Pow(System.Double,System.Double)")]
    public void IsWhitelisted_MathMethods_ReturnsTrue(string signature)
    {
        Assert.True(PurityWhitelist.IsWhitelisted(signature));
    }

    [Theory]
    [InlineData("System.String.IsNullOrEmpty(System.String)")]
    [InlineData("System.String.IsNullOrWhiteSpace(System.String)")]
    [InlineData("System.String.Concat(System.String,System.String)")]
    [InlineData("System.String.Contains(System.String)")]
    [InlineData("System.String.Substring(System.Int32)")]
    public void IsWhitelisted_StringMethods_ReturnsTrue(string signature)
    {
        Assert.True(PurityWhitelist.IsWhitelisted(signature));
    }

    [Theory]
    [InlineData("System.Linq.Enumerable.Where`1(System.Collections.Generic.IEnumerable`1,System.Func`2)")]
    [InlineData("System.Linq.Enumerable.Select`2(System.Collections.Generic.IEnumerable`1,System.Func`2)")]
    [InlineData("System.Linq.Enumerable.Count`1(System.Collections.Generic.IEnumerable`1)")]
    [InlineData("System.Linq.Enumerable.Any`1(System.Collections.Generic.IEnumerable`1)")]
    public void IsWhitelisted_LinqMethods_ReturnsTrue(string signature)
    {
        Assert.True(PurityWhitelist.IsWhitelisted(signature));
    }

    [Theory]
    [InlineData("System.Guid.Parse(System.String)")]
    [InlineData("System.Guid.TryParse(System.String,System.Guid@)")]
    [InlineData("System.Guid.ToString()")]
    [InlineData("System.Guid.Equals(System.Guid)")]
    [InlineData("System.Guid.GetHashCode()")]
    public void IsWhitelisted_GuidMethods_ReturnsTrue(string signature)
    {
        Assert.True(PurityWhitelist.IsWhitelisted(signature));
    }

    [Theory]
    [InlineData("System.Object.ToString()")]
    [InlineData("System.Object.GetHashCode()")]
    [InlineData("System.Object.Equals(System.Object)")]
    [InlineData("System.Object.ReferenceEquals(System.Object,System.Object)")]
    public void IsWhitelisted_ObjectMethods_ReturnsTrue(string signature)
    {
        Assert.True(PurityWhitelist.IsWhitelisted(signature));
    }

    [Theory]
    [InlineData("System.Math.E")]
    [InlineData("System.Math.PI")]
    [InlineData("System.Math.Tau")]
    public void IsWhitelisted_MathConstants_ReturnsTrue(string signature)
    {
        Assert.True(PurityWhitelist.IsWhitelisted(signature));
    }

    [Theory]
    [InlineData("System.Console.WriteLine(System.String)")]
    [InlineData("System.IO.File.ReadAllText(System.String)")]
    [InlineData("System.DateTime.Now")]
    [InlineData("System.Guid.NewGuid()")]
    [InlineData("NotARealMethod")]
    [InlineData("")]
    public void IsWhitelisted_NonPureMethods_ReturnsFalse(string signature)
    {
        Assert.False(PurityWhitelist.IsWhitelisted(signature));
    }

    [Fact]
    public void IsWhitelisted_CaseSensitive()
    {
        // Signatures must match exactly (case-sensitive)
        Assert.True(PurityWhitelist.IsWhitelisted("System.Math.Abs(System.Int32)"));
        Assert.False(PurityWhitelist.IsWhitelisted("system.math.abs(system.int32)"));
        Assert.False(PurityWhitelist.IsWhitelisted("SYSTEM.MATH.ABS(SYSTEM.INT32)"));
    }
}
