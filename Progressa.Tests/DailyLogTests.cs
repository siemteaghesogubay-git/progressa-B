using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using progressa.Data;
using progressa.Dtos;
using progressa.Repositories;
using progressa.Services;

namespace Progressa.Tests;

public class DailyLogTests : IDisposable
{
    private static readonly DateOnly Day = new(2026, 11, 16);
    private readonly SqliteConnection connection = new("Data Source=:memory:");
    private readonly DbContextOptions<ApplicationDbContext> options;

    public DailyLogTests()
    {
        connection.Open();
        options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        using var db = new ApplicationDbContext(options);
        db.Database.EnsureCreated();
        db.Users.AddRange(new ApplicationUser { Id = "first" }, new ApplicationUser { Id = "second" });
        db.SaveChanges();
    }

    [Fact]
    public async Task SaveDailyLog_PersistsAmountsSleepAndChecksForOnlyThatUserAndDay()
    {
        using var db = new ApplicationDbContext(options);
        var service = new DailyLogService(new DailyLogRepository(db));

        var saved = await service.SaveAsync("first", Day, new DailyLogDto
        {
            Amounts = new DailyLogAmountsDto { Calories = 1560.5m, Fat = 0 },
            Checks = new DailyLogChecksDto { Calories = false, Sleep = true },
            SleepMinutes = 431
        });

        using var readDb = new ApplicationDbContext(options);
        var reader = new DailyLogService(new DailyLogRepository(readDb));
        var log = await reader.GetAsync("first", Day);

        Assert.NotNull(saved);
        Assert.NotNull(log.Version);
        Assert.Equal(saved.Version, log.Version);
        Assert.Equal(1560.5m, log.Amounts.Calories);
        Assert.Equal(0m, log.Amounts.Fat);
        Assert.Null(log.Amounts.Protein);
        Assert.False(log.Checks.Calories);
        Assert.True(log.Checks.Sleep);
        Assert.Equal(431, log.SleepMinutes);
        Assert.Null((await reader.GetAsync("second", Day)).Version);
        Assert.Null((await reader.GetAsync("first", Day.AddDays(1))).Version);
    }

    [Fact]
    public async Task SaveDailyLog_RejectsOldVersionAfterSleepOnlyUpdate()
    {
        using var db = new ApplicationDbContext(options);
        var service = new DailyLogService(new DailyLogRepository(db));
        var original = await service.SaveAsync("first", Day, new DailyLogDto { SleepMinutes = 420 });
        Assert.NotNull(original);

        var change = new DailyLogDto { Version = original.Version, SleepMinutes = 445 };
        var saved = await service.SaveAsync("first", Day, change);
        Assert.NotNull(saved);
        Assert.NotEqual(original.Version, saved.Version);

        Assert.Null(await service.SaveAsync("first", Day, change));
        Assert.Equal(445, (await service.GetAsync("first", Day)).SleepMinutes);
    }

    [Fact]
    public async Task SaveChanges_RollsBackBothTablesForAConcurrentUpdate()
    {
        using var db = new ApplicationDbContext(options);
        var repository = new DailyLogRepository(db);
        var service = new DailyLogService(repository);
        var original = await service.SaveAsync("first", Day, new DailyLogDto { SleepMinutes = 420 });
        Assert.NotNull(original);
        var (nutrition, sleep) = await repository.GetAsync("first", Day);
        Assert.NotNull(nutrition);
        Assert.NotNull(sleep);

        using var otherDb = new ApplicationDbContext(options);
        var otherService = new DailyLogService(new DailyLogRepository(otherDb));
        var saved = await otherService.SaveAsync("first", Day, new DailyLogDto
        {
            Version = original.Version,
            SleepMinutes = 450
        });
        Assert.NotNull(saved);

        nutrition.Version = Guid.NewGuid();
        sleep.SleepHours = 8;
        Assert.False(await repository.SaveChangesAsync());

        using var readDb = new ApplicationDbContext(options);
        var reader = new DailyLogService(new DailyLogRepository(readDb));
        Assert.Equal(450, (await reader.GetAsync("first", Day)).SleepMinutes);
    }

    public void Dispose() => connection.Dispose();
}
