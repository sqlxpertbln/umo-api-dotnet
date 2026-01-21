// UMO System - Integrierte Anwendung mit Service Hub VoIP
// Kombiniert Stammdatenverwaltung und Notruf-Leitstelle

const API_BASE = '';
let currentSection = 'dashboard';

// ============================================
// SIP/WebRTC Configuration for VoIP
// ============================================
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

// ============================================
// Initialization
// ============================================
document.addEventListener('DOMContentLoaded', () => {
    initNavigation();
    loadDashboard();
    updateClock();
    setInterval(updateClock, 1000);
    
    // Initialize softphone
    initSoftphone();
    
    // Check for active alerts periodically
    setInterval(checkActiveAlerts, 30000);
    checkActiveAlerts();
    
    // Handle responsive softphone
    handleResponsiveSoftphone();
    window.addEventListener('resize', handleResponsiveSoftphone);
});

// ============================================
// Navigation
// ============================================
function initNavigation() {
    document.querySelectorAll('.nav-link[data-section]').forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            const section = link.dataset.section;
            showSection(section);
            
            // Update active state
            document.querySelectorAll('.nav-link[data-section]').forEach(l => l.classList.remove('active'));
            link.classList.add('active');
        });
    });
}

function showSection(section) {
    currentSection = section;
    document.querySelectorAll('.content-section').forEach(s => s.style.display = 'none');
    const sectionEl = document.getElementById(`${section}-section`);
    if (sectionEl) {
        sectionEl.style.display = 'block';
    }
    
    // Load section data
    switch(section) {
        case 'dashboard':
            loadDashboard();
            break;
        case 'clients':
            loadClients();
            break;
        case 'devices':
            loadDevices();
            break;
        case 'directProviders':
            loadDirectProviders();
            break;
        case 'professionalProviders':
            loadProfessionalProviders();
            break;
        case 'systemEntries':
            loadSystemEntries();
            break;
        case 'serviceHub':
            loadServiceHubDashboard();
            break;
        case 'emergencyDevices':
            loadEmergencyDevices();
            break;
        case 'emergencyContacts':
            loadEmergencyContacts();
            break;
        case 'dispatchers':
            loadDispatchers();
            break;
        case 'callHistory':
            loadCallHistory();
            break;
    }
}

function updateClock() {
    const now = new Date();
    const timeStr = now.toLocaleTimeString('de-DE');
    const el1 = document.getElementById('currentTime');
    const el2 = document.getElementById('serviceHubTime');
    if (el1) el1.textContent = timeStr;
    if (el2) el2.textContent = timeStr;
}

// ============================================
// Dashboard
// ============================================
async function loadDashboard() {
    try {
        // Load main stats
        const [clientsRes, devicesRes, directRes, profRes] = await Promise.all([
            fetch(`${API_BASE}/clients`),
            fetch(`${API_BASE}/devices`),
            fetch(`${API_BASE}/directProvider`),
            fetch(`${API_BASE}/professionalProvider`)
        ]);
        
        const clients = await clientsRes.json();
        const devices = await devicesRes.json();
        const directProviders = await directRes.json();
        const profProviders = await profRes.json();
        
        document.getElementById('total-clients').textContent = clients.totalCount || clients.length || 0;
        document.getElementById('total-devices').textContent = devices.totalCount || devices.length || 0;
        document.getElementById('total-direct-providers').textContent = directProviders.totalCount || directProviders.length || 0;
        document.getElementById('total-professional-providers').textContent = profProviders.totalCount || profProviders.length || 0;
        
        // Load Service Hub stats
        try {
            const shRes = await fetch(`${API_BASE}/servicehub/dashboard`);
            const shData = await shRes.json();
            
            document.getElementById('active-alerts').textContent = shData.activeAlerts || 0;
            document.getElementById('active-calls').textContent = shData.activeCalls || 0;
            document.getElementById('online-dispatchers').textContent = shData.onlineDispatchers || 0;
            document.getElementById('online-emergency-devices').textContent = shData.onlineDevices || 0;
            
            // Update alert badge
            updateAlertBadge(shData.activeAlerts || 0);
            
            // Load recent alerts for dashboard
            loadDashboardAlerts(shData.recentAlerts || []);
        } catch (e) {
            console.log('Service Hub not available:', e);
        }
        
        // Load recent clients
        loadRecentClients(clients.items || clients || []);
        
    } catch (error) {
        console.error('Error loading dashboard:', error);
        showToast('Fehler beim Laden des Dashboards', 'danger');
    }
}

