using System.ComponentModel.DataAnnotations;

namespace progressa.Dtos
{
    public class UpdateGoalDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "Målvärdet måste vara större än 0.")]
        public int Target { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsCompleted { get; set; }
        public int? CalendarId { get; set; }
    }
}
