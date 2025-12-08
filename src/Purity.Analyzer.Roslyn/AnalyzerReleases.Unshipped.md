; Analyzer release tracking - unshipped rules

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
PUR001 | Purity | Error | Pure method mutates field
PUR002 | Purity | Error | Pure method calls non-pure method
PUR003 | Purity | Error | Pure method performs I/O
PUR004 | Purity | Error | Pure method uses non-deterministic operation
PUR005 | Purity | Error | Pure method returns mutable type
PUR006 | Purity | Error | Pure method mutates parameter
PUR007 | Purity | Error | Pure method has ref/out parameter
PUR008 | Purity | Error | Pure method uses unsafe code
PUR009 | Purity | Error | Pure method uses reflection
PUR010 | Purity | Warning | Pure method may throw for control flow
PUR011 | Purity | Info | Method pending purity review
