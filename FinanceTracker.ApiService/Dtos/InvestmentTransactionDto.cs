// d:\source\FinanceTracker\FinanceTracker.ApiService\Dtos\InvestmentTransactionDto.cs
namespace FinanceTracker.ApiService.Dtos;

public class InvestmentTransactionDto : TransactionBaseDto
{
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalValue => Quantity * Price;
    public decimal? Fees { get; set; }
    public decimal? Commission { get; set; }
    public string? OrderType { get; set; }

    // Security information
    public int SecurityId { get; set; }
    public string SecuritySymbol { get; set; } = null!;
    public string SecurityName { get; set; } = null!;
    public string? SecurityIsin { get; set; }
    public string SecurityTypeName { get; set; } = null!;
    public string SecurityCurrencySymbol { get; set; } = null!;
}
