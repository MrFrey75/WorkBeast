using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkBeast.Api.Controllers;
using WorkBeast.Core.Models;
using WorkBeast.Data.Context;
using Xunit;

namespace WorkBeast.Tests.Controllers;

[Collection("FileSystem")]
public class ExercisesControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ExercisesController _controller;

    public ExercisesControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _controller = new ExercisesController(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var exercises = new List<Exercise>
        {
            new Exercise
            {
                Oid = 1,
                Name = "Bench Press",
                Description = "Chest exercise",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false,
                IsSystem = false
            },
            new Exercise
            {
                Oid = 2,
                Name = "Squat",
                Description = "Leg exercise",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false,
                IsSystem = true
            },
            new Exercise
            {
                Oid = 3,
                Name = "Deleted Exercise",
                Description = "This is deleted",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = true,
                IsSystem = false
            }
        };

        _context.Exercises.AddRange(exercises);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetExercises_ReturnsAllActiveExercises()
    {
        var result = await _controller.GetExercises();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var exercises = Assert.IsAssignableFrom<IEnumerable<Exercise>>(okResult.Value);
        Assert.Equal(2, exercises.Count());
    }

    [Fact]
    public async Task GetExercise_WithValidId_ReturnsExercise()
    {
        var result = await _controller.GetExercise(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var exercise = Assert.IsType<Exercise>(okResult.Value);
        Assert.Equal("Bench Press", exercise.Name);
    }

    [Fact]
    public async Task GetExercise_WithInvalidId_ReturnsNotFound()
    {
        var result = await _controller.GetExercise(999);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateExercise_WithValidData_ReturnsCreatedExercise()
    {
        var newExercise = new Exercise
        {
            Name = "Deadlift",
            Description = "Back exercise"
        };

        var result = await _controller.CreateExercise(newExercise);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var exercise = Assert.IsType<Exercise>(createdResult.Value);
        Assert.Equal("Deadlift", exercise.Name);
    }

    [Fact]
    public async Task DeleteExercise_WithSystemExercise_ReturnsBadRequest()
    {
        var result = await _controller.DeleteExercise(2);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
