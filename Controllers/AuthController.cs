using Microsoft.AspNetCore.Mvc;
using progressa.Dtos;
using progressa.Services.IServices;
using static progressa.Dtos.User_Dtos;

namespace progressa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await authService.RegisterAsync(dto);

            switch (result.Status)
            {
                case RegisterStatus.Success:
                    return StatusCode(StatusCodes.Status201Created, result.User);

                case RegisterStatus.EmailInUse:
                    return Conflict(new ProblemDetails
                    {
                        Status = StatusCodes.Status409Conflict,
                        Title = "E-postadressen används redan."
                    });

                default:
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("Password", error);
                    return ValidationProblem(ModelState);
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await authService.LoginAsync(dto);
            if (result == null)
            {
                return Unauthorized(new
                {
                    Message = "Felaktig e-post eller lösenord."
                });
            }
            return Ok(result);
        }
    }
}