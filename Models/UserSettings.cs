namespace progressa.Models
{
    public class UserSettings
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public bool NotificationsOn { get; set; } = true;
    }
}
