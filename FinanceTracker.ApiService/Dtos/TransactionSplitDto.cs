// d:\source\FinanceTracker\FinanceTracker.ApiService\Dtos\TransactionSplitDto.cs
namespace FinanceTracker.ApiService.Dtos;

public class TransactionSplitDto
{
    public int Id { get; set; }
    public int TransactionId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!; // Flattened from Category.Category
    public decimal Amount { get; set; }
    public string Description { get; set; } = null!;

}
