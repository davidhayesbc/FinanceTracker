# Testing Standards

This document outlines conventions for writing unit, integration, and end-to-end (E2E) tests for the FinanceTracker project.

## General Testing Principles

- **Test Coverage:** Strive for reasonable test coverage for critical application logic.
- **Readability & Maintainability:** Tests should be clear, concise, and easy to understand and maintain.
- **Independence:** Tests should be independent and runnable in any order. Avoid dependencies between tests.
- **Repeatability:** Tests should produce the same results every time they are run, given the same conditions.
- **Speed:** Unit tests should be fast. Integration and E2E tests can be slower but should be optimized where possible.

## C# Backend Testing (`FinanceTracker.ApiService`, `FinanceTracker.Data`)

- **Frameworks:**
  - Unit Tests: (Specify framework, e.g., xUnit, NUnit, MSTest).
  - Mocking: (Specify mocking library, e.g., Moq, NSubstitute).
- **Naming Conventions:**
  - Test Projects: `ProjectName.Tests.Unit`, `ProjectName.Tests.Integration`.
  - Test Classes: `ClassNameTests` (e.g., `TransactionServiceTests`).
  - Test Methods: `MethodName_Scenario_ExpectedBehavior` (e.g., `CreateTransaction_WithValidData_ReturnsCreatedTransaction`).
- **Arrangement, Act, Assert (AAA) Pattern:** Structure tests using the AAA pattern.
- **Unit Tests:**
  - Focus on testing individual units (classes, methods) in isolation.
  - Mock dependencies extensively.
- **Integration Tests:**
  - Test interactions between components (e.g., service with repository, API endpoint with database).
  - May involve a test database or in-memory database.
  - For API integration tests, use `WebApplicationFactory` or similar.
- **Assertions:** Use fluent assertion libraries if preferred (e.g., FluentAssertions).

## Vue.js / TypeScript Frontend Testing (`FinanceTracker.Web`)

- **Frameworks:**
  - Unit/Component Tests: (Specify framework, e.g., Vitest with Vue Test Utils).
  - E2E Tests: (Specify framework, e.g., Playwright, Cypress).
- **Naming Conventions:**
  - Test Files: `ComponentName.spec.ts` or `feature.spec.ts`.
  - Test Suites (`describe` blocks): Describe the component or feature being tested.
  - Test Cases (`it` or `test` blocks): Describe the specific behavior being tested.
- **Unit/Component Tests:**
  - Test individual components, composables, or utility functions.
  - Mock child components, API calls, and external dependencies.
  - Use Vue Test Utils for mounting and interacting with components.
- **End-to-End (E2E) Tests:**
  - Test user flows through the application.
  - Interact with the application as a user would (e.g., clicking buttons, filling forms).
  - Use page object models (POMs) to improve maintainability.
- **Selectors:** Prefer data attributes (e.g., `data-testid`) for selecting elements in tests to decouple tests from CSS or structural changes.

## Test Data

- **Clarity:** Use clear and descriptive test data.
- **Isolation:** Test data should be specific to the test case and not rely on global state.
- **Realism:** Use realistic data where appropriate, especially for integration and E2E tests.

---

_This guide should be updated as project standards evolve._
