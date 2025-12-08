using Microsoft.CodeAnalysis;

namespace Purity.Analyzer;

/// <summary>
/// Central registry of all purity-related diagnostics.
/// Each diagnostic follows the pattern: PUR{NNN} where NNN is a 3-digit identifier.
/// </summary>
public static class DiagnosticDescriptors
{
    private const string Category = "Purity";
    private const string HelpLinkBase = "https://github.com/ralphpw/Purity.Analyzer/blob/main/docs/Rules.md";

    /// <summary>
    /// PUR001: Pure method mutates a field.
    /// </summary>
    public static readonly DiagnosticDescriptor PUR001 = new(
        id: "PUR001",
        title: "Pure method mutates field",
        messageFormat: "Method '{0}' marked as pure mutates field '{1}'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkBase}#pur001");

    /// <summary>
    /// PUR002: Pure method calls a non-pure method.
    /// </summary>
    public static readonly DiagnosticDescriptor PUR002 = new(
        id: "PUR002",
        title: "Pure method calls non-pure method",
        messageFormat: "Method '{0}' marked as pure calls non-pure method '{1}'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkBase}#pur002");

    /// <summary>
    /// PUR003: Pure method performs I/O operations.
    /// </summary>
    public static readonly DiagnosticDescriptor PUR003 = new(
        id: "PUR003",
        title: "Pure method performs I/O",
        messageFormat: "Method '{0}' marked as pure performs I/O operation via '{1}'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkBase}#pur003");

    /// <summary>
    /// PUR004: Pure method uses non-deterministic operations.
    /// </summary>
    public static readonly DiagnosticDescriptor PUR004 = new(
        id: "PUR004",
        title: "Pure method uses non-deterministic operation",
        messageFormat: "Method '{0}' marked as pure uses non-deterministic operation '{1}'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkBase}#pur004");

    /// <summary>
    /// PUR005: Pure method returns a mutable type.
    /// </summary>
    public static readonly DiagnosticDescriptor PUR005 = new(
        id: "PUR005",
        title: "Pure method returns mutable type",
        messageFormat: "Method '{0}' marked as pure returns mutable type '{1}'. Consider using '{2}' instead.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkBase}#pur005");

    /// <summary>
    /// PUR006: Pure method mutates a parameter.
    /// </summary>
    public static readonly DiagnosticDescriptor PUR006 = new(
        id: "PUR006",
        title: "Pure method mutates parameter",
        messageFormat: "Method '{0}' marked as pure mutates parameter '{1}'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkBase}#pur006");

    /// <summary>
    /// PUR007: Pure method has ref/out parameter.
    /// </summary>
    public static readonly DiagnosticDescriptor PUR007 = new(
        id: "PUR007",
        title: "Pure method has ref/out parameter",
        messageFormat: "Method '{0}' marked as pure has {1} parameter '{2}'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkBase}#pur007");

    /// <summary>
    /// PUR008: Pure method uses unsafe code.
    /// </summary>
    public static readonly DiagnosticDescriptor PUR008 = new(
        id: "PUR008",
        title: "Pure method uses unsafe code",
        messageFormat: "Method '{0}' marked as pure uses unsafe code",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkBase}#pur008");

    /// <summary>
    /// PUR009: Pure method uses reflection.
    /// </summary>
    public static readonly DiagnosticDescriptor PUR009 = new(
        id: "PUR009",
        title: "Pure method uses reflection",
        messageFormat: "Method '{0}' marked as pure uses reflection via '{1}'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkBase}#pur009");

    /// <summary>
    /// PUR010: Pure method uses exception for control flow.
    /// </summary>
    public static readonly DiagnosticDescriptor PUR010 = new(
        id: "PUR010",
        title: "Pure method may throw for control flow",
        messageFormat: "Method '{0}' marked as pure calls '{1}' which may throw for control flow",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkBase}#pur010");

    /// <summary>
    /// PUR011: Method is whitelisted but marked for review.
    /// </summary>
    public static readonly DiagnosticDescriptor PUR011 = new(
        id: "PUR011",
        title: "Method pending purity review",
        messageFormat: "Method '{0}' calls '{1}' which is marked for review",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        helpLinkUri: $"{HelpLinkBase}#pur011");
}
