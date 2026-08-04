using System.Text.Json;
using Microsoft.Extensions.Options;

namespace ArenaDesk.Agent;

public sealed class ProcessedCommandStore(IOptions<AgentOptions> options)
{
    private const int MaximumEntries = 1_000;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private HashSet<Guid>? _processed;
    private Queue<Guid>? _history;

    public async Task<bool> ContainsAsync(Guid commandId, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await EnsureLoadedAsync(cancellationToken);
            return _processed!.Contains(commandId);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task MarkProcessedAsync(Guid commandId, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await EnsureLoadedAsync(cancellationToken);
            if (!_processed!.Add(commandId))
            {
                return;
            }

            _history!.Enqueue(commandId);
            while (_history.Count > MaximumEntries)
            {
                _processed.Remove(_history.Dequeue());
            }

            var path = GetStatePath();
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var temporaryPath = $"{path}.tmp";
            await File.WriteAllTextAsync(
                temporaryPath,
                JsonSerializer.Serialize(_history),
                cancellationToken);
            File.Move(temporaryPath, path, overwrite: true);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task EnsureLoadedAsync(CancellationToken cancellationToken)
    {
        if (_processed is not null)
        {
            return;
        }

        var path = GetStatePath();
        if (!File.Exists(path))
        {
            _processed = [];
            _history = [];
            return;
        }

        var json = await File.ReadAllTextAsync(path, cancellationToken);
        var history = JsonSerializer.Deserialize<List<Guid>>(json)
            ?? throw new InvalidDataException("The processed-command state file is invalid.");
        _history = new Queue<Guid>(history.TakeLast(MaximumEntries));
        _processed = _history.ToHashSet();
    }

    private string GetStatePath()
    {
        var configuredDirectory = options.Value.StateDirectory.Trim();
        var directory = configuredDirectory.Length > 0
            ? configuredDirectory
            : Path.Combine(
                Environment.GetFolderPath(OperatingSystem.IsWindows()
                    ? Environment.SpecialFolder.CommonApplicationData
                    : Environment.SpecialFolder.LocalApplicationData),
                "ArenaDesk");
        return Path.Combine(directory, "processed-commands.json");
    }
}
