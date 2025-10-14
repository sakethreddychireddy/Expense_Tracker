# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy and restore dependencies
COPY *.sln .
COPY Expense_Tracker/*.csproj ./Expense_Tracker/
RUN dotnet restore

# Copy all files and build
COPY . .
WORKDIR /app/Expense_Tracker
RUN dotnet publish -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Expense_Tracker.dll"]
