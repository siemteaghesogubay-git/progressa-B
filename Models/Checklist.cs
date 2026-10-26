using System.ComponentModel.DataAnnotations;

public class Checklist
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public string OwnerUserId { get; set; } = string.Empty;
    public ApplicationUser OwnerUser { get; set; } = null!;

    public int? TrainingSessionId { get; set; }
    public TrainingSession? TrainingSession { get; set; }

    public ICollection<ChecklistItem> Items { get; set; } = new List<ChecklistItem>();
}
