using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data.Models;

public partial class Account
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    [Precision(18, 2)] 
    public decimal OpeningBalance { get; set; }

    public DateOnly OpenDate { get; set; }

    public int AccountTypeId { get; set; }

    public virtual AccountType AccountType { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<RecurringTransaction> RecurringTransactions { get; set; } = new List<RecurringTransaction>();
}
