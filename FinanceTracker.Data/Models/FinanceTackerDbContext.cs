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

    public virtual DbSet<AccountType> AccountTypes { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionCategory> TransactionCategories { get; set; }

    public virtual DbSet<TransactionSplit> TransactionSplits { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:FinanceTracker");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__3214EC072E8BD840");

            entity.ToTable("Account");

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.OpeningBalance).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.AccountType).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.AccountTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_ToAccountType");
        });

        modelBuilder.Entity<AccountType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AccountT__3214EC0785814C1D");

            entity.ToTable("AccountType");

            entity.Property(e => e.Type).HasMaxLength(50);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC07E906A3AC");

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

        modelBuilder.Entity<TransactionCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC0788389A10");

            entity.ToTable("TransactionCategory");

            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(100);
        });

        modelBuilder.Entity<TransactionSplit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC078C80138B");

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
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC07D26F23FD");

            entity.ToTable("TransactionType");

            entity.Property(e => e.Type).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
