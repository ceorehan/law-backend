using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZALaw.Api.Data;
using ZALaw.Api.DTOs;
using ZALaw.Api.Models;
using ZALaw.Api.Services;

namespace ZALaw.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (await _db.Users.AnyAsync(u => u.Email == normalizedEmail))
            return Conflict(new { message = "An account with this email already exists." });

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            Cnic = request.Cnic.Trim(),
            PhoneNumber = request.Phone.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = UserRole.Client,
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _tokenService.CreateToken(user);
        return Ok(new AuthResponse(token, new UserSummary(user.Id, user.FullName, user.Email, user.Role.ToString())));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail && u.Role == UserRole.Client);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        var token = _tokenService.CreateToken(user);
        return Ok(new AuthResponse(token, new UserSummary(user.Id, user.FullName, user.Email, user.Role.ToString())));
    }

    [HttpPost("consultant-login")]
    public async Task<ActionResult<AuthResponse>> ConsultantLogin(LoginRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.Email == normalizedEmail && (u.Role == UserRole.Consultant || u.Role == UserRole.Admin));

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        var token = _tokenService.CreateToken(user);
        return Ok(new AuthResponse(token, new UserSummary(user.Id, user.FullName, user.Email, user.Role.ToString())));
    }
}
