using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Purity.Analyzer.Tests.Verifiers.CSharpAnalyzerVerifier<Purity.Analyzer.PurityAnalyzer>;

namespace Purity.Analyzer.Tests;

/// <summary>
/// Tests for PUR009: Pure method uses reflection.
/// Note: Reflection methods also trigger PUR002 (non-pure call) since they're not whitelisted.
/// </summary>
public sealed class PUR009_ReflectionTests
{
    [Fact]
    public async Task TypeGetType_ReportsDiagnostic()
    {
        const string code = """
            #nullable enable
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public Type? Bad(string name) => Type.GetType(name);
            }
            """;

        // Triggers both PUR002 (non-pure call) and PUR009 (reflection)
        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(8, 38, 8, 56)
                .WithArguments("Bad", "Type.GetType(string)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR009)
                .WithSpan(8, 38, 8, 56)
                .WithArguments("Bad", "Type.GetType(string)"));
    }

    [Fact]
    public async Task AssemblyLoad_ReportsDiagnostic()
    {
        const string code = """
            #nullable enable
            using System.Reflection;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public Assembly? Bad(string name) => Assembly.Load(name);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(8, 42, 8, 61)
                .WithArguments("Bad", "Assembly.Load(string)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR009)
                .WithSpan(8, 42, 8, 61)
                .WithArguments("Bad", "Assembly.Load(string)"));
    }

    [Fact]
    public async Task MethodInfoInvoke_ReportsDiagnostic()
    {
        const string code = """
            #nullable enable
            using System.Reflection;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public object? Bad(MethodInfo method, object target)
                    => method.Invoke(target, null);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(9, 12, 9, 39)
                .WithArguments("Bad", "MethodBase.Invoke(object?, object?[]?)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR009)
                .WithSpan(9, 12, 9, 39)
                .WithArguments("Bad", "MethodBase.Invoke(object?, object?[]?)"));
    }

    [Fact]
    public async Task ActivatorCreateInstance_ReportsDiagnostic()
    {
        const string code = """
            #nullable enable
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public object? Bad(Type type) => Activator.CreateInstance(type);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(8, 38, 8, 68)
                .WithArguments("Bad", "Activator.CreateInstance(Type)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR009)
                .WithSpan(8, 38, 8, 68)
                .WithArguments("Bad", "Activator.CreateInstance(Type)"));
    }

    [Fact]
    public async Task AssemblyGetTypes_ReportsDiagnostic()
    {
        const string code = """
            using System;
            using System.Reflection;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public Type[] Bad(Assembly asm) => asm.GetTypes();
            }
            """;

        // Also triggers PUR005 for mutable return type (array)
        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR005)
                .WithSpan(8, 12, 8, 18)
                .WithArguments("Bad", "Type[]", "ImmutableArray<Type>"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(8, 40, 8, 54)
                .WithArguments("Bad", "Assembly.GetTypes()"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR009)
                .WithSpan(8, 40, 8, 54)
                .WithArguments("Bad", "Assembly.GetTypes()"));
    }

    [Fact]
    public async Task TypeGetMethod_ReportsDiagnostic()
    {
        const string code = """
            #nullable enable
            using System;
            using System.Reflection;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public MethodInfo? Bad(Type type) => type.GetMethod("Test");
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(9, 42, 9, 64)
                .WithArguments("Bad", "Type.GetMethod(string)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR009)
                .WithSpan(9, 42, 9, 64)
                .WithArguments("Bad", "Type.GetMethod(string)"));
    }

    [Fact]
    public async Task TypeGetProperty_ReportsDiagnostic()
    {
        const string code = """
            #nullable enable
            using System;
            using System.Reflection;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public PropertyInfo? Bad(Type type) => type.GetProperty("Test");
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(9, 44, 9, 68)
                .WithArguments("Bad", "Type.GetProperty(string)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR009)
                .WithSpan(9, 44, 9, 68)
                .WithArguments("Bad", "Type.GetProperty(string)"));
    }

    [Fact]
    public async Task TypeGetField_ReportsDiagnostic()
    {
        const string code = """
            #nullable enable
            using System;
            using System.Reflection;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public FieldInfo? Bad(Type type) => type.GetField("Test");
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(9, 41, 9, 62)
                .WithArguments("Bad", "Type.GetField(string)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR009)
                .WithSpan(9, 41, 9, 62)
                .WithArguments("Bad", "Type.GetField(string)"));
    }

    [Fact]
    public async Task TypeOfExpression_NoDiagnostic()
    {
        const string code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public Type Good() => typeof(string);
            }
            """;

        // typeof() is compile-time, not reflection
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task GetTypeOnObject_NoDiagnostic()
    {
        const string code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public Type Good(object obj) => obj.GetType();
            }
            """;

        // object.GetType() is a runtime type check, not dynamic reflection
        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task ReflectionInNonPureMethod_NoDiagnostic()
    {
        const string code = """
            #nullable enable
            using System;

            public class Test
            {
                public Type? NotPure(string name) => Type.GetType(name);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task ReflectionWithBclPure_ReportsDiagnostic()
    {
        const string code = """
            #nullable enable
            using System;
            using System.Diagnostics.Contracts;

            public class Test
            {
                [Pure]
                public Type? Bad(string name) => Type.GetType(name);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(8, 38, 8, 56)
                .WithArguments("Bad", "Type.GetType(string)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR009)
                .WithSpan(8, 38, 8, 56)
                .WithArguments("Bad", "Type.GetType(string)"));
    }
}
