using progressa.Dtos;
using progressa.Repositories.interfaces;
using progressa.Services.IServices;

namespace progressa.Services;

public class GoalService(
    IGoalRepository goals,
    ICalendarRepository calendars) : IGoalService
{
    public async Task<List<GoalDto>> GetAllAsync(
        string userId,
        CancellationToken ct = default)
    {
        var userGoals = await goals.GetAllAsync(userId, ct);

        return userGoals
            .Select(MapToDto)
            .ToList();
    }

    public async Task<GoalDto?> GetByIdAsync(
        int id,
        string userId,
        CancellationToken ct = default)
    {
        var goal = await goals.GetByIdAsync(id, userId, ct);

        return goal is null ? null : MapToDto(goal);
    }

    public async Task<GoalDto> CreateAsync(
        CreateGoalDto dto,
        string userId,
        CancellationToken ct = default)
    {
        await ValidateAsync(
            dto.StartDate,
            dto.EndDate,
            dto.CalendarId,
            userId,
            ct);

        var goal = new Goal
        {
            Title = dto.Title,
            Target = dto.Target,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsCompleted = false,
            OwnerUserId = userId,
            CalendarId = dto.CalendarId
        };

        await goals.AddAsync(goal, ct);

        return MapToDto(goal);
    }

    public async Task<GoalDto?> UpdateAsync(
        int id,
        UpdateGoalDto dto,
        string userId,
        CancellationToken ct = default)
    {
        var goal = await goals.GetForUpdateAsync(id, userId, ct);

        if (goal is null)
            return null;

        await ValidateAsync(
            dto.StartDate,
            dto.EndDate,
            dto.CalendarId,
            userId,
            ct);

        goal.Title = dto.Title;
        goal.Target = dto.Target;
        goal.StartDate = dto.StartDate;
        goal.EndDate = dto.EndDate;
        goal.IsCompleted = dto.IsCompleted;
        goal.CalendarId = dto.CalendarId;

        await goals.SaveChangesAsync(ct);

        return MapToDto(goal);
    }

    public async Task<bool> DeleteAsync(
        int id,
        string userId,
        CancellationToken ct = default)
    {
        var goal = await goals.GetForUpdateAsync(id, userId, ct);

        if (goal is null)
            return false;

        if (await goals.HasChallengesAsync(id, userId, ct))
        {
            throw new InvalidOperationException(
                "Målet används av en challenge och kan inte tas bort.");
        }

        await goals.DeleteAsync(goal, ct);

        return true;
    }

    private async Task ValidateAsync(
        DateTime start,
        DateTime end,
        int? calendarId,
        string userId,
        CancellationToken ct)
    {
        if (start == default || end == default || end < start)
        {
            throw new ArgumentException("Ogiltiga datum.");
        }

        if (calendarId.HasValue)
        {
            var exists = await calendars.ExistsAsync(
                calendarId.Value,
                userId,
                ct);

            if (!exists)
            {
                throw new ArgumentException("Kalendern finns inte.");
            }
        }
    }

    private static GoalDto MapToDto(Goal goal)
    {
        return new GoalDto
        {
            Id = goal.Id,
            Title = goal.Title,
            Target = goal.Target,
            StartDate = goal.StartDate,
            EndDate = goal.EndDate,
            IsCompleted = goal.IsCompleted,
            CalendarId = goal.CalendarId
        };
    }
}