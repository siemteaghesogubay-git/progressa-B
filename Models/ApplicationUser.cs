using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using progressa.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(15)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(15)]
    public string LastName { get; set; } = string.Empty;

    public int Age { get; set; }
    public bool IsPrivate { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Goal> Goals { get; set; } = new List<Goal>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<GroupActivity> OwnedGroups { get; set; } = new List<GroupActivity>();
    public ICollection<GroupMember> Memberships { get; set; } = new List<GroupMember>();
    public ICollection<Challenge> OwnedChallenges { get; set; } = new List<Challenge>();
    public ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();
}