using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceTracker.ApiService.Dtos;
using FinanceTracker.Data.Models;

namespace FinanceTracker.ApiService.Tests.Infrastructure
{
    /// <summary>
    /// Provides test data creation utilities for integration tests
    /// </summary>
    public static class TestDataSeeder
    {
        /// <summary>
        /// Creates a basic account type for testing
        /// </summary>
        public static AccountType CreateTestAccountType(string typeName = "Test Account Type")
        {
            return new AccountType
            {
                Type = typeName
            };
        }

        /// <summary>
        /// Creates a basic currency for testing
        /// </summary>
        public static Currency CreateTestCurrency(string code = "USD", string symbol = "$")
        {
            return new Currency
            {
                Name = $"Test {code}",
                Symbol = symbol,
                DisplaySymbol = symbol
            };
        }

        /// <summary>
        /// Creates a basic transaction category for testing
        /// </summary>
        public static TransactionCategory CreateTestTransactionCategory(string name = "Test Category")
        {
            return new TransactionCategory
            {
                Category = name,
                Description = $"Test category for {name}"
            };
        }

        /// <summary>
        /// Creates a test cash account request DTO
        /// </summary>
        public static CreateCashAccountRequestDto CreateTestCashAccountRequest(
            string name = "Test Cash Account",
            int accountTypeId = 1,
            int currencyId = 1,
            decimal openingBalance = 1000.00m)
        {
            return new CreateCashAccountRequestDto
            {
                Name = name,
                Institution = "Test Bank",
                AccountTypeId = accountTypeId,
                CurrencyId = currencyId,
                InitialBalance = openingBalance,
                OverdraftLimit = 500.00m,
                IsActive = true
            };
        }

        /// <summary>
        /// Creates a test investment account request DTO
        /// </summary>
        public static CreateInvestmentAccountRequestDto CreateTestInvestmentAccountRequest(
            string name = "Test Investment Account",
            int accountTypeId = 1,
            int currencyId = 1,
            decimal openingBalance = 5000.00m)
        {
            return new CreateInvestmentAccountRequestDto
            {
                Name = name,
                Institution = "Test Broker Institution",
                AccountTypeId = accountTypeId,
                CurrencyId = currencyId,
                InitialBalance = openingBalance,
                BrokerAccountNumber = "TEST123456",
                IsTaxAdvantaged = false,
                IsActive = true
            };
        }

        /// <summary>
        /// Creates a test cash transaction request DTO
        /// </summary>
        public static CreateCashTransactionRequestDto CreateTestCashTransactionRequest(
            int accountPeriodId = 1,
            int categoryId = 1,
            decimal amount = 100.00m,
            DateOnly? transactionDate = null)
        {
            return new CreateCashTransactionRequestDto
            {
                AccountPeriodId = accountPeriodId,
                CategoryId = categoryId,
                Amount = amount,
                TransactionDate = transactionDate ?? DateOnly.FromDateTime(DateTime.Today),
                Description = "Test transaction",
                TransactionTypeId = 1 // Default transaction type
            };
        }

        /// <summary>
        /// Creates a test investment transaction request DTO
        /// </summary>
        public static CreateInvestmentTransactionRequestDto CreateTestInvestmentTransactionRequest(
            int accountPeriodId = 1,
            int securityId = 1,
            decimal quantity = 10.0m,
            decimal price = 50.00m,
            DateOnly? transactionDate = null)
        {
            return new CreateInvestmentTransactionRequestDto
            {
                AccountPeriodId = accountPeriodId,
                SecurityId = securityId,
                CategoryId = 1, // Default category
                TransactionTypeId = 1, // Default transaction type
                Quantity = quantity,
                Price = price,
                TransactionDate = transactionDate ?? DateOnly.FromDateTime(DateTime.Today),
                Description = "Test investment transaction",
                Fees = 0,
                Commission = 0
            };
        }

        /// <summary>
        /// Creates a test transaction split request DTO
        /// </summary>
        public static CreateTransactionSplitRequestDto CreateTestTransactionSplitRequest(
            int transactionId = 1,
            int categoryId = 1,
            decimal amount = 50.00m)
        {
            return new CreateTransactionSplitRequestDto
            {
                CashTransactionId = transactionId,
                CategoryId = categoryId,
                Amount = amount,
                Description = "Test transaction split"
            };
        }

        /// <summary>
        /// Creates a list of test account types with common values
        /// </summary>
        public static List<AccountType> CreateTestAccountTypes()
        {
            return new List<AccountType>
            {
                CreateTestAccountType("Checking"),
                CreateTestAccountType("Savings"),
                CreateTestAccountType("Investment"),
                CreateTestAccountType("Credit Card")
            };
        }

        /// <summary>
        /// Creates a list of test currencies with common values
        /// </summary>
        public static List<Currency> CreateTestCurrencies()
        {
            return new List<Currency>
            {
                CreateTestCurrency("USD", "$"),
                CreateTestCurrency("EUR", "€"),
                CreateTestCurrency("GBP", "£"),
                CreateTestCurrency("CAD", "C$")
            };
        }

        /// <summary>
        /// Creates a list of test transaction categories
        /// </summary>
        public static List<TransactionCategory> CreateTestTransactionCategories()
        {
            return new List<TransactionCategory>
            {
                CreateTestTransactionCategory("Food & Dining"),
                CreateTestTransactionCategory("Transportation"),
                CreateTestTransactionCategory("Shopping"),
                CreateTestTransactionCategory("Entertainment"),
                CreateTestTransactionCategory("Income"),
                CreateTestTransactionCategory("Bills & Utilities")
            };
        }
    }
}
