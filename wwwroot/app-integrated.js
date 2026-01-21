// UMO System - Integrierte Anwendung mit Service Hub VoIP
// Kombiniert Stammdatenverwaltung und Notruf-Leitstelle

const API_BASE = '';
let currentSection = 'dashboard';

// ============================================
// SIP/WebRTC Configuration for VoIP
// ============================================
const sipConfig = {
    server: 'sipgate.de',
    wsServer: 'wss://sip.sipgate.de',
    username: '3938564e0',
    password: 'ihFauejmdjkb',
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
    userAgent: null,
    registerer: null,
    remoteAudio: null
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
    
    // Initialize softphone with real SIP.js
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
        
        document.getElementById('total-clients').textContent = clients.count || clients.totalCount || (clients.results ? clients.results.length : 0) || 0;
        document.getElementById('total-devices').textContent = devices.count || devices.totalCount || (devices.results ? devices.results.length : 0) || 0;
        document.getElementById('total-direct-providers').textContent = directProviders.count || directProviders.totalCount || (directProviders.results ? directProviders.results.length : 0) || 0;
        document.getElementById('total-professional-providers').textContent = profProviders.count || profProviders.totalCount || (profProviders.results ? profProviders.results.length : 0) || 0;
        
        // Load Service Hub stats
        try {
            const shRes = await fetch(`${API_BASE}/servicehub/dashboard`);
            const shData = await shRes.json();
            
            // If dashboard returns empty data, load from individual endpoints
            if (shData.activeAlerts === 0 && shData.totalDispatchers === 0) {
                console.log('Dashboard returned empty, loading from individual endpoints...');
                await loadDashboardServiceHubDataDirectly();
            } else {
                document.getElementById('active-alerts').textContent = shData.activeAlerts || 0;
                document.getElementById('active-calls').textContent = shData.activeCalls || 0;
                document.getElementById('online-dispatchers').textContent = shData.onlineDispatchers || 0;
                document.getElementById('online-emergency-devices').textContent = shData.onlineDevices || 0;
                
                // Update alert badge
                updateAlertBadge(shData.activeAlerts || 0);
                
                // Load recent alerts for dashboard
                loadDashboardAlerts(shData.recentAlerts || []);
            }
        } catch (e) {
            console.log('Service Hub not available, trying direct load:', e);
            await loadDashboardServiceHubDataDirectly();
        }
        
        // Load recent clients
        loadRecentClients(clients.results || clients.items || clients || []);
        
    } catch (error) {
        console.error('Error loading dashboard:', error);
        showToast('Fehler beim Laden des Dashboards', 'danger');
    }
}

