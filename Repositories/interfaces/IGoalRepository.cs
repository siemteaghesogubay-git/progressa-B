using progressa.Models;

namespace progressa.Repositories.interfaces;

public interface IGoalRepository
{
    Task<List<Goal>> GetAllAsync(
        string userId,
        CancellationToken ct = default);

    Task<Goal?> GetByIdAsync(
        int id,
        string userId,
        CancellationToken ct = default);

    Task<Goal?> GetForUpdateAsync(
        int id,
        string userId,
        CancellationToken ct = default);

    Task<bool> HasChallengesAsync(
        int id,
        string userId,
        CancellationToken ct = default);

    Task AddAsync(
        Goal goal,
        CancellationToken ct = default);

    Task SaveChangesAsync(
        CancellationToken ct = default);

    Task DeleteAsync(
        Goal goal,
        CancellationToken ct = default);
}