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

# Ensure network DNS works (helps in some Docker setups)
RUN echo "nameserver 8.8.8.8" > /etc/resolv.conf

# Ensure SSL certificates are up to date
RUN apt-get update || true && apt-get install -y --no-install-recommends ca-certificates && update-ca-certificates

# Copy project file and restore dependencies
COPY ["Expense_Tracker.csproj", "."]
RUN dotnet restore "./Expense_Tracker.csproj" --disable-parallel --ignore-failed-sources

# Copy the rest of the source code and build
COPY . .
RUN dotnet build "./Expense_Tracker.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish for production
RUN dotnet publish "./Expense_Tracker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ==============================
#  Final runtime image
# ==============================
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Expense_Tracker.dll"]
