using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkBeast.Core.Models;
using WorkBeast.Data.Context;

namespace WorkBeast.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BodyPartsController : ControllerBase
{
    private readonly AppDbContext _context;

    public BodyPartsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all body parts
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BodyPart>>> GetBodyParts()
    {
        var bodyParts = await _context.BodyParts
            .Where(bp => !bp.IsDeleted)
            .OrderBy(bp => bp.Name)
            .ToListAsync();

        return Ok(bodyParts);
    }

    /// <summary>
    /// Get body part by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BodyPart>> GetBodyPart(int id)
    {
        var bodyPart = await _context.BodyParts
            .FirstOrDefaultAsync(bp => bp.Oid == id && !bp.IsDeleted);

        if (bodyPart == null)
        {
            return NotFound();
        }

        return Ok(bodyPart);
    }

    /// <summary>
    /// Create new body part (Admin or Trainer only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Trainer")]
    public async Task<ActionResult<BodyPart>> CreateBodyPart(BodyPart bodyPart)
    {
        bodyPart.CreatedAt = DateTime.UtcNow;
        bodyPart.UpdatedAt = DateTime.UtcNow;

        _context.BodyParts.Add(bodyPart);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBodyPart), new { id = bodyPart.Oid }, bodyPart);
    }

    /// <summary>
    /// Update body part (Admin or Trainer only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Trainer")]
    public async Task<IActionResult> UpdateBodyPart(int id, BodyPart bodyPart)
    {
        if (id != bodyPart.Oid)
        {
            return BadRequest();
        }

        var existingBodyPart = await _context.BodyParts.FindAsync(id);
        if (existingBodyPart == null || existingBodyPart.IsDeleted)
        {
            return NotFound();
        }

        if (existingBodyPart.IsSystem)
        {
            return BadRequest(new { message = "System body parts cannot be modified." });
        }

        existingBodyPart.Name = bodyPart.Name;
        existingBodyPart.Description = bodyPart.Description;
        existingBodyPart.UpdatedAt = DateTime.UtcNow;

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

    /// <summary>
    /// Delete body part (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBodyPart(int id)
    {
        var bodyPart = await _context.BodyParts.FindAsync(id);
        if (bodyPart == null || bodyPart.IsDeleted)
        {
            return NotFound();
        }

        if (bodyPart.IsSystem)
        {
            return BadRequest(new { message = "System body parts cannot be deleted." });
        }

        bodyPart.IsDeleted = true;
        bodyPart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
