FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY DNDTracker.Blazor.csproj .
RUN dotnet restore DNDTracker.Blazor.csproj
COPY . .
RUN dotnet publish DNDTracker.Blazor.csproj -c Release -o /app/publish --no-restore

FROM nginx:1.29-alpine AS final
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
EXPOSE 8080
