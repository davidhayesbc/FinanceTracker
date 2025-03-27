using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTracker.Data.Models
{
  /// <summary>
  /// Represents a foreign exchange rate between two currencies.
  /// </summary>
  public class FxRate
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public int FromCurrencyId { get; set; }

    [Required]
    public int ToCurrencyId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,6)")]
    public decimal Rate { get; set; }

    public DateOnly Date { get; set; }

    // Navigation properties
    [ForeignKey("FromCurrencyId")]
    public virtual Currency FromCurrencyNavigation { get; set; } = null!;

    [ForeignKey("ToCurrencyId")]
    public virtual Currency ToCurrencyNavigation { get; set; } = null!;
  }
}