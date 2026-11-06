using progressa.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Progressa.Tests
{
    public class PlannedSessionTests
    {
        [Fact]
        public void PlannedSession_PastDate_ShouldBeInvalid()
        {
            // Arrange - Skapa en PlannedSessionDto med ett datum som redan har passerat
            var dto = new CreatePlannedSessionDto
            {
                CalendarId = 1,
                CategoryId = 1,
                ScheduledAt = DateTimeOffset.UtcNow.AddDays(-1), // Past date
                PlannedDurationInMinutes = 60
            };

            // Förbered validering ohc en lista för fel
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();

            // Act - Kontrollera om passet är giltigt
            bool isValid = Validator.TryValidateObject(
                dto, context, results, true);

            // Kontrollera att passet är ogiltigt
            Assert.False(isValid);
            // Kontrollera att felet gäller datumet
            Assert.Contains(results, r =>
               r.MemberNames.Contains(nameof(dto.ScheduledAt)));
        }

        [Fact]
        public void PlannedSession_ZeroDuration_ShouldbeInvalid()
        {
            // Arrange - Skapa ett pass som är 0 minuter långt
            var dto = new CreatePlannedSessionDto
            {
                CalendarId = 1,
                CategoryId = 1,
                ScheduledAt = DateTimeOffset.UtcNow.AddDays(1),
                PlannedDurationInMinutes = 0
            };

            // Förbered valideringen och en lista för fel
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();

            // Act - Validera passet
            bool isValid = Validator.TryValidateObject(
                dto, context, results, true);

            // Kontrollera att passet är ogiltigt
            Assert.False(isValid);
            // Kontrollera att felet gäller passets längd
            Assert.Contains(results, r =>
               r.MemberNames.Contains(nameof(dto.PlannedDurationInMinutes)));
        }
    }
}
