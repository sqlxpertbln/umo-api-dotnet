# UMO API Modul-Dokumentation

**Version**: 1.0
**Datum**: 2026-01-23

## 1. Einleitung

Dieses Dokument beschreibt die Architektur, den Code und die Transformations-Metadaten des UMO API-Moduls. Die Dokumentation folgt den Code-Guidelines der App-Fabrik Meta-Plattform und dem 3-Stufen-Transformationskonzept.

## 2. Architektur-Überblick

Das UMO API-Modul ist als .NET 8 Web API implementiert und folgt den Prinzipien der Clean Architecture. Es ist in die folgenden Schichten unterteilt:

- **Domain Layer**: Enthält die Kern-Entitäten und die Geschäftslogik.
- **Application Layer**: Enthält die Anwendungslogik, Services und DTOs.
- **Infrastructure Layer**: Enthält die Implementierung der Datenzugriffslogik (Entity Framework Core) und externe Service-Integrationen.
- **Presentation Layer**: Enthält die API-Controller, die die Funktionalität über eine RESTful-Schnittstelle bereitstellen.

## 3. Transformations-Metadaten

Der gesamte Code ist mit Transformations-Kommentaren versehen, die die folgenden Aspekte abdecken:

- **Stage 1 (Template/Mockup)**: Markierung der UI-Komponenten und Mockup-Elemente.
- **Stage 2 (Core Application)**: Clean Architecture Layer-Zuordnung, Entity-Mapping, API-Dokumentation.
- **Stage 3 (Deployment)**: Aspire-Orchestrierung, Container-Konfiguration, CI/CD-Hinweise.

Diese Kommentare ermöglichen die automatisierte Transformation des Moduls in verschiedene Zielarchitekturen und die Integration in Automatisierungsframeworks wie n8n und Microsoft Foundry.

## 4. Modul-Struktur

Das Projekt ist wie folgt strukturiert:

- `/Controllers`: Enthält die API-Controller.
- `/Data`: Enthält den `UMOApiDbContext`.
- `/Models`: Enthält die Domain-Entitäten.
- `/Services`: Enthält die Anwendungs-Services.
- `Program.cs`: Konfiguriert die Anwendung und die Dependency Injection.

## 5. Code-Dokumentation

Der gesamte Code ist mit XML-Kommentaren und den oben beschriebenen Transformations-Kommentaren versehen. Diese Dokumentation wird automatisch von den CI/CD-Pipelines extrahiert und in die zentrale Dokumentations-Plattform der App-Fabrik integriert.
