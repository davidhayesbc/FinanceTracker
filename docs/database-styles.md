# Database Styling Guide

This document outlines styling and coding conventions for database interactions within the FinanceTracker project, particularly focusing on Entity Framework Core usage.

## Entity Framework Core & Database (SQL Server)

- **DbContext Usage:**
  - Follow established patterns for DbContext usage (e.g., repository pattern, or direct DbContext usage in service classes if simpler and appropriate for the project's scale).
  - If using a repository pattern, define clear interfaces for repositories (e.g., `IRepository<T>`) and their implementations.
- **Migrations:**
  - All database schema changes **must** be managed via EF Core migrations in the `FinanceTracker.Data` project.
  - Ensure migration names are descriptive (e.g., `AddUserEmailVerificationToken`, `IncreaseTransactionDescriptionLength`).
- \*\*Data Models (Entities - in `FinanceTracker.Data/Models/`):
  - Properties should be `PascalCase`.
  - Use appropriate data annotations for validation and EF Core mapping where necessary (e.g., `[Required]`, `[MaxLength(100)]`, `[Column(TypeName = "decimal(18,2)")]`).
  - Define navigation properties clearly.
- **Querying:**
  - Strive for efficient queries. Avoid N+1 problems by using `Include()` and `ThenInclude()` for eager loading where appropriate.
  - Use `AsNoTracking()` for read-only queries to improve performance if the entities are not going to be updated in the current context.
  - Project onto DTOs or anonymous types using `Select()` for queries that only need a subset of data, to reduce data transfer and improve performance.
- **Concurrency Control:** Implement concurrency control mechanisms (e.g., optimistic concurrency using row versioning/timestamp) for entities that are likely to be updated concurrently.
- **Transactions:** Use explicit database transactions (`DbContext.Database.BeginTransactionAsync()`) when multiple operations need to be atomic.
- **SQL Server Specifics:**
  - \*\*Naming Conventions (if directly interacting or for reference):
    - Tables: `PascalCase`, plural (e.g., `UserProfiles`, `Transactions`).
    - Columns: `PascalCase` (e.g., `FirstName`, `TransactionDate`).

---

_This guide should be updated as project standards evolve._
