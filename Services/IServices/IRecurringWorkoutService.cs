using progressa.Dtos;

namespace progressa.Services.IServices;

public enum ScheduleWriteStatus
{
    Success,
    NotFound,
    InvalidReferences,
    InvalidStartDate
}

public record ScheduleWriteResult(
    ScheduleWriteStatus Status,
    int? Id = null);

public interface IRecurringWorkoutService
{
    Task<List<RecurringWorkoutDto>> GetAllAsync(
        string userId,
        CancellationToken ct = default);

    Task<RecurringWorkoutDto?> GetAsync(
        int id,
        string userId,
        CancellationToken ct = default);

    Task<ScheduleWriteResult> SaveAsync(
        int? id,
        string userId,
        SaveRecurringWorkoutDto dto,
        CancellationToken ct = default);

    Task<bool> DeleteAsync(
        int id,
        string userId,
        CancellationToken ct = default);

    Task<List<RecurringWorkoutOccurrenceDto>> GetOccurrencesAsync(
        string userId,
        DateOnly from,
        DateOnly to,
        CancellationToken ct = default);
}