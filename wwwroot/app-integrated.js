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
// Direct Providers
// ============================================
async function loadDirectProviders() {
    try {
        const response = await fetch(`${API_BASE}/directProvider`);
        const data = await response.json();
        const providers = data.items || data || [];
        
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
        const providers = data.items || data || [];
        
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
        const response = await fetch(`${API_BASE}/servicehub/dashboard`);
        const data = await response.json();
        
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
        showToast('Fehler beim Laden der Notruf-Leitstelle', 'danger');
    }
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
                <button class="btn btn-sm btn-success" onclick="callNumber('${alert.callerNumber}')">
                    <i class="bi bi-telephone"></i> Anrufen
                </button>
                <button class="btn btn-sm btn-primary" onclick="acknowledgeAlert(${alert.id})">
                    <i class="bi bi-check"></i> Bestätigen
                </button>
                <button class="btn btn-sm btn-warning" onclick="notifyContacts(${alert.id})">
                    <i class="bi bi-envelope"></i> SMS senden
                </button>
                ${alert.status === 'InProgress' ? '<span class="badge bg-info ms-2">In Bearbeitung</span>' : ''}
                ${alert.status === 'New' ? '<span class="badge bg-danger ms-2">Neu</span>' : ''}
            </div>
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
// SOFTPHONE / VoIP Functions with SIP.js
// ============================================

async function initSoftphone() {
    updatePhoneStatus('offline', 'Verbinde...');
    
    // Create audio element for remote audio
    phoneState.remoteAudio = new Audio();
    phoneState.remoteAudio.autoplay = true;
    document.body.appendChild(phoneState.remoteAudio);
    
    try {
        // Check if SIP.js is loaded
        if (typeof SIP === 'undefined') {
            console.error('SIP.js not loaded');
            updatePhoneStatus('offline', 'SIP.js fehlt');
            return;
        }
        
        // Load SIP configuration from server
        let config = sipConfig;
        try {
            const configResponse = await fetch(`${API_BASE}/servicehub/sip-config`);
            if (configResponse.ok) {
                const serverConfig = await configResponse.json();
                if (serverConfig && serverConfig.sipServer) {
                    config = {
                        server: serverConfig.sipServer,
                        wsServer: serverConfig.webSocketUrl || `wss://${serverConfig.sipServer}`,
                        username: serverConfig.sipUsername,
                        password: serverConfig.sipPassword,
                        domain: serverConfig.sipDomain || serverConfig.sipServer
                    };
                }
            }
        } catch (e) {
            console.log('Using default SIP config:', e);
        }
        
        console.log('Initializing SIP with config:', { server: config.server, username: config.username });
        
        // Create SIP User Agent
        const uri = SIP.UserAgent.makeURI(`sip:${config.username}@${config.domain}`);
        
        const transportOptions = {
            server: config.wsServer,
            traceSip: true
        };
        
        const userAgentOptions = {
            uri: uri,
            transportOptions: transportOptions,
            authorizationUsername: config.username,
            authorizationPassword: config.password,
            displayName: 'UMO Leitstelle',
            sessionDescriptionHandlerFactoryOptions: {
                constraints: {
                    audio: true,
                    video: false
                },
                peerConnectionConfiguration: {
                    iceServers: [
                        { urls: 'stun:stun.l.google.com:19302' },
                        { urls: 'stun:stun1.l.google.com:19302' }
                    ]
                }
            },
            delegate: {
                onInvite: (invitation) => {
                    handleIncomingCall(invitation);
                }
            }
        };
        
        phoneState.userAgent = new SIP.UserAgent(userAgentOptions);
        
        // Start the User Agent
        await phoneState.userAgent.start();
        console.log('SIP User Agent started');
        
        // Create Registerer
        phoneState.registerer = new SIP.Registerer(phoneState.userAgent);
        
        // Handle registration state changes
        phoneState.registerer.stateChange.addListener((state) => {
            console.log('Registration state:', state);
            switch (state) {
                case SIP.RegistererState.Registered:
                    phoneState.registered = true;
                    updatePhoneStatus('online', 'Bereit');
                    showToast('Telefon verbunden', 'success');
                    break;
                case SIP.RegistererState.Unregistered:
                    phoneState.registered = false;
                    updatePhoneStatus('offline', 'Nicht registriert');
                    break;
                case SIP.RegistererState.Terminated:
                    phoneState.registered = false;
                    updatePhoneStatus('offline', 'Verbindung beendet');
                    break;
            }
        });
        
        // Register
        await phoneState.registerer.register();
        
        // Load recent calls
        loadRecentCalls();
        
    } catch (error) {
        console.error('Error initializing softphone:', error);
        updatePhoneStatus('offline', 'Verbindungsfehler');
        showToast('Telefon-Verbindung fehlgeschlagen: ' + error.message, 'danger');
    }
}

function handleIncomingCall(invitation) {
    console.log('Incoming call from:', invitation.remoteIdentity.uri.user);
    
    const callerNumber = invitation.remoteIdentity.uri.user || 'Unbekannt';
    const callerDisplay = invitation.remoteIdentity.displayName || callerNumber;
    
    // Show incoming call overlay
    document.getElementById('incomingCallerNumber').textContent = callerDisplay;
    document.getElementById('incomingCallOverlay').classList.add('visible');
    
    // Store the invitation for answering
    phoneState.currentSession = invitation;
    
    // Play ringtone (optional)
    // playRingtone();
    
    // Set up invitation state change handler
    invitation.stateChange.addListener((state) => {
        console.log('Invitation state:', state);
        switch (state) {
            case SIP.SessionState.Terminated:
                document.getElementById('incomingCallOverlay').classList.remove('visible');
                if (phoneState.inCall) {
                    endCallCleanup();
                }
                break;
        }
    });
    
    // Log incoming call
    logCall('Inbound', callerNumber);
}

async function answerCall() {
    if (!phoneState.currentSession) {
        console.error('No incoming call to answer');
        return;
    }
    
    try {
        document.getElementById('incomingCallOverlay').classList.remove('visible');
        
        const options = {
            sessionDescriptionHandlerOptions: {
                constraints: {
                    audio: true,
                    video: false
                }
            }
        };
        
        await phoneState.currentSession.accept(options);
        
        phoneState.inCall = true;
        phoneState.callStartTime = new Date();
        
        updatePhoneStatus('oncall', 'Im Gespräch');
        document.getElementById('callBtn').style.display = 'none';
        document.getElementById('hangupBtn').style.display = 'inline-block';
        
        callTimer = setInterval(updateCallDuration, 1000);
        
        // Setup audio
        setupSessionAudio(phoneState.currentSession);
        
    } catch (error) {
        console.error('Error answering call:', error);
        showToast('Fehler beim Annehmen des Anrufs', 'danger');
    }
}

function rejectCall() {
    if (phoneState.currentSession) {
        phoneState.currentSession.reject();
    }
    document.getElementById('incomingCallOverlay').classList.remove('visible');
    phoneState.currentSession = null;
}

async function makeCall() {
    if (!dialedNumber) {
        showToast('Bitte Nummer eingeben', 'warning');
        return;
    }
    
    if (!phoneState.userAgent || !phoneState.registered) {
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
        
        const target = SIP.UserAgent.makeURI(`sip:${targetNumber}@${sipConfig.domain}`);
        
        const inviter = new SIP.Inviter(phoneState.userAgent, target, {
            sessionDescriptionHandlerOptions: {
                constraints: {
                    audio: true,
                    video: false
                }
            }
        });
        
        phoneState.currentSession = inviter;
        
        // Handle session state changes
        inviter.stateChange.addListener((state) => {
            console.log('Outgoing call state:', state);
            switch (state) {
                case SIP.SessionState.Establishing:
                    updatePhoneStatus('oncall', 'Verbinde...');
                    break;
                case SIP.SessionState.Established:
                    updatePhoneStatus('oncall', 'Im Gespräch');
                    phoneState.inCall = true;
                    phoneState.callStartTime = new Date();
                    callTimer = setInterval(updateCallDuration, 1000);
                    setupSessionAudio(inviter);
                    break;
                case SIP.SessionState.Terminated:
                    endCallCleanup();
                    break;
            }
        });
        
        await inviter.invite();
        
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

function setupSessionAudio(session) {
    const sessionDescriptionHandler = session.sessionDescriptionHandler;
    if (sessionDescriptionHandler && sessionDescriptionHandler.peerConnection) {
        const pc = sessionDescriptionHandler.peerConnection;
        
        pc.ontrack = (event) => {
            console.log('Remote track received');
            if (event.streams && event.streams[0]) {
                phoneState.remoteAudio.srcObject = event.streams[0];
            }
        };
    }
}

function hangupCall() {
    if (phoneState.currentSession) {
        try {
            if (phoneState.currentSession.state === SIP.SessionState.Established) {
                phoneState.currentSession.bye();
            } else {
                phoneState.currentSession.cancel();
            }
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
    if (!phoneState.currentSession || !phoneState.inCall) return;
    
    phoneState.muted = !phoneState.muted;
    const btn = document.getElementById('muteBtn');
    btn.innerHTML = phoneState.muted ? '<i class="bi bi-mic-mute"></i>' : '<i class="bi bi-mic"></i>';
    btn.classList.toggle('active', phoneState.muted);
    
    // Mute/unmute the audio track
    try {
        const sessionDescriptionHandler = phoneState.currentSession.sessionDescriptionHandler;
        if (sessionDescriptionHandler && sessionDescriptionHandler.peerConnection) {
            const senders = sessionDescriptionHandler.peerConnection.getSenders();
            senders.forEach(sender => {
                if (sender.track && sender.track.kind === 'audio') {
                    sender.track.enabled = !phoneState.muted;
                }
            });
        }
    } catch (error) {
        console.error('Error toggling mute:', error);
    }
}

function toggleHold() {
    if (!phoneState.currentSession || !phoneState.inCall) return;
    
    phoneState.onHold = !phoneState.onHold;
    const btn = document.getElementById('holdBtn');
    btn.classList.toggle('active', phoneState.onHold);
    updatePhoneStatus('oncall', phoneState.onHold ? 'Gehalten' : 'Im Gespräch');
    
    // Hold/unhold the session
    try {
        if (phoneState.onHold) {
            phoneState.currentSession.hold();
        } else {
            phoneState.currentSession.unhold();
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
