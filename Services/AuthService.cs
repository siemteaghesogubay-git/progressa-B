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
    // =========================
    // REGISTRERING
    // =========================
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
            Age = dto.Age,
            EmailConfirmed = false
        };

        // Skapa standardkalender och kategorier
        user.Calendars.Add(new Calendar());
        user.Categories.Add(new Category { Name = "Löpning" });
        user.Categories.Add(new Category { Name = "Styrketräning" });
        user.Categories.Add(new Category { Name = "Promenad" });

        var create = await userManager.CreateAsync(
            user, dto.Password);

        if (!create.Succeeded)
        {
            if (create.Errors.Any(e =>
                e.Code is "DuplicateEmail" or "DuplicateUserName"))
            {
                return RegisterResult.EmailTaken();
            }

            return RegisterResult.Invalid(
                create.Errors.Select(e => e.Description));
        }

        // NYTT: Skicka verifieringsmejl
        try
        {
            await SendVerificationEmailAsync(user);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Kunde inte skicka verifieringsmejl.");
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

    // =========================
    // INLOGGNING
    // =========================
    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim();
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
            return null;

        var correctPassword =
            await userManager.CheckPasswordAsync(
                user, dto.Password);

        if (!correctPassword)
            return null;

        var jwt = configuration.GetSection("Jwt");
        var key = jwt["Key"]!;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!)
        };

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        var tokenString =
            new JwtSecurityTokenHandler().WriteToken(token);

        return new LoginResponseDto
        {
            Token = tokenString,
            UserId = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }

    // =========================
    // NYTT: VERIFIERA E-POST
    // =========================
    public async Task<bool> VerifyEmailAsync(
        string email,
        string token)
    {
        var user = await userManager.FindByEmailAsync(
            email.Trim());

        if (user is null)
            return false;

        string decodedToken;

        try
        {
            decodedToken = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(token));
        }
        catch (FormatException)
        {
            return false;
        }

        var result = await userManager.ConfirmEmailAsync(
            user,
            decodedToken);

        return result.Succeeded;
    }

    // =========================
    // NYTT: SKICKA NY VERIFIERINGSLÄNK
    // =========================
    public async Task ResendVerificationEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(
            email.Trim());

        if (user is null || user.EmailConfirmed)
            return;

        try
        {
            await SendVerificationEmailAsync(user);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Kunde inte skicka nytt verifieringsmejl.");
        }
    }

    // =========================
    // NYTT: SKICKA VERIFIERINGSMEJL
    // =========================
    private async Task SendVerificationEmailAsync(
        ApplicationUser user)
    {
        var token =
            await userManager.GenerateEmailConfirmationTokenAsync(
                user);

        var encodedToken = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(token));

        var baseUrl = (
            configuration["Frontend:BaseUrl"]
            ?? "http://localhost:5173"
        ).TrimEnd('/');

        var link =
            $"{baseUrl}/verify-email" +
            $"?email={Uri.EscapeDataString(user.Email!)}" +
            $"&token={encodedToken}";

        var body = $"""
            <h2>Välkommen till Progressa!</h2>

            <p>Hej {WebUtility.HtmlEncode(user.FirstName)},</p>

            <p>
                Tack för att du har registrerat dig hos Progressa.
                Klicka på knappen nedan för att verifiera
                din e-postadress.
            </p>

            <p>
                <a href="{WebUtility.HtmlEncode(link)}"
                   style="background-color:#2563eb;
                          color:white;
                          padding:12px 24px;
                          text-decoration:none;
                          border-radius:8px;
                          display:inline-block;">
                    Verifiera e-postadress
                </a>
            </p>

            <p>Verifieringslänken gäller i en timme.</p>

            <p>
                Om du inte har registrerat dig hos Progressa
                kan du ignorera detta mejl.
            </p>

            <p>Med vänliga hälsningar,<br>Progressa-teamet</p>
            """;

        await emailService.SendAsync(
            user.Email!,
            "Verifiera din e-postadress – Progressa",
            body);
    }

    // =========================
    // GLÖMT LÖSENORD
    // =========================
    public async Task ForgotPasswordAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(
            email.Trim());

        if (user is null)
            return;

        var token =
            await userManager.GeneratePasswordResetTokenAsync(
                user);

        var encoded = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(token));

        var baseUrl = (
            configuration["Frontend:BaseUrl"]
            ?? "http://localhost:5173"
        ).TrimEnd('/');

        var link =
            $"{baseUrl}/reset-password" +
            $"?email={Uri.EscapeDataString(user.Email!)}" +
            $"&token={encoded}";

        var body = $"""
            <p>Hej {WebUtility.HtmlEncode(user.FirstName)},</p>

            <p>
                Klicka på länken för att välja ett nytt
                lösenord. Den gäller i en timme.
            </p>

            <p>
                <a href="{WebUtility.HtmlEncode(link)}">
                    Återställ lösenord
                </a>
            </p>

            <p>
                Om du inte har begärt det kan du ignorera
                det här mejlet.
            </p>
            """;

        try
        {
            await emailService.SendAsync(
                user.Email!,
                "Återställ ditt lösenord",
                body);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Kunde inte skicka återställningsmail.");
        }
    }

    // =========================
    // ÅTERSTÄLL LÖSENORD
    // =========================
    public async Task<ResetPasswordResult> ResetPasswordAsync(
        ResetPasswordDto dto)
    {
        var passwordErrors =
            await ValidatePasswordAsync(dto.NewPassword);

        if (passwordErrors.Count > 0)
        {
            return new(
                ResetPasswordStatus.InvalidPassword,
                passwordErrors);
        }

        var invalid = new ResetPasswordResult(
            ResetPasswordStatus.InvalidToken,
            ["Ogiltig eller utgången återställningslänk."]);

        var user = await userManager.FindByEmailAsync(
            dto.Email.Trim());

        if (user is null)
            return invalid;

        string token;

        try
        {
            token = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(dto.Token));
        }
        catch (FormatException)
        {
            return invalid;
        }

        var result = await userManager.ResetPasswordAsync(
            user,
            token,
            dto.NewPassword);

        return result.Succeeded
            ? new(ResetPasswordStatus.Success, [])
            : invalid;
    }

    // =========================
    // VALIDERING AV LÖSENORD
    // =========================
    private async Task<List<string>> ValidatePasswordAsync(
        string password)
    {
        var errors = new List<string>();

        foreach (var validator in userManager.PasswordValidators)
        {
            var result = await validator.ValidateAsync(
                userManager,
                new ApplicationUser(),
                password);

            if (!result.Succeeded)
            {
                errors.AddRange(
                    result.Errors.Select(e => e.Description));
            }
        }

        return errors;
    }
}