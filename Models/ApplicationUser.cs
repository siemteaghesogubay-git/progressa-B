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
    public ICollection<Challenge> OwnedChallenges { get; set; } = new List<Challenge>();
    public ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();
    public ICollection<Calendar> Calendars { get; set; } = new List<Calendar>();
    public ICollection<ActivitySession> ActivitySessions { get; set; } = new List<ActivitySession>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<SleepLog> SleepLogs { get; set; } = new List<SleepLog>();
    public ICollection<NutritionLog> NutritionLogs { get; set; } = new List<NutritionLog>();
    public UserSettings? UserSettings { get; set; }
}