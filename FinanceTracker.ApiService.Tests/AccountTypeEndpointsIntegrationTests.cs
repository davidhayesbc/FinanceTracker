using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FinanceTracker.ApiService.Dtos;
using FinanceTracker.ApiService.Tests.Infrastructure;
using FinanceTracker.Data.Models;

namespace FinanceTracker.ApiService.Tests;

/// <summary>
/// Integration tests for AccountType endpoints (/accountTypes)
/// Tests all CRUD operations for account types
/// </summary>
[Collection("AspireApplication")]
public class AccountTypeEndpointsIntegrationTests : SharedAspireIntegrationTestBase
{
    public AccountTypeEndpointsIntegrationTests(AspireApplicationFixture fixture) : base(fixture)
    {
    }

    /// <summary>
    /// Override to NOT seed reference data for AccountType tests
    /// since we want to test account type CRUD operations in isolation
    /// </summary>
    public override async Task InitializeAsync()
    {
        await ClearDatabaseAsync();
        // Intentionally NOT calling SeededDataHelper.EnsureReferenceDataSeededAsync
        // for these tests since we want to test CRUD operations in isolation
    }

    /// <summary>
    /// Clear database by calling delete endpoints for all entities
    /// </summary>
    private async Task ClearDatabaseAsync()
    {
        try
        {
            // Clear accounts first (they depend on account types)
            var accountsResponse = await HttpClient.GetAsync("/api/v1/accounts");
            if (accountsResponse.IsSuccessStatusCode)
            {
                var accounts = await accountsResponse.Content.ReadFromJsonAsync<List<AccountBaseDto>>();
                if (accounts != null)
                {
                    foreach (var account in accounts)
                    {
                        await HttpClient.DeleteAsync($"/api/v1/accounts/{account.Id}");
                    }
                }
            }

            // Clear account types
            var accountTypesResponse = await HttpClient.GetAsync("/api/v1/accountTypes");
            if (accountTypesResponse.IsSuccessStatusCode)
            {
                var accountTypes = await accountTypesResponse.Content.ReadFromJsonAsync<List<AccountType>>();
                if (accountTypes != null)
                {
                    foreach (var accountType in accountTypes)
                    {
                        await HttpClient.DeleteAsync($"/api/v1/accountTypes/{accountType.Id}");
                    }
                }
            }

            // Clear currencies
            var currenciesResponse = await HttpClient.GetAsync("/api/v1/currencies");
            if (currenciesResponse.IsSuccessStatusCode)
            {
                var currencies = await currenciesResponse.Content.ReadFromJsonAsync<List<CurrencyDto>>();
                if (currencies != null)
                {
                    foreach (var currency in currencies)
                    {
                        await HttpClient.DeleteAsync($"/api/v1/currencies/{currency.Id}");
                    }
                }
            }
        }
        catch (Exception)
        {
            // Ignore errors during cleanup - database might be empty
        }
    }

    #region Get All Account Types Tests

