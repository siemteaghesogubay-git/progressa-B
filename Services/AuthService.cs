using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using progressa.Models;
using progressa.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using static progressa.Dtos.User_Dtos;

namespace progressa.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration,
    IEmailService emailService,
    ILogger<AuthService> logger
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
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!)
        };

        // Skapa signeringsnyckel
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

    public async Task ForgotPasswordAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email.Trim());
        if (user is null) return; // utåt blir svaret detsamma

        // Identity skapar en säker token som är knuten till användarens SecurityStamp
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var baseUrl = configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";
        var link = $"{baseUrl}/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={encoded}";

        var body = $"""
            <p>Hej {WebUtility.HtmlEncode(user.FirstName)},</p>
            <p>Klicka på länken för att välja ett nytt lösenord. Den gäller i en timme.</p>
            <p><a href="{WebUtility.HtmlEncode(link)}">Återställ lösenord</a></p>
            <p>Om du inte har begärt det kan du ignorera det här mejlet.</p>
            """;

        try
        {
            await emailService.SendAsync(user.Email!, "Återställ ditt lösenord", body);
        }
        catch (Exception ex)
        {
            // Felet får inte synas för anroparen, då avslöjas att kontot finns
            logger.LogError(ex, "Kunde inte skicka återställningsmail.");
        }
    }

    public async Task<ResetPasswordResult> ResetPasswordAsync(ResetPasswordDto dto)
    {
        // Lösenordet valideras först, lika för alla, så att svaret inte avslöjar om kontot finns
        var passwordErrors = await ValidatePasswordAsync(dto.NewPassword);
        if (passwordErrors.Count > 0)
            return new(ResetPasswordStatus.InvalidPassword, passwordErrors);

        var invalid = new ResetPasswordResult(
            ResetPasswordStatus.InvalidToken,
            ["Ogiltig eller utgången återställningslänk."]);

        var user = await userManager.FindByEmailAsync(dto.Email.Trim());
        if (user is null) return invalid;

        string token;
        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));
        }
        catch (FormatException)
        {
            return invalid;
        }

        // Token blir ogiltig efteråt eftersom SecurityStamp uppdateras
        var result = await userManager.ResetPasswordAsync(user, token, dto.NewPassword);

        return result.Succeeded
            ? new(ResetPasswordStatus.Success, [])
            : invalid;
    }

    private async Task<List<string>> ValidatePasswordAsync(string password)
    {
        var errors = new List<string>();

        foreach (var validator in userManager.PasswordValidators)
        {
            var r = await validator.ValidateAsync(userManager, new ApplicationUser(), password);
            if (!r.Succeeded)
                errors.AddRange(r.Errors.Select(e => e.Description));
        }

        return errors;
    }
}