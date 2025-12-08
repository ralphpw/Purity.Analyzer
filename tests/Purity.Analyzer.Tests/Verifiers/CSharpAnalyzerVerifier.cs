using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Purity.Analyzer.Tests.Verifiers;

/// <summary>
/// Helper for testing Roslyn analyzers with xUnit.
/// Provides simplified API for common test scenarios.
/// </summary>
public static class CSharpAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    /// <summary>
    /// Creates a diagnostic result for the specified descriptor.
    /// </summary>
    public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
        => CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(descriptor);

    /// <summary>
    /// Creates a diagnostic result for the specified diagnostic ID.
    /// </summary>
    public static DiagnosticResult Diagnostic(string diagnosticId)
        => CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(diagnosticId);

    /// <summary>
    /// Verifies that the analyzer produces the expected diagnostics for the given source.
    /// </summary>
    public static async Task VerifyAnalyzerAsync(
        string source,
        params DiagnosticResult[] expected)
    {
        var test = new Test { TestCode = source };
        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync(CancellationToken.None);
    }

    private sealed class Test : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
    {
        public Test()
        {
            // Use .NET 8 reference assemblies
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80;

            // Add reference to Purity.Contracts for [EnforcedPure] attribute
            TestState.AdditionalReferences.Add(
                typeof(Purity.Contracts.EnforcedPureAttribute).Assembly);

            // Configure nullable warnings
            SolutionTransforms.Add((solution, projectId) =>
            {
                var compilationOptions = solution.GetProject(projectId)?.CompilationOptions;
                if (compilationOptions is null)
                    return solution;

                compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(
                        NullableWarnings));

                return solution.WithProjectCompilationOptions(projectId, compilationOptions);
            });
        }

        private static ImmutableDictionary<string, ReportDiagnostic> NullableWarnings { get; }
            = GetNullableWarningsFromCompiler();

        private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
        {
            string[] args = ["/warnaserror:nullable"];
            var commandLineArguments = CSharpCommandLineParser.Default.Parse(
                args,
                baseDirectory: Environment.CurrentDirectory,
                sdkDirectory: Environment.CurrentDirectory);
            return commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;
        }
    }
}
