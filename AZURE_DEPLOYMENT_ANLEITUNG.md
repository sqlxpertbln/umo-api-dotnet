# UMO API - Azure App Service Deployment Anleitung

Diese Anleitung führt Sie Schritt für Schritt durch das Deployment der UMO API auf Azure App Service.

---

## Voraussetzungen

### 1. Azure CLI installieren

**Windows (PowerShell als Administrator):**
```powershell
# Option A: Mit winget (empfohlen)
winget install Microsoft.AzureCLI

# Option B: MSI-Installer herunterladen
# https://aka.ms/installazurecliwindows
```

**Mac:**
```bash
brew install azure-cli
```

**Linux (Ubuntu/Debian):**
```bash
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
```

### 2. Installation überprüfen

Öffnen Sie ein neues Terminal/PowerShell und führen Sie aus:
```bash
az --version
```

Sie sollten eine Ausgabe wie `azure-cli 2.x.x` sehen.

---

## Deployment durchführen

### Schritt 1: Bei Azure anmelden

```bash
az login
```

**Was passiert:** Ein Browser-Fenster öffnet sich. Melden Sie sich mit Ihrem Microsoft-Konto an (dc@sqlxpert.de).

### Schritt 2: Subscription auswählen

Zeigen Sie alle verfügbaren Subscriptions an:
```bash
az account list --output table
```

Setzen Sie die gewünschte Subscription:
```bash
az account set --subscription "839e940c-de42-4a75-9d17-f5e96e85454f"
```

**Hinweis:** Die ID `839e940c-de42-4a75-9d17-f5e96e85454f` entspricht "Visual Studio Enterprise-Abonnement – MPN - Daniel".

Überprüfen Sie die aktive Subscription:
```bash
az account show --output table
```

### Schritt 3: Resource Group erstellen

Eine Resource Group ist ein Container für alle Azure-Ressourcen:
```bash
az group create --name umo-api-rg --location westeurope
```

**Erwartete Ausgabe:**
```json
{
  "id": "/subscriptions/.../resourceGroups/umo-api-rg",
  "location": "westeurope",
  "name": "umo-api-rg",
  "properties": {
    "provisioningState": "Succeeded"
  }
}
```

### Schritt 4: App Service Plan erstellen

Der App Service Plan definiert die Rechenressourcen:
```bash
az appservice plan create \
  --name umo-api-plan \
  --resource-group umo-api-rg \
  --sku B1 \
  --is-linux
```

**SKU-Optionen:**
| SKU | Preis | Beschreibung |
|-----|-------|--------------|
| F1 | Kostenlos | Sehr begrenzt, für Tests |
| B1 | ~€12/Monat | Basic, für Entwicklung |
| S1 | ~€65/Monat | Standard, für Produktion |
| P1V2 | ~€130/Monat | Premium, hohe Last |

**Hinweis:** F1 (Free) unterstützt kein "Always On" und hat Cold Starts.

### Schritt 5: Web App erstellen

```bash
az webapp create \
  --name umo-api-sqlxpert \
  --resource-group umo-api-rg \
  --plan umo-api-plan \
  --runtime "DOTNETCORE:8.0"
```

**Wichtig:** Der Name `umo-api-sqlxpert` muss global eindeutig sein. Falls er bereits vergeben ist, wählen Sie einen anderen Namen (z.B. `umo-api-sqlxpert-2026`).

**Erwartete Ausgabe enthält:**
```json
{
  "defaultHostName": "umo-api-sqlxpert.azurewebsites.net",
  "state": "Running"
}
```

### Schritt 6: GitHub-Deployment konfigurieren

Verbinden Sie die Web App mit Ihrem GitHub-Repository:
```bash
az webapp deployment source config \
  --name umo-api-sqlxpert \
  --resource-group umo-api-rg \
  --repo-url https://github.com/sqlxpertbln/umo-api-dotnet \
  --branch main \
  --manual-integration
```

