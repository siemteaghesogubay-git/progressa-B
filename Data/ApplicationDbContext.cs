using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using progressa.Models;

namespace progressa.Data;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
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

    public DbSet<RecurringWorkoutSchedule> RecurringWorkoutSchedules
        => Set<RecurringWorkoutSchedule>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Användare
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.FirstName)
                .HasMaxLength(15)
                .IsRequired();

            entity.Property(user => user.LastName)
                .HasMaxLength(15)
                .IsRequired();
        });

        // Aktiviteter/kategorier
        builder.Entity<Category>(entity =>
        {
            entity.Property(category => category.Name)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasOne(category => category.OwnerUser)
                .WithMany(user => user.Categories)
                .HasForeignKey(category => category.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Mål
        builder.Entity<Goal>(entity =>
        {
            entity.Property(goal => goal.Title)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasOne(goal => goal.OwnerUser)
                .WithMany(user => user.Goals)
                .HasForeignKey(goal => goal.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(goal => goal.Calendar)
                .WithMany(calendar => calendar.Goals)
                .HasForeignKey(goal => goal.CalendarId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Utmaningar
        builder.Entity<Challenge>(entity =>
        {
            entity.Property(challenge => challenge.Title)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasOne(challenge => challenge.UserOwner)
                .WithMany(user => user.OwnedChallenges)
                .HasForeignKey(challenge => challenge.UserOwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(challenge => challenge.User)
                .WithMany(user => user.Challenges)
                .HasForeignKey(challenge => challenge.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(challenge => challenge.Goal)
                .WithMany(goal => goal.Challenges)
                .HasForeignKey(challenge => challenge.GoalId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(challenge => challenge.Category)
                .WithMany(category => category.Challenges)
                .HasForeignKey(challenge => challenge.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Kalendrar
        builder.Entity<Calendar>(entity =>
        {
            entity.HasOne(calendar => calendar.OwnerUser)
                .WithMany(user => user.Calendars)
                .HasForeignKey(calendar => calendar.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Enskilda aktivitetspass
        builder.Entity<ActivitySession>(entity =>
        {
            entity.HasOne(session => session.OwnerUser)
                .WithMany(user => user.ActivitySessions)
                .HasForeignKey(session => session.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(session => session.Calendar)
                .WithMany(calendar => calendar.ActivitySessions)
                .HasForeignKey(session => session.CalendarId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(session => session.Category)
                .WithMany(category => category.ActivitySessions)
                .HasForeignKey(session => session.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Återkommande veckoscheman
        builder.Entity<RecurringWorkoutSchedule>(entity =>
        {
            entity.Property(schedule => schedule.TimeZoneId)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(schedule => new
            {
                schedule.OwnerUserId,
                schedule.DayOfWeek
            });

            entity.HasOne(schedule => schedule.OwnerUser)
                .WithMany()
                .HasForeignKey(schedule => schedule.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(schedule => schedule.Calendar)
                .WithMany()
                .HasForeignKey(schedule => schedule.CalendarId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(schedule => schedule.Category)
                .WithMany()
                .HasForeignKey(schedule => schedule.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Notiser
        builder.Entity<Notification>(entity =>
        {
            entity.HasOne(notification => notification.User)
                .WithMany(user => user.Notifications)
                .HasForeignKey(notification => notification.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(notification => notification.ActivitySession)
                .WithMany()
                .HasForeignKey(notification => notification.ActivitySessionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Sömnloggar
        builder.Entity<SleepLog>(entity =>
        {
            entity.HasOne(log => log.User)
                .WithMany(user => user.SleepLogs)
                .HasForeignKey(log => log.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Kostloggar
        builder.Entity<NutritionLog>(entity =>
        {
            entity.HasOne(log => log.User)
                .WithMany(user => user.NutritionLogs)
                .HasForeignKey(log => log.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Användarinställningar
        builder.Entity<UserSettings>(entity =>
        {
            entity.HasKey(settings => settings.UserId);

            entity.HasOne(settings => settings.User)
                .WithOne(user => user.UserSettings)
                .HasForeignKey<UserSettings>(settings => settings.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}