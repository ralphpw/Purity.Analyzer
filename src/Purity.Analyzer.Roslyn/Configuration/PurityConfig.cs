using System.Collections.Immutable;

namespace Purity.Analyzer.Configuration;

/// <summary>
/// Represents the complete purity analyzer configuration loaded from .purity/config.json.
/// </summary>
public sealed record PurityConfig
{
    /// <summary>
    /// Trust mode that determines how external methods are evaluated for purity.
    /// </summary>
    public TrustMode TrustMode { get; init; } = TrustMode.Standard;

    /// <summary>
    /// User-defined whitelist and excludelist for methods.
    /// </summary>
    public WhitelistConfig Whitelist { get; init; } = WhitelistConfig.Empty;

    /// <summary>
    /// Methods that are whitelisted but flagged for manual review.
    /// Generates PUR011 (Info) diagnostics when called.
    /// </summary>
    public ImmutableHashSet<string> ReviewRequired { get; init; } = ImmutableHashSet<string>.Empty;

    /// <summary>
    /// Reserved for future use. When true, generates a pending-review.json manifest file.
    /// Currently deferred; users can capture PUR011 diagnostics via build tooling.
    /// </summary>
    /// <remarks>
    /// Roslyn analyzers cannot write files during analysis. This feature is planned
    /// for a future version via MSBuild task or CLI tool integration.
    /// </remarks>
    public bool OutputPendingReview { get; init; } = false;

    /// <summary>
    /// Default configuration when no config file is present.
    /// </summary>
    public static PurityConfig Default { get; } = new();
}

/// <summary>
/// User-defined whitelist configuration.
/// </summary>
public sealed record WhitelistConfig
{
    /// <summary>
    /// Method signatures to include in the whitelist.
    /// Supports exact matches and wildcard patterns (Namespace.Type.*).
    /// </summary>
    public ImmutableHashSet<string> Include { get; init; } = ImmutableHashSet<string>.Empty;

    /// <summary>
    /// Method signatures to exclude from purity consideration.
    /// Exclusions override all other sources (BCL whitelist, attributes).
    /// </summary>
    public ImmutableHashSet<string> Exclude { get; init; } = ImmutableHashSet<string>.Empty;

    /// <summary>
    /// Empty whitelist configuration.
    /// </summary>
    public static WhitelistConfig Empty { get; } = new();
}

/// <summary>
/// Determines how the analyzer trusts external purity claims.
/// </summary>
public enum TrustMode
{
    /// <summary>
    /// Default mode. Trusts:
    /// - BCL whitelist
    /// - [EnforcedPure] in any assembly
    /// - [Pure] with verification
    /// </summary>
    Standard,

    /// <summary>
    /// Strict mode. Trusts:
    /// - BCL whitelist
    /// - [EnforcedPure] only in current compilation
    /// External [EnforcedPure] must be verified or user-whitelisted.
    /// </summary>
    Strict,

    /// <summary>
    /// Zero-trust mode. Only trusts:
    /// - User whitelist (config.whitelist.include)
    /// All other methods, including BCL, require explicit whitelisting.
    /// </summary>
    ZeroTrust
}
