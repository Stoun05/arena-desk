using ArenaDesk.Api.Data;
using ArenaDesk.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArenaDesk.Api.Controllers;

[ApiController]
[Route("api/computers")]
[Authorize]
public sealed class ComputersController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ComputerResponse>>> GetAll()
    {
        var now = DateTimeOffset.UtcNow;
        var computers = await db.Computers
            .AsNoTracking()
            .Include(item => item.Sessions.Where(session => session.Status == "active"))
            .OrderBy(item => item.Name)
            .ToListAsync();
        var stations = computers.Select(item =>
        {
            var session = item.Sessions.FirstOrDefault();
            return new ComputerResponse(
                item.Id,
                item.Name,
                item.Zone,
                item.Status,
                session?.Id,
                session?.CustomerName,
                session is null ? null : Math.Max(0, (int)(session.EndsAt - now).TotalSeconds),
                session?.TotalPrice);
        }).ToList();
        return Ok(stations);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ComputerResponse>> Create(CreateComputerRequest request)
    {
        if (await db.Computers.AnyAsync(item => item.Name == request.Name))
            return Conflict(new { message = "Bu at bilen kompýuter eýýäm bar." });
        var computer = new Computer
        {
            Name = request.Name.Trim(),
            Zone = request.Zone.Trim(),
            Status = "available",
            MacAddress = request.MacAddress?.Trim(),
        };
        db.Computers.Add(computer);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new ComputerResponse(computer.Id, computer.Name, computer.Zone, computer.Status, null, null, null, null));
    }
}

public sealed record CreateComputerRequest(string Name, string Zone, string? MacAddress);
public sealed record ComputerResponse(Guid Id, string Name, string Zone, string Status, Guid? SessionId, string? Customer, int? RemainingSeconds, decimal? SessionPrice);