function loadRecentClients(clients) {
    const container = document.getElementById('recent-clients-list');
    const recentClients = clients.slice(0, 5);
    
    if (recentClients.length === 0) {
        container.innerHTML = '<p class="text-muted">Keine Klienten vorhanden</p>';
        return;
    }
    
    container.innerHTML = `
        <ul class="list-group list-group-flush">
            ${recentClients.map(c => `
                <li class="list-group-item d-flex justify-content-between align-items-center">
                    <div>
                        <strong>${c.firstName || ''} ${c.lastName || ''}</strong>
                        <br><small class="text-muted">ID: ${c.id || c.mandantId}-${c.clientId || ''}</small>
                    </div>
                    <span class="badge ${c.isActive ? 'bg-success' : 'bg-secondary'}">${c.isActive ? 'Aktiv' : 'Inaktiv'}</span>
                </li>
            `).join('')}
        </ul>
    `;
}

function loadDashboardAlerts(alerts) {
    const container = document.getElementById('dashboard-alerts');
    
    if (!alerts || alerts.length === 0) {
        container.innerHTML = `
            <div class="text-center text-muted py-3">
                <i class="bi bi-check-circle text-success" style="font-size: 2rem;"></i>
                <p class="mb-0 mt-2">Keine aktiven Alarme</p>
            </div>
        `;
        return;
    }
    
    container.innerHTML = alerts.slice(0, 3).map(alert => `
        <div class="alert-card ${(alert.priority || 'medium').toLowerCase()}" onclick="showSection('serviceHub')">
            <div class="alert-header">
                <div class="alert-type">
                    <i class="bi ${getAlertIcon(alert.alertType)}"></i>
                    <span>${formatAlertType(alert.alertType)}</span>
                </div>
                <span class="alert-badge badge-${(alert.priority || 'medium').toLowerCase()}">${alert.priority || 'Medium'}</span>
            </div>
            <div class="alert-client">${alert.clientName || 'Unbekannt'}</div>
            <div class="alert-info">
                <span><i class="bi bi-clock"></i> ${formatTimeAgo(alert.alertTime || alert.triggeredAt)}</span>
            </div>
        </div>
    `).join('');
}

