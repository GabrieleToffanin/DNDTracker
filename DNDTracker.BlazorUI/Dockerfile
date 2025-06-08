FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DNDTracker.BlazorUI/DNDTracker.BlazorUI.csproj", "DNDTracker.BlazorUI/"]
COPY ["src/DNDTracker.SharedKernel/DNDTracker.SharedKernel.csproj", "src/DNDTracker.SharedKernel/"]
COPY ["src/DNDTracker.Vocabulary/DNDTracker.Vocabulary.csproj", "src/DNDTracker.Vocabulary/"]
COPY ["src/DNDTracker.Domain/DNDTracker.Domain.csproj", "src/DNDTracker.Domain/"]
RUN dotnet restore "./DNDTracker.BlazorUI/DNDTracker.BlazorUI.csproj"
COPY . .
WORKDIR "/src/DNDTracker.BlazorUI"
RUN dotnet build "./DNDTracker.BlazorUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DNDTracker.BlazorUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DNDTracker.BlazorUI.dll"]