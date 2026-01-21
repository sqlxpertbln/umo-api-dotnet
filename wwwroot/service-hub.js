// Service Hub JavaScript - Notruf-Leitstelle mit VoIP Integration

const API_BASE = '';
let currentSection = 'dashboard';
let currentAlertId = null;

// SIP/WebRTC Configuration
const sipConfig = {
    server: 'sipconnect.sipgate.de',
    wsServer: 'wss://sipconnect.sipgate.de',
    username: '3938564t0',
    password: 'VEWqXdhf9wty',
    domain: 'sipgate.de'
};

// Phone state
let phoneState = {
    registered: false,
    inCall: false,
    muted: false,
    onHold: false,
    callStartTime: null,
    currentSession: null,
    userAgent: null
};

let dialedNumber = '';
let callTimer = null;

// Initialize on page load
document.addEventListener('DOMContentLoaded', () => {
    initNavigation();
    loadDashboard();
    updateClock();
    setInterval(updateClock, 1000);
    
    // Initialize softphone
    initSoftphone();
    
    // Auto-refresh dashboard every 30 seconds
    setInterval(() => {
        if (currentSection === 'dashboard') {
            loadDashboard();
        }
    }, 30000);
});

// Navigation
function initNavigation() {
    document.querySelectorAll('.nav-link[data-section]').forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            const section = link.dataset.section;
            showSection(section);
            
            // Update active state
            document.querySelectorAll('.nav-link').forEach(l => l.classList.remove('active'));
            link.classList.add('active');
        });
    });
}

function showSection(section) {
    currentSection = section;
    document.querySelectorAll('.content-section').forEach(s => s.style.display = 'none');
    document.getElementById(`${section}-section`).style.display = 'block';
    
    // Load section data
    switch(section) {
        case 'dashboard':
            loadDashboard();
            break;
        case 'alerts':
            loadAllAlerts();
            break;
        case 'calls':
            loadActiveCalls();
            break;
        case 'callhistory':
            loadCallHistory();
            break;
        case 'devices':
            loadDevices();
            break;
        case 'dispatchers':
            loadDispatchersGrid();
            break;
        case 'statistics':
            loadStatistics();
            break;
    }
}

function updateClock() {
    const now = new Date();
    document.getElementById('currentTime').textContent = now.toLocaleTimeString('de-DE');
}

