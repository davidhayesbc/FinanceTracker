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
    /// Ensures all reference data is seeded in the database for tests
    /// This method will create the reference data using API calls if it doesn't exist
    /// </summary>
    /// <param name="httpClient">HTTP client for API calls</param>
    /// <returns>Task</returns>
    public static async Task EnsureReferenceDataSeededAsync(HttpClient httpClient)
    {
        Console.WriteLine("EnsureReferenceDataSeededAsync: Starting reference data seeding...");

        try
        {
            await EnsureAccountTypesSeededAsync(httpClient);
            await EnsureTransactionTypesSeededAsync(httpClient);
            await EnsureTransactionCategoriesSeededAsync(httpClient);

            Console.WriteLine("EnsureReferenceDataSeededAsync: All reference data seeding completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EnsureReferenceDataSeededAsync: Error during reference data seeding: {ex.Message}");
            throw new InvalidOperationException($"Failed to ensure reference data is seeded: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Ensures account types are seeded by checking if they exist and creating them if they don't
    /// </summary>
    private static async Task EnsureAccountTypesSeededAsync(HttpClient httpClient)
    {
        try
        {
            var response = await httpClient.GetAsync("/api/v1/accountTypes");

            if (response.IsSuccessStatusCode)
            {
                var accountTypes = await response.Content.ReadFromJsonAsync<List<AccountType>>();

                if (accountTypes?.Count > 0)
                {
                    Console.WriteLine($"EnsureAccountTypesSeededAsync: Found {accountTypes.Count} existing account types: [{string.Join(", ", accountTypes.Select(at => at.Type))}]");
                    return; // Already seeded
                }
            }
            else
            {
                Console.WriteLine($"EnsureAccountTypesSeededAsync: Failed to check existing account types. Status: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EnsureAccountTypesSeededAsync: Error checking existing account types: {ex.Message}");
        }

        Console.WriteLine("EnsureAccountTypesSeededAsync: No account types found, creating them...");

        // Create the standard account types used in tests
        var accountTypesToCreate = new[]
        {
            AccountTypes.Savings,
            AccountTypes.Current,
            AccountTypes.Investment,
            AccountTypes.Property,
            AccountTypes.Mortgage,
            AccountTypes.CreditCard,
            AccountTypes.LineOfCredit
        };

        foreach (var accountTypeName in accountTypesToCreate)
        {
            var createRequest = new CreateAccountTypeRequestDto
            {
                Type = accountTypeName
            };

            try
            {
                Console.WriteLine($"EnsureAccountTypesSeededAsync: Creating account type '{accountTypeName}'...");
                var response = await httpClient.PostAsJsonAsync("/api/v1/accountTypes", createRequest);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"EnsureAccountTypesSeededAsync: Successfully created account type '{accountTypeName}'");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    Console.WriteLine($"EnsureAccountTypesSeededAsync: Account type '{accountTypeName}' already exists");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"EnsureAccountTypesSeededAsync: Failed to create account type '{accountTypeName}'. Status: {response.StatusCode}, Content: {errorContent}");
                }
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("409") || ex.Message.Contains("Conflict"))
            {
                Console.WriteLine($"EnsureAccountTypesSeededAsync: Account type '{accountTypeName}' already exists (HttpRequestException)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EnsureAccountTypesSeededAsync: Error creating account type '{accountTypeName}': {ex.Message}");

                // Wait a bit and retry once
                await Task.Delay(1000);
                try
                {
                    Console.WriteLine($"EnsureAccountTypesSeededAsync: Retrying creation of account type '{accountTypeName}'...");
                    var retryResponse = await httpClient.PostAsJsonAsync("/api/v1/accountTypes", createRequest);

                    if (retryResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"EnsureAccountTypesSeededAsync: Successfully created account type '{accountTypeName}' on retry");
                    }
                    else
                    {
                        var errorContent = await retryResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"EnsureAccountTypesSeededAsync: Failed to create account type '{accountTypeName}' on retry. Status: {retryResponse.StatusCode}, Content: {errorContent}");
                    }
                }
                catch (Exception retryEx)
                {
                    throw new InvalidOperationException($"Failed to create account type '{accountTypeName}' after retry. Original error: {ex.Message}, Retry error: {retryEx.Message}", retryEx);
                }
            }
        }

        Console.WriteLine("EnsureAccountTypesSeededAsync: Finished creating account types");
    }

    /// <summary>
    /// Ensures transaction types are seeded by checking if they exist and creating them if they don't
    /// </summary>
    private static async Task EnsureTransactionTypesSeededAsync(HttpClient httpClient)
    {
        try
        {
            var response = await httpClient.GetAsync("/api/v1/transactionTypes");

            if (response.IsSuccessStatusCode)
            {
                var transactionTypes = await response.Content.ReadFromJsonAsync<List<TransactionType>>();

                if (transactionTypes?.Count > 0)
                {
                    Console.WriteLine($"EnsureTransactionTypesSeededAsync: Found {transactionTypes.Count} existing transaction types: [{string.Join(", ", transactionTypes.Select(tt => tt.Type))}]");
                    return; // Already seeded
                }
            }
            else
            {
                Console.WriteLine($"EnsureTransactionTypesSeededAsync: Failed to check existing transaction types. Status: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EnsureTransactionTypesSeededAsync: Error checking existing transaction types: {ex.Message}");
        }

        Console.WriteLine("EnsureTransactionTypesSeededAsync: No transaction types found, creating them...");

        // Create the standard transaction types used in tests
        var transactionTypesToCreate = new[]
        {
            TransactionTypes.Buy,
            TransactionTypes.Sell,
            TransactionTypes.Short,
            TransactionTypes.Cover,
            TransactionTypes.Deposit,
            TransactionTypes.Withdrawal,
            TransactionTypes.Purchase,
            TransactionTypes.Refund
        };

        foreach (var transactionTypeName in transactionTypesToCreate)
        {
            var createRequest = new CreateTransactionTypeRequestDto
            {
                Type = transactionTypeName
            };

            try
            {
                Console.WriteLine($"EnsureTransactionTypesSeededAsync: Creating transaction type '{transactionTypeName}'...");
                var response = await httpClient.PostAsJsonAsync("/api/v1/transactionTypes", createRequest);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"EnsureTransactionTypesSeededAsync: Successfully created transaction type '{transactionTypeName}'");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    Console.WriteLine($"EnsureTransactionTypesSeededAsync: Transaction type '{transactionTypeName}' already exists");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"EnsureTransactionTypesSeededAsync: Failed to create transaction type '{transactionTypeName}'. Status: {response.StatusCode}, Content: {errorContent}");
                }
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("409") || ex.Message.Contains("Conflict"))
            {
                Console.WriteLine($"EnsureTransactionTypesSeededAsync: Transaction type '{transactionTypeName}' already exists (HttpRequestException)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EnsureTransactionTypesSeededAsync: Error creating transaction type '{transactionTypeName}': {ex.Message}");

                // Wait a bit and retry once
                await Task.Delay(1000);
                try
                {
                    Console.WriteLine($"EnsureTransactionTypesSeededAsync: Retrying creation of transaction type '{transactionTypeName}'...");
                    var retryResponse = await httpClient.PostAsJsonAsync("/api/v1/transactionTypes", createRequest);

                    if (retryResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"EnsureTransactionTypesSeededAsync: Successfully created transaction type '{transactionTypeName}' on retry");
                    }
                    else
                    {
                        var errorContent = await retryResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"EnsureTransactionTypesSeededAsync: Failed to create transaction type '{transactionTypeName}' on retry. Status: {retryResponse.StatusCode}, Content: {errorContent}");
                    }
                }
                catch (Exception retryEx)
                {
                    throw new InvalidOperationException($"Failed to create transaction type '{transactionTypeName}' after retry. Original error: {ex.Message}, Retry error: {retryEx.Message}", retryEx);
                }
            }
        }

        Console.WriteLine("EnsureTransactionTypesSeededAsync: Finished creating transaction types");
    }

    /// <summary>
    /// Ensures transaction categories are seeded by checking if they exist and creating them if they don't
    /// </summary>
    private static async Task EnsureTransactionCategoriesSeededAsync(HttpClient httpClient)
    {
        try
        {
            var response = await httpClient.GetAsync("/api/v1/transactionCategories");

            if (response.IsSuccessStatusCode)
            {
                var categories = await response.Content.ReadFromJsonAsync<List<TransactionCategory>>();

                if (categories?.Count > 0)
                {
                    Console.WriteLine($"EnsureTransactionCategoriesSeededAsync: Found {categories.Count} existing transaction categories: [{string.Join(", ", categories.Select(tc => tc.Category))}]");
                    return; // Already seeded
                }
            }
            else
            {
                Console.WriteLine($"EnsureTransactionCategoriesSeededAsync: Failed to check existing transaction categories. Status: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EnsureTransactionCategoriesSeededAsync: Error checking existing transaction categories: {ex.Message}");
        }

        Console.WriteLine("EnsureTransactionCategoriesSeededAsync: No transaction categories found, creating them...");

        // Create the standard transaction categories used in tests
        var categoriesToCreate = new[]
        {
            new { Category = Categories.Groceries.Category, Description = Categories.Groceries.Description },
            new { Category = Categories.Restaurants.Category, Description = Categories.Restaurants.Description },
            new { Category = Categories.Entertainment.Category, Description = Categories.Entertainment.Description },
            new { Category = Categories.Utilities.Category, Description = Categories.Utilities.Description },
            new { Category = Categories.Health.Category, Description = Categories.Health.Description }
        };

        foreach (var categoryData in categoriesToCreate)
        {
            var createRequest = new CreateTransactionCategoryRequestDto
            {
                Category = categoryData.Category,
                Description = categoryData.Description
            };

            try
            {
                Console.WriteLine($"EnsureTransactionCategoriesSeededAsync: Creating transaction category '{categoryData.Category}'...");
                var response = await httpClient.PostAsJsonAsync("/api/v1/transactionCategories", createRequest);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"EnsureTransactionCategoriesSeededAsync: Successfully created transaction category '{categoryData.Category}'");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    Console.WriteLine($"EnsureTransactionCategoriesSeededAsync: Transaction category '{categoryData.Category}' already exists");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"EnsureTransactionCategoriesSeededAsync: Failed to create transaction category '{categoryData.Category}'. Status: {response.StatusCode}, Content: {errorContent}");
                }
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("409") || ex.Message.Contains("Conflict"))
            {
                Console.WriteLine($"EnsureTransactionCategoriesSeededAsync: Transaction category '{categoryData.Category}' already exists (HttpRequestException)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EnsureTransactionCategoriesSeededAsync: Error creating transaction category '{categoryData.Category}': {ex.Message}");

                // Wait a bit and retry once
                await Task.Delay(1000);
                try
                {
                    Console.WriteLine($"EnsureTransactionCategoriesSeededAsync: Retrying creation of transaction category '{categoryData.Category}'...");
                    var retryResponse = await httpClient.PostAsJsonAsync("/api/v1/transactionCategories", createRequest);

                    if (retryResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"EnsureTransactionCategoriesSeededAsync: Successfully created transaction category '{categoryData.Category}' on retry");
                    }
                    else
                    {
                        var errorContent = await retryResponse.Content.ReadAsStringAsync();
                        Console.WriteLine($"EnsureTransactionCategoriesSeededAsync: Failed to create transaction category '{categoryData.Category}' on retry. Status: {retryResponse.StatusCode}, Content: {errorContent}");
                    }
                }
                catch (Exception retryEx)
                {
                    throw new InvalidOperationException($"Failed to create transaction category '{categoryData.Category}' after retry. Original error: {ex.Message}, Retry error: {retryEx.Message}", retryEx);
                }
            }
        }

        Console.WriteLine("EnsureTransactionCategoriesSeededAsync: Finished creating transaction categories");
    }
}
