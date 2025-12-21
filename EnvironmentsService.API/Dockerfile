# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier les fichiers .csproj et restaurer les dépendances
COPY ["EnvironmentsService.API/EnvironmentsService.API.csproj", "EnvironmentsService.API/"]
COPY ["EnvironmentsService.Application/EnvironmentsService.Application.csproj", "EnvironmentsService.Application/"]
COPY ["EnvironmentsService.Domain/EnvironmentsService.Domain.csproj", "EnvironmentsService.Domain/"]
COPY ["EnvironmentsService.Infrastructure/EnvironmentsService.Infrastructure.csproj", "EnvironmentsService.Infrastructure/"]

RUN dotnet restore "EnvironmentsService.API/EnvironmentsService.API.csproj"

# Copier tout le code source
COPY . .

# Build du projet
WORKDIR "/src/EnvironmentsService.API"
RUN dotnet build "EnvironmentsService.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "EnvironmentsService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EnvironmentsService.API.dll"]