// Dashboard
async function loadDashboard() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/dashboard`);
        const data = await response.json();
        
        // Update stats
        document.getElementById('activeAlertsCount').textContent = data.activeAlerts || 0;
        document.getElementById('activeCallsCount').textContent = data.activeCalls || 0;
        document.getElementById('onlineDispatchersCount').textContent = data.onlineDispatchers || 0;
        document.getElementById('onlineDevicesCount').textContent = data.onlineDevices || 0;
        document.getElementById('alertCount').textContent = data.activeAlerts || 0;
        
        // Load recent alerts
        loadRecentAlerts(data.recentAlerts || []);
        
        // Load dispatcher list
        loadDispatcherList(data.dispatchers || []);
        
    } catch (error) {
        console.error('Error loading dashboard:', error);
    }
}

function loadRecentAlerts(alerts) {
    const container = document.getElementById('recentAlerts');
    
    if (alerts.length === 0) {
        container.innerHTML = `
            <div class="alert-card" style="border-left-color: #28a745;">
                <div class="text-center py-4">
                    <i class="bi bi-check-circle text-success" style="font-size: 3rem;"></i>
                    <h5 class="mt-3">Keine aktiven Alarme</h5>
                    <p class="text-muted mb-0">Alle Systeme funktionieren normal.</p>
                </div>
            </div>
        `;
        return;
    }
    
    container.innerHTML = alerts.map(alert => createAlertCard(alert)).join('');
}

function createAlertCard(alert) {
    const priorityClass = alert.priority?.toLowerCase() || 'medium';
    const statusBadge = getStatusBadge(alert.status);
    const alertIcon = getAlertIcon(alert.alertType);
    const timeAgo = formatTimeAgo(alert.alertTime);
    
    return `
        <div class="alert-card ${priorityClass}" onclick="showAlertDetail(${alert.id})">
            <div class="alert-header">
                <div class="alert-type">
                    <i class="bi ${alertIcon}"></i>
                    <span>${formatAlertType(alert.alertType)}</span>
                </div>
                <span class="alert-badge badge-${priorityClass}">${alert.priority}</span>
            </div>
            <div class="alert-client">${alert.clientName || 'Unbekannt'}</div>
            <div class="alert-info">
                <span><i class="bi bi-telephone"></i> ${alert.callerNumber || 'N/A'}</span>
                <span><i class="bi bi-clock"></i> ${timeAgo}</span>
                ${alert.heartRate ? `<span><i class="bi bi-heart-pulse"></i> ${alert.heartRate} BPM</span>` : ''}
            </div>
            <div class="alert-actions">
                <button class="btn btn-sm btn-success" onclick="event.stopPropagation(); callClient(${alert.id}, '${alert.callerNumber}')">
                    <i class="bi bi-telephone"></i> Anrufen
                </button>
                <button class="btn btn-sm btn-warning" onclick="event.stopPropagation(); acknowledgeAlertById(${alert.id})">
                    <i class="bi bi-check"></i> Bestätigen
                </button>
                ${statusBadge}
            </div>
        </div>
    `;
}

function getAlertIcon(type) {
    const icons = {
        'FallDetection': 'bi-person-walking',
        'ManualAlert': 'bi-hand-index',
        'InactivityAlert': 'bi-hourglass',
        'HeartRateAlert': 'bi-heart-pulse',
        'LowBattery': 'bi-battery-low',
        'DeviceOffline': 'bi-wifi-off'
    };
    return icons[type] || 'bi-exclamation-triangle';
}

function formatAlertType(type) {
    const types = {
        'FallDetection': 'Sturzerkennung',
        'ManualAlert': 'Manueller Alarm',
        'InactivityAlert': 'Inaktivitätsalarm',
        'HeartRateAlert': 'Herzfrequenz-Alarm',
        'LowBattery': 'Niedriger Akkustand',
        'DeviceOffline': 'Gerät offline'
    };
    return types[type] || type;
}

function getStatusBadge(status) {
    const badges = {
        'New': '<span class="badge bg-primary">Neu</span>',
        'InProgress': '<span class="badge bg-warning text-dark">In Bearbeitung</span>',
        'Resolved': '<span class="badge bg-success">Abgeschlossen</span>'
    };
    return badges[status] || '';
}

function formatTimeAgo(dateString) {
    const date = new Date(dateString);
    const now = new Date();
    const diff = Math.floor((now - date) / 1000);
    
    if (diff < 60) return `vor ${diff} Sek.`;
    if (diff < 3600) return `vor ${Math.floor(diff / 60)} Min.`;
    if (diff < 86400) return `vor ${Math.floor(diff / 3600)} Std.`;
    return date.toLocaleDateString('de-DE');
}

function loadDispatcherList(dispatchers) {
    const container = document.getElementById('dispatcherList');
    
    container.innerHTML = dispatchers.map(d => `
        <div class="dispatcher-card">
            <div class="dispatcher-avatar">${d.firstName?.[0] || ''}${d.lastName?.[0] || ''}</div>
            <div class="dispatcher-info">
                <div class="dispatcher-name">${d.firstName} ${d.lastName}</div>
                <div class="dispatcher-status">
                    <span class="status-dot ${d.status?.toLowerCase() || 'offline'}"></span>
                    ${d.status || 'Offline'}
                    ${d.currentCalls > 0 ? `<span class="badge bg-warning text-dark ms-2">${d.currentCalls} Anruf(e)</span>` : ''}
                </div>
            </div>
            <div>
                <span class="badge bg-secondary">${d.extension || ''}</span>
            </div>
        </div>
    `).join('');
}

// Alerts
async function loadAllAlerts() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/alerts`);
        const alerts = await response.json();
        
        const container = document.getElementById('allAlerts');
        container.innerHTML = alerts.map(alert => createAlertCard(alert)).join('');
        
    } catch (error) {
        console.error('Error loading alerts:', error);
    }
}

