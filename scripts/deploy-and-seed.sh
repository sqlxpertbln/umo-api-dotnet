#!/bin/bash
# UMO API Deployment und Seed Script
# Baut die Anwendung, deployed auf Azure und lädt Seed-Daten

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
API_BASE_URL="${API_BASE_URL:-https://umo-api-sqlxpert-bpdeh4g3ewaybafn.germanywestcentral-01.azurewebsites.net}"

echo "============================================"
echo "UMO API Deployment und Seed Script"
echo "============================================"
echo "Projekt: $PROJECT_DIR"
echo "API URL: $API_BASE_URL"
echo "Startzeit: $(date)"
echo ""

# 1. Build
echo "=== Phase 1: Build ==="
cd "$PROJECT_DIR"
rm -rf publish
~/.dotnet/dotnet publish -c Release -o publish
echo "Build erfolgreich!"
echo ""

# 2. ZIP erstellen
echo "=== Phase 2: ZIP erstellen ==="
cd publish
rm -f ../deploy.zip
zip -r ../deploy.zip .
echo "ZIP erstellt: deploy.zip"
echo ""

# 3. Azure Deployment
echo "=== Phase 3: Azure Deployment ==="
cd "$PROJECT_DIR"
az webapp deploy \
    --resource-group umo-api-rg \
    --name umo-api-sqlxpert \
    --src-path deploy.zip \
    --type zip
echo "Deployment erfolgreich!"
echo ""

# 4. Warten auf App-Start
echo "=== Phase 4: Warten auf App-Start ==="
echo "Warte 30 Sekunden auf App-Initialisierung..."
sleep 30

# Health-Check
MAX_RETRIES=10
RETRY_COUNT=0
while [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
    HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" "$API_BASE_URL/clients" || echo "000")
    if [ "$HTTP_CODE" = "200" ]; then
        echo "App ist bereit (HTTP $HTTP_CODE)"
        break
    fi
    RETRY_COUNT=$((RETRY_COUNT + 1))
    echo "Warte auf App... (Versuch $RETRY_COUNT/$MAX_RETRIES, HTTP $HTTP_CODE)"
    sleep 10
done

if [ $RETRY_COUNT -eq $MAX_RETRIES ]; then
    echo "WARNUNG: App antwortet nicht wie erwartet, fahre trotzdem fort..."
fi
echo ""

# 5. Seed-Daten laden
echo "=== Phase 5: Seed-Daten laden ==="
export API_BASE_URL
"$SCRIPT_DIR/seed-data.sh"
echo ""

# 6. Git Commit (optional)
echo "=== Phase 6: Git Commit ==="
cd "$PROJECT_DIR"
if [ -n "$(git status --porcelain)" ]; then
    git add -A
    git commit -m "Deployment $(date '+%Y-%m-%d %H:%M:%S')"
    git push origin main
    echo "Änderungen auf GitHub gepusht"
else
    echo "Keine Änderungen zu committen"
fi
echo ""

echo "============================================"
echo "Deployment abgeschlossen!"
echo "API URL: $API_BASE_URL"
echo "Endzeit: $(date)"
echo "============================================"
