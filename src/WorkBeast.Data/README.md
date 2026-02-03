# WorkBeast.Data

Data access layer for WorkBeast using Entity Framework Core and SQLite.

Quick build (from repository root):

```bash
cd src
dotnet restore
dotnet build WorkBeast.Data/WorkBeast.Data.csproj
```

Next steps:

- Add `AppDbContext` under `WorkBeast.Data/Context`.
- Add entity configurations and Migrations folder.
