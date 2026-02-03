using Microsoft.EntityFrameworkCore;
using WorkBeast.Core.Models;
using WorkBeast.Core.Services;
using WorkBeast.Data.Context;

namespace WorkBeast.Data.Services;

/// <summary>
/// Service for seeding initial data into the database
/// </summary>
public class DataSeeder : IDataSeeder
{
    private readonly AppDbContext _context;

    public DataSeeder(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Seeds the database with initial data including system exercises and body parts
    /// </summary>
    public async Task SeedAsync()
    {
        // Check if data already exists
        if (await _context.Exercises.AnyAsync() || await _context.BodyParts.AnyAsync())
        {
            return; // Database already seeded
        }

        await SeedBodyPartsAsync();
        await SeedExercisesAsync();
        // Body parts are already saved in SeedBodyPartsAsync
        // Exercises will be saved here
        await _context.SaveChangesAsync();
    }

    private async Task SeedBodyPartsAsync()
    {
        var bodyParts = new List<BodyPart>
        {
            new() { Name = "Chest", Description = "Pectoral muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Back", Description = "Upper and lower back muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Shoulders", Description = "Deltoid muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Biceps", Description = "Front upper arm muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Triceps", Description = "Back upper arm muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Forearms", Description = "Lower arm muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Abs", Description = "Abdominal muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Obliques", Description = "Side abdominal muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Quadriceps", Description = "Front thigh muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Hamstrings", Description = "Back thigh muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Glutes", Description = "Gluteal muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Calves", Description = "Lower leg muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Traps", Description = "Trapezius muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Lats", Description = "Latissimus dorsi muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        await _context.BodyParts.AddRangeAsync(bodyParts);
        await _context.SaveChangesAsync(); // Save body parts first
    }

    private async Task SeedExercisesAsync()
    {
        // Get body parts for reference - they're already in the database
        var bodyParts = await _context.BodyParts.ToDictionaryAsync(bp => bp.Name);

        var chest = bodyParts["Chest"];
        var back = bodyParts["Back"];
        var shoulders = bodyParts["Shoulders"];
        var biceps = bodyParts["Biceps"];
        var triceps = bodyParts["Triceps"];
        var abs = bodyParts["Abs"];
        var quadriceps = bodyParts["Quadriceps"];
        var hamstrings = bodyParts["Hamstrings"];
        var glutes = bodyParts["Glutes"];
        var calves = bodyParts["Calves"];
        var traps = bodyParts["Traps"];
        var lats = bodyParts["Lats"];

        var exercises = new List<Exercise>
        {
            // Chest
            new() 
            { 
                Name = "Bench Press", 
                Description = "Barbell bench press for chest development",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { chest, shoulders, triceps }
            },
            new() 
            { 
                Name = "Dumbbell Fly", 
                Description = "Dumbbell chest fly for chest isolation",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { chest }
            },
            new() 
            { 
                Name = "Push-ups", 
                Description = "Bodyweight chest exercise",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { chest, shoulders, triceps }
            },

            // Back
            new() 
            { 
                Name = "Deadlift", 
                Description = "Barbell deadlift for overall back and posterior chain",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { back, hamstrings, glutes, traps }
            },
            new() 
            { 
                Name = "Pull-ups", 
                Description = "Bodyweight back exercise",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { lats, biceps }
            },
            new() 
            { 
                Name = "Barbell Row", 
                Description = "Bent over barbell row for back thickness",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { back, lats, biceps }
            },

            // Shoulders
            new() 
            { 
                Name = "Overhead Press", 
                Description = "Barbell or dumbbell overhead press",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { shoulders, triceps }
            },
            new() 
            { 
                Name = "Lateral Raise", 
                Description = "Dumbbell lateral raise for side delts",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { shoulders }
            },

            // Arms
            new() 
            { 
                Name = "Barbell Curl", 
                Description = "Barbell bicep curl",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { biceps }
            },
            new() 
            { 
                Name = "Tricep Dips", 
                Description = "Bodyweight or weighted tricep dips",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { triceps, chest }
            },

            // Legs
            new() 
            { 
                Name = "Squat", 
                Description = "Barbell back squat",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { quadriceps, hamstrings, glutes }
            },
            new() 
            { 
                Name = "Leg Press", 
                Description = "Machine leg press",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { quadriceps, glutes }
            },
            new() 
            { 
                Name = "Lunges", 
                Description = "Walking or stationary lunges",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { quadriceps, glutes, hamstrings }
            },
            new() 
            { 
                Name = "Calf Raise", 
                Description = "Standing or seated calf raise",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { calves }
            },

            // Core
            new() 
            { 
                Name = "Crunches", 
                Description = "Bodyweight abdominal crunches",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { abs }
            },
            new() 
            { 
                Name = "Plank", 
                Description = "Static core hold exercise",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { abs }
            }
        };

        await _context.Exercises.AddRangeAsync(exercises);
    }
}
