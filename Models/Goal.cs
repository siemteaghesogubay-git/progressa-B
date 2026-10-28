using System.ComponentModel.DataAnnotations;
using progressa.Models;

public class Goal
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public int Target { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCompleted { get; set; }

    public string OwnerUserId { get; set; } = string.Empty;
    public ApplicationUser OwnerUser { get; set; } = null!;

    public int? CalendarId { get; set; }
    public Calendar? Calendar { get; set; }

    public ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();
}