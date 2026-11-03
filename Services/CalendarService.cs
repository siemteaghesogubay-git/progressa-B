using progressa.Dtos;
using progressa.Repositories.interfaces;
using progressa.Services.IServices;

namespace progressa.Services;

public class CalendarService(
    IActivitySessionRepository sessions) : ICalendarService
{
    public Task<List<CalendarSessionDto>> GetByDayAsync(
        DateTime date,
        string userId,
        CancellationToken ct = default)
    {
        var start = date.Date;
        var end = start.AddDays(1);

        return GetPeriodAsync(start, end, userId, ct);
    }

    public Task<List<CalendarSessionDto>> GetByWeekAsync(
        DateTime date,
        string userId,
        CancellationToken ct = default)
    {
        // Veckan börjar på måndag.
        var start = date.Date.AddDays(-((int)date.DayOfWeek + 6) % 7);
        var end = start.AddDays(7);

        return GetPeriodAsync(start, end, userId, ct);
    }

    public Task<List<CalendarSessionDto>> GetByMonthAsync(
        DateTime date,
        string userId,
        CancellationToken ct = default)
    {
        var start = new DateTime(date.Year, date.Month, 1);
        var end = start.AddMonths(1);

        return GetPeriodAsync(start, end, userId, ct);
    }

    private async Task<List<CalendarSessionDto>> GetPeriodAsync(
        DateTime start,
        DateTime end,
        string userId,
        CancellationToken ct)
    {
        var activitySessions = await sessions.GetByPeriodAsync(
            userId,
            start,
            end,
            ct);

        return activitySessions
            .Select(session => new CalendarSessionDto
            {
                Id = session.Id,
                CalendarId = session.CalendarId,
                CategoryName = session.Category.Name,
                ScheduledAt = session.ScheduledAt,
                PlannedDurationInMinutes = session.PlannedDurationInMinutes,
                Status = session.Status
            })
            .ToList();
    }
}
