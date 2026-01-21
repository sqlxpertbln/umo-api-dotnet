# Service Hub API Dokumentation

## Übersicht

Das Service Hub Modul bietet eine vollständige REST API für den Betrieb einer Notruf-Leitstelle (Hausnotruf-Zentrale). Es integriert VoIP-Telefonie über SIP Gate, Notfall-Geräteverwaltung, Alarm-Management und SMS-Benachrichtigungen.

## Basis-URL

```
/api/servicehub
```

## Authentifizierung

Aktuell keine Authentifizierung erforderlich (Entwicklungsmodus). Für Produktionsumgebungen sollte JWT-Bearer-Token-Authentifizierung implementiert werden.

---

## Endpunkte

### 1. Dashboard

#### GET /api/servicehub/dashboard
Liefert Übersichtsdaten für das Leitstellen-Dashboard.

**Response:**
```json
{
  "activeAlerts": 5,
  "activeCalls": 2,
  "onlineDispatchers": 3,
  "onlineDevices": 42,
  "recentAlerts": [...],
  "recentCalls": [...]
}
```

---

### 2. Notruf-Geräte (Emergency Devices)

#### GET /api/servicehub/devices
Listet alle registrierten Notruf-Geräte auf.

**Response:**
```json
[
  {
    "id": 1,
    "deviceType": "AppleWatch",
    "serialNumber": "AW-2024-001",
    "clientId": 1,
    "clientName": "Max Mustermann",
    "status": "Online",
    "batteryLevel": 85,
    "lastHeartbeat": "2026-01-21T10:15:00Z",
    "firmwareVersion": "10.2.1",
    "registeredAt": "2024-06-15T08:00:00Z"
  }
]
```

#### GET /api/servicehub/devices/{id}
Ruft ein einzelnes Gerät ab.

#### POST /api/servicehub/devices
Registriert ein neues Notruf-Gerät.

**Request Body:**
```json
{
  "deviceType": "AppleWatch",
  "serialNumber": "AW-2024-005",
  "clientId": 1,
  "status": "Online",
  "batteryLevel": 100,
  "firmwareVersion": "10.2.1"
}
```

**Unterstützte Gerätetypen (DeviceType):**

| Typ | Beschreibung |
|-----|--------------|
| `AppleWatch` | Apple Watch mit Sturzerkennung und Notruf-Funktion |
| `HomeEmergencyButton` | Stationärer Hausnotruf-Knopf |
| `MobileEmergencyButton` | Mobiler Notruf-Sender (für unterwegs) |
| `FallDetector` | Spezieller Sturzsensor (Anhänger/Armband) |
| `GPSTracker` | GPS-Tracker für Demenz-Patienten |
| `SmartPendant` | Intelligenter Notruf-Anhänger |
| `BedSensor` | Bett-Sensor für Aktivitätsüberwachung |
| `DoorSensor` | Tür-Sensor für Weglauf-Prävention |
| `SmokeDetector` | Vernetzter Rauchmelder |
| `MotionSensor` | Bewegungsmelder für Inaktivitäts-Alarme |

#### PUT /api/servicehub/devices/{id}
Aktualisiert ein bestehendes Gerät.

#### DELETE /api/servicehub/devices/{id}
Löscht ein Gerät aus dem System.

---

### 3. Notfall-Alarme (Emergency Alerts)

#### GET /api/servicehub/alerts
Listet alle Alarme auf.

**Query Parameter:**
- `status` (optional): Filtert nach Status (New, InProgress, Resolved, Cancelled)
- `priority` (optional): Filtert nach Priorität (Low, Medium, High, Critical)

**Response:**
```json
[
  {
    "id": 1,
    "alertType": "FallDetection",
    "priority": "High",
    "status": "New",
    "deviceId": 1,
    "clientId": 1,
    "clientName": "Max Mustermann",
    "location": "Wohnzimmer",
    "gpsLatitude": 52.5200,
    "gpsLongitude": 13.4050,
    "heartRate": 110,
    "triggeredAt": "2026-01-21T10:18:00Z",
    "notes": "Automatische Sturzerkennung durch Apple Watch"
  }
]
```

#### GET /api/servicehub/alerts/active
Liefert nur aktive (nicht abgeschlossene) Alarme.

