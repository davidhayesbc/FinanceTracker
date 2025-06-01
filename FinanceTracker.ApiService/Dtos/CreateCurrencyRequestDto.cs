using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.ApiService.Dtos;

public class CreateCurrencyRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string Symbol { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string DisplaySymbol { get; set; } = string.Empty;
}
