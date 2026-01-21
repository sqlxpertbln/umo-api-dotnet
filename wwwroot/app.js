// UMO API Client Application
const API_BASE = '';
const MANDANT_ID = 1;

// State
let currentPage = 0;
const pageSize = 20;

// Initialize application
document.addEventListener('DOMContentLoaded', function() {
    initNavigation();
    loadDashboard();
    loadDropdownData();
});

// Navigation
function initNavigation() {
    document.querySelectorAll('.nav-link[data-section]').forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const section = this.dataset.section;
            
            // Update active state
            document.querySelectorAll('.nav-link').forEach(l => l.classList.remove('active'));
            this.classList.add('active');
            
            // Show section
            document.querySelectorAll('.content-section').forEach(s => s.style.display = 'none');
            document.getElementById(`${section}-section`).style.display = 'block';
            
            // Load data
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
            }
        });
    });
}

// API Helper
async function apiCall(endpoint, method = 'GET', data = null) {
    const options = {
        method,
        headers: {
            'Content-Type': 'application/json'
        }
    };
    
    if (data) {
        options.body = JSON.stringify(data);
    }
    
    try {
        showLoading(true);
        const response = await fetch(`${API_BASE}${endpoint}`, options);
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const result = await response.json();
        return result;
    } catch (error) {
        console.error('API Error:', error);
        showToast('Fehler bei der API-Anfrage: ' + error.message, 'danger');
        throw error;
    } finally {
        showLoading(false);
    }
}

// Dashboard
async function loadDashboard() {
    try {
        // Load counts
        const [clients, devices, directProviders, professionalProviders] = await Promise.all([
            apiCall('/clients?count=1'),
            apiCall('/devices?count=1'),
            apiCall('/directProvider?count=1'),
            apiCall('/professionalProvider?count=1')
        ]);
        
        document.getElementById('total-clients').textContent = clients.count || 0;
        document.getElementById('total-devices').textContent = devices.count || 0;
        document.getElementById('total-direct-providers').textContent = directProviders.count || 0;
        document.getElementById('total-professional-providers').textContent = professionalProviders.count || 0;
        
        // Load recent clients
        const recentClients = await apiCall('/clients?count=5');
        const recentList = document.getElementById('recent-clients-list');
        
        if (recentClients.results && recentClients.results.length > 0) {
            recentList.innerHTML = recentClients.results.map(c => `
                <div class="d-flex justify-content-between align-items-center py-2 border-bottom">
                    <div>
                        <strong>${c.lastName || ''}, ${c.firstName || ''}</strong>
                        <br><small class="text-muted">Nr. ${c.nummer || '-'}</small>
                    </div>
                    <span class="status-badge ${c.status === 'Aktiv' ? 'status-active' : 'status-inactive'}">${c.status || '-'}</span>
                </div>
            `).join('');
        } else {
            recentList.innerHTML = '<p class="text-muted">Keine Klienten vorhanden.</p>';
        }
    } catch (error) {
        console.error('Error loading dashboard:', error);
    }
}

// Clients
async function loadClients(page = 0) {
    currentPage = page;
    try {
        const data = await apiCall(`/clients?startIndex=${page * pageSize}&count=${pageSize}`);
        const tbody = document.getElementById('clients-table-body');
        
        if (data.results && data.results.length > 0) {
            tbody.innerHTML = data.results.map(c => `
                <tr>
                    <td>${c.nummer || '-'}</td>
                    <td>${c.lastName || ''}, ${c.firstName || ''}</td>
                    <td>${formatDate(c.birthDay)}</td>
                    <td><span class="status-badge ${c.status === 'Aktiv' ? 'status-active' : 'status-inactive'}">${c.status || '-'}</span></td>
                    <td>${formatDate(c.startContractDate)}</td>
                    <td>
                        <button class="btn btn-sm btn-outline-info btn-action" onclick="viewClientDetails(${MANDANT_ID}, ${c.id})">
                            <i class="bi bi-eye"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-primary btn-action" onclick="editClient(${c.id})">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger btn-action" onclick="deleteClient(${MANDANT_ID}, ${c.id})">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>
            `).join('');
        } else {
            tbody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">Keine Klienten gefunden.</td></tr>';
        }
        
        // Pagination
        renderPagination('clients-pagination', data.count, data.pageCount, page, loadClients);
    } catch (error) {
        console.error('Error loading clients:', error);
    }
}

