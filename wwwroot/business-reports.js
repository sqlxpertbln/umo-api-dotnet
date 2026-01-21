// Business Reports JavaScript
const API_BASE = '';
let charts = {};

// Section titles and subtitles
const sectionInfo = {
    'marketing-dashboard': { title: 'Marketing Dashboard', subtitle: 'Kampagnen, Leads und Marketing-Performance' },
    'lead-qualification': { title: 'Lead-Qualifizierung', subtitle: 'Lead-Scoring und Qualifizierungsanalyse' },
    'crm-activities': { title: 'CRM Aktivitäten', subtitle: 'Kundeninteraktionen und Follow-ups' },
    'sales-dashboard': { title: 'Sales Dashboard', subtitle: 'Umsatz, Pipeline und Vertriebsleistung' },
    'sales-pipeline': { title: 'Sales Pipeline', subtitle: 'Opportunities und Verkaufschancen' },
    'erp-inventory': { title: 'Lagerverwaltung', subtitle: 'Bestand, Artikel und Warenbewegungen' },
    'erp-devices': { title: 'Geräte & Artikel', subtitle: 'Geräteverwaltung und Artikelstamm' },
    'erp-medications': { title: 'Medikamente', subtitle: 'Medikamentenbestand und Verfallsdaten' },
    'billing-dashboard': { title: 'Abrechnung', subtitle: 'Rechnungen, Zahlungen und Mahnwesen' },
    'finance-overview': { title: 'Finanzübersicht', subtitle: 'Umsatz, Kosten und Profitabilität' },
    'finance-receivables': { title: 'Forderungen', subtitle: 'Offene Posten und Fälligkeitsanalyse' }
};

// Initialize
document.addEventListener('DOMContentLoaded', () => {
    setupNavigation();
    loadMarketingDashboard();
});

// Navigation setup
function setupNavigation() {
    document.querySelectorAll('.nav-link[data-section]').forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            const section = link.dataset.section;
            
            // Update active states
            document.querySelectorAll('.nav-link').forEach(l => l.classList.remove('active'));
            link.classList.add('active');
            
            // Show section
            document.querySelectorAll('.dashboard-section').forEach(s => s.classList.remove('active'));
            document.getElementById(section).classList.add('active');
            
            // Update header
            const info = sectionInfo[section];
            document.getElementById('section-title').textContent = info.title;
            document.getElementById('section-subtitle').textContent = info.subtitle;
            
            // Load data
            loadSectionData(section);
        });
    });
}

function loadSectionData(section) {
    switch(section) {
        case 'marketing-dashboard': loadMarketingDashboard(); break;
        case 'lead-qualification': loadLeadQualification(); break;
        case 'crm-activities': loadCRMActivities(); break;
        case 'sales-dashboard': loadSalesDashboard(); break;
        case 'sales-pipeline': loadSalesPipeline(); break;
        case 'erp-inventory': loadERPInventory(); break;
        case 'erp-devices': loadERPDevices(); break;
        case 'erp-medications': loadERPMedications(); break;
        case 'billing-dashboard': loadBillingDashboard(); break;
        case 'finance-overview': loadFinanceOverview(); break;
        case 'finance-receivables': loadFinanceReceivables(); break;
    }
}

function refreshCurrentSection() {
    const activeSection = document.querySelector('.dashboard-section.active');
    if (activeSection) {
        loadSectionData(activeSection.id);
    }
}

function exportReport() {
    alert('Export-Funktion wird in einer zukünftigen Version verfügbar sein.');
}

// Utility functions
function formatCurrency(value) {
    return new Intl.NumberFormat('de-DE', { style: 'currency', currency: 'EUR' }).format(value);
}

function formatNumber(value) {
    return new Intl.NumberFormat('de-DE').format(value);
}

function formatPercent(value) {
    return new Intl.NumberFormat('de-DE', { style: 'percent', minimumFractionDigits: 1 }).format(value / 100);
}

function destroyChart(chartId) {
    if (charts[chartId]) {
        charts[chartId].destroy();
        delete charts[chartId];
    }
}

