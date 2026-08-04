using System.Security.Cryptography;
using System.Text;
using ArenaDesk.Api.Options;
using ArenaDesk.Contracts;
using Microsoft.Extensions.Options;

namespace ArenaDesk.Api.Authentication;

public sealed class AgentChannelAuthenticationMiddleware(
    RequestDelegate next,
    IOptions<AgentChannelOptions> options)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/hubs/agents"))
        {
            await next(context);
            return;
        }

        var accessKey = context.Request.Headers[AgentProtocol.AccessKeyHeader].ToString();
        var agentId = context.Request.Headers[AgentProtocol.AgentIdHeader].ToString();
        var computerCode = context.Request.Headers[AgentProtocol.ComputerCodeHeader].ToString();
        if (!IsValidKey(accessKey, options.Value.AccessKey) ||
            string.IsNullOrWhiteSpace(agentId) || agentId.Length > 100 ||
            string.IsNullOrWhiteSpace(computerCode) || computerCode.Length > 20)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next(context);
    }

    private static bool IsValidKey(string presentedKey, string configuredKey)
    {
        if (presentedKey.Length != configuredKey.Length)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(presentedKey),
            Encoding.UTF8.GetBytes(configuredKey));
    }
}
