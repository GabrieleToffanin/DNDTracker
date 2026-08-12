FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
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
COPY ["src/DNDTracker.Inbound.AmqpAdapter/DNDTracker.Inbound.AmqpAdapter.csproj", "src/DNDTracker.Inbound.AmqpAdapter/"]
COPY ["src/DNDTracker.Outbounx.PostgresDb/DNDTracker.Outbounx.PostgresDb.csproj", "src/DNDTracker.Outbounx.PostgresDb/"]
COPY ["src/DNDTracker.Outbound.RabbitMq/DNDTracker.Outbound.RabbitMq.csproj", "src/DNDTracker.Outbound.RabbitMq/"]
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

# Install PostgreSQL client, curl and other dependencies for Datadog tracer
RUN apt-get update && apt-get install -y postgresql-client curl netcat-openbsd && rm -rf /var/lib/apt/lists/*

# Please select the corresponding download for your Linux containers https://github.com/DataDog/dd-trace-dotnet/releases/latest
# Download and install the Tracer
RUN mkdir -p /opt/datadog \
    && mkdir -p /var/log/datadog \
    && TRACER_VERSION=$(curl -s https://api.github.com/repos/DataDog/dd-trace-dotnet/releases/latest | grep tag_name | cut -d '"' -f 4 | cut -c2-) \
    && ARCH=$(dpkg --print-architecture) \
    && curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_${ARCH}.deb \
    && dpkg -i ./datadog-dotnet-apm_${TRACER_VERSION}_${ARCH}.deb \
    && rm ./datadog-dotnet-apm_${TRACER_VERSION}_${ARCH}.deb

# Enable the tracer
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
ENV DD_DOTNET_TRACER_HOME=/opt/datadog

# Create entrypoint script
RUN echo '#!/bin/bash' > /app/entrypoint.sh && \
    echo 'set -e' >> /app/entrypoint.sh && \
    echo 'POSTGRES_HOST="${POSTGRES_HOST:-postgres}"' >> /app/entrypoint.sh && \
    echo 'POSTGRES_PORT="${POSTGRES_PORT:-5432}"' >> /app/entrypoint.sh && \
    echo 'RABBITMQ_HOST="${RABBITMQ_HOST:-rabbitmq}"' >> /app/entrypoint.sh && \
    echo 'RABBITMQ_PORT="${RABBITMQ_PORT:-5672}"' >> /app/entrypoint.sh && \
    echo 'echo "Waiting for PostgreSQL at ${POSTGRES_HOST}:${POSTGRES_PORT}..."' >> /app/entrypoint.sh && \
    echo 'until pg_isready -h "${POSTGRES_HOST}" -p "${POSTGRES_PORT}" > /dev/null 2>&1; do sleep 2; done' >> /app/entrypoint.sh && \
    echo 'echo "PostgreSQL is ready."' >> /app/entrypoint.sh && \
    echo 'echo "Waiting for RabbitMQ at ${RABBITMQ_HOST}:${RABBITMQ_PORT}..."' >> /app/entrypoint.sh && \
    echo 'until nc -z "${RABBITMQ_HOST}" "${RABBITMQ_PORT}" > /dev/null 2>&1; do sleep 2; done' >> /app/entrypoint.sh && \
    echo 'echo "RabbitMQ is ready."' >> /app/entrypoint.sh && \
    echo 'exec dotnet DNDTracker.Main.dll' >> /app/entrypoint.sh && \
    chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]