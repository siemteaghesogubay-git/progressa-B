using System.ComponentModel.DataAnnotations;
using Xunit;
using static progressa.Dtos.ActivitySession_Dtos;

namespace Progressa.Tests;

public class ActivitySessionValidationTests
{
    [Fact]
    public void CreateActivitySession_FutureDate_ShouldBeInvalid()
    {
        // Arrange: Giltiga uppgifter, men starttiden är imorgon.
        var dto = new CreateActivitySessionDto
        {
            CalendarId = 1,
            CategoryId = 1,
            ScheduledAt = DateTimeOffset.UtcNow.AddDays(1),
            DurationInMinutes = 60
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        // Act: Kör attributvalidering och DTO:ns Validate-metod.
        var isValid = Validator.TryValidateObject(
            dto,
            context,
            results,
            validateAllProperties: true);

        // Assert: Passet nekas på grund av starttiden.
        Assert.False(isValid);

        Assert.Contains(results, error =>
            error.MemberNames.Contains(nameof(dto.ScheduledAt)));
    }

    [Fact]
    public void CreateActivitySession_PastDate_ShouldBeValid()
    {
        // Arrange: Giltiga uppgifter för ett pass som genomfördes igår.
        var dto = new CreateActivitySessionDto
        {
            CalendarId = 1,
            CategoryId = 1,
            ScheduledAt = DateTimeOffset.UtcNow.AddDays(-1),
            DurationInMinutes = 60
        };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        // Act: Validera uppgifterna.
        var isValid = Validator.TryValidateObject(
            dto,
            context,
            results,
            validateAllProperties: true);

        // Assert: Passet godkänns utan valideringsfel.
        Assert.True(isValid);
        Assert.Empty(results);
    }
}