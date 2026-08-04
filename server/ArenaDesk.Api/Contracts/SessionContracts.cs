namespace ArenaDesk.Api.Contracts;

public sealed record StartSessionRequest(Guid ComputerId, Guid TariffId, string CustomerName, int Minutes);
public sealed record ExtendSessionRequest(int Minutes);
