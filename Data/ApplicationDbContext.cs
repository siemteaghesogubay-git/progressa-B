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
        public DbSet<Calendar> Calendars => Set<Calendar>();
        public DbSet<ActivitySession> ActivitySessions => Set<ActivitySession>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<SleepLog> SleepLogs => Set<SleepLog>();
        public DbSet<NutritionLog> NutritionLogs => Set<NutritionLog>();
        public DbSet<UserSettings> UserSettings => Set<UserSettings>();

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

                e.HasOne(g => g.Calendar)
                    .WithMany(c => c.Goals)
                    .HasForeignKey(g => g.CalendarId)
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

            // Calendar
            b.Entity<Calendar>(e =>
            {
                e.HasOne(c => c.OwnerUser)
                    .WithMany(u => u.Calendars)
                    .HasForeignKey(c => c.OwnerUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ActivitySession
            b.Entity<ActivitySession>(e =>
            {
                e.HasOne(a => a.OwnerUser)
                    .WithMany(u => u.ActivitySessions)
                    .HasForeignKey(a => a.OwnerUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(a => a.Calendar)
                    .WithMany(c => c.ActivitySessions)
                    .HasForeignKey(a => a.CalendarId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(a => a.Category)
                    .WithMany(c => c.ActivitySessions)
                    .HasForeignKey(a => a.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Notification
            b.Entity<Notification>(e =>
            {
                e.HasOne(n => n.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(n => n.ActivitySession)
                    .WithMany()
                    .HasForeignKey(n => n.ActivitySessionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // SleepLog
            b.Entity<SleepLog>(e =>
            {
                e.HasOne(s => s.User)
                    .WithMany(u => u.SleepLogs)
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // NutritionLog
            b.Entity<NutritionLog>(e =>
            {
                e.HasOne(n => n.User)
                    .WithMany(u => u.NutritionLogs)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // UserSettings
            b.Entity<UserSettings>(e =>
            {
                e.HasKey(s => s.UserId);

                e.HasOne(us => us.User)
                    .WithOne(u => u.UserSettings)
                    .HasForeignKey<UserSettings>(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}