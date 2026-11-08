using System.ComponentModel.DataAnnotations;
using progressa.Dtos;
using progressa.Models;
using progressa.Repositories.interfaces;
using progressa.Services.IServices;

namespace progressa.Services;

public class RecurringWorkoutService(
    IRecurringWorkoutRepository repository)
    : IRecurringWorkoutService
{
    public async Task<List<RecurringWorkoutDto>> GetAllAsync(
        string userId,
        CancellationToken ct = default)
    {
        var schedules = await repository.GetAllAsync(userId, ct);

        return schedules.Select(MapToDto).ToList();
    }

    public async Task<RecurringWorkoutDto?> GetAsync(
        int id,
        string userId,
        CancellationToken ct = default)
    {
        var schedule = await repository.GetAsync(id, userId, ct);

        return schedule is null ? null : MapToDto(schedule);
    }

    public async Task<ScheduleWriteResult> SaveAsync(
        int? id,
        string userId,
        SaveRecurringWorkoutDto dto,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        Validator.ValidateObject(
            dto,
            new ValidationContext(dto),
            validateAllProperties: true);

        RecurringWorkoutSchedule schedule;

        if (id.HasValue)
        {
            var existing = await repository.GetAsync(
                id.Value,
                userId,
                ct);

            if (existing is null)
            {
                return new(ScheduleWriteStatus.NotFound);
            }

            schedule = existing;
        }
        else
        {
            schedule = new RecurringWorkoutSchedule
            {
                OwnerUserId = userId
            };
        }

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(
            dto.TimeZoneId);

        var localNow = TimeZoneInfo.ConvertTime(
            DateTimeOffset.UtcNow,
            timeZone);

        var today = DateOnly.FromDateTime(localNow.DateTime);
        var requestedStartDate = dto.StartDate!.Value;

        var keepsExistingStartDate =
            id.HasValue &&
            requestedStartDate == schedule.StartDate;

        if (requestedStartDate < today && !keepsExistingStartDate)
        {
            return new(ScheduleWriteStatus.InvalidStartDate);
        }

        var ownsReferences = await repository.OwnsReferencesAsync(
            userId,
            dto.CalendarId,
            dto.CategoryId,
            ct);

        if (!ownsReferences)
        {
            return new(ScheduleWriteStatus.InvalidReferences);
        }

        schedule.CalendarId = dto.CalendarId;
        schedule.CategoryId = dto.CategoryId;
        schedule.DayOfWeek = dto.DayOfWeek!.Value;
        schedule.LocalTime = dto.LocalTime!.Value;
        schedule.TimeZoneId = dto.TimeZoneId;
        schedule.StartDate = requestedStartDate;
        schedule.EndDate = dto.EndDate;
        schedule.DurationInMinutes = dto.DurationInMinutes;

        if (!id.HasValue)
        {
            repository.Add(schedule);
        }

        await repository.SaveAsync(ct);

        return new(ScheduleWriteStatus.Success, schedule.Id);
    }

    public async Task<bool> DeleteAsync(
        int id,
        string userId,
        CancellationToken ct = default)
    {
        var schedule = await repository.GetAsync(id, userId, ct);

        if (schedule is null)
        {
            return false;
        }

        repository.Remove(schedule);
        await repository.SaveAsync(ct);

        return true;
    }

    // Beräknar tillfällen utan att skapa ActivitySession-poster.
    // from inkluderas och to inkluderas inte.
    public async Task<List<RecurringWorkoutOccurrenceDto>>
        GetOccurrencesAsync(
            string userId,
            DateOnly from,
            DateOnly to,
            CancellationToken ct = default)
    {
        var numberOfDays = to.DayNumber - from.DayNumber;

        if (numberOfDays < 1 || numberOfDays > 366)
        {
            throw new ArgumentException(
                "Datumintervallet måste vara mellan 1 och 366 dagar.");
        }

        var schedules = await repository.GetAllAsync(userId, ct);
        var occurrences = new List<RecurringWorkoutOccurrenceDto>();

        foreach (var schedule in schedules)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(
                schedule.TimeZoneId);

            for (
                var dayNumber = from.DayNumber;
                dayNumber < to.DayNumber;
                dayNumber++)
            {
                ct.ThrowIfCancellationRequested();

                var date = DateOnly.FromDayNumber(dayNumber);

                if (date < schedule.StartDate)
                {
                    continue;
                }

                if (schedule.EndDate.HasValue &&
                    date > schedule.EndDate.Value)
                {
                    continue;
                }

                if (date.DayOfWeek != schedule.DayOfWeek)
                {
                    continue;
                }

                var localDateTime = date.ToDateTime(
                    schedule.LocalTime,
                    DateTimeKind.Unspecified);

                // Klockslag som inte finns vid vårens omställning
                // ger inget tillfälle.
                if (timeZone.IsInvalidTime(localDateTime))
                {
                    continue;
                }

                DateTimeOffset scheduledAtUtc;

                if (timeZone.IsAmbiguousTime(localDateTime))
                {
                    // Välj den senare förekomsten vid höstens omställning.
                    var offset = timeZone
                        .GetAmbiguousTimeOffsets(localDateTime)
                        .Min();

                    scheduledAtUtc = new DateTimeOffset(
                        localDateTime,
                        offset).ToUniversalTime();
                }
                else
                {
                    var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(
                        localDateTime,
                        timeZone);

                    scheduledAtUtc = new DateTimeOffset(utcDateTime);
                }

                occurrences.Add(new RecurringWorkoutOccurrenceDto(
                    ScheduleId: schedule.Id,
                    CalendarId: schedule.CalendarId,
                    CategoryId: schedule.CategoryId,
                    CategoryName: schedule.Category.Name,
                    LocalDate: date,
                    LocalTime: schedule.LocalTime,
                    ScheduledAtUtc: scheduledAtUtc,
                    DurationInMinutes: schedule.DurationInMinutes));
            }
        }

        return occurrences
            .OrderBy(occurrence => occurrence.ScheduledAtUtc)
            .ThenBy(occurrence => occurrence.ScheduleId)
            .ToList();
    }

    private static RecurringWorkoutDto MapToDto(
        RecurringWorkoutSchedule schedule)
    {
        return new RecurringWorkoutDto(
            Id: schedule.Id,
            CalendarId: schedule.CalendarId,
            CategoryId: schedule.CategoryId,
            CategoryName: schedule.Category.Name,
            DayOfWeek: schedule.DayOfWeek,
            LocalTime: schedule.LocalTime,
            TimeZoneId: schedule.TimeZoneId,
            StartDate: schedule.StartDate,
            EndDate: schedule.EndDate,
            DurationInMinutes: schedule.DurationInMinutes);
    }
}