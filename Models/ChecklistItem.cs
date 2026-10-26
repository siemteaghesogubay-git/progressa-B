using System.ComponentModel.DataAnnotations;

public class ChecklistItem
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Text { get; set; } = string.Empty;

    public bool IsDone { get; set; }

    public int ChecklistId { get; set; }
    public Checklist Checklist { get; set; } = null!;
}
