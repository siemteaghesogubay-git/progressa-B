using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using progressa.Services.IServices;
using System.Security.Claims;

namespace progressa.Controllers
{
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

        // Visar användarens aktiviteter för en specifik dag
        [HttpGet("day")]
        public async Task<IActionResult> GetByDayAsync(DateTime date)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userId == null)
                return Unauthorized();

            var sessions = await _calendarService.GetByDayAsync(date, userId);
            return Ok(sessions);
        }

        // Visar användarens aktiviteter för en specifik vecka
        [HttpGet("week")]
        public async Task<IActionResult> GetByWeekAsync(DateTime date)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var sessions = await _calendarService.GetByWeekAsync(date, userId);
            return Ok(sessions);
        }

        // Visar användarens aktiviteter för en specifik månad
        [HttpGet("month")]
        public async Task<IActionResult> GetByMonthAsync(DateTime date)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userId == null)
                return Unauthorized();

            var sessions = await _calendarService.GetByMonthAsync(date, userId);
            return Ok(sessions);
        }
    }
}
