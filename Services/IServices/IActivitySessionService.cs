using static progressa.Dtos.ActivitySession_Dtos;

namespace progressa.Services.IServices;

public enum CreateSessionStatus
{
    Success,
    CategoryNotFound,
    CalendarNotFound
}

public record CreateSessionResult(CreateSessionStatus Status, ActivitySessionDto? Session)
{
    public static CreateSessionResult Ok(ActivitySessionDto s) => new(CreateSessionStatus.Success, s);
    public static CreateSessionResult CategoryNotFound() => new(CreateSessionStatus.CategoryNotFound, null);
    public static CreateSessionResult CalendarNotFound() => new(CreateSessionStatus.CalendarNotFound, null);
}

public interface IActivitySessionService
{
    Task<IReadOnlyList<CategoryOptionDto>> GetCategoriesAsync(string userId);
    Task<CreateSessionResult> CreateAsync(string userId, CreateActivitySessionDto dto);
}