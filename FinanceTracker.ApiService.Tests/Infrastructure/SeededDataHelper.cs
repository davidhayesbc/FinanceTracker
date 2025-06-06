using FinanceTracker.Data.Models;
using FinanceTracker.ApiService.Dtos;
using System.Net.Http.Json;
using System.Text.Json;

namespace FinanceTracker.ApiService.Tests.Infrastructure;

/// <summary>
/// Helper class providing access to seeded reference data for integration tests.
/// This data matches the seeding logic in FinanceTracker.MigrationService.Worker.cs
/// </summary>
public static class SeededDataHelper
{
    /// <summary>
    /// Account types seeded by the migration service
    /// </summary>
    public static class AccountTypes
    {
        public const string Savings = "Savings";
        public const string Current = "Current";
        public const string Investment = "Investment";
        public const string Property = "Property";
        public const string Mortgage = "Mortgage";
        public const string CreditCard = "Credit Card";
        public const string LineOfCredit = "Line of Credit";
    }

    /// <summary>
    /// Transaction types seeded by the migration service
    /// </summary>
    public static class TransactionTypes
    {
        public const string Buy = "Buy";
        public const string Sell = "Sell";
        public const string Short = "Short";
        public const string Cover = "Cover";
        public const string Deposit = "Deposit";
        public const string Withdrawal = "Withdrawal";
        public const string Purchase = "Purchase";
        public const string Refund = "Refund";
    }

    /// <summary>
    /// Currencies seeded by the migration service
    /// </summary>
    public static class Currencies
    {
        public static class CAD
        {
            public const string Name = "Canadian Dollar";
            public const string Symbol = "CAD";
            public const string DisplaySymbol = "$";
        }

        public static class USD
        {
            public const string Name = "US Dollar";
            public const string Symbol = "USD";
            public const string DisplaySymbol = "$";
        }
    }

    /// <summary>
    /// Securities seeded by the migration service
    /// </summary>
    public static class Securities
    {
        public static class CAD_CASH
        {
            public const string Name = "Canadian Cash";
            public const string Symbol = "CAD-CASH";
            public const string SecurityType = "Cash";
        }

        public static class USD_CASH
        {
            public const string Name = "US Cash";
            public const string Symbol = "USD-CASH";
            public const string SecurityType = "Cash";
        }
    }

    /// <summary>
    /// Transaction categories seeded by the migration service
    /// </summary>
    public static class Categories
    {
        public static class Groceries
        {
            public const string Category = "Groceries";
            public const string Description = "Expenses related to grocery shopping";
        }

        public static class Restaurants
        {
            public const string Category = "Restaurants";
            public const string Description = "Expenses related to dining out";
        }

        public static class Entertainment
        {
            public const string Category = "Entertainment";
            public const string Description = "Expenses related to entertainment";
        }

        public static class Utilities
        {
            public const string Category = "Utilities";
            public const string Description = "Expenses related to utilities";
        }

        public static class Health
        {
            public const string Category = "Health";
            public const string Description = "Expenses related to health";
        }
    }

