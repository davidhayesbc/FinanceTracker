# C# Styling Guide

This document outlines styling and coding conventions for C# backend development within the FinanceTracker project.

## C# Backend (FinanceTracker.ApiService, FinanceTracker.Data, etc.)

- **Naming Conventions:**
  - `PascalCase` for namespaces, classes, interfaces, enums, methods, properties, and events.
  - `camelCase` for local variables and method parameters.
  - Prefix interfaces with `I` (e.g., `ITransactionService`, `IAccountRepository`).
- **Indentation:** Use 4 spaces for indentation.
- **Strings:**
  - Use double quotes (`"`) for string literals.
  - Use string interpolation (`$"Hello {name}"`) for dynamic strings.
- **`var` vs. Explicit Types:**
  - Use `var` when the type is obvious from the right-hand side of the assignment to reduce verbosity.
  - Otherwise, use explicit types for clarity, especially for public API signatures or complex types.
- **LINQ:**
  - Prefer method syntax (e.g., `context.Transactions.Where(t => t.Amount > 100)`) over query syntax, unless query syntax significantly improves readability for complex queries.
- **File-Scoped Namespaces:** Use file-scoped namespaces (e.g., `namespace FinanceTracker.ApiService.Controllers;`) if the project targets C# 10 or later.
- **Expression-Bodied Members:** Use expression-bodied members for simple, single-line methods and properties where it enhances conciseness without sacrificing readability.
- **Error Handling:** Implement robust error handling, potentially using custom exception types and middleware for consistent API error responses (e.g., `ProblemDetails`).
- **Async/Await:** Utilize `async/await` for all I/O-bound database operations and other genuinely asynchronous operations.

---

_This guide should be updated as project standards evolve._
