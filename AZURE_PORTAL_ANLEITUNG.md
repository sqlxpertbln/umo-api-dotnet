# UMO API - Azure Portal Deployment Anleitung

Diese Anleitung zeigt, wie Sie die UMO API **ohne Kommandozeile** direkt über das Azure Portal deployen können.

---

## Übersicht

| Schritt | Aktion | Dauer |
|---------|--------|-------|
| 1 | Azure Portal öffnen & anmelden | 1 Min |
| 2 | Resource Group erstellen | 2 Min |
| 3 | App Service erstellen | 3 Min |
| 4 | GitHub-Deployment konfigurieren | 3 Min |
| 5 | Anwendung testen | 1 Min |

**Gesamtdauer:** ca. 10-15 Minuten

---

## Schritt 1: Azure Portal öffnen

1. Öffnen Sie **https://portal.azure.com**
2. Melden Sie sich mit Ihrem Microsoft-Konto an (dc@sqlxpert.de)
3. Sie sehen das Azure Dashboard

---

## Schritt 2: Resource Group erstellen

Eine Resource Group ist ein Container für alle zusammengehörigen Azure-Ressourcen.

### 2.1 Resource Group anlegen

1. Klicken Sie auf **"Resource groups"** im linken Menü
   - Oder suchen Sie nach "Resource groups" in der Suchleiste oben

2. Klicken Sie auf **"+ Create"** (oben links)

3. Füllen Sie das Formular aus:

   | Feld | Wert |
   |------|------|
   | **Subscription** | Visual Studio Enterprise-Abonnement – MPN - Daniel |
   | **Resource group** | `umo-api-rg` |
   | **Region** | `(Europe) West Europe` |

4. Klicken Sie auf **"Review + create"**

5. Klicken Sie auf **"Create"**

✅ Die Resource Group wird in wenigen Sekunden erstellt.

---

## Schritt 3: App Service (Web App) erstellen

### 3.1 Web App anlegen

1. Klicken Sie auf **"+ Create a resource"** (oben links im Portal)

2. Suchen Sie nach **"Web App"** und wählen Sie es aus

3. Klicken Sie auf **"Create"**

### 3.2 Basics-Tab ausfüllen

| Feld | Wert | Erklärung |
|------|------|-----------|
| **Subscription** | Visual Studio Enterprise-Abonnement – MPN - Daniel | Ihre Subscription |
| **Resource Group** | `umo-api-rg` | Die eben erstellte Gruppe |
| **Name** | `umo-api-sqlxpert` | Muss global eindeutig sein! |
| **Publish** | `Code` | Wir deployen Quellcode |
| **Runtime stack** | `.NET 8 (LTS)` | Unsere Laufzeitumgebung |
| **Operating System** | `Linux` | Günstiger als Windows |
| **Region** | `West Europe` | Nächste Region zu Deutschland |

### 3.3 Pricing Plan konfigurieren

Unter **"Pricing plans"**:

1. Klicken Sie auf **"Create new"** bei "Linux Plan"

2. Geben Sie einen Namen ein: `umo-api-plan`

3. Klicken Sie auf **"Change size"** unter "Sku and size"

4. Wählen Sie einen Plan:

   | Plan | Preis | Empfehlung |
   |------|-------|------------|
   | **F1 (Free)** | Kostenlos | Nur zum Testen, sehr begrenzt |
   | **B1 (Basic)** | ~12€/Monat | ⭐ Empfohlen für Entwicklung |
   | **S1 (Standard)** | ~65€/Monat | Für Produktion |

5. Klicken Sie auf **"Select"**

### 3.4 Weitere Tabs (optional)

- **Database**: Überspringen (wir nutzen SQLite)
- **Deployment**: Hier konfigurieren wir später GitHub
- **Networking**: Standard belassen
- **Monitoring**: Application Insights kann aktiviert werden (optional)
- **Tags**: Optional für Organisation

### 3.5 Erstellen

1. Klicken Sie auf **"Review + create"**

2. Überprüfen Sie die Zusammenfassung

3. Klicken Sie auf **"Create"**

⏳ Warten Sie ca. 1-2 Minuten bis die Bereitstellung abgeschlossen ist.

4. Klicken Sie auf **"Go to resource"**

---

## Schritt 4: GitHub-Deployment konfigurieren

### 4.1 Deployment Center öffnen

1. In Ihrer Web App, klicken Sie im linken Menü auf **"Deployment Center"**
   (unter "Deployment")

### 4.2 GitHub verbinden

1. Unter **"Source"** wählen Sie **"GitHub"**

2. Klicken Sie auf **"Authorize"** um Azure mit GitHub zu verbinden
   - Ein Popup öffnet sich
   - Melden Sie sich bei GitHub an (falls nötig)
   - Autorisieren Sie Azure App Service

