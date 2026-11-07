using System.ComponentModel.DataAnnotations;
using progressa.Models;

public class Category
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public string OwnerUserId { get; set; } = string.Empty;
    public ApplicationUser OwnerUser { get; set; } = null!;

    public ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();
    public ICollection<ActivitySession> ActivitySessions { get; set; } = new List<ActivitySession>();
}