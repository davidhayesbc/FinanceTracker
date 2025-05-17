namespace FinanceTracker.Data.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public DateOnly TransactionDate { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; } = null!;

    public int TransactionTypeId { get; set; }

    public int CategoryId { get; set; }

    public int AccountPeriodId { get; set; }

    public virtual AccountPeriod AccountPeriod { get; set; } = null!;

    public virtual TransactionCategory Category { get; set; } = null!;

    public virtual ICollection<TransactionSplit> TransactionSplits { get; set; } = new List<TransactionSplit>();

    public virtual TransactionType TransactionType { get; set; } = null!;
}
