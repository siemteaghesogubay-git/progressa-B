namespace progressa.Models;

public class RecurringWorkoutSchedule
{
    public int Id { get; set; }

    // Användaren som äger schemat.
    public string OwnerUserId { get; set; } = string.Empty;
    public ApplicationUser OwnerUser { get; set; } = null!;

    // Kalendern som schemat tillhör.
    public int CalendarId { get; set; }
    public Calendar Calendar { get; set; } = null!;

    // Aktiviteten, exempelvis löpning eller styrketräning.
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    // Veckodagen då passet återkommer.
    public DayOfWeek DayOfWeek { get; set; }

    // Lokal klocktid, exempelvis 18:00.
    public TimeOnly LocalTime { get; set; }

    // Tidszon för att hantera sommar- och vintertid.
    public string TimeZoneId { get; set; } = "Europe/Stockholm";

    // Första datum då schemat gäller.
    public DateOnly StartDate { get; set; }

    // Valfritt slutdatum. Null betyder att schemat fortsätter.
    public DateOnly? EndDate { get; set; }

    // Planerad längd på varje träningspass.
    public int DurationInMinutes { get; set; }

}