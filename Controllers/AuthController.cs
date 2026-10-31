using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return NoContent();
        }
        
        [HttpPost("forgot-password")]
        [EnableRateLimiting("password-reset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            await authService.ForgotPasswordAsync(dto.Email);

            // Samma svar oavsett om kontot finns
            return Ok(new
            {
                message = "Om e-postadressen finns hos oss har vi skickat instruktioner för att återställa lösenordet."
            });
        }

        [HttpPost("reset-password")]
        [EnableRateLimiting("password-reset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var result = await authService.ResetPasswordAsync(dto);

            switch (result.Status)
            {
                case ResetPasswordStatus.Success:
                    return Ok(new { message = "Lösenordet har uppdaterats." });

                case ResetPasswordStatus.InvalidPassword:
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(nameof(dto.NewPassword), error);
                    return ValidationProblem(ModelState);

                default:
                    return BadRequest(new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = result.Errors.First()
                    });
            }
        }
    }
}