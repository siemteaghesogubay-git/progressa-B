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
    /// <summary>
    /// Raderar användaren och all data som hör till användaren permanent (hård radering).
    /// Raderingen sker i en transaktion: antingen tas allt bort eller ingenting.
    /// </summary>
    /// <param name="userId">Id för den inloggade användaren, hämtat från token.</param>
    /// <param name="password">Användarens lösenord, som bekräftelse.</param>
    /// <returns>
    /// <see cref="DeleteUserStatus.Success"/> om kontot raderades,
    /// <see cref="DeleteUserStatus.NotFound"/> om användaren inte finns,
    /// <see cref="DeleteUserStatus.InvalidPassword"/> om lösenordet är fel, och
    /// <see cref="DeleteUserStatus.Failed"/> om Identity inte kunde radera användaren.
    /// </returns>
    Task<DeleteUserResult> DeleteAccountAsync(string userId, string password);
}