// ============================================
// Clients
// ============================================
async function loadClients() {
    try {
        const response = await fetch(`${API_BASE}/clients`);
        const data = await response.json();
        const clients = data.items || data || [];
        
        const tbody = document.getElementById('clients-table-body');
        tbody.innerHTML = clients.map(c => `
            <tr>
                <td>${c.mandantId || 1}-${c.clientId || c.id}</td>
                <td>${c.firstName || ''} ${c.lastName || ''}</td>
                <td>${c.birthDate ? new Date(c.birthDate).toLocaleDateString('de-DE') : '-'}</td>
                <td><span class="status-badge ${c.isActive ? 'status-active' : 'status-inactive'}">${c.isActive ? 'Aktiv' : 'Inaktiv'}</span></td>
                <td>
                    <button class="btn btn-sm btn-outline-primary btn-action" onclick="viewClient(${c.mandantId || 1}, ${c.clientId || c.id})">
                        <i class="bi bi-eye"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-secondary btn-action" onclick="editClient(${c.mandantId || 1}, ${c.clientId || c.id})">
                        <i class="bi bi-pencil"></i>
                    </button>
                </td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Error loading clients:', error);
        showToast('Fehler beim Laden der Klienten', 'danger');
    }
}

// ============================================
// Devices
// ============================================
async function loadDevices() {
    try {
        const response = await fetch(`${API_BASE}/devices`);
        const data = await response.json();
        const devices = data.items || data || [];
        
        const tbody = document.getElementById('devices-table-body');
        tbody.innerHTML = devices.map(d => `
            <tr>
                <td>${d.mandantId || 1}-${d.deviceId || d.id}</td>
                <td>${d.serialNumber || '-'}</td>
                <td>${d.deviceType || '-'}</td>
                <td><span class="status-badge ${d.isActive ? 'status-active' : 'status-inactive'}">${d.isActive ? 'Aktiv' : 'Inaktiv'}</span></td>
                <td>
                    <button class="btn btn-sm btn-outline-primary btn-action" onclick="viewDevice(${d.mandantId || 1}, ${d.deviceId || d.id})">
                        <i class="bi bi-eye"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-secondary btn-action" onclick="editDevice(${d.mandantId || 1}, ${d.deviceId || d.id})">
                        <i class="bi bi-pencil"></i>
                    </button>
                </td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Error loading devices:', error);
        showToast('Fehler beim Laden der Geräte', 'danger');
    }
}

// ============================================
// Providers
// ============================================
async function loadDirectProviders() {
    try {
        const response = await fetch(`${API_BASE}/directProvider`);
        const data = await response.json();
        const providers = data.items || data || [];
        
        const tbody = document.getElementById('direct-providers-table-body');
        tbody.innerHTML = providers.map(p => `
            <tr>
                <td>${p.mandantId || 1}-${p.providerId || p.id}</td>
                <td>${p.name || p.firstName + ' ' + p.lastName || '-'}</td>
                <td>${p.providerType || '-'}</td>
                <td><span class="status-badge ${p.isActive ? 'status-active' : 'status-inactive'}">${p.isActive ? 'Aktiv' : 'Inaktiv'}</span></td>
                <td>
                    <button class="btn btn-sm btn-outline-primary btn-action">
                        <i class="bi bi-eye"></i>
                    </button>
                </td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Error loading direct providers:', error);
    }
}

async function loadProfessionalProviders() {
    try {
        const response = await fetch(`${API_BASE}/professionalProvider`);
        const data = await response.json();
        const providers = data.items || data || [];
        
        const tbody = document.getElementById('professional-providers-table-body');
        tbody.innerHTML = providers.map(p => `
            <tr>
                <td>${p.mandantId || 1}-${p.providerId || p.id}</td>
                <td>${p.name || '-'}</td>
                <td>${p.specialty || p.providerType || '-'}</td>
                <td><span class="status-badge ${p.isActive ? 'status-active' : 'status-inactive'}">${p.isActive ? 'Aktiv' : 'Inaktiv'}</span></td>
                <td>
                    <button class="btn btn-sm btn-outline-primary btn-action">
                        <i class="bi bi-eye"></i>
                    </button>
                </td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Error loading professional providers:', error);
    }
}

// ============================================
// System Entries
// ============================================
async function loadSystemEntries() {
    try {
        const [tarifsRes, languagesRes, diseasesRes] = await Promise.all([
            fetch(`${API_BASE}/tarifs`),
            fetch(`${API_BASE}/languages`),
            fetch(`${API_BASE}/diseases`)
        ]);
        
        const tarifs = await tarifsRes.json();
        const languages = await languagesRes.json();
        const diseases = await diseasesRes.json();
        
        document.getElementById('tarifs-list').innerHTML = (tarifs || []).map(t => 
            `<span class="badge bg-primary me-1 mb-1">${t.name || t.tarifName}</span>`
        ).join('') || '<p class="text-muted">Keine Tarife</p>';
        
        document.getElementById('languages-list').innerHTML = (languages || []).map(l => 
            `<span class="badge bg-info me-1 mb-1">${l.name || l.languageName}</span>`
        ).join('') || '<p class="text-muted">Keine Sprachen</p>';
        
        document.getElementById('diseases-list').innerHTML = (diseases || []).map(d => 
            `<span class="badge bg-secondary me-1 mb-1">${d.name || d.diseaseName}</span>`
        ).join('') || '<p class="text-muted">Keine Krankheiten</p>';
        
    } catch (error) {
        console.error('Error loading system entries:', error);
    }
}

// ============================================
// SERVICE HUB FUNCTIONS
// ============================================

async function loadServiceHubDashboard() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/dashboard`);
        const data = await response.json();
        
        // Update stats
        document.getElementById('sh-active-alerts').textContent = data.activeAlerts || 0;
        document.getElementById('sh-active-calls').textContent = data.activeCalls || 0;
        document.getElementById('sh-online-dispatchers').textContent = data.onlineDispatchers || 0;
        document.getElementById('sh-online-devices').textContent = data.onlineDevices || 0;
        
        // Update alert badge
        updateAlertBadge(data.activeAlerts || 0);
        
        // Load alerts
        loadServiceHubAlerts(data.recentAlerts || []);
        
        // Load dispatchers
        loadServiceHubDispatchers(data.dispatchers || []);
        
    } catch (error) {
        console.error('Error loading Service Hub dashboard:', error);
    }
}

function loadServiceHubAlerts(alerts) {
    const container = document.getElementById('sh-alerts-list');
    
    if (!alerts || alerts.length === 0) {
        container.innerHTML = `
            <div class="text-center py-4">
                <i class="bi bi-check-circle text-success" style="font-size: 3rem;"></i>
                <h5 class="mt-3">Keine aktiven Alarme</h5>
                <p class="text-muted mb-0">Alle Systeme funktionieren normal.</p>
            </div>
        `;
        return;
    }
    
    container.innerHTML = alerts.map(alert => createAlertCard(alert)).join('');
}

function createAlertCard(alert) {
    const priorityClass = (alert.priority || 'medium').toLowerCase();
    const statusBadge = getStatusBadge(alert.status);
    const alertIcon = getAlertIcon(alert.alertType);
    const timeAgo = formatTimeAgo(alert.alertTime || alert.triggeredAt);
    
    return `
        <div class="alert-card ${priorityClass}">
            <div class="alert-header">
                <div class="alert-type">
                    <i class="bi ${alertIcon}"></i>
                    <span>${formatAlertType(alert.alertType)}</span>
                </div>
                <span class="alert-badge badge-${priorityClass}">${alert.priority || 'Medium'}</span>
            </div>
            <div class="alert-client">${alert.clientName || 'Unbekannt'}</div>
            <div class="alert-info">
                <span><i class="bi bi-telephone"></i> ${alert.callerNumber || alert.phoneNumber || 'N/A'}</span>
                <span><i class="bi bi-clock"></i> ${timeAgo}</span>
                ${alert.heartRate ? `<span><i class="bi bi-heart-pulse"></i> ${alert.heartRate} BPM</span>` : ''}
            </div>
            <div class="alert-actions">
                <button class="btn btn-sm btn-success" onclick="callClient('${alert.callerNumber || alert.phoneNumber || ''}')">
                    <i class="bi bi-telephone"></i> Anrufen
                </button>
                <button class="btn btn-sm btn-warning" onclick="acknowledgeAlert(${alert.id})">
                    <i class="bi bi-check"></i> Bestätigen
                </button>
                <button class="btn btn-sm btn-info" onclick="notifyContacts(${alert.id})">
                    <i class="bi bi-send"></i> SMS senden
                </button>
                ${statusBadge}
            </div>
        </div>
    `;
}

function loadServiceHubDispatchers(dispatchers) {
    const container = document.getElementById('sh-dispatchers-list');
    
    if (!dispatchers || dispatchers.length === 0) {
        container.innerHTML = '<p class="text-muted">Keine Disponenten verfügbar</p>';
        return;
    }
    
    container.innerHTML = dispatchers.map(d => `
        <div class="dispatcher-card">
            <div class="dispatcher-avatar">${(d.name || d.firstName || 'U')[0]}${(d.lastName || '')[0] || ''}</div>
            <div class="dispatcher-info">
                <div class="dispatcher-name">${d.name || d.firstName + ' ' + d.lastName}</div>
                <div class="dispatcher-status">
                    <span class="status-dot ${d.isOnline ? 'online' : 'offline'}"></span>
                    ${d.isOnline ? 'Online' : 'Offline'}
                    ${d.extension ? `<span class="badge bg-secondary ms-2">${d.extension}</span>` : ''}
                </div>
            </div>
        </div>
    `).join('');
}

async function loadEmergencyDevices() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/devices`);
        const devices = await response.json();
        
        const container = document.getElementById('emergency-devices-grid');
        
        if (!devices || devices.length === 0) {
            container.innerHTML = '<div class="col-12"><p class="text-muted">Keine Notrufgeräte registriert</p></div>';
            return;
        }
        
        container.innerHTML = devices.map(d => `
            <div class="col-md-4 col-lg-3 mb-4">
                <div class="card h-100">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-start mb-3">
                            <i class="bi ${getDeviceIcon(d.deviceType)}" style="font-size: 2rem; color: var(--accent-color);"></i>
                            <span class="badge ${d.status === 'Online' ? 'bg-success' : 'bg-secondary'}">${d.status || 'Unbekannt'}</span>
                        </div>
                        <h6>${d.deviceType || 'Gerät'}</h6>
                        <p class="text-muted small mb-2">${d.serialNumber || '-'}</p>
                        <p class="small mb-1"><strong>Klient:</strong> ${d.clientName || 'Nicht zugewiesen'}</p>
                        <p class="small mb-0"><strong>Batterie:</strong> ${d.batteryLevel || 0}%</p>
                    </div>
                </div>
            </div>
        `).join('');
        
    } catch (error) {
        console.error('Error loading emergency devices:', error);
    }
}

