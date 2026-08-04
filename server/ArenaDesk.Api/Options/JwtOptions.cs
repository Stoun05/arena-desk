using System.ComponentModel.DataAnnotations;

namespace ArenaDesk.Api.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required, MinLength(32)]
    public string SigningKey { get; init; } = string.Empty;

    [Required]
    public string Issuer { get; init; } = "ArenaDesk.Api";

    [Required]
    public string Audience { get; init; } = "ArenaDesk.Web";

    [Range(1, 24)]
    public int ExpirationHours { get; init; } = 8;
}
