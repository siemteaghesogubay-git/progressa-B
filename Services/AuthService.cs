using Microsoft.AspNetCore.Identity;
using progressa.Models;
using progressa.Services.IServices;
using static progressa.Dtos.User_Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace progressa.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration
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

    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim();
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
            return null;

        var correctPassword = await userManager.CheckPasswordAsync(user, dto.Password);
        if (!correctPassword)
            return null;

        var jwt = configuration.GetSection("Jwt");
        var key = jwt["Key"]!;

        // Information som sparas i token
        var  claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!)
        };

        // skapa signeringsnyckel
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        // Skapa jwt-token
        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
            );

        // Omvandla token till text
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new LoginResponseDto
        {
            Token = tokenString,
            UserId = user.Id,
            Email = user.Email!
        };
    }
}