using progressa.Models;

namespace progressa.Repositories.interfaces;

public interface IUserSettingsRepository
{
    Task<UserSettings?> GetByUserIdAsync(
        string userId,
        CancellationToken ct = default);

    Task AddAsync(
        UserSettings settings,
        CancellationToken ct = default);

    Task SaveChangesAsync(
        CancellationToken ct = default);
}
