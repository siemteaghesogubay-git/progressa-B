using System.ComponentModel.DataAnnotations;

namespace progressa.Models
{
    public static class SessionStatus
    {
        public const string Planned = "Planned";
        public const string Completed = "Completed";
    }

    public class ActivitySession
    {
        public int Id { get; set; }

        public string OwnerUserId { get; set; } = string.Empty;
        public ApplicationUser OwnerUser { get; set; } = null!;

        public int CalendarId { get; set; }
        public Calendar Calendar { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public DateTime ScheduledAt { get; set; }

        [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
        public int PlannedDurationInMinutes { get; set; }

        [Range(0, 1440, ErrorMessage = "Actual duration must be between 0 and 1440 minutes.")]
        public int ActualDurationInMinutes { get; set; }

        [Required, MaxLength(30)]
        public string Status { get; set; } = SessionStatus.Planned;

        // Frivilliga uppgifter
        public int? CaloriesBurned { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}