async function showAlertDetail(alertId) {
    currentAlertId = alertId;
    
    try {
        const response = await fetch(`${API_BASE}/servicehub/alerts/${alertId}`);
        const alert = await response.json();
        
        const content = document.getElementById('alertDetailContent');
        content.innerHTML = `
            <div class="row">
                <div class="col-md-6">
                    <h6 class="text-muted">Alarm-Informationen</h6>
                    <table class="table table-dark table-sm">
                        <tr><td>Typ:</td><td>${formatAlertType(alert.alertType)}</td></tr>
                        <tr><td>Priorität:</td><td><span class="badge bg-${alert.priority?.toLowerCase()}">${alert.priority}</span></td></tr>
                        <tr><td>Status:</td><td>${alert.status}</td></tr>
                        <tr><td>Zeit:</td><td>${new Date(alert.alertTime).toLocaleString('de-DE')}</td></tr>
                        ${alert.heartRate ? `<tr><td>Herzfrequenz:</td><td>${alert.heartRate} BPM</td></tr>` : ''}
                    </table>
                </div>
                <div class="col-md-6">
                    <h6 class="text-muted">Klient</h6>
                    <table class="table table-dark table-sm">
                        <tr><td>Name:</td><td>${alert.clientName || 'Unbekannt'}</td></tr>
                        <tr><td>Telefon:</td><td>${alert.callerNumber || 'N/A'}</td></tr>
                        <tr><td>Gerät:</td><td>${alert.deviceName || 'N/A'}</td></tr>
                    </table>
                </div>
            </div>
            ${alert.latitude && alert.longitude ? `
            <div class="map-container mt-3">
                <i class="bi bi-geo-alt"></i> Standort: ${alert.latitude.toFixed(4)}, ${alert.longitude.toFixed(4)}
            </div>
            ` : ''}
            ${alert.notes ? `
            <div class="mt-3">
                <h6 class="text-muted">Notizen</h6>
                <p>${alert.notes}</p>
            </div>
            ` : ''}
        `;
        
        new bootstrap.Modal(document.getElementById('alertDetailModal')).show();
        
    } catch (error) {
        console.error('Error loading alert detail:', error);
    }
}

async function acknowledgeAlertById(alertId) {
    try {
        await fetch(`${API_BASE}/servicehub/alerts/${alertId}/acknowledge`, { method: 'POST' });
        loadDashboard();
    } catch (error) {
        console.error('Error acknowledging alert:', error);
    }
}

async function acknowledgeAlert() {
    if (currentAlertId) {
        await acknowledgeAlertById(currentAlertId);
        bootstrap.Modal.getInstance(document.getElementById('alertDetailModal')).hide();
    }
}

async function resolveAlert() {
    if (currentAlertId) {
        try {
            await fetch(`${API_BASE}/servicehub/alerts/${currentAlertId}/resolve`, { 
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ resolution: 'Resolved', notes: 'Abgeschlossen durch Disponenten' })
            });
            bootstrap.Modal.getInstance(document.getElementById('alertDetailModal')).hide();
            loadDashboard();
        } catch (error) {
            console.error('Error resolving alert:', error);
        }
    }
}

// Call History
async function loadCallHistory() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/calls/history`);
        const calls = await response.json();
        
        const tbody = document.getElementById('callHistoryTable');
        tbody.innerHTML = calls.map(call => `
            <tr>
                <td>${new Date(call.startTime).toLocaleString('de-DE')}</td>
                <td><span class="direction-badge ${call.direction?.toLowerCase()}">${call.direction === 'Inbound' ? 'Eingehend' : 'Ausgehend'}</span></td>
                <td>${call.direction === 'Inbound' ? call.callerNumber : call.calleeNumber}</td>
                <td>${call.clientName || '-'}</td>
                <td>${call.dispatcherName || '-'}</td>
                <td>${formatDuration(call.durationSeconds)}</td>
                <td><span class="badge bg-secondary">${call.callType || '-'}</span></td>
                <td><span class="badge ${call.status === 'Ended' ? 'bg-success' : 'bg-warning'}">${call.status}</span></td>
            </tr>
        `).join('');
        
    } catch (error) {
        console.error('Error loading call history:', error);
    }
}

function formatDuration(seconds) {
    if (!seconds) return '-';
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, '0')}`;
}