async function loadEmergencyContacts() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/contacts`);
        const contacts = await response.json();
        
        const tbody = document.getElementById('emergency-contacts-table');
        
        if (!contacts || contacts.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">Keine Notfallkontakte vorhanden</td></tr>';
            return;
        }
        
        tbody.innerHTML = contacts.map(c => `
            <tr>
                <td>${c.clientName || '-'}</td>
                <td>${c.name || '-'}</td>
                <td>${c.relationship || '-'}</td>
                <td><a href="tel:${c.phoneNumber}" class="text-info">${c.phoneNumber || '-'}</a></td>
                <td>${c.priority || '-'}</td>
                <td>
                    ${c.notifyViaSms ? '<i class="bi bi-chat-dots text-success me-1" title="SMS"></i>' : ''}
                    ${c.notifyViaEmail ? '<i class="bi bi-envelope text-info" title="E-Mail"></i>' : ''}
                </td>
                <td>
                    <button class="btn btn-sm btn-outline-success" onclick="callNumber('${c.phoneNumber}')">
                        <i class="bi bi-telephone"></i>
                    </button>
                </td>
            </tr>
        `).join('');
        
    } catch (error) {
        console.error('Error loading emergency contacts:', error);
    }
}

async function loadDispatchers() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/dispatchers`);
        const dispatchers = await response.json();
        
        const container = document.getElementById('dispatchers-grid');
        
        if (!dispatchers || dispatchers.length === 0) {
            container.innerHTML = '<div class="col-12"><p class="text-muted">Keine Disponenten vorhanden</p></div>';
            return;
        }
        
        container.innerHTML = dispatchers.map(d => `
            <div class="col-md-4 col-lg-3 mb-4">
                <div class="card h-100">
                    <div class="card-body text-center">
                        <div class="dispatcher-avatar mx-auto mb-3" style="width: 80px; height: 80px; font-size: 1.5rem;">
                            ${(d.name || d.firstName || 'U')[0]}${(d.lastName || '')[0] || ''}
                        </div>
                        <h6>${d.name || d.firstName + ' ' + d.lastName}</h6>
                        <p class="text-muted small mb-2">${d.email || '-'}</p>
                        <p class="small mb-2"><strong>Nebenstelle:</strong> ${d.extension || '-'}</p>
                        <span class="badge ${d.isOnline ? 'bg-success' : 'bg-secondary'}">${d.isOnline ? 'Online' : 'Offline'}</span>
                    </div>
                </div>
            </div>
        `).join('');
        
    } catch (error) {
        console.error('Error loading dispatchers:', error);
    }
}

