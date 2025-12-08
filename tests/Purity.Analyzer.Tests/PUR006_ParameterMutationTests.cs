using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Purity.Analyzer.Tests.Verifiers.CSharpAnalyzerVerifier<Purity.Analyzer.PurityAnalyzer>;

namespace Purity.Analyzer.Tests;

/// <summary>
/// Tests for PUR006: Pure method mutates parameter.
/// </summary>
public sealed class PUR006_ParameterMutationTests
{
    #region Mutating Method Calls

    [Fact]
    public async Task CallsListAdd_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad(List<int> list) => list.Add(42);
            }
            """;

        // Both PUR006 (parameter mutation) and PUR002 (non-pure call) are reported
        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR006)
                .WithSpan(7, 40, 7, 48)
                .WithArguments("Bad", "list"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(7, 40, 7, 52)
                .WithArguments("Bad", "List<int>.Add(int)"));
    }

    [Fact]
    public async Task CallsListRemove_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad(List<int> list) => list.Remove(42);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR006)
                .WithSpan(7, 40, 7, 51)
                .WithArguments("Bad", "list"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(7, 40, 7, 55)
                .WithArguments("Bad", "List<int>.Remove(int)"));
    }

    [Fact]
    public async Task CallsListClear_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad(List<int> list) => list.Clear();
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR006)
                .WithSpan(7, 40, 7, 50)
                .WithArguments("Bad", "list"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(7, 40, 7, 52)
                .WithArguments("Bad", "List<int>.Clear()"));
    }

    [Fact]
    public async Task CallsDictionaryAdd_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad(Dictionary<string, int> dict) => dict.Add("key", 1);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR006)
                .WithSpan(7, 54, 7, 62)
                .WithArguments("Bad", "dict"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(7, 54, 7, 72)
                .WithArguments("Bad", "Dictionary<string, int>.Add(string, int)"));
    }

    [Fact]
    public async Task CallsQueueEnqueue_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad(Queue<int> queue) => queue.Enqueue(42);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR006)
                .WithSpan(7, 42, 7, 55)
                .WithArguments("Bad", "queue"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(7, 42, 7, 59)
                .WithArguments("Bad", "Queue<int>.Enqueue(int)"));
    }

    [Fact]
    public async Task CallsStackPush_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad(Stack<int> stack) => stack.Push(42);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR006)
                .WithSpan(7, 42, 7, 52)
                .WithArguments("Bad", "stack"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(7, 42, 7, 56)
                .WithArguments("Bad", "Stack<int>.Push(int)"));
    }

    [Fact]
    public async Task CallsListSort_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad(List<int> list) => list.Sort();
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR006)
                .WithSpan(7, 40, 7, 49)
                .WithArguments("Bad", "list"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(7, 40, 7, 51)
                .WithArguments("Bad", "List<int>.Sort()"));
    }

    [Fact]
    public async Task CallsListReverse_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad(List<int> list) => list.Reverse();
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR006)
                .WithSpan(7, 40, 7, 52)
                .WithArguments("Bad", "list"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(7, 40, 7, 54)
                .WithArguments("Bad", "List<int>.Reverse()"));
    }

    #endregion

    #region Property Assignments

    [Fact]
    public async Task AssignsToParameterProperty_ReportsDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Person { public string Name { get; set; } }

            public class Test
            {
                [EnforcedPure]
                public void Bad(Person p) => p.Name = "Changed";
            }
            """;

        // Property setter is also a non-pure call
        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR006)
                .WithSpan(8, 34, 8, 40)
                .WithArguments("Bad", "p"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(8, 34, 8, 40)
                .WithArguments("Bad", "Person.Name"));
    }

    [Fact]
    public async Task AssignsToNestedProperty_ReportsDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Inner { public int Value { get; set; } }
            public class Outer { public Inner Inner { get; set; } }

            public class Test
            {
                [EnforcedPure]
                public void Bad(Outer obj) => obj.Inner.Value = 42;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR006)
                .WithSpan(9, 35, 9, 50)
                .WithArguments("Bad", "obj"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(9, 35, 9, 44)
                .WithArguments("Bad", "Outer.Inner"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(9, 35, 9, 50)
                .WithArguments("Bad", "Inner.Value"));
    }

    #endregion

    #region Indexer Assignments

    [Fact]
    public async Task AssignsToListIndexer_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad(List<int> list) => list[0] = 42;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR006)
                .WithSpan(7, 40, 7, 47)
                .WithArguments("Bad", "list"));
    }

    [Fact]
    public async Task AssignsToDictionaryIndexer_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad(Dictionary<string, int> dict) => dict["key"] = 42;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR006)
                .WithSpan(7, 54, 7, 65)
                .WithArguments("Bad", "dict"));
    }

    [Fact]
    public async Task AssignsToArrayIndexer_ReportsDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad(int[] arr) => arr[0] = 42;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR006)
                .WithSpan(6, 35, 6, 41)
                .WithArguments("Bad", "arr"));
    }

    #endregion

    #region Non-Mutating Operations (No Diagnostic for PUR006)

    [Fact]
    public async Task CallsStringContains_NoDiagnostic()
    {
        // String.Contains is whitelisted - no PUR002 or PUR006
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public bool Good(string s) => s.Contains("x");
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task ValueTypeParameter_Reassignment_NoDiagnostic()
    {
        // Value type parameter reassignment doesn't mutate the caller's value
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Good(int x)
                {
                    x = x + 1;  // Doesn't affect caller
                    return x;
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task ReadsStringLength_NoDiagnostic()
    {
        // String.Length is whitelisted
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Good(string s) => s.Length;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task CallsMathAbs_NoDiagnostic()
    {
        // Math.Abs is whitelisted - pure operation
        const string code = """
            using System;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Good(int x) => Math.Abs(x);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    #endregion

    #region Multiple Mutations

    [Fact]
    public async Task MultipleMutations_ReportsMultipleDiagnostics()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Bad(List<int> list)
                {
                    list.Add(1);
                    list.Add(2);
                    list.Clear();
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            // First Add - PUR006 then PUR002
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR006)
                .WithSpan(9, 9, 9, 17)
                .WithArguments("Bad", "list"),
            // Second Add - PUR006 then PUR002
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR006)
                .WithSpan(10, 9, 10, 17)
                .WithArguments("Bad", "list"),
            // Clear - PUR006 then PUR002
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR006)
                .WithSpan(11, 9, 11, 19)
                .WithArguments("Bad", "list"),
            // PUR002 diagnostics
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(9, 9, 9, 20)
                .WithArguments("Bad", "List<int>.Add(int)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(10, 9, 10, 20)
                .WithArguments("Bad", "List<int>.Add(int)"),
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR002)
                .WithSpan(11, 9, 11, 21)
                .WithArguments("Bad", "List<int>.Clear()"));
    }

    #endregion

    #region Non-Pure Methods

    [Fact]
    public async Task NonPureMethod_NoDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;

            public class Test
            {
                public void NotPure(List<int> list) => list.Add(42);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    #endregion
}
