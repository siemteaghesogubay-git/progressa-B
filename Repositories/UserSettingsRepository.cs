using Microsoft.EntityFrameworkCore;
using progressa.Data;
using progressa.Models;
using progressa.Repositories.interfaces;

namespace progressa.Repositories;

public class UserSettingsRepository(ApplicationDbContext db) : IUserSettingsRepository
{
    public Task<UserSettings?> GetByUserIdAsync(
        string userId,
        CancellationToken ct = default)
    {
        return db.UserSettings
            .FirstOrDefaultAsync(settings => settings.UserId == userId, ct);
    }

    public async Task AddAsync(
        UserSettings settings,
        CancellationToken ct = default)
    {
        db.UserSettings.Add(settings);
        await db.SaveChangesAsync(ct);
    }

    public async Task SaveChangesAsync(
        CancellationToken ct = default)
    {
        await db.SaveChangesAsync(ct);
    }
}
