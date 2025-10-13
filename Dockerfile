# ===========================
# STEP 1: Build the application
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# --- DNS FIX ---
# Fix name resolution issues that can block NuGet restore inside Docker
RUN echo "nameserver 8.8.8.8" > /etc/resolv.conf && \
    echo "nameserver 1.1.1.1" >> /etc/resolv.conf

# Copy project files and NuGet config first (for better layer caching)
COPY ["Expense_Tracker.csproj", "./"]
COPY NuGet.config ./

# Ensure NuGet source is configured
RUN dotnet nuget remove source nuget.org || true \
    && dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org

# Restore dependencies
RUN dotnet restore "./Expense_Tracker.csproj" --disable-parallel

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "./Expense_Tracker.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "./Expense_Tracker.csproj" -c Release -o /app/publish /p:UseAppHost=false


# ===========================
# STEP 2: Create runtime image
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# --- DNS FIX (again for runtime) ---
#RUN echo "nameserver 8.8.8.8" > /etc/resolv.conf && \
 #   echo "nameserver 1.1.1.1" >> /etc/resolv.conf

# Copy published files from build stage
COPY --from=build /app/publish .

# Expose the default HTTP port
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "Expense_Tracker.dll"]