async function loadCallHistory() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/calls`);
        const calls = await response.json();
        
        const tbody = document.getElementById('call-history-table');
        
        if (!calls || calls.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center text-muted">Keine Anrufe vorhanden</td></tr>';
            return;
        }
        
        tbody.innerHTML = calls.map(c => `
            <tr>
                <td>${c.startTime ? new Date(c.startTime).toLocaleString('de-DE') : '-'}</td>
                <td>
                    <span class="badge ${c.callDirection === 'Inbound' ? 'bg-info' : 'bg-primary'}">
                        <i class="bi bi-telephone-${c.callDirection === 'Inbound' ? 'inbound' : 'outbound'}"></i>
                        ${c.callDirection === 'Inbound' ? 'Eingehend' : 'Ausgehend'}
                    </span>
                </td>
                <td>${c.phoneNumber || '-'}</td>
                <td>${c.clientName || '-'}</td>
                <td>${c.dispatcherName || '-'}</td>
                <td>${c.duration ? formatDuration(c.duration) : '-'}</td>
                <td><span class="badge ${c.callStatus === 'Completed' ? 'bg-success' : 'bg-warning'}">${c.callStatus || '-'}</span></td>
            </tr>
        `).join('');
        
    } catch (error) {
        console.error('Error loading call history:', error);
    }
}

// ============================================
// Alert Actions
// ============================================

async function acknowledgeAlert(alertId) {
    try {
        const response = await fetch(`${API_BASE}/servicehub/alerts/${alertId}/status`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ status: 'InProgress', dispatcherId: 1 })
        });
        
        if (response.ok) {
            showToast('Alarm bestätigt', 'success');
            loadServiceHubDashboard();
        }
    } catch (error) {
        console.error('Error acknowledging alert:', error);
        showToast('Fehler beim Bestätigen', 'danger');
    }
}

async function notifyContacts(alertId) {
    try {
        const response = await fetch(`${API_BASE}/servicehub/alerts/${alertId}/notify-contacts`, {
            method: 'POST'
        });
        
        const result = await response.json();
        
        if (result.success) {
            showToast(`${result.notifiedContacts} Kontakte benachrichtigt`, 'success');
        } else {
            showToast('Keine Kontakte zum Benachrichtigen', 'warning');
        }
    } catch (error) {
        console.error('Error notifying contacts:', error);
        showToast('Fehler beim Senden der SMS', 'danger');
    }
}

function callClient(phoneNumber) {
    if (phoneNumber) {
        dialedNumber = phoneNumber;
        document.getElementById('dialDisplay').textContent = phoneNumber;
        makeCall();
    }
}

function callNumber(phoneNumber) {
    if (phoneNumber) {
        dialedNumber = phoneNumber;
        document.getElementById('dialDisplay').textContent = phoneNumber;
    }
}

// ============================================
// Alert Badge & Notifications
// ============================================

function updateAlertBadge(count) {
    const badge = document.getElementById('alertBadge');
    if (badge) {
        badge.textContent = count;
        badge.style.display = count > 0 ? 'inline-block' : 'none';
    }
}

async function checkActiveAlerts() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/alerts/active`);
        const alerts = await response.json();
        updateAlertBadge(alerts.length || 0);
    } catch (error) {
        // Silently fail
    }
}