3. Nach der Autorisierung füllen Sie aus:

   | Feld | Wert |
   |------|------|
   | **Organization** | `sqlxpertbln` |
   | **Repository** | `umo-api-dotnet` |
   | **Branch** | `main` |

### 4.3 Build-Einstellungen

1. Unter **"Build"**:
   - **Runtime stack**: `.NET`
   - **Version**: `8`

2. Klicken Sie auf **"Save"** (oben)

### 4.4 Deployment starten

Das Deployment startet automatisch nach dem Speichern!

1. Klicken Sie auf **"Logs"** Tab im Deployment Center

2. Sie sehen den Build-Fortschritt:
   - "Building..."
   - "Deploying..."
   - "Success" ✅

⏳ Das erste Deployment dauert ca. 3-5 Minuten.

---

## Schritt 5: Anwendung testen

### 5.1 URL finden

1. Gehen Sie zurück zur **"Overview"** Ihrer Web App

2. Finden Sie die **"Default domain"** (z.B. `umo-api-sqlxpert.azurewebsites.net`)

3. Klicken Sie auf die URL oder kopieren Sie sie

### 5.2 Anwendung öffnen

Öffnen Sie im Browser:

| Seite | URL |
|-------|-----|
| **Hauptseite** | https://umo-api-sqlxpert.azurewebsites.net |
| **API-Dokumentation** | https://umo-api-sqlxpert.azurewebsites.net/swagger |
| **Reports-Dashboard** | https://umo-api-sqlxpert.azurewebsites.net/reports.html |

---

## Fertig! 🎉

Ihre UMO API ist jetzt permanent online und erreichbar.

### Automatische Updates

Jedes Mal, wenn Sie Änderungen ins GitHub-Repository pushen, wird die Anwendung automatisch neu deployed.

---

## Zusätzliche Einstellungen (Optional)

### Custom Domain hinzufügen

1. Gehen Sie zu **"Custom domains"** im linken Menü
2. Klicken Sie auf **"+ Add custom domain"**
3. Folgen Sie den Anweisungen für DNS-Konfiguration

### SSL-Zertifikat

1. Gehen Sie zu **"TLS/SSL settings"**
2. Azure stellt automatisch ein kostenloses Zertifikat für `*.azurewebsites.net` bereit
3. Für Custom Domains: Klicken Sie auf **"+ Add TLS/SSL binding"**

### Skalierung

1. Gehen Sie zu **"Scale up (App Service plan)"** für mehr Ressourcen
2. Gehen Sie zu **"Scale out"** für mehrere Instanzen

### Logs anzeigen

1. Gehen Sie zu **"Log stream"** im linken Menü
2. Sie sehen Live-Logs der Anwendung

---

## Fehlerbehebung

### Problem: "Region nicht verfügbar"

**Lösung:** 
- Wählen Sie eine andere Region (z.B. "North Europe" oder "Germany West Central")
- Oder wechseln Sie die Subscription

### Problem: "Name bereits vergeben"

**Lösung:**
- Wählen Sie einen anderen Namen (z.B. `umo-api-sqlxpert-2026`)

### Problem: "Deployment fehlgeschlagen"

**Lösung:**
1. Gehen Sie zu "Deployment Center" → "Logs"
2. Klicken Sie auf das fehlgeschlagene Deployment
3. Lesen Sie die Fehlermeldung
4. Häufige Ursachen:
   - Falscher Runtime Stack
   - Repository nicht erreichbar
   - Build-Fehler im Code

### Problem: "502 Bad Gateway" nach Deployment

**Lösung:**
1. Warten Sie 2-3 Minuten (Anwendung startet noch)
2. Gehen Sie zu "Diagnose and solve problems"
3. Prüfen Sie die Logs unter "Log stream"

---

## Kosten im Überblick

| Ressource | Plan | Monatliche Kosten |
|-----------|------|-------------------|
| App Service Plan | F1 (Free) | 0€ |
| App Service Plan | B1 (Basic) | ~12€ |
| App Service Plan | S1 (Standard) | ~65€ |
| Bandbreite | Erste 5GB/Monat | 0€ |
| Bandbreite | Danach | ~0.08€/GB |

**Empfehlung:** Starten Sie mit B1 (~12€/Monat) für eine zuverlässige Entwicklungsumgebung.

---

## Ressourcen löschen

Um Kosten zu vermeiden, wenn Sie die Anwendung nicht mehr benötigen:

1. Gehen Sie zu **"Resource groups"**
2. Wählen Sie `umo-api-rg`
3. Klicken Sie auf **"Delete resource group"**
4. Geben Sie den Namen zur Bestätigung ein
5. Klicken Sie auf **"Delete"**

⚠️ **Achtung:** Dies löscht alle Ressourcen in der Gruppe unwiderruflich!
