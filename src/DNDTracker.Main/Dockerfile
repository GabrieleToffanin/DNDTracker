FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Adjust paths based on your build context
COPY ["DNDTracker.Main/DNDTracker.Main.csproj", "DNDTracker.Main/"]
RUN dotnet restore "DNDTracker.Main/DNDTracker.Main.csproj"
COPY . .
WORKDIR "/src/DNDTracker.Main"
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