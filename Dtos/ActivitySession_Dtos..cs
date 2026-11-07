using System.ComponentModel.DataAnnotations;

namespace progressa.Dtos;

public static class ActivitySession_Dtos
{
    public class CreateActivitySessionDto : IValidatableObject
    {
        [Range(1, int.MaxValue, ErrorMessage = "Välj en kalender.")]
        public int CalendarId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Välj en aktivitet.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Datum krävs.")]
        public DateTimeOffset? ScheduledAt { get; set; }

        [Range(1, 1440, ErrorMessage = "Varaktigheten måste vara mellan 1 och 1440 minuter.")]
        public int DurationInMinutes { get; set; }

        // Frivilliga
        [Range(0, 20000, ErrorMessage = "Kalorier måste vara mellan 0 och 20 000.")]
        public int? CaloriesBurned { get; set; }

        [MaxLength(500, ErrorMessage = "Anteckningen får vara högst 500 tecken.")]
        public string? Notes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            // Ett genomfört pass kan inte ligga i framtiden (5 min marginal för klockskillnader)
            if (ScheduledAt is not null && ScheduledAt.Value > DateTimeOffset.UtcNow.AddMinutes(5))
                yield return new ValidationResult(
                    "Datumet får inte ligga i framtiden.",
                    [nameof(ScheduledAt)]);
        }
    }

    public class ActivitySessionDto
    {
        public int Id { get; set; }
        public int CalendarId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public int PlannedDurationInMinutes { get; set; }
        public int ActualDurationInMinutes { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? CaloriesBurned { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CategoryOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}