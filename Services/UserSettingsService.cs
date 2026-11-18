using progressa.Dtos;
using progressa.Models;
using progressa.Repositories.interfaces;
using progressa.Services.IServices;

namespace progressa.Services;

public class UserSettingsService(IUserSettingsRepository userSettings) : IUserSettingsService
{
    public async Task<UserSettingsDto> GetAsync(
        string userId,
        CancellationToken ct = default)
    {
        // No row is saved until the user changes something, so a new user gets the defaults.
        var settings = await userSettings.GetByUserIdAsync(userId, ct)
            ?? new UserSettings();

        return MapToDto(settings);
    }

    public async Task<UserSettingsDto> UpdateAsync(
        string userId,
        UpdateUserSettingsDto dto,
        CancellationToken ct = default)
    {
        var settings = await userSettings.GetByUserIdAsync(userId, ct);

        if (settings is null)
        {
            settings = new UserSettings { UserId = userId };
            ApplyChanges(settings, dto);
            await userSettings.AddAsync(settings, ct);
        }
        else
        {
            ApplyChanges(settings, dto);
            await userSettings.SaveChangesAsync(ct);
        }

        return MapToDto(settings);
    }

    private static void ApplyChanges(UserSettings settings, UpdateUserSettingsDto dto)
    {
        settings.DistanceUnit = dto.DistanceUnit;
        settings.WeightUnit = dto.WeightUnit;
        settings.EnergyUnit = dto.EnergyUnit;
        settings.HeightUnit = dto.HeightUnit;
        settings.TimeFormat = dto.TimeFormat;
        settings.WeekStart = dto.WeekStart;
    }

    private static UserSettingsDto MapToDto(UserSettings settings)
    {
        return new UserSettingsDto
        {
            DistanceUnit = settings.DistanceUnit,
            WeightUnit = settings.WeightUnit,
            EnergyUnit = settings.EnergyUnit,
            HeightUnit = settings.HeightUnit,
            TimeFormat = settings.TimeFormat,
            WeekStart = settings.WeekStart
        };
    }
}
