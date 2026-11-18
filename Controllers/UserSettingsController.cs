using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using progressa.Dtos;
using progressa.Services.IServices;
using System.Security.Claims;

namespace progressa.Controllers
{
    [ApiController]
    [Route("api/user-settings")]
    [Authorize]
    public class UserSettingsController : ControllerBase
    {
        private readonly IUserSettingsService _userSettingsService;

        public UserSettingsController(IUserSettingsService userSettingsService)
        {
            _userSettingsService = userSettingsService;
        }

        [HttpGet]
        public async Task<ActionResult<UserSettingsDto>> GetAsync(CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var settings = await _userSettingsService.GetAsync(userId, ct);

            return Ok(settings);
        }

        [HttpPut]
        public async Task<ActionResult<UserSettingsDto>> UpdateAsync(
            UpdateUserSettingsDto dto,
            CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var settings = await _userSettingsService.UpdateAsync(userId, dto, ct);

            return Ok(settings);
        }
    }
}