// ============================================
// SOFTPHONE / VoIP Functions
// ============================================

function initSoftphone() {
    updatePhoneStatus('offline', 'Verbinde...');
    
    // Simulate connection (in production, this would use SIP.js)
    setTimeout(() => {
        updatePhoneStatus('online', 'Bereit');
        loadRecentCalls();
    }, 2000);
}

function updatePhoneStatus(status, text) {
    const indicator = document.getElementById('phoneStatus');
    const statusText = document.getElementById('phoneStatusText');
    const header = document.getElementById('softphoneHeader');
    
    indicator.className = 'status-indicator ' + status;
    statusText.textContent = text;
    
    header.className = 'softphone-header';
    if (status === 'oncall') header.classList.add('oncall');
    if (status === 'offline') header.classList.add('offline');
}

function dialDigit(digit) {
    dialedNumber += digit;
    document.getElementById('dialDisplay').textContent = dialedNumber;
}

function makeCall() {
    if (!dialedNumber) {
        showToast('Bitte Nummer eingeben', 'warning');
        return;
    }
    
    updatePhoneStatus('oncall', 'Verbinde...');
    document.getElementById('callBtn').style.display = 'none';
    document.getElementById('hangupBtn').style.display = 'inline-block';
    
    // Start call timer
    phoneState.callStartTime = new Date();
    callTimer = setInterval(updateCallDuration, 1000);
    
    // Log the call
    logCall('Outbound', dialedNumber);
    
    setTimeout(() => {
        updatePhoneStatus('oncall', 'Im Gespräch');
    }, 2000);
}