function searchClients() {
    const searchTerm = document.getElementById('client-search').value.toLowerCase();
    const rows = document.querySelectorAll('#clients-table-body tr');
    
    rows.forEach(row => {
        const text = row.textContent.toLowerCase();
        row.style.display = text.includes(searchTerm) ? '' : 'none';
    });
}

async function viewClientDetails(mandantId, id) {
    try {
        const client = await apiCall(`/clientdetails?mandantId=${mandantId}&id=${id}`);
        const content = document.getElementById('clientDetailsContent');
        
        content.innerHTML = `
            <div class="row">
                <div class="col-md-6">
                    <h5 class="mb-3">Persönliche Daten</h5>
                    <div class="row mb-2">
                        <div class="col-4 detail-label">Name</div>
                        <div class="col-8 detail-value">${client.titel || ''} ${client.prefix || ''} ${client.firstName || ''} ${client.lastName || ''}</div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-4 detail-label">Geschlecht</div>
                        <div class="col-8 detail-value">${client.sex === 'M' ? 'Männlich' : client.sex === 'F' ? 'Weiblich' : 'Divers'}</div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-4 detail-label">Geburtsdatum</div>
                        <div class="col-8 detail-value">${formatDate(client.birthDay)}</div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-4 detail-label">Familienstand</div>
                        <div class="col-8 detail-value">${client.maritalStatus || '-'}</div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-4 detail-label">Sprache</div>
                        <div class="col-8 detail-value">${client.language || '-'}</div>
                    </div>
                </div>
                <div class="col-md-6">
                    <h5 class="mb-3">Vertragsdaten</h5>
                    <div class="row mb-2">
                        <div class="col-4 detail-label">Kundennummer</div>
                        <div class="col-8 detail-value">${client.nummer || '-'}</div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-4 detail-label">Status</div>
                        <div class="col-8 detail-value"><span class="status-badge ${client.status === 'Aktiv' ? 'status-active' : 'status-inactive'}">${client.status || '-'}</span></div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-4 detail-label">Vertragsbeginn</div>
                        <div class="col-8 detail-value">${formatDate(client.startContractDate)}</div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-4 detail-label">Policennummer</div>
                        <div class="col-8 detail-value">${client.policyNumber || '-'}</div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-4 detail-label">Priorität</div>
                        <div class="col-8 detail-value">${client.priority || '-'}</div>
                    </div>
                </div>
            </div>
            <hr>
            <div class="row">
                <div class="col-md-6">
                    <h5 class="mb-3">Adresse</h5>
                    ${client.address ? `
                        <p>${client.address.street || ''} ${client.address.houseNumber || ''}<br>
                        ${client.address.zipCode || ''} ${client.address.city || ''}<br>
                        ${client.address.district || ''}, ${client.address.country || ''}</p>
                    ` : '<p class="text-muted">Keine Adresse hinterlegt.</p>'}
                </div>
                <div class="col-md-6">
                    <h5 class="mb-3">Tarif</h5>
                    ${client.tarif ? `
                        <p><strong>${client.tarif.name || ''}</strong><br>
                        ${client.tarif.description || ''}<br>
                        Preis: ${client.tarif.totalPrice ? client.tarif.totalPrice.toFixed(2) + ' €' : '-'}</p>
                    ` : '<p class="text-muted">Kein Tarif zugewiesen.</p>'}
                </div>
            </div>
            ${client.phones && client.phones.length > 0 ? `
                <hr>
                <h5 class="mb-3">Telefonnummern</h5>
                <ul class="list-group">
                    ${client.phones.map(p => `
                        <li class="list-group-item d-flex justify-content-between">
                            <span>${p.phoneNumber}</span>
                            <span class="badge bg-secondary">${p.phoneType || 'Telefon'}</span>
                        </li>
                    `).join('')}
                </ul>
            ` : ''}
            ${client.note ? `
                <hr>
                <h5 class="mb-3">Notizen</h5>
                <p>${client.note}</p>
            ` : ''}
        `;
        
        new bootstrap.Modal(document.getElementById('clientDetailsModal')).show();
    } catch (error) {
        console.error('Error loading client details:', error);
    }
}

