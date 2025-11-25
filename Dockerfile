FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN apt-get update && apt-get install -y curl
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src
COPY . .
RUN dotnet restore "Quizitor.sln"

FROM restore AS build
RUN dotnet build "Quizitor.sln" -c Release --no-restore

FROM build AS tested
RUN dotnet test  -c Release --no-build

FROM tested AS publish
RUN dotnet publish "src/Quizitor.Migrator/Quizitor.Migrator.csproj" -c Release -o /app/migrator /p:UseAppHost=false --no-build
RUN dotnet publish "src/Quizitor.Api/Quizitor.Api.csproj" -c Release -o /app/api /p:UseAppHost=false --no-build
RUN dotnet publish "src/Quizitor.Bots/Quizitor.Bots.csproj" -c Release -o /app/bots /p:UseAppHost=false --no-build
RUN dotnet publish "src/Quizitor.Sender/Quizitor.Sender.csproj" -c Release -o /app/sender /p:UseAppHost=false --no-build
RUN dotnet publish "src/Quizitor.Events/Quizitor.Events.csproj" -c Release -o /app/events /p:UseAppHost=false --no-build

### migrator
FROM base AS migrator
WORKDIR /app
COPY --from=publish /app/migrator .
ENTRYPOINT ["dotnet", "Quizitor.Migrator.dll"]

### api
FROM base AS api
WORKDIR /app
COPY --from=publish /app/api .
ENTRYPOINT ["dotnet", "Quizitor.Api.dll"]

### bots
FROM base AS bots
WORKDIR /app
COPY --from=publish /app/bots .
ENTRYPOINT ["dotnet", "Quizitor.Bots.dll"]

### sender
FROM base AS sender
WORKDIR /app
COPY --from=publish /app/sender .
ENTRYPOINT ["dotnet", "Quizitor.Sender.dll"]

### events
FROM base AS events
WORKDIR /app
COPY --from=publish /app/events .
ENTRYPOINT ["dotnet", "Quizitor.Events.dll"]
