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

**On first run, the application automatically:**
- Creates application directories in platform-specific locations
- Applies database migrations
- Seeds the database with system exercises and body parts
- Generates configuration and README files

### Application Data Location

The application stores data in platform-specific directories:

- **Windows**: `%LOCALAPPDATA%\WorkBeast\`
- **Linux**: `~/.local/share/WorkBeast/`
- **macOS**: `~/.local/share/WorkBeast/`

Directory structure:
```
WorkBeast/
├── workbeast.db          # SQLite database
├── config.json           # Application configuration
├── README.txt            # User guide
├── Logs/                 # Application logs
├── Backups/              # Database backups
├── Exports/              # Exported workout data
└── Imports/              # Import staging area
```

### Create Database Migrations (Optional)

Migrations are applied automatically on startup. To create new migrations:

```bash
cd WorkBeast.Data
dotnet ef migrations add YourMigrationName --startup-project ../WorkBeast.Api
```

## API Endpoints

### Authentication (Public)

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and receive JWT token
- `GET /api/auth/me` - Get current user profile (requires authentication)
- `POST /api/auth/change-password` - Change password (requires authentication)

### User Management (Admin Only)

- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users/assign-role` - Assign role to user
- `POST /api/users/remove-role` - Remove role from user
- `POST /api/users/{id}/activate` - Activate user account
- `POST /api/users/{id}/deactivate` - Deactivate user account

### Exercises (Requires Authentication)

- `GET /api/exercises` - Get all exercises (optional `?search=` parameter)
- `GET /api/exercises/{id}` - Get exercise by ID
- `POST /api/exercises` - Create new exercise
- `PUT /api/exercises/{id}` - Update exercise
- `DELETE /api/exercises/{id}` - Soft delete exercise

### Workout Sessions (Requires Authentication)

- `GET /api/workoutsessions` - Get all sessions (optional `?startDate=` and `?endDate=` filters)
- `GET /api/workoutsessions/{id}` - Get session by ID with exercises and sets
- `POST /api/workoutsessions` - Create new workout session
- `PUT /api/workoutsessions/{id}` - Update workout session
- `DELETE /api/workoutsessions/{id}` - Soft delete session

## Authentication & Authorization

### JWT Token-Based Authentication

WorkBeast uses JWT (JSON Web Tokens) for secure API authentication.

**Default Admin Credentials:**
- Email: `admin@workbeast.com`
- Password: `Admin@123`
- **⚠️ IMPORTANT**: Change this password immediately after first login!

### User Roles

- **Admin**: Full system access, user management, and all features
- **Trainer**: Can manage workout plans and exercises
- **User**: Standard user access to personal workouts

### Using the API with Authentication

1. **Register or Login** to receive a JWT token
2. **Include the token** in subsequent requests:
   ```
   Authorization: Bearer YOUR_JWT_TOKEN
   ```
3. Tokens expire after 7 days

**Example (using curl):**
```bash
# Login
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@workbeast.com","password":"Admin@123"}'

# Use token
curl -X GET https://localhost:5001/api/exercises \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Development

### Initialization Services

The application includes automatic initialization services:

**ApplicationInitializer**
- Creates required directories (Logs, Backups, Exports, Imports)
- Generates default configuration file
- Creates user documentation
- Platform-aware path resolution

**DataSeeder**
- Seeds database with 16 system exercises
- Creates 14 body part categories
- Links exercises to target muscle groups
- Only runs on empty database

**RoleSeeder**
- Creates default roles (Admin, Trainer, User)
- Seeds default admin account
- Configures role-based permissions

### Security Features

**ASP.NET Core Identity**
- Password hashing with PBKDF2
- Configurable password requirements
- Account lockout after failed attempts
- Email confirmation ready

**JWT Authentication**
- Secure token-based authentication
- 7-day token expiration
- Role-based authorization
- Claims-based identity

### Project Dependencies

- **WorkBeast.Api** → WorkBeast.Data → WorkBeast.Core
- All projects target .NET 8
- Entity Framework Core 8.0.5
- Swagger/Swashbuckle for API documentation

### Database

The application uses SQLite with platform-specific database location:
- Database file: `workbeast.db`
- Migrations are applied automatically on startup
- Initial seed data includes common exercises and body parts

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

Arthur Frey - [GitHub](https://github.com/MrFrey75)