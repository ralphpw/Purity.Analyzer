# System.Math Purity Whitelist

All `System.Math` static methods are pure mathematical functions with no side effects.

---

## Methods

| Signature | NoMutation | NoIO | Deterministic | NoExceptions | Reviewer | ReviewDate | DotNetVersion |
|-----------|------------|------|---------------|--------------|----------|------------|---------------|
| `System.Math.Abs(System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Abs(System.Int64)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Abs(System.Int16)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Abs(System.SByte)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Abs(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Abs(System.Single)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Abs(System.Decimal)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Acos(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Acosh(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Asin(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Asinh(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Atan(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Atan2(System.Double,System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Atanh(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.BigMul(System.Int32,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.BigMul(System.Int64,System.Int64,System.Int64@)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.BigMul(System.UInt64,System.UInt64,System.UInt64@)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.BitDecrement(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.BitIncrement(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Cbrt(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Ceiling(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Ceiling(System.Decimal)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Clamp(System.Int32,System.Int32,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Clamp(System.Int64,System.Int64,System.Int64)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Clamp(System.Double,System.Double,System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Clamp(System.Single,System.Single,System.Single)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Clamp(System.Decimal,System.Decimal,System.Decimal)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.CopySign(System.Double,System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Cos(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Cosh(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.DivRem(System.Int32,System.Int32,System.Int32@)` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.DivRem(System.Int64,System.Int64,System.Int64@)` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Exp(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Floor(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Floor(System.Decimal)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.FusedMultiplyAdd(System.Double,System.Double,System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.IEEERemainder(System.Double,System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.ILogB(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Log(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Log(System.Double,System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Log10(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Log2(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Max(System.Int32,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Max(System.Int64,System.Int64)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Max(System.Double,System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Max(System.Single,System.Single)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Max(System.Decimal,System.Decimal)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.MaxMagnitude(System.Double,System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Min(System.Int32,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Min(System.Int64,System.Int64)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Min(System.Double,System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Min(System.Single,System.Single)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Min(System.Decimal,System.Decimal)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.MinMagnitude(System.Double,System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Pow(System.Double,System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Round(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Round(System.Double,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Round(System.Double,System.MidpointRounding)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Round(System.Double,System.Int32,System.MidpointRounding)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Round(System.Decimal)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Round(System.Decimal,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Round(System.Decimal,System.MidpointRounding)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Round(System.Decimal,System.Int32,System.MidpointRounding)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.ScaleB(System.Double,System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Sign(System.Int32)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Sign(System.Int64)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Sign(System.Int16)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Sign(System.SByte)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Sign(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Sign(System.Single)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Sign(System.Decimal)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Sin(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.SinCos(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Sinh(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Sqrt(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Tan(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Tanh(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Truncate(System.Double)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Truncate(System.Decimal)` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |

---

## Constants

| Signature | NoMutation | NoIO | Deterministic | NoExceptions | Reviewer | ReviewDate | DotNetVersion |
|-----------|------------|------|---------------|--------------|----------|------------|---------------|
| `System.Math.E` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.PI` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| `System.Math.Tau` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |

---

## Notes

- `DivRem` methods can throw `DivideByZeroException` when the divisor is 0 (marked warn for NoExceptions)
- All methods are static and take value types, so mutation is inherently impossible
- Floating-point operations follow IEEE 754 and are deterministic for the same inputs
- `BigMul` overloads with `out` parameters write to the out parameter but don't mutate existing state
