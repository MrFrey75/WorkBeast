using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkBeast.Core.Models;
using WorkBeast.Data.Context;

namespace WorkBeast.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExercisesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExercisesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Exercise>>> GetExercises(
        [FromQuery] string? search = null)
    {
        var query = _context.Exercises
            .Include(e => e.TargetedBodyParts)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e => e.Name.Contains(search) || e.Description.Contains(search));
        }

        var exercises = await query
            .Where(e => !e.IsDeleted)
            .OrderBy(e => e.Name)
            .ToListAsync();

        return Ok(exercises);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Exercise>> GetExercise(int id)
    {
        var exercise = await _context.Exercises
            .Include(e => e.TargetedBodyParts)
            .FirstOrDefaultAsync(e => e.Oid == id && !e.IsDeleted);

        if (exercise == null)
        {
            return NotFound();
        }

        return Ok(exercise);
    }

    [HttpPost]
    public async Task<ActionResult<Exercise>> CreateExercise(Exercise exercise)
    {
        exercise.CreatedAt = DateTime.UtcNow;
        exercise.UpdatedAt = DateTime.UtcNow;

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExercise), new { id = exercise.Oid }, exercise);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExercise(int id, Exercise exercise)
    {
        if (id != exercise.Oid)
        {
            return BadRequest();
        }

        var existingExercise = await _context.Exercises.FindAsync(id);
        if (existingExercise == null || existingExercise.IsDeleted)
        {
            return NotFound();
        }

        existingExercise.Name = exercise.Name;
        existingExercise.Description = exercise.Description;
        existingExercise.UpdatedAt = DateTime.UtcNow;

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
    public async Task<IActionResult> DeleteExercise(int id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise == null || exercise.IsDeleted)
        {
            return NotFound();
        }

        if (exercise.IsSystem)
        {
            return BadRequest("System exercises cannot be deleted.");
        }

        exercise.IsDeleted = true;
        exercise.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
