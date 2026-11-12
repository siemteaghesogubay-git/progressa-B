using System.ComponentModel.DataAnnotations;

namespace progressa.Dtos;

public class CreateActivityCategoryDto
{
    [Required(ErrorMessage = "Ange ett namn för aktiviteten.")]
    [MaxLength(50, ErrorMessage = "Namnet får vara högst 50 tecken.")]
    public string Name { get; set; } = string.Empty;
}