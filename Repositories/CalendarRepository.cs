using Microsoft.EntityFrameworkCore;
using progressa.Data;
using progressa.Repositories.interfaces;

namespace progressa.Repositories;

public class CalendarRepository(ApplicationDbContext db)
    : ICalendarRepository
{
    public Task<bool> ExistsAsync(
        int id,
        string userId,
        CancellationToken ct = default)
    {
        return db.Calendars
            .AnyAsync(
                c => c.Id == id && c.OwnerUserId == userId,
                ct);
    }
}