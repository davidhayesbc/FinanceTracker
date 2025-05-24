namespace FinanceTracker.Data.Models;

public class InvestmentTransaction : TransactionBase
{
    public decimal Quantity { get; set; }

    public decimal Price { get; set; } // Price per unit


    public int SecurityId { get; set; }

    // Investment-specific properties

    public virtual Security Security { get; set; } = null!;
}
