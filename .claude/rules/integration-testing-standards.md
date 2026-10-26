---
description: Integration testing standards and guidelines
paths:
  - "**/tests/Havotto.StudentsManager.Tests/**"
---

# Integration Testing Standards

Tests should be in a separate project.
Use xUnit v3 (xunit.v3 package).

## 1. Test Scope

- Do not mock application layers (Endpoint, Service, DbContext).
- Only mock external dependencies that cannot run locally.
- Tests must validate full request → service → database flow.

## 2. Test Structure

- Follow Arrange → Act → Assert pattern.
- Mark the arrange, act assert sections of the test cases with // Arrange, // Act, and // Assert comments
- Keep test cases focused on a single behavior.
- Use `Fact.DisplayName` with a clear sentence alongside a short method name. Do not use underscores or overly long method names.

Example:
```c#
[Fact(DisplayName = "Should return all items")]
public async Task ShouldReturnAllItems() { ... }

[Fact(DisplayName = "Should return 404 ProblemDetails when item ID does not exist")]
public async Task ShouldReturn404WhenNotFound() { ... }
```

## 3. API Testing Approach

- Use `WebApplicationFactory` for full application context testing.
- Assert responses using DTO contracts and HTTP status code, not entity structures.
- Read bodies with `ReadFromJsonAsync<T>()`: the DTO type for success, `ProblemDetails` for business-rule errors (`404`, `409`), and `ValidationProblemDetails` for shape-validation errors (`400`).
- Return the response body as a typed object and assert using xUnit.

## 4. Error Response Validation

- Error responses must follow RFC 7807 `ProblemDetails`.
- Assert the correct HTTP status code for every error scenario.

## 5. Data Management

   - Tests run against in-memory SQLite.
   - Each test gets its own `WebApplicationFactory` and its own open `SqliteConnection`, created in the test class constructor or `InitializeAsync` and disposed afterward. Never share a database between tests, so there is no cleanup logic.
