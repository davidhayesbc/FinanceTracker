// d:\source\FinanceTracker\FinanceTracker.ApiService\Dtos\CashAccountDto.cs
namespace FinanceTracker.ApiService.Dtos;

public class CashAccountDto : AccountBaseDto
{
    public decimal? OverdraftLimit { get; set; }
}
