namespace FinanceTracker.Data.Models;

public partial class TransactionCategory
{
    public int Id { get; set; }

    public string Category { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<TransactionSplit> TransactionSplits { get; set; } = new List<TransactionSplit>(); public virtual ICollection<TransactionBase> Transactions { get; set; } = new List<TransactionBase>();

    public virtual ICollection<RecurringTransaction> RecurringTransactions { get; set; } = new List<RecurringTransaction>();

}
