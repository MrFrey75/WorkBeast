using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBeast.Core.DTOs;
using WorkBeast.Core.Services;

namespace WorkBeast.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IAuthService authService, ILogger<UsersController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetAllUsers()
    {
        var users = await _authService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Get user by ID (Admin only)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUser(int id)
    {
        var user = await _authService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Assign role to user (Admin only)
    /// </summary>
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {
        var result = await _authService.AssignRoleAsync(request.UserId, request.RoleName);
        if (!result)
        {
            return BadRequest(new { message = "Failed to assign role" });
        }

        _logger.LogInformation("Assigned role {Role} to user {UserId}", request.RoleName, request.UserId);
        return Ok(new { message = "Role assigned successfully" });
    }

    /// <summary>
    /// Remove role from user (Admin only)
    /// </summary>
    [HttpPost("remove-role")]
    public async Task<IActionResult> RemoveRole([FromBody] AssignRoleRequest request)
    {
        var result = await _authService.RemoveRoleAsync(request.UserId, request.RoleName);
        if (!result)
        {
            return BadRequest(new { message = "Failed to remove role" });
        }

        _logger.LogInformation("Removed role {Role} from user {UserId}", request.RoleName, request.UserId);
        return Ok(new { message = "Role removed successfully" });
    }

    /// <summary>
    /// Deactivate user (Admin only)
    /// </summary>
    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        var result = await _authService.DeactivateUserAsync(id);
        if (!result)
        {
            return BadRequest(new { message = "Failed to deactivate user" });
        }

        _logger.LogInformation("Deactivated user {UserId}", id);
        return Ok(new { message = "User deactivated successfully" });
    }

    /// <summary>
    /// Activate user (Admin only)
    /// </summary>
    [HttpPost("{id}/activate")]
    public async Task<IActionResult> ActivateUser(int id)
    {
        var result = await _authService.ActivateUserAsync(id);
        if (!result)
        {
            return BadRequest(new { message = "Failed to activate user" });
        }

        _logger.LogInformation("Activated user {UserId}", id);
        return Ok(new { message = "User activated successfully" });
    }
}
