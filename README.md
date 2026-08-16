[![.NET](https://github.com/GabrieleToffanin/DNDTracker/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/GabrieleToffanin/DNDTracker/actions/workflows/build-and-test.yml)

# DNDTracker

DNDTracker is a .NET 10 backend for managing Dungeons & Dragons campaigns, heroes, and asynchronous domain events. The codebase follows Clean Architecture, DDD, CQRS, and hexagonal architecture.

## What is in the repository

- **Domain model** for campaigns and heroes
- **Command side** for write use cases
- **Query side** for read use cases
- **REST API** exposed through ASP.NET Core controllers
- **RabbitMQ integration** for domain-event publishing and consumption
- **PostgreSQL persistence** with EF Core migrations
- **Observability stack** with OpenTelemetry, Serilog, Prometheus, Grafana, Jaeger, and Loki
- **Helm chart** for Kubernetes deployment

## Solution structure

```text
src/
├── DNDTracker.Domain                 # Aggregates, entities, domain events, repository ports
├── DNDTracker.Application            # Command handlers and write-side orchestration
├── DNDTracker.Application.Queries    # Query handlers and read-side orchestration
├── DNDTracker.Inbound.RestAdapter    # HTTP DTOs and controllers
├── DNDTracker.Inbound.AmqpAdapter    # RabbitMQ consumers / hosted services
├── DNDTracker.Outbound.PostgresDb    # EF Core DbContext, repositories, migrations
├── DNDTracker.Outbound.RabbitMq      # RabbitMQ publisher and topology setup
├── DNDTracker.DataAccessObject.Mapping # Domain <-> persistence mapping
├── DNDTracker.SharedKernel           # CQRS abstractions and base primitives
├── DNDTracker.Vocabulary             # Enums, exceptions, persistence models, value objects
└── DNDTracker.Main                   # Composition root and application startup

tst/
├── DNDTracker.Application.Tests
├── DNDTracker.Domain.Tests
├── DNDTracker.Inbound.RestAdapter.Tests
├── DNDTracker.BackendInfrastructure.PostgresDb.Tests
└── DNDTracker.Main.IntegrationTests
```

## Architecture at a glance

### Domain
`DNDTracker.Domain` owns the business model.

- `Campaign` is the aggregate root
- `Hero` is an entity inside the campaign aggregate
- IDs are strongly typed (`CampaignId`, `HeroId`)
- Domain events are raised from entities with `AddDomainEvent()`

### CQRS split
- **Commands** live in `src/DNDTracker.Application/UseCases`
- **Queries** live in `src/DNDTracker.Application.Queries/UseCases`
- Controllers translate HTTP payloads into MediatR requests

### Infrastructure
- PostgreSQL repositories are implemented in `DNDTracker.Outbound.PostgresDb`
- RabbitMQ event publishing is implemented in `DNDTracker.Outbound.RabbitMq`
- Consumer hosting is implemented in `DNDTracker.Inbound.AmqpAdapter`
- `DNDTracker.Main/Program.cs` wires the application together, applies migrations at startup, and initializes RabbitMQ topology

## Main API endpoints

The REST adapter currently exposes:

- `GET /api/Campaign` — list campaigns
- `GET /api/Campaign/{campaignName}` — fetch a campaign by name
- `POST /api/Campaign` — create a campaign
- `POST /api/Campaign/{campaignName}/heroes` — add a hero to a campaign
- `GET /api/Health` — controller-based health endpoint
- `GET /health` — lightweight health endpoint mapped in `Program.cs`
- `/scalar/v1` — Scalar API reference UI
- `/openapi/v1.json` — generated OpenAPI document
- `/metrics` — Prometheus scraping endpoint

### Example request: create a campaign

```json
{
  "campaignName": "Curse of Strahd",
  "campaignDescription": "Gothic horror campaign",
  "campaignImage": "strahd.jpg",
  "createdDate": "2026-08-13T12:00:00Z",
  "isActive": true
}
```

### Example request: add a hero

```json
{
  "hero": {
    "name": "Ludwin",
    "class": "Paladin",
    "race": "HalfElf",
    "alignment": "Good",
    "level": 1,
    "experience": 0,
    "hitPoints": 10,
    "hitDice": "D4"
  }
}
```

## Prerequisites

- .NET 10 SDK
- Docker and Docker Compose
- Optional: `dotnet-ef` tool for migration authoring
- Optional: user secret `NEW_RELIC_LICENSE_KEY` for the local observability stack

## Build and test

```bash
dotnet restore DNDTracker.sln
dotnet build DNDTracker.sln --no-restore

dotnet test DNDTracker.sln
dotnet test DNDTracker.sln --filter "Category!=Integration"
```

Useful targeted commands:

```bash
dotnet test tst/DNDTracker.Application.Tests/DNDTracker.Application.Tests.csproj

dotnet test tst/DNDTracker.Main.IntegrationTests/DNDTracker.Main.IntegrationTests.csproj
```

> There is no dedicated lint or formatting command configured in the repository.

## Run locally

### Fastest full-stack option

Copy the environment template and start Docker Compose:

```bash
cp .env.example .env
docker compose up --build
```

`NEW_RELIC_LICENSE_KEY` in `.env` is optional. If left empty the full stack still starts; telemetry is exported locally to Jaeger, Prometheus, and Loki. Set the key only if you want to forward telemetry to New Relic:

```bash
# .env
NEW_RELIC_LICENSE_KEY=<your-key>
```

#### Alternative: PowerShell helper (loads the key from dotnet user-secrets)

```powershell
dotnet user-secrets set "NEW_RELIC_LICENSE_KEY" "<value>" --id DndTracker
.\up-local.ps1 -Build
```

### Docker Compose services

`docker-compose.yml` starts:

- `dndtracker.api`
- `dndtracker.ui` (Blazor WebAssembly)
- `postgres`
- `postgres-init`
- `rabbitmq`
- `otel-collector`
- `jaeger`
- `prometheus`
- `loki`
- `grafana`

Useful local URLs:

| Service | URL |
|---|---|
| Blazor tabletop UI | `http://localhost:5173` |
| API | http://localhost:5169 |
| Scalar docs | http://localhost:5169/scalar/v1 |
| RabbitMQ UI | http://localhost:15672 |
| Grafana | http://localhost:3000 |
| Jaeger | http://localhost:16686 |
| Prometheus | http://localhost:9090 |
| Loki | http://localhost:3100 |

### Run only the API from the CLI

If PostgreSQL and RabbitMQ are already available locally:

```bash
dotnet run --project src/DNDTracker.Main/DNDTracker.Main.csproj
```

## Database and migrations

The EF Core `DbContext` is in `DNDTracker.Outbound.PostgresDb`, while the startup project is `DNDTracker.Main`.

```bash
docker compose -f docker-compose.yml up -d postgres

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

Notes:

- `Program.cs` waits for PostgreSQL and calls `Database.Migrate()` on startup
- EF configurations are discovered automatically from the `DNDTracker.Outbound.PostgresDb` assembly
- Persistence changes usually require updates to:
  - `DNDTracker.Vocabulary.Models`
  - `DNDTracker.Outbound.PostgresDb/Database/Postgres/Configuration`
  - `DNDTracker.DataAccessObject.Mapping`

## Messaging

RabbitMQ topology is configured in `src/DNDTracker.Main/appsettings.json`.

Current queues and bindings include:

- `HeroAddedDomainEvent` → `dndtracking.campaign.hero-added`
- `SpellLearnedDomainEvent` → `dndtracking.campaigns.spell-learned`
- Exchange: `dnd.events`

When adding a new event:

1. Raise the event from the domain model
2. Publish it from the application handler
3. Add queue and binding configuration in `appsettings.json`
4. Register a consumer hosted service when needed

## Testing strategy

- **Application tests** use dummy repositories and publishers
- **Domain tests** validate domain behavior and repository contracts
- **REST adapter tests** validate controller behavior
- **PostgreSQL tests** cover repository behavior against Postgres
- **Integration tests** use Testcontainers for PostgreSQL and RabbitMQ plus `WebApplicationFactory`

## Deployment

The `dndtracker/` directory contains the Helm chart.

```bash
helm dependency update dndtracker
helm install dndtracker dndtracker -f dndtracker/values.yaml
```

## Developer guidance for AI agents and contributors

- `.github/copilot-instructions.md` contains Copilot-specific repository guidance
- `AGENTS.md` contains detailed feature-delivery guidance for autonomous agents