function showClientModal(client = null) {
    document.getElementById('clientModalTitle').textContent = client ? 'Klient bearbeiten' : 'Neuer Klient';
    document.getElementById('clientForm').reset();
    document.getElementById('clientId').value = client ? client.id : '';
    
    if (client) {
        document.getElementById('clientTitel').value = client.titelId || '';
        document.getElementById('clientPrefix').value = client.prefixId || '';
        document.getElementById('clientFirstName').value = client.firstName || '';
        document.getElementById('clientLastName').value = client.lastName || '';
        document.getElementById('clientSex').value = client.sex || '';
        document.getElementById('clientBirthDay').value = client.birthDay ? client.birthDay.split('T')[0] : '';
        document.getElementById('clientNummer').value = client.nummer || '';
        document.getElementById('clientStatus').value = client.statusId || '';
        document.getElementById('clientMaritalStatus').value = client.maritalStatusId || '';
        document.getElementById('clientStartContractDate').value = client.startContractDate ? client.startContractDate.split('T')[0] : '';
        document.getElementById('clientPolicyNumber').value = client.policyNumber || '';
        document.getElementById('clientLanguage').value = client.languageId || '';
        document.getElementById('clientPriority').value = client.priorityId || '';
        document.getElementById('clientNote').value = client.note || '';
    }
    
    new bootstrap.Modal(document.getElementById('clientModal')).show();
}

async function editClient(id) {
    try {
        const client = await apiCall(`/clientdetails?mandantId=${MANDANT_ID}&id=${id}`);
        showClientModal(client);
    } catch (error) {
        console.error('Error loading client for edit:', error);
    }
}

async function saveClient() {
    const id = document.getElementById('clientId').value;
    const data = {
        mandantId: MANDANT_ID,
        nummer: parseInt(document.getElementById('clientNummer').value) || null,
        titelId: parseInt(document.getElementById('clientTitel').value) || null,
        prefixId: parseInt(document.getElementById('clientPrefix').value) || null,
        firstName: document.getElementById('clientFirstName').value,
        lastName: document.getElementById('clientLastName').value,
        sex: document.getElementById('clientSex').value || null,
        birthDay: document.getElementById('clientBirthDay').value || null,
        statusId: parseInt(document.getElementById('clientStatus').value) || null,
        maritalStatusId: parseInt(document.getElementById('clientMaritalStatus').value) || null,
        startContractDate: document.getElementById('clientStartContractDate').value || null,
        policyNumber: document.getElementById('clientPolicyNumber').value || null,
        languageId: parseInt(document.getElementById('clientLanguage').value) || null,
        priorityId: parseInt(document.getElementById('clientPriority').value) || null,
        note: document.getElementById('clientNote').value || null
    };
    
    try {
        if (id) {
            await apiCall(`/clientdetails?id=${id}`, 'PUT', data);
            showToast('Klient erfolgreich aktualisiert.', 'success');
        } else {
            await apiCall('/clientdetails', 'POST', data);
            showToast('Klient erfolgreich erstellt.', 'success');
        }
        
        bootstrap.Modal.getInstance(document.getElementById('clientModal')).hide();
        loadClients(currentPage);
    } catch (error) {
        console.error('Error saving client:', error);
    }
}

async function deleteClient(mandantId, id) {
    if (!confirm('Möchten Sie diesen Klienten wirklich löschen?')) return;
    
    try {
        await apiCall(`/clientdetails?mandantId=${mandantId}&id=${id}`, 'DELETE');
        showToast('Klient erfolgreich gelöscht.', 'success');
        loadClients(currentPage);
    } catch (error) {
        console.error('Error deleting client:', error);
    }
}

// Devices
async function loadDevices() {
    try {
        const data = await apiCall('/devices');
        const tbody = document.getElementById('devices-table-body');
        
        if (data.results && data.results.length > 0) {
            tbody.innerHTML = data.results.map(d => `
                <tr>
                    <td>${d.deviceType || '-'}</td>
                    <td>${d.serialNumber || '-'}</td>
                    <td>${d.manufacturer || '-'}</td>
                    <td>${d.model || '-'}</td>
                    <td><span class="status-badge ${d.status === 'Verfügbar' ? 'status-active' : 'status-inactive'}">${d.status || '-'}</span></td>
                    <td>
                        <button class="btn btn-sm btn-outline-primary btn-action" onclick="editDevice(${d.id})">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger btn-action" onclick="deleteDevice(${MANDANT_ID}, ${d.id})">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>
            `).join('');
        } else {
            tbody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">Keine Geräte gefunden.</td></tr>';
        }
    } catch (error) {
        console.error('Error loading devices:', error);
    }
}

