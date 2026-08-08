# AGENTS.md - DNDTracker Development Guide

## 🏗️ Architecture Overview

DNDTracker is a **Clean Architecture + Domain-Driven Design (DDD) + CQRS** application built on .NET 9.0 with hexagonal architecture. Commands and queries are strictly separated into different projects.

**Core Flow:** REST/AMQP Inbound → MediatR → Domain/Application Logic → PostgreSQL/RabbitMQ Outbound

## 📦 Key Components

| Component | Purpose | Files |
|-----------|---------|-------|
| **Domain** | Aggregate roots, entities, domain events, repository interfaces | `src/DNDTracker.Domain/{Campaigns,Heroes}/` |
| **Application** | CQRS command handlers, business logic orchestration | `src/DNDTracker.Application/UseCases/` |
| **Application.Queries** | Query handlers, DTOs, read-side logic (separate project) | `src/DNDTracker.Application.Queries/UseCases/` |
| **Inbound Adapters** | REST controllers, AMQP/RabbitMQ consumers | `src/DNDTracker.Inbound.RestAdapter/`, `src/DNDTracker.Inbound.AmqpAdapter/` |
| **Outbound Adapters** | PostgreSQL repositories, RabbitMQ publishers, in-memory implementations | `src/DNDTracker.Outbounx.PostgresDb/`, `src/DNDTracker.Outbound.RabbitMq/` |
| **SharedKernel** | Base classes (`AggregateRoot<T>`, `Entity`, `DomainEvent`), MediatR interfaces | `src/DNDTracker.SharedKernel/` |

## 🔧 Critical Development Patterns

### Aggregates & Entities
- Aggregate roots inherit from `AggregateRoot<TId>` with strong-typed IDs (e.g., `CampaignId`)
- Always use factory methods in aggregates (not public constructors)
- Domain logic lives in entities, not repositories
- Track domain events in base `Entity` class via `AddDomainEvent()`

**Example:** `src/DNDTracker.Domain/Campaigns/Campaign.cs` - public factory `Create()` method, private constructor

### CQRS Commands
- Handlers inherit from `ICommandHandler<TCommand, TResult>` (via `src/DNDTracker.SharedKernel/Commands/`)
- Request repository, validate, execute domain logic, save
- One handler per command - no shared logic between handlers

**Example:** `src/DNDTracker.Application/UseCases/Campaigns/CreateCampaign/CreateCampaignCommandHandler.cs`

### CQRS Queries
- Handlers in separate `DNDTracker.Application.Queries` project
- Handlers inherit from `IQueryHandler<TQuery, TResult>`
- Direct repository queries, return DTOs, no domain events

**Example:** `src/DNDTracker.Application.Queries/UseCases/GetCampaign/GetCampaignByNameHandler.cs`

### REST Integration
- Controllers use MediatR's `mediator.Send()` to dispatch commands/queries
- DTOs in `src/DNDTracker.Inbound.RestAdapter/Dtos/`
- Controllers use fluent response builders (see `CampaignController.cs`)

### Async Message Flow
- Domain events published by entities trigger outbound messages
- `HeroAddedEventHostedService` consumes from RabbitMQ, publishes to internal event bus
- Event handlers can trigger side effects without domain coupling

## 🧪 Testing Conventions

- **Framework:** xUnit + FluentAssertions + FsCheck.Xunit + Testcontainers
- **Pattern:** Arrange-Act-Assert with Dummy objects for dependencies
- **Test doubles:** Located in `Behaviors/Dummies/` folders
- **Integration tests:** Use Testcontainers for real PostgreSQL instances

**Example:** `tst/DNDTracker.Application.Tests/CreateCampaignUseCaseTest.cs`

```csharp
[Fact]
public async Task CreateCampaign_WithValidName_ShouldSucceed()
{
    // Arrange
    var repository = new DummyCampaignRepository();
    var handler = new CreateCampaignCommandHandler(repository);
    
    // Act
    var result = await handler.Handle(new CreateCampaignCommand("New Campaign"), CancellationToken.None);
    
    // Assert
    result.Should().NotBeEmpty();
}
```

## 🚀 Build & Deployment Workflows

```bash
# Build entire solution
dotnet build DNDTracker.sln

# Run with Docker Compose (includes API, PostgreSQL, RabbitMQ, ELK stack, OpenTelemetry)
docker-compose up --build

# Database migrations (from src/DNDTracker.Outbounx.PostgresDb directory)
dotnet ef migrations add <MigrationName> --context DNDTrackerPostgresDbContext
dotnet ef database update --context DNDTrackerPostgresDbContext

# Run all tests
dotnet test

# Run specific test project
dotnet test tst/DNDTracker.Main.IntegrationTests

# Deploy to Kubernetes with Helm
helm dependency update ./dndtracker
helm install dndtracker ./dndtracker -f values.yaml
```

## 🔗 Dependency Injection Setup

Entry point: `src/DNDTracker.Main/Program.cs` (87 lines)
- MediatR registration with custom handlers
- PostgreSQL DbContext with auto-migrations
- RabbitMQ connection setup
- AMQP adapter background services
- OpenTelemetry/ELK stack configuration

## 📊 Domain Events & Publishing

1. Entity raises domain event via `AddDomainEvent(event)`
2. Repository saves entity (EF Core tracks events)
3. Application publishes events via `IEventPublisher` 
4. Outbound adapters (RabbitMQ, in-memory) handle event distribution
5. Inbound adapters (AMQP) consume and trigger downstream handlers

**Key:** Events flow outward only; domain layer never depends on infrastructure.

## 🎯 When Adding New Features

1. **Define aggregate:** Create entity in `src/DNDTracker.Domain/{Entity}/`
2. **Add command:** Create handler in `src/DNDTracker.Application/UseCases/{Feature}/`
3. **Add query:** Create handler in `src/DNDTracker.Application.Queries/UseCases/{Feature}/`
4. **Add repository:** Implement in `src/DNDTracker.Outbounx.PostgresDb/`
5. **Add REST endpoint:** Add method to controller in `src/DNDTracker.Inbound.RestAdapter/Controllers/`
6. **Add tests:** Test handler, repository, and controller in `tst/`

**Important:** Commands and queries are **never** in the same handler; use separate MediatR requests.

## 🐳 Services & Ports

| Service | Port | URL |
|---------|------|-----|
| DNDTracker API | 5169 | http://localhost:5169 |
| Scalar API Docs | 5169 | http://localhost:5169/scalar/v1 |
| PostgreSQL | 5432 | Host=localhost;Database=dndtracker |
| RabbitMQ Management | 15672 | http://localhost:15672 (guest/guest) |
| Kibana (Logs) | 5601 | http://localhost:5601 |
| OpenTelemetry OTLP | 4317 | localhost:4317 (gRPC) |

## 📋 Key Files for Reference

- `src/DNDTracker.SharedKernel/Primitives/` - Base classes for entities, aggregates
- `src/DNDTracker.Domain/IEventPublisher.cs` - Event publishing interface
- `src/DNDTracker.Main/Program.cs` - Full DI setup
- `docker-compose.yml` - All services configuration
- `dndtracker/values.yaml` - Kubernetes Helm values