// Devices
async function loadDevices() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/devices`);
        const devices = await response.json();
        
        const container = document.getElementById('devicesList');
        container.innerHTML = devices.map(device => `
            <div class="col-md-6 col-lg-4">
                <div class="device-card">
                    <div class="device-header">
                        <div class="d-flex align-items-center gap-3">
                            <div class="device-icon ${device.deviceType?.toLowerCase() || 'hausnotruf'}">
                                <i class="bi ${getDeviceIcon(device.deviceType)}"></i>
                            </div>
                            <div>
                                <div class="fw-bold">${device.deviceName}</div>
                                <small class="text-muted">${device.manufacturer} ${device.model}</small>
                            </div>
                        </div>
                        <span class="badge ${device.isOnline ? 'bg-success' : 'bg-danger'}">
                            ${device.isOnline ? 'Online' : 'Offline'}
                        </span>
                    </div>
                    <div class="d-flex justify-content-between align-items-center mt-3">
                        <div class="battery-indicator">
                            <i class="bi bi-battery-${getBatteryIcon(device.batteryLevel)}"></i>
                            <div class="battery-bar">
                                <div class="battery-fill ${getBatteryClass(device.batteryLevel)}" style="width: ${device.batteryLevel}%"></div>
                            </div>
                            <span>${device.batteryLevel}%</span>
                        </div>
                        <small class="text-muted">
                            <i class="bi bi-clock"></i> ${formatTimeAgo(device.lastHeartbeat)}
                        </small>
                    </div>
                    <div class="mt-3 pt-3 border-top border-secondary">
                        <small class="text-muted">Klient: ${device.clientName || 'Nicht zugewiesen'}</small><br>
                        <small class="text-muted">Tel: ${device.phoneNumber || 'N/A'}</small>
                    </div>
                </div>
            </div>
        `).join('');
        
    } catch (error) {
        console.error('Error loading devices:', error);
    }
}

function getDeviceIcon(type) {
    const icons = {
        'AppleWatch': 'bi-smartwatch',
        'Hausnotruf': 'bi-house',
        'MobilNotruf': 'bi-phone'
    };
    return icons[type] || 'bi-device-hdd';
}

function getBatteryIcon(level) {
    if (level > 75) return 'full';
    if (level > 50) return 'half';
    if (level > 25) return 'low';
    return 'empty';
}

function getBatteryClass(level) {
    if (level > 50) return 'high';
    if (level > 25) return 'medium';
    return 'low';
}

// Dispatchers
async function loadDispatchersGrid() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/dispatchers`);
        const dispatchers = await response.json();
        
        const container = document.getElementById('dispatchersGrid');
        container.innerHTML = dispatchers.map(d => `
            <div class="col-md-6 col-lg-4">
                <div class="stat-card">
                    <div class="d-flex align-items-center gap-3 mb-3">
                        <div class="dispatcher-avatar" style="width: 60px; height: 60px; font-size: 1.5rem;">
                            ${d.firstName?.[0] || ''}${d.lastName?.[0] || ''}
                        </div>
                        <div class="text-start">
                            <h5 class="mb-0">${d.firstName} ${d.lastName}</h5>
                            <small class="text-muted">${d.role || 'Agent'}</small>
                        </div>
                    </div>
                    <div class="d-flex justify-content-between mb-2">
                        <span class="text-muted">Status:</span>
                        <span class="dispatcher-status">
                            <span class="status-dot ${d.status?.toLowerCase() || 'offline'}"></span>
                            ${d.status || 'Offline'}
                        </span>
                    </div>
                    <div class="d-flex justify-content-between mb-2">
                        <span class="text-muted">Nebenstelle:</span>
                        <span>${d.extension || '-'}</span>
                    </div>
                    <div class="d-flex justify-content-between mb-2">
                        <span class="text-muted">Anrufe heute:</span>
                        <span>${d.totalCallsHandled || 0}</span>
                    </div>
                    <div class="mt-3">
                        <button class="btn btn-sm btn-outline-light w-100" onclick="setDispatcherStatus(${d.id})">
                            <i class="bi bi-gear"></i> Status ändern
                        </button>
                    </div>
                </div>
            </div>
        `).join('');
        
    } catch (error) {
        console.error('Error loading dispatchers:', error);
    }
}

