using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Purity.Analyzer;

/// <summary>
/// Roslyn analyzer that enforces functional purity for methods marked with
/// [EnforcedPure] or [Pure] attributes.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PurityAnalyzer : DiagnosticAnalyzer
{
    private const string EnforcedPureAttributeFullName = "Purity.Contracts.EnforcedPureAttribute";
    private const string BclPureAttributeFullName = "System.Diagnostics.Contracts.PureAttribute";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
    [
        DiagnosticDescriptors.PUR001,
        DiagnosticDescriptors.PUR002,
        DiagnosticDescriptors.PUR003,
        DiagnosticDescriptors.PUR004,
        DiagnosticDescriptors.PUR005
    ];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(
            AnalyzeMethod,
            SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;
        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);

        if (methodSymbol is null)
            return;

        if (!IsPureMethod(methodSymbol))
            return;

        CheckForFieldMutation(context, methodDeclaration, methodSymbol);
    }

    /// <summary>
    /// PUR001: Detects field mutations within pure methods.
    /// Catches assignments, increments, decrements, and compound assignments to fields.
    /// </summary>
    private static void CheckForFieldMutation(
        SyntaxNodeAnalysisContext context,
        MethodDeclarationSyntax method,
        IMethodSymbol methodSymbol)
    {
        foreach (var descendant in method.DescendantNodes())
        {
            switch (descendant)
            {
                case AssignmentExpressionSyntax assignment:
                    CheckFieldMutationTarget(context, assignment.Left, methodSymbol);
                    break;

                case PrefixUnaryExpressionSyntax prefix
                    when prefix.IsKind(SyntaxKind.PreIncrementExpression)
                      || prefix.IsKind(SyntaxKind.PreDecrementExpression):
                    CheckFieldMutationTarget(context, prefix.Operand, methodSymbol);
                    break;

                case PostfixUnaryExpressionSyntax postfix
                    when postfix.IsKind(SyntaxKind.PostIncrementExpression)
                      || postfix.IsKind(SyntaxKind.PostDecrementExpression):
                    CheckFieldMutationTarget(context, postfix.Operand, methodSymbol);
                    break;
            }
        }
    }

    /// <summary>
    /// Reports PUR001 if the target expression is a field symbol.
    /// </summary>
    private static void CheckFieldMutationTarget(
        SyntaxNodeAnalysisContext context,
        ExpressionSyntax target,
        IMethodSymbol methodSymbol)
    {
        var symbol = context.SemanticModel.GetSymbolInfo(target).Symbol;

        if (symbol is not IFieldSymbol fieldSymbol)
            return;

        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.PUR001,
            target.GetLocation(),
            methodSymbol.Name,
            fieldSymbol.Name);

        context.ReportDiagnostic(diagnostic);
    }

    /// <summary>
    /// Determines whether a method should be analyzed for purity violations.
    /// A method is considered pure if it or its containing type has [EnforcedPure] or [Pure].
    /// </summary>
    private static bool IsPureMethod(IMethodSymbol method)
    {
        // Check method-level attributes first (more specific)
        if (HasPurityAttribute(method))
            return true;

        // Check containing type attributes (class/struct-level enforcement)
        if (method.ContainingType is { } containingType && HasPurityAttribute(containingType))
            return true;

        return false;
    }

    /// <summary>
    /// Checks whether a symbol has a purity-enforcing attribute.
    /// Recognizes both [EnforcedPure] and BCL [Pure] attributes.
    /// </summary>
    private static bool HasPurityAttribute(ISymbol symbol)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            var attributeName = attribute.AttributeClass?.ToDisplayString();

            if (attributeName is EnforcedPureAttributeFullName or BclPureAttributeFullName)
                return true;
        }

        return false;
    }
}
