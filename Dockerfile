# Multi-stage Dockerfile for Expense_Tracker (.NET 8) with diagnostics and proxy/CA support
# Usage examples:
#  docker build --build-arg HTTP_PROXY=http://proxy:3128 --build-arg HTTPS_PROXY=http://proxy:3128 --build-arg COMPANY_CA=company.crt -t expense-tracker:dev .
#  docker build --network=host -t expense-tracker:dev .

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Build-time args for proxy and optional corporate CA
ARG HTTP_PROXY
ARG HTTPS_PROXY
ARG NO_PROXY
ARG COMPANY_CA

# Export proxy args for apt/dotnet during build
ENV HTTP_PROXY=${HTTP_PROXY}
ENV HTTPS_PROXY=${HTTPS_PROXY}
ENV NO_PROXY=${NO_PROXY}

# Install CA certs and curl for diagnostics
RUN apt-get update && \
    apt-get install -y --no-install-recommends ca-certificates curl && \
    rm -rf /var/lib/apt/lists/*

# If a company CA file was supplied at build time, copy and install it
# Pass --build-arg COMPANY_CA=company.crt and include company.crt next to Dockerfile
COPY ${COMPANY_CA:-} /usr/local/share/ca-certificates/company.crt
RUN if [ -f /usr/local/share/ca-certificates/company.crt ]; then update-ca-certificates; fi

# Copy csproj and NuGet.config first (cache layer)
COPY ["Expense_Tracker.csproj", "./"]

# NOTE: Docker COPY does not accept shell redirections or "|| true".
# Copy NuGet.config normally; ensure NuGet.config exists in the build context.
# If you don't have a NuGet.config, either create an empty one or remove this line.
COPY ["NuGet.config", "./"]

# Show configured NuGet sources and diagnostics
RUN dotnet nuget list source --verbosity normal || true

# Diagnostic: try fetching NuGet service index (non-fatal)
RUN curl -fsS https://api.nuget.org/v3/index.json -o /tmp/nuget_index.json || { echo "curl to api.nuget.org failed"; true; }

# Restore with detailed verbosity to surface TLS/DNS/proxy errors
RUN dotnet restore "./Expense_Tracker.csproj" --disable-parallel --no-cache --verbosity detailed

# Copy remaining source
COPY . .

# Build and publish
RUN dotnet build "./Expense_Tracker.csproj" -c Release -o /app/build
RUN dotnet publish "./Expense_Tracker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    ASPNETCORE_URLS=http://+:80

COPY --from=build /app/publish .

# Create non-root user
RUN addgroup --system app && adduser --system --ingroup app app && chown -R app:app /app
USER app

EXPOSE 80
ENTRYPOINT ["dotnet", "Expense_Tracker.dll"]
    