#### POST /api/servicehub/alerts
Erstellt einen neuen Alarm (z.B. von einem Gerät).

**Request Body:**
```json
{
  "alertType": "FallDetection",
  "priority": "High",
  "deviceId": 1,
  "clientId": 1,
  "location": "Wohnzimmer",
  "gpsLatitude": 52.5200,
  "gpsLongitude": 13.4050,
  "heartRate": 110,
  "notes": "Automatische Sturzerkennung"
}
```

**Unterstützte Alarm-Typen (AlertType):**

| Typ | Beschreibung | Standard-Priorität |
|-----|--------------|-------------------|
| `FallDetection` | Automatische Sturzerkennung | High |
| `ManualAlert` | Manuell ausgelöster Notruf | Critical |
| `InactivityAlert` | Keine Aktivität erkannt | Medium |
| `LowBattery` | Niedriger Batteriestand | Low |
| `DeviceOffline` | Gerät nicht erreichbar | Medium |
| `SmokeDetected` | Rauch erkannt | Critical |
| `DoorOpened` | Unerwartete Türöffnung | Medium |
| `Wandering` | Weglauf-Alarm (GPS) | High |
| `HealthEmergency` | Gesundheitsnotfall (manuell) | Critical |
| `TestAlert` | Test-Alarm | Low |

#### PUT /api/servicehub/alerts/{id}/status
Aktualisiert den Status eines Alarms.

**Request Body:**
```json
{
  "status": "InProgress",
  "dispatcherId": 1,
  "notes": "Rückruf wird durchgeführt"
}
```

---

### 4. Anrufprotokoll (Call Log)

#### GET /api/servicehub/calls
Listet alle Anrufe auf.

**Response:**
```json
[
  {
    "id": 1,
    "callDirection": "Inbound",
    "phoneNumber": "+4930123456",
    "clientId": 1,
    "clientName": "Max Mustermann",
    "alertId": 1,
    "dispatcherId": 1,
    "dispatcherName": "Anna Schmidt",
    "callStatus": "Completed",
    "startTime": "2026-01-21T10:20:00Z",
    "endTime": "2026-01-21T10:25:30Z",
    "duration": 330,
    "notes": "Patient bestätigt Sturz, Rettungsdienst alarmiert"
  }
]
```

#### POST /api/servicehub/calls
Protokolliert einen neuen Anruf.

**Request Body:**
```json
{
  "callDirection": "Outbound",
  "phoneNumber": "+4930123456",
  "clientId": 1,
  "alertId": 1,
  "dispatcherId": 1,
  "notes": "Rückruf nach Sturz-Alarm"
}
```

#### PUT /api/servicehub/calls/{id}/end
Beendet einen aktiven Anruf.

**Request Body:**
```json
{
  "notes": "Entwarnung, Patient wohlauf"
}
```

---

### 5. Disponenten (Dispatchers)

#### GET /api/servicehub/dispatchers
Listet alle Disponenten auf.

#### GET /api/servicehub/dispatchers/online
Listet nur online verfügbare Disponenten auf.

#### POST /api/servicehub/dispatchers
Erstellt einen neuen Disponenten.

**Request Body:**
```json
{
  "name": "Anna Schmidt",
  "extension": "101",
  "email": "anna.schmidt@leitstelle.de",
  "isOnline": true,
  "skills": "Medizinische Notfälle, Demenz-Betreuung"
}
```

#### PUT /api/servicehub/dispatchers/{id}/status
Aktualisiert den Online-Status eines Disponenten.

---

### 6. Notfallkontakte (Emergency Contacts)

#### GET /api/servicehub/contacts
Listet alle Notfallkontakte auf.

#### GET /api/servicehub/contacts/client/{clientId}
Listet Notfallkontakte für einen bestimmten Klienten auf.

#### POST /api/servicehub/contacts
Erstellt einen neuen Notfallkontakt.

**Request Body:**
```json
{
  "clientId": 1,
  "name": "Maria Mustermann",
  "relationship": "Tochter",
  "phoneNumber": "+49151123456",
  "email": "maria@example.com",
  "priority": 1,
  "notifyOnAlert": true,
  "notifyViaSms": true,
  "notifyViaEmail": true
}
```

---

