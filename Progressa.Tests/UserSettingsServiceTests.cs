using progressa.Dtos;
using progressa.Models;
using progressa.Repositories.interfaces;
using progressa.Services;

namespace Progressa.Tests;

public class UserSettingsServiceTests
{
    [Fact]
    public async Task GetAsync_UserWithoutSavedSettings_ReturnsDefaults()
    {
        // Arrange
        var repository = new FakeUserSettingsRepository();
        var service = new UserSettingsService(repository);

        // Act
        var result = await service.GetAsync("user-1");

        // Assert
        Assert.Equal("km", result.DistanceUnit);
        Assert.Equal("24h", result.TimeFormat);
        Assert.Equal("mon", result.WeekStart);
        Assert.Empty(repository.Saved);
    }

    [Fact]
    public async Task UpdateAsync_UserWithoutSavedSettings_SavesThemForThatUser()
    {
        // Arrange
        var repository = new FakeUserSettingsRepository();
        var service = new UserSettingsService(repository);
        var dto = new UpdateUserSettingsDto
        {
            DistanceUnit = "mi",
            WeightUnit = "lb",
            EnergyUnit = "kj",
            HeightUnit = "ft",
            TimeFormat = "12h",
            WeekStart = "sun"
        };

        // Act
        var result = await service.UpdateAsync("user-1", dto);

        // Assert
        var saved = Assert.Single(repository.Saved);
        Assert.Equal("user-1", saved.UserId);
        Assert.Equal("sun", saved.WeekStart);
        Assert.Equal("mi", result.DistanceUnit);
    }

    // Stands in for the database so the service can be tested on its own.
    private class FakeUserSettingsRepository : IUserSettingsRepository
    {
        public List<UserSettings> Saved { get; } = new();

        public Task<UserSettings?> GetByUserIdAsync(
            string userId,
            CancellationToken ct = default)
        {
            return Task.FromResult(
                Saved.FirstOrDefault(settings => settings.UserId == userId));
        }

        public Task AddAsync(
            UserSettings settings,
            CancellationToken ct = default)
        {
            Saved.Add(settings);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(
            CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }
    }
}
