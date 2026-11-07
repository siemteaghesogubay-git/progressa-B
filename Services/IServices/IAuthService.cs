using progressa.Dtos;
using static progressa.Dtos.User_Dtos;

namespace progressa.Services.IServices;

// ---------- Registrering ----------
public enum RegisterStatus
{
    Success,
    EmailInUse,
    Invalid
}

public record RegisterResult(
    RegisterStatus Status,
    RegisterResponseDto? User,
    IEnumerable<string> Errors)
{
    public static RegisterResult Ok(RegisterResponseDto user) =>
        new(RegisterStatus.Success, user, []);

    public static RegisterResult EmailTaken() =>
        new(RegisterStatus.EmailInUse, null,
            ["E-postadressen används redan."]);

    public static RegisterResult Invalid(IEnumerable<string> errors) =>
        new(RegisterStatus.Invalid, null, errors);
}

// ---------- Återställning av lösenord ----------
public enum ResetPasswordStatus
{
    Success,
    InvalidToken,
    InvalidPassword
}

public record ResetPasswordResult(
    ResetPasswordStatus Status,
    IEnumerable<string> Errors);

// ---------- Gränssnitt ----------
public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterDto dto);
    Task<LoginResponseDto?> LoginAsync(LoginDto dto);
    Task ForgotPasswordAsync(string email);
    Task<ResetPasswordResult> ResetPasswordAsync(ResetPasswordDto dto);
}