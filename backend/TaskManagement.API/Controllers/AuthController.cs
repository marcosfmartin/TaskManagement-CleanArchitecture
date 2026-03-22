using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Services;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthDto dto)
    {
        try
        {
            var user = await _authService.RegisterAsync(dto);
            return Ok(new { message = "User registered successfully", username = user.Username });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred during registration.");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthDto dto)
    {
        var token = await _authService.LoginAsync(dto);

        if (token == null)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        return Ok(new { Token = token });
    }
}