async function loadDashboardServiceHubDataDirectly() {
    try {
        // Load alerts directly
        const alertsResponse = await fetch(`${API_BASE}/servicehub/alerts`);
        const alerts = await alertsResponse.json();
        const activeAlerts = alerts.filter(a => a.status !== 'Resolved' && a.status !== 'Cancelled');
        
        // Load dispatchers directly
        const dispatchersResponse = await fetch(`${API_BASE}/servicehub/dispatchers`);
        const dispatchers = await dispatchersResponse.json();
        const onlineDispatchers = dispatchers.filter(d => d.status === 'Online' || d.status === 'OnCall');
        
        // Load devices directly
        const devicesResponse = await fetch(`${API_BASE}/servicehub/devices`);
        const devices = await devicesResponse.json();
        const onlineDevices = devices.filter(d => d.isOnline);
        
        // Update stats
        document.getElementById('active-alerts').textContent = activeAlerts.length;
        document.getElementById('active-calls').textContent = 0;
        document.getElementById('online-dispatchers').textContent = onlineDispatchers.length;
        document.getElementById('online-emergency-devices').textContent = onlineDevices.length;
        
        // Update alert badge
        updateAlertBadge(activeAlerts.length);
        
        // Load recent alerts for dashboard
        loadDashboardAlerts(activeAlerts);
        
    } catch (error) {
        console.error('Error loading service hub data directly:', error);
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
                        <br><small class="text-muted">Nr: ${c.nummer || c.id}</small>
                    </div>
                    <span class="badge ${c.status === 'Aktiv' ? 'bg-success' : 'bg-secondary'}">${c.status || 'Unbekannt'}</span>
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
        const clients = data.results || data.items || data || [];
        
        const tbody = document.getElementById('clients-table-body');
        tbody.innerHTML = clients.map(c => `
            <tr>
                <td>${c.mandantId || 1}-${c.nummer || c.clientId || c.id}</td>
                <td>${c.firstName || ''} ${c.lastName || ''}</td>
                <td>${c.birthDay ? new Date(c.birthDay).toLocaleDateString('de-DE') : '-'}</td>
                <td><span class="status-badge ${c.status === 'Aktiv' ? 'status-active' : 'status-inactive'}">${c.status || 'Unbekannt'}</span></td>
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
        const devices = data.results || data.items || data || [];
        
        const tbody = document.getElementById('devices-table-body');
        tbody.innerHTML = devices.map(d => `
            <tr>
                <td>${d.mandantId || 1}-${d.deviceId || d.id}</td>
                <td>${d.serialNumber || '-'}</td>
                <td>${d.deviceType || '-'}</td>
                <td><span class="status-badge ${d.status === 'Aktiv' ? 'status-active' : 'status-inactive'}">${d.status || 'Unbekannt'}</span></td>
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
// Direct Providers
// ============================================
async function loadDirectProviders() {
    try {
        const response = await fetch(`${API_BASE}/directProvider`);
        const data = await response.json();
        const providers = data.results || data.items || data || [];
        
        const tbody = document.getElementById('direct-providers-table-body');
        tbody.innerHTML = providers.map(p => `
            <tr>
                <td>${p.id}</td>
                <td>${p.name || ''}</td>
                <td>${p.type || '-'}</td>
                <td>${p.phone || '-'}</td>
                <td><span class="status-badge ${p.isActive ? 'status-active' : 'status-inactive'}">${p.isActive ? 'Aktiv' : 'Inaktiv'}</span></td>
                <td>
                    <button class="btn btn-sm btn-outline-primary btn-action" onclick="viewDirectProvider(${p.id})">
                        <i class="bi bi-eye"></i>
                    </button>
                </td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Error loading direct providers:', error);
        showToast('Fehler beim Laden der Direct Provider', 'danger');
    }
}

// ============================================
// Professional Providers
// ============================================
async function loadProfessionalProviders() {
    try {
        const response = await fetch(`${API_BASE}/professionalProvider`);
        const data = await response.json();
        const providers = data.results || data.items || data || [];
        
        const tbody = document.getElementById('professional-providers-table-body');
        tbody.innerHTML = providers.map(p => `
            <tr>
                <td>${p.id}</td>
                <td>${p.name || ''}</td>
                <td>${p.specialty || '-'}</td>
                <td>${p.phone || '-'}</td>
                <td><span class="status-badge ${p.status === 'Aktiv' ? 'status-active' : 'status-inactive'}">${p.status || 'Unbekannt'}</span></td>
                <td>
                    <button class="btn btn-sm btn-outline-primary btn-action" onclick="viewProfessionalProvider(${p.id})">
                        <i class="bi bi-eye"></i>
                    </button>
                </td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Error loading professional providers:', error);
        showToast('Fehler beim Laden der Professional Provider', 'danger');
    }
}

// ============================================
// System Entries
// ============================================
async function loadSystemEntries() {
    try {
        const response = await fetch(`${API_BASE}/systementries`);
        const data = await response.json();
        const entries = data.items || data || [];
        
        const tbody = document.getElementById('system-entries-table-body');
        tbody.innerHTML = entries.map(e => `
            <tr>
                <td>${e.id}</td>
                <td>${e.type || '-'}</td>
                <td>${e.code || '-'}</td>
                <td>${e.description || '-'}</td>
                <td><span class="status-badge ${e.isActive ? 'status-active' : 'status-inactive'}">${e.isActive ? 'Aktiv' : 'Inaktiv'}</span></td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Error loading system entries:', error);
        showToast('Fehler beim Laden der Systemdaten', 'danger');
    }
}

// ============================================
// Service Hub Dashboard
// ============================================
async function loadServiceHubDashboard() {
    try {
        // Try dashboard endpoint first
        const response = await fetch(`${API_BASE}/servicehub/dashboard`);
        const data = await response.json();
        
        // If dashboard returns empty data, load from individual endpoints
        if (data.activeAlerts === 0 && data.totalDispatchers === 0) {
            console.log('Dashboard returned empty, loading from individual endpoints...');
            await loadServiceHubDataDirectly();
            return;
        }
        
        // Update stats
        document.getElementById('sh-active-alerts').textContent = data.activeAlerts || 0;
        document.getElementById('sh-active-calls').textContent = data.activeCalls || 0;
        document.getElementById('sh-online-dispatchers').textContent = data.onlineDispatchers || 0;
        document.getElementById('sh-online-devices').textContent = data.onlineDevices || 0;
        
        // Load alerts
        loadServiceHubAlerts(data.recentAlerts || []);
        
        // Load dispatchers
        loadOnlineDispatchers();
        
    } catch (error) {
        console.error('Error loading service hub dashboard:', error);
        // Fallback to direct loading
        await loadServiceHubDataDirectly();
    }
}

async function loadServiceHubDataDirectly() {
    try {
        // Load alerts directly
        const alertsResponse = await fetch(`${API_BASE}/servicehub/alerts`);
        const alerts = await alertsResponse.json();
        const activeAlerts = alerts.filter(a => a.status !== 'Resolved' && a.status !== 'Cancelled');
        
        // Load dispatchers directly
        const dispatchersResponse = await fetch(`${API_BASE}/servicehub/dispatchers`);
        const dispatchers = await dispatchersResponse.json();
        const onlineDispatchers = dispatchers.filter(d => d.status === 'Online' || d.status === 'OnCall');
        
        // Load devices directly
        const devicesResponse = await fetch(`${API_BASE}/servicehub/devices`);
        const devices = await devicesResponse.json();
        const onlineDevices = devices.filter(d => d.isOnline);
        
        // Update stats
        document.getElementById('sh-active-alerts').textContent = activeAlerts.length;
        document.getElementById('sh-active-calls').textContent = 0; // TODO: Load from calls endpoint
        document.getElementById('sh-online-dispatchers').textContent = onlineDispatchers.length;
        document.getElementById('sh-online-devices').textContent = onlineDevices.length;
        
        // Load alerts into UI
        loadServiceHubAlerts(activeAlerts);
        
        // Load dispatchers into UI
        loadOnlineDispatchersList(dispatchers);
        
    } catch (error) {
        console.error('Error loading service hub data directly:', error);
        showToast('Fehler beim Laden der Notruf-Leitstelle', 'danger');
    }
}

function loadOnlineDispatchersList(dispatchers) {
    const container = document.getElementById('online-dispatchers');
    if (!container) return;
    
    if (!dispatchers || dispatchers.length === 0) {
        container.innerHTML = '<div class="text-center py-3">Keine Disponenten verfügbar</div>';
        return;
    }
    
    container.innerHTML = dispatchers.map(d => `
        <div class="dispatcher-item ${d.status === 'Online' ? 'online' : d.status === 'Break' ? 'break' : 'offline'}">
            <div class="dispatcher-avatar">${d.firstName?.charAt(0) || ''}${d.lastName?.charAt(0) || ''}</div>
            <div class="dispatcher-info">
                <div class="dispatcher-name">${d.fullName || d.firstName + ' ' + d.lastName}</div>
                <div class="dispatcher-status">${d.status} - Ext. ${d.extension}</div>
            </div>
        </div>
    `).join('');
}

function loadServiceHubAlerts(alerts) {
    const container = document.getElementById('service-hub-alerts');
    
    if (!alerts || alerts.length === 0) {
        container.innerHTML = `
            <div class="text-center py-4">
                <i class="bi bi-check-circle text-success" style="font-size: 3rem;"></i>
                <p class="mt-2 mb-0">Keine aktiven Alarme</p>
            </div>
        `;
        return;
    }
    
    container.innerHTML = alerts.map(alert => `
        <div class="sh-alert-card ${(alert.priority || 'medium').toLowerCase()}">
            <div class="sh-alert-header">
                <div class="sh-alert-type">
                    <i class="bi ${getAlertIcon(alert.alertType)}"></i>
                    <span>${formatAlertType(alert.alertType)}</span>
                </div>
                <span class="sh-alert-badge ${(alert.priority || 'medium').toLowerCase()}">${alert.priority || 'Medium'}</span>
            </div>
            <div class="sh-alert-client">${alert.clientName || 'Unbekannt'}</div>
            <div class="sh-alert-info">
                <span><i class="bi bi-telephone"></i> ${alert.callerNumber || '-'}</span>
                <span><i class="bi bi-clock"></i> ${formatTimeAgo(alert.alertTime)}</span>
                ${alert.heartRate ? `<span><i class="bi bi-heart-pulse"></i> ${alert.heartRate} BPM</span>` : ''}
            </div>
            <div class="sh-alert-actions">
                <button class="btn btn-sm btn-danger" onclick="openEmergencyChain(${alert.id})" title="Notfallkette starten">
                    <i class="bi bi-exclamation-triangle"></i> Notfallkette
                </button>
                <button class="btn btn-sm btn-success" onclick="callNumber('${alert.callerNumber}')">
                    <i class="bi bi-telephone"></i> Anrufen
                </button>
                <button class="btn btn-sm btn-primary" onclick="acknowledgeAlert(${alert.id})">
                    <i class="bi bi-check"></i> Bestätigen
                </button>
                <button class="btn btn-sm btn-warning" onclick="notifyContacts(${alert.id})">
                    <i class="bi bi-envelope"></i> SMS
                </button>
                ${alert.status === 'InProgress' ? '<span class="badge bg-info ms-2">In Bearbeitung</span>' : ''}
                ${alert.status === 'New' ? '<span class="badge bg-danger ms-2">Neu</span>' : ''}
        </div>
    `).join('');
}

async function loadOnlineDispatchers() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/dispatchers`);
        const dispatchers = await response.json();
        
        const container = document.getElementById('online-dispatchers-list');
        const onlineDispatchers = dispatchers.filter(d => d.status === 'Online' || d.isAvailable);
        
        if (onlineDispatchers.length === 0) {
            container.innerHTML = '<p class="text-muted">Keine Disponenten online</p>';
            return;
        }
        
        container.innerHTML = onlineDispatchers.map(d => `
            <div class="dispatcher-item">
                <div class="dispatcher-avatar">${d.firstName?.charAt(0) || '?'}${d.lastName?.charAt(0) || ''}</div>
                <div class="dispatcher-info">
                    <div class="dispatcher-name">${d.firstName || ''} ${d.lastName || ''}</div>
                    <div class="dispatcher-ext">Ext: ${d.extension || '-'}</div>
                </div>
                <span class="dispatcher-status ${d.status?.toLowerCase() || 'offline'}">${d.status || 'Offline'}</span>
            </div>
        `).join('');
    } catch (error) {
        console.error('Error loading dispatchers:', error);
    }
}

async function acknowledgeAlert(alertId) {
    try {
        await fetch(`${API_BASE}/servicehub/alerts/${alertId}/acknowledge`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ dispatcherId: 1 })
        });
        showToast('Alarm bestätigt', 'success');
        loadServiceHubDashboard();
        loadDashboard();
    } catch (error) {
        console.error('Error acknowledging alert:', error);
        showToast('Fehler beim Bestätigen des Alarms', 'danger');
    }
}

async function notifyContacts(alertId) {
    try {
        await fetch(`${API_BASE}/servicehub/alerts/${alertId}/notify-contacts`, {
            method: 'POST'
        });
        showToast('Notfallkontakte benachrichtigt', 'success');
    } catch (error) {
        console.error('Error notifying contacts:', error);
        showToast('Fehler beim Benachrichtigen der Kontakte', 'danger');
    }
}

// ============================================
// Emergency Devices
// ============================================
async function loadEmergencyDevices() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/devices`);
        const devices = await response.json();
        
        const tbody = document.getElementById('emergency-devices-table-body');
        tbody.innerHTML = devices.map(d => `
            <tr>
                <td>${d.id}</td>
                <td>${d.deviceName || '-'}</td>
                <td>${formatDeviceType(d.deviceType)}</td>
                <td>${d.phoneNumber || '-'}</td>
                <td>
                    <span class="status-indicator ${d.isOnline ? 'online' : 'offline'}"></span>
                    ${d.isOnline ? 'Online' : 'Offline'}
                </td>
                <td>${d.batteryLevel || 0}%</td>
                <td>
                    <button class="btn btn-sm btn-outline-success" onclick="callNumber('${d.phoneNumber}')" ${!d.phoneNumber ? 'disabled' : ''}>
                        <i class="bi bi-telephone"></i>
                    </button>
                </td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Error loading emergency devices:', error);
        showToast('Fehler beim Laden der Notrufgeräte', 'danger');
    }
}

