# WorkBeast

WorkBeast is a workout management application designed to help users plan, track, and analyze their fitness routines. It offers a user-friendly interface and a variety of features to enhance the workout experience.

## Features

- **Workout Planning**: Create and customize workout plans based on your fitness goals.
- **Exercise Library**: Access a comprehensive library of exercises with detailed instructions.
- **Progress Tracking**: Monitor your progress with charts and statistics.
- **Workout Logging**: Track standard sets, supersets, drop sets, bodyweight exercises, and more.
- **RESTful API**: Access your workout data programmatically through a robust API.

## Tech Stack

WorkBeast is a .NET application built using the following technologies:

- .NET 8
- C#
- Entity Framework Core 8
- SQLite
- ASP.NET Core Web API
- Swagger/OpenAPI

## Solution Structure

The solution is organized into the following projects:

- **`WorkBeast.Core`**: Contains the core business logic and domain models.
  - Models: `Exercise`, `BodyPart`, `WorkoutSession`, `LoggedExercise`, `WorkoutSet`
  - Base entities with soft-delete support and audit tracking

- **`WorkBeast.Data`**: Handles data access and database interactions.
  - `AppDbContext` with Entity Framework Core configurations
  - Relationship mappings and indexes for optimal performance

- **`WorkBeast.Api`**: ASP.NET Core Web API project.
  - RESTful endpoints for exercises and workout sessions
  - Swagger UI for API documentation and testing
  - CORS support for frontend integration

- **`WorkBeast.Web`**: The web application project (coming soon).

- **`WorkBeast.Tests`**: Unit and integration tests (coming soon).

## Domain Model

### Core Entities

**Exercise**
- Name, Description
- Many-to-many relationship with BodyParts
- Support for search and filtering

**WorkoutSession**
- Date, Notes
- Contains multiple LoggedExercises
- Represents a single gym visit

**LoggedExercise**
- Links Exercise to WorkoutSession
- Sequence/ParentGroupId for supersets and rounds
- Contains multiple WorkoutSets

**WorkoutSet**
- Weight (0 for bodyweight), Reps
- SetType: Normal, Drop Set, Warmup
- ResistanceLevel for machines/bands
- SortOrder for proper sequencing

### Supported Workout Scenarios

✅ **Standard Sets**: Multiple sets with varying weights and reps  
✅ **Supersets/Rounds**: Grouped exercises with shared sequence IDs  
✅ **Bodyweight Exercises**: Weight = 0  
✅ **Drop Sets**: Identified by SetType property  
✅ **Machine/Band Resistance**: Dedicated ResistanceLevel field  

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQLite (included with .NET)

### Clone the Repository

```bash
git clone https://github.com/MrFrey75/WorkBeast.git
cd WorkBeast
```

### Build the Solution

```bash
cd src
dotnet restore
dotnet build
```

### Run the API

```bash
cd WorkBeast.Api
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`

### Create Database Migrations

```bash
cd WorkBeast.Data
dotnet ef migrations add InitialCreate --startup-project ../WorkBeast.Api
dotnet ef database update --startup-project ../WorkBeast.Api
```

## API Endpoints

### Exercises

- `GET /api/exercises` - Get all exercises (optional `?search=` parameter)
- `GET /api/exercises/{id}` - Get exercise by ID
- `POST /api/exercises` - Create new exercise
- `PUT /api/exercises/{id}` - Update exercise
- `DELETE /api/exercises/{id}` - Soft delete exercise

### Workout Sessions

- `GET /api/workoutsessions` - Get all sessions (optional `?startDate=` and `?endDate=` filters)
- `GET /api/workoutsessions/{id}` - Get session by ID with exercises and sets
- `POST /api/workoutsessions` - Create new workout session
- `PUT /api/workoutsessions/{id}` - Update workout session
- `DELETE /api/workoutsessions/{id}` - Soft delete session

## Development

### Project Dependencies

- **WorkBeast.Api** → WorkBeast.Data → WorkBeast.Core
- All projects target .NET 8
- Entity Framework Core 8.0.5
- Swagger/Swashbuckle for API documentation

### Database

The application uses SQLite with the connection string configured in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=workbeast.db"
}
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

Arthur Frey - [GitHub](https://github.com/MrFrey75)