    /// <summary>
    /// Gets the ID of an account type by name using HTTP API call
    /// </summary>
    /// <param name="httpClient">HTTP client for API calls</param>
    /// <param name="typeName">Account type name</param>
    /// <returns>Account type ID</returns>
    public static async Task<int> GetAccountTypeIdAsync(HttpClient httpClient, string typeName)
    {
        try
        {
            var response = await httpClient.GetAsync("/api/v1/accountTypes");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to retrieve account types from API. Status: {response.StatusCode}, Content: {errorContent}");
            }

            var accountTypes = await response.Content.ReadFromJsonAsync<List<AccountType>>();

            if (accountTypes == null)
                throw new InvalidOperationException("Failed to retrieve account types from API - response was null");

            var accountType = accountTypes.FirstOrDefault(at => at.Type == typeName);

            if (accountType == null)
            {
                var availableTypes = string.Join(", ", accountTypes.Select(at => at.Type));
                throw new InvalidOperationException($"Account type '{typeName}' not found. Available types: [{availableTypes}]. Ensure database is properly seeded.");
            }

            return accountType.Id;
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            throw new InvalidOperationException($"Error retrieving account type '{typeName}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets the ID of a transaction type by name using HTTP API call
    /// </summary>
    /// <param name="httpClient">HTTP client for API calls</param>
    /// <param name="typeName">Transaction type name</param>
    /// <returns>Transaction type ID</returns>
    public static async Task<int> GetTransactionTypeIdAsync(HttpClient httpClient, string typeName)
    {
        try
        {
            var response = await httpClient.GetAsync("/api/v1/transactionTypes");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to retrieve transaction types from API. Status: {response.StatusCode}, Content: {errorContent}");
            }

            var transactionTypes = await response.Content.ReadFromJsonAsync<List<TransactionType>>();

            if (transactionTypes == null)
                throw new InvalidOperationException("Failed to retrieve transaction types from API - response was null");

            var transactionType = transactionTypes.FirstOrDefault(tt => tt.Type == typeName);

            if (transactionType == null)
            {
                var availableTypes = string.Join(", ", transactionTypes.Select(tt => tt.Type));
                throw new InvalidOperationException($"Transaction type '{typeName}' not found. Available types: [{availableTypes}]. Ensure database is properly seeded.");
            }

            return transactionType.Id;
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            throw new InvalidOperationException($"Error retrieving transaction type '{typeName}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets the ID of a transaction category by name using HTTP API call
    /// </summary>
    /// <param name="httpClient">HTTP client for API calls</param>
    /// <param name="categoryName">Category name</param>
    /// <returns>Transaction category ID</returns>
    public static async Task<int> GetTransactionCategoryIdAsync(HttpClient httpClient, string categoryName)
    {
        try
        {
            var response = await httpClient.GetAsync("/api/v1/transactionCategories");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to retrieve transaction categories from API. Status: {response.StatusCode}, Content: {errorContent}");
            }

            var categories = await response.Content.ReadFromJsonAsync<List<TransactionCategory>>();

            if (categories == null)
                throw new InvalidOperationException("Failed to retrieve transaction categories from API - response was null");

            var category = categories.FirstOrDefault(tc => tc.Category == categoryName);

            if (category == null)
            {
                var availableCategories = string.Join(", ", categories.Select(tc => tc.Category));
                throw new InvalidOperationException($"Transaction category '{categoryName}' not found. Available categories: [{availableCategories}]. Ensure database is properly seeded.");
            }

            return category.Id;
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            throw new InvalidOperationException($"Error retrieving transaction category '{categoryName}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets all seeded account types using HTTP API call
    /// </summary>
    /// <param name="httpClient">HTTP client for API calls</param>
    /// <returns>List of account types</returns>
    public static async Task<List<AccountType>> GetAllAccountTypesAsync(HttpClient httpClient)
    {
        var accountTypes = await httpClient.GetFromJsonAsync<List<AccountType>>("/api/v1/accountTypes");
        return accountTypes ?? new List<AccountType>();
    }

    /// <summary>
    /// Gets all seeded transaction types using HTTP API call
    /// </summary>
    /// <param name="httpClient">HTTP client for API calls</param>
    /// <returns>List of transaction types</returns>
    public static async Task<List<TransactionType>> GetAllTransactionTypesAsync(HttpClient httpClient)
    {
        var transactionTypes = await httpClient.GetFromJsonAsync<List<TransactionType>>("/api/v1/transactionTypes");
        return transactionTypes ?? new List<TransactionType>();
    }

    /// <summary>
    /// Gets all seeded transaction categories using HTTP API call
    /// </summary>
    /// <param name="httpClient">HTTP client for API calls</param>
    /// <returns>List of transaction categories</returns>
    public static async Task<List<TransactionCategory>> GetAllTransactionCategoriesAsync(HttpClient httpClient)
    {
        var categories = await httpClient.GetFromJsonAsync<List<TransactionCategory>>("/api/v1/transactionCategories");
        return categories ?? new List<TransactionCategory>();
    }

    /// <summary>
    /// Gets the ID of a currency by symbol using HTTP API call
    /// </summary>
    /// <param name="httpClient">HTTP client for API calls</param>
    /// <param name="symbol">Currency symbol (e.g., CAD or USD)</param>
    /// <returns>Currency ID</returns>
    public static async Task<int> GetCurrencyIdAsync(HttpClient httpClient, string symbol)
    {
        try
        {
            var response = await httpClient.GetAsync("/api/v1/currencies");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to retrieve currencies from API. Status: {response.StatusCode}, Content: {errorContent}");
            }

            var currencies = await response.Content.ReadFromJsonAsync<List<Currency>>();

            if (currencies == null)
                throw new InvalidOperationException("Failed to retrieve currencies from API - response was null");

            var currency = currencies.FirstOrDefault(c => c.Symbol == symbol);

            if (currency == null)
            {
                var availableCurrencies = string.Join(", ", currencies.Select(c => c.Symbol));
                throw new InvalidOperationException($"Currency '{symbol}' not found. Available currencies: [{availableCurrencies}]. Ensure database is properly seeded.");
            }

            return currency.Id;
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            throw new InvalidOperationException($"Error retrieving currency '{symbol}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets the known ID of a currency by symbol for testing purposes.
    /// This method returns hardcoded IDs based on the expected seeding order.
    /// CAD is seeded first (ID=1), USD is seeded second (ID=2).
    /// </summary>
    /// <param name="symbol">Currency symbol (e.g., CAD or USD)</param>
    /// <returns>Currency ID</returns>
    /// <exception cref="ArgumentException">Thrown when the currency symbol is not recognized</exception>
    public static int GetKnownCurrencyId(string symbol)
    {
        return symbol switch
        {
            "CAD" => 1, // CAD is seeded first in SeedCurrenciesAsync
            "USD" => 2, // USD is seeded second in SeedCurrenciesAsync
            _ => throw new ArgumentException($"Unknown currency symbol: {symbol}. Only CAD and USD are supported in tests.", nameof(symbol))
        };
    }

    /// <summary>
    /// Ensures all reference data is available in the database for tests.
    /// Since we're using the containerized SQL Server with migration service,
    /// the data should already be seeded. This method verifies the data exists.
    /// </summary>
    /// <param name="httpClient">HTTP client for API calls</param>
    /// <returns>Task</returns>
    public static async Task EnsureReferenceDataSeededAsync(HttpClient httpClient)
    {
        Console.WriteLine("EnsureReferenceDataSeededAsync: Verifying reference data exists...");

        try
        {
            await VerifyReferenceDataExistsAsync(httpClient);
            Console.WriteLine("EnsureReferenceDataSeededAsync: All reference data verification completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EnsureReferenceDataSeededAsync: Error during reference data verification: {ex.Message}");
            throw new InvalidOperationException($"Failed to verify reference data exists: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Verifies that all expected reference data exists in the database
    /// </summary>
    private static async Task VerifyReferenceDataExistsAsync(HttpClient httpClient)
    {
        // Verify account types exist
        var accountTypes = await GetAllAccountTypesAsync(httpClient);
        if (accountTypes.Count == 0)
        {
            throw new InvalidOperationException("No account types found in database. Migration service may not have completed properly.");
        }
        Console.WriteLine($"EnsureReferenceDataSeededAsync: Verified {accountTypes.Count} account types exist: [{string.Join(", ", accountTypes.Select(at => at.Type))}]");

        // Verify transaction types exist
        var transactionTypes = await GetAllTransactionTypesAsync(httpClient);
        if (transactionTypes.Count == 0)
        {
            throw new InvalidOperationException("No transaction types found in database. Migration service may not have completed properly.");
        }
        Console.WriteLine($"EnsureReferenceDataSeededAsync: Verified {transactionTypes.Count} transaction types exist: [{string.Join(", ", transactionTypes.Select(tt => tt.Type))}]");

        // Verify transaction categories exist
        var categories = await GetAllTransactionCategoriesAsync(httpClient);
        if (categories.Count == 0)
        {
            throw new InvalidOperationException("No transaction categories found in database. Migration service may not have completed properly.");
        }
        Console.WriteLine($"EnsureReferenceDataSeededAsync: Verified {categories.Count} transaction categories exist: [{string.Join(", ", categories.Select(tc => tc.Category))}]");
    }
}
