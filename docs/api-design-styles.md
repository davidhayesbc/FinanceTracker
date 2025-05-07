# API Design and Documentation Standards

This document outlines conventions for designing, implementing, and documenting RESTful APIs within the FinanceTracker project, primarily concerning `FinanceTracker.ApiService`.

## General API Design Principles

- **RESTful Conventions:** Adhere to standard REST principles (use of HTTP verbs, status codes, resource-oriented URLs).
- **Idempotency:** Ensure PUT, DELETE, and PATCH operations are idempotent where applicable.
- **Versioning:** (Specify API versioning strategy, e.g., URL-based `v1/`, header-based).
- **Statelessness:** APIs should be stateless.

## URL Structure

- **Resource Naming:** Use plural nouns for resource collections (e.g., `/users`, `/transactions`).
- **Path Parameters:** Use path parameters for specific resource identifiers (e.g., `/users/{userId}`).
- **Query Parameters:** Use query parameters for filtering, sorting, and pagination (e.g., `/transactions?type=expense&sortBy=date`).
- **Case:** Use `kebab-case` for URL paths if not dictated by framework defaults. ASP.NET Core typically uses `PascalCase` for controller/action names which translate to URL segments. Align with framework behavior or specify overrides.

## Request/Response

- **Data Format:** Use JSON for request and response bodies.
- **HTTP Status Codes:** Use appropriate HTTP status codes (e.g., `200 OK`, `201 Created`, `204 No Content`, `400 Bad Request`, `401 Unauthorized`, `403 Forbidden`, `404 Not Found`, `500 Internal Server Error`).
- **Error Responses:**
  - Use a consistent error response format (e.g., `ProblemDetails` in ASP.NET Core).
  - Include a unique error code or identifier for traceability if possible.
  - Provide clear, user-friendly error messages where appropriate (avoid exposing sensitive internal details).
- **Data Transfer Objects (DTOs):**
  - Define clear DTOs for request and response payloads.
  - Use `PascalCase` for DTO property names in C#.
  - Apply validation attributes (e.g., `[Required]`, `[MaxLength]`) to request DTOs.
- **Pagination:** (Specify pagination strategy, e.g., offset/limit, cursor-based. Include conventions for response headers or body fields like `page`, `pageSize`, `totalPages`, `totalCount`).
- **Sorting & Filtering:** (Define conventions for query parameters used for sorting and filtering).

## API Documentation

- **Swagger/OpenAPI:** Generate OpenAPI (Swagger) documentation for all APIs.
  - Ensure XML documentation comments in C# controllers and DTOs are comprehensive to enrich Swagger output.
  - Document request/response models, parameters, and status codes clearly.
- **Descriptions:** Provide clear descriptions for endpoints, parameters, and responses.

## Security

- **Authentication:** (Specify authentication mechanisms, e.g., JWT Bearer tokens).
- **Authorization:** (Detail authorization strategies, e.g., role-based access control, policy-based authorization).
- **Input Validation:** Always validate input on the server-side.
- **Rate Limiting:** (Consider and define rate limiting strategies if applicable).

---

_This guide should be updated as project standards evolve._