// ============================================
// Emergency Contacts
// ============================================
async function loadEmergencyContacts() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/contacts`);
        const contacts = await response.json();
        
        const tbody = document.getElementById('emergency-contacts-table-body');
        tbody.innerHTML = contacts.map(c => `
            <tr>
                <td>${c.id}</td>
                <td>${c.firstName || ''} ${c.lastName || ''}</td>
                <td>${c.relationship || '-'}</td>
                <td>${c.phoneNumber || '-'}</td>
                <td>${c.mobileNumber || '-'}</td>
                <td>
                    <span class="badge ${c.isAvailable24h ? 'bg-success' : 'bg-secondary'}">${c.isAvailable24h ? '24h' : 'Eingeschränkt'}</span>
                    ${c.hasKey ? '<span class="badge bg-info ms-1">Schlüssel</span>' : ''}
                </td>
                <td>
                    <button class="btn btn-sm btn-outline-success" onclick="callNumber('${c.mobileNumber || c.phoneNumber}')" ${!c.mobileNumber && !c.phoneNumber ? 'disabled' : ''}>
                        <i class="bi bi-telephone"></i>
                    </button>
                </td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Error loading emergency contacts:', error);
        showToast('Fehler beim Laden der Notfallkontakte', 'danger');
    }
}

// ============================================
// Dispatchers
// ============================================
async function loadDispatchers() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/dispatchers`);
        const dispatchers = await response.json();
        
        const tbody = document.getElementById('dispatchers-table-body');
        tbody.innerHTML = dispatchers.map(d => `
            <tr>
                <td>${d.id}</td>
                <td>${d.firstName || ''} ${d.lastName || ''}</td>
                <td>${d.username || '-'}</td>
                <td>${d.extension || '-'}</td>
                <td>
                    <span class="status-indicator ${d.status?.toLowerCase() || 'offline'}"></span>
                    ${d.status || 'Offline'}
                </td>
                <td>${d.totalCallsHandled || 0}</td>
                <td>${d.role || '-'}</td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Error loading dispatchers:', error);
        showToast('Fehler beim Laden der Disponenten', 'danger');
    }
}

