# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

### Build
```sh
dotnet build Shopizy.sln
dotnet build Shopizy.sln --configuration Release
```

### Run the API
```sh
cd src/Shopizy.Api
dotnet run
```

### Run Tests
```sh
# All tests
dotnet test Shopizy.sln

# Single test project
dotnet test tests/Shopizy.Application.UnitTests
dotnet test tests/Shopizy.Domain.UnitTests
dotnet test tests/Shopizy.Infrastructure.UnitTests
dotnet test tests/Shopizy.Api.IntegrationTests

# Single test by name
dotnet test --filter "FullyQualifiedName~MyTestName"
```

### Database Migrations
```sh
# From solution root
dotnet ef migrations add <MigrationName> --project src/Shopizy.Infrastructure --startup-project src/Shopizy.Api
dotnet ef database update --project src/Shopizy.Infrastructure --startup-project src/Shopizy.Api
```

## Architecture

The solution follows Clean Architecture with five projects:

- **Shopizy.Domain** — Aggregates, entities, value objects, domain events, and custom errors. No dependencies on other solution projects.
- **Shopizy.SharedKernel** — Base types (`AggregateRoot`, `Entity`, `ValueObject`, `IDomainEvent`) and the CQRS messaging abstractions (`ICommand`, `IQuery`, `IDispatcher`, `ICommandHandler`, `IQueryHandler`, `IDomainEventHandler`). Also contains cross-cutting decorator behaviors (validation, unit-of-work, caching).
- **Shopizy.Application** — Use cases organized by feature (e.g. `Products/Commands/CreateProduct/`). Each handler is a class implementing `ICommandHandler<,>` or `IQueryHandler<,>`. Depends on Domain and SharedKernel.
- **Shopizy.Infrastructure** — EF Core (`AppDbContext`), repositories, JWT auth, Cloudinary media uploads, Stripe payments, Redis caching, and the `IDispatcher` implementation. Depends on Application and SharedKernel.
- **Shopizy.Contracts** — Request/response DTOs shared between the API and callers.
- **Shopizy.Api** — Minimal API endpoints using the `IEndpoint` / `ApiEndpoint` pattern. Each endpoint class maps one route. Depends on Application, Infrastructure, and Contracts.

### CQRS / Dispatcher Pattern

There is no MediatR. Commands and queries are dispatched via the custom `IDispatcher` (implemented in `Shopizy.Infrastructure/Messaging/Dispatcher.cs`), which resolves handlers from DI. Handlers are auto-registered with Scrutor assembly scanning in `Shopizy.Application/DependencyInjectionRegister.cs`.

Cross-cutting concerns are applied as Scrutor decorator chains, registered in this order:
1. `ValidationCommandHandlerDecorator` — runs FluentValidation before the handler.
2. `UnitOfWorkCommandHandlerDecorator` — wraps commands with a DB transaction.

Validators live next to their command/query class and are auto-registered via `AddValidatorsFromAssemblyContaining`.

**Domain event handlers run sequentially**, not concurrently. `Dispatcher.PublishAsync` resolves all `IDomainEventHandler<TEvent>` registrations via `GetServices` and awaits them in a `foreach` loop. This is intentional: handlers share the same scoped `DbContext`, and concurrent execution would cause EF Core's "A second operation was started on this context instance before a previous operation completed" error. Do not change this to `Task.WhenAll`.

**Never call `repository.Update(entity)` immediately after `repository.AddAsync(entity)`** without an intervening `SaveChangesAsync`. EF Core already tracks the entity as `Added`; calling `_dbContext.Update()` changes its state to `Modified`, which causes EF Core to generate an `UPDATE` on a row that doesn't exist yet → `DbUpdateConcurrencyException`. The `AddAsync` + any property mutations are captured by EF Core's change tracker and flushed as a single `INSERT` on `SaveChangesAsync`.

### Minimal API Endpoint Pattern

All endpoints extend `ApiEndpoint` (which implements `IEndpoint`). They are discovered and registered via reflection in `EndpointExtensions.AddEndpoints` / `MapEndpoints`. To add a new endpoint, create a class that extends `ApiEndpoint` and implement `MapEndpoint`.

Use `ApiEndpoint.HandleAsync(dispatcher, command, onSuccess, onError)` to dispatch and map `ErrorOr<T>` results to `IResult`.

#### User-centric vs Admin endpoint conventions

**User-centric endpoints** (`api/v1.0/users/{userId}/...`) scope resources to a specific user. The `userId` in the route must be validated against the JWT identity using `ClaimsPrincipalExtensions.IsAuthorized(userId)` — return 403 if they don't match. This prevents a user from acting on another user's data. Example: `GET api/v1.0/users/{userId}/cart`.

**Admin endpoints** (`api/v1.0/admin/...`) operate on system resources. Never put the admin's `userId` in the route — inject `ICurrentUser` and call `currentUser.GetCurrentUserId()` to silently capture the acting admin's identity for audit purposes (`ModifiedBy`, audit log). Example: `DELETE api/v1.0/admin/categories/{categoryId}`.

