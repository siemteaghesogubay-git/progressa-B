using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using progressa.Dtos;
using progressa.Services.IServices;
using System.Security.Claims;

namespace progressa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GoalsController : ControllerBase
    {
        private readonly IGoalService _goalService;

        public GoalsController(IGoalService goalService)
        {
            _goalService = goalService;
        }

        // Hämta alla mål
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var goals = await _goalService.GetAllAsync(userId);

            return Ok(goals);
        }

        // Hämta ett specifikt mål
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var goal = await _goalService.GetByIdAsync(id, userId);

            if (goal == null)
                return NotFound();

            return Ok(goal);
        }

        // Skapa ett nytt mål
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateGoalDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            try
            {
                var goal = await _goalService.CreateAsync(dto, userId);

                return StatusCode(201, goal);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Uppdatera ett mål
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, UpdateGoalDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            try
            {
                var goal = await _goalService.UpdateAsync(id, dto, userId);

                if (goal == null)
                    return NotFound();

                return Ok(goal);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Ta bort ett mål
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            try
            {
                var deleted = await _goalService.DeleteAsync(id, userId);

                if (!deleted)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}