﻿// <auto-generated />
using System;
using FinanceTracker.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FinanceTracker.Data.Migrations
{
    [DbContext(typeof(FinanceTackerDbContext))]
    [Migration("20250327201902_FixCurrencyTable")]
    partial class FixCurrencyTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FinanceTracker.Data.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountTypeId")
                        .HasColumnType("int");

                    b.Property<int>("CurrencyId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateOnly>("OpenDate")
                        .HasColumnType("date");

                    b.Property<decimal>("OpeningBalance")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("Id")
                        .HasName("PK_Account");

                    b.HasIndex("AccountTypeId");

                    b.HasIndex("CurrencyId");

                    b.ToTable("Account", (string)null);
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.AccountPeriod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<decimal>("ClosingBalance")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal>("OpeningBalance")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18, 2)");

                    b.Property<DateOnly>("PeriodCloseDate")
                        .HasColumnType("date");

                    b.Property<DateOnly>("PeriodEnd")
                        .HasColumnType("date");

                    b.Property<DateOnly>("PeriodStart")
                        .HasColumnType("date");

                    b.HasKey("Id")
                        .HasName("PK_AccountPeriod");

                    b.HasIndex("AccountId");

                    b.ToTable("AccountPeriod", (string)null);
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.AccountType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id")
                        .HasName("PK__AccountType");

                    b.HasIndex("Type")
                        .IsUnique()
                        .HasDatabaseName("UX_AccountType_Type");

                    b.ToTable("AccountType", (string)null);
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.HasKey("Id")
                        .HasName("PK_Currency");

                    b.HasIndex("Symbol")
                        .IsUnique()
                        .HasDatabaseName("UX_Currency_Symbol");

                    b.ToTable("Currency", (string)null);
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.FxRate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<int>("FromCurrencyId")
                        .HasColumnType("int");

                    b.Property<decimal>("Rate")
                        .HasColumnType("decimal(18, 6)");

                    b.Property<int>("ToCurrencyId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK_FxRate");

                    b.HasIndex("ToCurrencyId");

                    b.HasIndex("FromCurrencyId", "ToCurrencyId", "Date")
                        .IsUnique()
                        .HasDatabaseName("UX_FxRate_Currencies_Date");

                    b.ToTable("FxRate", (string)null);
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.Price", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("ClosePrice")
                        .HasPrecision(18, 6)
                        .HasColumnType("decimal(18, 6)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("SecurityId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK_Price");

                    b.HasIndex("SecurityId", "Date")
                        .IsUnique()
                        .HasDatabaseName("UX_Price_SecurityId_Date");

                    b.ToTable("Price", (string)null);
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.RecurringTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<decimal>("AmountVariancePercentage")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateOnly?>("EndDate")
                        .HasColumnType("date");

                    b.Property<string>("RecurrenceCronExpression")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<int>("TransactionTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK_RecurringTransaction");

                    b.HasIndex("AccountId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("TransactionTypeId");

                    b.ToTable("RecurringTransaction", (string)null);
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.Security", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CurrencyId")
                        .HasColumnType("int");

                    b.Property<string>("ISIN")
                        .HasMaxLength(12)
                        .HasColumnType("nvarchar(12)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SecurityType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id")
                        .HasName("PK_Security");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("ISIN")
                        .HasDatabaseName("IX_Security_ISIN");

                    b.HasIndex("Symbol")
                        .IsUnique()
                        .HasDatabaseName("UX_Security_Symbol");

                    b.ToTable("Security", (string)null);
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateOnly>("TransactionDate")
                        .HasColumnType("date");

                    b.Property<int>("TransactionTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK_Transaction");

                    b.HasIndex("AccountId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("TransactionTypeId");

                    b.ToTable("Transaction", (string)null);
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.TransactionCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id")
                        .HasName("PK_TransactionCategory");

                    b.HasIndex("Category")
                        .IsUnique()
                        .HasDatabaseName("UX_TransactionCategory_Category");

                    b.ToTable("TransactionCategory", (string)null);
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.TransactionSplit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("TransactionId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK_TransactionSplit");

                    b.HasIndex("CategoryId");

                    b.HasIndex("TransactionId");

                    b.ToTable("TransactionSplit", (string)null);
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.TransactionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id")
                        .HasName("PK_TransactionType");

                    b.HasIndex("Type")
                        .IsUnique()
                        .HasDatabaseName("UX_TransactionType_Type");

                    b.ToTable("TransactionType", (string)null);
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.Account", b =>
                {
                    b.HasOne("FinanceTracker.Data.Models.AccountType", "AccountType")
                        .WithMany("Accounts")
                        .HasForeignKey("AccountTypeId")
                        .IsRequired()
                        .HasConstraintName("FK_Account_ToAccountType");

                    b.HasOne("FinanceTracker.Data.Models.Currency", "Currency")
                        .WithMany("Accounts")
                        .HasForeignKey("CurrencyId")
                        .IsRequired()
                        .HasConstraintName("FK_Account_ToCurrency");

                    b.Navigation("AccountType");

                    b.Navigation("Currency");
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.AccountPeriod", b =>
                {
                    b.HasOne("FinanceTracker.Data.Models.Account", "Account")
                        .WithMany("AccountPeriods")
                        .HasForeignKey("AccountId")
                        .IsRequired()
                        .HasConstraintName("FK_AccountPeriod_ToAccount");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.FxRate", b =>
                {
                    b.HasOne("FinanceTracker.Data.Models.Currency", "FromCurrencyNavigation")
                        .WithMany("BaseCurrencyRates")
                        .HasForeignKey("FromCurrencyId")
                        .IsRequired()
                        .HasConstraintName("FK_FxRate_ToFromCurrency");

                    b.HasOne("FinanceTracker.Data.Models.Currency", "ToCurrencyNavigation")
                        .WithMany("CounterCurrencyRates")
                        .HasForeignKey("ToCurrencyId")
                        .IsRequired()
                        .HasConstraintName("FK_FxRate_ToToCurrency");

                    b.Navigation("FromCurrencyNavigation");

                    b.Navigation("ToCurrencyNavigation");
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.Price", b =>
                {
                    b.HasOne("FinanceTracker.Data.Models.Security", "Security")
                        .WithMany("Prices")
                        .HasForeignKey("SecurityId")
                        .IsRequired()
                        .HasConstraintName("FK_Price_ToSecurity");

                    b.Navigation("Security");
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.RecurringTransaction", b =>
                {
                    b.HasOne("FinanceTracker.Data.Models.Account", "Account")
                        .WithMany("RecurringTransactions")
                        .HasForeignKey("AccountId")
                        .IsRequired()
                        .HasConstraintName("FK_RecurringTransaction_ToAccount");

                    b.HasOne("FinanceTracker.Data.Models.TransactionCategory", "Category")
                        .WithMany("RecurringTransactions")
                        .HasForeignKey("CategoryId")
                        .IsRequired()
                        .HasConstraintName("FK_RecurringTransaction_ToTransactionCategory");

                    b.HasOne("FinanceTracker.Data.Models.TransactionType", "TransactionType")
                        .WithMany("RecurringTransactions")
                        .HasForeignKey("TransactionTypeId")
                        .IsRequired()
                        .HasConstraintName("FK_RecurringTransaction_ToTransactionType");

                    b.Navigation("Account");

                    b.Navigation("Category");

                    b.Navigation("TransactionType");
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.Security", b =>
                {
                    b.HasOne("FinanceTracker.Data.Models.Currency", "Currency")
                        .WithMany("Securities")
                        .HasForeignKey("CurrencyId")
                        .IsRequired()
                        .HasConstraintName("FK_Security_ToCurrency");

                    b.Navigation("Currency");
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.Transaction", b =>
                {
                    b.HasOne("FinanceTracker.Data.Models.Account", "Account")
                        .WithMany("Transactions")
                        .HasForeignKey("AccountId")
                        .IsRequired()
                        .HasConstraintName("FK_Transaction_ToAccount");

                    b.HasOne("FinanceTracker.Data.Models.TransactionCategory", "Category")
                        .WithMany("Transactions")
                        .HasForeignKey("CategoryId")
                        .IsRequired()
                        .HasConstraintName("FK_Transaction_ToTransactionCategory");

                    b.HasOne("FinanceTracker.Data.Models.TransactionType", "TransactionType")
                        .WithMany("Transactions")
                        .HasForeignKey("TransactionTypeId")
                        .IsRequired()
                        .HasConstraintName("FK_Transaction_ToTransactionType");

                    b.Navigation("Account");

                    b.Navigation("Category");

                    b.Navigation("TransactionType");
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.TransactionSplit", b =>
                {
                    b.HasOne("FinanceTracker.Data.Models.TransactionCategory", "Category")
                        .WithMany("TransactionSplits")
                        .HasForeignKey("CategoryId")
                        .IsRequired()
                        .HasConstraintName("FK_TransactionSplit_ToTransactionCategory");

                    b.HasOne("FinanceTracker.Data.Models.Transaction", "Transaction")
                        .WithMany("TransactionSplits")
                        .HasForeignKey("TransactionId")
                        .IsRequired()
                        .HasConstraintName("FK_TransactionSplit_ToTransaction");

                    b.Navigation("Category");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.Account", b =>
                {
                    b.Navigation("AccountPeriods");

                    b.Navigation("RecurringTransactions");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.AccountType", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.Currency", b =>
                {
                    b.Navigation("Accounts");

                    b.Navigation("BaseCurrencyRates");

                    b.Navigation("CounterCurrencyRates");

                    b.Navigation("Securities");
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.Security", b =>
                {
                    b.Navigation("Prices");
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.Transaction", b =>
                {
                    b.Navigation("TransactionSplits");
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.TransactionCategory", b =>
                {
                    b.Navigation("RecurringTransactions");

                    b.Navigation("TransactionSplits");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("FinanceTracker.Data.Models.TransactionType", b =>
                {
                    b.Navigation("RecurringTransactions");

                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