// Statistics
async function loadStatistics() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/statistics`);
        const stats = await response.json();
        
        // Alerts by type chart
        new Chart(document.getElementById('alertsChart'), {
            type: 'doughnut',
            data: {
                labels: stats.alertsByType?.map(a => formatAlertType(a.type)) || [],
                datasets: [{
                    data: stats.alertsByType?.map(a => a.count) || [],
                    backgroundColor: ['#dc3545', '#fd7e14', '#ffc107', '#28a745', '#17a2b8', '#6c757d']
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    title: {
                        display: true,
                        text: 'Alarme nach Typ',
                        color: '#fff'
                    },
                    legend: {
                        labels: { color: '#fff' }
                    }
                }
            }
        });
        
        // Calls over time chart
        new Chart(document.getElementById('callsChart'), {
            type: 'line',
            data: {
                labels: stats.callsOverTime?.map(c => c.date) || [],
                datasets: [{
                    label: 'Anrufe',
                    data: stats.callsOverTime?.map(c => c.count) || [],
                    borderColor: '#28a745',
                    backgroundColor: 'rgba(40, 167, 69, 0.2)',
                    fill: true,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    title: {
                        display: true,
                        text: 'Anrufe über Zeit',
                        color: '#fff'
                    },
                    legend: {
                        labels: { color: '#fff' }
                    }
                },
                scales: {
                    x: { ticks: { color: '#adb5bd' }, grid: { color: 'rgba(255,255,255,0.1)' } },
                    y: { ticks: { color: '#adb5bd' }, grid: { color: 'rgba(255,255,255,0.1)' } }
                }
            }
        });
        
    } catch (error) {
        console.error('Error loading statistics:', error);
    }
}

// Active Calls
async function loadActiveCalls() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/calls/active`);
        const calls = await response.json();
        
        const container = document.getElementById('activeCalls');
        
        if (calls.length === 0) {
            container.innerHTML = `
                <div class="stat-card text-center py-5">
                    <i class="bi bi-telephone-x text-muted" style="font-size: 4rem;"></i>
                    <h4 class="mt-3">Keine aktiven Anrufe</h4>
                </div>
            `;
            return;
        }
        
        container.innerHTML = calls.map(call => `
            <div class="alert-card medium">
                <div class="d-flex justify-content-between align-items-center">
                    <div>
                        <h5><i class="bi bi-telephone-fill text-success"></i> ${call.direction === 'Inbound' ? call.callerNumber : call.calleeNumber}</h5>
                        <p class="mb-0 text-muted">${call.clientName || 'Unbekannt'} • ${call.dispatcherName || 'Nicht zugewiesen'}</p>
                    </div>
                    <div class="text-end">
                        <div class="badge bg-success mb-2">Aktiv</div>
                        <div>${formatDuration(Math.floor((new Date() - new Date(call.connectTime)) / 1000))}</div>
                    </div>
                </div>
            </div>
        `).join('');
        
    } catch (error) {
        console.error('Error loading active calls:', error);
    }
}

// =====================
// SOFTPHONE FUNCTIONS
// =====================

function initSoftphone() {
    updatePhoneStatus('offline', 'Verbinde...');
    
    // For demo purposes, we'll simulate the connection
    // In production, this would use SIP.js to connect to sipgate
    setTimeout(() => {
        updatePhoneStatus('online', 'Bereit');
        phoneState.registered = true;
    }, 2000);
    
    // Note: Full SIP.js implementation would look like this:
    /*
    try {
        const uri = SIP.UserAgent.makeURI(`sip:${sipConfig.username}@${sipConfig.domain}`);
        phoneState.userAgent = new SIP.UserAgent({
            uri: uri,
            transportOptions: {
                server: sipConfig.wsServer
            },
            authorizationUsername: sipConfig.username,
            authorizationPassword: sipConfig.password
        });
        
        phoneState.userAgent.start().then(() => {
            const registerer = new SIP.Registerer(phoneState.userAgent);
            registerer.register();
            updatePhoneStatus('online', 'Bereit');
            phoneState.registered = true;
        });
        
        phoneState.userAgent.delegate = {
            onInvite: (invitation) => {
                showIncomingCall(invitation);
            }
        };
    } catch (error) {
        console.error('SIP initialization error:', error);
        updatePhoneStatus('offline', 'Fehler');
    }
    */
}

