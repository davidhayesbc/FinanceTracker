namespace FinanceTracker.Data.Models;

public class InvestmentTransaction : TransactionBase
{
    public decimal Quantity { get; set; }
    public decimal Price { get; set; } // Price per unit
    public decimal? Fees { get; set; }
    public decimal? Commission { get; set; }
    public string? OrderType { get; set; } // e.g., "Market", "Limit", "Stop"

    public int SecurityId { get; set; }

    // Navigation properties
    public virtual Security Security { get; set; } = null!;
}
