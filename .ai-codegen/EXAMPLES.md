Examples — Prompts and Expected Outputs

Example 1 — Add `Exercise` model
Prompt:

```
Implement an `Exercise` entity with properties Id, Name, Description, MuscleGroup. Add EF Core mapping and a migration. Add a unit test verifying CRUD via the data layer.
```

Expected output:

- `WorkBeast.Core/Models/Exercise.cs` (model)
- `WorkBeast.Data/EntityConfigurations/ExerciseConfiguration.cs` (mapping)
- `WorkBeast.Data/Migrations/XXXXXXXX_AddExercise.cs` (migration)
- `WorkBeast.Tests/ExerciseRepositoryTests.cs` (xUnit test)
- A short `ASSUMPTIONS.md` describing decisions.

Example 2 — Fix incorrect progress calculation
Prompt:

```
A reported bug: weekly progress percentage is calculated incorrectly when there are zero workouts. Provide a fix and an xUnit test that fails before and passes after the fix.
```

Expected output:

- Small fix in the service method handling zero-case.
- New or updated test in `WorkBeast.Tests` demonstrating expected behavior.

Example 3 — Add API endpoint
Prompt:

```
Add a read-only API endpoint `/api/exercises` returning paged exercises. Use minimal DTOs and AutoMapper profile. Include unit tests for controller and mapping.
```

Expected output:

- Controller in `WorkBeast.Web/Controllers/ExercisesApiController.cs`
- DTOs and AutoMapper `Profile` in `WorkBeast.Core` or `WorkBeast.Web`
- Tests for controller and mapping in `WorkBeast.Tests`.

When submitting outputs, always include simple commands to build and run tests:

```bash
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Add more examples here as needed.
