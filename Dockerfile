# ===========================
# STEP 1: Build the application
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# --- DNS Fix (only for build stage) ---
RUN echo "nameserver 8.8.8.8" > /etc/resolv.conf && \
    echo "nameserver 8.8.4.4" >> /etc/resolv.conf

# Copy the project file and restore dependencies
COPY ["Expense_Tracker.csproj", "./"]

# Copy NuGet configuration (ensures stable restore)
COPY NuGet.config ./ 

# Ensure NuGet source is configured
RUN dotnet nuget remove source nuget.org || true \
    && dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org

# Restore dependencies (disable parallel to avoid network hiccups)
RUN dotnet restore "./Expense_Tracker.csproj" --disable-parallel

# Copy the rest of the source code
COPY . .

# Build the project
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

# Copy published output from previous step
COPY --from=publish /app/publish .

# Expose ports (must match docker-compose.yml)
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Start the app
ENTRYPOINT ["dotnet", "Expense_Tracker.dll"]
