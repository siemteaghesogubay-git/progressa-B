using System.ComponentModel.DataAnnotations;

namespace progressa.Dtos;

public class CompletePlannedSessionDto
{
    [Range(1, 1440, ErrorMessage = "Ange mellan 1 och 1440 minuter.")]
    public int ActualDurationInMinutes { get; set; }


    [Range(0, 20000, ErrorMessage = "Kalorier måste vara mellan 0 och 20 000.")]
    public int? CaloriesBurned { get; set; }


    [MaxLength(500, ErrorMessage = "Anteckningen får vara högst 500 tecken.")]
    public string? Notes { get; set; }
}