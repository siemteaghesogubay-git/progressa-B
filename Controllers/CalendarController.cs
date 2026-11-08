using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using progressa.Services.IServices;

namespace progressa.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;

    public CalendarController(ICalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    private string? CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub");

    // Hämta aktiviteter för en svensk kalenderdag.
    [HttpGet("day")]
    public async Task<IActionResult> GetByDayAsync(
        [FromQuery] string? date)
    {
        var userId = CurrentUserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (!TryParseCalendarDate(date, out var calendarDate))
        {
            return ValidationProblem(ModelState);
        }

        var sessions = await _calendarService.GetByDayAsync(
            calendarDate,
            userId);

        return Ok(sessions);
    }

    // Hämta aktiviteter för veckan som innehåller datumet.
    // Veckan börjar på måndag.
    [HttpGet("week")]
    public async Task<IActionResult> GetByWeekAsync(
        [FromQuery] string? date)
    {
        var userId = CurrentUserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (!TryParseCalendarDate(date, out var calendarDate))
        {
            return ValidationProblem(ModelState);
        }

        var sessions = await _calendarService.GetByWeekAsync(
            calendarDate,
            userId);

        return Ok(sessions);
    }

    // Hämta aktiviteter för månaden som innehåller datumet.
    [HttpGet("month")]
    public async Task<IActionResult> GetByMonthAsync(
        [FromQuery] string? date)
    {
        var userId = CurrentUserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (!TryParseCalendarDate(date, out var calendarDate))
        {
            return ValidationProblem(ModelState);
        }

        var sessions = await _calendarService.GetByMonthAsync(
            calendarDate,
            userId);

        return Ok(sessions);
    }

    private bool TryParseCalendarDate(
        string? value,
        out DateTime calendarDate)
    {
        calendarDate = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            ModelState.AddModelError(
                "date",
                "Ange ett datum i formatet yyyy-MM-dd.");

            return false;
        }

        if (!DateOnly.TryParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDate))
        {
            ModelState.AddModelError(
                "date",
                "Ogiltigt datum. Använd formatet yyyy-MM-dd, " +
                "exempelvis 2026-11-09.");

            return false;
        }

        if (parsedDate.Year < 1900 || parsedDate.Year > 9998)
        {
            ModelState.AddModelError(
                "date",
                "Kalenderdatum måste ligga mellan år 1900 och 9998.");

            return false;
        }

        // Ett lokalt kalenderdatum, inte en UTC-tidpunkt.
        // CalendarService omvandlar periodens gränser till UTC.
        calendarDate = parsedDate.ToDateTime(
            TimeOnly.MinValue,
            DateTimeKind.Unspecified);

        return true;
    }
}