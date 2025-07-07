[![.NET](https://github.com/GabrieleToffanin/DNDTracker/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/GabrieleToffanin/DNDTracker/actions/workflows/build-and-test.yml)

# DNDTracker

A Campaign Tracker for Dungeons & Dragons built with C# and .NET 9.0, following Clean Architecture patterns with Domain-Driven Design (DDD) principles.

## 🏗️ Architecture Overview

DNDTracker implements Clean Architecture with Hexagonal Architecture patterns:

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

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK
- Docker and Docker Compose

### Running with Docker Compose

The easiest way to run the entire application stack is using Docker Compose:

```bash
# Clone the repository
git clone https://github.com/GabrieleToffanin/DNDTracker.git
cd DNDTracker

# Start all services
docker-compose up --build
```

This will start all services including the API, database, message queue, and monitoring stack.

### Manual Build and Run

```bash
# Build entire solution
dotnet build DNDTracker.sln

# Run the main API
dotnet run --project src/DNDTracker.Main
```

## 📋 Docker Compose Services

The `docker-compose.yml` file orchestrates multiple services:

### Core Application Services

#### DNDTracker API (`dndtracker.api`)
- **Purpose**: Main .NET API application
- **Ports**: 
  - `5169:8080` - HTTP API endpoint
  - `8081:8081` - Additional endpoint
- **Links**: 
  - API: http://localhost:5169
  - Scalar API Documentation: http://localhost:5169/scalar/v1

#### PostgreSQL Database (`postgres`)
- **Purpose**: Primary database for application data
- **Port**: `5432:5432`
- **Credentials**: 
  - Username: `postgres`
  - Password: `postgres`
  - Database: `dndtracker`
- **Connection**: `Host=localhost;Port=5432;Database=dndtracker;Username=postgres;Password=postgres`

#### RabbitMQ Message Broker (`rabbitmq`)
- **Purpose**: Message queue for asynchronous communication
- **Ports**: 
  - `5672:5672` - AMQP protocol
  - `15672:15672` - Management UI
- **Links**: 
  - Management UI: http://localhost:15672
  - Credentials: `guest/guest`

### Monitoring & Observability Stack (ELK + OpenTelemetry)

#### Elasticsearch (`es01`)
- **Purpose**: Search and analytics engine for logs and metrics
- **Port**: `9200:9200` (configurable via `ES_PORT` env var)
- **Link**: https://localhost:9200
- **Credentials**: `elastic/${ELASTIC_PASSWORD}`

#### Kibana (`kibana`)
- **Purpose**: Data visualization and exploration for Elasticsearch
- **Port**: `5601:5601` (configurable via `KIBANA_PORT` env var)
- **Link**: http://localhost:5601
- **Credentials**: `kibana_system/${KIBANA_PASSWORD}`

#### Logstash (`logstash01`)
- **Purpose**: Data processing pipeline for logs
- **Function**: Ingests, transforms, and ships logs to Elasticsearch

#### Metricbeat (`metricbeat01`)
- **Purpose**: Lightweight shipper for system and service metrics
- **Function**: Collects metrics from Docker containers and system

#### Filebeat (`filebeat01`)
- **Purpose**: Lightweight shipper for logs
- **Function**: Collects and ships log files to Elasticsearch

#### OpenTelemetry Collector (`otel-collector`)
- **Purpose**: Vendor-agnostic telemetry data collection
- **Ports**: 
  - `4317:4317` - OTLP gRPC receiver
  - `4318:4318` - OTLP HTTP receiver
  - `8888:8888` - Prometheus metrics
  - `8889:8889` - Prometheus exporter metrics

#### Setup Service (`setup`)
- **Purpose**: Initializes SSL certificates and configures the ELK stack
- **Function**: Generates certificates and sets up user passwords

## 🔧 Environment Configuration

The stack requires environment variables. Create a `.env` file with:

```env
# Elastic Stack Configuration
STACK_VERSION=8.11.0
CLUSTER_NAME=docker-cluster
LICENSE=basic
ES_PORT=9200
KIBANA_PORT=5601
ES_MEM_LIMIT=1073741824
KB_MEM_LIMIT=1073741824

# Security
ELASTIC_PASSWORD=your_elastic_password
KIBANA_PASSWORD=your_kibana_password
ENCRYPTION_KEY=your_32_char_encryption_key_here
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run specific test projects
dotnet test tst/DNDTracker.Application.Tests
dotnet test tst/DNDTracker.Domain.Tests
dotnet test tst/DNDTracker.Inbound.RestAdapter.Tests
dotnet test tst/DNDTracker.Main.IntegrationTests
dotnet test tst/DNDTracker.BackendInfrastructure.PostgresDb.Tests
```

## 🗄️ Database Operations

```bash
# Add new migration (from src/DNDTracker.Outbounx.PostgresDb directory)
dotnet ef migrations add <MigrationName> --context DNDTrackerPostgresDbContext

# Update database
dotnet ef database update --context DNDTrackerPostgresDbContext
```

## 📊 Key Domain Concepts

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

## 🔗 Service URLs Summary

| Service | URL | Purpose |
|---------|-----|---------|
| DNDTracker API | http://localhost:5169 | Main application API |
| API Documentation | http://localhost:5169/scalar/v1 | Scalar API docs |
| PostgreSQL | localhost:5432 | Database connection |
| RabbitMQ Management | http://localhost:15672 | Message queue management |
| Elasticsearch | https://localhost:9200 | Search engine |
| Kibana | http://localhost:5601 | Log/metrics visualization |
| OTLP gRPC | localhost:4317 | OpenTelemetry gRPC |
| OTLP HTTP | localhost:4318 | OpenTelemetry HTTP |
| Prometheus Metrics | localhost:8888 | Collector metrics |

## 🛠️ Development

### Testing Framework
- **xUnit** for unit and integration tests
- **FluentAssertions** for readable assertions
- **FsCheck.Xunit** for property-based testing
- **Testcontainers** for integration tests with real databases

### Project Structure
- `src/` - Application code
- `tst/` - Test projects
- `tools/` - SDK and utilities
- `dndtracker/` - Helm charts for Kubernetes deployment

## ⚓ Kubernetes Deployment with Helm

The project includes Helm charts for deploying DNDTracker to Kubernetes clusters.

### Helm Chart Structure

```
dndtracker/
├── Chart.yaml          # Chart metadata and dependencies
├── Chart.lock          # Dependency lock file
├── values.yaml         # Default configuration values
├── charts/             # Dependency charts
│   └── postgresql-12.5.9.tgz
└── templates/          # Kubernetes manifests
    ├── deployment.yaml      # Application deployment
    ├── service.yaml        # Service configuration
    ├── configmap.yaml      # Environment variables
    ├── ingress.yaml        # Ingress configuration
    ├── serviceaccount.yaml # Service account
    ├── hpa.yaml           # Horizontal Pod Autoscaler
    └── tests/
        └── test-connection.yaml
```

### Dependencies

The chart includes PostgreSQL as a dependency:
- **PostgreSQL**: Bitnami PostgreSQL chart (v12.5.x with PostgreSQL 16)
- Automatically deploys a PostgreSQL instance alongside the application

### Key Configuration

#### Application Settings (`values.yaml`)
```yaml
# Application image
image:
  repository: gabrieletoffanin/dndtracker
  tag: "latest"

# Service configuration
service:
  type: NodePort
  port: 8080          # Main API port
  scalarPort: 8081    # Scalar API documentation port

# Resource limits
resources:
  limits:
    cpu: 500m
    memory: 512Mi
  requests:
    cpu: 100m
    memory: 256Mi
```

#### Database Configuration
```yaml
postgresql:
  enabled: true
  image:
    tag: "16.1.0"
  auth:
    postgresPassword: postgres
    database: dndtracker
  persistence:
    size: 1Gi
```

### Deployment Commands

```bash
# Install Helm dependencies
helm dependency update ./dndtracker

# Install the chart
helm install dndtracker ./dndtracker

# Install with custom values
helm install dndtracker ./dndtracker -f custom-values.yaml

# Upgrade existing deployment
helm upgrade dndtracker ./dndtracker

# Uninstall
helm uninstall dndtracker
```

### Environment Configuration

The chart supports environment-specific configurations:

```yaml
# Development environment
environment:
  ASPNETCORE_ENVIRONMENT: Development
  ASPNETCORE_URLS: "http://+:8080"
  ConnectionStrings__DefaultConnection: "Host=dndtracker-postgresql;Port=5432;Database=dndtracker;Username=postgres;Password=postgres"
```

### Ingress Configuration

Enable external access with ingress:

```yaml
ingress:
  enabled: true
  className: "nginx"
  annotations:
    kubernetes.io/ingress.class: nginx
    kubernetes.io/tls-acme: "true"
  hosts:
    - host: dndtracker.yourdomain.com
      paths:
        - path: /
          pathType: ImplementationSpecific
  tls:
    - secretName: dndtracker-tls
      hosts:
        - dndtracker.yourdomain.com
```

### Autoscaling

Enable horizontal pod autoscaling:

```yaml
autoscaling:
  enabled: true
  minReplicas: 1
  maxReplicas: 10
  targetCPUUtilizationPercentage: 80
```

### Service Ports in Kubernetes

When deployed via Helm, services are accessible through:
- **Main API**: `http://dndtracker:8080` (internal) or via NodePort/Ingress
- **API Documentation**: `http://dndtracker:8081/scalar/v1` (internal) or via NodePort/Ingress
- **PostgreSQL**: `dndtracker-postgresql:5432` (internal cluster access)

### Running with Minikube

Minikube provides an easy way to run DNDTracker locally on Kubernetes:

#### Prerequisites
```bash
# Install Minikube (if not already installed)
# On macOS
brew install minikube

# On Ubuntu/Debian
curl -LO https://storage.googleapis.com/minikube/releases/latest/minikube-linux-amd64
sudo install minikube-linux-amd64 /usr/local/bin/minikube

# On Windows
choco install minikube
```

#### Setup and Deployment
```bash
# Start Minikube cluster
minikube start

# Enable required addons
minikube addons enable ingress
minikube addons enable metrics-server

# Build and load the Docker image into Minikube
# Option 1: Build locally and load into Minikube
docker build -t gabrieletoffanin/dndtracker:latest -f src/DNDTracker.Main/Dockerfile .
minikube image load gabrieletoffanin/dndtracker:latest

# Option 2: Use Minikube's Docker daemon
eval $(minikube docker-env)
docker build -t gabrieletoffanin/dndtracker:latest -f src/DNDTracker.Main/Dockerfile .

# Install Helm dependencies
helm dependency update ./dndtracker

# Deploy to Minikube
helm install dndtracker ./dndtracker
```

#### Accessing the Application
```bash
# Get service information
kubectl get services
kubectl get pods

# Access via NodePort (default service type)
minikube service dndtracker --url

# Or use port-forwarding for direct access
kubectl port-forward service/dndtracker 8080:8080
# API will be available at http://localhost:8080
# API Documentation at http://localhost:8080/scalar/v1
```

#### Minikube-Specific Configuration

Create a `minikube-values.yaml` file for local development:

```yaml
# minikube-values.yaml
image:
  repository: gabrieletoffanin/dndtracker
  tag: "latest"
  pullPolicy: Never  # Use local image, don't pull from registry

service:
  type: NodePort  # Use NodePort for easy access in Minikube

resources:
  limits:
    cpu: 200m      # Reduced for local development
    memory: 256Mi
  requests:
    cpu: 50m
    memory: 128Mi

postgresql:
  enabled: true
  persistence:
    size: 500Mi    # Smaller volume for local development
  resources:
    limits:
      cpu: 200m
      memory: 256Mi
    requests:
      cpu: 50m
      memory: 128Mi

ingress:
  enabled: true
  className: "nginx"
  hosts:
    - host: dndtracker.local
      paths:
        - path: /
          pathType: ImplementationSpecific
```

Deploy with Minikube-specific values:
```bash
helm install dndtracker ./dndtracker -f minikube-values.yaml
```

#### Using Ingress with Minikube

```bash
# Get Minikube IP
minikube ip

# Add to /etc/hosts (Linux/macOS) or C:\Windows\System32\drivers\etc\hosts (Windows)
echo "$(minikube ip) dndtracker.local" | sudo tee -a /etc/hosts

# Access via ingress
curl http://dndtracker.local
```

#### Development Workflow

```bash
# Make changes to your code
# Rebuild and reload image
docker build -t gabrieletoffanin/dndtracker:latest -f src/DNDTracker.Main/Dockerfile .
minikube image load gabrieletoffanin/dndtracker:latest

# Restart deployment to use new image
kubectl rollout restart deployment/dndtracker

# Or upgrade with Helm
helm upgrade dndtracker ./dndtracker -f minikube-values.yaml
```

#### Troubleshooting

```bash
# Check pod status
kubectl get pods
kubectl describe pod <pod-name>

# View logs
kubectl logs <pod-name>

# Check services
kubectl get svc
kubectl describe svc dndtracker

# Access Minikube dashboard
minikube dashboard

# Clean up
helm uninstall dndtracker
minikube stop
minikube delete
```

### Monitoring Integration

The Helm chart is designed to work with Kubernetes monitoring stacks:
- Supports Prometheus metrics scraping
- Compatible with Grafana dashboards
- Integrates with Kubernetes logging solutions

## 📝 Additional Notes

- Auto-migration runs on startup with connection retry logic
- OpenTelemetry traces are sent to the collector and forwarded to Elasticsearch
- SSL certificates are auto-generated for the ELK stack
- All services run in a shared Docker network for internal communication
- Helm charts follow Kubernetes best practices for production deployments