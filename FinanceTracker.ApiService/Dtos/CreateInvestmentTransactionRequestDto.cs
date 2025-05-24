// d:\source\FinanceTracker\FinanceTracker.ApiService\Dtos\CreateInvestmentTransactionRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.ApiService.Dtos;

public class CreateInvestmentTransactionRequestDto
{
    [Required]
    public DateOnly TransactionDate { get; set; }

    [Required]
    [Range(0.000001, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public decimal Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Description { get; set; } = null!;

    [Required]
    public int TransactionTypeId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int AccountPeriodId { get; set; }

    [Required]
    public int SecurityId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Fees must be a non-negative value.")]
    public decimal? Fees { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Commission must be a non-negative value.")]
    public decimal? Commission { get; set; }

    [StringLength(20)]
    public string? OrderType { get; set; }
}
