using System.ComponentModel.DataAnnotations;
using progressa.Dtos;

namespace Progressa.Tests;

public class DailyLogValidationTests
{
    [Fact]
    public void DailyLogAmounts_RejectsNegativeAndOverLimitValues()
    {
        var amounts = new DailyLogAmountsDto { Protein = -0.01m, Calories = 20000.01m };
        var results = new List<ValidationResult>();

        Assert.False(Validator.TryValidateObject(amounts, new ValidationContext(amounts), results, true));
        Assert.Contains(results, error => error.MemberNames.Contains(nameof(amounts.Protein)));
        Assert.Contains(results, error => error.MemberNames.Contains(nameof(amounts.Calories)));
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(1440, true)]
    [InlineData(1441, false)]
    public void DailyLogSleepMinutes_ValidatesDailyLimits(int minutes, bool expected)
    {
        var log = new DailyLogDto { SleepMinutes = minutes };

        var isValid = Validator.TryValidateObject(log, new ValidationContext(log), null, true);

        Assert.Equal(expected, isValid);
    }
}
