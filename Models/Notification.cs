namespace progressa.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int ActivitySessionId { get; set; }
        public ActivitySession ActivitySession { get; set; } = null!;

        public DateTime ScheduledFor { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
