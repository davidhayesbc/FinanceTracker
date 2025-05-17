using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data.Models;

public partial class AccountPeriod
{
    public int Id { get; set; }
    public int AccountId { get; set; }

    [Precision(18, 2)]
    public decimal OpeningBalance { get; set; }
    [Precision(18, 2)]
    public decimal? ClosingBalance { get; set; } = null;

    public DateOnly PeriodStart { get; set; }
    public DateOnly? PeriodEnd { get; set; } = null;
    public DateOnly? PeriodCloseDate { get; set; } = null;

    public virtual required Account Account { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>(); // Added

    //TODO: Figure out how to do a Mapping table in EF Core (for transactions)
}
