using Microsoft.EntityFrameworkCore;
using WorkBeast.Data.Context;
using WorkBeast.Data.Services;
using Xunit;

namespace WorkBeast.Tests.Services;

[Collection("FileSystem")]
public class DataSeederTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly DataSeeder _seeder;
    private readonly string _dbName;

    public DataSeederTests()
    {
        _dbName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _dbName)
            .Options;

        _context = new AppDbContext(options);
        _seeder = new DataSeeder(_context);
    }

    [Fact]
    public async Task SeedAsync_FirstTime_SeedsBodyPartsAndExercises()
    {
        // Act
        await _seeder.SeedAsync();

        // Assert
        var bodyParts = await _context.BodyParts.ToListAsync();
        var exercises = await _context.Exercises.ToListAsync();

        Assert.NotEmpty(bodyParts);
        Assert.NotEmpty(exercises);
        Assert.True(bodyParts.Count >= 31); // At least 31 granular body parts (expanded arms)
        Assert.True(exercises.Count >= 60); // At least 60 exercises (more arm + full body)
        Assert.All(bodyParts, bp => Assert.True(bp.IsSystem));
        Assert.All(exercises, ex => Assert.True(ex.IsSystem));
    }

    [Fact]
    public async Task SeedAsync_SecondTime_DoesNotDuplicateData()
    {
        // Arrange - Seed first time
        await _seeder.SeedAsync();
        var firstCount = await _context.Exercises.CountAsync();

        // Act - Seed second time
        await _seeder.SeedAsync();

        // Assert - Should not add more data
        var secondCount = await _context.Exercises.CountAsync();
        Assert.Equal(firstCount, secondCount);
    }

    [Fact]
    public async Task SeedAsync_CreatesExercisesWithBodyPartRelationships()
    {
        // Act
        await _seeder.SeedAsync();

        // Assert
        var benchPress = await _context.Exercises
            .Include(e => e.TargetedBodyParts)
            .FirstOrDefaultAsync(e => e.Name == "Flat Barbell Bench Press");

        Assert.NotNull(benchPress);
        Assert.NotEmpty(benchPress.TargetedBodyParts);
        Assert.Contains(benchPress.TargetedBodyParts, bp => bp.Name == "Mid Chest");
    }

    [Fact]
    public async Task SeedAsync_AllBodyPartsHaveRequiredFields()
    {
        // Act
        await _seeder.SeedAsync();

        // Assert
        var bodyParts = await _context.BodyParts.ToListAsync();
        
        Assert.All(bodyParts, bp =>
        {
            Assert.False(string.IsNullOrWhiteSpace(bp.Name));
            Assert.False(string.IsNullOrWhiteSpace(bp.Description));
            Assert.True(bp.IsSystem);
            Assert.False(bp.IsDeleted);
        });
    }

    [Fact]
    public async Task SeedAsync_AllExercisesHaveRequiredFields()
    {
        // Act
        await _seeder.SeedAsync();

        // Assert
        var exercises = await _context.Exercises
            .Include(e => e.TargetedBodyParts)
            .ToListAsync();
        
        Assert.All(exercises, ex =>
        {
            Assert.False(string.IsNullOrWhiteSpace(ex.Name));
            Assert.False(string.IsNullOrWhiteSpace(ex.Description));
            Assert.True(ex.IsSystem);
            Assert.False(ex.IsDeleted);
            Assert.NotEmpty(ex.TargetedBodyParts);
        });
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
