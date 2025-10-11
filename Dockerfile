# ===========================
# STEP 1: Build the application
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the csproj and restore dependencies separately for layer caching
COPY ["Expense_Tracker.csproj", "./"]

# Ensure NuGet source is set properly (removes duplicate errors)
RUN dotnet nuget remove source nuget.org || true \
    && dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org

# Restore dependencies (disable parallel to avoid network hiccups in Docker)
RUN dotnet restore "./Expense_Tracker.csproj" --disable-parallel

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "./Expense_Tracker.csproj" -c Release -o /app/build

# ===========================
# STEP 2: Publish the app
# ===========================
FROM build AS publish
RUN dotnet publish "./Expense_Tracker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===========================
# STEP 3: Final runtime image
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published output from build stage
COPY --from=publish /app/publish .

# Expose ports (must match docker-compose.yml)
EXPOSE 8080
EXPOSE 8081

# Set environment variables (match docker-compose)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Entry point for the API
ENTRYPOINT ["dotnet", "Expense_Tracker.dll"]
