using System.Net.NetworkInformation;

namespace ArenaDesk.Agent;

public static class MachineIdentity
{
    public static string? GetMacAddress() => NetworkInterface.GetAllNetworkInterfaces()
        .Where(item => item.OperationalStatus == OperationalStatus.Up && item.NetworkInterfaceType != NetworkInterfaceType.Loopback)
        .Select(item => item.GetPhysicalAddress().ToString())
        .FirstOrDefault(address => !string.IsNullOrWhiteSpace(address));
}
