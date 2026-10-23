using Microsoft.AspNetCore.Identity;
using progressa.Models;
using progressa.Services.IServices;
using static progressa.Dtos.User_Dtos;

namespace progressa.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager
) : IAuthService
{
    public async Task<RegisterResult> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim();

        if (await userManager.FindByEmailAsync(email) is not null)
            return RegisterResult.EmailTaken();

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Age = dto.Age
        };

        var create = await userManager.CreateAsync(user, dto.Password);

        if (!create.Succeeded)
        {
            if (create.Errors.Any(e =>
                e.Code is "DuplicateEmail" or "DuplicateUserName"))
            {
                return RegisterResult.EmailTaken();
            }

            return RegisterResult.Invalid(
                create.Errors.Select(e => e.Description)
            );
        }

        return RegisterResult.Ok(new RegisterResponseDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt
        });
    }
}