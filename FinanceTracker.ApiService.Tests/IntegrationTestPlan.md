# FinanceTracker API Integration Test Plan

This document outlines the comprehensive integration testing strategy for all API endpoints in the FinanceTracker.ApiService project. Each test will use .NET Aspire testing infrastructure to ensure proper integration between services.

## Testing Strategy

All integration tests will:
- Use the existing `DistributedApplicationTestingBuilder` with the AppHost
- Target the `apiservice` resource (not the `web` resource)
- Wait for the service to be in `Running` state before testing
- Use realistic test data following domain constraints
- Test both success and error scenarios
- Validate response status codes, headers, and content

## Test Structure

Tests will be organized into separate test classes by endpoint group:
- `AccountEndpointsIntegrationTests.cs`
- `TransactionEndpointsIntegrationTests.cs`
- `TransactionSplitEndpointsIntegrationTests.cs`
- `TransactionCategoryEndpointsIntegrationTests.cs`
- `TransactionTypeEndpointsIntegrationTests.cs`
- `AccountTypeEndpointsIntegrationTests.cs`
- `RecurringTransactionEndpointsIntegrationTests.cs`

---

## 1. Account Endpoints (`/accounts`)

### 1.1 Get All Accounts
- [ ] **GET `/accounts`** - Retrieve all accounts (both cash and investment)
  - [ ] Test returns 200 OK with empty list when no accounts exist
  - [ ] Test returns 200 OK with account list when accounts exist
  - [ ] Test response contains both cash and investment accounts
  - [ ] Test response includes computed current balance
  - [ ] Test response includes proper account type and currency information

### 1.2 Get Cash Accounts
- [ ] **GET `/accounts/cash`** - Retrieve only cash accounts
  - [ ] Test returns 200 OK with empty list when no cash accounts exist
  - [ ] Test returns 200 OK with cash account list when accounts exist
  - [ ] Test response excludes investment accounts
  - [ ] Test response includes overdraft limit for cash accounts

### 1.3 Get Investment Accounts
- [ ] **GET `/accounts/investment`** - Retrieve only investment accounts
  - [ ] Test returns 200 OK with empty list when no investment accounts exist
  - [ ] Test returns 200 OK with investment account list when accounts exist
  - [ ] Test response excludes cash accounts
  - [ ] Test response includes broker information for investment accounts

### 1.4 Get Account by ID
- [ ] **GET `/accounts/{id}`** - Retrieve specific account details
  - [ ] Test returns 200 OK with valid account ID
  - [ ] Test returns 404 Not Found with invalid account ID
  - [ ] Test response includes full account details with transactions
  - [ ] Test works for both cash and investment accounts

### 1.5 Get Account Transactions
- [ ] **GET `/accounts/{id}/transactions`** - Retrieve transactions for specific account
  - [ ] Test returns 200 OK with valid account ID
  - [ ] Test returns 404 Not Found with invalid account ID
  - [ ] Test returns empty list when account has no transactions
  - [ ] Test returns transaction list ordered by date (newest first)
  - [ ] Test includes transaction splits in response

### 1.6 Get Account Recurring Transactions
- [ ] **GET `/accounts/{id}/recurringTransactions`** - Retrieve recurring transactions for account
  - [ ] Test returns 200 OK with valid account ID
  - [ ] Test returns 404 Not Found with invalid account ID
  - [ ] Test returns empty list when account has no recurring transactions
  - [ ] Test returns recurring transaction list

### 1.7 Create Cash Account
- [ ] **POST `/accounts/cash`** - Create new cash account
  - [ ] Test returns 201 Created with valid request data
  - [ ] Test returns 400 Bad Request with invalid request data
  - [ ] Test validates required fields (Name, AccountTypeId, CurrencyId)
  - [ ] Test validates AccountTypeId exists
  - [ ] Test validates CurrencyId exists
  - [ ] Test creates account period with opening balance
  - [ ] Test response includes created account with generated ID

### 1.8 Create Investment Account
- [ ] **POST `/accounts/investment`** - Create new investment account
  - [ ] Test returns 201 Created with valid request data
  - [ ] Test returns 400 Bad Request with invalid request data
  - [ ] Test validates required fields (Name, AccountTypeId, CurrencyId, Broker)
  - [ ] Test validates AccountTypeId exists
  - [ ] Test validates CurrencyId exists
  - [ ] Test creates account period with opening balance
  - [ ] Test response includes created account with generated ID

---

## 2. Transaction Endpoints (`/transactions`)

### 2.1 Get Cash Transactions
- [ ] **GET `/transactions/cash`** - Retrieve all cash transactions
  - [ ] Test returns 200 OK with empty list when no transactions exist
  - [ ] Test returns 200 OK with transaction list when transactions exist
  - [ ] Test excludes investment transactions
  - [ ] Test includes account and category information
  - [ ] Test orders transactions by date (newest first)