function showDeviceModal(device = null) {
    document.getElementById('deviceModalTitle').textContent = device ? 'Gerät bearbeiten' : 'Neues Gerät';
    document.getElementById('deviceForm').reset();
    document.getElementById('deviceId').value = device ? device.id : '';
    
    if (device) {
        document.getElementById('deviceType').value = device.deviceType || '';
        document.getElementById('deviceSerialNumber').value = device.serialNumber || '';
        document.getElementById('deviceManufacturer').value = device.manufacturer || '';
        document.getElementById('deviceModel').value = device.model || '';
        document.getElementById('deviceStatus').value = device.status || 'Verfügbar';
        document.getElementById('devicePurchaseDate').value = device.purchaseDate ? device.purchaseDate.split('T')[0] : '';
        document.getElementById('deviceDescription').value = device.description || '';
    }
    
    new bootstrap.Modal(document.getElementById('deviceModal')).show();
}

async function editDevice(id) {
    try {
        const device = await apiCall(`/deviceDetails/${MANDANT_ID}-${id}`);
        showDeviceModal(device);
    } catch (error) {
        console.error('Error loading device for edit:', error);
    }
}

async function saveDevice() {
    const id = document.getElementById('deviceId').value;
    const data = {
        mandantId: MANDANT_ID,
        deviceType: document.getElementById('deviceType').value,
        serialNumber: document.getElementById('deviceSerialNumber').value,
        manufacturer: document.getElementById('deviceManufacturer').value || null,
        model: document.getElementById('deviceModel').value || null,
        status: document.getElementById('deviceStatus').value,
        purchaseDate: document.getElementById('devicePurchaseDate').value || null,
        description: document.getElementById('deviceDescription').value || null
    };
    
    try {
        if (id) {
            await apiCall(`/devicedetails?id=${id}`, 'PUT', data);
            showToast('Gerät erfolgreich aktualisiert.', 'success');
        } else {
            await apiCall('/devicedetails', 'POST', data);
            showToast('Gerät erfolgreich erstellt.', 'success');
        }
        
        bootstrap.Modal.getInstance(document.getElementById('deviceModal')).hide();
        loadDevices();
    } catch (error) {
        console.error('Error saving device:', error);
    }
}

async function deleteDevice(mandantId, id) {
    if (!confirm('Möchten Sie dieses Gerät wirklich löschen?')) return;
    
    try {
        await apiCall(`/devicedetails?mandantId=${mandantId}&id=${id}`, 'DELETE');
        showToast('Gerät erfolgreich gelöscht.', 'success');
        loadDevices();
    } catch (error) {
        console.error('Error deleting device:', error);
    }
}

// Direct Providers
async function loadDirectProviders() {
    try {
        const data = await apiCall('/directProvider');
        const tbody = document.getElementById('direct-providers-table-body');
        
        if (data.results && data.results.length > 0) {
            tbody.innerHTML = data.results.map(p => `
                <tr>
                    <td>${p.name || '-'}</td>
                    <td>${p.firstName || ''} ${p.lastName || ''}</td>
                    <td>${p.phone || '-'}</td>
                    <td>${p.email || '-'}</td>
                    <td><span class="status-badge ${p.status === 'Aktiv' ? 'status-active' : 'status-inactive'}">${p.status || '-'}</span></td>
                    <td>
                        <button class="btn btn-sm btn-outline-primary btn-action" onclick="editDirectProvider(${p.id})">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger btn-action" onclick="deleteDirectProvider(${p.id})">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>
            `).join('');
        } else {
            tbody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">Keine Provider gefunden.</td></tr>';
        }
    } catch (error) {
        console.error('Error loading direct providers:', error);
    }
}

