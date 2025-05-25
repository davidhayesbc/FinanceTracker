using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.ApiService.Dtos;

public class CreateCashAccountRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Institution { get; set; } = string.Empty;

    [Required]
    public int AccountTypeId { get; set; }

    [Required]
    public int CurrencyId { get; set; }

    public bool IsActive { get; set; } = true;

    public decimal? OverdraftLimit { get; set; }

    public decimal InitialBalance { get; set; } = 0;
}
