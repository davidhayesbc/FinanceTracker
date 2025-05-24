# FinanceTracker Solution Structure

This document outlines the architecture and organization of the FinanceTracker solution, which is built using .NET Aspire for orchestration and deployment.

## Solution Overview

FinanceTracker is a personal finance management application built with a modern microservices architecture using .NET Aspire. The solution consists of multiple projects that work together to provide a comprehensive finance tracking experience.

## Project Structure

### Core Projects

#### `FinanceTracker.AppHost`
- **Type:** .NET Aspire App Host
- **Purpose:** Orchestration and service discovery for local development and deployment
- **Key Responsibilities:**
  - Service registration and configuration
  - Local development environment setup
  - Azure deployment orchestration via `azure.yaml`
  - Infrastructure template management (`infra/` folder)
- **Key Files:**
  - `Program.cs` - Service registration and configuration
  - `infra/` - Azure infrastructure templates

#### `FinanceTracker.ServiceDefaults`
- **Type:** Shared Library
- **Purpose:** Common service configurations and extensions
- **Key Responsibilities:**
  - Shared service configurations
  - Common middleware and extensions
  - Service discovery helpers
  - Health check configurations

#### `FinanceTracker.ApiService`
- **Type:** ASP.NET Core Web API
- **Purpose:** Backend REST API for finance operations
- **Key Responsibilities:**
  - RESTful API endpoints for financial data
  - Business logic and data validation
  - Entity Framework Core integration
  - Authentication and authorization (when implemented)
- **Architecture:**
  - `Endpoints/` - Minimal API endpoint definitions
  - `Dtos/` - Data Transfer Objects for API contracts
  - `Properties/` - Launch settings and configuration

#### `FinanceTracker.Data`
- **Type:** Class Library
- **Purpose:** Data access layer and Entity Framework Core models
- **Key Responsibilities:**
  - Entity Framework DbContext
  - Database models and entities
  - Database migrations
  - Data access patterns
- **Architecture:**
  - `Models/` - Entity Framework entities
  - `Migrations/` - EF Core database migrations

#### `FinanceTracker.MigrationService`
- **Type:** Worker Service
- **Purpose:** Database migration and seeding service
- **Key Responsibilities:**
  - Automated database migration on startup
  - Database seeding with initial data
  - Database schema management
- **Key Files:**
  - `Worker.cs` - Background service implementation
  - `Program.cs` - Service configuration

#### `FinanceTracker.Web`
- **Type:** Vue.js SPA (Single Page Application)
- **Purpose:** Frontend user interface
- **Key Responsibilities:**
  - User interface for finance management
  - API integration with backend services
  - Client-side routing and state management
- **Technology Stack:**
  - Vue.js 3 with Composition API
  - TypeScript for type safety
  - Vite for build tooling
  - Vitest for testing
  - Playwright for E2E testing

## Domain Model

The application manages personal finances with the following core entities:

### Financial Accounts
- **Account Types:** Support for different account types (checking, savings, investment, etc.)
- **Account Management:** Track multiple financial accounts with balances and metadata

### Transactions
- **Transaction Types:** Income, expenses, transfers between accounts
- **Transaction Categories:** Categorization for budgeting and reporting
- **Transaction Splits:** Split transactions across multiple categories

### Categorization
- **Categories:** Hierarchical category system for organizing transactions
- **Category Types:** Different category types for different transaction types

## Technology Stack

### Backend
- **.NET 8+** - Core framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM and database access
- **SQL Server** - Primary database (configurable)
- **.NET Aspire** - Orchestration and deployment

### Frontend
- **Vue.js 3** - Progressive JavaScript framework
- **TypeScript** - Type-safe JavaScript
- **Vite** - Build tool and development server
- **Vitest** - Unit testing framework
- **Playwright** - End-to-end testing

### Infrastructure
- **Azure Container Apps** - Hosting platform
- **Azure SQL Database** - Managed database service
- **Azure Developer CLI (azd)** - Deployment tooling

## Development Workflow

### Local Development
1. The `FinanceTracker.AppHost` orchestrates all services
2. Run the AppHost to start all services in development mode
3. Services are automatically configured with service discovery
4. Database migrations run automatically via `FinanceTracker.MigrationService`

### Building and Testing
```bash
# Build entire solution
dotnet build FinanceTracker.sln

# Run backend tests
dotnet test

# Frontend development server (from FinanceTracker.Web)
npm run dev

# Frontend tests
npm run test
```

### Deployment
The solution uses Azure Developer CLI for deployment:
```bash
# Initialize Azure resources
azd init

# Deploy to Azure
azd up
```

## Configuration Management

### Development
- `appsettings.Development.json` files for service-specific configuration
- Environment variables for sensitive configuration
- Service discovery through .NET Aspire

### Production
- Azure App Configuration for centralized configuration
- Azure Key Vault for secrets management
- Environment-specific configuration through Azure

## API Design

The API follows RESTful conventions with:
- Resource-based URLs (`/accounts`, `/transactions`, `/categories`)
- Standard HTTP verbs (GET, POST, PUT, DELETE)
- Consistent response formats
- OpenAPI/Swagger documentation

## Data Flow

1. **User Interaction** → Vue.js frontend
2. **API Calls** → FinanceTracker.ApiService
3. **Data Access** → FinanceTracker.Data (Entity Framework)
4. **Database** → SQL Server

## Security Considerations

- Input validation on all API endpoints
- SQL injection protection through Entity Framework
- Future: Authentication and authorization middleware
- Future: HTTPS enforcement and security headers

## Extensibility

The modular architecture allows for:
- Additional service projects for new features
- Plugin-based category and rule systems
- Multiple frontend applications
- Integration with external financial services

---

_This document should be updated as the solution architecture evolves._
