using progressa.Dtos;
using static progressa.Dtos.ActivitySession_Dtos;

namespace progressa.Services.IServices;

public enum CreateSessionStatus
{
    Success,
    CategoryNotFound,
    CalendarNotFound
}

public record CreateSessionResult(
    CreateSessionStatus Status,
    ActivitySessionDto? Session)
{
    public static CreateSessionResult Ok(ActivitySessionDto session)
    {
        return new(CreateSessionStatus.Success, session);
    }

    public static CreateSessionResult CategoryNotFound()
    {
        return new(CreateSessionStatus.CategoryNotFound, null);
    }

    public static CreateSessionResult CalendarNotFound()
    {
        return new(CreateSessionStatus.CalendarNotFound, null);
    }
}

public enum UpdatePlannedSessionStatus
{
    Success,
    SessionNotFound,
    CategoryNotFound
}

public interface IActivitySessionService
{
    Task<IReadOnlyList<CategoryOptionDto>> GetCategoriesAsync(
        string userId,
        CancellationToken ct = default);

    Task<CreateSessionResult> CreateAsync(
        string userId,
        CreateActivitySessionDto dto,
        CancellationToken ct = default);

    Task<CreateSessionResult> CreatePlannedSessionAsync(
        string userId,
        CreatePlannedSessionDto dto,
        CancellationToken ct = default);

    Task<bool> CompletePlannedSessionAsync(
        int sessionId,
        string userId,
        CompletePlannedSessionDto dto,
        CancellationToken ct = default);

    Task<bool> DeletePlannedSessionAsync(
        int sessionId,
        string userId,
        CancellationToken ct = default);

    Task<UpdatePlannedSessionStatus> UpdatePlannedSessionAsync(
        int sessionId,
        string userId,
        UpdatePlannedSessionDto dto,
        CancellationToken ct = default);
}