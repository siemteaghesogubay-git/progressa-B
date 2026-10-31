using Microsoft.EntityFrameworkCore;
using progressa.Data;
using progressa.Models;
using progressa.Services.IServices;
using static progressa.Dtos.ActivitySession_Dtos;

namespace progressa.Services;

public class ActivitySessionService(ApplicationDbContext db) : IActivitySessionService
{
    public async Task<IReadOnlyList<CategoryOptionDto>> GetCategoriesAsync(string userId) =>
        await db.Categories
            .AsNoTracking()
            .Where(c => c.OwnerUserId == userId || !c.IsPrivate)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryOptionDto { Id = c.Id, Name = c.Name })
            .ToListAsync();

    public async Task<CreateSessionResult> CreateAsync(string userId, CreateActivitySessionDto dto)
    {
        // Aktiviteten måste finnas och vara tillgänglig för användaren
        var category = await db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == dto.CategoryId
                                      && (c.OwnerUserId == userId || !c.IsPrivate));
        if (category is null)
            return CreateSessionResult.CategoryNotFound();

        // OBS: kontrollerar bara att kalendern finns, se anmärkningen nedan
        if (!await db.Calendars.AnyAsync(c => c.Id == dto.CalendarId))
            return CreateSessionResult.CalendarNotFound();

        var session = new ActivitySession
        {
            OwnerUserId = userId,
            CalendarId = dto.CalendarId,
            CategoryId = category.Id,
            ScheduledAt = dto.ScheduledAt!.Value.UtcDateTime,
            // Ett registrerat pass är redan genomfört, så planerad och faktisk tid är lika
            PlannedDurationInMinutes = dto.DurationInMinutes,
            ActualDurationInMinutes = dto.DurationInMinutes,
            Status = SessionStatus.Completed,
            CaloriesBurned = dto.CaloriesBurned,
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim()
        };

        db.ActivitySessions.Add(session);
        await db.SaveChangesAsync();

        return CreateSessionResult.Ok(new ActivitySessionDto
        {
            Id = session.Id,
            CalendarId = session.CalendarId,
            CategoryId = category.Id,
            CategoryName = category.Name,
            ScheduledAt = session.ScheduledAt,
            PlannedDurationInMinutes = session.PlannedDurationInMinutes,
            ActualDurationInMinutes = session.ActualDurationInMinutes,
            Status = session.Status,
            CaloriesBurned = session.CaloriesBurned,
            Notes = session.Notes,
            CreatedAt = session.CreatedAt
        });
    }
}