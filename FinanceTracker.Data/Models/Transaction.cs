namespace FinanceTracker.Data.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public DateOnly TransactionDate { get; set; }

    public decimal Quantity { get; set; } // Renamed from Amount

    public decimal OriginalCost { get; set; } // Added OriginalCost

    public string Description { get; set; } = null!;

    public int TransactionTypeId { get; set; }

    public int CategoryId { get; set; }

    public int AccountPeriodId { get; set; }

    public int SecurityId { get; set; } // Added SecurityId foreign key

    public virtual AccountPeriod AccountPeriod { get; set; } = null!;

    public virtual TransactionCategory Category { get; set; } = null!;

    public virtual Security Security { get; set; } = null!; // Added Security navigation property

    public virtual ICollection<TransactionSplit> TransactionSplits { get; set; } = new List<TransactionSplit>();

    public virtual TransactionType TransactionType { get; set; } = null!;
}
