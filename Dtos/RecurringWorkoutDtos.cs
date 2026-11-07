using System.ComponentModel.DataAnnotations;

namespace progressa.Dtos;

// Används både för att skapa och uppdatera ett veckoschema.
public class SaveRecurringWorkoutDto : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "Välj en kalender.")]
    public int CalendarId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Välj en aktivitet.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Välj en veckodag.")]
    [EnumDataType(typeof(DayOfWeek), ErrorMessage = "Ogiltig veckodag.")]
    public DayOfWeek? DayOfWeek { get; set; }

    [Required(ErrorMessage = "Ange ett klockslag.")]
    public TimeOnly? LocalTime { get; set; }

    [Required(ErrorMessage = "Ange en tidszon.")]
    [MaxLength(100)]
    public string TimeZoneId { get; set; } = "Europe/Stockholm";

    [Required(ErrorMessage = "Ange ett startdatum.")]
    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [Range(
        1,
        1440,
        ErrorMessage = "Träningspasset måste vara mellan 1 och 1440 minuter.")]
    public int DurationInMinutes { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (StartDate == DateOnly.MinValue)
        {
            yield return new ValidationResult(
                "Ange ett giltigt startdatum.",
                [nameof(StartDate)]);
        }

        if (StartDate.HasValue &&
            EndDate.HasValue &&
            EndDate.Value < StartDate.Value)
        {
            yield return new ValidationResult(
                "Slutdatum får inte vara före startdatum.",
                [nameof(EndDate)]);
        }

        if (!string.Equals(
                TimeZoneId,
                "Europe/Stockholm",
                StringComparison.Ordinal))
        {
            yield return new ValidationResult(
                "Tidszonen måste vara Europe/Stockholm.",
                [nameof(TimeZoneId)]);
        }
    }
}

// Returneras när användaren hämtar ett schema.
public record RecurringWorkoutDto(
    int Id,
    int CalendarId,
    int CategoryId,
    string CategoryName,
    DayOfWeek DayOfWeek,
    TimeOnly LocalTime,
    string TimeZoneId,
    DateOnly StartDate,
    DateOnly? EndDate,
    int DurationInMinutes);

// Ett beräknat träningstillfälle från veckoschemat.
public record RecurringWorkoutOccurrenceDto(
    int ScheduleId,
    int CalendarId,
    int CategoryId,
    string CategoryName,
    DateOnly LocalDate,
    TimeOnly LocalTime,
    DateTimeOffset ScheduledAtUtc,
    int DurationInMinutes);