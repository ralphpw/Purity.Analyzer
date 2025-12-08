using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Purity.Analyzer.Tests.Verifiers.CSharpAnalyzerVerifier<Purity.Analyzer.PurityAnalyzer>;

namespace Purity.Analyzer.Tests;

/// <summary>
/// Tests for PUR005: Pure method returns mutable type.
/// </summary>
public sealed class PUR005_MutableReturnTests
{
    #region Array Types

    [Fact]
    public async Task ReturnsIntArray_ReportsDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int[] Bad() => new int[10];
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR005)
                .WithSpan(6, 12, 6, 17)
                .WithArguments("Bad", "int[]", "ImmutableArray<int>"));
    }

    [Fact]
    public async Task ReturnsStringArray_ReportsDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public string[] GetNames() => new[] { "a", "b" };
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR005)
                .WithSpan(6, 12, 6, 20)
                .WithArguments("GetNames", "string[]", "ImmutableArray<string>"));
    }

    [Fact]
    public async Task ReturnsJaggedArray_ReportsDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int[][] GetMatrix() => new int[2][];
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR005)
                .WithSpan(6, 12, 6, 19)
                .WithArguments("GetMatrix", "int[][]", "ImmutableArray<int[]>"));
    }

    #endregion

    #region Generic Collections

    [Fact]
    public async Task ReturnsList_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public List<int> Bad() => new List<int>();
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR005)
                .WithSpan(7, 12, 7, 21)
                .WithArguments("Bad", "List<int>", "ImmutableList<T>"));
    }

    [Fact]
    public async Task ReturnsDictionary_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public Dictionary<string, int> Bad() => new Dictionary<string, int>();
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR005)
                .WithSpan(7, 12, 7, 35)
                .WithArguments("Bad", "Dictionary<string, int>", "ImmutableDictionary<TKey, TValue>"));
    }

    [Fact]
    public async Task ReturnsHashSet_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public HashSet<string> Bad() => new HashSet<string>();
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR005)
                .WithSpan(7, 12, 7, 27)
                .WithArguments("Bad", "HashSet<string>", "ImmutableHashSet<T>"));
    }

    [Fact]
    public async Task ReturnsQueue_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public Queue<int> Bad() => new Queue<int>();
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR005)
                .WithSpan(7, 12, 7, 22)
                .WithArguments("Bad", "Queue<int>", "ImmutableQueue<T>"));
    }

    [Fact]
    public async Task ReturnsStack_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public Stack<int> Bad() => new Stack<int>();
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR005)
                .WithSpan(7, 12, 7, 22)
                .WithArguments("Bad", "Stack<int>", "ImmutableStack<T>"));
    }

    [Fact]
    public async Task ReturnsSortedSet_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public SortedSet<int> Bad() => new SortedSet<int>();
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR005)
                .WithSpan(7, 12, 7, 26)
                .WithArguments("Bad", "SortedSet<int>", "ImmutableSortedSet<T>"));
    }

    [Fact]
    public async Task ReturnsSortedDictionary_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public SortedDictionary<string, int> Bad() => new SortedDictionary<string, int>();
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR005)
                .WithSpan(7, 12, 7, 41)
                .WithArguments("Bad", "SortedDictionary<string, int>", "ImmutableSortedDictionary<TKey, TValue>"));
    }

    [Fact]
    public async Task ReturnsLinkedList_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public LinkedList<int> Bad() => new LinkedList<int>();
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR005)
                .WithSpan(7, 12, 7, 27)
                .WithArguments("Bad", "LinkedList<int>", "ImmutableList<T>"));
    }

    #endregion

    #region Non-Generic Collections

    [Fact]
    public async Task ReturnsArrayList_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public ArrayList Bad() => new ArrayList();
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR005)
                .WithSpan(7, 12, 7, 21)
                .WithArguments("Bad", "ArrayList", "ImmutableList"));
    }

    [Fact]
    public async Task ReturnsHashtable_ReportsDiagnostic()
    {
        const string code = """
            using System.Collections;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public Hashtable Bad() => new Hashtable();
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code,
            VerifyCS.Diagnostic(DiagnosticDescriptors.PUR005)
                .WithSpan(7, 12, 7, 21)
                .WithArguments("Bad", "Hashtable", "ImmutableDictionary"));
    }

    #endregion

    #region Allowed Types (No Diagnostic)

    [Fact]
    public async Task ReturnsImmutableList_NoDiagnostic()
    {
        const string code = """
            using System.Collections.Immutable;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public ImmutableList<int> Good() => ImmutableList<int>.Empty;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task ReturnsImmutableArray_NoDiagnostic()
    {
        const string code = """
            using System.Collections.Immutable;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public ImmutableArray<int> Good() => ImmutableArray<int>.Empty;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task ReturnsIEnumerable_NoDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using System.Linq;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public IEnumerable<int> Good() => Enumerable.Range(0, 10);
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task ReturnsIReadOnlyList_NoDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public IReadOnlyList<int> Good() => ImmutableList<int>.Empty;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task ReturnsPrimitiveType_NoDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public int Good() => 42;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task ReturnsString_NoDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public string Good() => "hello";
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    [Fact]
    public async Task ReturnsVoid_NoDiagnostic()
    {
        const string code = """
            using Purity.Contracts;

            public class Test
            {
                [EnforcedPure]
                public void Good() { }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    #endregion

    #region Non-Pure Methods

    [Fact]
    public async Task NonPureMethod_ReturnsMutableType_NoDiagnostic()
    {
        const string code = """
            using System.Collections.Generic;

            public class Test
            {
                public List<int> NotPure() => new List<int>();
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    #endregion
}
