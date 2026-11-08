namespace progressa.Services.IServices;

public interface IUserService
{
    Task<DeleteUserResult> DeleteAccountAsync(
        string userId,
        string password);
}

public enum DeleteUserStatus
{
    Success,
    NotFound,
    InvalidPassword,
    Failed
}

public class DeleteUserResult
{
    public DeleteUserStatus Status { get; init; }

    public IEnumerable<string> Errors { get; init; }
        = Array.Empty<string>();

    public static DeleteUserResult Ok()
    {
        return new DeleteUserResult
        {
            Status = DeleteUserStatus.Success
        };
    }

    public static DeleteUserResult NotFound()
    {
        return new DeleteUserResult
        {
            Status = DeleteUserStatus.NotFound
        };
    }

    public static DeleteUserResult InvalidPassword()
    {
        return new DeleteUserResult
        {
            Status = DeleteUserStatus.InvalidPassword
        };
    }

    public static DeleteUserResult Failed(
        IEnumerable<string> errors)
    {
        return new DeleteUserResult
        {
            Status = DeleteUserStatus.Failed,
            Errors = errors.ToArray()
        };
    }
}