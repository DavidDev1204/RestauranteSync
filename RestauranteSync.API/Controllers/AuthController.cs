using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestauranteSync.Application.Features.Auth;
using RestauranteSync.Domain.Entities;

namespace RestauranteSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    //private readonly IPasswordHasher _hasher = hasher;

    //[HttpGet("hash_password")]
    //public async Task<IActionResult> HashPassword([FromQuery] string password) 
    //{
    //    var val = _hasher.HashPassword(password);
    //    return Ok(val);
    //}

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "An error occurred during login" });
        }
    }

    [HttpGet("validate")]
    [Authorize]
    public ActionResult ValidateToken()
    {
        return Ok(new { message = "Token is valid", user = User.Identity?.Name });
    }
}
