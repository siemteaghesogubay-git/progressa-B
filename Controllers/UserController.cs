using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using progressa.Dtos;
using progressa.Services.IServices;
using System.Security.Claims;

namespace progressa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpDelete("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteAccountDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var result = await userService.DeleteAccountAsync(userId, dto.Password);

            switch (result.Status)
            {
                case DeleteUserStatus.Success:
                    return NoContent();

                case DeleteUserStatus.NotFound:
                    return NotFound(new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = result.Errors.First()
                    });

                // 400, inte 401: användaren är inloggad, det är bekräftelsen som är fel.
                // 401 får klienten att logga ut användaren.
                case DeleteUserStatus.InvalidPassword:
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(nameof(dto.Password), error);
                    }
                    return ValidationProblem(ModelState);

                default:
                    return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Account couldn't be deleted");
            }
        }
    }
}
