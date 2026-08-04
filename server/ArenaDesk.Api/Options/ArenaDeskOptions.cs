using System.ComponentModel.DataAnnotations;

namespace ArenaDesk.Api.Options;

public sealed class ArenaDeskOptions
{
    public const string SectionName = "ArenaDesk";

    [Required]
    public string ClubName { get; init; } = "ArenaDesk Demo Club";

    [Range(1, 100)]
    public int ComputerLimit { get; init; } = 10;
}
