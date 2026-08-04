using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ArenaDesk.Api.Domain;
using ArenaDesk.Api.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ArenaDesk.Api.Authentication;

public interface ITokenService
{
    (string Token, DateTimeOffset ExpiresAtUtc) CreateAccessToken(UserAccount user);
}

public sealed class TokenService(IOptions<JwtOptions> options) : ITokenService
{
    public (string Token, DateTimeOffset ExpiresAtUtc) CreateAccessToken(UserAccount user)
    {
        var jwt = options.Value;
        var expiresAtUtc = DateTimeOffset.UtcNow.AddHours(jwt.ExpirationHours);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwt.Issuer,
            audience: jwt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc.UtcDateTime,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
}
