using System.ComponentModel.DataAnnotations;

namespace progressa.Dtos
{
    public class CreatePlannedSessionDto : IValidatableObject
    {
        [Range(1, int.MaxValue, ErrorMessage = "Välj en kalender.")]
        public int CalendarId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Välj en kategori.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Välj ett datum och tid.")]
        public DateTimeOffset? ScheduledAt { get; set; }

        [Range(1, 1440, ErrorMessage = "Ange mellan 1 och 1440 minuter.")]
        public int PlannedDurationInMinutes { get; set; }

        [MaxLength(500, ErrorMessage = "Anteckningen får vara högst 500 tecken.")]
        public string? Notes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (ScheduledAt.HasValue &&
                ScheduledAt.Value <= DateTimeOffset.UtcNow)
            {
                yield return new ValidationResult(
                    "Datum och tid måste ligga i framtiden.",
                    [nameof(ScheduledAt)]);
            }
        }
    }
}