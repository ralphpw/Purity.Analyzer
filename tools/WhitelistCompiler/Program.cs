// Compiles whitelist markdown files into a single CSV for embedding as a resource.
// Input:  /whitelist/**/*.md
// Output: /src/Purity.Analyzer.Roslyn/Resources/whitelist.csv

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: WhitelistCompiler <inputDir> <outputPath>");
    Console.Error.WriteLine("  inputDir:   Directory containing *.md whitelist files");
    Console.Error.WriteLine("  outputPath: Output CSV file path");
    return 1;
}

var inputDir = args[0];
var outputPath = args[1];

if (!Directory.Exists(inputDir))
{
    Console.Error.WriteLine($"Input directory not found: {inputDir}");
    return 1;
}

var signatures = Directory.EnumerateFiles(inputDir, "*.md", SearchOption.AllDirectories)
    .SelectMany(ParseMarkdownTable)
    .Distinct()
    .OrderBy(s => s)
    .ToList();

var outputDir = Path.GetDirectoryName(outputPath);
if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
{
    Directory.CreateDirectory(outputDir);
}

File.WriteAllLines(outputPath, signatures);

Console.WriteLine($"Compiled {signatures.Count} signatures from {Directory.EnumerateFiles(inputDir, "*.md", SearchOption.AllDirectories).Count()} files");
Console.WriteLine($"Output: {outputPath}");

return 0;

/// <summary>
/// Parses markdown tables and extracts signatures from the first column.
/// Handles multiple tables per file (Methods, Constants, etc.).
/// </summary>
static IEnumerable<string> ParseMarkdownTable(string filePath)
{
    var lines = File.ReadAllLines(filePath);
    var inTable = false;

    foreach (var line in lines)
    {
        // Start of a new table (header row)
        if (line.StartsWith("| Signature") || line.StartsWith("| `"))
        {
            inTable = true;

            // If this is a data row (starts with signature), process it
            if (line.StartsWith("| `"))
            {
                var signature = ExtractSignature(line);
                if (!string.IsNullOrEmpty(signature))
                    yield return signature;
            }
            continue;
        }

        // Separator row (skip)
        if (line.StartsWith("|---") || line.StartsWith("| ---"))
            continue;

        // End of table
        if (!line.StartsWith("|"))
        {
            inTable = false;
            continue;
        }

        if (!inTable)
            continue;

        // Data row - extract signature from first column
        var sig = ExtractSignature(line);
        if (!string.IsNullOrEmpty(sig))
            yield return sig;
    }
}

/// <summary>
/// Extracts the signature from a markdown table row.
/// Expected format: | `Signature` | ... |
/// </summary>
static string? ExtractSignature(string line)
{
    var columns = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
    if (columns.Length == 0)
        return null;

    var signature = columns[0].Trim().Trim('`');
    return string.IsNullOrWhiteSpace(signature) ? null : signature;
}
