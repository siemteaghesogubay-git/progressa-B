using Microsoft.EntityFrameworkCore;
using progressa.Data;
using progressa.Dtos;
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

    public async Task<CategoryOptionDto> CreateCategoryAsync(
        string userId,
        CreateActivityCategoryDto dto,
        CancellationToken ct = default)
    {
        var name = dto.Name.Trim();
        var category = await db.Categories.FirstOrDefaultAsync(
            c => c.OwnerUserId == userId && c.Name == name,
            ct);

        if (category is null)
        {
            category = new Category
            {
                Name = name,
                OwnerUserId = userId
            };
            db.Categories.Add(category);
            await db.SaveChangesAsync(ct);
        }

        return new CategoryOptionDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }

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

    // Skapa ett planerat aktivitetspass
    public async Task<CreateSessionResult> CreatePlannedSessionAsync(
        string userId,
        CreatePlannedSessionDto dto,
        CancellationToken cancellationToken = default)
    {
        var category = await db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
               c.Id == dto.CategoryId
                && (c.OwnerUserId == userId), cancellationToken);
        if (category is null)
            return CreateSessionResult.CategoryNotFound();

        if (!await db.Calendars.AnyAsync(c =>
            c.Id == dto.CalendarId && c.OwnerUserId == userId,
            cancellationToken))
        {
            return CreateSessionResult.CalendarNotFound();
        }
        var session = new ActivitySession
        {
            OwnerUserId = userId,
            CalendarId = dto.CalendarId,
            CategoryId = category.Id,
            ScheduledAt = dto.ScheduledAt!.Value.UtcDateTime,
            PlannedDurationInMinutes = dto.PlannedDurationInMinutes,
            ActualDurationInMinutes = 0,
            Status = SessionStatus.Planned,
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

    // Markera ett planerat aktivitetspass som genomfört
    public async Task<bool> CompletePlannedSessionAsync(
        int sessionId,
        string userId,
        CompletePlannedSessionDto dto,
        CancellationToken ct = default)
    {
        var session = await db.ActivitySessions
            .FirstOrDefaultAsync(s =>
            s.Id == sessionId && s.OwnerUserId == userId, ct);

        if (session is null || session.Status != SessionStatus.Planned)
            return false;

        if (session.ScheduledAt > DateTime.UtcNow)
            return false;

        session.ActualDurationInMinutes = dto.ActualDurationInMinutes;
        session.CaloriesBurned = dto.CaloriesBurned;
        session.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();
        session.Status = SessionStatus.Completed;

        await db.SaveChangesAsync(ct);

        return true;

    }

    // Ta bort ett planerat aktivitetspass
    public async Task<bool> DeletePlannedSessionAsync(
        int sessionId,
        string userId,
        CancellationToken ct = default)
    {
        var deleteCount = await db.ActivitySessions
            .Where(s => s.Id == sessionId
                && s.OwnerUserId == userId
                && s.Status == SessionStatus.Planned)
            .ExecuteDeleteAsync(ct);

        return deleteCount > 0;
    }

    // Redigera ett planerat träningspass
    public async Task<UpdatePlannedSessionStatus> UpdatePlannedSessionAsync(
        int sessionId,
        string userId,
        UpdatePlannedSessionDto dto,
        CancellationToken ct = default)
    {
        var categoryExist = await db.Categories.AnyAsync(
            c => c.Id == dto.CategoryId && c.OwnerUserId == userId, ct);

        if (!categoryExist)
        {
            return UpdatePlannedSessionStatus.CategoryNotFound;
        }

        var scheduledAt = dto.ScheduledAt!.Value.UtcDateTime;
        var notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();

        var updateCount = await db.ActivitySessions
            .Where(s => s.Id == sessionId
                && s.OwnerUserId == userId
                && s.Status == SessionStatus.Planned)
            .ExecuteUpdateAsync(setters => setters
               .SetProperty(s => s.CategoryId, dto.CategoryId)
                .SetProperty(s => s.ScheduledAt, scheduledAt)
                .SetProperty(s => s.PlannedDurationInMinutes, dto.PlannedDurationInMinutes)
                .SetProperty(s => s.Notes, notes), ct);

        if (updateCount == 0)
            return UpdatePlannedSessionStatus.SessionNotFound;

        return UpdatePlannedSessionStatus.Success;
    }
}