# System.Linq.Enumerable Purity Whitelist

LINQ query operators from `System.Linq.Enumerable`. All methods are pure when used with pure predicates/selectors.

**Important:** The lambdas/delegates passed to these methods must also be pure. The analyzer verifies this separately.

---

## Query Operators

| Signature | NoMutation | NoIO | Deterministic | NoExceptions | Reviewer | ReviewDate | DotNetVersion |
|-----------|------------|------|---------------|--------------|----------|------------|---------------|
| ``System.Linq.Enumerable.Aggregate`1(System.Collections.Generic.IEnumerable`1,System.Func`3)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Aggregate`2(System.Collections.Generic.IEnumerable`1,`1,System.Func`3)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Aggregate`3(System.Collections.Generic.IEnumerable`1,`1,System.Func`3,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.All`1(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Any`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Any`1(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Append`1(System.Collections.Generic.IEnumerable`1,`0)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.AsEnumerable`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Average(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Cast`1(System.Collections.IEnumerable)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Chunk`1(System.Collections.Generic.IEnumerable`1,System.Int32)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Concat`1(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Contains`1(System.Collections.Generic.IEnumerable`1,`0)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Contains`1(System.Collections.Generic.IEnumerable`1,`0,System.Collections.Generic.IEqualityComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Count`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Count`1(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.DefaultIfEmpty`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.DefaultIfEmpty`1(System.Collections.Generic.IEnumerable`1,`0)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Distinct`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Distinct`1(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEqualityComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.DistinctBy`2(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.DistinctBy`2(System.Collections.Generic.IEnumerable`1,System.Func`2,System.Collections.Generic.IEqualityComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ElementAt`1(System.Collections.Generic.IEnumerable`1,System.Int32)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ElementAt`1(System.Collections.Generic.IEnumerable`1,System.Index)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ElementAtOrDefault`1(System.Collections.Generic.IEnumerable`1,System.Int32)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ElementAtOrDefault`1(System.Collections.Generic.IEnumerable`1,System.Index)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Empty`1()`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Except`1(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Except`1(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEqualityComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ExceptBy`2(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ExceptBy`2(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1,System.Func`2,System.Collections.Generic.IEqualityComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.First`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.First`1(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.FirstOrDefault`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.FirstOrDefault`1(System.Collections.Generic.IEnumerable`1,`0)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.FirstOrDefault`1(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.FirstOrDefault`1(System.Collections.Generic.IEnumerable`1,System.Func`2,`0)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.GroupBy`2(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.GroupBy`2(System.Collections.Generic.IEnumerable`1,System.Func`2,System.Collections.Generic.IEqualityComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.GroupBy`3(System.Collections.Generic.IEnumerable`1,System.Func`2,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.GroupBy`3(System.Collections.Generic.IEnumerable`1,System.Func`2,System.Func`3)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.GroupBy`4(System.Collections.Generic.IEnumerable`1,System.Func`2,System.Func`2,System.Func`3)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.GroupJoin`4(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1,System.Func`2,System.Func`2,System.Func`3)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Intersect`1(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Intersect`1(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEqualityComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.IntersectBy`2(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.IntersectBy`2(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1,System.Func`2,System.Collections.Generic.IEqualityComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Join`4(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1,System.Func`2,System.Func`2,System.Func`3)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Last`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Last`1(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.LastOrDefault`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.LastOrDefault`1(System.Collections.Generic.IEnumerable`1,`0)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.LastOrDefault`1(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.LastOrDefault`1(System.Collections.Generic.IEnumerable`1,System.Func`2,`0)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.LongCount`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.LongCount`1(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Max`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Max`2(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.MaxBy`2(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.MaxBy`2(System.Collections.Generic.IEnumerable`1,System.Func`2,System.Collections.Generic.IComparer`1)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Min`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Min`2(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.MinBy`2(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.MinBy`2(System.Collections.Generic.IEnumerable`1,System.Func`2,System.Collections.Generic.IComparer`1)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.OfType`1(System.Collections.IEnumerable)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Order`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Order`1(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.OrderBy`2(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.OrderBy`2(System.Collections.Generic.IEnumerable`1,System.Func`2,System.Collections.Generic.IComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.OrderByDescending`2(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.OrderByDescending`2(System.Collections.Generic.IEnumerable`1,System.Func`2,System.Collections.Generic.IComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.OrderDescending`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.OrderDescending`1(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Prepend`1(System.Collections.Generic.IEnumerable`1,`0)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Range(System.Int32,System.Int32)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Repeat`1(`0,System.Int32)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Reverse`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Select`2(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Select`2(System.Collections.Generic.IEnumerable`1,System.Func`3)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.SelectMany`2(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.SelectMany`2(System.Collections.Generic.IEnumerable`1,System.Func`3)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.SelectMany`3(System.Collections.Generic.IEnumerable`1,System.Func`2,System.Func`3)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.SelectMany`3(System.Collections.Generic.IEnumerable`1,System.Func`3,System.Func`3)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.SequenceEqual`1(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.SequenceEqual`1(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEqualityComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Single`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Single`1(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.SingleOrDefault`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.SingleOrDefault`1(System.Collections.Generic.IEnumerable`1,`0)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.SingleOrDefault`1(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.SingleOrDefault`1(System.Collections.Generic.IEnumerable`1,System.Func`2,`0)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Skip`1(System.Collections.Generic.IEnumerable`1,System.Int32)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.SkipLast`1(System.Collections.Generic.IEnumerable`1,System.Int32)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.SkipWhile`1(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.SkipWhile`1(System.Collections.Generic.IEnumerable`1,System.Func`3)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Sum(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Take`1(System.Collections.Generic.IEnumerable`1,System.Int32)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Take`1(System.Collections.Generic.IEnumerable`1,System.Range)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.TakeLast`1(System.Collections.Generic.IEnumerable`1,System.Int32)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.TakeWhile`1(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.TakeWhile`1(System.Collections.Generic.IEnumerable`1,System.Func`3)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ThenBy`2(System.Linq.IOrderedEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ThenBy`2(System.Linq.IOrderedEnumerable`1,System.Func`2,System.Collections.Generic.IComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ThenByDescending`2(System.Linq.IOrderedEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ThenByDescending`2(System.Linq.IOrderedEnumerable`1,System.Func`2,System.Collections.Generic.IComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ToArray`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ToDictionary`2(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ToDictionary`2(System.Collections.Generic.IEnumerable`1,System.Func`2,System.Collections.Generic.IEqualityComparer`1)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ToDictionary`3(System.Collections.Generic.IEnumerable`1,System.Func`2,System.Func`2)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ToDictionary`3(System.Collections.Generic.IEnumerable`1,System.Func`2,System.Func`2,System.Collections.Generic.IEqualityComparer`1)`` | yes | yes | yes | warn | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ToHashSet`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ToHashSet`1(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEqualityComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ToList`1(System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ToLookup`2(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ToLookup`2(System.Collections.Generic.IEnumerable`1,System.Func`2,System.Collections.Generic.IEqualityComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ToLookup`3(System.Collections.Generic.IEnumerable`1,System.Func`2,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.ToLookup`3(System.Collections.Generic.IEnumerable`1,System.Func`2,System.Func`2,System.Collections.Generic.IEqualityComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.TryGetNonEnumeratedCount`1(System.Collections.Generic.IEnumerable`1,System.Int32@)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Union`1(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Union`1(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEqualityComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.UnionBy`2(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.UnionBy`2(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1,System.Func`2,System.Collections.Generic.IEqualityComparer`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Where`1(System.Collections.Generic.IEnumerable`1,System.Func`2)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Where`1(System.Collections.Generic.IEnumerable`1,System.Func`3)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Zip`2(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Zip`3(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1,System.Func`3)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |
| ``System.Linq.Enumerable.Zip`3(System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1,System.Collections.Generic.IEnumerable`1)`` | yes | yes | yes | yes | claude-opus-4-20250514 | 2024-12-08 | 8.0 |

---

## Notes

- **Aggregate without seed** throws `InvalidOperationException` on empty sequences (marked warn)
- **Average** throws on empty sequences (marked warn)
- **Cast** throws `InvalidCastException` if element cannot be cast (marked warn)
- **Chunk** throws if size ≤ 0 (marked warn)
- **ElementAt** throws `ArgumentOutOfRangeException` if index out of bounds (marked warn)
- **First, Last, Single** throw on empty sequences or no match (marked warn)
- **SingleOrDefault** throws if more than one element matches (marked warn)
- **Max, Min, MaxBy, MinBy** throw on empty sequences (marked warn)
- **Range, Repeat** throw on invalid arguments (marked warn)
- **ToDictionary** throws on duplicate keys (marked warn)
- All LINQ operators use deferred execution (except `ToArray`, `ToList`, `ToDictionary`, `ToHashSet`, `ToLookup`, `Count`, `First`, `Single`, etc.)
- **The lambdas passed to these methods must also be pure** — the analyzer verifies this separately
