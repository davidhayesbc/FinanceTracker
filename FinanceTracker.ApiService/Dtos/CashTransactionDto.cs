// d:\source\FinanceTracker\FinanceTracker.ApiService\Dtos\CashTransactionDto.cs
namespace FinanceTracker.ApiService.Dtos;

public class CashTransactionDto : TransactionBaseDto
{
    public decimal Amount { get; set; }
    public int? TransferToCashAccountId { get; set; }
    public string? TransferToCashAccountName { get; set; }
}
