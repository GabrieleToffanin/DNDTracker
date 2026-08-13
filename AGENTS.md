# AGENTS.md — DNDTracker Agent Development Playbook

This file is for coding agents and autonomous contributors working inside DNDTracker.

## 1. Mission

Deliver small, correct changes that respect the repository architecture:

- Clean Architecture
- DDD
- CQRS
- Hexagonal boundaries
- ASP.NET Core on .NET 10
- PostgreSQL persistence
- RabbitMQ-based event flow

Prefer minimal, coherent changes over broad refactors.

## 2. Fast project orientation

Repository root: `.`

### Important directories

| Path | Purpose |
|---|---|
| `src/DNDTracker.Domain` | Aggregates, entities, domain events, repository interfaces |
| `src/DNDTracker.Application` | Command handlers and write-side use cases |
| `src/DNDTracker.Application.Queries` | Query handlers and read-side use cases |
| `src/DNDTracker.Inbound.RestAdapter` | HTTP controllers and transport DTOs |
| `src/DNDTracker.Inbound.AmqpAdapter` | RabbitMQ consumers and hosted services |
| `src/DNDTracker.Outbound.PostgresDb` | DbContext, EF configuration, migrations, repositories |
| `src/DNDTracker.Outbound.RabbitMq` | Event publisher and topology initialization |
| `src/DNDTracker.DataAccessObject.Mapping` | Domain/persistence mapping extensions |
| `src/DNDTracker.Vocabulary` | Enums, exceptions, persistence models, value objects |
| `src/DNDTracker.Main` | Composition root and host startup |
| `tst` | Automated tests |
| `dndtracker` | Helm chart |

### Runtime entry points

- Main program: `src/DNDTracker.Main/Program.cs`
- REST API controller surface: `src/DNDTracker.Inbound.RestAdapter/Controllers`
- RabbitMQ topology config: `src/DNDTracker.Main/appsettings.json`
- Local stack: `docker-compose.yml`
- Local bootstrap script: `up-local.ps1`

## 3. Architectural constraints

### Domain rules belong in the domain

Use aggregates and entities to enforce behavior.

- `Campaign` is the aggregate root.
- `Hero` lives inside the campaign aggregate.
- Keep validation and invariants in aggregate/entity methods.
- Raise domain events from the entity/aggregate layer.

Do not move business rules into:

- controllers
- repositories
- mapping layers
- startup code

### CQRS is strict here

- Commands go in `src/DNDTracker.Application/UseCases/...`
- Queries go in `src/DNDTracker.Application.Queries/UseCases/...`
- Do not combine read and write logic in the same handler.
- Do not add domain side effects inside query handlers.

### Repositories are ports + adapters

- Interfaces stay in the domain project.
- PostgreSQL implementations stay in `src/DNDTracker.Outbound.PostgresDb/Repositories`.
- Repository methods load or save aggregates; they should not become a second business layer.

### Mapping boundaries matter

Whenever persistence shape changes:

- update models in `src/DNDTracker.Vocabulary/Models`
- update EF configuration in `src/DNDTracker.Outbound.PostgresDb/Database/Postgres/Configuration`
- update mappings in `src/DNDTracker.DataAccessObject.Mapping`

## 4. Canonical implementation patterns

### Pattern A — add a command-backed feature

1. Start from the domain behavior.
2. Add or update aggregate/entity methods.
3. Add a command and handler under `src/DNDTracker.Application/UseCases`.
4. Load state via repository interfaces.
5. Execute domain behavior.
6. Publish domain events explicitly if the use case requires it.
7. Persist via repository.
8. Expose the use case through the REST adapter only if it is externally reachable.
9. Add focused tests in application, adapter, and integration layers as needed.

### Pattern B — add a query-backed feature

1. Create a request and handler under `src/DNDTracker.Application.Queries/UseCases`.
2. Read through repositories or existing read-oriented adapters.
3. Return DTOs or shared read models.
4. Keep query handlers side-effect free.
5. Add query/controller tests.

### Pattern C — add a new domain event workflow

1. Raise the event from aggregate/entity behavior.
2. Publish the event in the application handler.
3. Add queue metadata under `RabbitMQ:Topology:Queues` in `src/DNDTracker.Main/appsettings.json`.
4. Add a matching binding under `RabbitMQ:Topology:Bindings`.
5. Implement a consumer when the event needs inbound processing.
6. Register the consumer/hosted service through the AMQP adapter setup.
7. Add tests around the use case and any new consumer behavior.

### Pattern D — change persistence

1. Update the persistence model.
2. Update EF configuration.
3. Update the mapping extensions.
4. Generate a migration.
5. Run repository/integration tests.
6. Verify startup migration still succeeds.

## 5. Feature-development checklist for agents

When implementing a feature, work through this checklist.

### Understand the change
- Identify whether it is domain, command, query, API, persistence, messaging, or deployment work.
- Find the existing vertical slice closest to the requested behavior.
- Reuse existing patterns before introducing new ones.

### Touch the right layers only
- Domain-only change: stay in Domain + tests unless orchestration must change.
- New write use case: Domain + Application + maybe REST + tests.
- New read use case: Application.Queries + maybe REST + tests.
- DB schema change: Vocabulary + EF config + mapping + migration + tests.
- Event change: Domain + Application + RabbitMQ config + consumer + tests.

