using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Purity.Analyzer;

/// <summary>
/// Provides O(1) lookup for BCL methods that have been verified as pure.
/// The whitelist is compiled from markdown files in /whitelist/ and embedded as a CSV resource.
/// </summary>
internal static class PurityWhitelist
{
    private static readonly HashSet<string> WhitelistedSignatures = LoadWhitelist();

    /// <summary>
    /// Checks if a method signature is in the purity whitelist.
    /// </summary>
    /// <param name="signature">Fully-qualified method signature in CLR format.</param>
    /// <returns>True if the method is whitelisted as pure.</returns>
    public static bool IsWhitelisted(string signature) => WhitelistedSignatures.Contains(signature);

    /// <summary>
    /// Gets the number of whitelisted signatures (for diagnostics/testing).
    /// </summary>
    public static int Count => WhitelistedSignatures.Count;

    /// <summary>
    /// Loads the whitelist from the embedded CSV resource.
    /// Falls back to empty set if resource is not found (for design-time scenarios).
    /// </summary>
    private static HashSet<string> LoadWhitelist()
    {
        var assembly = typeof(PurityWhitelist).Assembly;
        var resourceName = "Purity.Analyzer.Resources.whitelist.csv";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            // Resource not embedded - return empty set (allows analyzer to function without whitelist)
            return new HashSet<string>(StringComparer.Ordinal);
        }

        using var reader = new StreamReader(stream);
        var signatures = new HashSet<string>(StringComparer.Ordinal);

        while (reader.ReadLine() is { } line)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                // Normalize: remove spaces after commas within parentheses for consistent matching
                var normalized = NormalizeSignature(line);
                signatures.Add(normalized);
            }
        }

        return signatures;
    }

    /// <summary>
    /// Normalizes a signature by removing spaces after commas within parameter lists.
    /// This handles format variations in the whitelist markdown files.
    /// </summary>
    private static string NormalizeSignature(string signature)
    {
        // Simple approach: replace ", " with "," within the signature
        return signature.Replace(", ", ",");
    }
}
