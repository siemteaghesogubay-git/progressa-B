namespace progressa.Models

{
    public class GroupMember
    {
        public int Id { get; set; }

        public string OwnerUserId { get; set; } = string.Empty;
        public ApplicationUser OwnerUser { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public string RoleId { get; set; } = string.Empty;
        public ApplicationRole Role { get; set; } = null!;

        public int GroupActivityId { get; set; }
        public GroupActivity GroupActivity { get; set; } = null!;

    }
}