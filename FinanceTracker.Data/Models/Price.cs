using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data.Models;

public partial class Price
{
  public int Id { get; set; }

  public int SecurityId { get; set; }

  public DateTime Date { get; set; }

  [Precision(18, 6)]
  public decimal ClosePrice { get; set; }

  public virtual Security Security { get; set; } = null!;
}
