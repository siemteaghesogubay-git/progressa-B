using System.ComponentModel.DataAnnotations;

namespace progressa.Models
{
    public class NutritionLog
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public DateOnly Date { get; set; }

        public decimal? ReachedCalories { get; set; }
        public decimal? ReachedProtein { get; set; }
        public decimal? ReachedCarbs { get; set; }
        public decimal? ReachedFat { get; set; }

        public bool? CaloriesGoalReached { get; set; }
        public bool? ProteinGoalReached { get; set; }
        public bool? CarbsGoalReached { get; set; }
        public bool? FatGoalReached { get; set; }

        [ConcurrencyCheck]
        public Guid Version { get; set; }
    }
}
