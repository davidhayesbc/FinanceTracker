using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.ApiService.Dtos;

public class CreateSecurityRequestDto
{
    [Required]
    [MaxLength(20)]
    public string Symbol { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(12)]
    public string? ISIN { get; set; }

    [Required]
    public int CurrencyId { get; set; }

    [Required]
    [MaxLength(50)]
    public string SecurityType { get; set; } = string.Empty;
}
