using Microsoft.EntityFrameworkCore;
using progressa.Data;
using progressa.Models;
using progressa.Repositories.interfaces;

namespace progressa.Repositories;

public class RecurringWorkoutRepository(ApplicationDbContext db)
    : IRecurringWorkoutRepository
{
    // Hämta användarens scheman för läsning.
    public async Task<List<RecurringWorkoutSchedule>> GetAllAsync(
        string userId,
        CancellationToken ct = default)
    {
        return await db.RecurringWorkoutSchedules
            .AsNoTracking()
            .Include(schedule => schedule.Category)
            .Where(schedule => schedule.OwnerUserId == userId)
            .OrderBy(schedule => schedule.DayOfWeek)
            .ThenBy(schedule => schedule.LocalTime)
            .ThenBy(schedule => schedule.Id)
            .ToListAsync(ct);
    }

    // Spårad hämtning så att schemat också kan ändras eller raderas.
    public async Task<RecurringWorkoutSchedule?> GetAsync(
        int id,
        string userId,
        CancellationToken ct = default)
    {
        return await db.RecurringWorkoutSchedules
            .Include(schedule => schedule.Category)
            .SingleOrDefaultAsync(
                schedule =>
                    schedule.Id == id &&
                    schedule.OwnerUserId == userId,
                ct);
    }

    // Kontrollera att både kalender och aktivitet tillhör användaren.
    public async Task<bool> OwnsReferencesAsync(
        string userId,
        int calendarId,
        int categoryId,
        CancellationToken ct = default)
    {
        var ownsCalendar = await db.Calendars
            .AnyAsync(
                calendar =>
                    calendar.Id == calendarId &&
                    calendar.OwnerUserId == userId,
                ct);

        if (!ownsCalendar)
        {
            return false;
        }

        return await db.Categories
            .AnyAsync(
                category =>
                    category.Id == categoryId &&
                    category.OwnerUserId == userId,
                ct);
    }

    public void Add(RecurringWorkoutSchedule schedule)
    {
        db.RecurringWorkoutSchedules.Add(schedule);
    }

    public void Remove(RecurringWorkoutSchedule schedule)
    {
        db.RecurringWorkoutSchedules.Remove(schedule);
    }

    public async Task SaveAsync(CancellationToken ct = default)
    {
        await db.SaveChangesAsync(ct);
    }
}