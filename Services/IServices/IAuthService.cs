using static progressa.Dtos.User_Dtos;

namespace progressa.Services.IServices;

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

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterDto dto);
}