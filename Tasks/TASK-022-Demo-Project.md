# TASK-022: Create Demo/Sample Project

**Status:** ✅ Complete  
**Priority:** Medium  
**Milestone:** v0.2.0  

## Objective

Create a standalone demo project in `samples/BasicUsage/` that consumes the published NuGet package and demonstrates real-world usage patterns.

## Scope

### 1. Project Structure
```
samples/
  BasicUsage/
    BasicUsage.csproj      # References Purity.Analyzer NuGet package
    Program.cs             # Demonstrates valid and invalid patterns
    Calculator.cs          # Pure business logic examples
    README.md              # Explains the demo
```

### 2. Demo Coverage

The sample should demonstrate:

#### Valid Pure Code
- Simple calculations (Add, Multiply)
- String transformations
- LINQ operations
- Immutable data structures
- Pure method calling other pure methods

#### Common Violations (Commented Out)
- Field mutation (PUR001)
- Calling non-pure methods (PUR002)
- Console I/O (PUR003)
- DateTime.Now usage (PUR004)
- Returning mutable types (PUR005)
- Parameter mutation (PUR006)
- ref/out parameters (PUR007)
- Unsafe code (PUR008)
- Reflection (PUR009)
- Exception control flow (PUR010)

### 3. Configuration Example

Include a `.purity/config.json` showing:
- Trust mode configuration
- Custom whitelist entries
- Review-required patterns

### 4. NuGet Source Configuration

Add `nuget.config` to reference:
- Local `./nupkg` folder (for testing pre-release builds)
- NuGet.org (for published packages)

## Acceptance Criteria

- [ ] `samples/BasicUsage/` project builds successfully
- [ ] Demonstrates at least 5 valid pure methods
- [ ] Shows commented examples of all PUR001-PUR010 violations
- [ ] Includes `.purity/config.json` with explanatory comments
- [ ] `README.md` in samples/ explains how to run and what to expect
- [ ] Can be used to smoke-test local NuGet package builds
- [ ] Can switch to published NuGet.org package via version change

## Benefits

1. **Smoke Test**: Validates packaging/dependencies work correctly
2. **Documentation**: Living examples are better than static docs
3. **Onboarding**: New users can clone and experiment immediately
4. **Marketing**: Shows the analyzer in action

## Implementation Notes

- Use `net8.0` as target framework (current LTS)
- Keep code simple and well-commented
- Focus on clarity over completeness
- Each violation example should have a comment explaining the error

## Related Tasks

- TASK-016: Announce v0.2.0 (demo makes announcement more compelling)
