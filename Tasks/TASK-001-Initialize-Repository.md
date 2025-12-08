# TASK-001: Initialize Repository

**Priority:** P0  
**Estimate:** 1 hour  
**Status:** ✅ Done

---

## Description

Set up the .NET solution structure and CI pipeline.

### Deliverables

1. Create .NET solution with three projects:
   - `Purity.Analyzer` (analyzer)
   - `Purity.Analyzer.Attributes` (attributes)
   - `Purity.Analyzer.Tests` (unit tests)

2. Configure project files:
   - Target frameworks (netstandard2.0 for analyzer/attributes, net8.0 for tests)
   - NuGet package references
   - Analyzer packaging configuration

3. Add repository files:
   - `.gitignore` (already exists)
   - `LICENSE` (already exists - MIT)
   - `README.md` (already exists)

4. Set up GitHub Actions CI:
   - Build on push to main
   - Run tests
   - Build NuGet packages

---

## Acceptance Criteria

- [ ] `dotnet build` succeeds
- [ ] `dotnet test` runs (even if no tests yet)
- [ ] CI workflow passes on main branch
- [ ] Solution structure matches architecture spec

---

## Technical Notes

### Solution Structure

```
Purity.Analyzer/
├── src/
│   ├── Purity.Analyzer/
│   │   └── Purity.Analyzer.csproj
│   └── Purity.Analyzer.Attributes/
│       └── Purity.Analyzer.Attributes.csproj
├── tests/
│   └── Purity.Analyzer.Tests/
│       └── Purity.Analyzer.Tests.csproj
└── Purity.Analyzer.sln
```

### Analyzer .csproj Template

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>
    <IsPackable>true</IsPackable>
    <IncludeBuildOutput>false</IncludeBuildOutput>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.0.0" PrivateAssets="all" />
    <PackageReference Include="Microsoft.CodeAnalysis.Analyzers" Version="3.3.4" PrivateAssets="all" />
  </ItemGroup>
  
  <ItemGroup>
    <None Include="$(OutputPath)\$(AssemblyName).dll" Pack="true" PackagePath="analyzers/dotnet/cs" Visible="false" />
  </ItemGroup>
</Project>
```

### GitHub Actions Workflow

```yaml
name: CI
on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: 8.0.x
    - name: Restore
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test
      run: dotnet test --no-build --verbosity normal
```

---

## Dependencies

None (first task)

---

## Blocks

- TASK-002 (needs Attributes project)
- TASK-003 (needs Analyzer project)
- TASK-004 (needs Tests project)
