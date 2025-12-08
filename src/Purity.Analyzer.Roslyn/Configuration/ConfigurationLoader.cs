using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Purity.Analyzer.Configuration;

/// <summary>
/// Loads and caches purity configuration from .purity/config.json files.
/// Configuration is loaded once per compilation and cached.
/// </summary>
internal static class ConfigurationLoader
{
    private const string ConfigFileName = "config.json";
    private const string ConfigDirectory = ".purity";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    /// <summary>
    /// Loads configuration from AdditionalFiles (analyzer options).
    /// Supports both project-level and solution-level config files.
    /// </summary>
    /// <param name="additionalFiles">Additional files from the analyzer context.</param>
    /// <returns>Merged configuration, or default if no config files found.</returns>
    public static PurityConfig Load(ImmutableArray<AdditionalText> additionalFiles)
    {
        var configs = additionalFiles
            .Where(IsConfigFile)
            .Select(file => TryParseConfig(file.GetText()?.ToString()))
            .Where(config => config is not null)
            .Cast<PurityConfig>()
            .ToList();

        return configs.Count == 0
            ? PurityConfig.Default
            : MergeConfigs(configs);
    }

    /// <summary>
    /// Loads configuration from a directory path (for testing only).
    /// Searches up the directory tree for .purity/config.json.
    /// </summary>
    /// <param name="projectDir">Project directory to start search from.</param>
    /// <returns>Configuration, or default if no config files found.</returns>
    /// <remarks>
    /// This method uses File I/O and is intended for unit testing only.
    /// The analyzer uses AdditionalFiles via the Load() method instead.
    /// </remarks>
#pragma warning disable RS1035 // File I/O is intentional for testing
    public static PurityConfig LoadFromDirectory(string projectDir)
    {
        var configs = new List<PurityConfig>();

        var currentDir = projectDir;
        while (!string.IsNullOrEmpty(currentDir))
        {
            var configPath = Path.Combine(currentDir, ConfigDirectory, ConfigFileName);
            if (File.Exists(configPath))
            {
                var config = TryParseConfig(File.ReadAllText(configPath));
                if (config is not null)
                    configs.Insert(0, config); // Insert at start (solution-level first)
            }

            var parentDir = Path.GetDirectoryName(currentDir);
            if (parentDir == currentDir)
                break;
            currentDir = parentDir;
        }

        return configs.Count == 0
            ? PurityConfig.Default
            : MergeConfigs(configs);
    }
#pragma warning restore RS1035

    /// <summary>
    /// Determines if an additional file is a purity config file.
    /// </summary>
    private static bool IsConfigFile(AdditionalText file) =>
        file.Path.EndsWith(Path.Combine(ConfigDirectory, ConfigFileName), StringComparison.OrdinalIgnoreCase)
        || file.Path.EndsWith($"{ConfigDirectory}/{ConfigFileName}", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Attempts to parse JSON content into a PurityConfig.
    /// Returns null if parsing fails (malformed JSON).
    /// </summary>
    private static PurityConfig? TryParseConfig(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            var dto = JsonSerializer.Deserialize<ConfigDto>(json!, JsonOptions);
            return dto is null ? null : MapFromDto(dto);
        }
        catch (JsonException)
        {
            // Malformed JSON - return null, use defaults
            return null;
        }
    }

    /// <summary>
    /// Maps the JSON DTO to the immutable config record.
    /// </summary>
    private static PurityConfig MapFromDto(ConfigDto dto)
    {
        var trustMode = dto.TrustMode?.ToLowerInvariant() switch
        {
            "standard" => TrustMode.Standard,
            "strict" => TrustMode.Strict,
            "zero-trust" or "zerotrust" => TrustMode.ZeroTrust,
            _ => TrustMode.Standard
        };

        var whitelist = new WhitelistConfig
        {
            Include = dto.Whitelist?.Include?.ToImmutableHashSet(StringComparer.Ordinal)
                      ?? ImmutableHashSet<string>.Empty,
            Exclude = dto.Whitelist?.Exclude?.ToImmutableHashSet(StringComparer.Ordinal)
                      ?? ImmutableHashSet<string>.Empty
        };

        var reviewRequired = dto.ReviewRequired?.ToImmutableHashSet(StringComparer.Ordinal)
                             ?? ImmutableHashSet<string>.Empty;

        return new PurityConfig
        {
            TrustMode = trustMode,
            Whitelist = whitelist,
            ReviewRequired = reviewRequired,
            OutputPendingReview = dto.OutputPendingReview ?? false
        };
    }

    /// <summary>
    /// Merges multiple configs with later configs (project-level) taking precedence.
    /// Whitelists are merged additively.
    /// </summary>
    private static PurityConfig MergeConfigs(List<PurityConfig> configs)
    {
        if (configs.Count == 1)
            return configs[0];

        // Start with first config (solution-level)
        var result = configs[0];

        // Merge subsequent configs (project-level) - later configs win
        for (var i = 1; i < configs.Count; i++)
        {
            var current = configs[i];

            // Trust mode: later config wins
            var trustMode = current.TrustMode;

            // Whitelists: merge additively
            var include = result.Whitelist.Include.Union(current.Whitelist.Include);
            var exclude = result.Whitelist.Exclude.Union(current.Whitelist.Exclude);

            // Review required: merge additively
            var reviewRequired = result.ReviewRequired.Union(current.ReviewRequired);

            // Output pending review: later config wins
            var outputPendingReview = current.OutputPendingReview;

            result = new PurityConfig
            {
                TrustMode = trustMode,
                Whitelist = new WhitelistConfig { Include = include, Exclude = exclude },
                ReviewRequired = reviewRequired,
                OutputPendingReview = outputPendingReview
            };
        }

        return result;
    }

    /// <summary>
    /// DTO for JSON deserialization. Uses nullable fields for optional values.
    /// </summary>
    private sealed class ConfigDto
    {
        public string? TrustMode { get; set; }
        public WhitelistDto? Whitelist { get; set; }
        public string[]? ReviewRequired { get; set; }
        public bool? OutputPendingReview { get; set; }
    }

    private sealed class WhitelistDto
    {
        public string[]? Include { get; set; }
        public string[]? Exclude { get; set; }
    }
}
