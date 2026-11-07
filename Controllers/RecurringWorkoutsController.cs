using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using progressa.Dtos;
using progressa.Services.IServices;

namespace progressa.Controllers;

[ApiController]
[Authorize]
[Route("api/recurring-workouts")]
public class RecurringWorkoutsController(
    IRecurringWorkoutService service) : ControllerBase
{
    private string? CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub");

    // Hämta användarens samtliga veckoscheman.
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var userId = CurrentUserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        return Ok(await service.GetAllAsync(userId, ct));
    }

    // Hämta ett eget veckoschema.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken ct)
    {
        var userId = CurrentUserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var schedule = await service.GetAsync(id, userId, ct);

        if (schedule is null)
        {
            return NotFound();
        }

        return Ok(schedule);
    }

    // Skapa ett veckoschema.
    [HttpPost]
    public async Task<IActionResult> Create(
        SaveRecurringWorkoutDto dto,
        CancellationToken ct)
    {
        var userId = CurrentUserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var result = await service.SaveAsync(
            null,
            userId,
            dto,
            ct);

        if (result.Status == ScheduleWriteStatus.InvalidReferences)
        {
            return InvalidReferences();
        }

        var schedule = await service.GetAsync(
            result.Id!.Value,
            userId,
            ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id.Value },
            schedule);
    }

    // Ersätt uppgifterna i ett eget veckoschema.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        SaveRecurringWorkoutDto dto,
        CancellationToken ct)
    {
        var userId = CurrentUserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var result = await service.SaveAsync(
            id,
            userId,
            dto,
            ct);

        switch (result.Status)
        {
            case ScheduleWriteStatus.NotFound:
                return NotFound();

            case ScheduleWriteStatus.InvalidReferences:
                return InvalidReferences();

            default:
                return NoContent();
        }
    }

    // Ta bort ett eget veckoschema.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken ct)
    {
        var userId = CurrentUserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var deleted = await service.DeleteAsync(id, userId, ct);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    // Beräkna träningstillfällen inom ett lokalt datumintervall.
    // from inkluderas, to inkluderas inte.
    [HttpGet("occurrences")]
    public async Task<IActionResult> GetOccurrences(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        CancellationToken ct)
    {
        var userId = CurrentUserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (!from.HasValue || !to.HasValue)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Ange både from och to.");
        }

        var numberOfDays = to.Value.DayNumber - from.Value.DayNumber;

        if (numberOfDays < 1 || numberOfDays > 366)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Datumintervallet måste vara mellan 1 och 366 dagar.");
        }

        var occurrences = await service.GetOccurrencesAsync(
            userId,
            from.Value,
            to.Value,
            ct);

        return Ok(occurrences);
    }

    private ObjectResult InvalidReferences()
    {
        return Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Kalendern eller aktiviteten finns inte eller tillhör inte dig.");
    }
}