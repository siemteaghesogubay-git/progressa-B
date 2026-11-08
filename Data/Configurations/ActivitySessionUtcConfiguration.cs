using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using progressa.Models;

namespace progressa.Data.Configurations;

public class ActivitySessionUtcConfiguration
    : IEntityTypeConfiguration<ActivitySession>
{
    public void Configure(EntityTypeBuilder<ActivitySession> builder)
    {
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            value => RequireUtc(value),
            value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

        builder.Property(session => session.ScheduledAt)
            .HasConversion(utcConverter);

        builder.Property(session => session.CreatedAt)
            .HasConversion(utcConverter);
    }

    private static DateTime RequireUtc(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
        {
            throw new InvalidOperationException(
                "Aktivitetspassets tidpunkt måste vara UTC innan den sparas.");
        }

        return value;
    }
}