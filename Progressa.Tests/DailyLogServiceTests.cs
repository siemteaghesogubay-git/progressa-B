using progressa.Dtos;
using progressa.Models;
using progressa.Repositories.interfaces;
using progressa.Services;

namespace Progressa.Tests;

public class DailyLogServiceTests
{
    private const string UserId = "test-user";
    private static readonly DateOnly Day = new(2026, 11, 16);

    [Fact]
    public async Task GetDailyLog_ReturnsEmptyLogForMissingDay()
    {
        var repository = new TestRepository();
        var service = new DailyLogService(repository);

        var log = await service.GetAsync(UserId, Day);

        Assert.Equal((UserId, Day), repository.RequestedLog);
        Assert.Null(log.Version);
        Assert.Null(log.Amounts.Calories);
        Assert.Null(log.Checks.Sleep);
        Assert.Null(log.SleepMinutes);
        Assert.Equal(0, repository.SaveCount);
    }

    [Fact]
    public async Task SaveDailyLog_CreatesEntriesWithRoundedNutritionAndExactSleepMinutes()
    {
        var repository = new TestRepository();
        var service = new DailyLogService(repository);

        var saved = await service.SaveAsync(UserId, Day, new DailyLogDto
        {
            Amounts = new DailyLogAmountsDto { Calories = 425.125m, Protein = 0 },
            Checks = new DailyLogChecksDto { Protein = false, Sleep = true },
            SleepMinutes = 431
        });

        Assert.NotNull(saved);
        Assert.NotNull(repository.AddedNutrition);
        Assert.NotNull(repository.AddedSleep);
        Assert.Equal(UserId, repository.AddedNutrition.UserId);
        Assert.Equal(Day, repository.AddedNutrition.Date);
        Assert.Equal(UserId, repository.AddedSleep.UserId);
        Assert.Equal(Day, repository.AddedSleep.Date);
        Assert.Equal(425.13m, saved.Amounts.Calories);
        Assert.Equal(0m, saved.Amounts.Protein);
        Assert.Null(saved.Amounts.Carbs);
        Assert.False(saved.Checks.Protein);
        Assert.True(saved.Checks.Sleep);
        Assert.Equal(431, saved.SleepMinutes);
        Assert.Equal(1, repository.SaveCount);
    }

    [Fact]
    public async Task SaveDailyLog_NutritionOnlyDoesNotCreateSleepEntry()
    {
        var repository = new TestRepository();
        var service = new DailyLogService(repository);

        var saved = await service.SaveAsync(UserId, Day, new DailyLogDto
        {
            Amounts = new DailyLogAmountsDto { Calories = 200 }
        });

        Assert.NotNull(saved);
        Assert.Null(repository.AddedSleep);
        Assert.Null(saved.SleepMinutes);
    }

    [Fact]
    public async Task SaveDailyLog_UpdatesExistingSleepAndChangesTheVersion()
    {
        var version = Guid.NewGuid();
        var nutrition = new NutritionLog { Version = version };
        var sleep = new SleepLog { SleepHours = 7 };
        var repository = new TestRepository(nutrition, sleep);
        var service = new DailyLogService(repository);

        var saved = await service.SaveAsync(UserId, Day, new DailyLogDto
        {
            Version = version,
            SleepMinutes = 0
        });

        Assert.NotNull(saved);
        Assert.NotEqual(version, saved.Version);
        Assert.Equal(nutrition.Version, saved.Version);
        Assert.Equal(0, saved.SleepMinutes);
        Assert.Null(repository.AddedNutrition);
        Assert.Null(repository.AddedSleep);
        Assert.Equal(1, repository.SaveCount);
    }

    [Fact]
    public async Task SaveDailyLog_RejectsStaleVersionWithoutWriting()
    {
        var nutrition = new NutritionLog { Version = Guid.NewGuid(), ReachedCalories = 200 };
        var repository = new TestRepository(nutrition);
        var service = new DailyLogService(repository);

        var saved = await service.SaveAsync(UserId, Day, new DailyLogDto
        {
            Amounts = new DailyLogAmountsDto { Calories = 350 }
        });

        Assert.Null(saved);
        Assert.Equal(200m, nutrition.ReachedCalories);
        Assert.Equal(0, repository.SaveCount);
    }

    [Fact]
    public async Task SaveDailyLog_ReturnsConflictWhenRepositoryCannotSave()
    {
        var repository = new TestRepository(saveSucceeds: false);
        var service = new DailyLogService(repository);

        var saved = await service.SaveAsync(UserId, Day, new DailyLogDto { SleepMinutes = 450 });

        Assert.Null(saved);
        Assert.Equal(1, repository.SaveCount);
    }

    private sealed class TestRepository(
        NutritionLog? nutrition = null,
        SleepLog? sleep = null,
        bool saveSucceeds = true) : IDailyLogRepository
    {
        public (string UserId, DateOnly Date)? RequestedLog { get; private set; }
        public NutritionLog? AddedNutrition { get; private set; }
        public SleepLog? AddedSleep { get; private set; }
        public int SaveCount { get; private set; }

        public Task<(NutritionLog? Nutrition, SleepLog? Sleep)> GetAsync(
            string userId, DateOnly date, CancellationToken ct = default)
        {
            RequestedLog = (userId, date);
            return Task.FromResult((nutrition, sleep));
        }

        public void Add(NutritionLog log) => AddedNutrition = log;
        public void Add(SleepLog log) => AddedSleep = log;

        public Task<bool> SaveChangesAsync(CancellationToken ct = default)
        {
            SaveCount++;
            return Task.FromResult(saveSucceeds);
        }
    }
}
