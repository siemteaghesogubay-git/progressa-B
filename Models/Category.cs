using System.ComponentModel.DataAnnotations;

public class Category
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public bool IsPrivate { get; set; }

    public string OwnerUserId { get; set; } = string.Empty;
    public ApplicationUser OwnerUser { get; set; } = null!;

    public ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();
}