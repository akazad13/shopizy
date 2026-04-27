# syntax=docker/dockerfile:1.7
# Multi-stage build for Shopizy.Api (.NET 10).
# Build context is expected to be the repository root: `docker build -f src/Shopizy.Api/Dockerfile .`

ARG DOTNET_SDK_VERSION=10.0
ARG DOTNET_RUNTIME_VERSION=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_SDK_VERSION} AS build
ARG TARGETARCH
WORKDIR /src

# Copy package management + project files first for restore-layer caching.
COPY Directory.Build.props Directory.Packages.props Shopizy.sln ./
COPY src/Shopizy.Api/Shopizy.Api.csproj src/Shopizy.Api/
COPY src/Shopizy.Application/Shopizy.Application.csproj src/Shopizy.Application/
COPY src/Shopizy.Contracts/Shopizy.Contracts.csproj src/Shopizy.Contracts/
COPY src/Shopizy.Domain/Shopizy.Domain.csproj src/Shopizy.Domain/
COPY src/Shopizy.Infrastructure/Shopizy.Infrastructure.csproj src/Shopizy.Infrastructure/
COPY src/Shopizy.SharedKernel/Shopizy.SharedKernel.csproj src/Shopizy.SharedKernel/

RUN dotnet restore src/Shopizy.Api/Shopizy.Api.csproj -a $TARGETARCH

# Now copy the rest and publish.
COPY . .
RUN dotnet publish src/Shopizy.Api/Shopizy.Api.csproj \
    -c Release \
    -a $TARGETARCH \
    --no-restore \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_RUNTIME_VERSION} AS runtime
WORKDIR /app

# Run as a non-root user. The aspnet image creates `app` (uid 1654) by convention.
USER $APP_UID

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_NOLOGO=true

EXPOSE 8080

COPY --from=build /app/publish .

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
  CMD wget -qO- http://127.0.0.1:8080/healthz || exit 1

ENTRYPOINT ["dotnet", "Shopizy.Api.dll"]
