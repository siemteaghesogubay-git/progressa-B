namespace progressa.Models
{
    public class UserSettings
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public bool NotificationsOn { get; set; } = true;

        public string DistanceUnit { get; set; } = "km";
        public string WeightUnit { get; set; } = "kg";
        public string EnergyUnit { get; set; } = "kcal";
        public string HeightUnit { get; set; } = "cm";
        public string TimeFormat { get; set; } = "24h";
        public string WeekStart { get; set; } = "mon";
    }
}
