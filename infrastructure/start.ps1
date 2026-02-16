#!/usr/bin/env pwsh

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  DÉMARRAGE AUTOMATIQUE DE L'INFRASTRUCTURE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Étape 1 : Démarrer Docker Compose
Write-Host "[1/4] Démarrage des conteneurs Docker..." -ForegroundColor Green
docker-compose up -d

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERREUR : Impossible de démarrer Docker Compose" -ForegroundColor Red
    exit 1
}

# Étape 2 : Attendre que les bases de données soient prêtes
Write-Host "[2/4] Attente que les services soient prêts (40 secondes)..." -ForegroundColor Yellow
Start-Sleep -Seconds 40

# Étape 3 : Appliquer les migrations
Write-Host "[3/4] Application des migrations de base de données..." -ForegroundColor Green

Write-Host "  - DiscussionService..." -ForegroundColor Cyan
Set-Location -Path "../DiscussionService"
dotnet ef database update
if ($LASTEXITCODE -ne 0) {
    Write-Host "    AVERTISSEMENT : Migration DiscussionService échouée" -ForegroundColor Yellow
}
Set-Location -Path "../infrastructure"

Write-Host "  - EnvironmentService..." -ForegroundColor Cyan
Set-Location -Path "../EnvironmentService/EnvironmentsService.API"
dotnet ef database update
if ($LASTEXITCODE -ne 0) {
    Write-Host "    AVERTISSEMENT : Migration EnvironmentService échouée" -ForegroundColor Yellow
}
Set-Location -Path "../../infrastructure"

Write-Host "  - UserService..." -ForegroundColor Cyan
Set-Location -Path "../UsersService/backend"
dotnet ef database update
if ($LASTEXITCODE -ne 0) {
    Write-Host "    AVERTISSEMENT : Migration UserService échouée" -ForegroundColor Yellow
}
Set-Location -Path "../../infrastructure"

# Étape 4 : Terminé
Write-Host ""
Write-Host "[4/4] Infrastructure prête !" -ForegroundColor Green
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  SERVICES DISPONIBLES" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Kafka:                 localhost:29092" -ForegroundColor White
Write-Host "Kafka UI:              localhost:8090" -ForegroundColor White
Write-Host "DiscussionService DB:  localhost:1434" -ForegroundColor White
Write-Host "UserService DB:        localhost:1433" -ForegroundColor White
Write-Host "EnvironmentService DB: localhost:1436" -ForegroundColor White
Write-Host ""
Write-Host "Mot de passe SQL Server: YourStrong@Password123" -ForegroundColor Yellow
Write-Host ""
Write-Host "Pour démarrer les services backend:" -ForegroundColor Green
Write-Host "  Terminal 1: cd ../DiscussionService && dotnet run" -ForegroundColor White
Write-Host "  Terminal 2: cd ../EnvironmentService/EnvironmentsService.API && dotnet run" -ForegroundColor White
Write-Host "  Terminal 3: cd ../UsersService/backend && dotnet run" -ForegroundColor White
Write-Host ""
Write-Host "Pour démarrer le frontend:" -ForegroundColor Green
Write-Host "  cd ../discussionfront/Project-Mangement-App && ng serve" -ForegroundColor White
Write-Host ""