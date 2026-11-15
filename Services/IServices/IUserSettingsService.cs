using progressa.Dtos;

namespace progressa.Services.IServices;

public interface IUserSettingsService
{
    Task<UserSettingsDto> GetAsync(
        string userId,
        CancellationToken ct = default);

    Task<UserSettingsDto> UpdateAsync(
        string userId,
        UpdateUserSettingsDto dto,
        CancellationToken ct = default);
}
