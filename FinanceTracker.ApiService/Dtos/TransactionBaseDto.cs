// d:\source\FinanceTracker\FinanceTracker.ApiService\Dtos\TransactionBaseDto.cs
namespace FinanceTracker.ApiService.Dtos;

public abstract class TransactionBaseDto
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public DateOnly TransactionDate { get; set; }
    public string Description { get; set; } = null!;
    public int TransactionTypeId { get; set; }
    public string TransactionTypeName { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public int AccountPeriodId { get; set; }
    public string TransactionKind { get; set; } = string.Empty; // "Cash" or "Investment"
}
