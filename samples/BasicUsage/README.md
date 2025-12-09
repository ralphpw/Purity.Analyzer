# Purity.Analyzer - Basic Usage Demo

This sample project demonstrates how to use **Purity.Analyzer** to enforce functional purity in C#.

## Running the Demo

```bash
cd samples/BasicUsage
dotnet restore
dotnet build
dotnet run
```

## What's Included

### ✅ Pure Examples (`PureExamples.cs`)

Valid pure methods that compile without warnings:
- Simple calculations
- String transformations
- Pure recursion
- Manual loops (no mutation)

All these methods are guaranteed to have no side effects.

### ❌ Violation Examples (`ViolationDemos.cs`)

Common purity violations with **fixes shown in comments**. This file is excluded from build by default.

Demonstrates:
- **PUR001**: Field mutation → Fix: Return computed values
- **PUR003**: I/O operations → Fix: Return data, let caller handle I/O
- **PUR004**: Non-deterministic APIs → Fix: Dependency injection
- **PUR005**: Mutable return types → Fix: Use ImmutableArray
- **PUR006**: Parameter mutation → Fix: Return new arrays
- **PUR007**: ref/out parameters → Fix: Return tuples
- **PUR009**: Reflection → Fix: Use generics
- **PUR010**: Exception control flow → Fix: Use TryParse patterns

**To see all violations:**
1. Open `BasicUsage.csproj`
2. Remove the `<Compile Remove="ViolationDemos.cs" />` line
3. Run `dotnet build`
4. Observe 8+ compiler errors with diagnostic IDs and fix suggestions

## Testing Local NuGet Package

To test the local `.nupkg` before publishing:

```bash
# From repository root
dotnet pack src/Purity.Analyzer.Roslyn/Purity.Analyzer.csproj -c Release -o ./nupkg
dotnet pack src/Purity.Analyzer.Attributes/Purity.Analyzer.Attributes.csproj -c Release -o ./nupkg

# Clear cache and restore
dotnet nuget locals all --clear
cd samples/BasicUsage
dotnet restore --force-evaluate
dotnet build
```

## Learn More

- [Full Documentation](../../README.md)
- [Rules Reference](../../docs/Rules.md)
- [Whitelist Guide](../../docs/Whitelist.md)
