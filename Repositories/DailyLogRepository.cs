using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using progressa.Data;
using progressa.Models;
using progressa.Repositories.interfaces;

namespace progressa.Repositories;

public class DailyLogRepository(ApplicationDbContext db) : IDailyLogRepository
{
    public async Task<(NutritionLog? Nutrition, SleepLog? Sleep)> GetAsync(
        string userId, DateOnly date, CancellationToken ct = default)
    {
        // Läs versionen först så att en ändring mellan frågorna upptäcks vid sparning.
        var nutrition = await db.NutritionLogs.SingleOrDefaultAsync(
            log => log.UserId == userId && log.Date == date, ct);
        var sleep = await db.SleepLogs.SingleOrDefaultAsync(
            log => log.UserId == userId && log.Date == date, ct);

        return (nutrition, sleep);
    }

    public void Add(NutritionLog log) => db.NutritionLogs.Add(log);

    public void Add(SleepLog log) => db.SleepLogs.Add(log);

    public async Task<bool> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            await db.SaveChangesAsync(ct);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            // Två anrop kan försöka skapa samma dagslogg samtidigt.
            return false;
        }
    }
}
