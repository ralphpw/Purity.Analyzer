using System;

namespace Purity.Analyzer.Configuration;

/// <summary>
/// Provides wildcard pattern matching for method signatures in user whitelist.
/// Supports exact matches and namespace/type wildcards (Namespace.Type.*).
/// </summary>
internal static class PatternMatcher
{
    /// <summary>
    /// Checks if a signature matches any pattern in the set.
    /// </summary>
    /// <param name="signature">Full method signature to check.</param>
    /// <param name="patterns">Set of patterns (exact or wildcard).</param>
    /// <returns>True if any pattern matches.</returns>
    public static bool MatchesAny(string signature, System.Collections.Immutable.ImmutableHashSet<string> patterns)
    {
        foreach (var pattern in patterns)
        {
            if (Matches(signature, pattern))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if a signature matches a single pattern.
    /// </summary>
    /// <param name="signature">Full method signature (e.g., "System.Math.Abs(System.Int32)").</param>
    /// <param name="pattern">Pattern to match. Supports:
    /// - Exact match: "System.Math.Abs(System.Int32)"
    /// - Type wildcard: "System.Math.*" (matches all methods in System.Math)
    /// - Namespace wildcard: "System.*" (matches all types in System namespace)
    /// </param>
    /// <returns>True if the signature matches the pattern.</returns>
    public static bool Matches(string signature, string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            return false;

        // Exact match
        if (string.Equals(signature, pattern, StringComparison.Ordinal))
            return true;

        // Wildcard pattern: ends with ".*"
        if (pattern.EndsWith(".*", StringComparison.Ordinal))
        {
            var prefix = pattern.Substring(0, pattern.Length - 2); // Remove ".*"
            
            // For "System.Math.*", we want to match:
            // - System.Math.Abs(System.Int32) ✓
            // - System.Math.Max(System.Int32,System.Int32) ✓
            // But NOT:
            // - System.MathF.Abs(System.Single) ✗
            // So we need to check for prefix + "." or prefix + "(" for type-level patterns
            
            // Check if signature starts with prefix followed by method separator
            if (signature.StartsWith(prefix + ".", StringComparison.Ordinal))
                return true;
            
            // Also handle methods directly under the wildcard (e.g., ExtensionClass.*)
            // where signature is ExtensionClass.Method(...)
            var parenIndex = signature.IndexOf('(');
            if (parenIndex > 0)
            {
                var signatureWithoutParams = signature.Substring(0, parenIndex);
                var lastDot = signatureWithoutParams.LastIndexOf('.');
                if (lastDot > 0)
                {
                    var typeQualifier = signatureWithoutParams.Substring(0, lastDot);
                    if (string.Equals(typeQualifier, prefix, StringComparison.Ordinal))
                        return true;
                }
            }
        }

        return false;
    }
}
