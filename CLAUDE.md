# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

DNDTracker is a Campaign Tracker for Dungeons & Dragons built with C# and .NET 9.0. It follows a Clean Architecture pattern with Domain-Driven Design (DDD) principles.

## Architecture

The solution implements Clean Architecture with Hexagonal Architecture patterns:

### Core Layers
- **Domain** (`DNDTracker.Domain`): Contains entities, domain events, and repository interfaces
- **Application** (`DNDTracker.Application`): Contains command handlers for business operations
- **Application.Queries** (`DNDTracker.Application.Queries`): Contains query handlers for read operations
- **SharedKernel**: Common primitives like `AggregateRoot<T>`, `Entity`, and `DomainEvent`
- **Vocabulary**: Enums, exceptions, models, and value objects

### Adapters (Infrastructure)
- **Inbound Adapters**: 
  - `DNDTracker.Inbound.RestAdapter`: REST API controllers
  - `DNDTracker.Inbound.InMemoryAdapter`: In-memory mediator for testing
- **Outbound Adapters**:
  - `DNDTracker.Outbounx.PostgresDb`: PostgreSQL repository implementations
  - `DNDTracker.Outbound.InMemoryAdapter`: In-memory implementations for messaging

### Main Application
- **DNDTracker.Main**: Entry point with dependency injection setup

## Development Commands

### Building and Running
```bash
# Build entire solution
dotnet build DNDTracker.sln

# Run the main API
dotnet run --project src/DNDTracker.Main

# Start with Docker Compose (includes PostgreSQL and RabbitMQ)
docker-compose up --build
```

### Testing
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tst/DNDTracker.Application.Tests
dotnet test tst/DNDTracker.Domain.Tests
dotnet test tst/DNDTracker.Inbound.RestAdapter.Tests
dotnet test tst/DNDTracker.Main.IntegrationTests

# Run integration tests with database
dotnet test tst/DNDTracker.BackendInfrastructure.PostgresDb.Tests
```

### Database Operations
```bash
# Add new migration (from src/DNDTracker.Outbounx.PostgresDb directory)
dotnet ef migrations add <MigrationName> --context DNDTrackerPostgresDbContext

# Update database
dotnet ef database update --context DNDTrackerPostgresDbContext
```

## Key Domain Concepts

### Aggregates
- **Campaign**: Main aggregate root with heroes, uses `CampaignId` as strong-typed identifier
- **Hero**: Entity within Campaign aggregate, uses `HeroId` as strong-typed identifier

### Domain Events
- `HeroAddedDomainEvent`: Triggered when a hero is added to a campaign
- `SpellLearnedDomainEvent`: Triggered when a hero learns a spell

### CQRS Pattern
Commands and queries are separated:
- Commands in `DNDTracker.Application/UseCases/`
- Queries in `DNDTracker.Application.Queries/UseCases/`
- Uses MediatR for command/query handling

## Development Environment

### Docker Services
The `compose.yaml` provides:
- **API**: Runs on ports 5169:8080 and 8081:8081
- **PostgreSQL**: Port 5432 (user: postgres, password: postgres, db: dndtracker)
- **RabbitMQ**: Port 5672 (AMQP), 15672 (Management UI)

### Configuration
- Connection strings configured via `ConnectionStrings__DefaultConnection`
- Scalar API documentation enabled in development
- Auto-migration on startup with connection retry logic

## Testing Framework
- **xUnit** for unit and integration tests
- **FluentAssertions** for readable assertions
- **FsCheck.Xunit** for property-based testing
- **Testcontainers** for integration tests with real databases
- Test specifications follow behavior-driven patterns

## Project Structure Notes
- `src/` contains all application code
- `tst/` contains all test projects
- `tools/` contains SDK and utilities
- `dndtracker/` contains Helm charts for Kubernetes deployment
- Tests follow the naming convention `<ProjectName>.Tests`