using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkBeast.Data.Context;

namespace WorkBeast.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(AppDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                return StatusCode(503, new { status = "Unhealthy", database = "Cannot connect" });
            }

            var bodyPartsCount = await _context.BodyParts.CountAsync();
            var exercisesCount = await _context.Exercises.CountAsync();

            return Ok(new
            {
                status = "Healthy",
                database = "Connected",
                timestamp = DateTime.UtcNow,
                data = new
                {
                    bodyParts = bodyPartsCount,
                    exercises = exercisesCount
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(503, new { status = "Unhealthy", error = ex.Message });
        }
    }

    /// <summary>
    /// Database statistics endpoint
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var stats = new
            {
                bodyParts = await _context.BodyParts.CountAsync(),
                exercises = await _context.Exercises.CountAsync(),
                users = await _context.Users.CountAsync(),
                roles = await _context.Roles.CountAsync(),
                workoutSessions = await _context.WorkoutSessions.CountAsync(),
                systemData = new
                {
                    bodyParts = await _context.BodyParts.CountAsync(bp => bp.IsSystem),
                    exercises = await _context.Exercises.CountAsync(e => e.IsSystem),
                    roles = await _context.Roles.CountAsync(r => r.IsSystem)
                },
                migrations = new
                {
                    applied = (await _context.Database.GetAppliedMigrationsAsync()).Count(),
                    pending = (await _context.Database.GetPendingMigrationsAsync()).Count()
                }
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get database statistics");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
