# System.String Purity Whitelist

`System.String` instance and static methods that are pure. Strings are immutable in .NET, so all operations return new strings.

---

## Static Methods

| Signature | NoMutation | NoIO | Deterministic | NoExceptions | Reviewer | ReviewDate | DotNetVersion |
|-----------|------------|------|---------------|--------------|----------|------------|---------------|
| `System.String.Compare(System.String,System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Compare(System.String,System.String,System.Boolean)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Compare(System.String,System.String,System.StringComparison)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Compare(System.String,System.Int32,System.String,System.Int32,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Compare(System.String,System.Int32,System.String,System.Int32,System.Int32,System.Boolean)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Compare(System.String,System.Int32,System.String,System.Int32,System.Int32,System.StringComparison)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.CompareOrdinal(System.String,System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.CompareOrdinal(System.String,System.Int32,System.String,System.Int32,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Concat(System.String,System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Concat(System.String,System.String,System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Concat(System.String,System.String,System.String,System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Concat(System.String[])` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Concat(System.Object)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Concat(System.Object,System.Object)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Concat(System.Object,System.Object,System.Object)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Concat(System.Object[])` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Equals(System.String,System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Equals(System.String,System.String,System.StringComparison)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Format(System.String,System.Object)` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Format(System.String,System.Object,System.Object)` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Format(System.String,System.Object,System.Object,System.Object)` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Format(System.String,System.Object[])` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Format(System.IFormatProvider,System.String,System.Object)` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Format(System.IFormatProvider,System.String,System.Object,System.Object)` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Format(System.IFormatProvider,System.String,System.Object,System.Object,System.Object)` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Format(System.IFormatProvider,System.String,System.Object[])` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.IsNullOrEmpty(System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.IsNullOrWhiteSpace(System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Join(System.String,System.String[])` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Join(System.String,System.Object[])` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Join(System.String,System.String[],System.Int32,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Join(System.Char,System.String[])` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Intern(System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.IsInterned(System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |

---

## Instance Methods

| Signature | NoMutation | NoIO | Deterministic | NoExceptions | Reviewer | ReviewDate | DotNetVersion |
|-----------|------------|------|---------------|--------------|----------|------------|---------------|
| `System.String.Contains(System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Contains(System.String,System.StringComparison)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Contains(System.Char)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Contains(System.Char,System.StringComparison)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.EndsWith(System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.EndsWith(System.String,System.StringComparison)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.EndsWith(System.String,System.Boolean,System.Globalization.CultureInfo)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.EndsWith(System.Char)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Equals(System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Equals(System.String,System.StringComparison)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Equals(System.Object)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.GetHashCode()` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.GetHashCode(System.StringComparison)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.IndexOf(System.Char)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.IndexOf(System.Char,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.IndexOf(System.Char,System.Int32,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.IndexOf(System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.IndexOf(System.String,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.IndexOf(System.String,System.Int32,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.IndexOf(System.String,System.StringComparison)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.IndexOf(System.String,System.Int32,System.StringComparison)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.IndexOf(System.String,System.Int32,System.Int32,System.StringComparison)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.IndexOfAny(System.Char[])` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.IndexOfAny(System.Char[],System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.IndexOfAny(System.Char[],System.Int32,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Insert(System.Int32,System.String)` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.LastIndexOf(System.Char)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.LastIndexOf(System.Char,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.LastIndexOf(System.Char,System.Int32,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.LastIndexOf(System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.LastIndexOf(System.String,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.LastIndexOf(System.String,System.Int32,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.LastIndexOf(System.String,System.StringComparison)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.LastIndexOf(System.String,System.Int32,System.StringComparison)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.LastIndexOf(System.String,System.Int32,System.Int32,System.StringComparison)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.LastIndexOfAny(System.Char[])` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.LastIndexOfAny(System.Char[],System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.LastIndexOfAny(System.Char[],System.Int32,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.PadLeft(System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.PadLeft(System.Int32,System.Char)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.PadRight(System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.PadRight(System.Int32,System.Char)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Remove(System.Int32)` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Remove(System.Int32,System.Int32)` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Replace(System.Char,System.Char)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Replace(System.String,System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Replace(System.String,System.String,System.StringComparison)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Replace(System.String,System.String,System.Boolean,System.Globalization.CultureInfo)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Split(System.Char)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Split(System.Char,System.StringSplitOptions)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Split(System.Char,System.Int32,System.StringSplitOptions)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Split(System.Char[])` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Split(System.Char[],System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Split(System.Char[],System.StringSplitOptions)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Split(System.Char[],System.Int32,System.StringSplitOptions)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Split(System.String,System.StringSplitOptions)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Split(System.String,System.Int32,System.StringSplitOptions)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Split(System.String[],System.StringSplitOptions)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Split(System.String[],System.Int32,System.StringSplitOptions)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.StartsWith(System.String)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.StartsWith(System.String,System.StringComparison)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.StartsWith(System.String,System.Boolean,System.Globalization.CultureInfo)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.StartsWith(System.Char)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Substring(System.Int32)` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Substring(System.Int32,System.Int32)` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.ToCharArray()` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.ToCharArray(System.Int32,System.Int32)` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.ToLower()` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.ToLower(System.Globalization.CultureInfo)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.ToLowerInvariant()` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.ToString()` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.ToUpper()` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.ToUpper(System.Globalization.CultureInfo)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.ToUpperInvariant()` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Trim()` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Trim(System.Char)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Trim(System.Char[])` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.TrimEnd()` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.TrimEnd(System.Char)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.TrimEnd(System.Char[])` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.TrimStart()` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.TrimStart(System.Char)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.TrimStart(System.Char[])` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |

---

## Properties

| Signature | NoMutation | NoIO | Deterministic | NoExceptions | Reviewer | ReviewDate | DotNetVersion |
|-----------|------------|------|---------------|--------------|----------|------------|---------------|
| `System.String.Length` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Empty` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.String.Chars(System.Int32)` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |

---

## Notes

- `String.Format` can throw `FormatException` if format string is invalid (marked warn)
- `Substring`, `Insert`, `Remove` can throw `ArgumentOutOfRangeException` (marked warn)
- `Chars` (indexer) can throw `IndexOutOfRangeException` (marked warn)
- All string instance methods are inherently non-mutating as strings are immutable
- Culture-sensitive overloads are deterministic for the same culture settings
- `ToCharArray` returns a new array—mutation of the array doesn't affect the source string
