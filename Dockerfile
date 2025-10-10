# ===========================
# STEP 1: Build the application
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything first (this helps dotnet restore work properly)
COPY ["Expense_Tracker.csproj", "./"]

# Add NuGet source explicitly (in case it's missing)
RUN dotnet nuget remove source nuget.org || true
RUN dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org

# Copy custom NuGet.config if present (optional)
# COPY NuGet.config ./

# Restore dependencies (disable parallel to reduce transient network errors)
RUN dotnet restore "./Expense_Tracker.csproj" --disable-parallel --ignore-failed-sources

# Copy the rest of the project and build
COPY . .
RUN dotnet build "./Expense_Tracker.csproj" -c Release -o /app/build

# ===========================
# STEP 2: Publish
# ===========================
FROM build AS publish
RUN dotnet publish "./Expense_Tracker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===========================
# STEP 3: Final runtime image
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose the default port
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Start the app
ENTRYPOINT ["dotnet", "Expense_Tracker.dll"]
