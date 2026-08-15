# syntax=docker/dockerfile:1

# ---- build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY src/PlatformService/PlatformService.csproj PlatformService/
RUN dotnet restore PlatformService/PlatformService.csproj

COPY src/PlatformService/ PlatformService/
WORKDIR /src/PlatformService
RUN dotnet publish -c Release -o /app/publish --no-restore

# ---- runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:8080 \
    SERVICE_NAME=platform-service \
    DOTNET_EnableDiagnostics=0

COPY --from=build /app/publish .

EXPOSE 8080
# .NET 8+ base images ship a built-in non-root "app" user (APP_UID); no manual useradd needed.
USER $APP_UID

ENTRYPOINT ["dotnet", "PlatformService.dll"]
