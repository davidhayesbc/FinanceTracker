namespace FinanceTracker.ApiService.Dtos;

public class SecurityDto
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ISIN { get; set; }
    public int CurrencyId { get; set; }
    public string CurrencySymbol { get; set; } = string.Empty;
    public string CurrencyDisplaySymbol { get; set; } = string.Empty;
    public string SecurityType { get; set; } = string.Empty;
}
