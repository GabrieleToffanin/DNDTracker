FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Adjust paths based on your build context
COPY ["src/DNDTracker.Main/DNDTracker.Main.csproj", "src/DNDTracker.Main/"]
COPY ["src/DNDTracker.Application/DNDTracker.Application.csproj", "src/DNDTracker.Application/"]
COPY ["src/DNDTracker.Application.Queries/DNDTracker.Application.Queries.csproj", "src/DNDTracker.Application.Queries/"]
COPY ["src/DNDTracker.Domain/DNDTracker.Domain.csproj", "src/DNDTracker.Domain/"]
COPY ["src/DNDTracker.SharedKernel/DNDTracker.SharedKernel.csproj", "src/DNDTracker.SharedKernel/"]
COPY ["src/DNDTracker.Vocabulary/DNDTracker.Vocabulary.csproj", "src/DNDTracker.Vocabulary/"]
COPY ["src/DNDTracker.Inbound.RestAdapter/DNDTracker.Inbound.RestAdapter.csproj", "src/DNDTracker.Inbound.RestAdapter/"]
COPY ["src/DNDTracker.Outbounx.PostgresDb/DNDTracker.Outbounx.PostgresDb.csproj", "src/DNDTracker.Outbounx.PostgresDb/"]
COPY ["src/DNDTracker.Outbound.InMemoryAdapter/DNDTracker.Outbound.InMemoryAdapter.csproj", "src/DNDTracker.Outbound.InMemoryAdapter/"]
COPY ["src/DNDTracker.DataAccessObject.Mapping/DNDTracker.DataAccessObject.Mapping.csproj", "src/DNDTracker.DataAccessObject.Mapping/"]
RUN dotnet restore "src/DNDTracker.Main/DNDTracker.Main.csproj"
COPY . .
WORKDIR "/src/src/DNDTracker.Main"
RUN dotnet build "DNDTracker.Main.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DNDTracker.Main.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install PostgreSQL client for health checks
RUN apt-get update && apt-get install -y postgresql-client && rm -rf /var/lib/apt/lists/*

# Create entrypoint script
RUN echo '#!/bin/bash' > /app/entrypoint.sh && \
    echo 'echo "Waiting for PostgreSQL..."' >> /app/entrypoint.sh && \
    echo 'dotnet DNDTracker.Main.dll' >> /app/entrypoint.sh && \
    chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]