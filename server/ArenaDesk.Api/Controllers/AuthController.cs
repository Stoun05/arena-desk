using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ArenaDesk.Api.Contracts;
using ArenaDesk.Api.Data;
using ArenaDesk.Api.Models;
using ArenaDesk.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArenaDesk.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AppDbContext db, JwtTokenService tokens, IHostEnvironment environment) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<UserResponse>> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.SingleOrDefaultAsync(item => item.Email.ToLower() == email);
        if (user is null) return Unauthorized(new { message = "Email ýa-da parol nädogry." });

        var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Email ýa-da parol nädogry." });

        Response.Cookies.Append(JwtTokenService.CookieName, tokens.Create(user), new CookieOptions
        {
            HttpOnly = true,
            Secure = environment.IsProduction(),
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(8),
            Path = "/",
        });
        return ToResponse(user);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> Me()
    {
        var subject = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!Guid.TryParse(subject, out var userId)) return Unauthorized();
        var user = await db.Users.FindAsync(userId);
        return user is null ? Unauthorized() : ToResponse(user);
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(JwtTokenService.CookieName, new CookieOptions { Path = "/" });
        return NoContent();
    }

    private static UserResponse ToResponse(User user) =>
        new(user.Id, user.Name, user.Email, user.Role.ToString().ToLowerInvariant());
}