// ============================================
// Call History
// ============================================
async function loadCallHistory() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/calls`);
        const calls = await response.json();
        
        const tbody = document.getElementById('call-history-table-body');
        tbody.innerHTML = calls.map(c => `
            <tr>
                <td>${c.id}</td>
                <td>
                    <i class="bi bi-telephone-${c.direction === 'Inbound' ? 'inbound text-info' : 'outbound text-success'}"></i>
                    ${c.direction === 'Inbound' ? 'Eingehend' : 'Ausgehend'}
                </td>
                <td>${c.callerNumber || '-'}</td>
                <td>${c.calleeNumber || '-'}</td>
                <td>${c.startTime ? new Date(c.startTime).toLocaleString('de-DE') : '-'}</td>
                <td>${formatDuration(c.durationSeconds)}</td>
                <td><span class="badge ${c.status === 'Ended' ? 'bg-secondary' : 'bg-success'}">${c.status || '-'}</span></td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Error loading call history:', error);
        showToast('Fehler beim Laden des Anrufprotokolls', 'danger');
    }
}

// ============================================
// Alert Badge
// ============================================
function updateAlertBadge(count) {
    const badge = document.getElementById('alert-badge');
    if (badge) {
        badge.textContent = count;
        badge.style.display = count > 0 ? 'inline-block' : 'none';
    }
}

async function checkActiveAlerts() {
    try {
        const response = await fetch(`${API_BASE}/servicehub/dashboard`);
        const data = await response.json();
        updateAlertBadge(data.activeAlerts || 0);
    } catch (error) {
        console.log('Could not check alerts:', error);
    }
}

// ============================================
// SOFTPHONE / VoIP Functions with SIP.js Simple
// ============================================

async function initSoftphone() {
    updatePhoneStatus('offline', 'Verbinde...');
    
    // Create audio elements for remote audio
    phoneState.remoteAudio = document.createElement('audio');
    phoneState.remoteAudio.id = 'remoteAudio';
    phoneState.remoteAudio.autoplay = true;
    document.body.appendChild(phoneState.remoteAudio);
    
    try {
        // Check if SIP.js is loaded
        if (typeof SIP === 'undefined') {
            console.error('SIP.js not loaded - window.SIP:', window.SIP);
            updatePhoneStatus('offline', 'SIP.js fehlt');
            return;
        }
        
        console.log('SIP.js loaded successfully');
        console.log('SIP object keys:', Object.keys(SIP));
        console.log('SIP.Web:', SIP.Web);
        console.log('SIP.Web.Simple:', SIP.Web && SIP.Web.Simple);
        
        // Use hardcoded SIP configuration for Sipgate
        const config = {
            server: 'sipgate.de',
            wsServer: 'wss://sip.sipgate.de',
            username: '3938564e0',
            password: 'ihFauejmdjkb',
            domain: 'sipgate.de'
        };
        
        console.log('Initializing SIP.js Simple with config:', { server: config.server, username: config.username, wsServer: config.wsServer });
        
        // Check if SIP.Web.Simple exists
        if (!SIP.Web || !SIP.Web.Simple) {
            console.error('SIP.Web.Simple not available');
            updatePhoneStatus('offline', 'SIP.js Simple fehlt');
            return;
        }
        
        // Use SIP.js Simple API for easier WebRTC calls
        const simpleOptions = {
            media: {
                remote: {
                    audio: phoneState.remoteAudio
                }
            },
            ua: {
                uri: `sip:${config.username}@${config.domain}`,
                wsServers: [config.wsServer],
                traceSip: true,
                authorizationUser: config.username,
                password: config.password,
                displayName: 'UMO Leitstelle',
                register: true,
                log: {
                    level: 'debug'
                }
            }
        };
        
        console.log('Creating SIP.Web.Simple with URI:', simpleOptions.ua.uri);
        phoneState.simple = new SIP.Web.Simple(simpleOptions);
        
        // Handle registration events
        phoneState.simple.on('registered', () => {
            console.log('SIP Registered');
            phoneState.registered = true;
            updatePhoneStatus('online', 'Bereit');
            showToast('Telefon verbunden', 'success');
        });
        
        phoneState.simple.on('unregistered', () => {
            console.log('SIP Unregistered');
            phoneState.registered = false;
            updatePhoneStatus('offline', 'Nicht registriert');
        });
        
        phoneState.simple.on('registrationFailed', (cause) => {
            console.error('SIP Registration failed:', cause);
            phoneState.registered = false;
            updatePhoneStatus('offline', 'Registrierung fehlgeschlagen');
            showToast('Telefon-Registrierung fehlgeschlagen: ' + cause, 'danger');
        });
        
        // Handle incoming calls
        phoneState.simple.on('ringing', () => {
            console.log('Incoming call ringing');
            handleIncomingCallSimple();
        });
        
        // Handle call connected
        phoneState.simple.on('connected', () => {
            console.log('Call connected');
            phoneState.inCall = true;
            phoneState.callStartTime = new Date();
            updatePhoneStatus('oncall', 'Im Gespräch');
            document.getElementById('callBtn').style.display = 'none';
            document.getElementById('hangupBtn').style.display = 'inline-block';
            callTimer = setInterval(updateCallDuration, 1000);
        });
        
        // Handle call ended
        phoneState.simple.on('ended', () => {
            console.log('Call ended');
            endCallCleanup();
        });
        
        console.log('SIP.js Simple initialized');
        
        // Load recent calls
        loadRecentCalls();
        
    } catch (error) {
        console.error('Error initializing softphone:', error);
        updatePhoneStatus('offline', 'Verbindungsfehler');
        showToast('Telefon-Verbindung fehlgeschlagen: ' + error.message, 'danger');
    }
}

function handleIncomingCallSimple() {
    // Show incoming call overlay
    document.getElementById('incomingCallerNumber').textContent = 'Eingehender Anruf';
    document.getElementById('incomingCallOverlay').classList.add('visible');
    
    // Log incoming call
    logCall('Inbound', 'Unbekannt');
}

async function answerCall() {
    if (!phoneState.simple) {
        console.error('No softphone instance');
        return;
    }
    
    try {
        document.getElementById('incomingCallOverlay').classList.remove('visible');
        phoneState.simple.answer();
    } catch (error) {
        console.error('Error answering call:', error);
        showToast('Fehler beim Annehmen des Anrufs', 'danger');
    }
}

function rejectCall() {
    if (phoneState.simple) {
        phoneState.simple.reject();
    }
    document.getElementById('incomingCallOverlay').classList.remove('visible');
}

