using FinanceTracker.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.ApiService.Tests.Integration;

/// <summary>
/// Test data builder for creating test entities
/// </summary>
public static class TestDataBuilder
{
    public static Currency CreateCurrency(string name = "US Dollar", string symbol = "$", string displaySymbol = "$")
    {
        return new Currency
        {
            Name = name,
            Symbol = symbol,
            DisplaySymbol = displaySymbol
        };
    }

    public static AccountType CreateAccountType(string type = "Checking")
    {
        return new AccountType
        {
            Type = type
        };
    }

    public static TransactionType CreateTransactionType(string type = "Income")
    {
        return new TransactionType
        {
            Type = type
        };
    }

    public static TransactionCategory CreateTransactionCategory(string category = "Salary")
    {
        return new TransactionCategory
        {
            Category = category
        };
    }

    public static CashAccount CreateCashAccount(
        string name = "Test Checking Account",
        int accountTypeId = 1,
        int currencyId = 1,
        string institution = "Test Bank",
        decimal? overdraftLimit = null)
    {
        return new CashAccount
        {
            Name = name,
            AccountTypeId = accountTypeId,
            CurrencyId = currencyId,
            Institution = institution,
            IsActive = true,
            OverdraftLimit = overdraftLimit
        };
    }

    public static InvestmentAccount CreateInvestmentAccount(
        string name = "Test Investment Account",
        int accountTypeId = 2,
        int currencyId = 1,
        string institution = "Test Broker",
        string? brokerAccountNumber = "12345",
        bool isTaxAdvantaged = false,
        string? taxAdvantageType = null)
    {
        return new InvestmentAccount
        {
            Name = name,
            AccountTypeId = accountTypeId,
            CurrencyId = currencyId,
            Institution = institution,
            IsActive = true,
            BrokerAccountNumber = brokerAccountNumber,
            IsTaxAdvantaged = isTaxAdvantaged,
            TaxAdvantageType = taxAdvantageType
        };
    }

    public static AccountPeriod CreateAccountPeriod(
        int accountId,
        decimal openingBalance = 1000m,
        DateOnly? periodStart = null,
        DateOnly? periodEnd = null,
        decimal? closingBalance = null,
        DateOnly? periodCloseDate = null,
        AccountBase? account = null)
    {
        return new AccountPeriod
        {
            AccountId = accountId,
            OpeningBalance = openingBalance,
            PeriodStart = periodStart ?? DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)),
            PeriodEnd = periodEnd,
            ClosingBalance = closingBalance,
            PeriodCloseDate = periodCloseDate,
            Account = account ?? CreateCashAccount("Test Account") // Provide a default account
        };
    }

    public static CashTransaction CreateCashTransaction(
        int accountPeriodId,
        decimal amount = 100m,
        DateOnly? transactionDate = null,
        string description = "Test Transaction",
        int transactionTypeId = 1,
        int categoryId = 1,
        int? transferToCashAccountId = null)
    {
        return new CashTransaction
        {
            AccountPeriodId = accountPeriodId,
            Amount = amount,
            TransactionDate = transactionDate ?? DateOnly.FromDateTime(DateTime.Today),
            Description = description,
            TransactionTypeId = transactionTypeId,
            CategoryId = categoryId,
            TransferToCashAccountId = transferToCashAccountId
        };
    }

    public static Security CreateSecurity(
        string name = "Test Stock",
        string symbol = "TEST",
        string isin = "US1234567890",
        int currencyId = 1,
        string securityType = "Stock")
    {
        return new Security
        {
            Name = name,
            Symbol = symbol,
            ISIN = isin,
            CurrencyId = currencyId,
            SecurityType = securityType
        };
    }

    public static InvestmentTransaction CreateInvestmentTransaction(
        int accountPeriodId,
        int securityId,
        decimal quantity = 10m,
        decimal price = 50m,
        DateOnly? transactionDate = null,
        string description = "Test Investment Transaction",
        int transactionTypeId = 1,
        int categoryId = 1,
        decimal? fees = null,
        decimal? commission = null,
        string? orderType = "Market")
    {
        return new InvestmentTransaction
        {
            AccountPeriodId = accountPeriodId,
            SecurityId = securityId,
            Quantity = quantity,
            Price = price,
            TransactionDate = transactionDate ?? DateOnly.FromDateTime(DateTime.Today),
            Description = description,
            TransactionTypeId = transactionTypeId,
            CategoryId = categoryId,
            Fees = fees,
            Commission = commission,
            OrderType = orderType
        };
    }

    public static TransactionSplit CreateTransactionSplit(
        int cashTransactionId,
        int categoryId,
        decimal amount)
    {
        return new TransactionSplit
        {
            CashTransactionId = cashTransactionId,
            CategoryId = categoryId,
            Amount = amount
        };
    }

    public static RecurringTransaction CreateRecurringTransaction(
        int cashAccountId,
        decimal amount = 100m,
        string description = "Test Recurring Transaction",
        string recurrenceCronExpression = "0 0 1 * *", // Monthly on 1st
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        decimal amountVariancePercentage = 0m,
        int transactionTypeId = 1,
        int categoryId = 1)
    {
        return new RecurringTransaction
        {
            CashAccountId = cashAccountId,
            Amount = amount,
            Description = description,
            RecurrenceCronExpression = recurrenceCronExpression,
            StartDate = startDate ?? DateOnly.FromDateTime(DateTime.Today),
            EndDate = endDate,
            AmountVariancePercentage = amountVariancePercentage,
            TransactionTypeId = transactionTypeId,
            CategoryId = categoryId
        };
    }

    /// <summary>
    /// Cleans all data from the database for test isolation
    /// </summary>
    /// <param name="context">The database context</param>
    public static async Task CleanDatabase(FinanceTackerDbContext context)
    {
        // Remove data in dependency order
        context.InvestmentTransactions.RemoveRange(context.InvestmentTransactions);
        context.CashTransactions.RemoveRange(context.CashTransactions);
        context.TransactionSplits.RemoveRange(context.TransactionSplits);
        context.RecurringTransactions.RemoveRange(context.RecurringTransactions);
        context.AccountPeriods.RemoveRange(context.AccountPeriods);
        context.InvestmentAccounts.RemoveRange(context.InvestmentAccounts);
        context.CashAccounts.RemoveRange(context.CashAccounts);
        context.Securities.RemoveRange(context.Securities);
        context.TransactionCategories.RemoveRange(context.TransactionCategories);
        context.TransactionTypes.RemoveRange(context.TransactionTypes);
        context.AccountTypes.RemoveRange(context.AccountTypes);
        context.Currencies.RemoveRange(context.Currencies);
        
        await context.SaveChangesAsync();
    }
}
