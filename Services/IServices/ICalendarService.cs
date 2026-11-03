using progressa.Dtos;

namespace progressa.Services.IServices;

public interface ICalendarService
{
    Task<List<CalendarSessionDto>> GetByDayAsync(
        DateTime date,
        string userId,
        CancellationToken ct = default);

    Task<List<CalendarSessionDto>> GetByWeekAsync(
        DateTime date,
        string userId,
        CancellationToken ct = default);

    Task<List<CalendarSessionDto>> GetByMonthAsync(
        DateTime date,
        string userId,
        CancellationToken ct = default);
}