### Schritt 7: Deployment starten

Lösen Sie das Deployment aus:
```bash
az webapp deployment source sync \
  --name umo-api-sqlxpert \
  --resource-group umo-api-rg
```

**Hinweis:** Das Deployment dauert ca. 2-5 Minuten.

### Schritt 8: Deployment-Status prüfen

```bash
az webapp log deployment show \
  --name umo-api-sqlxpert \
  --resource-group umo-api-rg
```

### Schritt 9: Anwendung testen

Öffnen Sie im Browser:
```
https://umo-api-sqlxpert.azurewebsites.net
```

Oder testen Sie mit curl:
```bash
curl https://umo-api-sqlxpert.azurewebsites.net/clients
```

---

## Komplettes Deployment-Skript (Copy & Paste)

Speichern Sie dieses Skript als `deploy-azure.sh` (Linux/Mac) oder `deploy-azure.ps1` (Windows):

### Linux/Mac (Bash):
```bash
#!/bin/bash

# Variablen
SUBSCRIPTION_ID="839e940c-de42-4a75-9d17-f5e96e85454f"
RESOURCE_GROUP="umo-api-rg"
LOCATION="westeurope"
APP_SERVICE_PLAN="umo-api-plan"
WEB_APP_NAME="umo-api-sqlxpert"
GITHUB_REPO="https://github.com/sqlxpertbln/umo-api-dotnet"

echo "=== UMO API Azure Deployment ==="
echo ""

# 1. Login
echo "Schritt 1: Azure Login..."
az login

# 2. Subscription setzen
echo "Schritt 2: Subscription setzen..."
az account set --subscription "$SUBSCRIPTION_ID"
az account show --output table

# 3. Resource Group erstellen
echo "Schritt 3: Resource Group erstellen..."
az group create --name "$RESOURCE_GROUP" --location "$LOCATION"

# 4. App Service Plan erstellen
echo "Schritt 4: App Service Plan erstellen..."
az appservice plan create \
  --name "$APP_SERVICE_PLAN" \
  --resource-group "$RESOURCE_GROUP" \
  --sku B1 \
  --is-linux

# 5. Web App erstellen
echo "Schritt 5: Web App erstellen..."
az webapp create \
  --name "$WEB_APP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --plan "$APP_SERVICE_PLAN" \
  --runtime "DOTNETCORE:8.0"

# 6. GitHub Deployment konfigurieren
echo "Schritt 6: GitHub Deployment konfigurieren..."
az webapp deployment source config \
  --name "$WEB_APP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --repo-url "$GITHUB_REPO" \
  --branch main \
  --manual-integration

# 7. Deployment starten
echo "Schritt 7: Deployment starten..."
az webapp deployment source sync \
  --name "$WEB_APP_NAME" \
  --resource-group "$RESOURCE_GROUP"

echo ""
echo "=== Deployment abgeschlossen! ==="
echo "Ihre Anwendung ist erreichbar unter:"
echo "https://${WEB_APP_NAME}.azurewebsites.net"
echo ""
echo "API-Dokumentation (Swagger):"
echo "https://${WEB_APP_NAME}.azurewebsites.net/swagger"
```

