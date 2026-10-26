using System.ComponentModel.DataAnnotations;

public class TrainingSession
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public DateTime PerformedAt { get; set; }
    public int DurationMinutes { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public string OwnerUserId { get; set; } = string.Empty;
    public ApplicationUser OwnerUser { get; set; } = null!;

    public ICollection<Checklist> Checklists { get; set; } = new List<Checklist>();
}
