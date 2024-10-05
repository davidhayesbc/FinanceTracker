namespace FinanceTracker.Data.Models;

public partial class TransactionType
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<RecurringTransaction> RecurringTransactions { get; set; } = new List<RecurringTransaction>();

}
