# WorkBeast.AdminConsole

Administrative console application for WorkBeast - a command-line tool for managing the WorkBeast application.

## Features

### Database Management

- View database statistics
- Apply pending migrations
- Seed system data (exercises and body parts)
- Seed roles and admin user
- Backup database
- Verify database integrity
- Clear all data (for development/testing)

### User Management

- List all users
- View user details
- Create new users
- Assign/remove roles (Admin, Trainer, User)
- Activate/deactivate users
- Reset user passwords
- Delete users

### Data Management

- List exercises and body parts
- Add custom exercises and body parts
- View workout sessions
- Delete non-system data
- Export data to JSON

### Configuration Management

- View current configuration
- View configuration file path
- Reset configuration to defaults
- Update application settings
- Toggle auto backup

### System Information

- View OS and .NET version
- View database and app data paths
- View machine and user information

## Usage

Run the console application:

```bash
cd src/WorkBeast.AdminConsole
dotnet run
```

Or build and run:

```bash
cd src
dotnet build WorkBeast.AdminConsole/WorkBeast.AdminConsole.csproj
dotnet run --project WorkBeast.AdminConsole/WorkBeast.AdminConsole.csproj
```

## Initial Setup

**IMPORTANT: You must initialize the database before using the admin console!**

### Quick Start (First Time)

1. **Apply database migrations**:
   ```bash
   cd src
   dotnet ef database update --project WorkBeast.Data/WorkBeast.Data.csproj --startup-project WorkBeast.Api/WorkBeast.Api.csproj
   ```

2. **Run the admin console**:
   ```bash
   cd WorkBeast.AdminConsole
   dotnet run
   ```

3. **Seed data** (from the admin console menu):
   - Database Management → Seed System Data (adds 50+ exercises and 30+ body parts)
   - Database Management → Seed Roles and Admin User (creates roles and admin account)

Default admin credentials (created during seeding):
- Email: `admin@workbeast.com`
- Password: `Admin123!`

### Alternative: Use the Setup Script

```bash
cd WorkBeast.AdminConsole
./run-first-time.sh
```

This script will:
- Check for existing database
- Apply all migrations
- Create the database at `~/.local/share/WorkBeast/workbeast.db`

Default admin credentials:

- Email: `admin@workbeast.com`
- Password: `Admin123!`

## Configuration

The console uses the same database and configuration as the main API. It reads from:

- `appsettings.json` for connection strings and JWT settings
- Platform-specific app data folder for encrypted configuration

## Dependencies

- .NET 10
- Entity Framework Core 9.0.1
- Spectre.Console (for rich terminal UI)
- Serilog (for logging)
- WorkBeast.Core and WorkBeast.Data projects

## Logging

Logs are written to:

- Console (real-time)
- `logs/admin-console-{date}.log` files

## Notes

- System entities (marked with `IsSystem = true`) cannot be deleted
- User passwords must meet minimum requirements (6 chars, uppercase, lowercase, digit, special char)
- Database backups are created in the current directory
- Data exports are saved as JSON files with timestamps
