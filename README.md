# UMO API - .NET Core Web API mit Reports & Dashboards

Eine vollständige ASP.NET Core 8.0 Web API mit integrierter SQLite-Datenbank, webbasiertem Client und umfassenden Business-Dashboards.

## Features

- **REST API** für Klienten, Geräte, Provider und Systemdaten
- **SQLite-Datenbank** (integriert, kein separater SQL-Server erforderlich)
- **Web-Client** mit Bootstrap 5 und Chart.js
- **6 Business-Dashboards** für verschiedene Geschäftsbereiche
- **Swagger/OpenAPI** Dokumentation

## Dashboards

| Dashboard | Beschreibung |
|-----------|--------------|
| Marketing | Klientenakquise, Regionen, Altersverteilung |
| Vertrieb | Geräteauslastung, Provider-Performance |
| Geschäftsbereich | Status, Kapazitäten, Workload |
| Finanzbuchhaltung | Umsätze, Kosten, Profitabilität |
| Controlling | KPIs, Benchmarks, Jahresvergleich |
| Rechnungswesen | Abrechnungen, Zahlungsstatus, Aging |

## Schnellstart

### Lokal ausführen

```bash
dotnet restore
dotnet run --urls="http://localhost:5000"
```

### Mit Docker

```bash
docker-compose up -d
```

## Deployment auf Render.com (Kostenlos)

1. Forken Sie dieses Repository
2. Gehen Sie zu [render.com](https://render.com) und erstellen Sie einen Account
3. Klicken Sie auf "New" → "Web Service"
4. Verbinden Sie Ihr GitHub-Repository
5. Konfiguration:
   - **Environment**: Docker
   - **Region**: Frankfurt (EU)
   - **Instance Type**: Free
6. Klicken Sie auf "Create Web Service"

Die Anwendung wird automatisch gebaut und deployed.

## API-Endpunkte

### Stammdaten
- `GET /clients` - Klientenliste (paginiert)
- `GET /clientdetails/{id}` - Klientendetails
- `GET /devices` - Geräteliste
- `GET /directProvider` - Pflegedienste
- `GET /professionalProvider` - Ärzte

### Reports
- `GET /reports/overview` - Übersicht aller Reports
- `GET /reports/marketing` - Marketing Dashboard
- `GET /reports/sales` - Vertrieb Dashboard
- `GET /reports/operations` - Geschäftsbereich Dashboard
- `GET /reports/finance` - Finanzbuchhaltung Dashboard
- `GET /reports/controlling` - Controlling Dashboard
- `GET /reports/billing` - Rechnungswesen Dashboard

## Technologie-Stack

- ASP.NET Core 8.0
- Entity Framework Core 8.0
- SQLite
- Bootstrap 5
- Chart.js

## Lizenz

MIT License

# CI/CD Pipeline aktiv - Thu Jan 22 11:08:03 EST 2026
