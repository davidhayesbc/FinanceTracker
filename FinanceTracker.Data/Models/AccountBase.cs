using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Data.Models;

public abstract class AccountBase
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int AccountTypeId { get; set; }

    public int CurrencyId { get; set; }
    public bool IsActive { get; set; } = true;

    [MaxLength(100)]
    public string Institution { get; set; } = null!;

    public virtual AccountType AccountType { get; set; } = null!;

    public virtual Currency Currency { get; set; } = null!;

    public virtual ICollection<AccountPeriod> AccountPeriods { get; set; } = new List<AccountPeriod>();
}
