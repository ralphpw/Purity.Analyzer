# TASK-018: User Configuration & Trust Modes

**Priority:** P1  
**Estimate:** 6 hours  
**Status:** Not Started  
**Depends On:** TASK-003, TASK-006

---

## Description

Implement user configuration via `.purity/config.json` including:
- Trust modes (standard, strict, zero-trust)
- User whitelist (include/exclude)
- Review-required tracking
- Pending review manifest output

See [docs/Whitelist.md](../docs/Whitelist.md#user-configuration) for full specification.

---

## Configuration Schema

```json
{
  "$schema": "https://purity-analyzer.dev/schema/config.json",
  "trustMode": "standard | strict | zero-trust",
  "whitelist": {
    "include": ["Namespace.Type.Method", "Namespace.Type.*"],
    "exclude": ["Namespace.Type.Method"]
  },
  "reviewRequired": ["Namespace.Type.Method"],
  "outputPendingReview": false
}
```

---

## Implementation

### 1. Configuration Loading

```csharp
public record PurityConfig(
    TrustMode TrustMode,
    WhitelistConfig Whitelist,
    ImmutableHashSet<string> ReviewRequired,
    bool OutputPendingReview);

public record WhitelistConfig(
    ImmutableHashSet<string> Include,
    ImmutableHashSet<string> Exclude);

public enum TrustMode { Standard, Strict, ZeroTrust }

public static class ConfigurationLoader
{
    private static readonly string[] ConfigPaths =
    [
        ".purity/config.json",
        "../.purity/config.json",  // Solution-level
    ];
    
    public static PurityConfig Load(string projectDir)
    {
        var configs = ConfigPaths
            .Select(p => Path.Combine(projectDir, p))
            .Where(File.Exists)
            .Select(ParseConfig)
            .ToList();
        
        return MergeConfigs(configs);
    }
    
    private static PurityConfig MergeConfigs(List<PurityConfig> configs)
    {
        // Later configs (project-level) override earlier (solution-level)
        // Whitelists merge additively
        // ...
    }
}
```

### 2. Trust Mode Integration

Modify `IsMethodPure()` in PUR002 analyzer:

```csharp
private bool IsMethodPure(IMethodSymbol method, PurityConfig config)
{
    var signature = GetFullSignature(method);
    
    // User exclude always wins
    if (config.Whitelist.Exclude.Contains(signature))
        return false;
    
    // User include is always trusted
    if (MatchesIncludePattern(signature, config.Whitelist.Include))
        return true;
    
    return config.TrustMode switch
    {
        TrustMode.Standard => IsMethodPureStandard(method),
        TrustMode.Strict => IsMethodPureStrict(method),
        TrustMode.ZeroTrust => false,  // Only user whitelist trusted
        _ => throw new InvalidOperationException($"Unknown trust mode: {config.TrustMode}")
    };
}

private bool IsMethodPureStandard(IMethodSymbol method)
{
    // BCL whitelist
    if (_bclWhitelist.Contains(GetFullSignature(method)))
        return true;
    
    // [EnforcedPure] in any assembly
    if (HasEnforcedPureAttribute(method))
        return true;
    
    // [Pure] requires verification (handled elsewhere)
    return false;
}

private bool IsMethodPureStrict(IMethodSymbol method)
{
    // BCL whitelist
    if (_bclWhitelist.Contains(GetFullSignature(method)))
        return true;
    
    // [EnforcedPure] only in current compilation
    if (HasEnforcedPureAttribute(method) && IsInCurrentCompilation(method))
        return true;
    
    // External [EnforcedPure] must be verified
    return false;
}
```

### 3. Wildcard Pattern Matching

Support `Namespace.Type.*` patterns:

```csharp
private bool MatchesIncludePattern(string signature, ImmutableHashSet<string> patterns)
{
    foreach (var pattern in patterns)
    {
        if (pattern.EndsWith(".*"))
        {
            var prefix = pattern[..^2];  // Remove ".*"
            if (signature.StartsWith(prefix))
                return true;
        }
        else if (signature == pattern)
        {
            return true;
        }
    }
    return false;
}
```

### 4. PUR011 Diagnostic

```csharp
public static readonly DiagnosticDescriptor PUR011 = new(
    id: "PUR011",
    title: "Method pending review",
    messageFormat: "Method '{0}' calls '{1}' which is marked for review",
    category: "Purity",
    defaultSeverity: DiagnosticSeverity.Info,
    isEnabledByDefault: true,
    description: "The called method is whitelisted but flagged for manual review.");
```

### 5. Pending Review Manifest

When `outputPendingReview: true`:

```csharp
public class PendingReviewCollector
{
    private readonly ConcurrentBag<PendingReviewEntry> _entries = new();
    
    public void Add(IMethodSymbol caller, IMethodSymbol callee, Location location)
    {
        _entries.Add(new(
            GetFullSignature(callee),
            location.GetLineSpan().Path,
            location.GetLineSpan().StartLinePosition.Line,
            GetFullSignature(caller)));
    }
    
    public void WriteManifest(string projectDir)
    {
        var manifest = new
        {
            generated = DateTime.UtcNow.ToString("o"),
            trustMode = _config.TrustMode.ToString().ToLowerInvariant(),
            pendingReview = _entries
                .GroupBy(e => e.Signature)
                .Select(g => new
                {
                    signature = g.Key,
                    calledFrom = g.Select(e => new
                    {
                        file = e.FilePath,
                        line = e.Line,
                        method = e.CallerSignature
                    })
                })
        };
        
        var outputPath = Path.Combine(projectDir, ".purity", "pending-review.json");
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        File.WriteAllText(outputPath, JsonSerializer.Serialize(manifest, _jsonOptions));
    }
}
```

---

## Test Cases

### Configuration Loading

- [ ] Loads project-level config
- [ ] Loads solution-level config
- [ ] Merges configs with correct precedence
- [ ] Handles missing config (uses defaults)
- [ ] Handles malformed JSON gracefully

### Trust Modes

- [ ] Standard: trusts BCL whitelist
- [ ] Standard: trusts `[EnforcedPure]` in references
- [ ] Strict: trusts BCL whitelist
- [ ] Strict: re-verifies `[EnforcedPure]` in references
- [ ] Zero-trust: only user whitelist trusted
- [ ] Zero-trust: emits PUR002 for BCL methods not in user whitelist

### User Whitelist

- [ ] `include` whitelists exact signature
- [ ] `include` supports wildcard `Namespace.*`
- [ ] `exclude` overrides BCL whitelist
- [ ] `exclude` overrides `[EnforcedPure]`

### Review Tracking

- [ ] PUR011 emitted for `reviewRequired` methods
- [ ] Pending review manifest generated when enabled
- [ ] Manifest contains all required fields

---

## Notes

- Configuration is loaded once per compilation, cached
- Use `AdditionalFiles` analyzer option to locate config
- Consider source generator for config validation
