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

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

        if (user == null)
        {
            return Unauthorized(new LoginResponse 
            { 
                Success = false, 
                Message = "Invalid username or password." 
            });
        }

        // Simple password check (in production, use proper hashing)
        // For demo purposes, accept "admin123" for admin user
        bool passwordValid = (request.Username == "admin" && request.Password == "admin123") ||
                            VerifyPassword(request.Password, user.PasswordHash);

        if (!passwordValid)
        {
            return Unauthorized(new LoginResponse 
            { 
                Success = false, 
                Message = "Invalid username or password." 
            });
        }

        // Update last login
        user.LastLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Generate a simple token (in production, use JWT)
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return Ok(new LoginResponse
        {
            Success = true,
            Token = token,
            Message = "Login successful.",
            UserId = user.Id,
            MandantId = user.MandantId,
            Username = user.Username,
            Role = user.Role
        });
    }

    private bool VerifyPassword(string password, string? passwordHash)
    {
        // Simple comparison for demo - in production use BCrypt or similar
        return !string.IsNullOrEmpty(passwordHash) && 
               passwordHash == Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }
}
