using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using ArenaDesk.Contracts;

namespace ArenaDesk.Player;

public sealed class PlayerChannelClient(PlayerScreenOptions options)
{
    public event Action<PlayerScreenState>? StateReceived;
    public event Action<bool>? ConnectionChanged;

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ConnectAndReadAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch
            {
            }

            ConnectionChanged?.Invoke(false);
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }

    private async Task ConnectAndReadAsync(CancellationToken cancellationToken)
    {
        await using var pipe = new NamedPipeClientStream(
            ".",
            options.PipeName,
            PipeDirection.InOut,
            PipeOptions.Asynchronous);
        await pipe.ConnectAsync(5_000, cancellationToken);

        await using var writer = new StreamWriter(pipe, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), leaveOpen: true)
        {
            AutoFlush = true,
        };
        using var reader = new StreamReader(pipe, Encoding.UTF8, leaveOpen: true);
        await writer.WriteLineAsync(
            JsonSerializer.Serialize(new PlayerScreenHandshake(options.AccessKey)).AsMemory(),
            cancellationToken);
        ConnectionChanged?.Invoke(true);

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line is null)
            {
                break;
            }

            var state = JsonSerializer.Deserialize<PlayerScreenState>(line);
            if (state is not null)
            {
                StateReceived?.Invoke(state);
            }
        }

    }
}
