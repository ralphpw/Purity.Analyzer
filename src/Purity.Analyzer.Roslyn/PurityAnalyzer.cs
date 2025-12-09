using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Purity.Analyzer.Configuration;

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
        ImmutableArray.Create(
            DiagnosticDescriptors.PUR001,
            DiagnosticDescriptors.PUR002,
            DiagnosticDescriptors.PUR003,
            DiagnosticDescriptors.PUR004,
            DiagnosticDescriptors.PUR005,
            DiagnosticDescriptors.PUR006,
            DiagnosticDescriptors.PUR007,
            DiagnosticDescriptors.PUR008,
            DiagnosticDescriptors.PUR009,
            DiagnosticDescriptors.PUR010,
            DiagnosticDescriptors.PUR011
        );

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Use compilation start to load config once per compilation
        context.RegisterCompilationStartAction(compilationContext =>
        {
            var config = ConfigurationLoader.Load(compilationContext.Options.AdditionalFiles);

            compilationContext.RegisterSyntaxNodeAction(
                nodeContext => AnalyzeMethod(nodeContext, config),
                SyntaxKind.MethodDeclaration);
        });
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context, PurityConfig config)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;
        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);

        if (methodSymbol is null)
            return;

        if (!IsPureMethod(methodSymbol))
            return;

        CheckForFieldMutation(context, methodDeclaration, methodSymbol);
        CheckForNonPureCalls(context, methodDeclaration, methodSymbol, config);
        CheckForIoOperations(context, methodDeclaration, methodSymbol);
        CheckForNonDeterministicApis(context, methodDeclaration, methodSymbol);
        CheckForMutableReturnType(context, methodDeclaration, methodSymbol);
        CheckForParameterMutation(context, methodDeclaration, methodSymbol);
        CheckForRefOutParameters(context, methodSymbol);
        CheckForUnsafeCode(context, methodDeclaration, methodSymbol);
        CheckForReflection(context, methodDeclaration, methodSymbol);
        CheckForExceptionControlFlow(context, methodDeclaration, methodSymbol);
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
    /// Also emits PUR011 for methods marked for review.
    /// </summary>
    private static void CheckForNonPureCalls(
        SyntaxNodeAnalysisContext context,
        MethodDeclarationSyntax method,
        IMethodSymbol methodSymbol,
        PurityConfig config)
    {
        foreach (var invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var calledSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;

            if (calledSymbol is null)
                continue;

            var signature = BuildMethodSignature(calledSymbol);
            var purityResult = IsCalledMethodPure(calledSymbol, signature, config, context.Compilation);

            // Check for review-required methods (PUR011)
            if (purityResult.IsPure && purityResult.IsReviewRequired)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.PUR011,
                    invocation.GetLocation(),
                    methodSymbol.Name,
                    calledSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            }

            if (purityResult.IsPure)
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
                if (propertySymbol.GetMethod is { } getter)
                {
                    var getterSignature = BuildMethodSignature(getter);
                    var getterPurity = IsCalledMethodPure(getter, getterSignature, config, context.Compilation);

                    // Check for review-required (PUR011)
                    if (getterPurity.IsPure && getterPurity.IsReviewRequired)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            DiagnosticDescriptors.PUR011,
                            memberAccess.GetLocation(),
                            methodSymbol.Name,
                            propertySymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
                    }

                    if (getterPurity.IsPure)
                        continue;

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
    /// Result of purity check including whether the method is marked for review.
    /// </summary>
    private readonly struct PurityCheckResult
    {
        public bool IsPure { get; }
        public bool IsReviewRequired { get; }

        private PurityCheckResult(bool isPure, bool isReviewRequired)
        {
            IsPure = isPure;
            IsReviewRequired = isReviewRequired;
        }

        public static PurityCheckResult Pure() => new(true, false);
        public static PurityCheckResult PureWithReview() => new(true, true);
        public static PurityCheckResult NotPure() => new(false, false);
    }

    /// <summary>
    /// Determines whether a called method is pure based on attributes, whitelist, and config.
    /// </summary>
    private static PurityCheckResult IsCalledMethodPure(
        IMethodSymbol method,
        string signature,
        PurityConfig config,
        Compilation compilation)
    {
        // User exclude always wins - method is NOT pure
        if (PatternMatcher.MatchesAny(signature, config.Whitelist.Exclude))
            return PurityCheckResult.NotPure();

        // User include is always trusted
        if (PatternMatcher.MatchesAny(signature, config.Whitelist.Include))
        {
            var isReviewRequired = config.ReviewRequired.Contains(signature)
                                   || PatternMatcher.MatchesAny(signature, config.ReviewRequired);
            return isReviewRequired ? PurityCheckResult.PureWithReview() : PurityCheckResult.Pure();
        }

        // Apply trust mode
        return config.TrustMode switch
        {
            TrustMode.Standard => IsMethodPureStandard(method, signature),
            TrustMode.Strict => IsMethodPureStrict(method, signature, compilation),
            TrustMode.ZeroTrust => PurityCheckResult.NotPure(), // Only user whitelist trusted
            _ => IsMethodPureStandard(method, signature)
        };
    }

    /// <summary>
    /// Standard mode: trusts BCL whitelist and [EnforcedPure] in any assembly.
    /// </summary>
    private static PurityCheckResult IsMethodPureStandard(IMethodSymbol method, string signature)
    {
        // Check for purity attributes on the method itself
        if (HasPurityAttribute(method))
            return PurityCheckResult.Pure();

        // Check for purity attributes on the containing type
        if (method.ContainingType is { } containingType && HasPurityAttribute(containingType))
            return PurityCheckResult.Pure();

        // Check BCL whitelist with full signature
        if (PurityWhitelist.IsWhitelisted(signature))
            return PurityCheckResult.Pure();

        return PurityCheckResult.NotPure();
    }

    /// <summary>
    /// Strict mode: trusts BCL whitelist and [EnforcedPure] only in current compilation.
    /// </summary>
    private static PurityCheckResult IsMethodPureStrict(
        IMethodSymbol method,
        string signature,
        Compilation compilation)
    {
        // BCL whitelist is trusted
        if (PurityWhitelist.IsWhitelisted(signature))
            return PurityCheckResult.Pure();

        // [EnforcedPure] only trusted if in current compilation
        if (HasPurityAttribute(method) && IsInCurrentCompilation(method, compilation))
            return PurityCheckResult.Pure();

        // [EnforcedPure] on containing type only trusted if in current compilation
        if (method.ContainingType is { } containingType
            && HasPurityAttribute(containingType)
            && IsInCurrentCompilation(containingType, compilation))
            return PurityCheckResult.Pure();

        return PurityCheckResult.NotPure();
    }

    /// <summary>
    /// Determines if a symbol is defined in the current compilation (not a reference).
    /// </summary>
    private static bool IsInCurrentCompilation(ISymbol symbol, Compilation compilation) =>
        SymbolEqualityComparer.Default.Equals(symbol.ContainingAssembly, compilation.Assembly);

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

    #region PUR005 - Mutable Return Types

    /// <summary>
    /// Maps mutable collection types to their immutable alternatives for diagnostic messages.
    /// </summary>
    private static readonly Dictionary<string, string> MutableToImmutable = new()
    {
        ["System.Collections.Generic.List"] = "ImmutableList<T>",
        ["System.Collections.Generic.Dictionary"] = "ImmutableDictionary<TKey, TValue>",
        ["System.Collections.Generic.HashSet"] = "ImmutableHashSet<T>",
        ["System.Collections.Generic.Queue"] = "ImmutableQueue<T>",
        ["System.Collections.Generic.Stack"] = "ImmutableStack<T>",
        ["System.Collections.Generic.SortedSet"] = "ImmutableSortedSet<T>",
        ["System.Collections.Generic.SortedDictionary"] = "ImmutableSortedDictionary<TKey, TValue>",
        ["System.Collections.Generic.LinkedList"] = "ImmutableList<T>",
        ["System.Collections.Generic.SortedList"] = "ImmutableSortedDictionary<TKey, TValue>",
        ["System.Collections.ArrayList"] = "ImmutableList",
        ["System.Collections.Hashtable"] = "ImmutableDictionary",
    };

    /// <summary>
    /// PUR005: Detects mutable return types in pure methods.
    /// Pure methods should not return mutable collections that callers could modify.
    /// </summary>
    private static void CheckForMutableReturnType(
        SyntaxNodeAnalysisContext context,
        MethodDeclarationSyntax method,
        IMethodSymbol methodSymbol)
    {
        var returnType = methodSymbol.ReturnType;

        // Check for array types
        if (returnType is IArrayTypeSymbol arrayType)
        {
            var elementType = arrayType.ElementType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.PUR005,
                method.ReturnType.GetLocation(),
                methodSymbol.Name,
                returnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                $"ImmutableArray<{elementType}>"));
            return;
        }

        // Check for mutable collection types
        if (returnType is INamedTypeSymbol namedType)
        {
            var originalDef = namedType.OriginalDefinition.ToDisplayString();
            var lookupKey = originalDef.Contains('<') 
                ? originalDef.Substring(0, originalDef.IndexOf('<')) 
                : originalDef;

            if (MutableToImmutable.TryGetValue(lookupKey, out var immutableAlternative))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.PUR005,
                    method.ReturnType.GetLocation(),
                    methodSymbol.Name,
                    returnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                    immutableAlternative));
            }
        }
    }

    #endregion

    #region PUR006 - Parameter Mutation

    /// <summary>
    /// Method name patterns that typically mutate their target object.
    /// </summary>
    private static readonly string[] MutatingMethodNames =
    [
        "Add", "AddRange", "Insert", "InsertRange",
        "Remove", "RemoveAt", "RemoveAll", "RemoveRange", "RemoveWhere",
        "Clear", "Pop", "Push", "Enqueue", "Dequeue",
        "Sort", "Reverse", "Shuffle",
        "Set", "SetValue",
        "TrimExcess", "EnsureCapacity"
    ];

    /// <summary>
    /// PUR006: Detects mutations to parameters within pure methods.
    /// Catches mutating method calls on parameters and property/field assignments.
    /// </summary>
    private static void CheckForParameterMutation(
        SyntaxNodeAnalysisContext context,
        MethodDeclarationSyntax method,
        IMethodSymbol methodSymbol)
    {
        var parameterNames = new HashSet<string>(
            methodSymbol.Parameters
                .Where(p => !p.Type.IsValueType) // Only check reference types
                .Select(p => p.Name));

        if (parameterNames.Count == 0)
            return;

        foreach (var node in method.DescendantNodes())
        {
            switch (node)
            {
                // Check for mutating method calls on parameters: param.Add(x)
                case InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax memberAccess }:
                    CheckParameterMutatingMethodCall(context, memberAccess, parameterNames, methodSymbol);
                    break;

                // Check for property setter assignments: param.Property = value
                case AssignmentExpressionSyntax { Left: MemberAccessExpressionSyntax assignmentTarget }:
                    CheckParameterPropertyAssignment(context, assignmentTarget, parameterNames, methodSymbol);
                    break;

                // Check for indexer assignments: param[i] = value
                case AssignmentExpressionSyntax { Left: ElementAccessExpressionSyntax elementAccess }:
                    CheckParameterIndexerAssignment(context, elementAccess, parameterNames, methodSymbol);
                    break;
            }
        }
    }

    /// <summary>
    /// Checks if an invocation is a mutating method call on a parameter.
    /// </summary>
    private static void CheckParameterMutatingMethodCall(
        SyntaxNodeAnalysisContext context,
        MemberAccessExpressionSyntax memberAccess,
        HashSet<string> parameterNames,
        IMethodSymbol methodSymbol)
    {
        // Get the target of the member access (e.g., "list" in "list.Add(x)")
        var targetName = GetRootIdentifierName(memberAccess.Expression);
        if (targetName is null || !parameterNames.Contains(targetName))
            return;

        // Check if the method name is a mutating method
        var calledMethodName = memberAccess.Name.Identifier.Text;
        if (!MutatingMethodNames.Contains(calledMethodName))
            return;

        context.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.PUR006,
            memberAccess.GetLocation(),
            methodSymbol.Name,
            targetName));
    }

    /// <summary>
    /// Checks if an assignment targets a property on a parameter.
    /// </summary>
    private static void CheckParameterPropertyAssignment(
        SyntaxNodeAnalysisContext context,
        MemberAccessExpressionSyntax assignmentTarget,
        HashSet<string> parameterNames,
        IMethodSymbol methodSymbol)
    {
        var targetName = GetRootIdentifierName(assignmentTarget.Expression);
        if (targetName is null || !parameterNames.Contains(targetName))
            return;

        context.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.PUR006,
            assignmentTarget.GetLocation(),
            methodSymbol.Name,
            targetName));
    }

    /// <summary>
    /// Checks if an indexer assignment targets a parameter.
    /// </summary>
    private static void CheckParameterIndexerAssignment(
        SyntaxNodeAnalysisContext context,
        ElementAccessExpressionSyntax elementAccess,
        HashSet<string> parameterNames,
        IMethodSymbol methodSymbol)
    {
        var targetName = GetRootIdentifierName(elementAccess.Expression);
        if (targetName is null || !parameterNames.Contains(targetName))
            return;

        context.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.PUR006,
            elementAccess.GetLocation(),
            methodSymbol.Name,
            targetName));
    }

    /// <summary>
    /// Gets the root identifier name from an expression (handles chained member access).
    /// For "a.b.c" returns "a", for "x" returns "x".
    /// </summary>
    private static string? GetRootIdentifierName(ExpressionSyntax expression) =>
        expression switch
        {
            IdentifierNameSyntax identifier => identifier.Identifier.Text,
            MemberAccessExpressionSyntax memberAccess => GetRootIdentifierName(memberAccess.Expression),
            _ => null
        };

    #endregion

    #region PUR007 - Ref/Out Parameters

    /// <summary>
    /// PUR007: Detects ref/out parameters in pure methods.
    /// Pure methods should not have ref or out parameters as they allow mutation of caller state.
    /// </summary>
    private static void CheckForRefOutParameters(
        SyntaxNodeAnalysisContext context,
        IMethodSymbol methodSymbol)
    {
        foreach (var parameter in methodSymbol.Parameters)
        {
            if (parameter.RefKind is RefKind.Ref or RefKind.Out)
            {
                var refKindName = parameter.RefKind == RefKind.Ref ? "ref" : "out";
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.PUR007,
                    parameter.Locations.FirstOrDefault() ?? methodSymbol.Locations.FirstOrDefault(),
                    methodSymbol.Name,
                    refKindName,
                    parameter.Name));
            }
        }
    }

    #endregion

    #region PUR008 - Unsafe Code

    /// <summary>
    /// PUR008: Detects unsafe code in pure methods.
    /// Unsafe code can bypass type safety and perform arbitrary memory manipulation.
    /// </summary>
    private static void CheckForUnsafeCode(
        SyntaxNodeAnalysisContext context,
        MethodDeclarationSyntax method,
        IMethodSymbol methodSymbol)
    {
        // Check if method itself is marked unsafe
        if (method.Modifiers.Any(SyntaxKind.UnsafeKeyword))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.PUR008,
                method.Identifier.GetLocation(),
                methodSymbol.Name));
            return;
        }

        // Check for unsafe blocks, fixed statements, pointer types
        foreach (var node in method.DescendantNodes())
        {
            switch (node)
            {
                case UnsafeStatementSyntax unsafeStatement:
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.PUR008,
                        unsafeStatement.UnsafeKeyword.GetLocation(),
                        methodSymbol.Name));
                    break;

                case FixedStatementSyntax fixedStatement:
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.PUR008,
                        fixedStatement.FixedKeyword.GetLocation(),
                        methodSymbol.Name));
                    break;

                case PointerTypeSyntax pointerType:
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.PUR008,
                        pointerType.GetLocation(),
                        methodSymbol.Name));
                    break;
            }
        }
    }

    #endregion

    #region PUR009 - Reflection

    /// <summary>
    /// Reflection types/methods that violate purity.
    /// </summary>
    private static readonly string[] ReflectionPatterns =
    [
        "System.Type.GetType",
        "System.Type.GetMethod",
        "System.Type.GetProperty",
        "System.Type.GetField",
        "System.Type.GetConstructor",
        "System.Type.GetMember",
        "System.Type.GetMethods",
        "System.Type.GetProperties",
        "System.Type.GetFields",
        "System.Type.GetConstructors",
        "System.Type.GetMembers",
        "System.Reflection.Assembly.Load",
        "System.Reflection.Assembly.LoadFrom",
        "System.Reflection.Assembly.LoadFile",
        "System.Reflection.Assembly.GetTypes",
        "System.Reflection.Assembly.GetType",
        "System.Reflection.MethodBase.Invoke",
        "System.Reflection.MethodInfo.Invoke",
        "System.Reflection.ConstructorInfo.Invoke",
        "System.Reflection.PropertyInfo.SetValue",
        "System.Reflection.PropertyInfo.GetValue",
        "System.Reflection.FieldInfo.SetValue",
        "System.Reflection.FieldInfo.GetValue",
        "System.Activator.CreateInstance",
    ];

    /// <summary>
    /// PUR009: Detects reflection usage in pure methods.
    /// Reflection can bypass compile-time checks and perform side effects.
    /// </summary>
    private static void CheckForReflection(
        SyntaxNodeAnalysisContext context,
        MethodDeclarationSyntax method,
        IMethodSymbol methodSymbol)
    {
        foreach (var invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var calledSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (calledSymbol is null)
                continue;

            var containingType = calledSymbol.ContainingType?.ToDisplayString();
            if (containingType is null)
                continue;

            var fullName = containingType + "." + calledSymbol.Name;
            
            if (ReflectionPatterns.Any(pattern => fullName == pattern))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.PUR009,
                    invocation.GetLocation(),
                    methodSymbol.Name,
                    calledSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            }
        }
    }

    #endregion

    #region PUR010 - Exception Control Flow

    /// <summary>
    /// Methods that throw exceptions for expected failures (control flow).
    /// These are warnings, not errors.
    /// </summary>
    private static readonly string[] ExceptionControlFlowMethods =
    [
        "System.Int32.Parse",
        "System.Int64.Parse",
        "System.Double.Parse",
        "System.Decimal.Parse",
        "System.Single.Parse",
        "System.DateTime.Parse",
        "System.DateTimeOffset.Parse",
        "System.Guid.Parse",
        "System.Enum.Parse",
        "System.Convert.ToInt32",
        "System.Convert.ToInt64",
        "System.Convert.ToDouble",
        "System.Convert.ToDecimal",
        "System.Convert.ToDateTime",
    ];

    /// <summary>
    /// PUR010: Warns about methods that throw exceptions for control flow.
    /// Suggests using TryParse patterns instead.
    /// </summary>
    private static void CheckForExceptionControlFlow(
        SyntaxNodeAnalysisContext context,
        MethodDeclarationSyntax method,
        IMethodSymbol methodSymbol)
    {
        foreach (var invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var calledSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (calledSymbol is null)
                continue;

            // Build fully qualified CLR name using metadata (System.Int32 not int)
            var containingType = calledSymbol.ContainingType;
            if (containingType is null)
                continue;

            var containingNamespace = containingType.ContainingNamespace?.ToDisplayString();
            var typeName = containingType.MetadataName; // Always returns CLR name (Int32, not int)
            var fullName = string.IsNullOrEmpty(containingNamespace)
                ? typeName + "." + calledSymbol.Name
                : containingNamespace + "." + typeName + "." + calledSymbol.Name;
            
            if (ExceptionControlFlowMethods.Contains(fullName))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.PUR010,
                    invocation.GetLocation(),
                    methodSymbol.Name,
                    calledSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            }
        }
    }

    #endregion
}