function showDirectProviderModal(provider = null) {
    document.getElementById('directProviderModalTitle').textContent = provider ? 'Provider bearbeiten' : 'Neuer Direct Provider';
    document.getElementById('directProviderForm').reset();
    document.getElementById('directProviderId').value = provider ? provider.id : '';
    
    if (provider) {
        document.getElementById('directProviderName').value = provider.name || '';
        document.getElementById('directProviderFirstName').value = provider.firstName || '';
        document.getElementById('directProviderLastName').value = provider.lastName || '';
        document.getElementById('directProviderStreet').value = provider.street || '';
        document.getElementById('directProviderHouseNumber').value = provider.houseNumber || '';
        document.getElementById('directProviderZipCode').value = provider.zipCode || '';
        document.getElementById('directProviderCity').value = provider.city || '';
        document.getElementById('directProviderPhone').value = provider.phone || '';
        document.getElementById('directProviderEmail').value = provider.email || '';
        document.getElementById('directProviderStatus').value = provider.status || 'Aktiv';
    }
    
    new bootstrap.Modal(document.getElementById('directProviderModal')).show();
}

async function editDirectProvider(id) {
    try {
        const provider = await apiCall(`/directProviderDetails?mandantId=${MANDANT_ID}&id=${id}`);
        showDirectProviderModal(provider);
    } catch (error) {
        console.error('Error loading provider for edit:', error);
    }
}

async function saveDirectProvider() {
    const id = document.getElementById('directProviderId').value;
    const data = {
        mandantId: MANDANT_ID,
        name: document.getElementById('directProviderName').value,
        firstName: document.getElementById('directProviderFirstName').value || null,
        lastName: document.getElementById('directProviderLastName').value || null,
        street: document.getElementById('directProviderStreet').value || null,
        houseNumber: document.getElementById('directProviderHouseNumber').value || null,
        zipCode: document.getElementById('directProviderZipCode').value || null,
        city: document.getElementById('directProviderCity').value || null,
        country: 'Deutschland',
        phone: document.getElementById('directProviderPhone').value || null,
        email: document.getElementById('directProviderEmail').value || null,
        status: document.getElementById('directProviderStatus').value
    };
    
    try {
        if (id) {
            await apiCall(`/directProviderDetails?id=${id}`, 'PUT', data);
            showToast('Provider erfolgreich aktualisiert.', 'success');
        } else {
            await apiCall('/directProviderDetails', 'POST', data);
            showToast('Provider erfolgreich erstellt.', 'success');
        }
        
        bootstrap.Modal.getInstance(document.getElementById('directProviderModal')).hide();
        loadDirectProviders();
    } catch (error) {
        console.error('Error saving provider:', error);
    }
}

async function deleteDirectProvider(id) {
    if (!confirm('Möchten Sie diesen Provider wirklich löschen?')) return;
    
    try {
        await apiCall(`/directProviderDetails?providerId=${id}&mandantId=${MANDANT_ID}`, 'DELETE');
        showToast('Provider erfolgreich gelöscht.', 'success');
        loadDirectProviders();
    } catch (error) {
        console.error('Error deleting provider:', error);
    }
}

// Professional Providers
async function loadProfessionalProviders() {
    try {
        const data = await apiCall('/professionalProvider');
        const tbody = document.getElementById('professional-providers-table-body');
        
        if (data.results && data.results.length > 0) {
            tbody.innerHTML = data.results.map(p => `
                <tr>
                    <td>${p.firstName || ''} ${p.lastName || ''}</td>
                    <td>${p.specialty || '-'}</td>
                    <td>${p.phone || '-'}</td>
                    <td>${p.email || '-'}</td>
                    <td><span class="status-badge ${p.status === 'Aktiv' ? 'status-active' : 'status-inactive'}">${p.status || '-'}</span></td>
                    <td>
                        <button class="btn btn-sm btn-outline-primary btn-action" onclick="editProfessionalProvider(${p.id})">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger btn-action" onclick="deleteProfessionalProvider(${p.id})">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>
            `).join('');
        } else {
            tbody.innerHTML = '<tr><td colspan="6" class="text-center text-muted">Keine Provider gefunden.</td></tr>';
        }
    } catch (error) {
        console.error('Error loading professional providers:', error);
    }
}

