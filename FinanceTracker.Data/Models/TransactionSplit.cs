namespace FinanceTracker.Data.Models;

public partial class TransactionSplit
{
    public int Id { get; set; }
    public int CashTransactionId { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; } = null!;

    public int CategoryId { get; set; }

    public virtual TransactionCategory Category { get; set; } = null!;

    public virtual CashTransaction CashTransaction { get; set; } = null!;
}