async function makeCall() {
    if (!dialedNumber) {
        showToast('Bitte Nummer eingeben', 'warning');
        return;
    }
    
    if (!phoneState.simple || !phoneState.registered) {
        showToast('Telefon nicht verbunden', 'danger');
        return;
    }
    
    try {
        updatePhoneStatus('oncall', 'Wähle...');
        
        // Format number for SIP
        let targetNumber = dialedNumber.replace(/[^0-9+]/g, '');
        if (!targetNumber.startsWith('+') && !targetNumber.startsWith('00')) {
            // Add German country code if not present
            if (targetNumber.startsWith('0')) {
                targetNumber = '+49' + targetNumber.substring(1);
            }
        }
        
        // Use SIP.js Simple call method
        phoneState.simple.call(`sip:${targetNumber}@${sipConfig.domain}`);
        
        document.getElementById('callBtn').style.display = 'none';
        document.getElementById('hangupBtn').style.display = 'inline-block';
        
        // Log the call
        logCall('Outbound', targetNumber);
        
    } catch (error) {
        console.error('Error making call:', error);
        showToast('Fehler beim Anrufen: ' + error.message, 'danger');
        updatePhoneStatus('online', 'Bereit');
    }
}

function hangupCall() {
    if (phoneState.simple) {
        try {
            phoneState.simple.hangup();
        } catch (error) {
            console.error('Error hanging up:', error);
        }
    }
    endCallCleanup();
}

function endCallCleanup() {
    phoneState.inCall = false;
    phoneState.currentSession = null;
    phoneState.callStartTime = null;
    phoneState.muted = false;
    phoneState.onHold = false;
    
    if (callTimer) {
        clearInterval(callTimer);
        callTimer = null;
    }
    
    updatePhoneStatus('online', 'Bereit');
    document.getElementById('callBtn').style.display = 'inline-block';
    document.getElementById('hangupBtn').style.display = 'none';
    document.getElementById('callDuration').textContent = '';
    
    dialedNumber = '';
    document.getElementById('dialDisplay').textContent = '';
    
    // Reset mute/hold buttons
    document.getElementById('muteBtn').classList.remove('active');
    document.getElementById('holdBtn').classList.remove('active');
    document.getElementById('muteBtn').innerHTML = '<i class="bi bi-mic"></i>';
    
    loadRecentCalls();
}

function updatePhoneStatus(status, text) {
    const indicator = document.getElementById('phoneStatus');
    const statusText = document.getElementById('phoneStatusText');
    const header = document.getElementById('softphoneHeader');
    
    if (indicator) indicator.className = 'status-indicator ' + status;
    if (statusText) statusText.textContent = text;
    
    if (header) {
        header.className = 'softphone-header';
        if (status === 'oncall') header.classList.add('oncall');
        if (status === 'offline') header.classList.add('offline');
    }
}

function dialDigit(digit) {
    dialedNumber += digit;
    document.getElementById('dialDisplay').textContent = dialedNumber;
}

function clearDial() {
    dialedNumber = '';
    document.getElementById('dialDisplay').textContent = '';
}

function callNumber(number) {
    if (!number) return;
    dialedNumber = number;
    document.getElementById('dialDisplay').textContent = number;
    makeCall();
}

function toggleMute() {
    if (!phoneState.simple || !phoneState.inCall) return;
    
    phoneState.muted = !phoneState.muted;
    const btn = document.getElementById('muteBtn');
    btn.innerHTML = phoneState.muted ? '<i class="bi bi-mic-mute"></i>' : '<i class="bi bi-mic"></i>';
    btn.classList.toggle('active', phoneState.muted);
    
    // Mute/unmute using SIP.js Simple
    try {
        if (phoneState.muted) {
            phoneState.simple.mute();
        } else {
            phoneState.simple.unmute();
        }
    } catch (error) {
        console.error('Error toggling mute:', error);
    }
}

