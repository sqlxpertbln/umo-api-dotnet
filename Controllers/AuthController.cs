// =================================================================================================
// APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Presentation Layer)
// This controller is part of the Presentation Layer in Clean Architecture. It handles HTTP requests
// related to Authentication and translates them into application-level commands or queries.
//
// META-DATA:
//   - Layer: Presentation (API Controller)
//   - Responsibility: Expose Authentication-related functionality via a RESTful API.
// =================================================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMOApi.Data;
using UMOApi.Models;

namespace UMOApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UMOApiDbContext _context;

    public AuthController(UMOApiDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Authenticates a user and returns a login response.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new LoginResponse 
            { 
                Success = false, 
                Message = "Username and password are required." 
            });
        }

        // In a real application, you would validate against a user table.
        // Here, we mock validation against the Dispatcher table for demo purposes.
        var user = await _context.Dispatchers.FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
        {
            return Unauthorized(new LoginResponse 
            { 
                Success = false, 
                Message = "Invalid username or password." 
            });
        }

        // Simple password check (in production, use proper hashing like BCrypt)
        // For demo purposes, we accept any password for a valid user.
        bool passwordValid = true; 

        if (!passwordValid)
        {
            return Unauthorized(new LoginResponse 
            { 
                Success = false, 
                Message = "Invalid username or password." 
            });
        }

        // Generate a simple token (in production, use JWT)
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return Ok(new LoginResponse
        {
            Success = true,
            Token = token,
            Message = "Login successful.",
            UserId = user.Id,
            MandantId = 1, // Mock MandantId
            Username = user.Username,
            Role = user.Role
        });
    }
}
