# TASK-002: Create EnforcedPureAttribute

**Priority:** P0  
**Estimate:** 30 minutes  
**Status:** ✅ Done  
**Depends On:** TASK-001

---

## Description

Define the `[EnforcedPure]` attribute in the `Purity.Analyzer.Attributes` project.

---

## Deliverables

Create `EnforcedPureAttribute.cs`:

```csharp
using System;

namespace Purity.Contracts
{
    /// <summary>
    /// Marks a method, class, or struct as pure. The Purity.Analyzer verifies
    /// that code marked with this attribute is free from side effects, mutation,
    /// and non-deterministic behavior.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When applied to a method, that method must be pure.
    /// When applied to a class or struct, all methods in that type must be pure.
    /// </para>
    /// <para>
    /// Methods marked with [EnforcedPure] in referenced assemblies are trusted
    /// without re-analysis, under the assumption they were verified at compile time.
    /// </para>
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct,
        Inherited = false,
        AllowMultiple = false)]
    public sealed class EnforcedPureAttribute : Attribute
    {
    }
}
```

---

## Acceptance Criteria

- [ ] Attribute compiles
- [ ] Attribute can be referenced from test project
- [ ] Attribute can be applied to methods, classes, and structs
- [ ] NuGet package builds successfully
- [ ] XML documentation is included

---

## Technical Notes

### Package Configuration

In `Purity.Analyzer.Attributes.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    
    <!-- NuGet Package -->
    <PackageId>Purity.Analyzer.Attributes</PackageId>
    <Version>0.1.0</Version>
    <Authors>ralphpw</Authors>
    <Description>Attributes for Purity.Analyzer - compile-time verification of functional purity in C#</Description>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <PackageProjectUrl>https://github.com/ralphpw/Purity.Analyzer</PackageProjectUrl>
    <RepositoryUrl>https://github.com/ralphpw/Purity.Analyzer</RepositoryUrl>
    <PackageTags>roslyn;analyzer;purity;functional;immutable</PackageTags>
  </PropertyGroup>
</Project>
```

### Usage Example

```csharp
using Purity.Contracts;

public class Calculator
{
    [EnforcedPure]
    public int Add(int a, int b) => a + b;
}

[EnforcedPure]  // All methods must be pure
public class PureService
{
    public int Double(int x) => x * 2;
    public int Triple(int x) => x * 3;
}
```

---

## Dependencies

- TASK-001 (repository structure)

---

## Blocks

- TASK-003 (analyzer needs to detect this attribute)
- TASK-005 through TASK-011 (all diagnostics check for this attribute)
