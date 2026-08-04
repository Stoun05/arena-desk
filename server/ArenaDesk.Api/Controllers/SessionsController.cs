using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ArenaDesk.Api.Contracts;
using ArenaDesk.Api.Data;
using ArenaDesk.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArenaDesk.Api.Controllers;

[ApiController]
[Route("api/sessions")]
[Authorize(Roles = "Admin,Cashier")]
public sealed class SessionsController(AppDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Start(StartSessionRequest request)
    {
        if (request.Minutes is < 15 or > 720)
            return BadRequest(new { message = "Wagt 15–720 minut arasynda bolmaly." });
        if (string.IsNullOrWhiteSpace(request.CustomerName))
            return BadRequest(new { message = "Müşderiniň ady gerek." });

        var computer = await db.Computers.FindAsync(request.ComputerId);
        var tariff = await db.Tariffs.FindAsync(request.TariffId);
        if (computer is null || tariff is null || !tariff.IsActive) return NotFound();
        if (computer.Status != "available") return Conflict(new { message = "Kompýuter boş däl." });

        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        var startedAt = DateTimeOffset.UtcNow;
        var session = new GameSession
        {
            ComputerId = computer.Id,
            TariffId = tariff.Id,
            CreatedByUserId = userId.Value,
            CustomerName = request.CustomerName.Trim(),
            PurchasedMinutes = request.Minutes,
            StartedAt = startedAt,
            EndsAt = startedAt.AddMinutes(request.Minutes),
            TotalPrice = decimal.Round(tariff.HourlyRate * request.Minutes / 60m, 2),
            Status = "active",
        };
        computer.Status = "active";
        db.Sessions.Add(session);
        await db.SaveChangesAsync();
        return Created($"/api/sessions/{session.Id}", new { session.Id, session.EndsAt, session.TotalPrice });
    }

    [HttpPost("{id:guid}/extend")]
    public async Task<IActionResult> Extend(Guid id, ExtendSessionRequest request)
    {
        if (request.Minutes is < 15 or > 360) return BadRequest();
        var session = await db.Sessions.Include(item => item.Tariff).SingleOrDefaultAsync(item => item.Id == id && item.Status == "active");
        if (session is null) return NotFound();
        session.PurchasedMinutes += request.Minutes;
        session.EndsAt = session.EndsAt.AddMinutes(request.Minutes);
        session.TotalPrice = decimal.Round(session.Tariff.HourlyRate * session.PurchasedMinutes / 60m, 2);
        await db.SaveChangesAsync();
        return Ok(new { session.Id, session.EndsAt, session.TotalPrice });
    }

    [HttpPost("{id:guid}/finish")]
    public async Task<IActionResult> Finish(Guid id)
    {
        var session = await db.Sessions.Include(item => item.Computer).SingleOrDefaultAsync(item => item.Id == id && item.Status == "active");
        if (session is null) return NotFound();
        session.Status = "finished";
        session.FinishedAt = DateTimeOffset.UtcNow;
        session.Computer.Status = "available";
        await db.SaveChangesAsync();
        return NoContent();
    }

    private Guid? GetUserId() => Guid.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id) ? id : null;
}