function toggleHold() {
    if (!phoneState.simple || !phoneState.inCall) return;
    
    phoneState.onHold = !phoneState.onHold;
    const btn = document.getElementById('holdBtn');
    btn.classList.toggle('active', phoneState.onHold);
    updatePhoneStatus('oncall', phoneState.onHold ? 'Gehalten' : 'Im Gespräch');
    
    // Hold/unhold using SIP.js Simple
    try {
        if (phoneState.onHold) {
            phoneState.simple.hold();
        } else {
            phoneState.simple.unhold();
        }
    } catch (error) {
        console.error('Error toggling hold:', error);
    }
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

async function logCall(direction, phoneNumber) {
    try {
        await fetch(`${API_BASE}/servicehub/calls`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                direction: direction,
                callerNumber: direction === 'Inbound' ? phoneNumber : 'Leitstelle',
                calleeNumber: direction === 'Outbound' ? phoneNumber : 'Leitstelle',
                dispatcherId: 1,
                status: 'Connected',
                callType: 'Manual'
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
        if (!container) return;
        
        const recentCalls = (calls || []).slice(0, 5);
        
        if (recentCalls.length === 0) {
            container.innerHTML = '<p class="text-muted small">Keine Anrufe</p>';
            return;
        }
        
        container.innerHTML = recentCalls.map(c => `
            <div class="recent-call-item" onclick="callNumber('${c.callerNumber || c.calleeNumber}')">
                <i class="bi bi-telephone-${c.direction === 'Inbound' ? 'inbound text-info' : 'outbound text-success'}"></i>
                <span class="call-number">${c.direction === 'Inbound' ? c.callerNumber : c.calleeNumber || '-'}</span>
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
        'SmokeDetected': 'Rauchmelder',
        'Wandering': 'Weglauftendenz'
    };
    return types[type] || type;
}

function formatDeviceType(type) {
    const types = {
        'AppleWatch': 'Apple Watch',
        'Hausnotruf': 'Hausnotruf',
        'MobilNotruf': 'Mobiler Notruf',
        'Sturzmelder': 'Sturzmelder',
        'GPSTracker': 'GPS-Tracker',
        'Rauchmelder': 'Rauchmelder'
    };
    return types[type] || type;
}

function formatTimeAgo(dateString) {
    if (!dateString) return '-';
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now - date;
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);
    
    if (diffMins < 1) return 'Gerade eben';
    if (diffMins < 60) return `vor ${diffMins} Min.`;
    if (diffHours < 24) return `vor ${diffHours} Std.`;
    return `vor ${diffDays} Tag(en)`;
}

function formatDuration(seconds) {
    if (!seconds) return '-';
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, '0')}`;
}

function showToast(message, type = 'info') {
    // Create toast container if it doesn't exist
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        container.style.cssText = 'position: fixed; top: 20px; right: 20px; z-index: 9999;';
        document.body.appendChild(container);
    }
    
    const toast = document.createElement('div');
    toast.className = `alert alert-${type} alert-dismissible fade show`;
    toast.style.cssText = 'min-width: 250px; margin-bottom: 10px; box-shadow: 0 4px 15px rgba(0,0,0,0.2);';
    toast.innerHTML = `
        ${message}
        <button type="button" class="btn-close" onclick="this.parentElement.remove()"></button>
    `;
    container.appendChild(toast);
    
    setTimeout(() => {
        toast.remove();
    }, 5000);
}

// Placeholder functions for view/edit operations
function viewClient(mandantId, clientId) {
    showToast(`Klient ${mandantId}-${clientId} anzeigen`, 'info');
}

function editClient(mandantId, clientId) {
    showToast(`Klient ${mandantId}-${clientId} bearbeiten`, 'info');
}

function viewDevice(mandantId, deviceId) {
    showToast(`Gerät ${mandantId}-${deviceId} anzeigen`, 'info');
}

function editDevice(mandantId, deviceId) {
    showToast(`Gerät ${mandantId}-${deviceId} bearbeiten`, 'info');
}

function viewDirectProvider(id) {
    showToast(`Direct Provider ${id} anzeigen`, 'info');
}

function viewProfessionalProvider(id) {
    showToast(`Professional Provider ${id} anzeigen`, 'info');
}


// ============================================
// Emergency Chain / Notfallkette
// ============================================

// Öffnet das Notfallketten-Modal für einen Alarm
async function openEmergencyChain(alertId) {
    try {
        // Lade Alarm-Details und Klient-Informationen
        const response = await fetch(`${API_BASE}/emergencychain/alerts/${alertId}/client-info`);
        const clientInfo = await response.json();
        
        // Lade Notfallketten-Status
        const statusResponse = await fetch(`${API_BASE}/emergencychain/alerts/${alertId}/status`);
        const chainStatus = await statusResponse.json();
        
        // Modal erstellen und anzeigen
        showEmergencyChainModal(alertId, clientInfo, chainStatus);
        
    } catch (error) {
        console.error('Error opening emergency chain:', error);
        showToast('Fehler beim Laden der Notfallkette', 'danger');
    }
}

function showEmergencyChainModal(alertId, clientInfo, chainStatus) {
    // Entferne bestehendes Modal falls vorhanden
    const existingModal = document.getElementById('emergencyChainModal');
    if (existingModal) existingModal.remove();
    
    const modal = document.createElement('div');
    modal.id = 'emergencyChainModal';
    modal.className = 'modal fade show';
    modal.style.cssText = 'display: block; background: rgba(0,0,0,0.7);';
    
    const medicationHtml = clientInfo.medications?.length > 0 
        ? clientInfo.medications.map(m => `
            <tr>
                <td><strong>${m.name}</strong></td>
                <td>${m.dosage}</td>
                <td>${m.frequency}</td>
                <td>${m.category || '-'}</td>
            </tr>
        `).join('')
        : '<tr><td colspan="4" class="text-muted">Keine Medikamente hinterlegt</td></tr>';
    
    modal.innerHTML = `
        <div class="modal-dialog modal-xl">
            <div class="modal-content bg-dark text-light">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title">
                        <i class="bi bi-exclamation-triangle-fill me-2"></i>
                        Notfallkette - ${clientInfo.name || 'Unbekannt'}
                    </h5>
                    <button type="button" class="btn-close btn-close-white" onclick="closeEmergencyChainModal()"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <!-- Klient-Informationen -->
                        <div class="col-md-4">
                            <div class="card bg-secondary mb-3">
                                <div class="card-header">
                                    <i class="bi bi-person-fill me-2"></i>Klient-Daten
                                </div>
                                <div class="card-body">
                                    <p><strong>Name:</strong> ${clientInfo.name || '-'}</p>
                                    <p><strong>Geburtsdatum:</strong> ${clientInfo.birthDate ? new Date(clientInfo.birthDate).toLocaleDateString('de-DE') : '-'}</p>
                                    <p><strong>Adresse:</strong> ${clientInfo.address || '-'}</p>
                                    <p><strong>Telefon:</strong> ${clientInfo.phone || '-'}</p>
                                    ${clientInfo.medicalNotes ? `<p><strong>Med. Hinweise:</strong> ${clientInfo.medicalNotes}</p>` : ''}
                                    <hr>
                                    <p><strong>Aufzeichnung:</strong> 
                                        <span class="badge ${clientInfo.recordingEnabled ? 'bg-success' : 'bg-secondary'}">
                                            ${clientInfo.recordingEnabled ? 'Aktiviert' : 'Deaktiviert'}
                                        </span>
                                    </p>
                                </div>
                            </div>
                        </div>
                        
                        <!-- Medikamentenliste -->
                        <div class="col-md-4">
                            <div class="card bg-secondary mb-3">
                                <div class="card-header">
                                    <i class="bi bi-capsule me-2"></i>Medikamentenliste
                                </div>
                                <div class="card-body" style="max-height: 300px; overflow-y: auto;">
                                    <table class="table table-dark table-sm">
                                        <thead>
                                            <tr>
                                                <th>Medikament</th>
                                                <th>Dosierung</th>
                                                <th>Häufigkeit</th>
                                                <th>Kategorie</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            ${medicationHtml}
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                        
                        <!-- Notfallketten-Status -->
                        <div class="col-md-4">
                            <div class="card bg-secondary mb-3">
                                <div class="card-header">
                                    <i class="bi bi-list-check me-2"></i>Notfallketten-Status
                                </div>
                                <div class="card-body">
                                    <div class="chain-step ${chainStatus.contactsNotified ? 'completed' : ''}">
                                        <i class="bi bi-${chainStatus.contactsNotified ? 'check-circle-fill text-success' : 'circle'}"></i>
                                        Angehörige benachrichtigt
                                    </div>
                                    <div class="chain-step ${chainStatus.doctorCalled ? 'completed' : ''}">
                                        <i class="bi bi-${chainStatus.doctorCalled ? 'check-circle-fill text-success' : 'circle'}"></i>
                                        Arzt informiert
                                    </div>
                                    <div class="chain-step ${chainStatus.ambulanceCalled ? 'completed' : ''}">
                                        <i class="bi bi-${chainStatus.ambulanceCalled ? 'check-circle-fill text-success' : 'circle'}"></i>
                                        Rettungsdienst alarmiert
                                    </div>
                                    <div class="chain-step ${chainStatus.conferenceActive ? 'completed' : ''}">
                                        <i class="bi bi-${chainStatus.conferenceActive ? 'check-circle-fill text-success' : 'circle'}"></i>
                                        Konferenzschaltung aktiv
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <!-- Aktions-Buttons -->
                    <div class="row mt-3">
                        <div class="col-12">
                            <div class="card bg-dark border-light">
                                <div class="card-header">
                                    <i class="bi bi-telephone-fill me-2"></i>Notfallketten-Aktionen
                                </div>
                                <div class="card-body">
                                    <div class="d-flex flex-wrap gap-2">
                                        <button class="btn btn-warning btn-lg" onclick="notifyAllContacts(${alertId})" ${chainStatus.contactsNotified ? 'disabled' : ''}>
                                            <i class="bi bi-people-fill me-2"></i>Angehörige benachrichtigen
                                        </button>
                                        <button class="btn btn-info btn-lg" onclick="callDoctor(${alertId})" ${chainStatus.doctorCalled ? 'disabled' : ''}>
                                            <i class="bi bi-heart-pulse me-2"></i>Arzt anrufen
                                        </button>
                                        <button class="btn btn-danger btn-lg" onclick="callAmbulance(${alertId})">
                                            <i class="bi bi-truck me-2"></i>Rettungsdienst (112)
                                        </button>
                                        <button class="btn btn-primary btn-lg" onclick="startConference(${alertId})" ${chainStatus.conferenceActive ? 'disabled' : ''}>
                                            <i class="bi bi-people me-2"></i>Konferenz starten
                                        </button>
                                    </div>
                                    
                                    ${chainStatus.conferenceActive ? `
                                        <div class="mt-3 p-3 bg-secondary rounded">
                                            <h6><i class="bi bi-broadcast me-2"></i>Aktive Konferenz</h6>
                                            <p>Teilnehmer: ${chainStatus.conferenceParticipants || 'Disponent'}</p>
                                            <div class="d-flex gap-2">
                                                <button class="btn btn-outline-light" onclick="addParticipantDialog(${alertId})">
                                                    <i class="bi bi-person-plus me-2"></i>Teilnehmer hinzufügen
                                                </button>
                                                <button class="btn btn-outline-danger" onclick="endConference(${alertId})">
                                                    <i class="bi bi-x-circle me-2"></i>Konferenz beenden
                                                </button>
                                            </div>
                                        </div>
                                    ` : ''}
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <!-- Protokoll -->
                    <div class="row mt-3">
                        <div class="col-12">
                            <div class="card bg-dark border-light">
                                <div class="card-header">
                                    <i class="bi bi-journal-text me-2"></i>Aktionsprotokoll
                                </div>
                                <div class="card-body" style="max-height: 200px; overflow-y: auto;">
                                    <div id="chainActionLog">
                                        <p class="text-muted">Lade Protokoll...</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-success" onclick="resolveAlertFromChain(${alertId})">
                        <i class="bi bi-check-circle me-2"></i>Alarm abschließen
                    </button>
                    <button type="button" class="btn btn-secondary" onclick="closeEmergencyChainModal()">Schließen</button>
                </div>
            </div>
        </div>
    `;
    
    document.body.appendChild(modal);
    
    // Lade Aktionsprotokoll
    loadChainActionLog(alertId);
}

function closeEmergencyChainModal() {
    const modal = document.getElementById('emergencyChainModal');
    if (modal) modal.remove();
}

async function loadChainActionLog(alertId) {
    try {
        const response = await fetch(`${API_BASE}/emergencychain/alerts/${alertId}/actions`);
        const actions = await response.json();
        
        const container = document.getElementById('chainActionLog');
        if (actions.length === 0) {
            container.innerHTML = '<p class="text-muted">Noch keine Aktionen durchgeführt</p>';
            return;
        }
        
        container.innerHTML = actions.map(a => `
            <div class="action-log-item mb-2 p-2 bg-secondary rounded">
                <div class="d-flex justify-content-between">
                    <span class="badge bg-${getActionBadgeColor(a.actionType)}">${formatActionType(a.actionType)}</span>
                    <small>${new Date(a.timestamp).toLocaleString('de-DE')}</small>
                </div>
                <p class="mb-0 mt-1 small">${a.notes || '-'}</p>
                ${a.targetContact ? `<small class="text-muted">Kontakt: ${a.targetContact}</small>` : ''}
            </div>
        `).join('');
        
    } catch (error) {
        console.error('Error loading action log:', error);
    }
}

function getActionBadgeColor(actionType) {
    const colors = {
        'NotifyContact': 'warning',
        'CallDoctor': 'info',
        'CallAmbulance': 'danger',
        'StartConference': 'primary',
        'AddParticipant': 'secondary',
        'EndConference': 'dark',
        'ResolveAlert': 'success'
    };
    return colors[actionType] || 'secondary';
}

function formatActionType(actionType) {
    const types = {
        'NotifyContact': 'Kontakt benachrichtigt',
        'CallDoctor': 'Arzt angerufen',
        'CallAmbulance': 'Rettungsdienst',
        'StartConference': 'Konferenz gestartet',
        'AddParticipant': 'Teilnehmer hinzugefügt',
        'EndConference': 'Konferenz beendet',
        'ResolveAlert': 'Alarm abgeschlossen'
    };
    return types[actionType] || actionType;
}

// Notfallketten-Aktionen
async function notifyAllContacts(alertId) {
    try {
        showToast('Benachrichtige Angehörige...', 'info');
        
        const response = await fetch(`${API_BASE}/emergencychain/alerts/${alertId}/notify-all-contacts`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ dispatcherId: 1 })
        });
        
        const result = await response.json();
        
        if (result.success) {
            showToast(`${result.notifiedCount} Kontakte benachrichtigt`, 'success');
            // Modal aktualisieren
            const statusResponse = await fetch(`${API_BASE}/emergencychain/alerts/${alertId}/status`);
            const chainStatus = await statusResponse.json();
            loadChainActionLog(alertId);
        } else {
            showToast('Fehler beim Benachrichtigen: ' + result.message, 'danger');
        }
        
    } catch (error) {
        console.error('Error notifying contacts:', error);
        showToast('Fehler beim Benachrichtigen der Kontakte', 'danger');
    }
}

