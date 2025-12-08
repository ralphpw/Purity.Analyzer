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

    /// <summary>
    /// Members that return non-deterministic values (DateTime.Now, Guid.NewGuid, etc.).
    /// </summary>
    private static readonly string[] NonDeterministicMembers =
    [
        "System.DateTime.Now",
        "System.DateTime.UtcNow",
        "System.DateTime.Today",
        "System.DateTimeOffset.Now",
        "System.DateTimeOffset.UtcNow",
        "System.Guid.NewGuid",
        "System.Environment.TickCount",
        "System.Environment.TickCount64",
        "System.Environment.ProcessId",
    ];

    /// <summary>
    /// Types that are inherently non-deterministic (Random).
    /// </summary>
    private static readonly string[] NonDeterministicTypes =
    [
        "System.Random",
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
        CheckForNonPureCalls(context, methodDeclaration, methodSymbol);
        CheckForIoOperations(context, methodDeclaration, methodSymbol);
        CheckForNonDeterministicApis(context, methodDeclaration, methodSymbol);
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
    /// PUR002: Detects calls to non-pure methods within pure methods.
    /// A method is considered pure if it has [EnforcedPure], [Pure], or is in the BCL whitelist.
    /// Skips invocations already covered by PUR003 (I/O) or PUR004 (non-deterministic).
    /// </summary>
    private static void CheckForNonPureCalls(
        SyntaxNodeAnalysisContext context,
        MethodDeclarationSyntax method,
        IMethodSymbol methodSymbol)
    {
        foreach (var invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var calledSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;

            if (calledSymbol is null)
                continue;

            if (IsCalledMethodPure(calledSymbol))
                continue;

            // Skip if already covered by PUR003 (I/O) or PUR004 (non-deterministic)
            if (IsIoOperation(calledSymbol) || IsNonDeterministicMethod(calledSymbol))
                continue;

            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.PUR002,
                invocation.GetLocation(),
                methodSymbol.Name,
                calledSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat));

            context.ReportDiagnostic(diagnostic);
        }

        // Also check property getters (member access without invocation)
        foreach (var memberAccess in method.DescendantNodes().OfType<MemberAccessExpressionSyntax>())
        {
            // Skip if this is part of an invocation (already handled above)
            if (memberAccess.Parent is InvocationExpressionSyntax)
                continue;

            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol;

            // Check property getters
            if (symbol is IPropertySymbol propertySymbol)
            {
                if (propertySymbol.GetMethod is { } getter && !IsCalledMethodPure(getter))
                {
                    // Skip if property is whitelisted (constants like Math.PI)
                    var propertySignature = BuildPropertySignature(propertySymbol);
                    if (PurityWhitelist.IsWhitelisted(propertySignature))
                        continue;

                    // Skip if already covered by PUR004 (non-deterministic property)
                    if (IsNonDeterministicProperty(propertySymbol))
                        continue;

                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptors.PUR002,
                        memberAccess.GetLocation(),
                        methodSymbol.Name,
                        propertySymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat));

                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }

    /// <summary>
    /// Determines whether a called method is pure (has purity attribute or is whitelisted).
    /// </summary>
    private static bool IsCalledMethodPure(IMethodSymbol method)
    {
        // Check for purity attributes on the method itself
        if (HasPurityAttribute(method))
            return true;

        // Check for purity attributes on the containing type
        if (method.ContainingType is { } containingType && HasPurityAttribute(containingType))
            return true;

        // Check BCL whitelist with full signature
        var signature = BuildMethodSignature(method);
        if (PurityWhitelist.IsWhitelisted(signature))
            return true;

        return false;
    }

    /// <summary>
    /// Symbol display format for CLR-style signatures (used for whitelist matching).
    /// Uses metadata names (System.Int32 not int, System.String not string).
    /// </summary>
    private static readonly SymbolDisplayFormat ClrSignatureFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.None,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

    /// <summary>
    /// Builds a CLR-format method signature for whitelist lookup.
    /// Format: Namespace.Type.Method(ParamType1,ParamType2)
    /// Generic methods use backtick notation: Method`1
    /// </summary>
    private static string BuildMethodSignature(IMethodSymbol method)
    {
        var containingType = GetClrTypeName(method.ContainingType);
        var methodName = method.Name;

        // Add generic arity if method is generic
        if (method.Arity > 0)
        {
            methodName += "`" + method.Arity;
        }

        // Build parameter list using CLR type names
        var parameters = string.Join(",", method.Parameters.Select(p =>
        {
            var typeName = GetClrTypeName(p.Type);
            // Handle ref/out parameters
            if (p.RefKind == RefKind.Ref)
                typeName += "&";
            else if (p.RefKind == RefKind.Out)
                typeName += "@";
            return typeName;
        }));

        return $"{containingType}.{methodName}({parameters})";
    }

    /// <summary>
    /// Gets the CLR-style type name (System.Int32 not int, etc.).
    /// </summary>
    private static string GetClrTypeName(ITypeSymbol? type)
    {
        if (type is null)
            return string.Empty;

        // Use metadata name for special types to get System.Int32 instead of int
        if (type.SpecialType != SpecialType.None)
        {
            return type.ContainingNamespace?.ToDisplayString() + "." + type.MetadataName;
        }

        // For generic types, build the name with generic arity
        if (type is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            var baseName = type.ContainingNamespace?.ToDisplayString() + "." + type.MetadataName;
            return baseName;
        }

        return type.ToDisplayString();
    }

    /// <summary>
    /// Builds a signature for property lookup (e.g., Math.PI).
    /// </summary>
    private static string BuildPropertySignature(IPropertySymbol property)
    {
        var containingType = GetClrTypeName(property.ContainingType);
        return $"{containingType}.{property.Name}";
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
    /// Determines whether a method call is non-deterministic (e.g., Guid.NewGuid).
    /// Used to skip PUR002 when PUR004 will report a more specific diagnostic.
    /// </summary>
    private static bool IsNonDeterministicMethod(IMethodSymbol method)
    {
        var containingType = method.ContainingType?.ToDisplayString();

        if (containingType is null)
            return false;

        // Check if the containing type is inherently non-deterministic (e.g., Random)
        if (NonDeterministicTypes.Any(t => containingType == t))
            return true;

        // Check if this specific method is in the non-deterministic members list
        var fullName = containingType + "." + method.Name;
        return NonDeterministicMembers.Contains(fullName);
    }

    /// <summary>
    /// Determines whether a property is non-deterministic (e.g., DateTime.Now).
    /// Used to skip PUR002 when PUR004 will report a more specific diagnostic.
    /// </summary>
    private static bool IsNonDeterministicProperty(IPropertySymbol property)
    {
        var containingType = property.ContainingType?.ToDisplayString();

        if (containingType is null)
            return false;

        var fullName = containingType + "." + property.Name;
        return NonDeterministicMembers.Contains(fullName);
    }

    /// <summary>
    /// PUR004: Detects non-deterministic API usage within pure methods.
    /// Catches DateTime.Now, Guid.NewGuid, Random, Environment.TickCount, etc.
    /// </summary>
    private static void CheckForNonDeterministicApis(
        SyntaxNodeAnalysisContext context,
        MethodDeclarationSyntax method,
        IMethodSymbol methodSymbol)
    {
        foreach (var node in method.DescendantNodes())
        {
            switch (node)
            {
                case MemberAccessExpressionSyntax memberAccess:
                    CheckNonDeterministicMemberAccess(context, memberAccess, methodSymbol);
                    break;

                case ObjectCreationExpressionSyntax creation:
                    CheckNonDeterministicObjectCreation(context, creation, methodSymbol);
                    break;

                case InvocationExpressionSyntax invocation:
                    CheckNonDeterministicInvocation(context, invocation, methodSymbol);
                    break;
            }
        }
    }

    /// <summary>
    /// Checks if a member access (property/method) is non-deterministic.
    /// </summary>
    private static void CheckNonDeterministicMemberAccess(
        SyntaxNodeAnalysisContext context,
        MemberAccessExpressionSyntax memberAccess,
        IMethodSymbol methodSymbol)
    {
        var symbol = context.SemanticModel.GetSymbolInfo(memberAccess).Symbol;

        if (symbol is null)
            return;

        var fullName = symbol.ContainingType?.ToDisplayString() + "." + symbol.Name;

        if (!NonDeterministicMembers.Contains(fullName))
            return;

        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.PUR004,
            memberAccess.GetLocation(),
            methodSymbol.Name,
            fullName);

        context.ReportDiagnostic(diagnostic);
    }

    /// <summary>
    /// Checks if an object creation is non-deterministic (e.g., new Random()).
    /// </summary>
    private static void CheckNonDeterministicObjectCreation(
        SyntaxNodeAnalysisContext context,
        ObjectCreationExpressionSyntax creation,
        IMethodSymbol methodSymbol)
    {
        var typeSymbol = context.SemanticModel.GetTypeInfo(creation).Type;

        if (typeSymbol is null)
            return;

        var typeName = typeSymbol.ToDisplayString();

        if (!NonDeterministicTypes.Contains(typeName))
            return;

        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.PUR004,
            creation.GetLocation(),
            methodSymbol.Name,
            $"new {typeSymbol.Name}()");

        context.ReportDiagnostic(diagnostic);
    }

    /// <summary>
    /// Checks if a method invocation on a non-deterministic type (e.g., random.Next()).
    /// </summary>
    private static void CheckNonDeterministicInvocation(
        SyntaxNodeAnalysisContext context,
        InvocationExpressionSyntax invocation,
        IMethodSymbol methodSymbol)
    {
        var calledSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;

        if (calledSymbol is null)
            return;

        var containingType = calledSymbol.ContainingType?.ToDisplayString();

        if (containingType is null || !NonDeterministicTypes.Contains(containingType))
            return;

        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.PUR004,
            invocation.GetLocation(),
            methodSymbol.Name,
            calledSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat));

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
