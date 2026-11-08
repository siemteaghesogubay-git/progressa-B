using progressa.Dtos;

namespace progressa.Services.IServices;

public interface ICalendarService
{
    Task<int?> GetDefaultCalendarIdAsync(string userId);

    Task<List<CalendarSessionDto>> GetByDayAsync(
        DateTime date,
        string userId);

    Task<List<CalendarSessionDto>> GetByWeekAsync(
        DateTime date,
        string userId);

    Task<List<CalendarSessionDto>> GetByMonthAsync(
        DateTime date,
        string userId);

}