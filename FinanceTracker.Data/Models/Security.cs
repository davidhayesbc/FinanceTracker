using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data.Models;

public partial class Security
{
  public int Id { get; set; }

  public string Symbol { get; set; } = null!;

  public string Name { get; set; } = null!;

  public string? ISIN { get; set; }

  public int CurrencyId { get; set; }

  public string SecurityType { get; set; } = null!;
  public virtual Currency Currency { get; set; } = null!;

  public virtual ICollection<Price> Prices { get; set; } = new List<Price>();

  public virtual ICollection<InvestmentTransaction> InvestmentTransactions { get; set; } = new List<InvestmentTransaction>();
}
