# Service Hub - Notruf-Geräte API Dokumentation

Diese Dokumentation beschreibt, wie neue Notruf-Gerätetypen (z.B. GPS-Tracker, Apple Watch, Hausnotruf-Geräte) über die `/servicehub/devices` API hinzugefügt und verwaltet werden können.

## Inhaltsverzeichnis

1. [Übersicht](#übersicht)
2. [Unterstützte Gerätetypen](#unterstützte-gerätetypen)
3. [API-Endpunkte](#api-endpunkte)
4. [Beispiele](#beispiele)
5. [Webhook-Integration](#webhook-integration)
6. [Fehlerbehandlung](#fehlerbehandlung)

---

## Übersicht

Das Service Hub Modul unterstützt verschiedene Notruf-Gerätetypen, die über die REST API verwaltet werden können. Jedes Gerät wird einem Klienten zugeordnet und kann Alarme auslösen.

### Basis-URL

```
https://umo-api-sqlxpert-bpdeh4g3ewaybafn.germanywestcentral-01.azurewebsites.net/servicehub
```

### Authentifizierung

Alle API-Aufrufe erfordern eine gültige Authentifizierung (Bearer Token oder API-Key).

---

## Unterstützte Gerätetypen

| Gerätetyp | Beschreibung | Hersteller-Beispiele |
|-----------|--------------|----------------------|
| `AppleWatch` | Apple Watch mit Sturzerkennung und SOS | Apple |
| `Hausnotruf` | Klassisches Hausnotruf-Gerät | Tunstall, Bosch, Philips |
| `GPSTracker` | GPS-Tracker mit Notruffunktion | PAJ, Tracki, Invoxia |
| `Smartphone` | Smartphone mit Notruf-App | Android, iOS |
| `MedicalAlert` | Medizinischer Alarmgeber | Medical Guardian, Life Alert |
| `FallDetector` | Spezieller Sturzsensor | Philips Lifeline, Bay Alarm |
| `PanicButton` | Panik-Knopf / Notruftaste | Verschiedene |
| `WearableDevice` | Sonstige Wearables | Fitbit, Garmin, Samsung |

---

## API-Endpunkte

### 1. Alle Geräte abrufen

```http
GET /servicehub/devices
```

**Response:**
```json
[
  {
    "id": 1,
    "deviceName": "Apple Watch Series 9 - Müller",
    "deviceType": "AppleWatch",
    "manufacturer": "Apple",
    "model": "Series 9",
    "serialNumber": "DMPC12345678",
    "phoneNumber": "+4930123456",
    "sipIdentifier": "watch_mueller_001",
    "clientId": 1,
    "clientName": "Max Müller",
    "status": "Active",
    "isOnline": true,
    "batteryLevel": 85,
    "lastHeartbeat": "2026-01-21T16:30:00Z",
    "lastAlertTime": null,
    "createdAt": "2026-01-15T10:00:00Z"
  }
]
```

### 2. Einzelnes Gerät abrufen

```http
GET /servicehub/devices/{id}
```

### 3. Neues Gerät hinzufügen

```http
POST /servicehub/devices
Content-Type: application/json
```

**Request Body:**
```json
{
  "deviceName": "GPS-Tracker Outdoor",
  "deviceType": "GPSTracker",
  "manufacturer": "PAJ",
  "model": "ALLROUND Finder 4G",
  "serialNumber": "PAJ-2024-001234",
  "phoneNumber": "+4915112345678",
  "sipIdentifier": "gps_outdoor_001",
  "clientId": 5,
  "status": "Active"
}
```

**Response (201 Created):**
```json
{
  "id": 10,
  "deviceName": "GPS-Tracker Outdoor",
  "deviceType": "GPSTracker",
  "manufacturer": "PAJ",
  "model": "ALLROUND Finder 4G",
  "serialNumber": "PAJ-2024-001234",
  "phoneNumber": "+4915112345678",
  "sipIdentifier": "gps_outdoor_001",
  "clientId": 5,
  "clientName": "Erika Schmidt",
  "status": "Active",
  "isOnline": false,
  "batteryLevel": 100,
  "createdAt": "2026-01-21T17:00:00Z"
}
```

### 4. Gerät aktualisieren

```http
PUT /servicehub/devices/{id}
Content-Type: application/json
```

**Request Body:**
```json
{
  "deviceName": "GPS-Tracker Outdoor - Aktualisiert",
  "status": "Active",
  "batteryLevel": 75,
  "isOnline": true
}
```

### 5. Gerät löschen

```http
DELETE /servicehub/devices/{id}
```

### 6. Gerät-Heartbeat aktualisieren

```http
POST /servicehub/devices/{id}/heartbeat
Content-Type: application/json
```

**Request Body:**
```json
{
  "batteryLevel": 80,
  "latitude": 52.5200,
  "longitude": 13.4050,
  "signalStrength": -65
}
```

---

## Beispiele

### Beispiel 1: GPS-Tracker hinzufügen

```bash
curl -X POST "https://umo-api-sqlxpert-bpdeh4g3ewaybafn.germanywestcentral-01.azurewebsites.net/servicehub/devices" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceName": "PAJ GPS-Tracker für Herrn Meier",
    "deviceType": "GPSTracker",
    "manufacturer": "PAJ",
    "model": "ALLROUND Finder 4G",
    "serialNumber": "PAJ-2024-005678",
    "phoneNumber": "+4915198765432",
    "sipIdentifier": "gps_meier_001",
    "clientId": 3,
    "status": "Active"
  }'
```

### Beispiel 2: Apple Watch hinzufügen

```bash
curl -X POST "https://umo-api-sqlxpert-bpdeh4g3ewaybafn.germanywestcentral-01.azurewebsites.net/servicehub/devices" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceName": "Apple Watch Ultra 2 - Frau Schmidt",
    "deviceType": "AppleWatch",
    "manufacturer": "Apple",
    "model": "Ultra 2",
    "serialNumber": "FVFXC2ABCDEF",
    "phoneNumber": "+4917612345678",
    "sipIdentifier": "watch_schmidt_001",
    "clientId": 7,
    "status": "Active"
  }'
```

### Beispiel 3: Hausnotruf-Gerät hinzufügen

```bash
curl -X POST "https://umo-api-sqlxpert-bpdeh4g3ewaybafn.germanywestcentral-01.azurewebsites.net/servicehub/devices" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceName": "Tunstall Lifeline Vi+ - Wohnung 12",
    "deviceType": "Hausnotruf",
    "manufacturer": "Tunstall",
    "model": "Lifeline Vi+",
    "serialNumber": "TUN-2024-112233",
    "phoneNumber": "+493012345678",
    "sipIdentifier": "hausnotruf_wohnung12",
    "clientId": 12,
    "status": "Active"
  }'
```

### Beispiel 4: Medizinischer Alarmgeber hinzufügen

```bash
curl -X POST "https://umo-api-sqlxpert-bpdeh4g3ewaybafn.germanywestcentral-01.azurewebsites.net/servicehub/devices" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceName": "Medical Guardian - Herr Weber",
    "deviceType": "MedicalAlert",
    "manufacturer": "Medical Guardian",
    "model": "Mobile 2.0",
    "serialNumber": "MG-2024-789012",
    "phoneNumber": "+4916012345678",
    "sipIdentifier": "medical_weber_001",
    "clientId": 15,
    "status": "Active"
  }'
```

---

## Webhook-Integration

Für automatische Alarme von Geräten können Webhooks konfiguriert werden.

### Alarm-Webhook

```http
POST /servicehub/alerts
Content-Type: application/json
```

**Request Body:**
```json
{
  "alertType": "FallDetection",
  "priority": "Critical",
  "emergencyDeviceId": 10,
  "callerNumber": "+4915112345678",
  "latitude": 52.5200,
  "longitude": 13.4050,
  "heartRate": 95,
  "notes": "Automatischer Sturz-Alarm von GPS-Tracker"
}
```

### Unterstützte Alarm-Typen

| AlertType | Beschreibung |
|-----------|--------------|
| `FallDetection` | Sturzerkennung |
| `ManualAlert` | Manueller Notruf (Taste gedrückt) |
| `InactivityAlert` | Keine Aktivität erkannt |
| `LowBattery` | Niedriger Akkustand |
| `Panic` | Panik-Alarm |
| `Medical` | Medizinischer Notfall |
| `Geofence` | Geofence-Verletzung |
| `SOS` | SOS-Signal |

---

## Fehlerbehandlung

### HTTP-Statuscodes

| Code | Beschreibung |
|------|--------------|
| 200 | Erfolg |
| 201 | Ressource erstellt |
| 204 | Erfolgreich, keine Inhalte |
| 400 | Ungültige Anfrage |
| 404 | Ressource nicht gefunden |
| 409 | Konflikt (z.B. doppelte Seriennummer) |
| 500 | Serverfehler |

### Fehler-Response

```json
{
  "error": "Validation failed",
  "message": "DeviceType ist erforderlich",
  "field": "deviceType"
}
```

---

## Datenmodell

### EmergencyDevice

| Feld | Typ | Beschreibung | Erforderlich |
|------|-----|--------------|--------------|
| `deviceName` | string(100) | Name des Geräts | Ja |
| `deviceType` | string(50) | Gerätetyp (siehe Liste) | Ja |
| `manufacturer` | string(50) | Hersteller | Nein |
| `model` | string(100) | Modellbezeichnung | Nein |
| `serialNumber` | string(50) | Seriennummer | Nein |
| `phoneNumber` | string(20) | Telefonnummer des Geräts | Nein |
| `sipIdentifier` | string(50) | SIP-ID für Identifikation | Nein |
| `clientId` | int | ID des zugeordneten Klienten | Nein |
| `status` | string(20) | Status (Active, Inactive, Maintenance) | Nein |

---

## Swagger-Dokumentation

Die vollständige interaktive API-Dokumentation ist verfügbar unter:

```
https://umo-api-sqlxpert-bpdeh4g3ewaybafn.germanywestcentral-01.azurewebsites.net/swagger
```

Navigieren Sie zum Abschnitt **ServiceHub** für alle verfügbaren Endpunkte.
