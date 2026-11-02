using progressa.Data;
using progressa.Dtos;
using progressa.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace progressa.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly ApplicationDbContext _context;

        public CalendarService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hämta användarens aktivitetspass för en specifik dag
        public async Task<List<CalendarSessionDto>> GetByDayAsync(DateTime date, string userId)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            return await _context.ActivitySessions
                .Where(a => a.OwnerUserId == userId && a.ScheduledAt >= start && a.ScheduledAt < end)
                .Select(a => new CalendarSessionDto
                {
                    Id = a.Id,
                    CalendarId = a.CalendarId,
                    CategoryName = a.Category.Name,
                    ScheduledAt = a.ScheduledAt,
                    PlannedDurationInMinutes = a.PlannedDurationInMinutes,
                    Status = a.Status
                })
                .ToListAsync();
        }

        // Hämta användarens aktivitetspass för en specifik vecka
        public async Task<List<CalendarSessionDto>> GetByWeekAsync(DateTime date, string userId)
        {
            var start = date.Date.AddDays(-((int)date.DayOfWeek + 6) % 7);
            var end = start.AddDays(7);

            return await _context.ActivitySessions
                .Where(a => a.OwnerUserId == userId && a.ScheduledAt >= start && a.ScheduledAt < end)
                .OrderBy(a => a.ScheduledAt)
                .Select(a => new CalendarSessionDto
                {
                    Id = a.Id,
                    CalendarId = a.CalendarId,
                    CategoryName = a.Category.Name,
                    ScheduledAt = a.ScheduledAt,
                    PlannedDurationInMinutes = a.PlannedDurationInMinutes,
                    Status = a.Status
                })
                .ToListAsync();
        }

        // Hämta användarens aktivitetspass för en specifik månad
        public async Task<List<CalendarSessionDto>> GetByMonthAsync(DateTime date, string userId)
        {
            var start = new DateTime(date.Year, date.Month, 1);
            var end = start.AddMonths(1);

            return await _context.ActivitySessions
                .Where(a => a.OwnerUserId == userId && a.ScheduledAt >= start && a.ScheduledAt < end)
                .OrderBy(a => a.ScheduledAt)
                .Select(a => new CalendarSessionDto
                {
                    Id = a.Id,
                    CalendarId = a.CalendarId,
                    CategoryName = a.Category.Name,
                    ScheduledAt = a.ScheduledAt,
                    PlannedDurationInMinutes = a.PlannedDurationInMinutes,
                    Status = a.Status
                })
                .ToListAsync();
        }
    }
}
