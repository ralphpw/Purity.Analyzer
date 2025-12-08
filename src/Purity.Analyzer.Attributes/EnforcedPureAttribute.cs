using System;

namespace Purity.Contracts;

/// <summary>
/// Marks a method, class, or struct as pure. The Purity.Analyzer verifies
/// that code marked with this attribute is free from side effects, mutation,
/// and non-deterministic behavior.
/// </summary>
/// <remarks>
/// <para>
/// When applied to a method, that method must be pure.
/// When applied to a class or struct, all methods in that type must be pure.
/// </para>
/// <para>
/// Methods marked with [EnforcedPure] in referenced assemblies are trusted
/// without re-analysis, under the assumption they were verified at compile time.
/// </para>
/// </remarks>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct,
    Inherited = false,
    AllowMultiple = false)]
public sealed class EnforcedPureAttribute : Attribute
{
}