Never pass `Guid.Empty` as a `UserId` in any command — always supply the real identity from `ICurrentUser`.

### Eventual Consistency / Domain Events

`EventualConsistencyMiddleware` wraps every non-GET request in a DB transaction. After commit, it dequeues domain events stored in `HttpContext.Items` (keyed by `EventualConsistencyMiddleware.DomainEventsKey`) and publishes them via `IDispatcher.PublishAsync`. Failure during event publishing does not roll back the already-committed transaction ("best-effort" consistency).

Domain events are collected in `AppDbContext.SaveChangesAsync` by calling `PopDomainEvents()` on tracked aggregate roots.

### Database

The database provider is selected at runtime via the `UsePostgreSql` flag in `appsettings.json`:
- `true` — Npgsql (PostgreSQL)
- `false` (default for dev) — SQL Server / LocalDB

Integration tests always use PostgreSQL via Testcontainers.

EF Core entity configurations are in `src/Shopizy.Infrastructure/<Aggregate>/Persistence/<Aggregate>Configurations.cs`. Migrations are in `src/Shopizy.Infrastructure/Migrations/`.

`DbMigrationsHelper` auto-applies pending migrations on startup.

**RowVersion / optimistic concurrency**: `Orders` and `Carts` have a `RowVersion` shadow property (`byte[]`). The configurations declare it as a plain column (`builder.Property<byte[]>("RowVersion")`) without `IsRowVersion()`. SQL Server–specific concurrency tracking (`IsConcurrencyToken = true`, `ValueGenerated = OnAddOrUpdate`, column type `rowversion`) is applied conditionally in `AppDbContext.OnModelCreating` only when `Database.ProviderName == "Microsoft.EntityFrameworkCore.SqlServer"`. Do not add `IsRowVersion()` back to the entity configurations — it breaks PostgreSQL integration tests because Npgsql never auto-updates the column, causing `WHERE "RowVersion" = NULL` (always false) → `DbUpdateConcurrencyException` on every write.

### Security / Authorization

JWT bearer authentication is configured in `Infrastructure/DependencyInjection/AuthenticationRegister.cs`. The current user is resolved from the `"id"` JWT claim via `ICurrentUser` (scoped service in Application).

JWT tokens contain three claim types: `"id"` (user GUID), `ClaimTypes.Role` (role name string), and `"permissions"` (one claim per permission name). `IJwtTokenGenerator.GenerateToken` accepts `UserId`, `role` string, and `IEnumerable<string> permissions`.

Role-based access uses the `UserRole` enum (`Shopizy.Domain/Users/Enums/UserRole.cs`): `Customer = 1`, `Admin = 2`. Each `User` aggregate carries a `Role` property and can be updated via `UpdateRole(UserRole)`.

Permission-based authorization is enforced through `IPolicyEnforcer`. Permission records are seeded via migration. A user's permission set can be replaced atomically via `UpdatePermissions(IList<PermissionId>)`.

Admin role management is exposed at `PATCH api/v1.0/admin/users/{id}/role` (`UpdateUserRoleEndpoint`), which requires the `Admin.UpdateUserRole` permission and dispatches `UpdateUserRoleCommand(UserId, Role, PermissionIds)`.

### Configuration

Key `appsettings.json` sections:
- `ConnectionStrings:DefaultConnection`
- `UsePostgreSql` (bool)
- `JwtSettings` — `Secret`, `Issuer`, `Audience`, `TokenExpirationMinutes`
- `CloudinarySettings` — image uploads
- `StripeSettings` — card payments
- `RedisCacheSettings` — caching endpoint/port

### Error Handling

All handlers return `ErrorOr<T>`. Domain-level errors are defined as static partial classes under `Shopizy.Domain/Common/CustomErrors/CustomErrors.<Aggregate>.cs`. The global exception handler is at `Shopizy.Api/Common/Errors/GlobalExceptionHandler.cs`.

**CustomErrors conventions:**
- Error codes must use the aggregate prefix matching the file, e.g. `"Product.ProductNotFound"`, `"Order.OrderNotFound"`, `"Cart.CartProductNotFound"`. Never use `"User."` as the prefix in a non-User error file.
- `NotFound` errors must use `Error.NotFound(...)`, not `Error.Validation(...)`.
- Persistence failures (repository save failures) use `Error.Failure(...)`.
- Validation rule violations use `Error.Validation(...)`.
- Duplicate/conflict checks use `Error.Conflict(...)`.

**GlobalExceptionHandler** maps unhandled exceptions to HTTP status codes:
- `DbUpdateConcurrencyException` → 409
- `OperationCanceledException` / `TaskCanceledException` → 499
- `ArgumentException` / `InvalidOperationException` → 400
- `UnauthorizedAccessException` → 401
- `TimeoutException` → 503
- Everything else → 500

**CustomResults** (`Shopizy.Api/Endpoints/CustomResults.cs`) converts `IList<Error>` to `IResult`. All-validation error lists become `ValidationProblem` (400); otherwise the first error's `ErrorType` determines the status code (404, 409, 401, 403, 500).
