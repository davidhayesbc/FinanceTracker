// d:\source\FinanceTracker\FinanceTracker.ApiService\Dtos\AccountBaseDto.cs
namespace FinanceTracker.ApiService.Dtos;

public abstract class AccountBaseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public decimal CurrentBalance { get; set; }
    public string AccountTypeName { get; set; } = string.Empty;
    public string CurrencySymbol { get; set; } = string.Empty;
    public string CurrencyDisplaySymbol { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string AccountKind { get; set; } = string.Empty; // "Cash" or "Investment"
}
