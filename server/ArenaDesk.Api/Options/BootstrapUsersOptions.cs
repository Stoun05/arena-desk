using System.ComponentModel.DataAnnotations;

namespace ArenaDesk.Api.Options;

public sealed class BootstrapUsersOptions
{
    public const string SectionName = "BootstrapUsers";

    [Required, MinLength(2)]
    public string AdministratorUsername { get; init; } = "admin";

    [Required, MinLength(8)]
    public string AdministratorPassword { get; init; } = string.Empty;

    [Required, MinLength(2)]
    public string CashierUsername { get; init; } = "cashier";

    [Required, MinLength(8)]
    public string CashierPassword { get; init; } = string.Empty;
}
