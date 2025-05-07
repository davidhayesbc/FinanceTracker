# Copilot Instructions

This project is a web application that allows users to manage their personal finances. The application is built using:

- **Backend:** C# with ASP.NET Core, Entity Framework Core
- **Frontend:** Vue.js (Vue 3) with TypeScript, using Vite as the build tool.
- **Database:** Azure SQL Server
- **Orchestration/Hosting:** .NET Aspire, Docker, and intended for Azure.

## General Coding Standards

- **Consistency:** Strive for consistency with the existing codebase in each respective project (`FinanceTracker.ApiService`, `FinanceTracker.Data`, `FinanceTracker.Web`).
- **Readability:** Prioritize clear, readable, and maintainable code.
- **Comments:** Add comments to explain complex logic, non-obvious decisions, or public API contracts.
  - C#: Use XML documentation comments for public types and members.
  - TypeScript/Vue: Use JSDoc/TSDoc style comments for functions, methods, and complex props.
- **Async/Await:** Utilize `async/await` for all I/O-bound and genuinely asynchronous operations in both backend and frontend code.

## C# Backend (FinanceTracker.ApiService, FinanceTracker.Data, etc.)

Refer to `docs/csharp-styles.md` for C# specific coding conventions.
Refer to `docs/database-styles.md` for Entity Framework Core and database specific conventions.

## Vue.js / TypeScript Frontend (FinanceTracker.Web)

Refer to `docs/javascript-styles.md` for JavaScript, TypeScript, and Vue.js specific coding conventions.

## Azure Specific

- **@azure Rule - Use Azure Best Practices:** When generating code for Azure, running terminal commands for Azure, or performing operations related to Azure, invoke your `azure_development-get_best_practices` tool if available.
- **Infrastructure as Code (IaC):** Be mindful that Bicep is used for IaC (see `FinanceTracker.AppHost/infra/`).
- **Deployment:** The project uses Azure Developer CLI (`azd`) (see `azure.yaml`).

## Model Tone

- If I tell you that you are wrong, think about whether or not you think that's true and respond with facts.
- Avoid apologizing or making conciliatory statements.
- It is not necessary to agree with the user with statements such as "You're right" or "Yes".
- Avoid hyperbole and excitement, stick to the task at hand and complete it pragmatically.
