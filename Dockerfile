# ---- Build Stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY *.sln ./
COPY **/*.csproj ./
RUN for file in $(find . -maxdepth 2 -name '*.csproj'); do dotnet restore "$file"; done

# Copy everything and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ---- Runtime Stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .
EXPOSE 80

ENTRYPOINT ["dotnet", "Expense_Tracker.dll"]
