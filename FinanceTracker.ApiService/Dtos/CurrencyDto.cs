namespace FinanceTracker.ApiService.Dtos;

public class CurrencyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string DisplaySymbol { get; set; } = string.Empty;
}
