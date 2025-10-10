# ==============================
#  Base image for runtime
# ==============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ==============================
#  Build and publish stage
# ==============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Fix SSL issues and ensure NuGet works properly
RUN apt-get update && apt-get install -y --no-install-recommends ca-certificates && update-ca-certificates && \
    dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org

# Copy project file and restore dependencies
COPY ["Expense_Tracker.csproj", "."]
RUN dotnet restore "./Expense_Tracker.csproj" --disable-parallel --ignore-failed-sources

# Copy the rest of the project files and build
COPY . .
RUN dotnet build "./Expense_Tracker.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish for production (no app host for portability)
RUN dotnet publish "./Expense_Tracker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ==============================
#  Final runtime image
# ==============================
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Expense_Tracker.dll"]
