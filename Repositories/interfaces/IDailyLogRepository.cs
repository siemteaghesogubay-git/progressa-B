using progressa.Models;

namespace progressa.Repositories.interfaces;

public interface IDailyLogRepository
{
    Task<(NutritionLog? Nutrition, SleepLog? Sleep)> GetAsync(
        string userId, DateOnly date, CancellationToken ct = default);

    void Add(NutritionLog log);
    void Add(SleepLog log);
    Task<bool> SaveChangesAsync(CancellationToken ct = default);
}
