using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.ApiService.Dtos;

public class CreateTransactionCategoryRequestDto
{
    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Description { get; set; } = string.Empty;
}