function hangupCall() {
    updatePhoneStatus('online', 'Bereit');
    document.getElementById('callBtn').style.display = 'inline-block';
    document.getElementById('hangupBtn').style.display = 'none';
    document.getElementById('callDuration').textContent = '';
    
    if (callTimer) {
        clearInterval(callTimer);
        callTimer = null;
    }
    
    dialedNumber = '';
    document.getElementById('dialDisplay').textContent = '';
    phoneState.callStartTime = null;
    
    loadRecentCalls();
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
    updatePhoneStatus(phoneState.onHold ? 'oncall' : 'oncall', phoneState.onHold ? 'Gehalten' : 'Im Gespräch');
}

function updateCallDuration() {
    if (phoneState.callStartTime) {
        const duration = Math.floor((new Date() - phoneState.callStartTime) / 1000);
        const minutes = Math.floor(duration / 60);
        const seconds = duration % 60;
        document.getElementById('callDuration').textContent = 
            `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
    }
}

function answerCall() {
    document.getElementById('incomingCallOverlay').classList.remove('visible');
    updatePhoneStatus('oncall', 'Im Gespräch');
    document.getElementById('callBtn').style.display = 'none';
    document.getElementById('hangupBtn').style.display = 'inline-block';
    
    phoneState.callStartTime = new Date();
    callTimer = setInterval(updateCallDuration, 1000);
}

function rejectCall() {
    document.getElementById('incomingCallOverlay').classList.remove('visible');
}

async function logCall(direction, phoneNumber) {
    try {
        await fetch(`${API_BASE}/servicehub/calls`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                callDirection: direction,
                phoneNumber: phoneNumber,
                dispatcherId: 1
            })
        });
    } catch (error) {
        console.error('Error logging call:', error);
    }
}

async function loadRecentCalls() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/calls`);
        const calls = await response.json();
        
        const container = document.getElementById('recentCallsList');
        const recentCalls = (calls || []).slice(0, 5);
        
        if (recentCalls.length === 0) {
            container.innerHTML = '<p class="text-muted small">Keine Anrufe</p>';
            return;
        }
        
        container.innerHTML = recentCalls.map(c => `
            <div class="recent-call-item" onclick="callNumber('${c.phoneNumber}')">
                <i class="bi bi-telephone-${c.callDirection === 'Inbound' ? 'inbound text-info' : 'outbound text-success'}"></i>
                <span class="call-number">${c.phoneNumber || '-'}</span>
                <span class="call-time">${formatTimeAgo(c.startTime)}</span>
            </div>
        `).join('');
        
    } catch (error) {
        console.error('Error loading recent calls:', error);
    }
}

// ============================================
// Responsive Softphone
// ============================================

function handleResponsiveSoftphone() {
    const panel = document.getElementById('softphonePanel');
    const toggle = document.getElementById('softphoneToggle');
    const mainContent = document.querySelector('.main-content');
    
    if (window.innerWidth <= 1400) {
        panel.classList.add('hidden');
        toggle.classList.add('visible');
        mainContent.classList.add('softphone-hidden');
    } else {
        panel.classList.remove('hidden');
        toggle.classList.remove('visible');
        mainContent.classList.remove('softphone-hidden');
    }
}

function toggleSoftphone() {
    const panel = document.getElementById('softphonePanel');
    panel.classList.toggle('hidden');
    panel.classList.toggle('visible');
}

// ============================================
// Helper Functions
// ============================================

