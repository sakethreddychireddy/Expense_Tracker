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
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/publish .

# Expose HTTP port
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "Expense_Tracker.dll"]
