using progressa.Dtos;

namespace progressa.Services.IServices;

public interface IDailyLogService
{
    Task<DailyLogDto> GetAsync(string userId, DateOnly date, CancellationToken ct = default);

    Task<DailyLogDto?> SaveAsync(
        string userId, DateOnly date, DailyLogDto dto, CancellationToken ct = default);
}
