using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using ArenaDesk.Contracts;
using Microsoft.Extensions.Options;

namespace ArenaDesk.Agent;

public sealed class PlayerScreenChannel(
    IOptions<AgentOptions> options,
    ILogger<PlayerScreenChannel> logger) : BackgroundService
{
    private readonly object _stateGate = new();
    private PlayerScreenState _latestState = LockedState("Sessiya açylmagyna garaşylýar");

    public Task PublishRegistrationAsync(AgentRegistration registration)
    {
        var state = registration.ActiveSessionId is Guid sessionId && registration.EndsAtUtc > DateTimeOffset.UtcNow
            ? ActiveState(registration.ComputerCode, sessionId, registration.EndsAtUtc.Value)
            : LockedState("Sessiya açylmagyna garaşylýar", registration.ComputerCode);
        SetLatestState(state);
        return Task.CompletedTask;
    }

    public Task PublishCommandAsync(AgentCommand command)
    {
        if ((command.Type is AgentCommandTypes.Unlock or AgentCommandTypes.SyncSession) &&
            command.SessionId is Guid sessionId &&
            command.EndsAtUtc is DateTimeOffset endsAtUtc)
        {
            SetLatestState(ActiveState(options.Value.ComputerCode, sessionId, endsAtUtc));
        }
        else if (command.Type is AgentCommandTypes.Logout or AgentCommandTypes.Sleep or AgentCommandTypes.Shutdown)
        {
            SetLatestState(LockedState("Sessiya tamamlandy", options.Value.ComputerCode));
        }

        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await AcceptPlayerAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Player Screen channel stopped; accepting a new connection.");
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }
    }

    private async Task AcceptPlayerAsync(CancellationToken cancellationToken)
    {
        await using var pipe = CreatePipe();
        logger.LogInformation("Waiting for Player Screen on local pipe {PipeName}.", options.Value.PlayerPipeName);
        await pipe.WaitForConnectionAsync(cancellationToken);

        using var reader = new StreamReader(pipe, Encoding.UTF8, leaveOpen: true);
        await using var writer = new StreamWriter(pipe, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), leaveOpen: true)
        {
            AutoFlush = true,
        };
        var handshakeLine = await reader.ReadLineAsync(cancellationToken);
        var handshake = handshakeLine is null
            ? null
            : JsonSerializer.Deserialize<PlayerScreenHandshake>(handshakeLine);
        if (handshake is null ||
            string.IsNullOrEmpty(handshake.AccessKey) ||
            !IsValidAccessKey(handshake.AccessKey))
        {
            logger.LogWarning("Rejected an unauthenticated local Player Screen connection.");
            return;
        }

        logger.LogInformation("Player Screen connected for {ComputerCode}.", options.Value.ComputerCode);
        while (pipe.IsConnected && !cancellationToken.IsCancellationRequested)
        {
            PlayerScreenState state;
            lock (_stateGate)
            {
                state = _latestState;
            }

            await writer.WriteLineAsync(JsonSerializer.Serialize(state).AsMemory(), cancellationToken);
            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
        }
    }

    private NamedPipeServerStream CreatePipe()
    {
        if (!OperatingSystem.IsWindows())
        {
            return new NamedPipeServerStream(
                options.Value.PlayerPipeName,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous);
        }

        var pipeSecurity = new PipeSecurity();
        pipeSecurity.AddAccessRule(new PipeAccessRule(
            new SecurityIdentifier(WellKnownSidType.LocalSystemSid, domainSid: null),
            PipeAccessRights.FullControl,
            AccessControlType.Allow));
        pipeSecurity.AddAccessRule(new PipeAccessRule(
            new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, domainSid: null),
            PipeAccessRights.FullControl,
            AccessControlType.Allow));
        pipeSecurity.AddAccessRule(new PipeAccessRule(
            new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, domainSid: null),
            PipeAccessRights.ReadWrite,
            AccessControlType.Allow));

        return NamedPipeServerStreamAcl.Create(
            options.Value.PlayerPipeName,
            PipeDirection.InOut,
            1,
            PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous,
            inBufferSize: 0,
            outBufferSize: 0,
            pipeSecurity,
            HandleInheritability.None,
            (PipeAccessRights)0);
    }

    private bool IsValidAccessKey(string presentedKey)
    {
        var configuredKey = options.Value.PlayerAccessKey;
        if (presentedKey.Length != configuredKey.Length)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(presentedKey),
            Encoding.UTF8.GetBytes(configuredKey));
    }

    private void SetLatestState(PlayerScreenState state)
    {
        lock (_stateGate)
        {
            _latestState = state;
        }
    }

    private static PlayerScreenState ActiveState(string computerCode, Guid sessionId, DateTimeOffset endsAtUtc) => new(
        PlayerScreenProtocol.ActiveMode,
        computerCode,
        sessionId,
        endsAtUtc,
        "Sessiya dowam edýär",
        DateTimeOffset.UtcNow);

    private static PlayerScreenState LockedState(string message, string computerCode = "") => new(
        PlayerScreenProtocol.LockedMode,
        computerCode,
        null,
        null,
        message,
        DateTimeOffset.UtcNow);
}
