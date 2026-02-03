using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkBeast.Core.Models;
using WorkBeast.Data.Context;

namespace WorkBeast.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutSessionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public WorkoutSessionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutSession>>> GetWorkoutSessions(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var query = _context.WorkoutSessions
            .Include(ws => ws.LoggedExercises)
                .ThenInclude(le => le.Exercise)
            .Include(ws => ws.LoggedExercises)
                .ThenInclude(le => le.Sets)
            .AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(ws => ws.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(ws => ws.Date <= endDate.Value);
        }

        var sessions = await query
            .Where(ws => !ws.IsDeleted)
            .OrderByDescending(ws => ws.Date)
            .ToListAsync();

        return Ok(sessions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutSession>> GetWorkoutSession(int id)
    {
        var session = await _context.WorkoutSessions
            .Include(ws => ws.LoggedExercises)
                .ThenInclude(le => le.Exercise)
            .Include(ws => ws.LoggedExercises)
                .ThenInclude(le => le.Sets)
            .FirstOrDefaultAsync(ws => ws.Oid == id && !ws.IsDeleted);

        if (session == null)
        {
            return NotFound();
        }

        return Ok(session);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutSession>> CreateWorkoutSession(WorkoutSession session)
    {
        session.CreatedAt = DateTime.UtcNow;
        session.UpdatedAt = DateTime.UtcNow;

        foreach (var loggedExercise in session.LoggedExercises)
        {
            loggedExercise.CreatedAt = DateTime.UtcNow;
            loggedExercise.UpdatedAt = DateTime.UtcNow;

            foreach (var set in loggedExercise.Sets)
            {
                set.CreatedAt = DateTime.UtcNow;
                set.UpdatedAt = DateTime.UtcNow;
            }
        }

        _context.WorkoutSessions.Add(session);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetWorkoutSession), new { id = session.Oid }, session);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWorkoutSession(int id, WorkoutSession session)
    {
        if (id != session.Oid)
        {
            return BadRequest();
        }

        var existingSession = await _context.WorkoutSessions
            .Include(ws => ws.LoggedExercises)
                .ThenInclude(le => le.Sets)
            .FirstOrDefaultAsync(ws => ws.Oid == id);

        if (existingSession == null || existingSession.IsDeleted)
        {
            return NotFound();
        }

        existingSession.Date = session.Date;
        existingSession.Notes = session.Notes;
        existingSession.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return StatusCode(500);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkoutSession(int id)
    {
        var session = await _context.WorkoutSessions.FindAsync(id);
        if (session == null || session.IsDeleted)
        {
            return NotFound();
        }

        session.IsDeleted = true;
        session.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