    [Fact]
    public async Task GetAllAccountTypes_WithNoAccountTypes_ReturnsEmptyList()
    {
        // Arrange
        // HttpClient is already available from base class

        // Act
        var response = await HttpClient.GetAsync("/api/v1/accountTypes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var accountTypes = await response.Content.ReadFromJsonAsync<List<AccountType>>();
        accountTypes.Should().NotBeNull();
        accountTypes.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAccountTypes_WithExistingAccountTypes_ReturnsAllAccountTypes()
    {
        // Arrange
        var createRequests = new[]
        {
            new CreateAccountTypeRequestDto { Type = "Checking" },
            new CreateAccountTypeRequestDto { Type = "Savings" },
            new CreateAccountTypeRequestDto { Type = "Investment" }
        };

        var createdAccountTypes = new List<AccountType>();
        foreach (var request in createRequests)
        {
            var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", request);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdAccountType = await createResponse.Content.ReadFromJsonAsync<AccountType>();
            createdAccountTypes.Add(createdAccountType!);
        }

        // Act
        var response = await HttpClient.GetAsync("/api/v1/accountTypes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var accountTypes = await response.Content.ReadFromJsonAsync<List<AccountType>>();
        accountTypes.Should().NotBeNull();
        accountTypes.Should().HaveCount(3);
        accountTypes.Should().Contain(at => at.Type == "Checking");
        accountTypes.Should().Contain(at => at.Type == "Savings");
        accountTypes.Should().Contain(at => at.Type == "Investment");
    }

    #endregion

    #region Get Account Type By ID Tests

    [Fact]
    public async Task GetAccountTypeById_WithValidId_ReturnsAccountType()
    {
        // Arrange
        var createRequest = new CreateAccountTypeRequestDto { Type = "Checking" };
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", createRequest);
        var createdAccountType = await createResponse.Content.ReadFromJsonAsync<AccountType>();

        // Act
        var response = await HttpClient.GetAsync($"/api/v1/accountTypes/{createdAccountType!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var accountType = await response.Content.ReadFromJsonAsync<AccountType>();
        accountType.Should().NotBeNull();
        accountType!.Id.Should().Be(createdAccountType.Id);
        accountType.Type.Should().Be("Checking");
    }

    [Fact]
    public async Task GetAccountTypeById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        const int invalidId = 999;

        // Act
        var response = await HttpClient.GetAsync($"/api/v1/accountTypes/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Create Account Type Tests

    [Fact]
    public async Task CreateAccountType_WithValidData_ReturnsCreatedAccountType()
    {
        // Arrange
        var request = new CreateAccountTypeRequestDto
        {
            Type = "Checking"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var accountType = await response.Content.ReadFromJsonAsync<AccountType>();
        accountType.Should().NotBeNull();
        accountType!.Id.Should().BeGreaterThan(0);
        accountType.Type.Should().Be("Checking");

        // Verify location header
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain($"/api/v1/accountTypes/{accountType.Id}");
    }

    [Fact]
    public async Task CreateAccountType_WithDuplicateType_ReturnsConflict()
    {
        // Arrange
        var request = new CreateAccountTypeRequestDto { Type = "Checking" };

        // Create first account type
        var firstResponse = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", request);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act - try to create duplicate
        var response = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateAccountType_WithNullRequest_ReturnsBadRequest()
    {
        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", (CreateAccountTypeRequestDto?)null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAccountType_WithEmptyType_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountTypeRequestDto { Type = "" };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAccountType_WithTooLongType_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateAccountTypeRequestDto
        {
            Type = new string('A', 51) // MaxLength is 50
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Update Account Type Tests

    [Fact]
    public async Task UpdateAccountType_WithValidData_ReturnsUpdatedAccountType()
    {
        // Arrange
        var createRequest = new CreateAccountTypeRequestDto { Type = "Checking" };
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", createRequest);
        var createdAccountType = await createResponse.Content.ReadFromJsonAsync<AccountType>();

        var updateRequest = new UpdateAccountTypeRequestDto { Type = "Updated Checking" };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1/accountTypes/{createdAccountType!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedAccountType = await response.Content.ReadFromJsonAsync<AccountType>();
        updatedAccountType.Should().NotBeNull();
        updatedAccountType!.Id.Should().Be(createdAccountType.Id);
        updatedAccountType.Type.Should().Be("Updated Checking");
    }

    [Fact]
    public async Task UpdateAccountType_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        const int invalidId = 999;
        var updateRequest = new UpdateAccountTypeRequestDto { Type = "Updated Type" };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1/accountTypes/{invalidId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateAccountType_WithDuplicateType_ReturnsBadRequest()
    {
        // Arrange
        // Create two account types
        var createRequest1 = new CreateAccountTypeRequestDto { Type = "Checking" };
        var createResponse1 = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", createRequest1);
        var accountType1 = await createResponse1.Content.ReadFromJsonAsync<AccountType>();

        var createRequest2 = new CreateAccountTypeRequestDto { Type = "Savings" };
        var createResponse2 = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", createRequest2);
        var accountType2 = await createResponse2.Content.ReadFromJsonAsync<AccountType>();

        // Try to update accountType2 to have the same type as accountType1
        var updateRequest = new UpdateAccountTypeRequestDto { Type = "Checking" };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1/accountTypes/{accountType2!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAccountType_WithNullRequest_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new CreateAccountTypeRequestDto { Type = "Checking" };
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", createRequest);
        var createdAccountType = await createResponse.Content.ReadFromJsonAsync<AccountType>();

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1/accountTypes/{createdAccountType!.Id}", (UpdateAccountTypeRequestDto?)null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAccountType_WithEmptyType_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new CreateAccountTypeRequestDto { Type = "Checking" };
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", createRequest);
        var createdAccountType = await createResponse.Content.ReadFromJsonAsync<AccountType>();

        var updateRequest = new UpdateAccountTypeRequestDto { Type = "" };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1/accountTypes/{createdAccountType!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAccountType_WithTooLongType_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new CreateAccountTypeRequestDto { Type = "Checking" };
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", createRequest);
        var createdAccountType = await createResponse.Content.ReadFromJsonAsync<AccountType>();

        var updateRequest = new UpdateAccountTypeRequestDto
        {
            Type = new string('A', 51) // MaxLength is 50
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1/accountTypes/{createdAccountType!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Delete Account Type Tests

    [Fact]
    public async Task DeleteAccountType_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var createRequest = new CreateAccountTypeRequestDto { Type = "Checking" };
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", createRequest);
        var createdAccountType = await createResponse.Content.ReadFromJsonAsync<AccountType>();

        // Act
        var response = await HttpClient.DeleteAsync($"/api/v1/accountTypes/{createdAccountType!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the account type is actually deleted
        var getResponse = await HttpClient.GetAsync($"/api/v1/accountTypes/{createdAccountType.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAccountType_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        const int invalidId = 999;

        // Act
        var response = await HttpClient.DeleteAsync($"/api/v1/accountTypes/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAccountType_WhenUsedByAccounts_ReturnsBadRequest()
    {
        // Arrange
        // First create necessary reference data
        var currencyRequest = new CreateCurrencyRequestDto
        {
            Name = "Japanese Yen",
            Symbol = "JPY",
            DisplaySymbol = "¥"
        };
        var currencyResponse = await HttpClient.PostAsJsonAsync("/api/v1/currencies", currencyRequest);
        var currency = await currencyResponse.Content.ReadFromJsonAsync<CurrencyDto>();

        var accountTypeRequest = new CreateAccountTypeRequestDto { Type = "Checking" };
        var accountTypeResponse = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", accountTypeRequest);
        var accountType = await accountTypeResponse.Content.ReadFromJsonAsync<AccountType>();

        // Create a cash account that uses this account type
        var accountRequest = new CreateCashAccountRequestDto
        {
            Name = "Test Account",
            AccountTypeId = accountType!.Id,
            CurrencyId = currency!.Id,
            InitialBalance = 1000.00m
        };
        var accountResponse = await HttpClient.PostAsJsonAsync("/api/v1/accounts/cash", accountRequest);
        accountResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act - try to delete the account type that's being used
        var response = await HttpClient.DeleteAsync($"/api/v1/accountTypes/{accountType.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadAsStringAsync();
        error.Should().NotBeNullOrWhiteSpace(); // Optionally check for a specific error message
    }

    #endregion

    #region Edge Cases and Integration Tests

    [Fact]
    public async Task AccountType_FullCrudLifecycle_WorksCorrectly()
    {
        // Arrange & Act & Assert

        // 1. Create account type
        var createRequest = new CreateAccountTypeRequestDto { Type = "Test Account Type" };
        var createResponse = await HttpClient.PostAsJsonAsync("/api/v1/accountTypes", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdAccountType = await createResponse.Content.ReadFromJsonAsync<AccountType>();
        createdAccountType.Should().NotBeNull();
        createdAccountType!.Type.Should().Be("Test Account Type");

        // 2. Read account type by ID
        var getResponse = await HttpClient.GetAsync($"/api/v1/accountTypes/{createdAccountType.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrievedAccountType = await getResponse.Content.ReadFromJsonAsync<AccountType>();
        retrievedAccountType.Should().NotBeNull();
        retrievedAccountType!.Should().BeEquivalentTo(createdAccountType);

        // 3. Update account type
        var updateRequest = new UpdateAccountTypeRequestDto { Type = "Updated Account Type" };
        var updateResponse = await HttpClient.PutAsJsonAsync($"/api/v1/accountTypes/{createdAccountType.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedAccountType = await updateResponse.Content.ReadFromJsonAsync<AccountType>();
        updatedAccountType.Should().NotBeNull();
        updatedAccountType!.Id.Should().Be(createdAccountType.Id);
        updatedAccountType.Type.Should().Be("Updated Account Type");

        // 4. Verify update by reading again
        var getUpdatedResponse = await HttpClient.GetAsync($"/api/v1/accountTypes/{createdAccountType.Id}");
        var verifyUpdatedAccountType = await getUpdatedResponse.Content.ReadFromJsonAsync<AccountType>();
        verifyUpdatedAccountType!.Type.Should().Be("Updated Account Type");

        // 5. Delete account type
        var deleteResponse = await HttpClient.DeleteAsync($"/api/v1/accountTypes/{createdAccountType.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 6. Verify deletion
        var getDeletedResponse = await HttpClient.GetAsync($"/api/v1/accountTypes/{createdAccountType.Id}");
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AccountType_ConcurrentOperations_HandleCorrectly()
    {
        // This test ensures that concurrent operations work correctly
        // Arrange
        var tasks = new List<Task<HttpResponseMessage>>();

        // Create multiple account types concurrently
        for (int i = 0; i < 5; i++)
        {
            var request = new CreateAccountTypeRequestDto { Type = $"Concurrent Type {i}" };
            tasks.Add(HttpClient.PostAsJsonAsync("/api/v1/accountTypes", request));
        }

        // Act
        var responses = await Task.WhenAll(tasks);

        // Assert
        responses.Should().HaveCount(5);
        responses.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.Created);

        // Verify all account types were created
        var getAllResponse = await HttpClient.GetAsync("/api/v1/accountTypes");
        var allAccountTypes = await getAllResponse.Content.ReadFromJsonAsync<List<AccountType>>();
        allAccountTypes.Should().HaveCount(5);
    }

    #endregion
}
