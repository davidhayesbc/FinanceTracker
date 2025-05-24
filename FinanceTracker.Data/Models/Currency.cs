using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data.Models;

public partial class Currency
{
  public int Id { get; set; }

  public string Name { get; set; } = null!;

  public string Symbol { get; set; } = null!;

  public string DisplaySymbol { get; set; } = null!;
  public virtual ICollection<AccountBase> Accounts { get; set; } = new List<AccountBase>();

  public virtual ICollection<CashAccount> CashAccounts { get; set; } = new List<CashAccount>();

  public virtual ICollection<InvestmentAccount> InvestmentAccounts { get; set; } = new List<InvestmentAccount>();

  public virtual ICollection<Security> Securities { get; set; } = new List<Security>();

  public virtual ICollection<FxRate> BaseCurrencyRates { get; set; } = new List<FxRate>();

  public virtual ICollection<FxRate> CounterCurrencyRates { get; set; } = new List<FxRate>();
}