function getAlertIcon(type) {
    const icons = {
        'FallDetection': 'bi-person-walking',
        'ManualAlert': 'bi-hand-index',
        'InactivityAlert': 'bi-hourglass',
        'HeartRateAlert': 'bi-heart-pulse',
        'LowBattery': 'bi-battery-low',
        'DeviceOffline': 'bi-wifi-off',
        'SmokeDetected': 'bi-cloud-fog2',
        'Wandering': 'bi-geo-alt'
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
        'DeviceOffline': 'Gerät offline',
        'SmokeDetected': 'Rauch erkannt',
        'Wandering': 'Weglauf-Alarm'
    };
    return types[type] || type || 'Alarm';
}

function getStatusBadge(status) {
    const badges = {
        'New': '<span class="badge bg-primary">Neu</span>',
        'InProgress': '<span class="badge bg-warning text-dark">In Bearbeitung</span>',
        'Resolved': '<span class="badge bg-success">Abgeschlossen</span>'
    };
    return badges[status] || '';
}

function getDeviceIcon(type) {
    const icons = {
        'AppleWatch': 'bi-smartwatch',
        'HomeEmergencyButton': 'bi-house',
        'MobileEmergencyButton': 'bi-phone',
        'FallDetector': 'bi-person-walking',
        'GPSTracker': 'bi-geo-alt',
        'SmartPendant': 'bi-diamond',
        'SmokeDetector': 'bi-cloud-fog2'
    };
    return icons[type] || 'bi-device-hdd';
}

function formatTimeAgo(dateString) {
    if (!dateString) return '-';
    const date = new Date(dateString);
    const now = new Date();
    const diff = Math.floor((now - date) / 1000);
    
    if (diff < 60) return `vor ${diff} Sek.`;
    if (diff < 3600) return `vor ${Math.floor(diff / 60)} Min.`;
    if (diff < 86400) return `vor ${Math.floor(diff / 3600)} Std.`;
    return date.toLocaleDateString('de-DE');
}

function formatDuration(seconds) {
    const minutes = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${minutes}:${secs.toString().padStart(2, '0')}`;
}

// ============================================
// Toast Notifications
// ============================================

function showToast(message, type = 'info') {
    const container = document.getElementById('toastContainer');
    const toastId = 'toast-' + Date.now();
    
    const bgClass = {
        'success': 'bg-success',
        'danger': 'bg-danger',
        'warning': 'bg-warning text-dark',
        'info': 'bg-info'
    }[type] || 'bg-info';
    
    const toastHtml = `
        <div id="${toastId}" class="toast ${bgClass} text-white" role="alert">
            <div class="toast-body d-flex justify-content-between align-items-center">
                ${message}
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `;
    
    container.insertAdjacentHTML('beforeend', toastHtml);
    const toastEl = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastEl, { autohide: true, delay: 4000 });
    toast.show();
    
    toastEl.addEventListener('hidden.bs.toast', () => toastEl.remove());
}

// ============================================
// Modal Functions (Placeholders)
// ============================================

function showClientModal() {
    showToast('Klienten-Dialog wird geladen...', 'info');
}

function showDeviceModal() {
    showToast('Geräte-Dialog wird geladen...', 'info');
}

function showDirectProviderModal() {
    showToast('Provider-Dialog wird geladen...', 'info');
}

function showProfessionalProviderModal() {
    showToast('Provider-Dialog wird geladen...', 'info');
}

function showEmergencyDeviceModal() {
    showToast('Notrufgerät-Dialog wird geladen...', 'info');
}

function showEmergencyContactModal() {
    showToast('Notfallkontakt-Dialog wird geladen...', 'info');
}

function showDispatcherModal() {
    showToast('Disponenten-Dialog wird geladen...', 'info');
}

function viewClient(mandantId, clientId) {
    showToast(`Klient ${mandantId}-${clientId} wird geladen...`, 'info');
}

function editClient(mandantId, clientId) {
    showToast(`Klient ${mandantId}-${clientId} bearbeiten...`, 'info');
}

function viewDevice(mandantId, deviceId) {
    showToast(`Gerät ${mandantId}-${deviceId} wird geladen...`, 'info');
}

function editDevice(mandantId, deviceId) {
    showToast(`Gerät ${mandantId}-${deviceId} bearbeiten...`, 'info');
}
