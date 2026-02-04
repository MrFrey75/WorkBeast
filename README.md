# WorkBeast

WorkBeast is a workout management application designed to help users plan, track, and analyze their fitness routines. It offers a user-friendly interface and a variety of features to enhance the workout experience.

## Features

- **Workout Planning**: Create and customize workout plans based on your fitness goals.
- **Exercise Library**: Access a comprehensive library of exercises with detailed instructions and videos.
- **Progress Tracking**: Monitor your progress with charts and statistics.
- **Social Sharing**: Share your achievements and workout plans with friends.
- **Notifications**: Set reminders for your workouts to stay on track.

## Tech Stack

WorkBeast is a .NET application built using the following technologies:

- .NET 10
- C#
- Entity Framework Core 9
- Dependency Injection
- Serilog for logging
- AutoMapper for object mapping
- xUnit for testing
- SQLite
- ASP.NET Core Web API
- JWT Authentication
- Spectre.Console for CLI

### Solution Structure

The solution is organized into the following projects:

- `WorkBeast.Core`: Contains the core business logic and models.
- `WorkBeast.Data`: Handles data access and database interactions.
- `WorkBeast.Api`: The Web API project that serves the backend REST API.
- `WorkBeast.AdminConsole`: A console application for administrative tasks.
- `WorkBeast.Tests`: Contains unit and integration tests for the application.

## Getting Started

To get started with WorkBeast, follow these steps:

1. Clone the repository:

   ```bash
   git clone https://github.com/MrFrey75/WorkBeast.git
   cd WorkBeast
   ```

2. Build the solution:

   ```bash
   cd src
   dotnet restore
   dotnet build
   ```

3. Initialize the database using the Admin Console:

   ```bash
   cd WorkBeast.AdminConsole
   dotnet run
   ```

   From the admin console menu:
   - Database Management → Apply Pending Migrations
   - Database Management → Seed System Data
   - Database Management → Seed Roles and Admin User

4. Run the API:

   ```bash
   cd ../WorkBeast.Api
   dotnet run
   ```

   The API will be available at `http://localhost:5202` (or as configured in launchSettings.json).

5. Access the Swagger UI at `http://localhost:5202/swagger` for API documentation.

## Administrative Console

The `WorkBeast.AdminConsole` provides a command-line interface for:

- **Database Management**: Migrations, seeding, backups, and integrity checks
- **User Management**: Create, modify, activate/deactivate users and manage roles
- **Data Management**: View and manage exercises, body parts, and workout sessions
- **Configuration Management**: View and modify application configuration
- **System Information**: View system details and paths

Run the admin console with:

```bash
cd src/WorkBeast.AdminConsole
dotnet run
```

Default admin credentials (created during seeding):
- Email: `admin@workbeast.com`
- Password: `Admin123!`

## API Documentation

The API provides endpoints for:

- **Authentication**: `/api/auth` - Register, login, profile management
- **Exercises**: `/api/exercises` - CRUD operations for exercises
- **Body Parts**: `/api/bodyparts` - Manage targeted body parts
- **Workout Sessions**: `/api/workoutsessions` - Track workouts
- **Users**: `/api/users` - User management (admin only)
- **Configuration**: `/api/configuration` - Application configuration
- **Health**: `/api/health` - Health check endpoint

All endpoints except registration and login require JWT authentication.

