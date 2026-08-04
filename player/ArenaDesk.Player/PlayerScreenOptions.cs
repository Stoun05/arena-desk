using System.IO;
using System.Text.Json;
using ArenaDesk.Contracts;

namespace ArenaDesk.Player;

public sealed class PlayerScreenOptions
{
    public string PipeName { get; init; } = PlayerScreenProtocol.DefaultPipeName;
    public string AccessKey { get; init; } = string.Empty;

    public static PlayerScreenOptions Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("appsettings.json tapylmady.", path);
        }

        using var document = JsonDocument.Parse(File.ReadAllText(path));
        if (!document.RootElement.TryGetProperty("PlayerScreen", out var section))
        {
            throw new InvalidDataException("PlayerScreen sazlama bölümi tapylmady.");
        }

        var options = section.Deserialize<PlayerScreenOptions>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidDataException("PlayerScreen sazlamasy okalmady.");
        if (string.IsNullOrWhiteSpace(options.PipeName) ||
            string.IsNullOrWhiteSpace(options.AccessKey) ||
            options.AccessKey.Length < 32)
        {
            throw new InvalidDataException("PipeName gerek we AccessKey azyndan 32 nyşan bolmaly.");
        }

        return options;
    }
}
