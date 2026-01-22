# CI/CD Pipeline Setup für UMO API

Diese Anleitung beschreibt, wie Sie die GitHub Actions CI/CD Pipeline für die UMO API einrichten.

## Übersicht

Die Pipeline automatisiert:
- **Build**: Kompiliert die .NET 8 Anwendung
- **Test**: Führt Unit Tests aus
- **Deploy**: Deployed auf die entsprechende Azure-Umgebung basierend auf dem Branch

| Branch | Umgebung | URL |
|--------|----------|-----|
| `dev` | Development | https://umo-api-dev.azurewebsites.net |
| `test` | Test | https://umo-api-test.azurewebsites.net |
| `main` | Production | https://umo-api-sqlxpert-bpdeh4g3ewaybafn.germanywestcentral-01.azurewebsites.net |

## Schritt 1: Workflow-Datei hinzufügen

1. Öffnen Sie https://github.com/sqlxpertbln/umo-api-dotnet
2. Klicken Sie auf **Add file** → **Create new file**
3. Geben Sie als Pfad ein: `.github/workflows/ci-cd.yml`
4. Kopieren Sie den folgenden Inhalt:

```yaml
# UMO API CI/CD Pipeline
# Automatisches Build, Test und Deployment auf Azure Web Apps

name: CI/CD Pipeline

on:
  push:
    branches:
      - main
      - test
      - dev
  pull_request:
    branches:
      - main
      - test

env:
  DOTNET_VERSION: '8.0.x'
  AZURE_WEBAPP_NAME_DEV: 'umo-api-dev'
  AZURE_WEBAPP_NAME_TEST: 'umo-api-test'
  AZURE_WEBAPP_NAME_PROD: 'umo-api-sqlxpert'

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    
    steps:
    - name: Checkout code
      uses: actions/checkout@v4
      
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
        
    - name: Restore dependencies
      run: dotnet restore UMOApi.csproj
      
    - name: Build
      run: dotnet build UMOApi.csproj --configuration Release --no-restore
      
    - name: Run Unit Tests
      run: dotnet test Tests/UMOApi.Tests.csproj --configuration Release --verbosity normal --filter "FullyQualifiedName!~Integration"
      continue-on-error: false
      
    - name: Publish
      run: dotnet publish UMOApi.csproj --configuration Release --output ./publish
      
    - name: Upload artifact
      uses: actions/upload-artifact@v4
      with:
        name: webapp
        path: ./publish

  deploy-dev:
    needs: build-and-test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/dev'
    environment:
      name: Development
      url: https://umo-api-dev.azurewebsites.net
    
    steps:
    - name: Download artifact
      uses: actions/download-artifact@v4
      with:
        name: webapp
        path: ./publish
        
    - name: Deploy to Azure Web App (DEV)
      uses: azure/webapps-deploy@v3
      with:
        app-name: ${{ env.AZURE_WEBAPP_NAME_DEV }}
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE_DEV }}
        package: ./publish

  deploy-test:
    needs: build-and-test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/test'
    environment:
      name: Test
      url: https://umo-api-test.azurewebsites.net
    
    steps:
    - name: Download artifact
      uses: actions/download-artifact@v4
      with:
        name: webapp
        path: ./publish
        
    - name: Deploy to Azure Web App (TEST)
      uses: azure/webapps-deploy@v3
      with:
        app-name: ${{ env.AZURE_WEBAPP_NAME_TEST }}
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE_TEST }}
        package: ./publish

  deploy-prod:
    needs: build-and-test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    environment:
      name: Production
      url: https://umo-api-sqlxpert-bpdeh4g3ewaybafn.germanywestcentral-01.azurewebsites.net
    
    steps:
    - name: Download artifact
      uses: actions/download-artifact@v4
      with:
        name: webapp
        path: ./publish
        
    - name: Deploy to Azure Web App (PROD)
      uses: azure/webapps-deploy@v3
      with:
        app-name: ${{ env.AZURE_WEBAPP_NAME_PROD }}
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE_PROD }}
        package: ./publish
```

5. Klicken Sie auf **Commit new file**

## Schritt 2: GitHub Secrets hinzufügen

1. Öffnen Sie https://github.com/sqlxpertbln/umo-api-dotnet/settings/secrets/actions
2. Klicken Sie auf **New repository secret**
3. Fügen Sie die folgenden Secrets hinzu:

### Secret 1: AZURE_WEBAPP_PUBLISH_PROFILE_DEV
- Name: `AZURE_WEBAPP_PUBLISH_PROFILE_DEV`
- Value: (Inhalt der Datei `publish-profile-dev.xml`)

### Secret 2: AZURE_WEBAPP_PUBLISH_PROFILE_TEST
- Name: `AZURE_WEBAPP_PUBLISH_PROFILE_TEST`
- Value: (Inhalt der Datei `publish-profile-test.xml`)

### Secret 3: AZURE_WEBAPP_PUBLISH_PROFILE_PROD
- Name: `AZURE_WEBAPP_PUBLISH_PROFILE_PROD`
- Value: (Inhalt der Datei `publish-profile-prod.xml`)

## Schritt 3: Publish Profiles aus Azure herunterladen

Falls Sie die Publish Profiles nicht haben:

1. Öffnen Sie das Azure Portal: https://portal.azure.com
2. Navigieren Sie zu **App Services**
3. Wählen Sie die jeweilige App (umo-api-dev, umo-api-test, umo-api-sqlxpert)
4. Klicken Sie oben auf **Get publish profile**
5. Öffnen Sie die heruntergeladene Datei und kopieren Sie den gesamten Inhalt

## Workflow testen

Nach der Einrichtung:

1. **DEV-Deployment testen:**
   ```bash
   git checkout dev
   git push origin dev
   ```

2. **TEST-Deployment testen:**
   ```bash
   git checkout test
   git merge dev
   git push origin test
   ```

3. **PROD-Deployment testen:**
   ```bash
   git checkout main
   git merge test
   git push origin main
   ```

## Branch-Strategie

```
dev (Entwicklung)
 ↓ merge
test (QA/Testing)
 ↓ merge
main (Produktion)
```

- Entwickler arbeiten auf Feature-Branches und mergen nach `dev`
- Nach erfolgreichen Tests wird `dev` nach `test` gemerged
- Nach Abnahme wird `test` nach `main` gemerged

## Troubleshooting

### Pipeline schlägt fehl
- Prüfen Sie die GitHub Actions Logs unter **Actions** Tab
- Stellen Sie sicher, dass alle Secrets korrekt gesetzt sind

### Deployment funktioniert nicht
- Prüfen Sie, ob die Publish Profiles aktuell sind
- Laden Sie neue Publish Profiles aus dem Azure Portal herunter