async function callDoctor(alertId) {
    try {
        showToast('Rufe Arzt an...', 'info');
        
        const response = await fetch(`${API_BASE}/emergencychain/alerts/${alertId}/call-doctor`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ dispatcherId: 1 })
        });
        
        const result = await response.json();
        
        if (result.success) {
            showToast(`Arzt ${result.doctorName} wird angerufen`, 'success');
            loadChainActionLog(alertId);
        } else {
            showToast('Fehler: ' + result.message, 'danger');
        }
        
    } catch (error) {
        console.error('Error calling doctor:', error);
        showToast('Fehler beim Anrufen des Arztes', 'danger');
    }
}

async function callAmbulance(alertId) {
    // Bestätigung anfordern
    if (!confirm('Rettungsdienst (112) alarmieren?\n\nDie Medikamentenliste wird automatisch übermittelt.')) {
        return;
    }
    
    try {
        showToast('Alarmiere Rettungsdienst...', 'info');
        
        const response = await fetch(`${API_BASE}/emergencychain/alerts/${alertId}/call-ambulance`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ 
                dispatcherId: 1,
                ambulanceNumber: '112'
            })
        });
        
        const result = await response.json();
        
        if (result.success) {
            showToast('Rettungsdienst wird alarmiert', 'success');
            
            // Zeige Medikamentenliste für Übergabe
            if (result.medications?.length > 0) {
                showMedicationHandover(result);
            }
            
            loadChainActionLog(alertId);
        } else {
            showToast('Fehler: ' + result.message, 'danger');
        }
        
    } catch (error) {
        console.error('Error calling ambulance:', error);
        showToast('Fehler beim Alarmieren des Rettungsdienstes', 'danger');
    }
}

