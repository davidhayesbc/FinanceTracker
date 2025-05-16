// filepath: d:\source\FinanceTracker\FinanceTracker.ApiService\Dtos\AccountDto.cs
namespace FinanceTracker.ApiService.Dtos;

public class AccountDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal OpeningBalance { get; set; } // Mapped from OpeningBalance
    public string AccountTypeName { get; set; } = string.Empty; // Mapped from AccountType.Type
    public string CurrencySymbol { get; set; } = string.Empty; // Mapped from Currency.Symbol
    public string CurrencyDisplaySymbol { get; set; } = string.Empty; // Mapped from Currency.DisplaySymbol
    public DateOnly OpenedDate { get; set; } // Mapped from OpenDate
}
