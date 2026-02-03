AI Code Generation Rules — WorkBeast

Goal
- Produce safe, small, and reviewable changes that are buildable and testable locally.

Core Rules
1. Follow repository tech stack and style
   - Language: C# targeting .NET 8
   - ORM: Entity Framework Core
   - Tests: xUnit
   - UI: ASP.NET Core MVC + Bootstrap 5
2. Keep PRs small
   - One feature or fix per PR. Prefer incremental commits with clear messages.
3. Provide runnable code
   - Add or update `Program.cs`/startup where needed so the project builds and runs.
   - For schema changes include EF Core migration code and a seed method.
4. Include tests
   - New features must include unit tests in `WorkBeast.Tests` covering business logic.
5. No secrets or credentials
   - Never include keys, credentials, or secrets in code or config files. Use environment variables.
6. Security and defaults
   - Use secure defaults (e.g., Validate inputs, use parameterized queries via EF Core).
7. Minimal external copying
   - Do not paste large third-party code. Prefer referencing and adding package references.
8. Documentation
   - Update `README.md` or add a short note in the feature folder explaining how to run locally.
9. Explain assumptions
   - Each generated change must include a short `ASSUMPTIONS.md` or PR note describing assumptions made.
10. Licensing and attribution
   - If external code/snippets are used, include the source and ensure license compatibility.
11. Formatting and linting
   - Run `dotnet format` before submitting. Keep code consistent with existing style.
12. Migration and data
   - Include a migration and a safe seed that doesn't delete user data. Seed should be idempotent.
13. CI expectations
   - Changes should not break CI; include tests and ensure they pass locally.

Review checklist (for reviewers)

- Does the code compile and run?
- Are there unit tests with meaningful assertions?
- Are secrets absent from the diff?
- Are migrations included for data changes?
- Are instructions to run the change included?

If anything in these rules conflicts with an explicit maintainer instruction, follow the maintainer instruction.