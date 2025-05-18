// d:\source\FinanceTracker\FinanceTracker.ApiService\Dtos\TransactionDto.cs
namespace FinanceTracker.ApiService.Dtos;

public class TransactionDto
{
    public int Id { get; set; }
    public int AccountId { get; set; } // From Transaction.AccountPeriod.AccountId
    public DateOnly TransactionDate { get; set; }
    public string Description { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal OriginalCost { get; set; } // From Transaction.OriginalCost

    // Flattened Security Properties
    public int? SecurityId { get; set; }
    public string? SecuritySymbol { get; set; }         // From Transaction.Security.Symbol
    public string? SecurityName { get; set; }           // From Transaction.Security.Name
    public string? SecurityIsin { get; set; }           // From Transaction.Security.ISIN
    public string? SecurityTypeName { get; set; }       // From Transaction.Security.SecurityType (string property on Security)
    public string? SecurityCurrencySymbol { get; set; } // From Transaction.Security.Currency.Symbol

    public string TransactionTypeName { get; set; } = null!; // From Transaction.TransactionType.Type
}
