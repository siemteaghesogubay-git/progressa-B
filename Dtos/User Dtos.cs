using System.ComponentModel.DataAnnotations;

namespace progressa.Dtos
{
    public class User_Dtos
    {
        public class RegisterDto
        {
            [Required(ErrorMessage = "First name is required.")]
            [MaxLength(15, ErrorMessage = "First name must not exceed 15 characters.")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Last name is required.")]
            [MaxLength(15, ErrorMessage = "Last name must not exceed 15 characters.")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email address is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            [MaxLength(256)]
            public string Email { get; set; } = string.Empty;

            [Range(1, 120, ErrorMessage = "Age must be between 1 and 120.")]
            public int Age { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
            [MaxLength(100)]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please confirm your password.")]
            [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        
        public class RegisterResponseDto
        {
            public string Id { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
        }

        public class LoginDto
        {
            [Required(ErrorMessage = "Email is required.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required.")]
            public string Password { get; set; } = string.Empty;

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                if (!string.IsNullOrWhiteSpace(Email) &&
                    !new EmailAddressAttribute().IsValid(Email))
                {
                    yield return new ValidationResult(
                        "Invalid email address.",
                         [nameof(Email)]);
                }
            }
        }


        public class LoginResponseDto
        {
            public string Token { get; set; } = string.Empty;
            public string UserId { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }



        public class ForgotPasswordDto
        {
            [Required(ErrorMessage = "E-postadress krävs.")]
            [EmailAddress(ErrorMessage = "Ogiltig e-postadress.")]
            public string Email { get; set; } = string.Empty;
        }

        public class ResetPasswordDto
        {
            [Required(ErrorMessage = "E-postadress krävs.")]
            [EmailAddress(ErrorMessage = "Ogiltig e-postadress.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Token krävs.")]
            public string Token { get; set; } = string.Empty;

            [Required(ErrorMessage = "Nytt lösenord krävs.")]
            [MinLength(8, ErrorMessage = "Lösenordet måste vara minst 8 tecken.")]
            [MaxLength(100)]
            public string NewPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Bekräfta lösenordet.")]
            [Compare(nameof(NewPassword), ErrorMessage = "Lösenorden matchar inte.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }


    }
}
        

    