### Validate completeness
- DTO updated if API contract changed
- Handler registered by assembly scanning pattern
- Mapping updated if persistence shape changed
- Migration added if schema changed
- Tests added/updated at the lowest effective level
- Documentation updated when workflow or setup changed

## 6. Testing playbook

### Default commands

```bash
dotnet restore DNDTracker.sln
dotnet build DNDTracker.sln --no-restore
dotnet test DNDTracker.sln
dotnet test DNDTracker.sln --filter "Category!=Integration"
```

### Which tests to run

| Change type | Minimum validation |
|---|---|
| Domain behavior | `tst/DNDTracker.Domain.Tests` and/or `tst/DNDTracker.Application.Tests` |
| Command handler | `tst/DNDTracker.Application.Tests` |
| Query/controller change | `tst/DNDTracker.Inbound.RestAdapter.Tests` |
| PostgreSQL repository change | `tst/DNDTracker.BackendInfrastructure.PostgresDb.Tests` |
| Startup / middleware / cross-layer behavior | `tst/DNDTracker.Main.IntegrationTests` |
| Broad change | relevant focused suite first, then `dotnet test DNDTracker.sln` |

### Existing testing patterns

- xUnit is the test framework.
- FluentAssertions is the assertion library.
- Dummy collaborators live under `Behaviors/Dummies`.
- Integration tests use Testcontainers.
- API integration tests use `WebApplicationFactory<Program>`.

### Testing guidance for agents

- Start narrow: run the smallest relevant suite first.
- Expand only when the change crosses boundaries.
- If a handler orchestrates domain events, verify both state change and event publication.
- If a controller changes, verify HTTP status codes and request/response mapping.
- If persistence changes, verify mapping round-trips and EF behavior.

## 7. Local runtime and infrastructure guide

### Full local stack

```powershell
.\up-local.ps1 -Build
```

This script:

- reads `NEW_RELIC_LICENSE_KEY` from user-secrets
- exports it into the local shell environment
- starts `docker compose up`

### Required local secret

```powershell
dotnet user-secrets set "NEW_RELIC_LICENSE_KEY" "<value>" --id DndTracker
```

### Useful URLs

| Service | URL |
|---|---|
| API | `http://localhost:5169` |
| Scalar docs | `http://localhost:5169/scalar/v1` |
| RabbitMQ UI | `http://localhost:15672` |
| Grafana | `http://localhost:3000` |
| Jaeger | `http://localhost:16686` |
| Prometheus | `http://localhost:9090` |

## 8. EF Core migration workflow

Use both the database project and the startup project.

```bash
dotnet tool install --global dotnet-ef

dotnet ef migrations add <MigrationName> \
  --project src/DNDTracker.Outbound.PostgresDb \
  --startup-project src/DNDTracker.Main \
  --context DNDTrackerPostgresDbContext

dotnet ef database update \
  --project src/DNDTracker.Outbound.PostgresDb \
  --startup-project src/DNDTracker.Main \
  --context DNDTrackerPostgresDbContext
```

Agent reminders:

- `Program.cs` already applies migrations on startup.
- The DbContext discovers entity configurations automatically from its assembly.
- A schema change without mapping updates is usually incomplete.

## 9. HTTP/API conventions

Controllers should:

- accept transport DTOs
- translate them into commands/queries
- call `IMediator.Send(...)`
- return HTTP responses

Controllers should not:

- contain domain rules
- perform direct persistence logic
- become orchestration-heavy when a handler should own the workflow

Current public controller surface is centered on campaigns and health.

## 10. Messaging conventions

RabbitMQ is part of the normal architecture, not an afterthought.

When editing event-driven behavior:

- ensure event names stay aligned with topology keys
- ensure queue names used in bindings match configured queue names
- ensure consumer registration exists when a consumer is introduced
- verify handlers explicitly publish domain events when expected

## 11. Documentation expectations

Update documentation when changes affect:

- developer setup
- runtime dependencies
- migration workflow
- API shape
- agent/contributor guidance

Primary docs in this repo:

- `README.md` — contributor/user-facing overview
- `.github/copilot-instructions.md` — Copilot-specific guidance
- `AGENTS.md` — autonomous agent delivery guide

## 12. Common mistakes to avoid

- Adding business rules to controllers
- Mixing commands and queries in the same handler or project
- Forgetting domain-to-model mapping updates after persistence changes
- Forgetting migrations after schema changes
- Assuming EF Core auto-publishes domain events
- Adding RabbitMQ consumers without topology config
- Running only one narrow test suite for a broad cross-layer change
- Documenting the wrong target framework

## 13. Done definition for agent-delivered work

A feature is only done when all relevant items below are true:

- Architecture boundaries are still respected
- The right projects were changed and unrelated layers were left alone
- Tests relevant to the touched layers pass
- Persistence/messaging wiring is complete when applicable
- Documentation is updated when setup or workflow changed
- No secrets were introduced

## 14. Quick commands

```bash
# Restore and build
dotnet restore DNDTracker.sln
dotnet build DNDTracker.sln --no-restore

# Run all tests
dotnet test DNDTracker.sln

# Run the non-integration command used by CI
dotnet test DNDTracker.sln --filter "Category!=Integration"

# Run one test project
dotnet test tst/DNDTracker.Application.Tests/DNDTracker.Application.Tests.csproj

# Start only PostgreSQL if needed for local DB work
docker compose up -d postgres
```
