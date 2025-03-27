using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data.Models;

public partial class AccountPeriod
{
    public int Id { get; set; }
    public int AccountId { get; set; }

    [Precision(18, 2)]
    public decimal OpeningBalance { get; set; }
    [Precision(18, 2)]
    public decimal ClosingBalance { get; set; }

    public DateOnly PeriodStart{ get; set; }
    public DateOnly PeriodEnd { get; set; }
    public DateOnly PeriodCloseDate { get; set; }

    public virtual Account Account { get; set; } = null!;

    //TODO: Figure out how to do a Mapping table in EF Core (for transactions)
}
