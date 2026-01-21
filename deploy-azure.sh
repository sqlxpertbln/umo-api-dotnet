#!/bin/bash
# ============================================
# UMO API - Azure App Service Deployment
# Bash Script für Linux/Mac
# ============================================

# Variablen - Passen Sie diese bei Bedarf an
SUBSCRIPTION_ID="839e940c-de42-4a75-9d17-f5e96e85454f"
RESOURCE_GROUP="umo-api-rg"
LOCATION="westeurope"
APP_SERVICE_PLAN="umo-api-plan"
WEB_APP_NAME="umo-api-sqlxpert"
GITHUB_REPO="https://github.com/sqlxpertbln/umo-api-dotnet"
SKU="B1"  # B1 = Basic (~12€/Monat), F1 = Free (begrenzt)

# Farben
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
NC='\033[0m' # No Color

# Funktionen
print_step() {
    echo ""
    echo -e "${CYAN}========================================${NC}"
    echo -e "${YELLOW}$1${NC}"
    echo -e "${CYAN}========================================${NC}"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_info() {
    echo -e "${NC}$1${NC}"
}

# Start
clear
echo ""
echo -e "${MAGENTA}  _   _ __  __  ___       _    ____ ___  ${NC}"
echo -e "${MAGENTA} | | | |  \\/  |/ _ \\     / \\  |  _ \\_ _| ${NC}"
echo -e "${MAGENTA} | | | | |\\/| | | | |   / _ \\ | |_) | |  ${NC}"
echo -e "${MAGENTA} | |_| | |  | | |_| |  / ___ \\|  __/| |  ${NC}"
echo -e "${MAGENTA}  \\___/|_|  |_|\\___/  /_/   \\_\\_|  |___| ${NC}"
echo ""
echo "  Azure App Service Deployment Script"
echo ""

# Prüfen ob Azure CLI installiert ist
print_step "Schritt 0: Azure CLI prüfen"
if ! command -v az &> /dev/null; then
    print_error "Azure CLI ist nicht installiert!"
    echo ""
    echo "Bitte installieren Sie Azure CLI:"
    echo ""
    echo "  Mac:   brew install azure-cli"
    echo "  Linux: curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash"
    echo ""
    exit 1
fi
AZ_VERSION=$(az --version | head -n 1)
print_success "Azure CLI gefunden: $AZ_VERSION"

# Schritt 1: Login
print_step "Schritt 1: Bei Azure anmelden"
print_info "Ein Browser-Fenster wird geöffnet. Bitte melden Sie sich an."
az login
if [ $? -ne 0 ]; then
    print_error "Login fehlgeschlagen!"
    exit 1
fi
print_success "Login erfolgreich!"

# Schritt 2: Subscription setzen
print_step "Schritt 2: Subscription auswählen"
print_info "Setze Subscription: $SUBSCRIPTION_ID"
az account set --subscription "$SUBSCRIPTION_ID"
if [ $? -ne 0 ]; then
    print_error "Subscription konnte nicht gesetzt werden!"
    echo "Verfügbare Subscriptions:"
    az account list --output table
    exit 1
fi
print_success "Subscription gesetzt!"
az account show --output table

# Schritt 3: Resource Group erstellen
print_step "Schritt 3: Resource Group erstellen"
print_info "Erstelle Resource Group: $RESOURCE_GROUP in $LOCATION"
az group create --name "$RESOURCE_GROUP" --location "$LOCATION" --output table
if [ $? -ne 0 ]; then
    print_error "Resource Group konnte nicht erstellt werden!"
    exit 1
fi
print_success "Resource Group erstellt!"

# Schritt 4: App Service Plan erstellen
print_step "Schritt 4: App Service Plan erstellen"
print_info "Erstelle App Service Plan: $APP_SERVICE_PLAN (SKU: $SKU)"
az appservice plan create \
    --name "$APP_SERVICE_PLAN" \
    --resource-group "$RESOURCE_GROUP" \
    --sku "$SKU" \
    --is-linux \
    --output table
if [ $? -ne 0 ]; then
    print_error "App Service Plan konnte nicht erstellt werden!"
    exit 1
fi
print_success "App Service Plan erstellt!"

# Schritt 5: Web App erstellen
print_step "Schritt 5: Web App erstellen"
print_info "Erstelle Web App: $WEB_APP_NAME"
az webapp create \
    --name "$WEB_APP_NAME" \
    --resource-group "$RESOURCE_GROUP" \
    --plan "$APP_SERVICE_PLAN" \
    --runtime "DOTNETCORE:8.0" \
    --output table
if [ $? -ne 0 ]; then
    print_error "Web App konnte nicht erstellt werden!"
    echo "Möglicherweise ist der Name bereits vergeben."
    echo "Versuchen Sie einen anderen Namen, z.B.: umo-api-sqlxpert-2026"
    exit 1
fi
print_success "Web App erstellt!"

# Schritt 6: GitHub Deployment konfigurieren
print_step "Schritt 6: GitHub Deployment konfigurieren"
print_info "Verbinde mit Repository: $GITHUB_REPO"
az webapp deployment source config \
    --name "$WEB_APP_NAME" \
    --resource-group "$RESOURCE_GROUP" \
    --repo-url "$GITHUB_REPO" \
    --branch main \
    --manual-integration \
    --output table
print_success "GitHub Deployment konfiguriert!"

# Schritt 7: Deployment starten
print_step "Schritt 7: Deployment starten"
print_info "Starte Deployment... (Dies kann 2-5 Minuten dauern)"
az webapp deployment source sync \
    --name "$WEB_APP_NAME" \
    --resource-group "$RESOURCE_GROUP"
print_success "Deployment gestartet!"

# Schritt 8: Warten und Status prüfen
print_step "Schritt 8: Deployment-Status prüfen"
print_info "Warte 30 Sekunden auf Deployment..."
sleep 30

WEBAPP_URL="https://$WEB_APP_NAME.azurewebsites.net"
print_info "Prüfe Erreichbarkeit: $WEBAPP_URL"

HTTP_STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$WEBAPP_URL" --max-time 30)
if [ "$HTTP_STATUS" = "200" ]; then
    print_success "Anwendung ist erreichbar!"
else
    echo -e "${YELLOW}Anwendung noch nicht vollständig gestartet. Bitte warten Sie noch 1-2 Minuten.${NC}"
fi

# Abschluss
echo ""
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}  DEPLOYMENT ABGESCHLOSSEN!${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""
echo -e "${CYAN}Ihre Anwendung ist erreichbar unter:${NC}"
echo ""
echo "  Web-Anwendung:     $WEBAPP_URL"
echo "  API-Dokumentation: $WEBAPP_URL/swagger"
echo "  Reports-Dashboard: $WEBAPP_URL/reports.html"
echo ""
echo -e "${CYAN}Azure Portal:${NC}"
echo "  https://portal.azure.com/#resource/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP"
echo ""
echo -e "${YELLOW}Geschätzte monatliche Kosten: ~12€ (B1 Plan)${NC}"
echo ""

# Browser öffnen (optional)
read -p "Möchten Sie die Anwendung im Browser öffnen? (j/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[JjYy]$ ]]; then
    if command -v xdg-open &> /dev/null; then
        xdg-open "$WEBAPP_URL"
    elif command -v open &> /dev/null; then
        open "$WEBAPP_URL"
    fi
fi

echo ""
echo -e "${GREEN}Fertig!${NC}"
