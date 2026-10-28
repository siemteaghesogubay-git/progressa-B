namespace progressa.Services.IServices;

public enum DeleteUserStatus
{
    Success,
    NotFound,
    InvalidPassword,
    Failed
}

public record DeleteUserResult(DeleteUserStatus Status, IEnumerable<string> Errors)
{
    public static DeleteUserResult Ok() => new(DeleteUserStatus.Success, []);

    public static DeleteUserResult NotFound() =>
        new(DeleteUserStatus.NotFound, ["Användaren hittades inte."]);

    public static DeleteUserResult InvalidPassword() =>
        new(DeleteUserStatus.InvalidPassword, ["Fel lösenord."]);

    public static DeleteUserResult Failed(IEnumerable<string> errors) =>
        new(DeleteUserStatus.Failed, errors);
}


public interface IUserService
{
    Task<DeleteUserResult> DeleteAccountAsync(string userId, string password);
}
