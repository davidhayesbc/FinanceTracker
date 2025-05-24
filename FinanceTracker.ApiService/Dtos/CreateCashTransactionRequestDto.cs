// d:\source\FinanceTracker\FinanceTracker.ApiService\Dtos\CreateCashTransactionRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.ApiService.Dtos;

public class CreateCashTransactionRequestDto
{
    [Required]
    public DateOnly TransactionDate { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Description { get; set; } = null!;

    [Required]
    public int TransactionTypeId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int AccountPeriodId { get; set; }

    // Optional - for transfers between cash accounts
    public int? TransferToCashAccountId { get; set; }
}
