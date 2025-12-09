# Contributing to Purity.Analyzer

Thank you for your interest in contributing! This guide will help you get started. See [docs/](docs/) for detailed specifications.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with C# extension

### Clone and Build

```bash
git clone https://github.com/ralphpw/Purity.Analyzer.git
cd Purity.Analyzer
dotnet build
```

### Run Tests

```bash
dotnet test
```

---

## Project Structure

```
Purity.Analyzer/
├── src/
│   ├── Purity.Analyzer/              # The Roslyn analyzer
│   │   ├── Analyzers/                # Analyzer implementations
│   │   ├── Diagnostics/              # Diagnostic descriptors
│   │   └── Resources/                # Embedded resources (whitelist)
│   └── Purity.Analyzer.Attributes/   # Attribute definitions
├── tests/
│   └── Purity.Analyzer.Tests/        # Unit tests
├── docs/                             # Documentation
└── Tasks/                            # Implementation tasks
```

---

## Troubleshooting Tests

**Tests fail to build or run?**

Ensure you have .NET 8 SDK or later installed:

```bash
dotnet --version
```

If you have only older .NET versions, [download .NET 8 or later](https://dotnet.microsoft.com/download/dotnet).

---

## Code Style

Follow the guidelines in [.github/LLMStyleGuideline.md](../.github/LLMStyleGuideline.md).

### Quick Reference

**Naming:**
- Private fields: `_camelCase`
- Private methods: `PascalCase`
- Public API: `PascalCase`

**Style:**
- Functional over imperative (LINQ > foreach)
- Fail early with explicit exceptions
- Switch expressions must throw on default
- Modern C# syntax (collection expressions, target-typed new)

**Prohibited:**
- No `dynamic`
- No reflection in analyzer code
- No `async void`
- No `.Result` or `.Wait()`

---

## Adding a New Diagnostic

### 1. Create the Diagnostic Descriptor

In `Diagnostics/DiagnosticDescriptors.cs`:

```csharp
public static readonly DiagnosticDescriptor PUR011 = new(
    id: "PUR011",
    title: "Brief title",
    messageFormat: "Method '{0}' marked as pure {1}",
    category: "Purity",
    defaultSeverity: DiagnosticSeverity.Error,
    isEnabledByDefault: true,
    description: "Longer description of what this rule detects.");
```

### 2. Implement the Check

In the appropriate analyzer (or create a new one):

```csharp
private void AnalyzeMethodForNewRule(SyntaxNodeAnalysisContext context)
{
    var methodDeclaration = (MethodDeclarationSyntax)context.Node;
    
    if (!HasPurityAttribute(methodDeclaration, context.SemanticModel))
        return;
    
    // Your detection logic here
    
    if (violation)
    {
        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.PUR011,
            location,
            methodName,
            violationDetails);
        context.ReportDiagnostic(diagnostic);
    }
}
```

### 3. Register the Analyzer

```csharp
public override void Initialize(AnalysisContext context)
{
    context.RegisterSyntaxNodeAction(
        AnalyzeMethodForNewRule,
        SyntaxKind.MethodDeclaration);
}
```

### 4. Write Tests

Create `PUR011Tests.cs`:

```csharp
public class PUR011Tests : AnalyzerTestBase
{
    [Test]
    public async Task PUR011_WhenViolation_ReportsDiagnostic()
    {
        var code = @"
using Purity.Contracts;

public class Test
{
    [EnforcedPure]
    public void Method()
    {
        // Code that triggers PUR011
    }
}";
        
        var expected = Verify.Diagnostic(DiagnosticDescriptors.PUR011)
            .WithLocation(8, 9)
            .WithArguments("Method", "violation details");
        
        await Verify.VerifyAnalyzerAsync(code, expected);
    }
    
    [Test]
    public async Task PUR011_WhenNoViolation_NoDiagnostic()
    {
        var code = @"
using Purity.Contracts;

public class Test
{
    [EnforcedPure]
    public void Method()
    {
        // Clean code
    }
}";
        
        await Verify.VerifyAnalyzerAsync(code);
    }
}
```

### 5. Document the Rule

Add to `docs/Rules.md` with:
- What it detects
- Why it matters
- Code examples (violations and allowed)
- How to fix

---

## Expanding the Whitelist

### Adding BCL Methods

1. Verify the method is truly pure:
   - No side effects
   - Deterministic
   - No I/O
   - No parameter mutation

2. Add to `Resources/bcl-whitelist.json`:
   ```json
   {
     "methods": {
       "System.NewNamespace.NewMethod": true
     }
   }
   ```

3. Write a test demonstrating purity (ideally property-based)

4. Submit PR with evidence

---

## Pull Request Process

### Before Submitting

1. **Run all tests:** `dotnet test`
2. **Check for errors:** `dotnet build`
3. **Follow code style:** See [LLMStyleGuideline.md](../.github/LLMStyleGuideline.md)

### PR Requirements

- Clear title and description
- Tests for new functionality
- Documentation updates if needed
- No breaking changes without discussion

### Commit Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation only
- `test`: Adding tests
- `refactor`: Code change that neither fixes a bug nor adds a feature
- `perf`: Performance improvement
- `chore`: Build process or auxiliary tool changes

**Examples:**
```
feat(analyzer): implement PUR006 parameter mutation detection
fix(whitelist): add missing Math.Clamp to BCL whitelist
docs(rules): improve PUR001 examples
test(PUR002): add edge case for generic method calls
```

---

## Development Tips

### Testing Analyzers Locally

1. Build the analyzer:
   ```bash
   dotnet build src/Purity.Analyzer
   ```

2. Reference in a test project:
   ```xml
   <ProjectReference Include="..\Purity.Analyzer\Purity.Analyzer.csproj"
                     OutputItemType="Analyzer"
                     ReferenceOutputAssembly="false" />
   ```

3. Write code and observe diagnostics in the IDE

### Debugging Analyzers

In Visual Studio:
1. Set `Purity.Analyzer.Tests` as startup project
2. Set breakpoint in analyzer code
3. Run tests in debug mode

In VS Code:
1. Use the `.vscode/launch.json` configuration for debugging tests
2. Set breakpoints in analyzer code

### Useful Resources

- [Roslyn Analyzer Tutorial](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/tutorials/how-to-write-csharp-analyzer-code-fix)
- [Syntax Visualizer](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/syntax-visualizer)
- [Roslyn Quoter](https://roslynquoter.azurewebsites.net/) - Generate syntax tree code
- [SharpLab](https://sharplab.io/) - View syntax trees and IL

---

## Questions?

- Open a [GitHub Issue](https://github.com/ralphpw/Purity.Analyzer/issues)
- Start a [Discussion](https://github.com/ralphpw/Purity.Analyzer/discussions)

---

## License

By contributing, you agree that your contributions will be licensed under the [MIT License](../LICENSE).
