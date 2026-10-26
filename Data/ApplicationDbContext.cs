using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using progressa.Models;

namespace progressa.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
    {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Goal> Goals => Set<Goal>();
        public DbSet<Challenge> Challenges => Set<Challenge>();
        public DbSet<GroupActivity> GroupActivities => Set<GroupActivity>();
        public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
        public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();
        public DbSet<Checklist> Checklists => Set<Checklist>();
        public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b); // viktigt för Identity-tabellerna

            // ApplicationUser
            b.Entity<ApplicationUser>(e =>
            {
                e.Property(u => u.FirstName).HasMaxLength(15).IsRequired();
                e.Property(u => u.LastName).HasMaxLength(15).IsRequired();
            });

            // Category
            b.Entity<Category>(e =>
            {
                e.Property(c => c.Name).HasMaxLength(50).IsRequired();
                e.HasOne(c => c.OwnerUser)
                    .WithMany(u => u.Categories)
                    .HasForeignKey(c => c.OwnerUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Goal
            b.Entity<Goal>(e =>
            {
                e.Property(g => g.Title).HasMaxLength(100).IsRequired();
                e.HasOne(g => g.OwnerUser)
                    .WithMany(u => u.Goals)
                    .HasForeignKey(g => g.OwnerUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Challenge (två FK till användare, därför Restrict)
            b.Entity<Challenge>(e =>
            {
                e.Property(c => c.Title).HasMaxLength(100).IsRequired();

                e.HasOne(c => c.UserOwner)
                    .WithMany(u => u.OwnedChallenges)
                    .HasForeignKey(c => c.UserOwnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(c => c.User)
                    .WithMany(u => u.Challenges)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(c => c.Goal)
                    .WithMany(g => g.Challenges)
                    .HasForeignKey(c => c.GoalId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(c => c.Category)
                    .WithMany(cat => cat.Challenges)
                    .HasForeignKey(c => c.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // GroupActivity
            b.Entity<GroupActivity>(e =>
            {
                e.Property(g => g.Name).HasMaxLength(100).IsRequired();
                e.HasOne(g => g.OwnerUser)
                    .WithMany(u => u.OwnedGroups)
                    .HasForeignKey(g => g.OwnerUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // GroupMember
            b.Entity<GroupMember>(e =>
            {
                e.HasOne(m => m.OwnerUser)
                    .WithMany()
                    .HasForeignKey(m => m.OwnerUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(m => m.User)
                    .WithMany(u => u.Memberships)
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(m => m.Role)
                    .WithMany()
                    .HasForeignKey(m => m.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(m => m.GroupActivity)
                    .WithMany(g => g.Members)
                    .HasForeignKey(m => m.GroupActivityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // TrainingSession
            b.Entity<TrainingSession>(e =>
            {
                e.Property(s => s.Title).HasMaxLength(100).IsRequired();
                e.Property(s => s.Notes).HasMaxLength(500);

                e.HasOne(s => s.OwnerUser)
                    .WithMany(u => u.TrainingSessions)
                    .HasForeignKey(s => s.OwnerUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Checklist
            b.Entity<Checklist>(e =>
            {
                e.Property(c => c.Title).HasMaxLength(100).IsRequired();

                e.HasOne(c => c.OwnerUser)
                    .WithMany(u => u.Checklists)
                    .HasForeignKey(c => c.OwnerUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(c => c.TrainingSession)
                    .WithMany(s => s.Checklists)
                    .HasForeignKey(c => c.TrainingSessionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ChecklistItem
            b.Entity<ChecklistItem>(e =>
            {
                e.Property(i => i.Text).HasMaxLength(200).IsRequired();

                e.HasOne(i => i.Checklist)
                    .WithMany(c => c.Items)
                    .HasForeignKey(i => i.ChecklistId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}