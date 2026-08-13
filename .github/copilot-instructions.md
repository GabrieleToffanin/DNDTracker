# DNDTracker Copilot Instructions

## Repository snapshot

DNDTracker is a backend organized with Clean Architecture, DDD, CQRS, and hexagonal boundaries.

- Target framework: `net10.0`
- Entry point: `/home/runner/work/DNDTracker/DNDTracker/src/DNDTracker.Main/Program.cs`
- Main aggregate: `Campaign`
- Main entity inside aggregate: `Hero`
- Persistence: PostgreSQL via EF Core
- Messaging: RabbitMQ
- Observability: Serilog + OpenTelemetry + Prometheus

## Solution map

- `src/DNDTracker.Domain` — aggregates, entities, repository ports, domain events
- `src/DNDTracker.Application` — command handlers only
- `src/DNDTracker.Application.Queries` — query handlers only
- `src/DNDTracker.Inbound.RestAdapter` — controllers and HTTP DTOs
- `src/DNDTracker.Inbound.AmqpAdapter` — RabbitMQ consumers and hosted services
- `src/DNDTracker.Outbound.PostgresDb` — DbContext, EF configuration, migrations, repositories
- `src/DNDTracker.Outbound.RabbitMq` — event publishing and topology initialization
- `src/DNDTracker.DataAccessObject.Mapping` — domain/model mapping extensions
- `src/DNDTracker.Vocabulary` — enums, exceptions, models, value objects
- `tst/*` — unit, adapter, repository, and integration tests

## Architectural rules

### Domain
- Keep business rules in aggregates/entities, not in controllers or repositories.
- Use factory methods such as `Campaign.Create(...)` and `Hero.Create(...)`; do not add public constructors to aggregates.
- Strong IDs (`CampaignId`, `HeroId`) are part of the domain model and should remain inside domain boundaries.
- Raise domain events with `AddDomainEvent()` from entity behavior.

### CQRS
- Put writes in `src/DNDTracker.Application/UseCases/...`.
- Put reads in `src/DNDTracker.Application.Queries/UseCases/...`.
- Do not mix command and query logic in the same handler.
- Register new handlers through the assemblies already loaded in `Program.ConfigureMediatR()`.

### REST adapter
- Controllers should map transport DTOs to MediatR requests and return HTTP responses.
- Do not place domain rules in controllers.
- Keep request/response DTO changes inside the REST adapter unless shared contracts are explicitly needed elsewhere.

### Persistence
- Repository interfaces belong to the domain.
- PostgreSQL implementations belong in `src/DNDTracker.Outbound.PostgresDb/Repositories`.
- EF configurations live in `src/DNDTracker.Outbound.PostgresDb/Database/Postgres/Configuration`.
- Persistence models live in `src/DNDTracker.Vocabulary/Models`.
- Domain/model conversions belong in `src/DNDTracker.DataAccessObject.Mapping`.

### Messaging
- RabbitMQ topology is configuration-driven from `src/DNDTracker.Main/appsettings.json`.
- Domain events are currently published explicitly by application handlers; saving with EF Core does not auto-dispatch them.
- When adding a new event, update queue configuration, binding configuration, and AMQP consumer registration when needed.

## Feature delivery playbook

### Adding or changing domain behavior
1. Update the aggregate/entity behavior first.
2. Keep invariants and validation inside the domain.
3. Raise domain events from the behavior when external reactions are required.
4. Update application handlers only to orchestrate repositories and event publication.

### Adding a new command
1. Create the command under `src/DNDTracker.Application/UseCases/...`.
2. Add a dedicated handler implementing `ICommandHandler<...>`.
3. Load state via the domain repository.
4. Execute domain behavior.
5. Persist changes.
6. Publish queued domain events when needed.
7. Add or update REST endpoints if the command is API-facing.
8. Add unit tests in `tst/DNDTracker.Application.Tests`.

### Adding a new query
1. Create the request and handler in `src/DNDTracker.Application.Queries/UseCases/...`.
2. Read from repositories without introducing domain side effects.
3. Return DTOs or shared read models.
4. Add query/controller tests as needed.

### Adding a persistence change
1. Update the persistence model in `src/DNDTracker.Vocabulary/Models`.
2. Update EF configuration in `src/DNDTracker.Outbound.PostgresDb/Database/Postgres/Configuration`.
3. Update mapping extensions in `src/DNDTracker.DataAccessObject.Mapping`.
4. Add an EF migration using both the Postgres project and the Main startup project.
5. Cover the change with repository or integration tests when behavior changes.

### Adding a RabbitMQ-driven feature
1. Define or reuse the domain event.
2. Publish it from the command handler.
3. Add queue topology entries in `appsettings.json`.
4. Implement a consumer in `src/DNDTracker.Inbound.AmqpAdapter/Consumers` when needed.
5. Register the hosted service through the AMQP adapter extension.

## Testing and validation

Use the existing commands:

```powershell
dotnet restore DNDTracker.sln
dotnet build DNDTracker.sln --no-restore
dotnet test DNDTracker.sln
dotnet test DNDTracker.sln --filter "Category!=Integration"
```

Targeted test examples:

```powershell
dotnet test tst\DNDTracker.Application.Tests\DNDTracker.Application.Tests.csproj

dotnet test tst\DNDTracker.Main.IntegrationTests\DNDTracker.Main.IntegrationTests.csproj
```

Testing guidance:

- Application tests use dummy collaborators in `Behaviors/Dummies`.
- Integration tests use Testcontainers and `WebApplicationFactory`.
- Prefer the narrowest relevant test suite first, then widen if the change spans layers.
- There is no dedicated linter configured.

## Local environment and secrets

For the local stack:

```powershell
.\up-local.ps1 -Build
```

Required secret:

```powershell
dotnet user-secrets set "NEW_RELIC_LICENSE_KEY" "<value>" --id DndTracker
```

Useful local endpoints:

- API: `http://localhost:5169`
- Scalar: `http://localhost:5169/scalar/v1`
- RabbitMQ UI: `http://localhost:15672`
- Grafana: `http://localhost:3000`
- Jaeger: `http://localhost:16686`

## Notes for future maintenance

- Keep repository guidance aligned with the real target framework in the `.csproj` files.
- If CI workflow setup differs from the target framework, update the workflow as part of a future maintenance pass.
