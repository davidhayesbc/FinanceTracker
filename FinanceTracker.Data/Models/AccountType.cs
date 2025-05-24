namespace FinanceTracker.Data.Models;

public partial class AccountType
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<AccountBase> Accounts { get; set; } = new List<AccountBase>();
}
