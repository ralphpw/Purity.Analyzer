using System.Collections.Immutable;
using System.Linq;
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

    /// <summary>
    /// Types whose members perform I/O operations and violate purity.
    /// </summary>
    private static readonly string[] IoTypePatterns =
    [
        "System.Console",
        "System.IO.File",
        "System.IO.Directory",
        "System.IO.StreamReader",
        "System.IO.StreamWriter",
        "System.IO.FileStream",
        "System.IO.BinaryReader",
        "System.IO.BinaryWriter",
        "System.Net.Http.HttpClient",
        "System.Net.WebRequest",
        "System.Net.WebClient",
    ];

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
        CheckForIoOperations(context, methodDeclaration, methodSymbol);
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
    /// PUR003: Detects I/O operations within pure methods.
    /// Catches calls to Console, File, Directory, Stream, and network types.
    /// </summary>
    private static void CheckForIoOperations(
        SyntaxNodeAnalysisContext context,
        MethodDeclarationSyntax method,
        IMethodSymbol methodSymbol)
    {
        foreach (var invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var calledSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;

            if (calledSymbol is null)
                continue;

            if (!IsIoOperation(calledSymbol))
                continue;

            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.PUR003,
                invocation.GetLocation(),
                methodSymbol.Name,
                calledSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat));

            context.ReportDiagnostic(diagnostic);
        }

        // Also check object creations (e.g., new StreamReader("file.txt"))
        foreach (var creation in method.DescendantNodes().OfType<ObjectCreationExpressionSyntax>())
        {
            var typeSymbol = context.SemanticModel.GetTypeInfo(creation).Type;

            if (typeSymbol is null)
                continue;

            var typeName = typeSymbol.ToDisplayString();

            if (!IoTypePatterns.Any(pattern => typeName.StartsWith(pattern)))
                continue;

            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.PUR003,
                creation.GetLocation(),
                methodSymbol.Name,
                $"new {typeSymbol.Name}()");

            context.ReportDiagnostic(diagnostic);
        }
    }

    /// <summary>
    /// Determines whether a method call is an I/O operation.
    /// </summary>
    private static bool IsIoOperation(IMethodSymbol method)
    {
        var containingType = method.ContainingType?.ToDisplayString();

        if (containingType is null)
            return false;

        return IoTypePatterns.Any(pattern => containingType.StartsWith(pattern));
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
