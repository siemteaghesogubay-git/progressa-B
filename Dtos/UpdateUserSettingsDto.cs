using System.ComponentModel.DataAnnotations;

namespace progressa.Dtos
{
    public class UpdateUserSettingsDto
    {
        [Required]
        [RegularExpression("^(km|mi)$", ErrorMessage = "Distance unit must be km or mi.")]
        public string DistanceUnit { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(kg|lb)$", ErrorMessage = "Weight unit must be kg or lb.")]
        public string WeightUnit { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(kcal|kj)$", ErrorMessage = "Energy unit must be kcal or kj.")]
        public string EnergyUnit { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(cm|ft)$", ErrorMessage = "Height unit must be cm or ft.")]
        public string HeightUnit { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(24h|12h)$", ErrorMessage = "Time format must be 24h or 12h.")]
        public string TimeFormat { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(mon|sun)$", ErrorMessage = "Week start must be mon or sun.")]
        public string WeekStart { get; set; } = string.Empty;
    }
}
