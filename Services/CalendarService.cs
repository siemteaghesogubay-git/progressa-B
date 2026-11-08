using Microsoft.EntityFrameworkCore;
using progressa.Data;
using progressa.Dtos;
using progressa.Services.IServices;

namespace progressa.Services;

public class CalendarService(ApplicationDbContext context)
    : ICalendarService
{
    private static readonly TimeZoneInfo CalendarTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");

    public async Task<int?> GetDefaultCalendarIdAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        return await context.Calendars
            .AsNoTracking()
            .Where(calendar => calendar.OwnerUserId == userId)
            .OrderBy(calendar => calendar.Id)
            .Select(calendar => (int?)calendar.Id)
            .FirstOrDefaultAsync();
    }

    public Task<List<CalendarSessionDto>> GetByDayAsync(
        DateTime date,
        string userId)
    {
        ValidateDate(date);

        var localStart = AsLocalDate(date);
        var localEnd = localStart.AddDays(1);

        return GetByLocalPeriodAsync(
            localStart,
            localEnd,
            userId);
    }

    public Task<List<CalendarSessionDto>> GetByWeekAsync(
        DateTime date,
        string userId)
    {
        ValidateDate(date);

        var localDate = AsLocalDate(date);

        // Veckan börjar på måndag.
        var daysSinceMonday =
            ((int)localDate.DayOfWeek + 6) % 7;

        var localStart = localDate.AddDays(-daysSinceMonday);
        var localEnd = localStart.AddDays(7);

        return GetByLocalPeriodAsync(
            localStart,
            localEnd,
            userId);
    }

    public Task<List<CalendarSessionDto>> GetByMonthAsync(
        DateTime date,
        string userId)
    {
        ValidateDate(date);

        var localStart = new DateTime(
            date.Year,
            date.Month,
            1,
            0,
            0,
            0,
            DateTimeKind.Unspecified);

        var localEnd = localStart.AddMonths(1);

        return GetByLocalPeriodAsync(
            localStart,
            localEnd,
            userId);
    }

    private async Task<List<CalendarSessionDto>> GetByLocalPeriodAsync(
        DateTime localStart,
        DateTime localEnd,
        string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        // Omvandla båda gränserna separat så att även
        // dygn med 23 eller 25 timmar hanteras korrekt.
        var startUtc = TimeZoneInfo.ConvertTimeToUtc(
            localStart,
            CalendarTimeZone);

        var endUtc = TimeZoneInfo.ConvertTimeToUtc(
            localEnd,
            CalendarTimeZone);

        return await context.ActivitySessions
            .AsNoTracking()
            .Where(session =>
                session.OwnerUserId == userId &&
                session.ScheduledAt >= startUtc &&
                session.ScheduledAt < endUtc)
            .OrderBy(session => session.ScheduledAt)
            .ThenBy(session => session.Id)
            .Select(session => new CalendarSessionDto
            {
                Id = session.Id,
                CalendarId = session.CalendarId,
                CategoryName = session.Category.Name,
                ScheduledAt = session.ScheduledAt,
                PlannedDurationInMinutes =
                    session.PlannedDurationInMinutes,
                Status = session.Status
            })
            .ToListAsync();
    }

    private static DateTime AsLocalDate(DateTime date)
    {
        // Datumet representerar en svensk kalenderdag.
        return DateTime.SpecifyKind(
            date.Date,
            DateTimeKind.Unspecified);
    }

    private static void ValidateDate(DateTime date)
    {
        if (date.Year < 1900 || date.Year > 9998)
        {
            throw new ArgumentException(
                "Kalenderdatum måste ligga mellan år 1900 och 9998.",
                nameof(date));
        }
    }
}