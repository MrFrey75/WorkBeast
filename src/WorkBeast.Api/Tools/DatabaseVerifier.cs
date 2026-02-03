using Microsoft.EntityFrameworkCore;
using WorkBeast.Data.Context;

namespace WorkBeast.Api.Tools;

/// <summary>
/// Tool to verify database setup and seeding
/// </summary>
public class DatabaseVerifier
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseVerifier> _logger;

    public DatabaseVerifier(AppDbContext context, ILogger<DatabaseVerifier> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Verifies database setup and prints statistics
    /// </summary>
    public async Task VerifyAsync()
    {
        _logger.LogInformation("=== Database Verification ===");

        try
        {
            // Check database connection
            var canConnect = await _context.Database.CanConnectAsync();
            _logger.LogInformation("Database Connection: {Status}", canConnect ? "✓ OK" : "✗ Failed");

            if (!canConnect)
            {
                _logger.LogError("Cannot connect to database");
                return;
            }

            // Check migrations
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();
            
            _logger.LogInformation("Applied Migrations: {Count}", appliedMigrations.Count());
            _logger.LogInformation("Pending Migrations: {Count}", pendingMigrations.Count());

            if (pendingMigrations.Any())
            {
                _logger.LogWarning("Pending migrations detected: {Migrations}", 
                    string.Join(", ", pendingMigrations));
            }

            // Check data
            var bodyPartsCount = await _context.BodyParts.CountAsync();
            var exercisesCount = await _context.Exercises.CountAsync();
            var usersCount = await _context.Users.CountAsync();
            var rolesCount = await _context.Roles.CountAsync();
            var workoutSessionsCount = await _context.WorkoutSessions.CountAsync();

            _logger.LogInformation("=== Data Statistics ===");
            _logger.LogInformation("Body Parts: {Count}", bodyPartsCount);
            _logger.LogInformation("Exercises: {Count}", exercisesCount);
            _logger.LogInformation("Users: {Count}", usersCount);
            _logger.LogInformation("Roles: {Count}", rolesCount);
            _logger.LogInformation("Workout Sessions: {Count}", workoutSessionsCount);

            // Check system data
            var systemBodyParts = await _context.BodyParts.CountAsync(bp => bp.IsSystem);
            var systemExercises = await _context.Exercises.CountAsync(e => e.IsSystem);
            var systemRoles = await _context.Roles.CountAsync(r => r.IsSystem);

            _logger.LogInformation("=== System Data ===");
            _logger.LogInformation("System Body Parts: {Count}", systemBodyParts);
            _logger.LogInformation("System Exercises: {Count}", systemExercises);
            _logger.LogInformation("System Roles: {Count}", systemRoles);

            // List sample exercises
            var sampleExercises = await _context.Exercises
                .Include(e => e.TargetedBodyParts)
                .Take(5)
                .ToListAsync();

            if (sampleExercises.Any())
            {
                _logger.LogInformation("=== Sample Exercises ===");
                foreach (var exercise in sampleExercises)
                {
                    var bodyParts = string.Join(", ", exercise.TargetedBodyParts.Select(bp => bp.Name));
                    _logger.LogInformation("{Name} - Targets: {BodyParts}", exercise.Name, bodyParts);
                }
            }

            _logger.LogInformation("=== Verification Complete ===");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during database verification");
        }
    }
}
