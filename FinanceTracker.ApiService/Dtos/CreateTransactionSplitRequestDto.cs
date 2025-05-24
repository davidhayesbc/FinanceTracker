// d:\source\FinanceTracker\FinanceTracker.ApiService\Dtos\CreateTransactionSplitRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.ApiService.Dtos;

public class CreateTransactionSplitRequestDto
{
    [Required]
    public int CashTransactionId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Description { get; set; } = null!;
}
