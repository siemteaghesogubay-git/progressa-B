using progressa.Dtos;
using progressa.Repositories.interfaces;
using progressa.Services.IServices;

namespace progressa.Services;

public class GoalService(
    IGoalRepository goals,
    ICalendarRepository calendars,
    ICategoryRepository categories) : IGoalService
{
    public async Task<List<GoalDto>> GetAllAsync(
        string userId,
        CancellationToken ct = default)
    {
        var userGoals = await goals.GetAllAsync(userId, ct);

        var result = new List<GoalDto>();
        foreach (var goal in userGoals)
        {
            result.Add(await MapToDtoAsync(goal, ct));
        }
        return result;
    }

    public async Task<GoalDto?> GetByIdAsync(
        int id,
        string userId,
        CancellationToken ct = default)
    {
        var goal = await goals.GetByIdAsync(id, userId, ct);

        return goal is null ? null : await MapToDtoAsync(goal, ct);
    }

    public async Task<GoalDto> CreateAsync(
        CreateGoalDto dto,
        string userId,
        CancellationToken ct = default)
    {
        await ValidateGoalMetricAsync(dto.Metric, dto.CategoryId, userId, ct);
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
            Metric = dto.Metric,
            CategoryId = dto.CategoryId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsCompleted = false,
            OwnerUserId = userId,
            CalendarId = dto.CalendarId
        };

        await goals.AddAsync(goal, ct);

        return await MapToDtoAsync(goal, ct);
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
        await ValidateGoalMetricAsync(dto.Metric, dto.CategoryId, userId, ct);
        await ValidateAsync(
            dto.StartDate,
            dto.EndDate,
            dto.CalendarId,
            userId,
            ct);

        goal.Title = dto.Title;
        goal.Target = dto.Target;
        if (dto.Metric.HasValue)
        {
            goal.Metric = dto.Metric;
            goal.CategoryId = dto.CategoryId;
        }
        goal.StartDate = dto.StartDate;
        goal.EndDate = dto.EndDate;
        goal.IsCompleted = dto.IsCompleted;
        goal.CalendarId = dto.CalendarId;

        await goals.SaveChangesAsync(ct);

        return await MapToDtoAsync(goal, ct);
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

    private async Task ValidateGoalMetricAsync(
    GoalMetric? metric,
    int? categoryId,
    string userId,
    CancellationToken ct)
    {
        if (metric.HasValue && !Enum.IsDefined(typeof(GoalMetric), metric.Value))
        {
            throw new ArgumentException("Ogiltigt mått för målet.");
        }

        if (categoryId is <= 0)
        {
            throw new ArgumentException("Ogiltigt id för aktiviteten.");
        }

        if (metric == GoalMetric.Sessions && !categoryId.HasValue)
        {
            throw new ArgumentException("Välj en aktivitet för att räkna pass.");
        }

        if (categoryId.HasValue && (metric is null || metric == GoalMetric.Variety))
        {
            throw new ArgumentException("Det här målet ska inte kopplas till en enskild aktivitet.");
        }

        if (categoryId.HasValue)
        {
            var category = await categories.GetByIdAsync(categoryId.Value, userId, ct);

            if (category is null)
            {
                throw new ArgumentException("Aktiviteten finns inte bland dina aktiviteter.");
            }
        }
    }

    private async Task<GoalDto> MapToDtoAsync(Goal goal, CancellationToken ct)
    {
        int? progress = null;

        if (goal.Metric.HasValue)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");
            var startDate = DateTime.SpecifyKind(goal.StartDate.Date, DateTimeKind.Unspecified);
            var endDate = DateTime.SpecifyKind(goal.EndDate.Date, DateTimeKind.Unspecified);

            var startUtc = TimeZoneInfo.ConvertTimeToUtc(startDate, timeZone);
            var endExclusiveUtc = endDate == DateTime.MaxValue.Date
                ? DateTime.MaxValue
                : TimeZoneInfo.ConvertTimeToUtc(endDate.AddDays(1), timeZone);

            progress = await goals.GetProgressAsync(goal, startUtc, endExclusiveUtc, ct);
        }

        return new GoalDto
        {
            Id = goal.Id,
            Title = goal.Title,
            Target = goal.Target,
            Metric = goal.Metric,
            CategoryId = goal.CategoryId,
            Progress = progress,
            StartDate = goal.StartDate,
            EndDate = goal.EndDate,
            IsCompleted = goal.IsCompleted,
            CalendarId = goal.CalendarId
        };
    }
}