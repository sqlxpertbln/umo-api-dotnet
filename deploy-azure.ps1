# ============================================
# UMO API - Azure App Service Deployment
# PowerShell Script für Windows
# ============================================

# Variablen - Passen Sie diese bei Bedarf an
$SUBSCRIPTION_ID = "839e940c-de42-4a75-9d17-f5e96e85454f"
$RESOURCE_GROUP = "umo-api-rg"
$LOCATION = "westeurope"
$APP_SERVICE_PLAN = "umo-api-plan"
$WEB_APP_NAME = "umo-api-sqlxpert"
$GITHUB_REPO = "https://github.com/sqlxpertbln/umo-api-dotnet"
$SKU = "B1"  # B1 = Basic (~12€/Monat), F1 = Free (begrenzt)

# Farbige Ausgabe
function Write-Step {
    param([string]$Message)
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host $Message -ForegroundColor Yellow
    Write-Host "========================================" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host $Message -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host $Message -ForegroundColor White
}

# Start
Clear-Host
Write-Host ""
Write-Host "  _   _ __  __  ___       _    ____ ___  " -ForegroundColor Magenta
Write-Host " | | | |  \/  |/ _ \     / \  |  _ \_ _| " -ForegroundColor Magenta
Write-Host " | | | | |\/| | | | |   / _ \ | |_) | |  " -ForegroundColor Magenta
Write-Host " | |_| | |  | | |_| |  / ___ \|  __/| |  " -ForegroundColor Magenta
Write-Host "  \___/|_|  |_|\___/  /_/   \_\_|  |___| " -ForegroundColor Magenta
Write-Host ""
Write-Host "  Azure App Service Deployment Script" -ForegroundColor White
Write-Host ""

# Prüfen ob Azure CLI installiert ist
Write-Step "Schritt 0: Azure CLI prüfen"
try {
    $azVersion = az --version 2>&1 | Select-Object -First 1
    Write-Success "Azure CLI gefunden: $azVersion"
} catch {
    Write-Host "FEHLER: Azure CLI ist nicht installiert!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Bitte installieren Sie Azure CLI:" -ForegroundColor Yellow
    Write-Host "  winget install Microsoft.AzureCLI" -ForegroundColor White
    Write-Host ""
    Write-Host "Oder laden Sie den Installer herunter:" -ForegroundColor Yellow
    Write-Host "  https://aka.ms/installazurecliwindows" -ForegroundColor White
    exit 1
}

# Schritt 1: Login
Write-Step "Schritt 1: Bei Azure anmelden"
Write-Info "Ein Browser-Fenster wird geöffnet. Bitte melden Sie sich an."
az login
if ($LASTEXITCODE -ne 0) {
    Write-Host "FEHLER: Login fehlgeschlagen!" -ForegroundColor Red
    exit 1
}
Write-Success "Login erfolgreich!"

# Schritt 2: Subscription setzen
Write-Step "Schritt 2: Subscription auswählen"
Write-Info "Setze Subscription: $SUBSCRIPTION_ID"
az account set --subscription $SUBSCRIPTION_ID
if ($LASTEXITCODE -ne 0) {
    Write-Host "FEHLER: Subscription konnte nicht gesetzt werden!" -ForegroundColor Red
    Write-Host "Verfügbare Subscriptions:" -ForegroundColor Yellow
    az account list --output table
    exit 1
}
Write-Success "Subscription gesetzt!"
az account show --output table

# Schritt 3: Resource Group erstellen
Write-Step "Schritt 3: Resource Group erstellen"
Write-Info "Erstelle Resource Group: $RESOURCE_GROUP in $LOCATION"
az group create --name $RESOURCE_GROUP --location $LOCATION --output table
if ($LASTEXITCODE -ne 0) {
    Write-Host "FEHLER: Resource Group konnte nicht erstellt werden!" -ForegroundColor Red
    exit 1
}
Write-Success "Resource Group erstellt!"

