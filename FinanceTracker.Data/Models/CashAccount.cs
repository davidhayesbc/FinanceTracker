namespace FinanceTracker.Data.Models;

public class CashAccount : AccountBase
{
    // Cash-specific properties
    public decimal? OverdraftLimit { get; set; }

    // Navigation properties for cash transactions
    public virtual ICollection<CashTransaction> CashTransactions { get; set; } = new List<CashTransaction>();

    public virtual ICollection<RecurringTransaction> RecurringTransactions { get; set; } = new List<RecurringTransaction>();
}
