// d:\source\FinanceTracker\FinanceTracker.ApiService\Dtos\CreateTransactionSplitRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.ApiService.Dtos;

public class CreateTransactionSplitRequestDto
{
    [Required]
    public int TransactionId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Description { get; set; } = null!;

    // Assuming Notes is not part of TransactionSplit based on schema,
    // if it is, add: public string? Notes { get; set; }
}
