using progressa.Dtos;
using progressa.Models;
using progressa.Repositories.interfaces;
using progressa.Services.IServices;

namespace progressa.Services;

public class DailyLogService(IDailyLogRepository logs) : IDailyLogService
{
    public async Task<DailyLogDto> GetAsync(
        string userId, DateOnly date, CancellationToken ct = default)
    {
        var (nutrition, sleep) = await logs.GetAsync(userId, date, ct);
        return ToDto(nutrition, sleep);
    }

    public async Task<DailyLogDto?> SaveAsync(
        string userId, DateOnly date, DailyLogDto dto, CancellationToken ct = default)
    {
        var (nutrition, sleep) = await logs.GetAsync(userId, date, ct);

        if (nutrition?.Version != dto.Version)
        {
            return null;
        }

        // Samma version täcker både kost och sömn, även när man bara loggar sömn.
        if (nutrition is null)
        {
            nutrition = new NutritionLog { UserId = userId, Date = date };
            logs.Add(nutrition);
        }

        nutrition.Version = Guid.NewGuid();
        nutrition.ReachedCalories = RoundAmount(dto.Amounts.Calories);
        nutrition.ReachedProtein = RoundAmount(dto.Amounts.Protein);
        nutrition.ReachedCarbs = RoundAmount(dto.Amounts.Carbs);
        nutrition.ReachedFat = RoundAmount(dto.Amounts.Fat);
        nutrition.CaloriesGoalReached = dto.Checks.Calories;
        nutrition.ProteinGoalReached = dto.Checks.Protein;
        nutrition.CarbsGoalReached = dto.Checks.Carbs;
        nutrition.FatGoalReached = dto.Checks.Fat;

        if (sleep is null && (dto.SleepMinutes.HasValue || dto.Checks.Sleep.HasValue))
        {
            sleep = new SleepLog { UserId = userId, Date = date };
            logs.Add(sleep);
        }

        if (sleep is not null)
        {
            sleep.SleepHours = dto.SleepMinutes / 60.0;
            sleep.GoalReached = dto.Checks.Sleep;
        }

        return await logs.SaveChangesAsync(ct) ? ToDto(nutrition, sleep) : null;
    }

    private static decimal? RoundAmount(decimal? amount) =>
        amount.HasValue ? Math.Round(amount.Value, 2, MidpointRounding.AwayFromZero) : null;

    private static DailyLogDto ToDto(NutritionLog? nutrition, SleepLog? sleep) => new()
    {
        Version = nutrition?.Version,
        Amounts = new DailyLogAmountsDto
        {
            Calories = nutrition?.ReachedCalories,
            Protein = nutrition?.ReachedProtein,
            Carbs = nutrition?.ReachedCarbs,
            Fat = nutrition?.ReachedFat
        },
        Checks = new DailyLogChecksDto
        {
            Calories = nutrition?.CaloriesGoalReached,
            Protein = nutrition?.ProteinGoalReached,
            Carbs = nutrition?.CarbsGoalReached,
            Fat = nutrition?.FatGoalReached,
            Sleep = sleep?.GoalReached
        },
        SleepMinutes = sleep?.SleepHours is double hours ? (int)Math.Round(hours * 60) : null
    };
}
