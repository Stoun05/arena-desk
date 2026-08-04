using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ArenaDesk.Contracts;

namespace ArenaDesk.Agent;

public sealed partial class WindowsCommandExecutor(ILogger<WindowsCommandExecutor> logger)
{
    public bool CanExecute(string commandType) => commandType is
        AgentCommandTypes.Logout or AgentCommandTypes.Sleep or AgentCommandTypes.Shutdown;

    public Task ExecuteAsync(AgentCommand command, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("ArenaDesk system commands require Windows.");
        }

        switch (command.Type)
        {
            case AgentCommandTypes.Logout:
                var activeSessionId = WTSGetActiveConsoleSessionId();
                if (activeSessionId == uint.MaxValue || !WTSLogoffSession(IntPtr.Zero, activeSessionId, wait: false))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                break;
            case AgentCommandTypes.Shutdown:
                StartShutdown("/s /t 5 /c \"ArenaDesk session completed\"");
                break;
            case AgentCommandTypes.Sleep:
                if (!SetSuspendState(hibernate: false, forceCritical: false, disableWakeEvent: false))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                break;
            default:
                logger.LogInformation(
                    "Command {CommandType} updates ArenaDesk state and has no Windows system action.",
                    command.Type);
                break;
        }

        return Task.CompletedTask;
    }

    private static void StartShutdown(string arguments)
    {
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "shutdown.exe",
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true
        });
        if (process is null)
        {
            throw new InvalidOperationException("Windows shutdown command could not be started.");
        }
    }

    [LibraryImport("powrprof.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetSuspendState(
        [MarshalAs(UnmanagedType.Bool)] bool hibernate,
        [MarshalAs(UnmanagedType.Bool)] bool forceCritical,
        [MarshalAs(UnmanagedType.Bool)] bool disableWakeEvent);

    [LibraryImport("kernel32.dll")]
    private static partial uint WTSGetActiveConsoleSessionId();

    [LibraryImport("wtsapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool WTSLogoffSession(
        IntPtr serverHandle,
        uint sessionId,
        [MarshalAs(UnmanagedType.Bool)] bool wait);
}
