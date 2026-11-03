using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
}