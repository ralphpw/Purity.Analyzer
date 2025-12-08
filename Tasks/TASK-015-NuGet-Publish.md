# TASK-015: Package and Publish to NuGet

**Priority:** P0  
**Estimate:** 2 hours  
**Status:** Not Started  
**Depends On:** TASK-001 through TASK-009

---

## Description

Configure NuGet packaging and publish to NuGet.org.

---

## Deliverables

### 1. Analyzer Package Configuration

`Purity.Analyzer.csproj`:
```xml
<PropertyGroup>
  <PackageId>Purity.Analyzer</PackageId>
  <Version>0.1.0</Version>
  <Authors>ralphpw</Authors>
  <Description>Compile-time verification of functional purity in C#</Description>
  <PackageLicenseExpression>MIT</PackageLicenseExpression>
  <PackageProjectUrl>https://github.com/ralphpw/Purity.Analyzer</PackageProjectUrl>
  <RepositoryUrl>https://github.com/ralphpw/Purity.Analyzer</RepositoryUrl>
  <PackageTags>roslyn;analyzer;purity;functional;immutable;csharp</PackageTags>
  <PackageReadmeFile>README.md</PackageReadmeFile>
  <DevelopmentDependency>true</DevelopmentDependency>
  <NoPackageAnalysis>true</NoPackageAnalysis>
  <IncludeBuildOutput>false</IncludeBuildOutput>
</PropertyGroup>

<ItemGroup>
  <None Include="..\..\README.md" Pack="true" PackagePath="\" />
  <None Include="$(OutputPath)\$(AssemblyName).dll" Pack="true" PackagePath="analyzers/dotnet/cs" />
</ItemGroup>
```

### 2. Build and Pack

```bash
dotnet build -c Release
dotnet pack -c Release
```

### 3. Publish

```bash
dotnet nuget push ./bin/Release/Purity.Analyzer.0.1.0.nupkg -k <API_KEY> -s https://api.nuget.org/v3/index.json
```

### 4. GitHub Release

- Tag: `v0.1.0`
- Title: `Purity.Analyzer v0.1.0`
- Release notes with changelog

---

## Acceptance Criteria

- [ ] `dotnet add package Purity.Analyzer` installs successfully
- [ ] Analyzer runs automatically on installation
- [ ] Attributes package available separately
- [ ] GitHub release created with tag

---

## Dependencies

- All P0 tasks (TASK-001 through TASK-009, TASK-012)
