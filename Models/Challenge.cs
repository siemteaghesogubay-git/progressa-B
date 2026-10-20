using System.ComponentModel.DataAnnotations;

public class Challenge
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string UserOwnerId { get; set; } = string.Empty;
    public ApplicationUser UserOwner { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int GoalId { get; set; }
    public Goal Goal { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}