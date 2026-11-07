using System.ComponentModel.DataAnnotations;

namespace progressa.Models
{
    public class NutritionLog
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public DateTime Date { get; set; }

        [Range(0, 10000, ErrorMessage = "Calories must be between 0 and 10,000.")]
        public int ReachedCalories { get; set; }

        [Range(0, 1000, ErrorMessage = "Protein must be between 0 and 1000.")]
        public double ReachedProtein { get; set; }

        [Range(0, 1000, ErrorMessage = "Carbs must be between 0 and 1000.")]
        public double ReachedCarbs { get; set; }

        [Range(0, 1000, ErrorMessage = "Fat must be between 0 and 1000.")]
        public double ReachedFat { get; set; }
    }
}
