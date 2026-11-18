using progressa.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Progressa.Tests.Dtos
{
    public class DeleteAccountDtoTests
    {
        [Fact]
        public void DeleteAccount_EmptyPassword_ShouldBeInvalid()
        {
            //Arrange - DTO med tomt lösenord
            var dto = new DeleteAccountDto{Password = ""};
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();

            //Act - Validering
            bool isValid = Validator.TryValidateObject(dto, context, results, true);


            //Assert
            Assert.False(isValid);
            Assert.Contains(results, e => e.MemberNames.Contains(nameof(dto.Password)));
        }

        [Fact]
        public void DeleteAccount_WithPassword_ShouldBeValid()
        {
            //Arrange - Dto med korrekt lösenord
            var dto = new DeleteAccountDto { Password = "Test123!" };
            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();

            //Act - Validering
            bool isValid = Validator.TryValidateObject(dto, context, results, true);

            //Assert
            Assert.Empty(results);
        }
    }
}