### 2.2 Get Investment Transactions
- [ ] **GET `/transactions/investment`** - Retrieve all investment transactions
  - [ ] Test returns 200 OK with empty list when no transactions exist
  - [ ] Test returns 200 OK with transaction list when transactions exist
  - [ ] Test excludes cash transactions
  - [ ] Test includes security and account information
  - [ ] Test orders transactions by date (newest first)

### 2.3 Create Cash Transaction
- [ ] **POST `/transactions/cash`** - Create new cash transaction
  - [ ] Test returns 201 Created with valid request data
  - [ ] Test returns 400 Bad Request with invalid request data
  - [ ] Test validates required fields (AccountPeriodId, CategoryId, Amount, TransactionDate)
  - [ ] Test validates AccountPeriodId exists and is active
  - [ ] Test validates CategoryId exists
  - [ ] Test validates Amount is not zero
  - [ ] Test validates TransactionDate is valid
  - [ ] Test updates account period balance
  - [ ] Test response includes created transaction with generated ID

### 2.4 Create Investment Transaction
- [ ] **POST `/transactions/investment`** - Create new investment transaction
  - [ ] Test returns 201 Created with valid request data
  - [ ] Test returns 400 Bad Request with invalid request data
  - [ ] Test validates required fields (AccountPeriodId, SecurityId, Quantity, Price, TransactionDate)
  - [ ] Test validates AccountPeriodId exists and is active
  - [ ] Test validates SecurityId exists
  - [ ] Test validates Quantity and Price are valid numbers
  - [ ] Test validates TransactionDate is valid
  - [ ] Test calculates total value correctly (Quantity × Price)
  - [ ] Test response includes created transaction with generated ID

---

## 3. Transaction Split Endpoints (`/transactionSplits`)

### 3.1 Get Transaction Splits
- [ ] **GET `/transactionSplits`** - Retrieve all transaction splits
  - [ ] Test returns 200 OK with empty list when no splits exist
  - [ ] Test returns 200 OK with split list when splits exist
  - [ ] Test includes transaction and category information
  - [ ] Test validates split amounts sum to transaction amount

### 3.2 Create Transaction Split
- [ ] **POST `/transactionSplits`** - Create new transaction split
  - [ ] Test returns 201 Created with valid request data
  - [ ] Test returns 400 Bad Request with invalid request data
  - [ ] Test validates required fields (TransactionId, CategoryId, Amount)
  - [ ] Test validates TransactionId exists
  - [ ] Test validates CategoryId exists
  - [ ] Test validates Amount is not zero
  - [ ] Test prevents split amounts from exceeding transaction amount
  - [ ] Test response includes created split with generated ID

---

## 4. Transaction Category Endpoints (`/transactionCategories`)

### 4.1 Get Transaction Categories
- [ ] **GET `/transactionCategories`** - Retrieve all transaction categories
  - [ ] Test returns 200 OK with empty list when no categories exist
  - [ ] Test returns 200 OK with category list when categories exist
  - [ ] Test includes hierarchical category structure (parent/child relationships)
  - [ ] Test includes category type information

### 4.2 Create Transaction Category
- [ ] **POST `/transactionCategories`** - Create new transaction category
  - [ ] Test returns 201 Created with valid request data
  - [ ] Test returns 400 Bad Request with invalid request data
  - [ ] Test validates required fields (Name, TransactionCategoryTypeId)
  - [ ] Test validates TransactionCategoryTypeId exists
  - [ ] Test validates ParentCategoryId exists (if provided)
  - [ ] Test prevents circular category references
  - [ ] Test response includes created category with generated ID

---

## 5. Transaction Type Endpoints (`/transactionTypes`)

### 5.1 Get Transaction Types
- [ ] **GET `/transactionTypes`** - Retrieve all transaction types
  - [ ] Test returns 200 OK with empty list when no types exist
  - [ ] Test returns 200 OK with type list when types exist
  - [ ] Test includes type name and description

### 5.2 Create Transaction Type
- [ ] **POST `/transactionTypes`** - Create new transaction type
  - [ ] Test returns 201 Created with valid request data
  - [ ] Test returns 400 Bad Request with invalid request data
  - [ ] Test validates required fields (Type)
  - [ ] Test prevents duplicate transaction type names
  - [ ] Test response includes created type with generated ID

---

## 6. Account Type Endpoints (`/accountTypes`)

### 6.1 Get Account Types
- [ ] **GET `/accountTypes`** - Retrieve all account types
  - [ ] Test returns 200 OK with empty list when no types exist
  - [ ] Test returns 200 OK with type list when types exist
  - [ ] Test includes type name and description

### 6.2 Create Account Type
- [ ] **POST `/accountTypes`** - Create new account type
  - [ ] Test returns 201 Created with valid request data
  - [ ] Test returns 400 Bad Request with invalid request data
  - [ ] Test validates required fields (Type)
  - [ ] Test prevents duplicate account type names
  - [ ] Test response includes created type with generated ID

