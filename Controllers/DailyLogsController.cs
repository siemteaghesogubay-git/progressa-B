using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using progressa.Dtos;
using progressa.Services.IServices;

namespace progressa.Controllers;

[ApiController]
[Authorize]
[Route("api/daily-logs")]
public class DailyLogsController(IDailyLogService service) : ControllerBase
{
    private string? CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

    [HttpGet("{date}")]
    public async Task<IActionResult> Get(string date, CancellationToken ct)
    {
        var userId = CurrentUserId;
        if (userId is null) return Unauthorized();
        if (!TryParseDate(date, out var day)) return ValidationProblem(ModelState);

        return Ok(await service.GetAsync(userId, day, ct));
    }

    [HttpPut("{date}")]
    public async Task<IActionResult> Save(string date, DailyLogDto dto, CancellationToken ct)
    {
        var userId = CurrentUserId;
        if (userId is null) return Unauthorized();
        if (!TryParseDate(date, out var day)) return ValidationProblem(ModelState);

        var saved = await service.SaveAsync(userId, day, dto, ct);
        if (saved is null)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                detail: "Dagsloggen har ändrats. Hämta den igen innan du sparar.");
        }

        return Ok(saved);
    }

    private bool TryParseDate(string date, out DateOnly day)
    {
        if (DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out day) && day.Year is >= 1900 and <= 9998)
        {
            return true;
        }

        ModelState.AddModelError(nameof(date), "Ange ett giltigt datum i formatet yyyy-MM-dd.");
        return false;
    }
}
