# ---------- Build Stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project file first (better caching)
COPY ConsoleApp_121_FinalProjectShell.sln .
COPY ConsoleApp_121_FinalProjectShell/ConsoleApp_121_FinalProjectShell.csproj ConsoleApp_121_FinalProjectShell/

# Restore dependencies
RUN dotnet restore ConsoleApp_121_FinalProjectShell/ConsoleApp_121_FinalProjectShell.csproj

# Copy the rest of the source
COPY . .

# Publish the console app
RUN dotnet publish ConsoleApp_121_FinalProjectShell/ConsoleApp_121_FinalProjectShell.csproj \
    -c Release \
    -o /app/publish

# ---------- Runtime Stage ----------
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ConsoleApp_121_FinalProjectShell.dll"]
