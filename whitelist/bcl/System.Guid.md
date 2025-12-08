# System.Guid Pure Methods

Methods on `System.Guid` that are pure (no side effects, deterministic given same input).

## Parsing and Conversion

| Signature | Notes |
| --------- | ----- |
| System.Guid.Parse(System.String) | Parses string to Guid |
| System.Guid.Parse(System.ReadOnlySpan`1[System.Char]) | Parses span to Guid |
| System.Guid.ParseExact(System.String, System.String) | Parses with format |
| System.Guid.ParseExact(System.ReadOnlySpan`1[System.Char], System.ReadOnlySpan`1[System.Char]) | Parses span with format |
| System.Guid.TryParse(System.String, System.Guid@) | Safe parse |
| System.Guid.TryParse(System.ReadOnlySpan`1[System.Char], System.Guid@) | Safe parse from span |
| System.Guid.TryParseExact(System.String, System.String, System.Guid@) | Safe parse with format |
| System.Guid.TryParseExact(System.ReadOnlySpan`1[System.Char], System.ReadOnlySpan`1[System.Char], System.Guid@) | Safe parse span with format |

## Instance Methods

| Signature | Notes |
| --------- | ----- |
| System.Guid.ToString() | Converts to string |
| System.Guid.ToString(System.String) | Converts with format |
| System.Guid.ToString(System.String, System.IFormatProvider) | Converts with format and culture |
| System.Guid.ToByteArray() | Gets bytes representation |
| System.Guid.TryFormat(System.Span`1[System.Char], System.Int32@, System.ReadOnlySpan`1[System.Char]) | Formats to span |
| System.Guid.GetHashCode() | Gets hash code |
| System.Guid.Equals(System.Object) | Object equality |
| System.Guid.Equals(System.Guid) | Guid equality |
| System.Guid.CompareTo(System.Object) | Comparison |
| System.Guid.CompareTo(System.Guid) | Typed comparison |

## Static Constants

| Signature | Notes |
| --------- | ----- |
| System.Guid.Empty | Empty Guid constant |
