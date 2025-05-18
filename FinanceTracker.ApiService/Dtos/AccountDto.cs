// filepath: d:\source\FinanceTracker\FinanceTracker.ApiService\Dtos\AccountDto.cs
namespace FinanceTracker.ApiService.Dtos;

public class AccountDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public decimal CurrentBalance { get; set; }
    public string AccountTypeName { get; set; } = string.Empty;
    public string CurrencySymbol { get; set; } = string.Empty;
    public string CurrencyDisplaySymbol { get; set; } = string.Empty;

}
