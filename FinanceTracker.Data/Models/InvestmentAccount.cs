namespace FinanceTracker.Data.Models;

public class InvestmentAccount : AccountBase
{
    // Investment-specific properties
    public string? BrokerAccountNumber { get; set; }
    public bool IsTaxAdvantaged { get; set; }
    public string? TaxAdvantageType { get; set; } // e.g., "401k", "IRA", "Roth IRA"

    // Navigation properties for investment transactions
    public virtual ICollection<InvestmentTransaction> InvestmentTransactions { get; set; } = new List<InvestmentTransaction>();
}
