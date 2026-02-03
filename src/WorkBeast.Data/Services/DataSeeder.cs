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
            
            // Arms - Granular
            // Biceps (2 heads)
            new() { Name = "Biceps Long Head", Description = "Outer bicep head (contributes to peak)", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Biceps Short Head", Description = "Inner bicep head (contributes to width)", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Brachialis", Description = "Muscle under biceps (contributes to arm thickness)", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Triceps (3 heads)
            new() { Name = "Triceps Long Head", Description = "Inner/medial tricep head (largest head)", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Triceps Lateral Head", Description = "Outer tricep head (horseshoe shape)", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Triceps Medial Head", Description = "Deep tricep head (stabilizer)", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Forearms
            new() { Name = "Forearm Flexors", Description = "Wrist and finger flexors (palm side)", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Forearm Extensors", Description = "Wrist and finger extensors (back side)", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Name = "Brachioradialis", Description = "Upper forearm muscle (elbow flexor)", IsSystem = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
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
        var bicepsLongHead = bodyParts["Biceps Long Head"];
        var bicepsShortHead = bodyParts["Biceps Short Head"];
        var brachialis = bodyParts["Brachialis"];
        var tricepsLongHead = bodyParts["Triceps Long Head"];
        var tricepsLateralHead = bodyParts["Triceps Lateral Head"];
        var tricepsMedialHead = bodyParts["Triceps Medial Head"];
        var forearmFlexors = bodyParts["Forearm Flexors"];
        var forearmExtensors = bodyParts["Forearm Extensors"];
        var brachioradialis = bodyParts["Brachioradialis"];
        
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
                TargetedBodyParts = new List<BodyPart> { midChest, upperChest, frontDelts, tricepsLateralHead, tricepsMedialHead }
            },
            new() 
            { 
                Name = "Incline Barbell Bench Press", 
                Description = "Incline bench press emphasizing upper chest",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { upperChest, frontDelts, tricepsLateralHead, tricepsMedialHead }
            },
            new() 
            { 
                Name = "Decline Barbell Bench Press", 
                Description = "Decline bench press emphasizing lower chest",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { lowerChest, tricepsLateralHead, tricepsMedialHead }
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
                TargetedBodyParts = new List<BodyPart> { midChest, frontDelts, tricepsLateralHead, tricepsMedialHead, serratus }
            },

            // Back Exercises
            new() 
            { 
                Name = "Deadlift", 
                Description = "Barbell deadlift for overall back and posterior chain",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { lowerBack, midBack, traps, hamstrings, glutes, forearmFlexors, forearmExtensors }
            },
            new() 
            { 
                Name = "Pull-ups", 
                Description = "Bodyweight back exercise",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { lats, midBack, bicepsLongHead, bicepsShortHead, forearmFlexors }
            },
            new() 
            { 
                Name = "Barbell Row", 
                Description = "Bent over barbell row for back thickness",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { midBack, lats, traps, rearDelts, bicepsLongHead, bicepsShortHead }
            },
            new() 
            { 
                Name = "Lat Pulldown", 
                Description = "Cable lat pulldown for lat width",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { lats, midBack, bicepsLongHead, bicepsShortHead }
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
                TargetedBodyParts = new List<BodyPart> { frontDelts, midDelts, tricepsLateralHead, tricepsMedialHead, upperChest }
            },
            new() 
            { 
                Name = "Dumbbell Shoulder Press", 
                Description = "Seated dumbbell shoulder press",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { frontDelts, midDelts, tricepsLateralHead, tricepsMedialHead }
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

            // Arm Exercises - Biceps
            new() 
            { 
                Name = "Barbell Curl", 
                Description = "Standing barbell bicep curl - emphasizes both heads",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { bicepsLongHead, bicepsShortHead, forearmFlexors }
            },
            new() 
            { 
                Name = "Incline Dumbbell Curl", 
                Description = "Incline bench dumbbell curl - emphasizes long head (peak)",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { bicepsLongHead, bicepsShortHead, forearmFlexors }
            },
            new() 
            { 
                Name = "Preacher Curl", 
                Description = "Preacher curl - emphasizes short head (width) and brachialis",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { bicepsShortHead, brachialis, forearmFlexors }
            },
            new() 
            { 
                Name = "Hammer Curl", 
                Description = "Dumbbell hammer curl - emphasizes brachialis and brachioradialis",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { brachialis, brachioradialis, bicepsLongHead, forearmFlexors }
            },
            new() 
            { 
                Name = "Concentration Curl", 
                Description = "Seated concentration curl - peak contraction for bicep heads",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { bicepsShortHead, bicepsLongHead }
            },
            new() 
            { 
                Name = "Cable Curl", 
                Description = "Cable bicep curl - constant tension on both heads",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { bicepsLongHead, bicepsShortHead, forearmFlexors }
            },

            // Arm Exercises - Triceps
            new() 
            { 
                Name = "Close-Grip Bench Press", 
                Description = "Close-grip bench press - compound movement for all tricep heads",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { tricepsLateralHead, tricepsMedialHead, tricepsLongHead, midChest }
            },
            new() 
            { 
                Name = "Tricep Dips", 
                Description = "Bodyweight or weighted tricep dips - emphasizes lateral and long head",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { tricepsLateralHead, tricepsLongHead, tricepsMedialHead, lowerChest }
            },
            new() 
            { 
                Name = "Overhead Tricep Extension", 
                Description = "Overhead extension - emphasizes long head (inner tricep)",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { tricepsLongHead, tricepsLateralHead, tricepsMedialHead }
            },
            new() 
            { 
                Name = "Skull Crushers", 
                Description = "Lying tricep extension - targets all three heads with emphasis on long head",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { tricepsLongHead, tricepsLateralHead, tricepsMedialHead }
            },
            new() 
            { 
                Name = "Cable Tricep Pushdown", 
                Description = "Cable pushdown - emphasizes lateral head (horseshoe)",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { tricepsLateralHead, tricepsMedialHead }
            },
            new() 
            { 
                Name = "Diamond Push-ups", 
                Description = "Close-hand push-ups - bodyweight tricep builder",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { tricepsLateralHead, tricepsMedialHead, tricepsLongHead, midChest }
            },
            new() 
            { 
                Name = "Tricep Kickback", 
                Description = "Dumbbell tricep kickback - isolation for lateral head",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { tricepsLateralHead, tricepsLongHead }
            },

            // Arm Exercises - Forearms
            new() 
            { 
                Name = "Wrist Curl", 
                Description = "Seated wrist curl - targets forearm flexors",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { forearmFlexors }
            },
            new() 
            { 
                Name = "Reverse Wrist Curl", 
                Description = "Reverse wrist curl - targets forearm extensors",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { forearmExtensors, brachioradialis }
            },
            new() 
            { 
                Name = "Farmer's Walk", 
                Description = "Heavy carry for grip strength and forearm development",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { forearmFlexors, forearmExtensors, traps, upperBack }
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
            },

            // Full Body / Compound Exercises
            new() 
            { 
                Name = "Burpees", 
                Description = "Full body explosive movement - cardio and strength",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { quadriceps, glutes, midChest, tricepsLateralHead, upperAbs, lowerAbs }
            },
            new() 
            { 
                Name = "Thrusters", 
                Description = "Front squat to overhead press - full body power",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { quadriceps, glutes, frontDelts, midDelts, tricepsLateralHead, upperAbs }
            },
            new() 
            { 
                Name = "Clean and Press", 
                Description = "Olympic lift variation - explosive full body power",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { traps, lowerBack, hamstrings, glutes, frontDelts, midDelts, tricepsLateralHead, forearmFlexors }
            },
            new() 
            { 
                Name = "Man Makers", 
                Description = "Dumbbell burpee + renegade row + squat thrust",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { midBack, lats, midChest, tricepsLateralHead, quadriceps, glutes, upperAbs, frontDelts }
            },
            new() 
            { 
                Name = "Turkish Get-Up", 
                Description = "Full body kettlebell/dumbbell movement - stability and strength",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { frontDelts, midDelts, upperAbs, obliques, glutes, quadriceps, tricepsLongHead }
            },
            new() 
            { 
                Name = "Medicine Ball Slams", 
                Description = "Explosive full body power - overhead slam",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { upperAbs, lats, frontDelts, tricepsLateralHead, glutes, quadriceps }
            },
            new() 
            { 
                Name = "Battle Ropes", 
                Description = "Wave motion rope training - cardio and upper body endurance",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { frontDelts, midDelts, forearmFlexors, upperAbs, obliques, quadriceps }
            },
            new() 
            { 
                Name = "Box Jumps", 
                Description = "Plyometric lower body power exercise",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { quadriceps, glutes, calves, hamstrings }
            },
            new() 
            { 
                Name = "Sled Push", 
                Description = "Heavy sled pushing - lower body and conditioning",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { quadriceps, glutes, calves, upperAbs, tricepsLateralHead }
            },
            new() 
            { 
                Name = "Sled Pull", 
                Description = "Heavy sled pulling - posterior chain and back",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { hamstrings, glutes, lats, midBack, bicepsLongHead, forearmFlexors }
            },
            new() 
            { 
                Name = "Rowing Machine", 
                Description = "Full body cardio with emphasis on back and legs",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { lats, midBack, bicepsLongHead, quadriceps, hamstrings, glutes }
            },
            new() 
            { 
                Name = "Assault Bike", 
                Description = "Full body cardio - arms and legs",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { quadriceps, hamstrings, tricepsLateralHead, frontDelts }
            },
            new() 
            { 
                Name = "Wall Balls", 
                Description = "Squat + overhead throw - full body conditioning",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { quadriceps, glutes, frontDelts, midDelts, tricepsLateralHead, upperAbs }
            },
            new() 
            { 
                Name = "Kettlebell Swings", 
                Description = "Hip hinge explosive movement - posterior chain power",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { glutes, hamstrings, lowerBack, frontDelts, forearmFlexors }
            },
            new() 
            { 
                Name = "Bear Crawls", 
                Description = "Quadruped movement - full body stability and endurance",
                IsSystem = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TargetedBodyParts = new List<BodyPart> { frontDelts, tricepsMedialHead, upperAbs, quadriceps, hipFlexors }
            }
        };

        await _context.Exercises.AddRangeAsync(exercises);
    }
}
