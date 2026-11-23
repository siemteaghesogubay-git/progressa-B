using System.ComponentModel.DataAnnotations;
using progressa.Dtos;

namespace Progressa.Tests;

public class UserSettingsValidationTests
{
    [Fact]
    public void UpdateUserSettings_UnknownWeekStart_ShouldBeInvalid()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.WeekStart = "fri";

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(
            dto,
            context,
            results,
            validateAllProperties: true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, error =>
            error.MemberNames.Contains(nameof(dto.WeekStart)));
    }

    [Fact]
    public void UpdateUserSettings_AllowedValues_ShouldBeValid()
    {
        // Arrange
        var dto = CreateValidDto();

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(
            dto,
            context,
            results,
            validateAllProperties: true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    private static UpdateUserSettingsDto CreateValidDto()
    {
        return new UpdateUserSettingsDto
        {
            DistanceUnit = "mi",
            WeightUnit = "lb",
            EnergyUnit = "kj",
            HeightUnit = "ft",
            TimeFormat = "12h",
            WeekStart = "sun"
        };
    }
}
