# TASK-004: Set Up Unit Test Infrastructure

**Priority:** P0  
**Estimate:** 2 hours  
**Status:** Not Started  
**Depends On:** TASK-001, TASK-003

---

## Description

Set up the test project with Roslyn analyzer testing infrastructure and write initial smoke tests.

---

## Deliverables

### 1. Test Project Configuration

`Purity.Analyzer.Tests.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="NUnit" Version="3.14.0" />
    <PackageReference Include="NUnit3TestAdapter" Version="4.5.0" />
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.NUnit" Version="1.1.1" />
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp.Workspaces" Version="4.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\Purity.Analyzer\Purity.Analyzer.csproj" />
    <ProjectReference Include="..\..\src\Purity.Analyzer.Attributes\Purity.Analyzer.Attributes.csproj" />
  </ItemGroup>
</Project>
```

### 2. Test Verifier Helper

`Verifiers/CSharpAnalyzerVerifier.cs`:

```csharp
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Purity.Analyzer.Tests.Verifiers;

public static class CSharpAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
        => CSharpAnalyzerVerifier<TAnalyzer, NUnitVerifier>.Diagnostic(descriptor);

    public static DiagnosticResult Diagnostic(string diagnosticId)
        => CSharpAnalyzerVerifier<TAnalyzer, NUnitVerifier>.Diagnostic(diagnosticId);

    public static async Task VerifyAnalyzerAsync(
        string source,
        params DiagnosticResult[] expected)
    {
        var test = new Test
        {
            TestCode = source,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };
        
        test.TestState.AdditionalReferences.Add(
            typeof(Purity.Contracts.EnforcedPureAttribute).Assembly);
        
        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync(CancellationToken.None);
    }

    private sealed class Test : CSharpAnalyzerTest<TAnalyzer, NUnitVerifier>
    {
        public Test()
        {
            SolutionTransforms.Add((solution, projectId) =>
            {
                var compilationOptions = solution.GetProject(projectId)?.CompilationOptions;
                if (compilationOptions is not null)
                {
                    compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                        compilationOptions.SpecificDiagnosticOptions.SetItems(
                            CSharpVerifierHelper.NullableWarnings));
                    solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);
                }
                return solution;
            });
        }
    }
}

internal static class CSharpVerifierHelper
{
    internal static ImmutableDictionary<string, ReportDiagnostic> NullableWarnings { get; }
        = GetNullableWarningsFromCompiler();

    private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
    {
        string[] args = { "/warnaserror:nullable" };
        var commandLineArguments = CSharpCommandLineParser.Default.Parse(
            args,
            baseDirectory: Environment.CurrentDirectory,
            sdkDirectory: Environment.CurrentDirectory);
        return commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;
    }
}
```

### 3. Smoke Test

`SmokeTests.cs`:

```csharp
using NUnit.Framework;
using Purity.Analyzer.Tests.Verifiers;

namespace Purity.Analyzer.Tests;

[TestFixture]
public class SmokeTests
{
    [Test]
    public async Task Analyzer_LoadsSuccessfully()
    {
        var code = @"
using Purity.Contracts;

public class Test
{
    [EnforcedPure]
    public int Add(int a, int b) => a + b;
}";
        
        // No diagnostics expected for valid pure code
        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Test]
    public async Task Analyzer_DetectsEnforcedPureAttribute()
    {
        var code = @"
using Purity.Contracts;

public class Test
{
    [EnforcedPure]
    public int Method() => 42;
}";
        
        // Analyzer should load and not crash
        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Test]
    public async Task Analyzer_DetectsPureAttribute()
    {
        var code = @"
using System.Diagnostics.Contracts;

public class Test
{
    [Pure]
    public int Method() => 42;
}";
        
        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }

    [Test]
    public async Task Analyzer_DetectsAttributeOnClass()
    {
        var code = @"
using Purity.Contracts;

[EnforcedPure]
public class Test
{
    public int Method() => 42;
}";
        
        await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
    }
}
```

---

## Acceptance Criteria

- [ ] `dotnet test` runs and passes
- [ ] Test verifier correctly references Purity.Contracts
- [ ] Tests can verify diagnostics are (or aren't) reported
- [ ] Smoke tests verify analyzer loads for all attribute scenarios

---

## Technical Notes

### Test Pattern

Each diagnostic will follow this pattern:

```csharp
[Test]
public async Task PURXXX_WhenViolation_ReportsDiagnostic()
{
    var code = @"/* code with violation */";
    
    var expected = CSharpAnalyzerVerifier<PurityAnalyzer>
        .Diagnostic(DiagnosticDescriptors.PURXXX)
        .WithLocation(line, column)
        .WithArguments(/* message args */);
    
    await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code, expected);
}

[Test]
public async Task PURXXX_WhenClean_NoDiagnostic()
{
    var code = @"/* clean code */";
    
    await CSharpAnalyzerVerifier<PurityAnalyzer>.VerifyAnalyzerAsync(code);
}
```

### Reference Assemblies

Tests need access to:
- .NET 8 BCL (via `ReferenceAssemblies.Net.Net80`)
- `Purity.Contracts.EnforcedPureAttribute` (via project reference)
- `System.Diagnostics.Contracts.PureAttribute` (in BCL)

---

## Dependencies

- TASK-001 (project structure)
- TASK-003 (analyzer skeleton to test)

---

## Blocks

- TASK-005 through TASK-011 (all need test infrastructure)
