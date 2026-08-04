using ArenaDesk.Api.Contracts;
using ArenaDesk.Api.Data;
using ArenaDesk.Api.Models;
using ArenaDesk.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArenaDesk.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/agent")]
public sealed class AgentController(AppDbContext db, AgentAuthenticationService authentication) : ControllerBase
{
    private const int HeartbeatIntervalSeconds = 10;

    [HttpPost("register")]
    public async Task<ActionResult<AgentRegistrationResponse>> Register(AgentRegistrationRequest request)
    {
        if (!authentication.IsBootstrapKeyValid(Request.Headers["X-Arena-Bootstrap-Key"].FirstOrDefault()))
            return Unauthorized();
        if (string.IsNullOrWhiteSpace(request.ComputerName) || string.IsNullOrWhiteSpace(request.MachineName) || string.IsNullOrWhiteSpace(request.AgentVersion))
            return BadRequest(new { message = "ComputerName, MachineName we AgentVersion hökmany." });

        var normalizedName = request.ComputerName.Trim();
        var computer = await db.Computers.SingleOrDefaultAsync(item => item.Name == normalizedName);
        if (computer is null)
        {
            computer = new Computer
            {
                Name = normalizedName,
                Zone = "Standard",
                Status = "available",
            };
            db.Computers.Add(computer);
        }

        var token = AgentAuthenticationService.CreateToken();
        computer.MachineName = request.MachineName.Trim();
        computer.MacAddress = string.IsNullOrWhiteSpace(request.MacAddress) ? computer.MacAddress : request.MacAddress.Trim();
        computer.AgentVersion = request.AgentVersion.Trim();
        computer.AgentTokenHash = AgentAuthenticationService.HashToken(token);
        computer.LastSeenAt = DateTimeOffset.UtcNow;
        if (computer.Status == "offline") computer.Status = "available";
        await db.SaveChangesAsync();

        return Ok(new AgentRegistrationResponse(computer.Id, token, HeartbeatIntervalSeconds));
    }

    [HttpPost("heartbeat")]
    public async Task<ActionResult<AgentHeartbeatResponse>> Heartbeat(AgentHeartbeatRequest request)
    {
        var computer = await AuthenticateAgentAsync();
        if (computer is null) return Unauthorized();
        if (string.IsNullOrWhiteSpace(request.AgentVersion)) return BadRequest(new { message = "AgentVersion hökmany." });

        var now = DateTimeOffset.UtcNow;
        computer.LastSeenAt = now;
        computer.AgentVersion = request.AgentVersion.Trim();

        var retryBefore = now.AddSeconds(-30);
        var command = await db.DeviceCommands
            .Where(item => item.ComputerId == computer.Id && item.DeliveryAttempts < 5 &&
                (item.Status == "pending" || (item.Status == "delivered" && item.DeliveredAt < retryBefore)))
            .OrderBy(item => item.CreatedAt)
            .FirstOrDefaultAsync();

        AgentCommandResponse? response = null;
        if (command is not null)
        {
            command.Status = "delivered";
            command.DeliveredAt = now;
            command.DeliveryAttempts += 1;
            response = new AgentCommandResponse(command.Id, command.Type, command.Payload);
        }

        await db.SaveChangesAsync();
        return Ok(new AgentHeartbeatResponse(now, response));
    }

    [HttpPost("commands/{id:guid}/result")]
    public async Task<IActionResult> CompleteCommand(Guid id, AgentCommandResultRequest request)
    {
        var computer = await AuthenticateAgentAsync();
        if (computer is null) return Unauthorized();

        var allowedOutcomes = new[] { "succeeded", "simulated", "failed" };
        if (string.IsNullOrWhiteSpace(request.Outcome)) return BadRequest(new { message = "Outcome hökmany." });
        var outcome = request.Outcome.Trim().ToLowerInvariant();
        if (!allowedOutcomes.Contains(outcome)) return BadRequest(new { message = "Outcome nädogry." });

        var command = await db.DeviceCommands.SingleOrDefaultAsync(item => item.Id == id && item.ComputerId == computer.Id);
        if (command is null) return NotFound();
        if (command.Status is "succeeded" or "simulated" or "failed") return NoContent();

        command.Status = outcome;
        var resultMessage = request.Message?.Trim();
        command.ResultMessage = string.IsNullOrWhiteSpace(resultMessage) ? null : resultMessage[..Math.Min(resultMessage.Length, 500)];
        command.CompletedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }

    private Task<Computer?> AuthenticateAgentAsync() => authentication.AuthenticateAsync(Request.Headers.Authorization.FirstOrDefault());
}
