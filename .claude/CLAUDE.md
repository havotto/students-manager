# Students Manager

`students-manager` is a Student Management System for managing student records (create, read, update, delete).

## Principles to follow

1. Don't assume. Don't hide confusion. Surface trade-offs.
2. Minimum code that solves the problem. Nothing speculative.
3. Touch only what you must. Clean up only your own mess.
   Mention unrelated issues; don't fix them.
4. Define success criteria. Loop until verified: `dotnet build` and
   `dotnet test` must pass before you call anything done.

## Technology Stack

- **Backend:** .NET 10, ASP.NET Core 10, Entity Framework Core 10
- **Database:** In-memory SQLite (one shared open connection for app lifetime)
- **Frontend:** Vanilla JS SPA - single static HTML file served by backend. No npm, no build step, no frameworks, no CDN scripts. The UI must communicate with the backend exclusively through REST APIs
- **Testing:** XUnit 3

Root namespace: `Havotto.StudentsManager`

## Testing
- Integration tests only, no unit tests.
- Use `WebApplicationFactory<T>` against real HTTP endpoints.
- No Playwright or any E2E framework.

## Core Operational Commands
- Build: `dotnet build`
- Run: `dotnet run --project src/Havotto.StudentsManager`
- Test: `dotnet test`
- Publish: `dotnet publish -c Release`
