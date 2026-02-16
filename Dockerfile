# ---------- Restore Stage (better caching) ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src

# Copy solution + project files first so restore can be cached
COPY ConsoleApp_121_FinalProjectShell.sln ./

COPY ConsoleApp_121_FinalProjectShell/ConsoleApp_121_FinalProjectShell.csproj \
     ConsoleApp_121_FinalProjectShell/

COPY UnitTesting/UnitTesting.csproj \
     UnitTesting/

# Restore using the solution so both app + tests dependencies are restored
RUN dotnet restore ConsoleApp_121_FinalProjectShell.sln


# ---------- Unit Test Stage ----------
FROM restore AS unit-tests
WORKDIR /src

# Copy the rest of the repo
COPY . .

# Run tests (target the test project)
RUN dotnet test UnitTesting/UnitTesting.csproj -c Release --no-restore


# ---------- Publish Stage ----------
FROM restore AS build
WORKDIR /src

# Copy the rest of the repo
COPY . .

# Publish the console app
RUN dotnet publish ConsoleApp_121_FinalProjectShell/ConsoleApp_121_FinalProjectShell.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


# ---------- Runtime Stage ----------
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ConsoleApp_121_FinalProjectShell.dll"]
