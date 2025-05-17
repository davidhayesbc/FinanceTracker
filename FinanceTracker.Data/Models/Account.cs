using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Data.Models;

public partial class Account
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int AccountTypeId { get; set; }

    public int CurrencyId { get; set; }

    [MaxLength(100)]
    public string Institution { get; set; } = null!;

    public virtual AccountType AccountType { get; set; } = null!;

    public virtual Currency Currency { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<RecurringTransaction> RecurringTransactions { get; set; } = new List<RecurringTransaction>();

    public virtual ICollection<AccountPeriod> AccountPeriods { get; set; } = new List<AccountPeriod>();
}