### 7. SMS-Benachrichtigung

#### POST /api/servicehub/sms/send
Sendet eine SMS über SIP Gate.

**Request Body:**
```json
{
  "phoneNumber": "+49151123456",
  "message": "Notruf-Alarm für Max Mustermann. Bitte kontaktieren Sie die Leitstelle: 030-123456"
}
```

**Response:**
```json
{
  "success": true,
  "messageId": "sms-12345",
  "message": "SMS erfolgreich gesendet"
}
```

#### POST /api/servicehub/alerts/{alertId}/notify-contacts
Benachrichtigt alle Notfallkontakte eines Klienten per SMS.

**Response:**
```json
{
  "success": true,
  "notifiedContacts": 3,
  "message": "3 Kontakte wurden per SMS benachrichtigt"
}
```

---

### 8. Statistiken

#### GET /api/servicehub/statistics
Liefert Statistiken für das Dashboard.

**Query Parameter:**
- `period` (optional): Zeitraum (day, week, month, year). Standard: month

**Response:**
```json
{
  "period": "month",
  "totalAlerts": 156,
  "alertsByType": {
    "FallDetection": 45,
    "ManualAlert": 32,
    "InactivityAlert": 28,
    "LowBattery": 51
  },
  "alertsByPriority": {
    "Critical": 12,
    "High": 45,
    "Medium": 67,
    "Low": 32
  },
  "averageResponseTime": 45,
  "totalCalls": 234,
  "averageCallDuration": 180
}
```

---

## SIP Gate Integration

### Konfiguration

Die SIP Gate Integration ist vorkonfiguriert mit:

| Parameter | Wert |
|-----------|------|
| SIP Server | sipconnect.sipgate.de |
| Benutzer | 3938564t0 |
| WebSocket | wss://sipconnect.sipgate.de |
| API URL | https://api.sipgate.com/v2 |

### WebRTC Softphone

Das Service Hub Frontend enthält einen integrierten WebRTC-Softphone, der über SIP.js mit dem SIP Gate Server kommuniziert. Funktionen:

- Eingehende Anrufe annehmen
- Ausgehende Anrufe tätigen
- Anrufe halten/fortsetzen
- Stummschaltung
- DTMF-Töne senden

---

## Neue Gerätetypen hinzufügen

Um einen neuen Gerätetyp hinzuzufügen:

1. **Gerät registrieren:**
```bash
curl -X POST "https://api.example.com/api/servicehub/devices" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceType": "NewDeviceType",
    "serialNumber": "NEW-2024-001",
    "clientId": 1,
    "status": "Online",
    "batteryLevel": 100,
    "firmwareVersion": "1.0.0"
  }'
```

2. **Alarm-Empfang konfigurieren:**
Das Gerät sollte Alarme an den POST-Endpunkt `/api/servicehub/alerts` senden.

3. **Heartbeat implementieren:**
Regelmäßige Status-Updates über PUT `/api/servicehub/devices/{id}` senden.

---

## Fehler-Codes

| HTTP Status | Bedeutung |
|-------------|-----------|
| 200 | Erfolgreiche Anfrage |
| 201 | Ressource erstellt |
| 400 | Ungültige Anfrage |
| 404 | Ressource nicht gefunden |
| 500 | Server-Fehler |

---

## Beispiel-Workflow: Alarm-Bearbeitung

1. **Alarm wird ausgelöst** (z.B. Sturzerkennung auf Apple Watch)
2. **POST /api/servicehub/alerts** - Alarm wird erstellt
3. **POST /api/servicehub/alerts/{id}/notify-contacts** - Kontakte werden benachrichtigt
4. **Disponent sieht Alarm** im Dashboard
5. **PUT /api/servicehub/alerts/{id}/status** - Status auf "InProgress" setzen
6. **POST /api/servicehub/calls** - Anruf wird protokolliert
7. **Rückruf an Klienten** über WebRTC-Softphone
8. **PUT /api/servicehub/calls/{id}/end** - Anruf beenden
9. **PUT /api/servicehub/alerts/{id}/status** - Status auf "Resolved" setzen

---

## Kontakt

Bei Fragen zur API wenden Sie sich an:
- E-Mail: support@sqlxpert.de
- Dokumentation: /swagger
