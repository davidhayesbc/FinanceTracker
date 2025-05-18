// d:\source\FinanceTracker\FinanceTracker.ApiService\Dtos\CreateTransactionRequestDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.ApiService.Dtos;

public class CreateTransactionRequestDto
{
    [Required]
    public DateOnly TransactionDate { get; set; }

    [Required]
    [Range(0.000001, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public decimal Quantity { get; set; }

    [Required]
    [Range(0.00, double.MaxValue, ErrorMessage = "Original Cost must be a non-negative value.")]
    public decimal OriginalCost { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Description { get; set; } = null!;

    [Required]
    public int TransactionTypeId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int AccountPeriodId { get; set; }

    [Required]
    public int SecurityId { get; set; }
}
