namespace FinanceTracker.Data.Models;

public partial class TransactionType
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<TransactionBase> Transactions { get; set; } = new List<TransactionBase>();

    public virtual ICollection<CashTransaction> CashTransactions { get; set; } = new List<CashTransaction>();

    public virtual ICollection<InvestmentTransaction> InvestmentTransactions { get; set; } = new List<InvestmentTransaction>();

    public virtual ICollection<RecurringTransaction> RecurringTransactions { get; set; } = new List<RecurringTransaction>();

}