function updatePhoneStatus(status, text) {
    const indicator = document.getElementById('phoneStatus');
    const statusText = document.getElementById('phoneStatusText');
    
    indicator.className = 'status-indicator ' + status;
    statusText.textContent = text;
}

function dialDigit(digit) {
    dialedNumber += digit;
    document.getElementById('dialDisplay').textContent = dialedNumber;
}

function clearDial() {
    dialedNumber = '';
    document.getElementById('dialDisplay').textContent = '';
}

async function makeCall() {
    if (!dialedNumber) {
        alert('Bitte geben Sie eine Rufnummer ein.');
        return;
    }
    
    if (!phoneState.registered) {
        alert('Telefon nicht verbunden. Bitte warten Sie.');
        return;
    }
    
    // Start call via API
    try {
        const response = await fetch(`${API_BASE}/servicehub/calls/initiate`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                calleeNumber: dialedNumber,
                dispatcherId: 1 // Current dispatcher
            })
        });
        
        if (response.ok) {
            phoneState.inCall = true;
            phoneState.callStartTime = new Date();
            updatePhoneStatus('oncall', `Anruf: ${dialedNumber}`);
            
            document.getElementById('callBtn').style.display = 'none';
            document.getElementById('hangupBtn').style.display = 'block';
            
            startCallTimer();
        }
    } catch (error) {
        console.error('Error initiating call:', error);
        alert('Anruf konnte nicht gestartet werden.');
    }
}

function hangupCall() {
    phoneState.inCall = false;
    phoneState.callStartTime = null;
    updatePhoneStatus('online', 'Bereit');
    
    document.getElementById('callBtn').style.display = 'block';
    document.getElementById('hangupBtn').style.display = 'none';
    document.getElementById('callDuration').textContent = '';
    
    stopCallTimer();
    clearDial();
}

function toggleMute() {
    phoneState.muted = !phoneState.muted;
    const btn = document.getElementById('muteBtn');
    btn.innerHTML = phoneState.muted ? '<i class="bi bi-mic-mute"></i>' : '<i class="bi bi-mic"></i>';
    btn.classList.toggle('active', phoneState.muted);
}

function toggleHold() {
    phoneState.onHold = !phoneState.onHold;
    const btn = document.getElementById('holdBtn');
    btn.classList.toggle('active', phoneState.onHold);
    
    if (phoneState.onHold) {
        updatePhoneStatus('oncall', 'Gehalten');
    } else {
        updatePhoneStatus('oncall', `Anruf: ${dialedNumber}`);
    }
}

function startCallTimer() {
    callTimer = setInterval(() => {
        if (phoneState.callStartTime) {
            const duration = Math.floor((new Date() - phoneState.callStartTime) / 1000);
            document.getElementById('callDuration').textContent = formatDuration(duration);
        }
    }, 1000);
}

function stopCallTimer() {
    if (callTimer) {
        clearInterval(callTimer);
        callTimer = null;
    }
}

function showIncomingCall(callerNumber, callerName) {
    document.getElementById('incomingCallerNumber').textContent = callerNumber;
    document.getElementById('incomingCallerName').textContent = callerName || 'Unbekannt';
    document.getElementById('incomingCallModal').classList.add('show');
    
    // Play ringtone (would need audio element)
}

function answerCall() {
    document.getElementById('incomingCallModal').classList.remove('show');
    phoneState.inCall = true;
    phoneState.callStartTime = new Date();
    updatePhoneStatus('oncall', 'Verbunden');
    
    document.getElementById('callBtn').style.display = 'none';
    document.getElementById('hangupBtn').style.display = 'block';
    
    startCallTimer();
}

function rejectCall() {
    document.getElementById('incomingCallModal').classList.remove('show');
}

function callClient(alertId, phoneNumber) {
    dialedNumber = phoneNumber;
    document.getElementById('dialDisplay').textContent = phoneNumber;
    makeCall();
}

// Simulate incoming call for demo
function simulateIncomingCall() {
    showIncomingCall('+4930123456001', 'Müller, Hans (Sturzerkennung)');
}
