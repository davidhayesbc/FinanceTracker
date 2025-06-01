using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.ApiService.Dtos;

public class CreateTransactionTypeRequestDto
{
    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;
}
