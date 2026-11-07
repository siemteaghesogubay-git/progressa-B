using progressa.Dtos;

namespace progressa.Services.IServices;

public interface IGoalService
{
    Task<List<GoalDto>> GetAllAsync(
        string userId,
        CancellationToken ct = default);

    Task<GoalDto?> GetByIdAsync(
        int id,
        string userId,
        CancellationToken ct = default);

    Task<GoalDto> CreateAsync(
        CreateGoalDto dto,
        string userId,
        CancellationToken ct = default);

    Task<GoalDto?> UpdateAsync(
        int id,
        UpdateGoalDto dto,
        string userId,
        CancellationToken ct = default);

    Task<bool> DeleteAsync(
        int id,
        string userId,
        CancellationToken ct = default);
}