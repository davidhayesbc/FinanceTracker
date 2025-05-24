namespace FinanceTracker.Data.Models;

public partial class RecurringTransaction
{
    public int Id { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal Amount { get; set; }

    public decimal AmountVariancePercentage { get; set; }

    public string Description { get; set; } = null!;

    public string RecurrenceCronExpression { get; set; } = null!;

    public int CashAccountId { get; set; }

    public int TransactionTypeId { get; set; }

    public int CategoryId { get; set; }


    public virtual CashAccount CashAccount { get; set; } = null!;

    public virtual TransactionCategory Category { get; set; } = null!;

    public virtual TransactionType TransactionType { get; set; } = null!;
}

