namespace progressa.Models
{
    public class Calendar
    {
        public int Id { get; set; }
        public string OwnerUserId { get; set; } = string.Empty;
        public ApplicationUser OwnerUser { get; set; } = null!;

        public ICollection<Goal> Goals { get; set; } = new List<Goal>();

        public ICollection<ActivitySession> ActivitySessions { get; set; } = new List<ActivitySession>();
    }
}
