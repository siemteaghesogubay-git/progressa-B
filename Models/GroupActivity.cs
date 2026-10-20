using System.ComponentModel.DataAnnotations;
using progressa.Models;
using Microsoft.AspNetCore.Identity;
using progressa.Data;

public class GroupActivity
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string OwnerUserId { get; set; } = string.Empty;
    public ApplicationUser OwnerUser { get; set; } = null!;

    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
}