---

## 7. Recurring Transaction Endpoints (`/recurringTransactions`)

### 7.1 Get Recurring Transactions
- [ ] **GET `/recurringTransactions`** - Retrieve all recurring transactions
  - [ ] Test returns 200 OK with empty list when no recurring transactions exist
  - [ ] Test returns 200 OK with recurring transaction list when they exist
  - [ ] Test includes account and category information
  - [ ] Test includes recurrence pattern details

### 7.2 Create Recurring Transaction
- [ ] **POST `/recurringTransactions`** - Create new recurring transaction
  - [ ] Test returns 201 Created with valid request data
  - [ ] Test returns 400 Bad Request with invalid request data
  - [ ] Test validates required fields (AccountId, CategoryId, Amount, StartDate, RecurrencePattern)
  - [ ] Test validates AccountId exists
  - [ ] Test validates CategoryId exists
  - [ ] Test validates Amount is not zero
  - [ ] Test validates StartDate is valid
  - [ ] Test validates RecurrencePattern is valid
  - [ ] Test response includes created recurring transaction with generated ID

---

## 8. Error Handling and Edge Cases

### 8.1 Authentication/Authorization Tests
- [ ] Test endpoints without authentication (if auth is implemented)
- [ ] Test endpoints with invalid authentication tokens
- [ ] Test role-based access control (if implemented)

### 8.2 Validation Tests
- [ ] Test all endpoints with malformed JSON payloads
- [ ] Test all endpoints with missing required fields
- [ ] Test all endpoints with invalid data types
- [ ] Test all endpoints with boundary values (max/min lengths, amounts)

### 8.3 Database Constraint Tests
- [ ] Test foreign key constraint violations
- [ ] Test unique constraint violations
- [ ] Test null constraint violations
- [ ] Test check constraint violations

### 8.4 Concurrency Tests
- [ ] Test concurrent creation of accounts
- [ ] Test concurrent transaction creation for same account
- [ ] Test optimistic concurrency control (if implemented)

---

## 9. Performance Tests

### 9.1 Load Tests
- [ ] Test endpoints with large datasets
- [ ] Test pagination performance (if implemented)
- [ ] Test query performance with complex joins
- [ ] Test response time under normal load

### 9.2 Memory Tests
- [ ] Test memory usage with large result sets
- [ ] Test for memory leaks in long-running scenarios

---

## 10. Data Consistency Tests

### 10.1 Transaction Integrity
- [ ] Test account balance updates after transaction creation
- [ ] Test account balance consistency across multiple transactions
- [ ] Test transaction split amount consistency
- [ ] Test account period balance calculations

### 10.2 Referential Integrity
- [ ] Test cascade deletion behavior
- [ ] Test orphaned record prevention
- [ ] Test cross-table consistency

---

## Implementation Progress

**Overall Progress: 0/120+ tests planned**

### Test Class Creation Status
- [ ] `AccountEndpointsIntegrationTests.cs` (0/24 tests)
- [ ] `TransactionEndpointsIntegrationTests.cs` (0/16 tests)
- [ ] `TransactionSplitEndpointsIntegrationTests.cs` (0/8 tests)
- [ ] `TransactionCategoryEndpointsIntegrationTests.cs` (0/8 tests)
- [ ] `TransactionTypeEndpointsIntegrationTests.cs` (0/8 tests)
- [ ] `AccountTypeEndpointsIntegrationTests.cs` (0/8 tests)
- [ ] `RecurringTransactionEndpointsIntegrationTests.cs` (0/8 tests)
- [ ] Error handling tests (0/16 tests)
- [ ] Performance tests (0/8 tests)
- [ ] Data consistency tests (0/8 tests)

### Infrastructure Status
- [x] Base integration test example (`IntegrationTest1.cs`)
- [x] Aspire testing configuration
- [x] Test project with required packages
- [x] Test data seeding utilities
- [x] Test helper methods for common operations
- [x] Test base class with shared setup/teardown

---

## Next Steps

1. **Create Test Infrastructure**
   - Create base test classes with common setup
   - Implement test data seeding utilities
   - Create helper methods for HTTP operations

2. **Start with Core Endpoints**
   - Begin with Account endpoints (most fundamental)
   - Progress to Transaction endpoints
   - Add supporting endpoint tests

3. **Add Advanced Testing**
   - Implement error scenario testing
   - Add performance benchmarks
   - Create data consistency validation

4. **Continuous Integration**
   - Ensure tests run in CI/CD pipeline
   - Add test coverage reporting
   - Implement test result tracking

---

*This plan covers comprehensive integration testing for all API endpoints. Tests should be implemented incrementally, starting with the most critical functionality and expanding to cover edge cases and performance scenarios.*
