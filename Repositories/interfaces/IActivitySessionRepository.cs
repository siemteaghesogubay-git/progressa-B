using progressa.Models;

namespace progressa.Repositories.interfaces;

public interface IActivitySessionRepository
{
    Task<List<ActivitySession>> GetByPeriodAsync(
        string userId,
        DateTime start,
        DateTime end,
        CancellationToken ct = default);

    Task AddAsync(
        ActivitySession session,
        CancellationToken ct = default);
}