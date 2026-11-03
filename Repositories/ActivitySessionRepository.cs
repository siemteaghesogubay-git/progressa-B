using Microsoft.EntityFrameworkCore;
using progressa.Data;
using progressa.Models;
using progressa.Repositories.interfaces;

namespace progressa.Repositories;

public class ActivitySessionRepository(ApplicationDbContext db)
    : IActivitySessionRepository
{
    public Task<List<ActivitySession>> GetByPeriodAsync(
        string userId,
        DateTime start,
        DateTime end,
        CancellationToken ct = default)
    {
        return db.ActivitySessions
            .AsNoTracking()
            .Include(a => a.Category)
            .Where(a =>
                a.OwnerUserId == userId &&
                a.ScheduledAt >= start &&
                a.ScheduledAt < end)
            .OrderBy(a => a.ScheduledAt)
            .ThenBy(a => a.Id)
            .ToListAsync(ct);
    }

    public async Task AddAsync(
        ActivitySession session,
        CancellationToken ct = default)
    {
        db.ActivitySessions.Add(session);
        await db.SaveChangesAsync(ct);
    }
}