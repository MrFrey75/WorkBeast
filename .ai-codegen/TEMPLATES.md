Prompt Templates for WorkBeast (use as a starting point)

Feature Implementation

```
Context: Repository uses .NET 8, C#, EF Core, ASP.NET Core MVC. Keep changes minimal and include EF migrations and xUnit tests.
Task: Implement <short feature description>.
Requirements:
- Add domain model(s) in `WorkBeast.Core`.
- Add data mappings and migration in `WorkBeast.Data`.
- Add service and unit tests in `WorkBeast.Tests`.
- Update `WorkBeast.Web` controller/view if required.
- Provide a short ASSUMPTIONS.md and run instructions.
Output: Code diffs only, separated by file path, with brief explanations and example commands to run tests.
```

Bug Fix

```
Context: Provide failing test or minimal reproduction steps.
Task: Fix the bug and add a unit test that reproduces the failure then passes.
Requirements:
- Small focused change.
- Add/modify one test in `WorkBeast.Tests` that would have failed before the fix.
- No credentials or environment-specific changes.
Output: Code diffs and the failing->passing test description.
```

Unit Test

```
Context: Describe the class/method to test and any test doubles required.
Task: Add xUnit test(s) covering behavior and edge cases.
Requirements:
- Use in-memory providers (e.g., `UseSqlite` in-memory mode or EF Core InMemory) for DB tests.
- Keep tests deterministic and fast.
Output: Test file(s) and command to run them.
```

PR Description Template

```
Title: Short descriptive title
Summary: Brief summary of changes
Checklist:
- [ ] Builds locally
- [ ] Tests added/updated
- [ ] Migration included (if applicable)
- [ ] Docs updated
Assumptions: <short list>
How to run locally: <commands>
```

Use these templates as starting points and adapt to the situation.