using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [Authorize]
    [HttpGet("profile")]
    public IActionResult Profile()
    {
        return Ok(new
        {
            UserId = User.FindFirst(
        ClaimTypes.NameIdentifier)?.Value,
            Email = User.FindFirst(
        ClaimTypes.Email)?.Value,
            Role = User.FindFirst(
        ClaimTypes.Role)?.Value
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult Admin()
    {
        return Ok("Admin Access Granted");
    }
    [Authorize(Roles = "Researcher")]
    [HttpGet("research")]
    public IActionResult Research()
    {
        return Ok("Researcher Access Granted");
    }

    [Authorize]
    [HttpGet("claims")]
    public IActionResult Claims()
    {
        return Ok(new
        {
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Email = User.FindFirst(ClaimTypes.Email)?.Value,
            Role = User.FindFirst(ClaimTypes.Role)?.Value
        });
    }
}