using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data.Models;

public partial class FinanceTackerDbContext : DbContext
{
    public FinanceTackerDbContext()
    {
    }

    public FinanceTackerDbContext(DbContextOptions<FinanceTackerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountPeriod> AccountPeriods { get; set; }

    public virtual DbSet<AccountType> AccountTypes { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }
    public virtual DbSet<RecurringTransaction> RecurringTransactions { get; set; }

    public virtual DbSet<TransactionCategory> TransactionCategories { get; set; }

    public virtual DbSet<TransactionSplit> TransactionSplits { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Account");

            entity.ToTable("Account");

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.OpeningBalance).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.AccountType).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.AccountTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_ToAccountType");
        });

        modelBuilder.Entity<AccountPeriod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_AccountPeriod");

            entity.ToTable("AccountPeriod");

            entity.Property(e => e.OpeningBalance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ClosingBalance).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Account).WithMany(p => p.AccountPeriods)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AccountPeriod_ToAccount");
        });

        modelBuilder.Entity<AccountType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AccountType");

            entity.ToTable("AccountType");

            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasIndex(e => e.Type).IsUnique().HasDatabaseName("UX_AccountType_Type");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Transaction");

            entity.ToTable("Transaction");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Description).HasMaxLength(100);

            entity.HasOne(d => d.Account).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_ToAccount");

            entity.HasOne(d => d.Category).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_ToTransactionCategory");

            entity.HasOne(d => d.TransactionType).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TransactionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_ToTransactionType");
        });

        modelBuilder.Entity<RecurringTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_RecurringTransaction");

            entity.ToTable("RecurringTransaction");

            entity.Property(e => e.StartDate).HasColumnType("date");
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AmountVariancePercentage).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.RecurrenceCronExpression).HasMaxLength(100);

            entity.HasOne(d => d.Account).WithMany(p => p.RecurringTransactions)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RecurringTransaction_ToAccount");

            entity.HasOne(d => d.Category).WithMany(p => p.RecurringTransactions)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RecurringTransaction_ToTransactionCategory");

            entity.HasOne(d => d.TransactionType).WithMany(p => p.RecurringTransactions)
                .HasForeignKey(d => d.TransactionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RecurringTransaction_ToTransactionType");
        });
        modelBuilder.Entity<TransactionCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TransactionCategory");

            entity.ToTable("TransactionCategory");

            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(100);

            entity.HasIndex(e => e.Category).IsUnique().HasDatabaseName("UX_TransactionCategory_Category");
        });

        modelBuilder.Entity<TransactionSplit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TransactionSplit");

            entity.ToTable("TransactionSplit");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Description).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.TransactionSplits)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransactionSplit_ToTransactionCategory");

            entity.HasOne(d => d.Transaction).WithMany(p => p.TransactionSplits)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransactionSplit_ToTransaction");
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TransactionType");

            entity.ToTable("TransactionType");

            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasIndex(e => e.Type).IsUnique().HasDatabaseName("UX_TransactionType_Type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
