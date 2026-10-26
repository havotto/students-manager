---
description: Backend architectural and engineering standards
paths:
  - "**/src/Havotto.StudentsManager/**"
---

# Backend Engineering Standards

## 1. Namespace Structure

All backend source code must reside under the root namespace defined in `CLAUDE.md` using a consistent layer-based structure.

- `Endpoints` -> REST minimal API endpoints. Responsible only for request handling and delegation. No business logic.
- `Dto` -> API request and response models (immutable).
- `Entities` -> Entity Framework entities.
- `Data`: DbContext related files
- `Services` -> Business logic and transactional boundaries.
- `Mappers` -> Conversion between Entity and DTO objects, can be stateless static classes.
- `Util` -> Stateless helper and utility classes.

## 2. Model & Data Layer Isolation

- DTOs are `record` types and represent all API request and response models.
- Entities are persistence models only. They are referenced only by `Data`,
  `Services`, and `Mappers`, and never appear in API responses.
- Entity <-> DTO conversion happens only in `Mappers`.

## 3. Dependency Injection

- Constructor injection everywhere, except minimal API handlers, which receive services as handler parameters.

## 4. Service Layer

- All business logic lives in services. No repository or unit-of-work layer;
  services use `DbContext` directly.
- Services return `Result<T>` where `T` is a DTO. `Result<T>` is hand-rolled
  and lives in `Util`.
- All I/O is async and accepts a `CancellationToken` from the endpoint.

## 5. API Design & Validation

- Follow REST conventions: correct HTTP methods and status codes (`200`, `201`, `204`, `400`, `404`, `409`).
- Each resource is one static extension class using `MapGroup`. `POST` returns `201` with a `Location` header.
- Input shape validation (required, length, range) uses DataAnnotations on request DTOs via `AddValidation()`. It runs before the service and returns `400` automatically. Do not repeat these checks in services.
- Business-rule failures (not found, conflict, duplicate email) are returned from services as `Result<T>`.

## 6. Exception Handling & Error Responses

- Expected failures (validation, not found, conflict) are returned as
  `Result<T>`, never thrown. Endpoints map them to `ProblemDetails`.
- Exceptions are for unexpected errors only and are handled by a single global
  `IExceptionHandler`.
- All error responses use RFC 7807 `ProblemDetails`. Never expose stack traces
  or internal details.

## 7. Configuration Management

- Service registration, middleware pipeline setup, endpoint mapping, and database setup are extension methods
  (`AddStudentsManager`, `MapStudentsManager` etc.) so tests reuse them.
- `Program` is `public partial class Program;` for `WebApplicationFactory`.
