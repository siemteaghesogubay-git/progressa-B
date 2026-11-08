using progressa.Models;

namespace progressa.Repositories.interfaces;

public interface IRecurringWorkoutRepository
{
    Task<List<RecurringWorkoutSchedule>> GetAllAsync(
        string userId,
        CancellationToken ct = default);

    Task<RecurringWorkoutSchedule?> GetAsync(
        int id,
        string userId,
        CancellationToken ct = default);

    Task<bool> OwnsReferencesAsync(
        string userId,
        int calendarId,
        int categoryId,
        CancellationToken ct = default);

    void Add(RecurringWorkoutSchedule schedule);

    void Remove(RecurringWorkoutSchedule schedule);

    Task SaveAsync(CancellationToken ct = default);
}