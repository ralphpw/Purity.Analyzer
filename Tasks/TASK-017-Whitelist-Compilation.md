# TASK-017: Whitelist Compilation Pipeline

**Priority:** P0  
**Estimate:** 3 hours  
**Status:** Not Started  
**Depends On:** TASK-001

---

## Description

Implement the build-time compilation of whitelist source files (`/whitelist/**/*.md`) into the runtime artifact (`whitelist.csv`) that is embedded as an assembly resource.

See [docs/Whitelist.md](../docs/Whitelist.md) for the architecture specification.

---

## Implementation

### Build Target

Add MSBuild target to compile whitelist at build time:

```xml
<Target Name="CompileWhitelist" BeforeTargets="BeforeBuild">
  <Exec Command="dotnet run --project tools/WhitelistCompiler -- $(MSBuildProjectDirectory)/../whitelist $(MSBuildProjectDirectory)/Resources/whitelist.csv" />
</Target>
```

### Compiler Tool

Create `tools/WhitelistCompiler/Program.cs`:

```csharp
// Input: /whitelist/**/*.md
// Output: /Purity.Analyzer/Resources/whitelist.csv

var inputDir = args[0];
var outputPath = args[1];

var signatures = Directory.EnumerateFiles(inputDir, "*.md", SearchOption.AllDirectories)
    .SelectMany(ParseMarkdownTable)
    .Distinct()
    .OrderBy(s => s);

File.WriteAllLines(outputPath, signatures);

static IEnumerable<string> ParseMarkdownTable(string filePath)
{
    var lines = File.ReadAllLines(filePath);
    var inTable = false;
    
    foreach (var line in lines)
    {
        if (line.StartsWith("| Signature"))
        {
            inTable = true;
            continue;
        }
        if (line.StartsWith("|---"))
            continue;
        if (!line.StartsWith("|"))
        {
            inTable = false;
            continue;
        }
        if (!inTable)
            continue;
        
        var columns = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
        if (columns.Length > 0)
        {
            var signature = columns[0].Trim().Trim('`');
            if (!string.IsNullOrEmpty(signature))
                yield return signature;
        }
    }
}
```

### Embedded Resource

Ensure `whitelist.csv` is embedded:

```xml
<ItemGroup>
  <EmbeddedResource Include="Resources\whitelist.csv" />
</ItemGroup>
```

---

## Validation

1. Build the analyzer project
2. Verify `whitelist.csv` contains expected signatures
3. Verify embedded resource is accessible at runtime

---

## Test Cases

- [ ] Parses single-table markdown files
- [ ] Handles multiple tables per file (Methods, Constants)
- [ ] Ignores malformed rows
- [ ] Produces deterministic output (sorted, no duplicates)
- [ ] Handles generic method signatures correctly

---

## Notes

- The compiler is intentionally simple—a .NET tool, not a complex build task
- Future enhancement: validate signature format before emitting
- Consider caching to avoid recompilation when source unchanged
