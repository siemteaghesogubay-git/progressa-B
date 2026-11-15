namespace progressa.Dtos
{
    public class UserSettingsDto
    {
        public string DistanceUnit { get; set; } = string.Empty;
        public string WeightUnit { get; set; } = string.Empty;
        public string EnergyUnit { get; set; } = string.Empty;
        public string HeightUnit { get; set; } = string.Empty;
        public string TimeFormat { get; set; } = string.Empty;
        public string WeekStart { get; set; } = string.Empty;
    }
}
