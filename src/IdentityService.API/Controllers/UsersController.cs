using System.Security.Claims;
//using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    //private readonly IUserRepository _userRepository;
    //private readonly ICacheService _cacheService;

    // public UsersController(
    //     IUserRepository userRepository,
    //     ICacheService cacheService)
    // {
    //     _userRepository = userRepository;
    //     _cacheService = cacheService;
    // }
 
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
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
    // public async Task<IActionResult> Profile()
    // {
    //     var userId = User.FindFirst(
    //         ClaimTypes.NameIdentifier)?.Value;

    //     if (string.IsNullOrEmpty(userId))
    //     {
    //         return Unauthorized();
    //     }

    //     var cacheKey = $"user:{userId}";

    //     var cachedUser =
    //         await _cacheService.GetAsync<User>(
    //             cacheKey);

    //     if (cachedUser != null)
    //     {
    //         Console.WriteLine(
    //             "Cache Hit");

    //         return Ok(new
    //         {
    //             Source = "Redis Cache",
    //             User = cachedUser
    //         });
    //     }

    //     Console.WriteLine(
    //         "Cache Miss");

    //     var user =
    //         await _userRepository.GetByIdAsync(
    //             Guid.Parse(userId));

    //     if (user == null)
    //     {
    //         return NotFound();
    //     }

    //     await _cacheService.SetAsync(
    //         cacheKey,
    //         user,
    //         TimeSpan.FromMinutes(30));

    //     return Ok(new
    //     {
    //         Source = "Database",
    //         User = user,
    //         UserId = User.FindFirst(
    //     ClaimTypes.NameIdentifier)?.Value,
    //         Email = User.FindFirst(
    //     ClaimTypes.Email)?.Value,
    //         Role = User.FindFirst(
    //     ClaimTypes.Role)?.Value
    //     });
    // }
    
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
            Role = User.FindFirst(ClaimTypes.Role)?.Value,
            Permissions = User.FindAll("permission")
                .Select(c => c.Value)
                .ToList()
        });
    }

    [Authorize(Policy = "CreateUser")]
    [HttpGet("create-user")]
    public IActionResult CreateUser()
    {
        return Ok(
            "CreateUser Permission Granted");
    }
    [Authorize(Policy = "DeleteUser")]
    [HttpGet("delete-user")]
    public IActionResult DeleteUser()
    {
        return Ok(
            "DeleteUser Permission Granted");
    }

    [Authorize(Policy = "ManageRoles")]
    [HttpGet("manage-roles")]
    public IActionResult ManageRoles()
    {
        return Ok(
            "ManageRoles Permission Granted");
    }
}