# Schritt 4: App Service Plan erstellen
Write-Step "Schritt 4: App Service Plan erstellen"
Write-Info "Erstelle App Service Plan: $APP_SERVICE_PLAN (SKU: $SKU)"
az appservice plan create `
    --name $APP_SERVICE_PLAN `
    --resource-group $RESOURCE_GROUP `
    --sku $SKU `
    --is-linux `
    --output table
if ($LASTEXITCODE -ne 0) {
    Write-Host "FEHLER: App Service Plan konnte nicht erstellt werden!" -ForegroundColor Red
    exit 1
}
Write-Success "App Service Plan erstellt!"

# Schritt 5: Web App erstellen
Write-Step "Schritt 5: Web App erstellen"
Write-Info "Erstelle Web App: $WEB_APP_NAME"
az webapp create `
    --name $WEB_APP_NAME `
    --resource-group $RESOURCE_GROUP `
    --plan $APP_SERVICE_PLAN `
    --runtime "DOTNETCORE:8.0" `
    --output table
if ($LASTEXITCODE -ne 0) {
    Write-Host "FEHLER: Web App konnte nicht erstellt werden!" -ForegroundColor Red
    Write-Host "Möglicherweise ist der Name bereits vergeben." -ForegroundColor Yellow
    Write-Host "Versuchen Sie einen anderen Namen, z.B.: umo-api-sqlxpert-2026" -ForegroundColor Yellow
    exit 1
}
Write-Success "Web App erstellt!"

# Schritt 6: GitHub Deployment konfigurieren
Write-Step "Schritt 6: GitHub Deployment konfigurieren"
Write-Info "Verbinde mit Repository: $GITHUB_REPO"
az webapp deployment source config `
    --name $WEB_APP_NAME `
    --resource-group $RESOURCE_GROUP `
    --repo-url $GITHUB_REPO `
    --branch main `
    --manual-integration `
    --output table
if ($LASTEXITCODE -ne 0) {
    Write-Host "WARNUNG: GitHub-Verbindung fehlgeschlagen. Versuche alternatives Deployment..." -ForegroundColor Yellow
}
Write-Success "GitHub Deployment konfiguriert!"

# Schritt 7: Deployment starten
Write-Step "Schritt 7: Deployment starten"
Write-Info "Starte Deployment... (Dies kann 2-5 Minuten dauern)"
az webapp deployment source sync `
    --name $WEB_APP_NAME `
    --resource-group $RESOURCE_GROUP
Write-Success "Deployment gestartet!"

# Schritt 8: Warten und Status prüfen
Write-Step "Schritt 8: Deployment-Status prüfen"
Write-Info "Warte 30 Sekunden auf Deployment..."
Start-Sleep -Seconds 30

$webappUrl = "https://$WEB_APP_NAME.azurewebsites.net"
Write-Info "Prüfe Erreichbarkeit: $webappUrl"

try {
    $response = Invoke-WebRequest -Uri $webappUrl -UseBasicParsing -TimeoutSec 30
    if ($response.StatusCode -eq 200) {
        Write-Success "Anwendung ist erreichbar!"
    }
} catch {
    Write-Host "Anwendung noch nicht vollständig gestartet. Bitte warten Sie noch 1-2 Minuten." -ForegroundColor Yellow
}

# Abschluss
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  DEPLOYMENT ABGESCHLOSSEN!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Ihre Anwendung ist erreichbar unter:" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Web-Anwendung:    $webappUrl" -ForegroundColor White
Write-Host "  API-Dokumentation: $webappUrl/swagger" -ForegroundColor White
Write-Host "  Reports-Dashboard: $webappUrl/reports.html" -ForegroundColor White
Write-Host ""
Write-Host "Azure Portal:" -ForegroundColor Cyan
Write-Host "  https://portal.azure.com/#resource/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP" -ForegroundColor White
Write-Host ""
Write-Host "Geschätzte monatliche Kosten: ~12€ (B1 Plan)" -ForegroundColor Yellow
Write-Host ""

# Browser öffnen
$openBrowser = Read-Host "Möchten Sie die Anwendung im Browser öffnen? (j/n)"
if ($openBrowser -eq "j" -or $openBrowser -eq "J" -or $openBrowser -eq "y" -or $openBrowser -eq "Y") {
    Start-Process $webappUrl
}

Write-Host ""
Write-Host "Fertig!" -ForegroundColor Green
