# DNDTracker Copilot Instructions

## Build and test

The solution targets **.NET 10** (`net10.0`) with nullable reference types and implicit usings enabled.

```powershell
# Restore and build the solution
dotnet restore DNDTracker.sln
dotnet build DNDTracker.sln --no-restore

# Run all tests
dotnet test DNDTracker.sln

# Run the non-integration suite used by CI
dotnet test DNDTracker.sln --filter "Category!=Integration"

# Run one test by its fully qualified name
dotnet test tst\DNDTracker.Application.Tests\DNDTracker.Application.Tests.csproj --filter "FullyQualifiedName~CreateCampaignUseCaseTest.GivenValidRequest_WhenHandle_ThenCampaignIsCreated"
```

There is no dedicated lint or formatting command configured in the repository.

## Architecture

This is a hexagonal, Clean Architecture application using DDD and CQRS:

- `DNDTracker.Domain` owns aggregates (`Campaign` and its `Hero` entities), strong ID types, repository ports, and domain events. Aggregate constructors are private; create and modify aggregates through their factory and behavior methods.
- Commands are in `DNDTracker.Application/UseCases` and implement `ICommandHandler<...>`; queries are intentionally separate in `DNDTracker.Application.Queries/UseCases` and implement `IQueryHandler<...>`. Keep read and write requests/handlers in their respective projects.
- The REST adapter maps HTTP DTOs to MediatR commands or queries. `DNDTracker.Main/Program.cs` is the composition root: it loads the REST controllers, registers both handler assemblies, wires repositories/messaging, applies EF migrations at startup, and initializes RabbitMQ topology.
- PostgreSQL persistence uses EF Core models from `DNDTracker.Vocabulary.Models`, mapped to and from domain objects by extension methods in `DNDTracker.DataAccessObject.Mapping`. Repository implementations belong in `DNDTracker.Outbound.PostgresDb`.
- RabbitMQ is the outbound event transport and the AMQP adapter hosts consumers. Topology is configuration-driven in `src/DNDTracker.Main/appsettings.json`.

## Repository conventions

- Keep domain rules inside aggregate/entity behavior, not repositories. Raise domain events with `AddDomainEvent()`.
- Domain events are currently published explicitly by command handlers (for example, `AddHeroToCampaignCommandHandler` publishes `campaign.DomainEvents` before calling `UpdateAsync`); `DNDTrackerPostgresDbContext` does not automatically dispatch them during `SaveChanges`.
- When adding a RabbitMQ event, add its queue under `RabbitMQ:Topology:Queues` using the event class name as the dictionary key, add a binding using that queue's configured name, and register a consumer as a hosted service through the AMQP adapter when one is needed.
- Controllers should only translate transport DTOs and dispatch via `IMediator`; return HTTP responses there rather than placing domain logic in controllers.
- Repository reads map persistence models to domain objects. Writes map domain objects to persistence models; EF entity configuration lives under `Database\Postgres\Configuration`.
- Tests use xUnit and FluentAssertions. Application tests use dummy repositories in `Behaviors\Dummies`; PostgreSQL and end-to-end coverage use Testcontainers and `WebApplicationFactory` fixtures.

## Local services and secrets

Use Docker Compose for the API, PostgreSQL, RabbitMQ, and New Relic stack:

```powershell
# Requires NEW_RELIC_LICENSE_KEY stored in user-secrets
.\up-local.ps1 -Build
```

For local secrets, use the shared user-secrets ID, not only the project path:

```powershell
dotnet user-secrets set "NEW_RELIC_LICENSE_KEY" "<value>" --id DndTracker
```

## EF Core migrations and local database

The PostgreSQL `DbContext` is in `DNDTracker.Outbound.PostgresDb`, while the executable startup project is `DNDTracker.Main`; always pass both to EF commands. Install the EF CLI once if `dotnet ef` is unavailable:

```powershell
dotnet tool install --global dotnet-ef

# Start only the local PostgreSQL service when API/RabbitMQ/New Relic are not needed
docker compose up -d postgres

# Create a migration after changing persistence models or EF configuration
dotnet ef migrations add <MigrationName> --project src\DNDTracker.Outbound.PostgresDb --startup-project src\DNDTracker.Main --context DNDTrackerPostgresDbContext

# Apply migrations to the configured database
dotnet ef database update --project src\DNDTracker.Outbound.PostgresDb --startup-project src\DNDTracker.Main --context DNDTrackerPostgresDbContext
```

`Program.cs` calls `Database.Migrate()` during API startup after waiting for PostgreSQL, so existing migrations are applied automatically when the API starts. For schema changes, update the persistence model in `DNDTracker.Vocabulary.Models`, its EF configuration in `Database\Postgres\Configuration`, and the domain/persistence mapping extensions before creating the migration. Do not expect EF configuration discovery to require manual registration: the `DbContext` applies all configurations from its assembly.

The API is exposed at `http://localhost:5169`, Scalar documentation at `/scalar/v1`, and RabbitMQ management at `http://localhost:15672`.