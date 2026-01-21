# UMO API - Vollständige Deployment-Anleitung

Diese Anleitung beschreibt mehrere Wege, die UMO API Anwendung permanent online zu bringen.

---

## Inhaltsverzeichnis

1. [Option A: Azure App Service (über Visual Studio)](#option-a-azure-app-service-über-visual-studio)
2. [Option B: Azure App Service (über Azure CLI)](#option-b-azure-app-service-über-azure-cli)
3. [Option C: Railway.app (Empfohlen - Einfachste Option)](#option-c-railwayapp-empfohlen)
4. [Option D: Docker auf eigenem Server](#option-d-docker-auf-eigenem-server)
5. [Option E: Render.com](#option-e-rendercom)

---

## Option A: Azure App Service (über Visual Studio)

### Voraussetzungen
- Visual Studio 2022 mit "ASP.NET und Webentwicklung" Workload
- Azure-Konto mit aktiver Subscription

### Schritt-für-Schritt

1. **Projekt in Visual Studio öffnen**
   - Öffnen Sie `UMOApi.csproj` in Visual Studio

2. **Publish-Profil erstellen**
   - Rechtsklick auf das Projekt → "Publish..."
   - Wählen Sie "Azure" → "Next"
   - Wählen Sie "Azure App Service (Linux)" → "Next"

3. **App Service konfigurieren**
   - Klicken Sie auf "+" um einen neuen App Service zu erstellen
   - Name: `umo-api-sqlxpert` (oder ähnlich)
   - Subscription: Wählen Sie Ihre Subscription
   - Resource Group: Erstellen Sie eine neue oder wählen Sie eine bestehende
   - Hosting Plan: Wählen Sie "Free" für Tests oder "Basic B1" für Produktion
   - Region: "West Europe" oder "Germany West Central"

4. **Veröffentlichen**
   - Klicken Sie auf "Create"
   - Nach der Erstellung klicken Sie auf "Publish"
   - Visual Studio deployed die Anwendung automatisch

5. **URL abrufen**
   - Nach dem Deployment finden Sie die URL im Output-Fenster
   - Format: `https://umo-api-sqlxpert.azurewebsites.net`

---

## Option B: Azure App Service (über Azure CLI)

Falls das Azure Portal Probleme macht, können Sie die Azure CLI verwenden.

### 1. Azure CLI installieren

**Windows (PowerShell als Admin):**
```powershell
winget install Microsoft.AzureCLI
```

**Mac:**
```bash
brew install azure-cli
```

**Linux:**
```bash
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
```

### 2. Bei Azure anmelden

```bash
az login
```

### 3. Subscription auswählen

```bash
# Alle Subscriptions anzeigen
az account list --output table

# Subscription setzen
az account set --subscription "Visual Studio Enterprise – MPN - Arthur"
```

### 4. Resource Group erstellen

```bash
az group create --name umo-api-rg --location westeurope
```

### 5. App Service Plan erstellen (Free Tier)

```bash
az appservice plan create \
  --name umo-api-plan \
  --resource-group umo-api-rg \
  --sku F1 \
  --is-linux
```

### 6. Web App erstellen

```bash
az webapp create \
  --name umo-api-sqlxpert \
  --resource-group umo-api-rg \
  --plan umo-api-plan \
  --runtime "DOTNET|8.0"
```

### 7. GitHub Deployment konfigurieren

```bash
az webapp deployment source config \
  --name umo-api-sqlxpert \
  --resource-group umo-api-rg \
  --repo-url https://github.com/sqlxpertbln/umo-api-dotnet \
  --branch main \
  --manual-integration
```

### 8. Deployment starten

```bash
az webapp deployment source sync \
  --name umo-api-sqlxpert \
  --resource-group umo-api-rg
```

**Ihre App ist jetzt erreichbar unter:**
`https://umo-api-sqlxpert.azurewebsites.net`

---

## Option C: Railway.app (Empfohlen)

Railway.app ist eine moderne Alternative zu Heroku - einfach, schnell und mit kostenlosem Einstieg.

### Vorteile
- Einfachste Einrichtung (< 5 Minuten)
- $5 kostenloses Guthaben pro Monat
- Automatisches Deployment bei Git Push
- Keine Kreditkarte für den Start nötig

### Schritt-für-Schritt

1. **Account erstellen**
   - Gehen Sie zu [railway.app](https://railway.app)
   - Klicken Sie auf "Login" → "Login with GitHub"

2. **Neues Projekt erstellen**
   - Klicken Sie auf "New Project"
   - Wählen Sie "Deploy from GitHub repo"
   - Wählen Sie `umo-api-dotnet`

3. **Umgebungsvariablen setzen**
   - Klicken Sie auf das Service
   - Gehen Sie zu "Variables"
   - Fügen Sie hinzu:
     ```
     ASPNETCORE_ENVIRONMENT=Production
     PORT=8080
     ```

4. **Domain generieren**
   - Gehen Sie zu "Settings" → "Networking"
   - Klicken Sie auf "Generate Domain"
   - Sie erhalten eine URL wie: `umo-api-production.up.railway.app`

5. **Fertig!**
   - Railway deployed automatisch bei jedem Git Push
   - Logs sind im Dashboard einsehbar

---

## Option D: Docker auf eigenem Server

Wenn Sie einen eigenen Server (VPS, Raspberry Pi, etc.) haben.

### Voraussetzungen
- Server mit Docker installiert
- SSH-Zugang zum Server

### 1. Repository klonen

```bash
git clone https://github.com/sqlxpertbln/umo-api-dotnet.git
cd umo-api-dotnet
```

### 2. Docker Image bauen und starten

```bash
docker-compose up -d --build
```

### 3. Mit Nginx Reverse Proxy (für HTTPS)

Erstellen Sie `/etc/nginx/sites-available/umo-api`:

```nginx
server {
    listen 80;
    server_name api.ihre-domain.de;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### 4. SSL mit Let's Encrypt

```bash
sudo apt install certbot python3-certbot-nginx
sudo certbot --nginx -d api.ihre-domain.de
```

---

## Option E: Render.com

### Schritt-für-Schritt

1. **Account erstellen**
   - Gehen Sie zu [render.com](https://render.com)
   - Registrieren Sie sich mit GitHub

2. **Neuen Web Service erstellen**
   - Klicken Sie auf "New +" → "Web Service"
   - Verbinden Sie Ihr GitHub-Konto
   - Wählen Sie `umo-api-dotnet`

3. **Konfiguration**
   - Name: `umo-api`
   - Region: `Frankfurt (EU Central)`
   - Branch: `main`
   - Runtime: `Docker`
   - Instance Type: `Free`

4. **Umgebungsvariablen**
   - Fügen Sie hinzu:
     ```
     ASPNETCORE_ENVIRONMENT=Production
     ```

5. **Erstellen**
   - Klicken Sie auf "Create Web Service"
   - Warten Sie ca. 5 Minuten für den Build

**Ihre App ist erreichbar unter:**
`https://umo-api-xxxx.onrender.com`

---

## Vergleich der Optionen

| Option | Kosten | Aufwand | Cold Start | Empfehlung |
|--------|--------|---------|------------|------------|
| Azure (VS) | ~$13/Monat (B1) | Mittel | Nein | Professionell |
| Azure (CLI) | ~$13/Monat (B1) | Mittel | Nein | Professionell |
| Railway | $5 Guthaben/Monat | Gering | Nein | ⭐ Beste Balance |
| Docker/VPS | ~$5/Monat | Hoch | Nein | Volle Kontrolle |
| Render | Kostenlos | Gering | Ja (~30s) | Zum Testen |

---

## Troubleshooting

### Azure Portal zeigt keine Regionen
- Versuchen Sie die Azure CLI (Option B)
- Oder wechseln Sie die Subscription
- Prüfen Sie Ihre Kontingente unter "Subscriptions" → "Usage + quotas"

### Deployment schlägt fehl
- Prüfen Sie die Logs im jeweiligen Dashboard
- Stellen Sie sicher, dass `ASPNETCORE_ENVIRONMENT=Production` gesetzt ist
- Bei Docker: Prüfen Sie mit `docker logs <container-id>`

### Datenbank wird nicht erstellt
- Die SQLite-Datenbank wird beim ersten Start automatisch erstellt
- Stellen Sie sicher, dass der Ordner beschreibbar ist
- Bei persistenten Daten: Mounten Sie ein Volume für `/app/UMOApi.db`

---

## Support

Bei Fragen zum Projekt:
- GitHub Issues: https://github.com/sqlxpertbln/umo-api-dotnet/issues
- Repository: https://github.com/sqlxpertbln/umo-api-dotnet
