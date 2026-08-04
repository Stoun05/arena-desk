using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace ArenaDesk.Agent;

public sealed class ArenaApiClient(HttpClient httpClient, IOptions<AgentOptions> options, ILogger<ArenaApiClient> logger)
{
    private readonly AgentOptions _options = options.Value;
    private string? _agentToken = string.IsNullOrWhiteSpace(options.Value.AgentToken) ? null : options.Value.AgentToken;

    public async Task EnsureRegisteredAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_agentToken)) return;

        var computerName = string.IsNullOrWhiteSpace(_options.ComputerName) ? Environment.MachineName : _options.ComputerName.Trim();
        var request = new RegistrationRequest(computerName, Environment.MachineName, MachineIdentity.GetMacAddress(), AgentVersion);
        using var message = new HttpRequestMessage(HttpMethod.Post, "agent/register")
        {
            Content = JsonContent.Create(request),
        };
        message.Headers.Add("X-Arena-Bootstrap-Key", _options.BootstrapKey);

        using var response = await httpClient.SendAsync(message, cancellationToken);
        response.EnsureSuccessStatusCode();
        var registration = await response.Content.ReadFromJsonAsync<RegistrationResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Agent registration returned an empty response.");
        _agentToken = registration.AgentToken;
        logger.LogInformation("Registered ArenaDesk computer {ComputerId}. The token is kept in memory for this service run.", registration.ComputerId);
    }

    public async Task<HeartbeatResponse> SendHeartbeatAsync(CancellationToken cancellationToken)
    {
        using var request = CreateAuthenticatedRequest(HttpMethod.Post, "agent/heartbeat", JsonContent.Create(new HeartbeatRequest(AgentVersion)));
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<HeartbeatResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Heartbeat returned an empty response.");
    }

    public async Task ReportCommandResultAsync(Guid commandId, CommandExecutionResult result, CancellationToken cancellationToken)
    {
        using var request = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"agent/commands/{commandId}/result",
            JsonContent.Create(new CommandResultRequest(result.Outcome, result.Message)));
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private HttpRequestMessage CreateAuthenticatedRequest(HttpMethod method, string path, HttpContent content)
    {
        if (string.IsNullOrWhiteSpace(_agentToken)) throw new InvalidOperationException("Agent is not registered.");
        var request = new HttpRequestMessage(method, path) { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _agentToken);
        return request;
    }

    private static string AgentVersion => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
}
