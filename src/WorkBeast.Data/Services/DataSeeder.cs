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
            // Chest - Granular
            new() { Name = "Upper Chest", Description = "Clavicular head of pectoralis major", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Mid Chest", Description = "Sternal head of pectoralis major (middle fibers)", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Lower Chest", Description = "Sternal head of pectoralis major (lower fibers)", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Shoulders - Granular
            new() { Name = "Front Delts", Description = "Anterior deltoid muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Mid Delts", Description = "Lateral/middle deltoid muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Rear Delts", Description = "Posterior deltoid muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Back - Granular
            new() { Name = "Upper Back", Description = "Upper trapezius and rhomboids", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Mid Back", Description = "Middle trapezius, rhomboids, and mid lats", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Lower Back", Description = "Lower back, erector spinae muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Lats", Description = "Latissimus dorsi muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Traps", Description = "Trapezius muscles (upper, middle, lower)", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Arms
            new() { Name = "Biceps", Description = "Biceps brachii - front upper arm", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Triceps", Description = "Triceps brachii - back upper arm", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Forearms", Description = "Wrist flexors and extensors", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Core - Granular
            new() { Name = "Upper Abs", Description = "Upper rectus abdominis", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Lower Abs", Description = "Lower rectus abdominis", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Obliques", Description = "Internal and external obliques", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Serratus", Description = "Serratus anterior muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Legs - Granular
            new() { Name = "Quadriceps", Description = "Front thigh muscles (4 heads)", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Hamstrings", Description = "Back thigh muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Glutes", Description = "Gluteus maximus, medius, and minimus", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Hip Flexors", Description = "Iliopsoas and hip flexor muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Adductors", Description = "Inner thigh muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Abductors", Description = "Outer hip muscles (gluteus medius/minimus)", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Calves", Description = "Gastrocnemius and soleus muscles", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        await _context.BodyParts.AddRangeAsync(bodyParts);
        await _context.SaveChangesAsync(); // Save body parts first
    }

    private async Task SeedExercisesAsync()
    {
        // Get body parts for reference - they're already in the database
        var bodyParts = await _context.BodyParts.ToDictionaryAsync(bp => bp.Name);

        // Chest
        var upperChest = bodyParts["Upper Chest"];
        var midChest = bodyParts["Mid Chest"];
        var lowerChest = bodyParts["Lower Chest"];
        
        // Shoulders
        var frontDelts = bodyParts["Front Delts"];
        var midDelts = bodyParts["Mid Delts"];
        var rearDelts = bodyParts["Rear Delts"];
        
        // Back
        var upperBack = bodyParts["Upper Back"];
        var midBack = bodyParts["Mid Back"];
        var lowerBack = bodyParts["Lower Back"];
        var lats = bodyParts["Lats"];
        var traps = bodyParts["Traps"];
        
        // Arms
        var biceps = bodyParts["Biceps"];
        var triceps = bodyParts["Triceps"];
        var forearms = bodyParts["Forearms"];
        
        // Core
        var upperAbs = bodyParts["Upper Abs"];
        var lowerAbs = bodyParts["Lower Abs"];
        var obliques = bodyParts["Obliques"];
        var serratus = bodyParts["Serratus"];
        
        // Legs
        var quadriceps = bodyParts["Quadriceps"];
        var hamstrings = bodyParts["Hamstrings"];
        var glutes = bodyParts["Glutes"];
        var hipFlexors = bodyParts["Hip Flexors"];
        var adductors = bodyParts["Adductors"];
        var abductors = bodyParts["Abductors"];
        var calves = bodyParts["Calves"];

        var exercises = new List<Exercise>
        {
            // Chest Exercises
            new() 
            { 
                Name = "Flat Barbell Bench Press", 
                Description = "Barbell bench press for overall chest development",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { midChest, upperChest, frontDelts, triceps }
            },
            new() 
            { 
                Name = "Incline Barbell Bench Press", 
                Description = "Incline bench press emphasizing upper chest",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { upperChest, frontDelts, triceps }
            },
            new() 
            { 
                Name = "Decline Barbell Bench Press", 
                Description = "Decline bench press emphasizing lower chest",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { lowerChest, triceps }
            },
            new() 
            { 
                Name = "Dumbbell Fly", 
                Description = "Dumbbell chest fly for chest isolation",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { midChest, upperChest }
            },
            new() 
            { 
                Name = "Cable Crossover", 
                Description = "Cable chest crossover for chest definition",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { midChest, lowerChest }
            },
            new() 
            { 
                Name = "Push-ups", 
                Description = "Bodyweight chest exercise",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { midChest, frontDelts, triceps, serratus }
            },

            // Back Exercises
            new() 
            { 
                Name = "Deadlift", 
                Description = "Barbell deadlift for overall back and posterior chain",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { lowerBack, midBack, traps, hamstrings, glutes, forearms }
            },
            new() 
            { 
                Name = "Pull-ups", 
                Description = "Bodyweight back exercise",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { lats, midBack, biceps, forearms }
            },
            new() 
            { 
                Name = "Barbell Row", 
                Description = "Bent over barbell row for back thickness",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { midBack, lats, traps, rearDelts, biceps }
            },
            new() 
            { 
                Name = "Lat Pulldown", 
                Description = "Cable lat pulldown for lat width",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { lats, midBack, biceps }
            },
            new() 
            { 
                Name = "Face Pulls", 
                Description = "Cable face pulls for rear delts and upper back",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { rearDelts, upperBack, traps }
            },

            // Shoulder Exercises
            new() 
            { 
                Name = "Barbell Overhead Press", 
                Description = "Standing or seated barbell overhead press",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { frontDelts, midDelts, triceps, upperChest }
            },
            new() 
            { 
                Name = "Dumbbell Shoulder Press", 
                Description = "Seated dumbbell shoulder press",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { frontDelts, midDelts, triceps }
            },
            new() 
            { 
                Name = "Lateral Raise", 
                Description = "Dumbbell lateral raise for side delts",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { midDelts }
            },
            new() 
            { 
                Name = "Front Raise", 
                Description = "Dumbbell or barbell front raise",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { frontDelts }
            },
            new() 
            { 
                Name = "Reverse Fly", 
                Description = "Dumbbell or cable reverse fly for rear delts",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { rearDelts, upperBack }
            },

            // Arm Exercises
            new() 
            { 
                Name = "Barbell Curl", 
                Description = "Standing barbell bicep curl",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { biceps, forearms }
            },
            new() 
            { 
                Name = "Hammer Curl", 
                Description = "Dumbbell hammer curl for biceps and forearms",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { biceps, forearms }
            },
            new() 
            { 
                Name = "Tricep Dips", 
                Description = "Bodyweight or weighted tricep dips",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { triceps, lowerChest }
            },
            new() 
            { 
                Name = "Skull Crushers", 
                Description = "Lying tricep extension (skull crushers)",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { triceps }
            },
            new() 
            { 
                Name = "Cable Tricep Pushdown", 
                Description = "Cable tricep pushdown",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { triceps }
            },

            // Leg Exercises
            new() 
            { 
                Name = "Barbell Back Squat", 
                Description = "Barbell back squat",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { quadriceps, hamstrings, glutes, lowerBack }
            },
            new() 
            { 
                Name = "Romanian Deadlift", 
                Description = "Romanian deadlift for hamstrings and glutes",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { hamstrings, glutes, lowerBack }
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
                Name = "Walking Lunges", 
                Description = "Walking or stationary lunges",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { quadriceps, glutes, hamstrings }
            },
            new() 
            { 
                Name = "Leg Curl", 
                Description = "Lying or seated leg curl for hamstrings",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { hamstrings }
            },
            new() 
            { 
                Name = "Leg Extension", 
                Description = "Machine leg extension for quadriceps",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { quadriceps }
            },
            new() 
            { 
                Name = "Hip Thrust", 
                Description = "Barbell hip thrust for glutes",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { glutes, hamstrings }
            },
            new() 
            { 
                Name = "Standing Calf Raise", 
                Description = "Standing calf raise on machine or Smith machine",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { calves }
            },

            // Core Exercises
            new() 
            { 
                Name = "Crunches", 
                Description = "Bodyweight abdominal crunches",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { upperAbs }
            },
            new() 
            { 
                Name = "Reverse Crunches", 
                Description = "Reverse crunches for lower abs",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { lowerAbs }
            },
            new() 
            { 
                Name = "Plank", 
                Description = "Static core hold exercise",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { upperAbs, lowerAbs, obliques, serratus }
            },
            new() 
            { 
                Name = "Russian Twists", 
                Description = "Seated twisting motion for obliques",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { obliques, upperAbs }
            },
            new() 
            { 
                Name = "Hanging Leg Raise", 
                Description = "Hanging leg raise for lower abs and hip flexors",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { lowerAbs, hipFlexors, serratus }
            }
        };

        await _context.Exercises.AddRangeAsync(exercises);
    }
}
