using Microsoft.EntityFrameworkCore;
using progressa.Data;
using progressa.Models;
using progressa.Services.IServices;
using static progressa.Dtos.ActivitySession_Dtos;

namespace progressa.Services;

public class ActivitySessionService(ApplicationDbContext db) : IActivitySessionService
{
    public async Task<IReadOnlyList<CategoryOptionDto>> GetCategoriesAsync(string userId, CancellationToken cancellationToken = default) =>
        await db.Categories
            .AsNoTracking()
            .Where(c => c.OwnerUserId == userId)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryOptionDto { Id = c.Id, Name = c.Name })
            .ToListAsync(cancellationToken);

    public async Task<CreateSessionResult> CreateAsync(string userId, CreateActivitySessionDto dto, CancellationToken cancellationToken = default)
    {
        // Aktiviteten måste finnas och vara tillgänglig för användaren
        var category = await db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == dto.CategoryId
                                      && (c.OwnerUserId == userId), cancellationToken);
        if (category is null)
            return CreateSessionResult.CategoryNotFound();

        // Kontrollerar att kalendern finns och tillhör användaren
        if (!await db.Calendars.AnyAsync(c => c.Id == dto.CalendarId && c.OwnerUserId == userId, cancellationToken))
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
        await db.SaveChangesAsync(cancellationToken);

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