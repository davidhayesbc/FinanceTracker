// d:\source\FinanceTracker\FinanceTracker.ApiService\Dtos\InvestmentAccountDto.cs
namespace FinanceTracker.ApiService.Dtos;

public class InvestmentAccountDto : AccountBaseDto
{
    public string? BrokerAccountNumber { get; set; }
    public bool IsTaxAdvantaged { get; set; }
    public string? TaxAdvantageType { get; set; }
}
