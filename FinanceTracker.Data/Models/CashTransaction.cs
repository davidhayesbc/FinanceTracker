namespace FinanceTracker.Data.Models;

public class CashTransaction : TransactionBase
{
    public decimal Amount { get; set; }

    // Cash-specific properties
    public int? TransferToCashAccountId { get; set; } // For transfers between cash accounts

    public virtual CashAccount? TransferToCashAccount { get; set; }

    public virtual ICollection<TransactionSplit> TransactionSplits { get; set; } = new List<TransactionSplit>();
}
