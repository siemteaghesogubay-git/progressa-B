using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using progressa.Dtos;
using progressa.Services.IServices;
using static progressa.Dtos.ActivitySession_Dtos;

namespace progressa.Controllers;

[ApiController]
[Authorize]
[Route("api/activity-sessions")]
public class ActivitySessionsController(IActivitySessionService service) : ControllerBase
{
    private string? CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

    // Aktiviteterna användaren kan välja bland
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryOptionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCategories()
    {
        if (CurrentUserId is null) return Unauthorized();
        return Ok(await service.GetCategoriesAsync(CurrentUserId));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ActivitySessionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(CreateActivitySessionDto dto)
    {
        // Ägaren kommer alltid från token, aldrig från requesten
        if (CurrentUserId is null) return Unauthorized();

        var result = await service.CreateAsync(CurrentUserId, dto);

        switch (result.Status)
        {
            case CreateSessionStatus.Success:
                return StatusCode(StatusCodes.Status201Created, result.Session);

            case CreateSessionStatus.CategoryNotFound:
                ModelState.AddModelError(nameof(dto.CategoryId), "Aktiviteten finns inte.");
                return ValidationProblem(ModelState);

            default:
                ModelState.AddModelError(nameof(dto.CalendarId), "Kalendern finns inte.");
                return ValidationProblem(ModelState);
        }

    }

    // Skapa ett planerat aktivitetspass
    [HttpPost("planned")]
    [ProducesResponseType(typeof(ActivitySessionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreatePlanned(CreatePlannedSessionDto dto)
    {
        if (CurrentUserId is null)
            return Unauthorized();

        var result = await service.CreatePlannedSessionAsync(CurrentUserId, dto);

        switch (result.Status)
        {
            case CreateSessionStatus.Success:
                return StatusCode(StatusCodes.Status201Created, result.Session);

            case CreateSessionStatus.CategoryNotFound:
                ModelState.AddModelError(nameof(dto.CategoryId), "Kategorin finns inte.");
                return ValidationProblem(ModelState);

            default:
                ModelState.AddModelError(nameof(dto.CalendarId), "Kalendern finns inte.");
                return ValidationProblem(ModelState);
        }
    }

    // Markera ett planerat aktivitetspass som genomfört
    [HttpPatch("{id:int}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompletePlanned(
        int id,
        CompletePlannedSessionDto dto)
    {
        if (CurrentUserId is null)
            return Unauthorized();

        var success = await service.CompletePlannedSessionAsync(
            id, 
            CurrentUserId, dto);

        if (!success)
            return NotFound();

        return NoContent();
    }

    // Ta bort ett planerat aktivitetspass
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePlanned(
        int id,
        CancellationToken ct)
    {
        var userId = CurrentUserId;

        if (userId is null)
            return Unauthorized();

        var deleted = await service.DeletePlannedSessionAsync(id, userId, ct);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}