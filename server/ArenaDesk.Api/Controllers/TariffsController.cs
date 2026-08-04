using ArenaDesk.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArenaDesk.Api.Controllers;

[ApiController]
[Route("api/tariffs")]
[Authorize]
public sealed class TariffsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetActive() => Ok(await db.Tariffs
        .AsNoTracking()
        .Where(item => item.IsActive)
        .OrderBy(item => item.HourlyRate)
        .Select(item => new { item.Id, item.Name, item.HourlyRate })
        .ToListAsync());
}