function showMedicationHandover(ambulanceResult) {
    const medList = ambulanceResult.medications.map(m => 
        `• ${m.name} - ${m.dosage} (${m.frequency})`
    ).join('\n');
    
    alert(`MEDIKAMENTENLISTE FÜR RETTUNGSDIENST:\n\nAdresse: ${ambulanceResult.clientAddress}\n\n${ambulanceResult.medicationListText || medList}`);
}

async function startConference(alertId) {
    try {
        showToast('Starte Konferenzschaltung...', 'info');
        
        const response = await fetch(`${API_BASE}/emergencychain/alerts/${alertId}/start-conference`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ dispatcherId: 1 })
        });
        
        const result = await response.json();
        
        if (result.success) {
            showToast('Konferenzschaltung gestartet', 'success');
            // Modal neu laden
            openEmergencyChain(alertId);
        } else {
            showToast('Fehler: ' + result.message, 'danger');
        }
        
    } catch (error) {
        console.error('Error starting conference:', error);
        showToast('Fehler beim Starten der Konferenz', 'danger');
    }
}

function addParticipantDialog(alertId) {
    const name = prompt('Name des Teilnehmers:');
    if (!name) return;
    
    const phone = prompt('Telefonnummer:');
    if (!phone) return;
    
    const role = prompt('Rolle (z.B. Angehöriger, Arzt, Rettungsdienst):') || 'Teilnehmer';
    
    addConferenceParticipant(alertId, name, phone, role);
}

async function addConferenceParticipant(alertId, name, phone, role) {
    try {
        const response = await fetch(`${API_BASE}/emergencychain/alerts/${alertId}/add-participant`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ 
                participantName: name,
                phoneNumber: phone,
                role: role,
                dispatcherId: 1
            })
        });
        
        const result = await response.json();
        
        if (result.success) {
            showToast(`${name} zur Konferenz hinzugefügt`, 'success');
            openEmergencyChain(alertId);
        } else {
            showToast('Fehler: ' + result.message, 'danger');
        }
        
    } catch (error) {
        console.error('Error adding participant:', error);
        showToast('Fehler beim Hinzufügen des Teilnehmers', 'danger');
    }
}

async function endConference(alertId) {
    if (!confirm('Konferenzschaltung beenden?')) return;
    
    try {
        const response = await fetch(`${API_BASE}/emergencychain/alerts/${alertId}/end-conference`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ dispatcherId: 1 })
        });
        
        const result = await response.json();
        
        if (result.success) {
            showToast('Konferenz beendet', 'success');
            openEmergencyChain(alertId);
        }
        
    } catch (error) {
        console.error('Error ending conference:', error);
    }
}

async function resolveAlertFromChain(alertId) {
    const resolution = prompt('Abschlussnotiz (optional):') || 'Alarm abgeschlossen';
    
    try {
        await fetch(`${API_BASE}/servicehub/alerts/${alertId}/resolve`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ 
                dispatcherId: 1,
                resolution: resolution
            })
        });
        
        showToast('Alarm abgeschlossen', 'success');
        closeEmergencyChainModal();
        loadServiceHubDashboard();
        loadDashboard();
        
    } catch (error) {
        console.error('Error resolving alert:', error);
        showToast('Fehler beim Abschließen des Alarms', 'danger');
    }
}

// ============================================
// Recording Settings / Aufzeichnungseinstellungen
// ============================================

async function toggleRecording(clientId, enabled) {
    try {
        const response = await fetch(`${API_BASE}/emergencychain/clients/${clientId}/recording`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ 
                recordingEnabled: enabled,
                recordingConsent: enabled,
                consentDate: enabled ? new Date().toISOString() : null
            })
        });
        
        if (response.ok) {
            showToast(`Aufzeichnung ${enabled ? 'aktiviert' : 'deaktiviert'}`, 'success');
        } else {
            showToast('Fehler beim Ändern der Aufzeichnungseinstellung', 'danger');
        }
        
    } catch (error) {
        console.error('Error toggling recording:', error);
        showToast('Fehler beim Ändern der Aufzeichnungseinstellung', 'danger');
    }
}

// CSS für Notfallkette
const emergencyChainStyles = document.createElement('style');
emergencyChainStyles.textContent = `
    .chain-step {
        padding: 8px 12px;
        margin-bottom: 8px;
        border-radius: 4px;
        background: rgba(255,255,255,0.1);
        display: flex;
        align-items: center;
        gap: 10px;
    }
    .chain-step.completed {
        background: rgba(40, 167, 69, 0.2);
    }
    .action-log-item {
        border-left: 3px solid #6c757d;
    }
    .action-log-item .badge {
        font-size: 0.75rem;
    }
    #emergencyChainModal .modal-xl {
        max-width: 1200px;
    }
    #emergencyChainModal .card {
        height: 100%;
    }
    #emergencyChainModal .btn-lg {
        padding: 12px 20px;
        font-size: 1rem;
    }
`;
document.head.appendChild(emergencyChainStyles);
