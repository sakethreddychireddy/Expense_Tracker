# Multi-stage Dockerfile for Expense_Tracker (.NET 8)
# ===========================
# STEP 1: Build the application
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Install CA certificates for HTTPS
RUN apt-get update && \
    apt-get install -y --no-install-recommends ca-certificates && \
    rm -rf /var/lib/apt/lists/*

# Copy project file and NuGet.config first (for caching)
COPY ["Expense_Tracker.csproj", "./"]
COPY ["NuGet.config", "./"]

# Verify NuGet source
RUN dotnet nuget list source

# Restore dependencies
RUN dotnet restore "./Expense_Tracker.csproj" --disable-parallel --no-cache

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "./Expense_Tracker.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "./Expense_Tracker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===========================
# STEP 2: Runtime image
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Recommended runtime environment settings
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    ASPNETCORE_URLS=http://+:80

# Copy published app
COPY --from=build /app/publish .

# Create non-root user and set ownership (optional but recommended)
RUN addgroup --system app && adduser --system --ingroup app app \
    && chown -R app:app /app
USER app

EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "Expense_Tracker.dll"]
