using System.ComponentModel.DataAnnotations;

namespace progressa.Dtos;

public class DailyLogDto
{
    public Guid? Version { get; init; }

    [Required]
    public DailyLogAmountsDto Amounts { get; init; } = new();

    [Required]
    public DailyLogChecksDto Checks { get; init; } = new();

    [Range(0, 1440, ErrorMessage = "Ange mellan 0 och 1440 minuters sömn.")]
    public int? SleepMinutes { get; init; }
}

public class DailyLogAmountsDto
{
    [Range(typeof(decimal), "0", "20000")]
    public decimal? Calories { get; init; }

    [Range(typeof(decimal), "0", "2000")]
    public decimal? Protein { get; init; }

    [Range(typeof(decimal), "0", "2000")]
    public decimal? Carbs { get; init; }

    [Range(typeof(decimal), "0", "2000")]
    public decimal? Fat { get; init; }
}

public class DailyLogChecksDto
{
    public bool? Calories { get; init; }
    public bool? Protein { get; init; }
    public bool? Carbs { get; init; }
    public bool? Fat { get; init; }
    public bool? Sleep { get; init; }
}