// Chart colors
const chartColors = {
    primary: '#0d6efd',
    success: '#28a745',
    warning: '#ffc107',
    danger: '#dc3545',
    info: '#17a2b8',
    secondary: '#6c757d',
    purple: '#6f42c1',
    pink: '#e83e8c',
    orange: '#fd7e14',
    teal: '#20c997'
};

const colorPalette = [chartColors.primary, chartColors.success, chartColors.warning, chartColors.danger, chartColors.info, chartColors.purple, chartColors.orange, chartColors.teal];

// Marketing Dashboard
async function loadMarketingDashboard() {
    try {
        const response = await fetch(`${API_BASE}/businessreports/marketing/dashboard`);
        const data = await response.json();
        
        // KPIs
        document.getElementById('mkt-total-campaigns').textContent = data.totalCampaigns;
        document.getElementById('mkt-active-campaigns').textContent = `${data.activeCampaigns} aktiv`;
        document.getElementById('mkt-total-leads').textContent = data.totalLeads;
        document.getElementById('mkt-new-leads').textContent = `${data.newLeadsThisMonth} neu diesen Monat`;
        document.getElementById('mkt-budget').textContent = formatCurrency(data.totalMarketingBudget);
        document.getElementById('mkt-spend').textContent = `${formatCurrency(data.totalMarketingSpend)} ausgegeben`;
        document.getElementById('mkt-conversion').textContent = `${data.leadConversionRate.toFixed(1)}%`;
        document.getElementById('mkt-cpl').textContent = `CPL: ${formatCurrency(data.costPerLead)}`;
        
        // Lead Trend Chart
        destroyChart('mkt-lead-trend-chart');
        charts['mkt-lead-trend-chart'] = new Chart(document.getElementById('mkt-lead-trend-chart'), {
            type: 'line',
            data: {
                labels: data.leadTrend.map(t => t.period),
                datasets: [{
                    label: 'Leads',
                    data: data.leadTrend.map(t => t.value),
                    borderColor: chartColors.primary,
                    backgroundColor: chartColors.primary + '20',
                    fill: true,
                    tension: 0.4
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Leads by Source Chart
        destroyChart('mkt-leads-source-chart');
        charts['mkt-leads-source-chart'] = new Chart(document.getElementById('mkt-leads-source-chart'), {
            type: 'doughnut',
            data: {
                labels: data.leadsBySource.map(s => s.label),
                datasets: [{
                    data: data.leadsBySource.map(s => s.value),
                    backgroundColor: colorPalette
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Campaign Performance Chart
        destroyChart('mkt-campaign-chart');
        charts['mkt-campaign-chart'] = new Chart(document.getElementById('mkt-campaign-chart'), {
            type: 'bar',
            data: {
                labels: data.campaignPerformance.map(c => c.label),
                datasets: [{
                    label: 'Leads',
                    data: data.campaignPerformance.map(c => c.value),
                    backgroundColor: chartColors.primary
                }]
            },
            options: { responsive: true, maintainAspectRatio: false, indexAxis: 'y' }
        });
        
        // Top Campaigns Table
        const tbody = document.querySelector('#mkt-top-campaigns-table tbody');
        tbody.innerHTML = data.topCampaigns.map(c => `
            <tr>
                <td>${c.name}</td>
                <td><span class="badge bg-secondary">${c.type}</span></td>
                <td>${c.leads}</td>
                <td>${c.conversions}</td>
                <td class="${c.roi >= 0 ? 'text-success' : 'text-danger'}">${c.roi.toFixed(0)}%</td>
            </tr>
        `).join('');
        
    } catch (error) {
        console.error('Error loading marketing dashboard:', error);
    }
}

// Lead Qualification
async function loadLeadQualification() {
    try {
        const response = await fetch(`${API_BASE}/businessreports/marketing/lead-qualification`);
        const data = await response.json();
        
        // KPIs
        document.getElementById('lq-hot-leads').textContent = data.hotLeads;
        document.getElementById('lq-warm-leads').textContent = data.warmLeads;
        document.getElementById('lq-cold-leads').textContent = data.coldLeads;
        document.getElementById('lq-qual-rate').textContent = `${data.qualificationRate.toFixed(1)}%`;
        
        // Qualification Chart
        destroyChart('lq-qualification-chart');
        charts['lq-qualification-chart'] = new Chart(document.getElementById('lq-qualification-chart'), {
            type: 'doughnut',
            data: {
                labels: data.leadsByQualification.map(q => q.label),
                datasets: [{
                    data: data.leadsByQualification.map(q => q.value),
                    backgroundColor: [chartColors.danger, chartColors.warning, chartColors.info]
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Score Distribution Chart
        destroyChart('lq-score-chart');
        charts['lq-score-chart'] = new Chart(document.getElementById('lq-score-chart'), {
            type: 'bar',
            data: {
                labels: data.leadScoreDistribution.map(s => s.label),
                datasets: [{
                    label: 'Leads',
                    data: data.leadScoreDistribution.map(s => s.value),
                    backgroundColor: [chartColors.info, chartColors.warning, chartColors.orange, chartColors.danger]
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Leads Table
        const tbody = document.querySelector('#lq-leads-table tbody');
        tbody.innerHTML = data.recentQualifiedLeads.map(l => `
            <tr>
                <td>${l.name}</td>
                <td>${l.company || '-'}</td>
                <td>${l.source}</td>
                <td><strong>${l.score}</strong></td>
                <td>${formatCurrency(l.estimatedValue)}</td>
                <td><span class="badge badge-status badge-${l.status?.toLowerCase() === 'qualified' ? 'warm' : 'active'}">${l.status}</span></td>
            </tr>
        `).join('');
        
    } catch (error) {
        console.error('Error loading lead qualification:', error);
    }
}

// CRM Activities
async function loadCRMActivities() {
    try {
        const response = await fetch(`${API_BASE}/businessreports/crm/activities`);
        const data = await response.json();
        
        // KPIs
        document.getElementById('crm-total-activities').textContent = data.totalActivities;
        document.getElementById('crm-calls').textContent = data.callsMade;
        document.getElementById('crm-emails').textContent = data.emailsSent;
        document.getElementById('crm-meetings').textContent = data.meetingsHeld;
        document.getElementById('crm-this-week').textContent = data.activitiesThisWeek;
        document.getElementById('crm-overdue').textContent = data.overdueFollowUps;
        
        // Activity Trend Chart
        destroyChart('crm-activity-trend-chart');
        charts['crm-activity-trend-chart'] = new Chart(document.getElementById('crm-activity-trend-chart'), {
            type: 'bar',
            data: {
                labels: data.activityTrend.map(t => t.period),
                datasets: [{
                    label: 'Aktivitäten',
                    data: data.activityTrend.map(t => t.value),
                    backgroundColor: chartColors.primary
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Activity Type Chart
        destroyChart('crm-activity-type-chart');
        charts['crm-activity-type-chart'] = new Chart(document.getElementById('crm-activity-type-chart'), {
            type: 'pie',
            data: {
                labels: data.activitiesByType.map(t => t.label),
                datasets: [{
                    data: data.activitiesByType.map(t => t.value),
                    backgroundColor: colorPalette
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // User Table
        const tbody = document.querySelector('#crm-user-table tbody');
        tbody.innerHTML = data.activityByUser.map(u => `
            <tr>
                <td><strong>${u.userName}</strong></td>
                <td>${u.totalActivities}</td>
                <td>${u.calls}</td>
                <td>${u.emails}</td>
                <td>${u.meetings}</td>
                <td><span class="text-success">${u.conversions}</span></td>
            </tr>
        `).join('');
        
    } catch (error) {
        console.error('Error loading CRM activities:', error);
    }
}

// Sales Dashboard
async function loadSalesDashboard() {
    try {
        const response = await fetch(`${API_BASE}/businessreports/sales/dashboard`);
        const data = await response.json();
        
        // KPIs
        document.getElementById('sales-revenue').textContent = formatCurrency(data.totalRevenue);
        document.getElementById('sales-growth').textContent = `${data.revenueGrowth >= 0 ? '+' : ''}${data.revenueGrowth.toFixed(1)}% vs. Vormonat`;
        document.getElementById('sales-pipeline').textContent = formatCurrency(data.pipelineValue);
        document.getElementById('sales-weighted').textContent = `Gewichtet: ${formatCurrency(data.weightedPipeline)}`;
        document.getElementById('sales-winrate').textContent = `${data.winRate.toFixed(0)}%`;
        document.getElementById('sales-orders').textContent = data.totalOrders;
        document.getElementById('sales-avg-order').textContent = `Ø ${formatCurrency(data.averageOrderValue)}`;
        
        // Revenue Trend Chart
        destroyChart('sales-revenue-chart');
        charts['sales-revenue-chart'] = new Chart(document.getElementById('sales-revenue-chart'), {
            type: 'line',
            data: {
                labels: data.revenueTrend.map(t => t.period),
                datasets: [{
                    label: 'Umsatz',
                    data: data.revenueTrend.map(t => t.value),
                    borderColor: chartColors.success,
                    backgroundColor: chartColors.success + '20',
                    fill: true,
                    tension: 0.4
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Pipeline Chart
        destroyChart('sales-pipeline-chart');
        charts['sales-pipeline-chart'] = new Chart(document.getElementById('sales-pipeline-chart'), {
            type: 'doughnut',
            data: {
                labels: data.pipelineByStage.map(s => s.label),
                datasets: [{
                    data: data.pipelineByStage.map(s => s.value),
                    backgroundColor: colorPalette
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Sales Reps Table
        const tbody = document.querySelector('#sales-reps-table tbody');
        tbody.innerHTML = data.topSalesReps.map(r => `
            <tr>
                <td><strong>${r.name}</strong></td>
                <td>${formatCurrency(r.revenue)}</td>
                <td>${r.deals}</td>
                <td>${r.winRate.toFixed(0)}%</td>
                <td>${formatCurrency(r.averageDealSize)}</td>
                <td>
                    <div class="progress progress-thin">
                        <div class="progress-bar ${r.achievement >= 100 ? 'bg-success' : 'bg-primary'}" style="width: ${Math.min(r.achievement, 100)}%"></div>
                    </div>
                    <small>${r.achievement.toFixed(0)}%</small>
                </td>
            </tr>
        `).join('');
        
    } catch (error) {
        console.error('Error loading sales dashboard:', error);
    }
}

// Sales Pipeline
async function loadSalesPipeline() {
    try {
        const response = await fetch(`${API_BASE}/businessreports/sales/pipeline`);
        const data = await response.json();
        
        // KPIs
        document.getElementById('pipe-total').textContent = formatCurrency(data.totalPipelineValue);
        document.getElementById('pipe-weighted').textContent = formatCurrency(data.weightedValue);
        document.getElementById('pipe-opportunities').textContent = data.totalOpportunities;
        document.getElementById('pipe-forecast').textContent = formatCurrency(data.forecastedRevenue);
        
        // Stages Chart
        destroyChart('pipe-stages-chart');
        charts['pipe-stages-chart'] = new Chart(document.getElementById('pipe-stages-chart'), {
            type: 'bar',
            data: {
                labels: data.pipelineStages.map(s => s.name),
                datasets: [{
                    label: 'Wert',
                    data: data.pipelineStages.map(s => s.value),
                    backgroundColor: chartColors.primary
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Win/Loss Chart
        destroyChart('pipe-winloss-chart');
        charts['pipe-winloss-chart'] = new Chart(document.getElementById('pipe-winloss-chart'), {
            type: 'doughnut',
            data: {
                labels: data.winLossAnalysis.map(w => w.label),
                datasets: [{
                    data: data.winLossAnalysis.map(w => w.value),
                    backgroundColor: data.winLossAnalysis.map(w => w.color)
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Opportunities Table
        const tbody = document.querySelector('#pipe-opportunities-table tbody');
        tbody.innerHTML = data.topOpportunities.map(o => `
            <tr>
                <td><strong>${o.name}</strong></td>
                <td>${o.clientName}</td>
                <td>${formatCurrency(o.amount)}</td>
                <td>${o.probability}%</td>
                <td><span class="badge bg-primary">${o.stage}</span></td>
                <td>${o.expectedClose ? new Date(o.expectedClose).toLocaleDateString('de-DE') : '-'}</td>
            </tr>
        `).join('');
        
    } catch (error) {
        console.error('Error loading sales pipeline:', error);
    }
}

// ERP Inventory
async function loadERPInventory() {
    try {
        const response = await fetch(`${API_BASE}/businessreports/erp/inventory`);
        const data = await response.json();
        
        // KPIs
        document.getElementById('inv-total').textContent = data.totalArticles;
        document.getElementById('inv-value').textContent = formatCurrency(data.totalInventoryValue);
        document.getElementById('inv-low').textContent = data.lowStockItems;
        document.getElementById('inv-out').textContent = data.outOfStockItems;
        document.getElementById('inv-expiring').textContent = data.expiringItems;
        document.getElementById('inv-turnover').textContent = `${data.inventoryTurnover.toFixed(1)}x`;
        
        // Category Chart
        destroyChart('inv-category-chart');
        charts['inv-category-chart'] = new Chart(document.getElementById('inv-category-chart'), {
            type: 'bar',
            data: {
                labels: data.stockByCategory.map(c => c.label),
                datasets: [{
                    label: 'Bestand',
                    data: data.stockByCategory.map(c => c.value),
                    backgroundColor: colorPalette
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Value Chart
        destroyChart('inv-value-chart');
        charts['inv-value-chart'] = new Chart(document.getElementById('inv-value-chart'), {
            type: 'doughnut',
            data: {
                labels: data.valueByCategory.map(c => c.label),
                datasets: [{
                    data: data.valueByCategory.map(c => c.value),
                    backgroundColor: colorPalette
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Alerts Table
        const alertsTbody = document.querySelector('#inv-alerts-table tbody');
        alertsTbody.innerHTML = data.lowStockAlerts.map(a => `
            <tr>
                <td>${a.name}</td>
                <td>${a.category}</td>
                <td class="${a.currentStock === 0 ? 'text-danger' : 'text-warning'}"><strong>${a.currentStock}</strong></td>
                <td>${a.minStock}</td>
                <td><span class="badge ${a.status === 'Out of Stock' ? 'bg-danger' : 'bg-warning'}">${a.status}</span></td>
            </tr>
        `).join('');
        
        // Top Items Table
        const topTbody = document.querySelector('#inv-top-table tbody');
        topTbody.innerHTML = data.topMovingItems.map(i => `
            <tr>
                <td>${i.name}</td>
                <td>${i.category}</td>
                <td>${i.currentStock}</td>
                <td>${formatCurrency(i.value)}</td>
            </tr>
        `).join('');
        
    } catch (error) {
        console.error('Error loading ERP inventory:', error);
    }
}

// ERP Devices
async function loadERPDevices() {
    try {
        const response = await fetch(`${API_BASE}/businessreports/erp/devices`);
        const data = await response.json();
        
        // KPIs
        document.getElementById('dev-total').textContent = data.totalDevices;
        document.getElementById('dev-active').textContent = data.activeDevices;
        document.getElementById('dev-assigned').textContent = data.assignedDevices;
        document.getElementById('dev-available').textContent = data.availableDevices;
        document.getElementById('dev-maintenance').textContent = data.devicesInMaintenance;
        document.getElementById('dev-value').textContent = formatCurrency(data.totalDeviceValue);
        
        // Type Chart
        destroyChart('dev-type-chart');
        charts['dev-type-chart'] = new Chart(document.getElementById('dev-type-chart'), {
            type: 'bar',
            data: {
                labels: data.devicesByType.map(t => t.label),
                datasets: [{
                    label: 'Geräte',
                    data: data.devicesByType.map(t => t.value),
                    backgroundColor: colorPalette
                }]
            },
            options: { responsive: true, maintainAspectRatio: false, indexAxis: 'y' }
        });
        
        // Status Chart
        destroyChart('dev-status-chart');
        charts['dev-status-chart'] = new Chart(document.getElementById('dev-status-chart'), {
            type: 'doughnut',
            data: {
                labels: data.devicesByStatus.map(s => s.label),
                datasets: [{
                    data: data.devicesByStatus.map(s => s.value),
                    backgroundColor: colorPalette
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
    } catch (error) {
        console.error('Error loading ERP devices:', error);
    }
}

// ERP Medications
async function loadERPMedications() {
    try {
        const response = await fetch(`${API_BASE}/businessreports/erp/medications`);
        const data = await response.json();
        
        // KPIs
        document.getElementById('med-total').textContent = data.totalMedications;
        document.getElementById('med-active').textContent = data.activeMedications;
        document.getElementById('med-prescription').textContent = data.prescriptionRequired;
        document.getElementById('med-low').textContent = data.lowStockMedications;
        document.getElementById('med-expiring').textContent = data.expiringMedications;
        document.getElementById('med-value').textContent = formatCurrency(data.totalMedicationValue);
        
        // Form Chart
        destroyChart('med-form-chart');
        charts['med-form-chart'] = new Chart(document.getElementById('med-form-chart'), {
            type: 'doughnut',
            data: {
                labels: data.medicationsByForm.map(f => f.label),
                datasets: [{
                    data: data.medicationsByForm.map(f => f.value),
                    backgroundColor: colorPalette
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Manufacturer Chart
        destroyChart('med-manufacturer-chart');
        charts['med-manufacturer-chart'] = new Chart(document.getElementById('med-manufacturer-chart'), {
            type: 'bar',
            data: {
                labels: data.medicationsByManufacturer.map(m => m.label),
                datasets: [{
                    label: 'Medikamente',
                    data: data.medicationsByManufacturer.map(m => m.value),
                    backgroundColor: colorPalette
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
    } catch (error) {
        console.error('Error loading ERP medications:', error);
    }
}

// Billing Dashboard
async function loadBillingDashboard() {
    try {
        const response = await fetch(`${API_BASE}/businessreports/billing/dashboard`);
        const data = await response.json();
        
        // KPIs
        document.getElementById('bill-invoiced').textContent = formatCurrency(data.totalInvoiced);
        document.getElementById('bill-paid').textContent = formatCurrency(data.totalPaid);
        document.getElementById('bill-outstanding').textContent = formatCurrency(data.totalOutstanding);
        document.getElementById('bill-overdue').textContent = formatCurrency(data.overdueAmount);
        
        // Trend Chart
        destroyChart('bill-trend-chart');
        charts['bill-trend-chart'] = new Chart(document.getElementById('bill-trend-chart'), {
            type: 'line',
            data: {
                labels: data.revenueTrend.map(t => t.period),
                datasets: [
                    {
                        label: 'Fakturiert',
                        data: data.revenueTrend.map(t => t.value),
                        borderColor: chartColors.primary,
                        tension: 0.4
                    },
                    {
                        label: 'Bezahlt',
                        data: data.paymentTrend.map(t => t.value),
                        borderColor: chartColors.success,
                        tension: 0.4
                    }
                ]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Status Chart
        destroyChart('bill-status-chart');
        charts['bill-status-chart'] = new Chart(document.getElementById('bill-status-chart'), {
            type: 'doughnut',
            data: {
                labels: data.invoicesByStatus.map(s => s.label),
                datasets: [{
                    data: data.invoicesByStatus.map(s => s.value),
                    backgroundColor: colorPalette
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Aging Chart
        destroyChart('bill-aging-chart');
        charts['bill-aging-chart'] = new Chart(document.getElementById('bill-aging-chart'), {
            type: 'bar',
            data: {
                labels: data.agingReport.map(a => a.period),
                datasets: [{
                    label: 'Betrag',
                    data: data.agingReport.map(a => a.amount),
                    backgroundColor: [chartColors.success, chartColors.info, chartColors.warning, chartColors.orange, chartColors.danger]
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Overdue Table
        const tbody = document.querySelector('#bill-overdue-table tbody');
        tbody.innerHTML = data.overdueInvoicesList.map(i => `
            <tr>
                <td>${i.invoiceNumber}</td>
                <td>${i.clientName}</td>
                <td>${formatCurrency(i.amount)}</td>
                <td class="text-danger">${formatCurrency(i.outstanding)}</td>
                <td><span class="badge bg-danger">${i.daysOverdue} Tage</span></td>
            </tr>
        `).join('');
        
    } catch (error) {
        console.error('Error loading billing dashboard:', error);
    }
}

// Finance Overview
async function loadFinanceOverview() {
    try {
        const response = await fetch(`${API_BASE}/businessreports/finance/overview`);
        const data = await response.json();
        
        // KPIs
        document.getElementById('fin-revenue').textContent = formatCurrency(data.totalRevenue);
        document.getElementById('fin-revenue-yoy').textContent = `+${data.revenueYoY.toFixed(1)}% YoY`;
        document.getElementById('fin-costs').textContent = formatCurrency(data.totalCosts);
        document.getElementById('fin-profit').textContent = formatCurrency(data.grossProfit);
        document.getElementById('fin-profit-yoy').textContent = `+${data.profitYoY.toFixed(1)}% YoY`;
        document.getElementById('fin-margin').textContent = `${data.grossMargin.toFixed(1)}%`;
        
        // Trend Chart
        destroyChart('fin-trend-chart');
        charts['fin-trend-chart'] = new Chart(document.getElementById('fin-trend-chart'), {
            type: 'bar',
            data: {
                labels: data.revenueCostTrend.map(t => t.period),
                datasets: [
                    {
                        label: 'Umsatz',
                        data: data.revenueCostTrend.map(t => t.value),
                        backgroundColor: chartColors.success
                    },
                    {
                        label: 'Kosten',
                        data: data.revenueCostTrend.map(t => t.value2),
                        backgroundColor: chartColors.danger
                    }
                ]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Costs Chart
        destroyChart('fin-costs-chart');
        charts['fin-costs-chart'] = new Chart(document.getElementById('fin-costs-chart'), {
            type: 'doughnut',
            data: {
                labels: data.costBreakdown.map(c => c.label),
                datasets: [{
                    data: data.costBreakdown.map(c => c.value),
                    backgroundColor: colorPalette
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Cost Center Table
        const tbody = document.querySelector('#fin-costcenter-table tbody');
        tbody.innerHTML = data.costCenterPerformance.map(c => `
            <tr>
                <td>${c.code}</td>
                <td><strong>${c.name}</strong></td>
                <td>${c.department}</td>
                <td>${formatCurrency(c.budget)}</td>
                <td>${formatCurrency(c.actual)}</td>
                <td class="${c.variance >= 0 ? 'text-success' : 'text-danger'}">${formatCurrency(c.variance)}</td>
                <td>
                    <div class="progress progress-thin">
                        <div class="progress-bar ${c.actual <= c.budget ? 'bg-success' : 'bg-danger'}" style="width: ${Math.min((c.actual / c.budget) * 100, 100)}%"></div>
                    </div>
                </td>
            </tr>
        `).join('');
        
    } catch (error) {
        console.error('Error loading finance overview:', error);
    }
}

// Finance Receivables
async function loadFinanceReceivables() {
    try {
        const response = await fetch(`${API_BASE}/businessreports/finance/receivables`);
        const data = await response.json();
        
        // KPIs
        document.getElementById('rec-total').textContent = formatCurrency(data.totalReceivables);
        document.getElementById('rec-current').textContent = formatCurrency(data.current);
        document.getElementById('rec-overdue').textContent = formatCurrency(data.days30 + data.days60 + data.days90Plus);
        document.getElementById('rec-dso').textContent = data.dso;
        
        // Aging Chart
        destroyChart('rec-aging-chart');
        charts['rec-aging-chart'] = new Chart(document.getElementById('rec-aging-chart'), {
            type: 'bar',
            data: {
                labels: data.agingAnalysis.map(a => a.period),
                datasets: [{
                    label: 'Betrag',
                    data: data.agingAnalysis.map(a => a.amount),
                    backgroundColor: [chartColors.success, chartColors.info, chartColors.warning, chartColors.orange, chartColors.danger]
                }]
            },
            options: { responsive: true, maintainAspectRatio: false }
        });
        
        // Debtors Table
        const tbody = document.querySelector('#rec-debtors-table tbody');
        tbody.innerHTML = data.topDebtors.map(d => `
            <tr>
                <td><strong>${d.clientName}</strong></td>
                <td>${formatCurrency(d.totalOutstanding)}</td>
                <td class="text-danger">${formatCurrency(d.overdue)}</td>
                <td>${d.openInvoices}</td>
            </tr>
        `).join('');
        
    } catch (error) {
        console.error('Error loading finance receivables:', error);
    }
}
