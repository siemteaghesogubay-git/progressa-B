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
