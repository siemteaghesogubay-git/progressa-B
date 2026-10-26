using System.ComponentModel.DataAnnotations;

namespace progressa.Models
{
    public class SleepLog
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public DateTime Date { get; set; }

        [Range(0, 24, ErrorMessage = "Sleep hours must be between 0 and 24.")]
        public double SleepHours { get; set; }
        public DateTime BedTime { get; set; }
        public DateTime WakeTime { get; set; }
    }
}
