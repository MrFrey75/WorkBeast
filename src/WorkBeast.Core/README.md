# WorkBeast.Core

Core domain models and services for WorkBeast.

This project is a .NET 8 class library. It contains the domain entities and business service interfaces/implementations.

Quick build (from repository root):

```bash
cd src
dotnet restore
dotnet build WorkBeast.Core/WorkBeast.Core.csproj
```

Next steps:
- Add Entity Framework mappings and `DbContext` in `WorkBeast.Data`.
- Add unit tests in `WorkBeast.Tests` that reference this project.
