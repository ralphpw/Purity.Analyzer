# TASK-003: Set Up Roslyn Analyzer Skeleton

**Priority:** P0  
**Estimate:** 2 hours  
**Status:** Not Started  
**Depends On:** TASK-001, TASK-002

---

## Description

Create the basic Roslyn analyzer infrastructure that detects `[EnforcedPure]` and `[Pure]` attributes on methods.

---

## Deliverables

### 1. PurityAnalyzer.cs

```csharp
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Purity.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class PurityAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(/* diagnostic descriptors */);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            
            context.RegisterSyntaxNodeAction(
                AnalyzeMethod,
                SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);
            
            if (methodSymbol is null)
                return;
            
            if (!IsPureMethod(methodSymbol))
                return;
            
            // Placeholder: future diagnostics will be checked here
        }

        private static bool IsPureMethod(IMethodSymbol method)
        {
            // Check method attributes
            if (HasPurityAttribute(method))
                return true;
            
            // Check containing type attributes
            if (method.ContainingType is { } containingType && HasPurityAttribute(containingType))
                return true;
            
            return false;
        }

        private static bool HasPurityAttribute(ISymbol symbol)
        {
            foreach (var attribute in symbol.GetAttributes())
            {
                var name = attribute.AttributeClass?.ToDisplayString();
                if (name is "Purity.Contracts.EnforcedPureAttribute" 
                    or "System.Diagnostics.Contracts.PureAttribute")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
```

### 2. DiagnosticDescriptors.cs

```csharp
using Microsoft.CodeAnalysis;

namespace Purity.Analyzer
{
    public static class DiagnosticDescriptors
    {
        private const string Category = "Purity";

        public static readonly DiagnosticDescriptor PUR001 = new(
            id: "PUR001",
            title: "Pure method mutates field",
            messageFormat: "Method '{0}' marked as pure mutates field '{1}'",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        // Add other descriptors as they're implemented...
    }
}
```

---

## Acceptance Criteria

- [ ] Analyzer compiles
- [ ] Analyzer is loaded when referenced by a project
- [ ] `IsPureMethod` correctly detects `[EnforcedPure]` attribute
- [ ] `IsPureMethod` correctly detects `[Pure]` attribute
- [ ] `IsPureMethod` detects attribute on containing type
- [ ] Analyzer uses concurrent execution
- [ ] Analyzer skips generated code

---

## Technical Notes

### Attribute Detection

Must detect:
1. `[EnforcedPure]` on method
2. `[Pure]` on method (BCL attribute)
3. `[EnforcedPure]` on containing class/struct (applies to all methods)
4. `[Pure]` on containing class/struct

### Performance Considerations

- Use `EnableConcurrentExecution()` for parallel analysis
- Cache attribute lookups where possible
- Skip generated code with `ConfigureGeneratedCodeAnalysis`

### Project Reference

The analyzer project should reference the attributes project:

```xml
<ItemGroup>
  <ProjectReference Include="..\Purity.Analyzer.Attributes\Purity.Analyzer.Attributes.csproj" />
</ItemGroup>
```

---

## Dependencies

- TASK-001 (project structure)
- TASK-002 (attribute definition)

---

## Blocks

- TASK-005 through TASK-011 (all diagnostics build on this skeleton)