function showProfessionalProviderModal(provider = null) {
    document.getElementById('professionalProviderModalTitle').textContent = provider ? 'Provider bearbeiten' : 'Neuer Professional Provider';
    document.getElementById('professionalProviderForm').reset();
    document.getElementById('professionalProviderId').value = provider ? provider.id : '';
    
    if (provider) {
        document.getElementById('professionalProviderFirstName').value = provider.firstName || '';
        document.getElementById('professionalProviderLastName').value = provider.lastName || '';
        document.getElementById('professionalProviderSpecialty').value = provider.specialty || '';
        document.getElementById('professionalProviderLicenseNumber').value = provider.licenseNumber || '';
        document.getElementById('professionalProviderStreet').value = provider.street || '';
        document.getElementById('professionalProviderHouseNumber').value = provider.houseNumber || '';
        document.getElementById('professionalProviderZipCode').value = provider.zipCode || '';
        document.getElementById('professionalProviderCity').value = provider.city || '';
        document.getElementById('professionalProviderPhone').value = provider.phone || '';
        document.getElementById('professionalProviderEmail').value = provider.email || '';
        document.getElementById('professionalProviderStatus').value = provider.status || 'Aktiv';
    }
    
    new bootstrap.Modal(document.getElementById('professionalProviderModal')).show();
}

async function editProfessionalProvider(id) {
    try {
        const provider = await apiCall(`/professionalProviderDetail?mandantId=${MANDANT_ID}&id=${id}`);
        showProfessionalProviderModal(provider);
    } catch (error) {
        console.error('Error loading provider for edit:', error);
    }
}

async function saveProfessionalProvider() {
    const id = document.getElementById('professionalProviderId').value;
    const firstName = document.getElementById('professionalProviderFirstName').value;
    const lastName = document.getElementById('professionalProviderLastName').value;
    
    const data = {
        mandantId: MANDANT_ID,
        name: `${firstName} ${lastName}`.trim(),
        firstName: firstName,
        lastName: lastName,
        specialty: document.getElementById('professionalProviderSpecialty').value || null,
        licenseNumber: document.getElementById('professionalProviderLicenseNumber').value || null,
        street: document.getElementById('professionalProviderStreet').value || null,
        houseNumber: document.getElementById('professionalProviderHouseNumber').value || null,
        zipCode: document.getElementById('professionalProviderZipCode').value || null,
        city: document.getElementById('professionalProviderCity').value || null,
        country: 'Deutschland',
        phone: document.getElementById('professionalProviderPhone').value || null,
        email: document.getElementById('professionalProviderEmail').value || null,
        status: document.getElementById('professionalProviderStatus').value
    };
    
    try {
        if (id) {
            await apiCall(`/professionalProviderDetails?id=${id}`, 'PUT', data);
            showToast('Provider erfolgreich aktualisiert.', 'success');
        } else {
            await apiCall('/professionalProviderDetails', 'POST', data);
            showToast('Provider erfolgreich erstellt.', 'success');
        }
        
        bootstrap.Modal.getInstance(document.getElementById('professionalProviderModal')).hide();
        loadProfessionalProviders();
    } catch (error) {
        console.error('Error saving provider:', error);
    }
}

async function deleteProfessionalProvider(id) {
    if (!confirm('Möchten Sie diesen Provider wirklich löschen?')) return;
    
    try {
        // Note: No delete endpoint in original API, but we can implement it
        showToast('Löschen nicht implementiert in der Original-API.', 'warning');
    } catch (error) {
        console.error('Error deleting provider:', error);
    }
}

// System Entries
async function loadSystemEntries() {
    try {
        const [titles, prefixes, statuses, maritalStatuses, invoiceMethods, priorities, languages, tarifs] = await Promise.all([
            apiCall('/title'),
            apiCall('/prefix'),
            apiCall('/clientStatus'),
            apiCall('/maritalStatus'),
            apiCall('/invoiceMethod'),
            apiCall('/priority'),
            apiCall('/language'),
            apiCall('/Tarifs')
        ]);
        
        renderSystemEntryList('titles-list', titles);
        renderSystemEntryList('prefixes-list', prefixes);
        renderSystemEntryList('statuses-list', statuses);
        renderSystemEntryList('marital-statuses-list', maritalStatuses);
        renderSystemEntryList('invoice-methods-list', invoiceMethods);
        renderSystemEntryList('priorities-list', priorities);
        renderSystemEntryList('languages-list', languages);
        renderTarifsList('tarifs-list', tarifs);
    } catch (error) {
        console.error('Error loading system entries:', error);
    }
}

function renderSystemEntryList(containerId, entries) {
    const container = document.getElementById(containerId);
    if (entries && entries.length > 0) {
        container.innerHTML = `
            <ul class="list-group list-group-flush">
                ${entries.map(e => `
                    <li class="list-group-item d-flex justify-content-between align-items-center">
                        ${e.description || '-'}
                        <span class="badge bg-secondary">${e.code || ''}</span>
                    </li>
                `).join('')}
            </ul>
        `;
    } else {
        container.innerHTML = '<p class="text-muted">Keine Einträge vorhanden.</p>';
    }
}

