# =========================
# STAGE 1: Build the app
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy all source files and build the project
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# =========================
# STAGE 2: Run the app
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the build output from previous stage
COPY --from=build /app/publish .

# Set environment
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Expense_Tracker.dll"]