### Windows (PowerShell):
```powershell
# Variablen
$SUBSCRIPTION_ID = "839e940c-de42-4a75-9d17-f5e96e85454f"
$RESOURCE_GROUP = "umo-api-rg"
$LOCATION = "westeurope"
$APP_SERVICE_PLAN = "umo-api-plan"
$WEB_APP_NAME = "umo-api-sqlxpert"
$GITHUB_REPO = "https://github.com/sqlxpertbln/umo-api-dotnet"

Write-Host "=== UMO API Azure Deployment ===" -ForegroundColor Green
Write-Host ""

# 1. Login
Write-Host "Schritt 1: Azure Login..." -ForegroundColor Yellow
az login

# 2. Subscription setzen
Write-Host "Schritt 2: Subscription setzen..." -ForegroundColor Yellow
az account set --subscription $SUBSCRIPTION_ID
az account show --output table

# 3. Resource Group erstellen
Write-Host "Schritt 3: Resource Group erstellen..." -ForegroundColor Yellow
az group create --name $RESOURCE_GROUP --location $LOCATION

# 4. App Service Plan erstellen
Write-Host "Schritt 4: App Service Plan erstellen..." -ForegroundColor Yellow
az appservice plan create `
  --name $APP_SERVICE_PLAN `
  --resource-group $RESOURCE_GROUP `
  --sku B1 `
  --is-linux

# 5. Web App erstellen
Write-Host "Schritt 5: Web App erstellen..." -ForegroundColor Yellow
az webapp create `
  --name $WEB_APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --plan $APP_SERVICE_PLAN `
  --runtime "DOTNETCORE:8.0"

# 6. GitHub Deployment konfigurieren
Write-Host "Schritt 6: GitHub Deployment konfigurieren..." -ForegroundColor Yellow
az webapp deployment source config `
  --name $WEB_APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --repo-url $GITHUB_REPO `
  --branch main `
  --manual-integration

# 7. Deployment starten
Write-Host "Schritt 7: Deployment starten..." -ForegroundColor Yellow
az webapp deployment source sync `
  --name $WEB_APP_NAME `
  --resource-group $RESOURCE_GROUP

Write-Host ""
Write-Host "=== Deployment abgeschlossen! ===" -ForegroundColor Green
Write-Host "Ihre Anwendung ist erreichbar unter:" -ForegroundColor Cyan
Write-Host "https://$WEB_APP_NAME.azurewebsites.net"
Write-Host ""
Write-Host "API-Dokumentation (Swagger):" -ForegroundColor Cyan
Write-Host "https://$WEB_APP_NAME.azurewebsites.net/swagger"
```

---

## Fehlerbehebung

### Fehler: "The subscription is not registered to use namespace 'Microsoft.Web'"

Lösung:
```bash
az provider register --namespace Microsoft.Web
az provider show --namespace Microsoft.Web --query "registrationState"
# Warten bis "Registered" angezeigt wird (ca. 1-2 Minuten)
```

### Fehler: "The webapp name is already taken"

Lösung: Wählen Sie einen anderen Namen für `WEB_APP_NAME`, z.B.:
```bash
WEB_APP_NAME="umo-api-sqlxpert-2026"
```

### Fehler: "Quota exceeded"

Lösung: Prüfen Sie Ihre Kontingente im Azure Portal unter:
`Subscriptions → Ihre Subscription → Usage + quotas`

### Logs anzeigen

Live-Logs der Anwendung:
```bash
az webapp log tail --name umo-api-sqlxpert --resource-group umo-api-rg
```

### Anwendung neu starten

```bash
az webapp restart --name umo-api-sqlxpert --resource-group umo-api-rg
```

---

## Ressourcen löschen (bei Bedarf)

Um alle erstellten Ressourcen zu löschen und Kosten zu vermeiden:
```bash
az group delete --name umo-api-rg --yes --no-wait
```

**Achtung:** Dies löscht die Resource Group und ALLE darin enthaltenen Ressourcen unwiderruflich!

---

## Zusammenfassung

Nach erfolgreichem Deployment haben Sie:

| Ressource | Name | URL |
|-----------|------|-----|
| Resource Group | umo-api-rg | - |
| App Service Plan | umo-api-plan | - |
| Web App | umo-api-sqlxpert | https://umo-api-sqlxpert.azurewebsites.net |
| Swagger | - | https://umo-api-sqlxpert.azurewebsites.net/swagger |
| Reports | - | https://umo-api-sqlxpert.azurewebsites.net/reports.html |

Die Anwendung wird automatisch bei jedem Push auf den `main`-Branch des GitHub-Repositories aktualisiert.