function renderTarifsList(containerId, tarifs) {
    const container = document.getElementById(containerId);
    if (tarifs && tarifs.length > 0) {
        container.innerHTML = `
            <ul class="list-group list-group-flush">
                ${tarifs.map(t => `
                    <li class="list-group-item">
                        <div class="d-flex justify-content-between">
                            <strong>${t.name || '-'}</strong>
                            <span>${t.totalPrice ? t.totalPrice.toFixed(2) + ' €' : '-'}</span>
                        </div>
                        <small class="text-muted">${t.description || ''}</small>
                    </li>
                `).join('')}
            </ul>
        `;
    } else {
        container.innerHTML = '<p class="text-muted">Keine Tarife vorhanden.</p>';
    }
}

// Load dropdown data
async function loadDropdownData() {
    try {
        const [titles, prefixes, statuses, maritalStatuses, languages, priorities] = await Promise.all([
            apiCall('/title'),
            apiCall('/prefix'),
            apiCall('/clientStatus'),
            apiCall('/maritalStatus'),
            apiCall('/language'),
            apiCall('/priority')
        ]);
        
        populateSelect('clientTitel', titles, 'id', 'description');
        populateSelect('clientPrefix', prefixes, 'id', 'description');
        populateSelect('clientStatus', statuses, 'id', 'description');
        populateSelect('clientMaritalStatus', maritalStatuses, 'id', 'description');
        populateSelect('clientLanguage', languages, 'id', 'description');
        populateSelect('clientPriority', priorities, 'id', 'description');
    } catch (error) {
        console.error('Error loading dropdown data:', error);
    }
}

function populateSelect(selectId, options, valueField, textField) {
    const select = document.getElementById(selectId);
    if (!select) return;
    
    select.innerHTML = '<option value="">-- Auswählen --</option>';
    if (options && options.length > 0) {
        options.forEach(opt => {
            const option = document.createElement('option');
            option.value = opt[valueField];
            option.textContent = opt[textField];
            select.appendChild(option);
        });
    }
}

// Utility functions
function formatDate(dateString) {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleDateString('de-DE');
}

function renderPagination(containerId, totalCount, pageCount, currentPage, loadFunction) {
    const container = document.getElementById(containerId);
    if (pageCount <= 1) {
        container.innerHTML = '';
        return;
    }
    
    let html = '';
    
    // Previous button
    html += `<li class="page-item ${currentPage === 0 ? 'disabled' : ''}">
        <a class="page-link" href="#" onclick="event.preventDefault(); ${currentPage > 0 ? `${loadFunction.name}(${currentPage - 1})` : ''}">Zurück</a>
    </li>`;
    
    // Page numbers
    for (let i = 0; i < pageCount && i < 5; i++) {
        html += `<li class="page-item ${i === currentPage ? 'active' : ''}">
            <a class="page-link" href="#" onclick="event.preventDefault(); ${loadFunction.name}(${i})">${i + 1}</a>
        </li>`;
    }
    
    // Next button
    html += `<li class="page-item ${currentPage >= pageCount - 1 ? 'disabled' : ''}">
        <a class="page-link" href="#" onclick="event.preventDefault(); ${currentPage < pageCount - 1 ? `${loadFunction.name}(${currentPage + 1})` : ''}">Weiter</a>
    </li>`;
    
    container.innerHTML = html;
}

function showLoading(show) {
    document.getElementById('loadingSpinner').style.display = show ? 'block' : 'none';
}

function showToast(message, type = 'info') {
    const container = document.getElementById('toastContainer');
    const toastId = 'toast-' + Date.now();
    
    const bgClass = {
        'success': 'bg-success',
        'danger': 'bg-danger',
        'warning': 'bg-warning',
        'info': 'bg-info'
    }[type] || 'bg-info';
    
    const html = `
        <div id="${toastId}" class="toast align-items-center text-white ${bgClass} border-0" role="alert">
            <div class="d-flex">
                <div class="toast-body">${message}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `;
    
    container.insertAdjacentHTML('beforeend', html);
    const toastElement = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastElement, { delay: 3000 });
    toast.show();
    
    toastElement.addEventListener('hidden.bs.toast', () => toastElement.remove());
}
