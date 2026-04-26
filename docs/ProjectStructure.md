# Shopizy Project Structure Reference

Last updated: 2026-04-25

This document is a quick map of the repository to guide safe, targeted changes.
Use it before editing to choose the right layer and keep boundaries clear.

## Solution Overview

```
shopizy/
├─ Shopizy.sln
├─ Directory.Build.props
├─ Directory.Packages.props
├─ README.md
├─ docs/
├─ src/
├─ tests/
└─ Bruno/
```

## Root-Level Files and Folders

- `Shopizy.sln`: Solution entry point.
- `Directory.Build.props`: Shared MSBuild settings.
- `Directory.Packages.props`: Centralized NuGet package versions.
- `README.md`: General setup and usage.
- `docs/`: Project documentation.
- `src/`: Production code.
- `tests/`: Unit and integration tests.
- `Bruno/`: API collection assets for manual testing.

## Source Projects (`src/`)

### `src/Shopizy.Api`

API host and HTTP edge of the system.

- `Program.cs`: App startup and middleware wiring.
- `DependencyInjectionRegister.cs`: API-level service registrations.
- `Endpoints/`: HTTP endpoints grouped by feature.
- `Common/`: Shared API concerns.
- `appsettings*.json`: Environment-specific settings.

### `src/Shopizy.Application`

Application layer use cases and orchestration.

- Feature folders: `Admin/`, `Auth/`, `Products/`, `Orders/`, `Users/`, etc.
- `Common/`: Shared application patterns/utilities.
- `DependencyInjectionRegister.cs`: Application service wiring.

### `src/Shopizy.Contracts`

DTOs/contracts shared across boundaries.

- Feature folders mirror business modules (`Product/`, `Order/`, `User/`, etc.).
- Keep transport models here, not domain behavior.

### `src/Shopizy.Domain`

Core business model and domain rules.

- Feature folders: `Products/`, `Orders/`, `PromoCodes/`, etc.
- `Common/`: Domain base abstractions/shared primitives.
- `Permissions/`: Authorization-related domain concepts.

### `src/Shopizy.Infrastructure`

Technical implementations for external concerns.

- `DependencyInjection/` and `DependencyInjectionRegister.cs`: Infra wiring.
- `Migrations/`: Database migrations.
- `ExternalServices/`: Third-party integrations.
- `Messaging/` and `Outbox/`: Messaging and reliability patterns.
- `Security/`, `Services/`, `Common/`: Cross-cutting infrastructure code.
- `RequestPipeline.cs`, `LoggerMessages.cs`: Pipeline/logging helpers.

### `src/Shopizy.SharedKernel`

Cross-cutting shared abstractions used by multiple projects.

- `Application/`: Shared application-level primitives.
- `Domain/`: Shared domain-level primitives.

## Tests (`tests/`)

### `tests/Shopizy.Api.IntegrationTests`

End-to-end API behavior with test host setup.

- `BaseIntegrationTest.cs`
- `IntegrationTestWebAppFactory.cs`
- Feature-oriented test folders (`Products/`, `Orders/`, etc.)

### `tests/Shopizy.Application.UnitTests`

Unit tests for application handlers/services by feature.

### `tests/Shopizy.Domain.UnitTests`

Unit tests for domain behavior and invariants.

### `tests/Shopizy.Infrastructure.UnitTests`

Unit tests for infrastructure services, security, and integrations.

## API Collections (`Bruno/`)

### `Bruno/Shopizy`

- `collection.bru`: Main API collection.
- `environments/`: Environment configurations.
- Feature folders (for example `Product/`, `User/`) for endpoint request sets.

## Existing Docs (`docs/`)

- `Api.md`: API-focused notes.
- `Domain.md`: Domain-focused notes.
- `ProjectStructure.md`: This structure guide.

## Change Routing Checklist

When making a change, route work using this quick mapping:

1. API route/contract handling -> `Shopizy.Api` and possibly `Shopizy.Contracts`.
2. Use case/business flow -> `Shopizy.Application`.
3. Business rules/invariants/entities -> `Shopizy.Domain`.
4. Persistence/external systems/security plumbing -> `Shopizy.Infrastructure`.
5. Shared abstractions -> `Shopizy.SharedKernel`.
6. Add or update tests under matching project in `tests/`.

## Notes for Ongoing Maintenance

- Keep this file updated when adding/removing major feature folders or projects.
- Prefer feature-aligned changes across `Api`, `Application`, `Domain`, `Infrastructure`, and tests.
- Avoid adding behavior to `Contracts`; keep it for transport models only.