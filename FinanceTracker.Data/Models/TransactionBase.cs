namespace FinanceTracker.Data.Models;

public abstract class TransactionBase
{
    public int Id { get; set; }

    public DateOnly TransactionDate { get; set; }

    public string Description { get; set; } = null!;

    public int TransactionTypeId { get; set; }

    public int CategoryId { get; set; }

    public int AccountPeriodId { get; set; }

    public virtual AccountPeriod AccountPeriod { get; set; } = null!;

    public virtual TransactionCategory Category { get; set; } = null!;

    public virtual TransactionType TransactionType { get; set; } = null!;
}
