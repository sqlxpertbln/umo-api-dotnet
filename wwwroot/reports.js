// UMO Reports Dashboard JavaScript
const API_BASE = '';
let charts = {};

// Initialize
document.addEventListener('DOMContentLoaded', function() {
    initNavigation();
    loadOverview();
    hideLoading();
});

function initNavigation() {
    document.querySelectorAll('.sidebar .nav-link[data-report]').forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const report = this.getAttribute('data-report');
            showReport(report);
        });
    });
}

function showReport(reportName) {
    // Update navigation
    document.querySelectorAll('.sidebar .nav-link').forEach(link => {
        link.classList.remove('active');
        if (link.getAttribute('data-report') === reportName) {
            link.classList.add('active');
        }
    });
    
    // Hide all sections
    document.querySelectorAll('.report-section').forEach(section => {
        section.style.display = 'none';
    });
    
    // Show selected section
    const section = document.getElementById(`${reportName}-section`);
    if (section) {
        section.style.display = 'block';
        loadReportData(reportName);
    }
}

function loadReportData(reportName) {
    showLoading();
    switch(reportName) {
        case 'overview':
            loadOverview();
            break;
        case 'marketing':
            loadMarketingDashboard();
            break;
        case 'sales':
            loadSalesDashboard();
            break;
        case 'operations':
            loadOperationsDashboard();
            break;
        case 'finance':
            loadFinanceDashboard();
            break;
        case 'controlling':
            loadControllingDashboard();
            break;
        case 'billing':
            loadBillingDashboard();
            break;
    }
}

function loadOverview() {
    hideLoading();
}

// ============================================
// MARKETING DASHBOARD
// ============================================

async function loadMarketingDashboard() {
    try {
        const response = await fetch(`${API_BASE}/reports/marketing`);
        const data = await response.json();
        
        // Update KPIs
        document.getElementById('marketing-total-clients').textContent = data.totalClients;
        document.getElementById('marketing-new-this-month').textContent = data.newClientsThisMonth;
        document.getElementById('marketing-new-last-month').textContent = data.newClientsLastMonth;
        document.getElementById('marketing-growth-rate').textContent = `${data.growthRatePercent}%`;
        
        // Trend Chart
        destroyChart('marketing-trend-chart');
        charts['marketing-trend-chart'] = new Chart(document.getElementById('marketing-trend-chart'), {
            type: 'line',
            data: {
                labels: data.clientAcquisitionTrend.map(t => t.month),
                datasets: [{
                    label: 'Neukunden',
                    data: data.clientAcquisitionTrend.map(t => t.count),
                    borderColor: '#667eea',
                    backgroundColor: 'rgba(102, 126, 234, 0.1)',
                    fill: true,
                    tension: 0.4
                }]
            },
            options: getLineChartOptions()
        });
        
        // Age Distribution Chart
        destroyChart('marketing-age-chart');
        if (data.clientsByAgeGroup && data.clientsByAgeGroup.length > 0) {
            charts['marketing-age-chart'] = new Chart(document.getElementById('marketing-age-chart'), {
                type: 'doughnut',
                data: {
                    labels: data.clientsByAgeGroup.map(a => a.ageGroup),
                    datasets: [{
                        data: data.clientsByAgeGroup.map(a => a.count),
                        backgroundColor: ['#667eea', '#764ba2', '#f093fb', '#f5576c', '#4facfe', '#00f2fe', '#11998e']
                    }]
                },
                options: getDoughnutChartOptions()
            });
        }
        
        // Region Chart
        destroyChart('marketing-region-chart');
        if (data.clientsByRegion && data.clientsByRegion.length > 0) {
            charts['marketing-region-chart'] = new Chart(document.getElementById('marketing-region-chart'), {
                type: 'bar',
                data: {
                    labels: data.clientsByRegion.map(r => r.city),
                    datasets: [{
                        label: 'Klienten',
                        data: data.clientsByRegion.map(r => r.clientCount),
                        backgroundColor: '#4facfe'
                    }]
                },
                options: getBarChartOptions()
            });
        }
        
        // Language Chart
        destroyChart('marketing-language-chart');
        if (data.clientsByLanguage && data.clientsByLanguage.length > 0) {
            charts['marketing-language-chart'] = new Chart(document.getElementById('marketing-language-chart'), {
                type: 'pie',
                data: {
                    labels: data.clientsByLanguage.map(l => l.language),
                    datasets: [{
                        data: data.clientsByLanguage.map(l => l.count),
                        backgroundColor: ['#11998e', '#38ef7d', '#667eea', '#f093fb', '#f5576c']
                    }]
                },
                options: getDoughnutChartOptions()
            });
        }
        
        hideLoading();
    } catch (error) {
        console.error('Error loading marketing dashboard:', error);
        hideLoading();
    }
}

// ============================================
// SALES DASHBOARD
// ============================================

async function loadSalesDashboard() {
    try {
        const response = await fetch(`${API_BASE}/reports/sales`);
        const data = await response.json();
        
        // Update KPIs
        document.getElementById('sales-total-devices').textContent = data.totalDevices;
        document.getElementById('sales-active-devices').textContent = data.activeDevices;
        document.getElementById('sales-inactive-devices').textContent = data.inactiveDevices;
        document.getElementById('sales-utilization').textContent = `${data.deviceUtilizationPercent}%`;
        
        // Device Type Chart
        destroyChart('sales-device-type-chart');
        if (data.devicesByType && data.devicesByType.length > 0) {
            charts['sales-device-type-chart'] = new Chart(document.getElementById('sales-device-type-chart'), {
                type: 'bar',
                data: {
                    labels: data.devicesByType.map(d => d.deviceType),
                    datasets: [
                        {
                            label: 'Aktiv',
                            data: data.devicesByType.map(d => d.active),
                            backgroundColor: '#10b981'
                        },
                        {
                            label: 'Inaktiv',
                            data: data.devicesByType.map(d => d.inactive),
                            backgroundColor: '#ef4444'
                        }
                    ]
                },
                options: getStackedBarChartOptions()
            });
        }
        
        // Tarif Chart
        destroyChart('sales-tarif-chart');
        if (data.clientsByTarif && data.clientsByTarif.length > 0) {
            charts['sales-tarif-chart'] = new Chart(document.getElementById('sales-tarif-chart'), {
                type: 'doughnut',
                data: {
                    labels: data.clientsByTarif.map(t => t.tarifName),
                    datasets: [{
                        data: data.clientsByTarif.map(t => t.clientCount),
                        backgroundColor: ['#667eea', '#764ba2', '#f093fb', '#4facfe', '#11998e']
                    }]
                },
                options: getDoughnutChartOptions()
            });
        }
        
        // Provider Table
        const tableBody = document.querySelector('#sales-provider-table tbody');
        tableBody.innerHTML = '';
        if (data.topProviders) {
            data.topProviders.forEach((provider, index) => {
                const row = document.createElement('tr');
                const statusClass = provider.performanceScore >= 80 ? 'status-good' : 
                                   provider.performanceScore >= 60 ? 'status-warning' : 'status-critical';
                row.innerHTML = `
                    <td><strong>#${index + 1}</strong></td>
                    <td>${provider.providerName}</td>
                    <td><span class="badge bg-secondary">${provider.providerType}</span></td>
                    <td>${provider.clientCount}</td>
                    <td>
                        <div class="progress-bar-custom" style="width: 100px;">
                            <div class="fill bg-primary" style="width: ${provider.performanceScore}%;"></div>
                        </div>
                        <small>${provider.performanceScore}%</small>
                    </td>
                    <td><span class="status-indicator ${statusClass}"></span>${getPerformanceLabel(provider.performanceScore)}</td>
                `;
                tableBody.appendChild(row);
            });
        }
        
        hideLoading();
    } catch (error) {
        console.error('Error loading sales dashboard:', error);
        hideLoading();
    }
}

// ============================================
// OPERATIONS DASHBOARD
// ============================================

async function loadOperationsDashboard() {
    try {
        const response = await fetch(`${API_BASE}/reports/operations`);
        const data = await response.json();
        
        // Update KPIs
        document.getElementById('ops-active-clients').textContent = data.totalActiveClients;
        document.getElementById('ops-inactive-clients').textContent = data.totalInactiveClients;
        document.getElementById('ops-with-devices').textContent = data.clientsWithDevices;
        document.getElementById('ops-capacity').textContent = `${data.capacityUtilizationPercent}%`;
        
        // Status Chart
        destroyChart('ops-status-chart');
        if (data.clientsByStatus && data.clientsByStatus.length > 0) {
            charts['ops-status-chart'] = new Chart(document.getElementById('ops-status-chart'), {
                type: 'doughnut',
                data: {
                    labels: data.clientsByStatus.map(s => s.status),
                    datasets: [{
                        data: data.clientsByStatus.map(s => s.count),
                        backgroundColor: data.clientsByStatus.map(s => s.colorCode || '#6c757d')
                    }]
                },
                options: getDoughnutChartOptions()
            });
        }
        
        // Priority Chart
        destroyChart('ops-priority-chart');
        if (data.clientsByPriority && data.clientsByPriority.length > 0) {
            charts['ops-priority-chart'] = new Chart(document.getElementById('ops-priority-chart'), {
                type: 'bar',
                data: {
                    labels: data.clientsByPriority.map(p => p.priority),
                    datasets: [{
                        label: 'Klienten',
                        data: data.clientsByPriority.map(p => p.count),
                        backgroundColor: ['#ef4444', '#f59e0b', '#10b981', '#6b7280']
                    }]
                },
                options: getBarChartOptions()
            });
        }
        
        // Workload Container
        const workloadContainer = document.getElementById('ops-workload-container');
        workloadContainer.innerHTML = '';
        if (data.providerWorkload) {
            data.providerWorkload.forEach(provider => {
                const colorClass = provider.workloadPercent >= 80 ? 'bg-danger' : 
                                  provider.workloadPercent >= 60 ? 'bg-warning' : 'bg-success';
                workloadContainer.innerHTML += `
                    <div class="metric-row">
                        <div>
                            <strong>${provider.providerName}</strong>
                            <br><small class="text-muted">${provider.assignedClients} / ${provider.maxCapacity} Klienten</small>
                        </div>
                        <div style="width: 200px;">
                            <div class="progress" style="height: 20px;">
                                <div class="progress-bar ${colorClass}" style="width: ${Math.min(provider.workloadPercent, 100)}%;">
                                    ${provider.workloadPercent}%
                                </div>
                            </div>
                        </div>
                    </div>
                `;
            });
        }
        
        hideLoading();
    } catch (error) {
        console.error('Error loading operations dashboard:', error);
        hideLoading();
    }
}

// ============================================
// FINANCE DASHBOARD
// ============================================

async function loadFinanceDashboard() {
    try {
        const response = await fetch(`${API_BASE}/reports/finance`);
        const data = await response.json();
        
        // Update KPIs
        document.getElementById('finance-monthly-revenue').textContent = formatCurrency(data.totalMonthlyRevenue);
        document.getElementById('finance-yearly-revenue').textContent = formatCurrency(data.totalYearlyRevenue);
        document.getElementById('finance-costs').textContent = formatCurrency(data.totalCosts);
        document.getElementById('finance-profit').textContent = formatCurrency(data.netProfit);
        document.getElementById('finance-margin').textContent = `${data.profitMarginPercent}%`;
        document.getElementById('finance-avg-revenue').textContent = formatCurrency(data.averageRevenuePerClient);
        
        // Trend Chart
        destroyChart('finance-trend-chart');
        if (data.monthlyFinanceTrend && data.monthlyFinanceTrend.length > 0) {
            charts['finance-trend-chart'] = new Chart(document.getElementById('finance-trend-chart'), {
                type: 'line',
                data: {
                    labels: data.monthlyFinanceTrend.map(t => t.month),
                    datasets: [
                        {
                            label: 'Umsatz',
                            data: data.monthlyFinanceTrend.map(t => t.revenue),
                            borderColor: '#10b981',
                            backgroundColor: 'rgba(16, 185, 129, 0.1)',
                            fill: true,
                            tension: 0.4
                        },
                        {
                            label: 'Kosten',
                            data: data.monthlyFinanceTrend.map(t => t.costs),
                            borderColor: '#ef4444',
                            backgroundColor: 'rgba(239, 68, 68, 0.1)',
                            fill: true,
                            tension: 0.4
                        },
                        {
                            label: 'Gewinn',
                            data: data.monthlyFinanceTrend.map(t => t.profit),
                            borderColor: '#667eea',
                            backgroundColor: 'rgba(102, 126, 234, 0.1)',
                            fill: true,
                            tension: 0.4
                        }
                    ]
                },
                options: getLineChartOptions()
            });
        }
        
        // Cost Breakdown Chart
        destroyChart('finance-cost-chart');
        if (data.costBreakdown && data.costBreakdown.length > 0) {
            charts['finance-cost-chart'] = new Chart(document.getElementById('finance-cost-chart'), {
                type: 'doughnut',
                data: {
                    labels: data.costBreakdown.map(c => c.costCategory),
                    datasets: [{
                        data: data.costBreakdown.map(c => c.amount),
                        backgroundColor: ['#667eea', '#764ba2', '#f093fb', '#4facfe', '#11998e']
                    }]
                },
                options: getDoughnutChartOptions()
            });
        }
        
        // Revenue by Tarif Chart
        destroyChart('finance-tarif-chart');
        if (data.revenueByTarif && data.revenueByTarif.length > 0) {
            charts['finance-tarif-chart'] = new Chart(document.getElementById('finance-tarif-chart'), {
                type: 'bar',
                data: {
                    labels: data.revenueByTarif.map(t => t.tarifName),
                    datasets: [{
                        label: 'Monatsumsatz (€)',
                        data: data.revenueByTarif.map(t => t.totalMonthlyRevenue),
                        backgroundColor: '#4facfe'
                    }]
                },
                options: getBarChartOptions()
            });
        }
        
        // Top Clients Table
        const tableBody = document.querySelector('#finance-top-clients-table tbody');
        tableBody.innerHTML = '';
        if (data.topClientsByRevenue) {
            data.topClientsByRevenue.forEach(client => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td><strong>${client.clientName}</strong></td>
                    <td><span class="badge bg-info">${client.tarifName}</span></td>
                    <td>${formatCurrency(client.monthlyRevenue)}</td>
                    <td>${formatCurrency(client.yearlyRevenue)}</td>
                `;
                tableBody.appendChild(row);
            });
        }
        
        hideLoading();
    } catch (error) {
        console.error('Error loading finance dashboard:', error);
        hideLoading();
    }
}

// ============================================
// CONTROLLING DASHBOARD
// ============================================

async function loadControllingDashboard() {
    try {
        const response = await fetch(`${API_BASE}/reports/controlling`);
        const data = await response.json();
        
        // KPI Cards
        const kpiContainer = document.getElementById('controlling-kpi-container');
        kpiContainer.innerHTML = '';
        if (data.keyPerformanceIndicators) {
            data.keyPerformanceIndicators.forEach(kpi => {
                const statusClass = kpi.status === 'good' ? 'status-good' : 
                                   kpi.status === 'warning' ? 'status-warning' : 'status-critical';
                const changeClass = kpi.changePercent >= 0 ? 'trend-up' : 'trend-down';
                const changeIcon = kpi.changePercent >= 0 ? 'bi-arrow-up' : 'bi-arrow-down';
                
                kpiContainer.innerHTML += `
                    <div class="col-md-4 mb-3">
                        <div class="card h-100">
                            <div class="card-body">
                                <div class="d-flex justify-content-between align-items-start">
                                    <div>
                                        <h6 class="text-muted mb-1">${kpi.name}</h6>
                                        <h3 class="mb-0">${formatKpiValue(kpi.currentValue, kpi.unit)}</h3>
                                    </div>
                                    <span class="status-indicator ${statusClass}"></span>
                                </div>
                                <div class="mt-2">
                                    <small class="${changeClass}">
                                        <i class="bi ${changeIcon}"></i> ${Math.abs(kpi.changePercent)}%
                                    </small>
                                    <small class="text-muted ms-2">Ziel: ${formatKpiValue(kpi.targetValue, kpi.unit)}</small>
                                </div>
                                <div class="progress mt-2" style="height: 4px;">
                                    <div class="progress-bar bg-primary" style="width: ${Math.min((kpi.currentValue / kpi.targetValue) * 100, 100)}%;"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
            });
        }
        
        // Year over Year Chart
        destroyChart('controlling-yoy-chart');
        if (data.yearOverYearComparison && data.yearOverYearComparison.length > 0) {
            charts['controlling-yoy-chart'] = new Chart(document.getElementById('controlling-yoy-chart'), {
                type: 'bar',
                data: {
                    labels: data.yearOverYearComparison.map(y => y.metric),
                    datasets: [
                        {
                            label: 'Vorjahr',
                            data: data.yearOverYearComparison.map(y => y.previousYear),
                            backgroundColor: '#94a3b8'
                        },
                        {
                            label: 'Aktuelles Jahr',
                            data: data.yearOverYearComparison.map(y => y.currentYear),
                            backgroundColor: '#667eea'
                        }
                    ]
                },
                options: getBarChartOptions()
            });
        }
        
        // Benchmark Chart
        destroyChart('controlling-benchmark-chart');
        if (data.benchmarks && data.benchmarks.length > 0) {
            charts['controlling-benchmark-chart'] = new Chart(document.getElementById('controlling-benchmark-chart'), {
                type: 'bar',
                data: {
                    labels: data.benchmarks.map(b => b.category),
                    datasets: [
                        {
                            label: 'Benchmark',
                            data: data.benchmarks.map(b => b.benchmarkValue),
                            backgroundColor: '#94a3b8'
                        },
                        {
                            label: 'Aktuell',
                            data: data.benchmarks.map(b => b.actualValue),
                            backgroundColor: '#10b981'
                        }
                    ]
                },
                options: getBarChartOptions()
            });
        }
        
        // Efficiency Table
        const efficiencyTable = document.querySelector('#controlling-efficiency-table tbody');
        efficiencyTable.innerHTML = '';
        if (data.efficiencyMetrics) {
            data.efficiencyMetrics.forEach(metric => {
                const trendIcon = metric.trend === 'up' ? 'bi-arrow-up text-success' : 
                                 metric.trend === 'down' ? 'bi-arrow-down text-danger' : 'bi-dash text-muted';
                efficiencyTable.innerHTML += `
                    <tr>
                        <td><strong>${metric.metricName}</strong></td>
                        <td>${metric.value}</td>
                        <td>${metric.unit}</td>
                        <td><i class="bi ${trendIcon}"></i></td>
                    </tr>
                `;
            });
        }
        
        hideLoading();
    } catch (error) {
        console.error('Error loading controlling dashboard:', error);
        hideLoading();
    }
}

// ============================================
// BILLING DASHBOARD
// ============================================

async function loadBillingDashboard() {
    try {
        const response = await fetch(`${API_BASE}/reports/billing`);
        const data = await response.json();
        
        // Update KPIs
        document.getElementById('billing-total-billed').textContent = formatCurrencyShort(data.totalBilled);
        document.getElementById('billing-total-paid').textContent = formatCurrencyShort(data.totalPaid);
        document.getElementById('billing-outstanding').textContent = formatCurrencyShort(data.totalOutstanding);
        document.getElementById('billing-invoice-count').textContent = data.invoiceCount;
        document.getElementById('billing-overdue').textContent = data.overdueInvoices;
        document.getElementById('billing-collection-rate').textContent = `${data.collectionRatePercent}%`;
        
        // Payment Status Chart
        destroyChart('billing-status-chart');
        if (data.paymentStatus && data.paymentStatus.length > 0) {
            charts['billing-status-chart'] = new Chart(document.getElementById('billing-status-chart'), {
                type: 'doughnut',
                data: {
                    labels: data.paymentStatus.map(s => s.status),
                    datasets: [{
                        data: data.paymentStatus.map(s => s.amount),
                        backgroundColor: ['#10b981', '#f59e0b', '#ef4444']
                    }]
                },
                options: getDoughnutChartOptions()
            });
        }
        
        // Aging Chart
        destroyChart('billing-aging-chart');
        if (data.agingReport && data.agingReport.length > 0) {
            charts['billing-aging-chart'] = new Chart(document.getElementById('billing-aging-chart'), {
                type: 'bar',
                data: {
                    labels: data.agingReport.map(a => a.agingBucket),
                    datasets: [{
                        label: 'Betrag (€)',
                        data: data.agingReport.map(a => a.amount),
                        backgroundColor: ['#10b981', '#84cc16', '#f59e0b', '#f97316', '#ef4444']
                    }]
                },
                options: getBarChartOptions()
            });
        }
        
        // Billing Trend Chart
        destroyChart('billing-trend-chart');
        if (data.monthlyBillingTrend && data.monthlyBillingTrend.length > 0) {
            charts['billing-trend-chart'] = new Chart(document.getElementById('billing-trend-chart'), {
                type: 'line',
                data: {
                    labels: data.monthlyBillingTrend.map(t => t.month),
                    datasets: [
                        {
                            label: 'Abgerechnet',
                            data: data.monthlyBillingTrend.map(t => t.billed),
                            borderColor: '#667eea',
                            tension: 0.4
                        },
                        {
                            label: 'Eingezogen',
                            data: data.monthlyBillingTrend.map(t => t.collected),
                            borderColor: '#10b981',
                            tension: 0.4
                        },
                        {
                            label: 'Ausstehend',
                            data: data.monthlyBillingTrend.map(t => t.outstanding),
                            borderColor: '#ef4444',
                            tension: 0.4
                        }
                    ]
                },
                options: getLineChartOptions()
            });
        }
        
        // Invoice Method Chart
        destroyChart('billing-method-chart');
        if (data.invoicesByMethod && data.invoicesByMethod.length > 0) {
            charts['billing-method-chart'] = new Chart(document.getElementById('billing-method-chart'), {
                type: 'pie',
                data: {
                    labels: data.invoicesByMethod.map(m => m.method),
                    datasets: [{
                        data: data.invoicesByMethod.map(m => m.count),
                        backgroundColor: ['#667eea', '#764ba2', '#f093fb', '#4facfe']
                    }]
                },
                options: getDoughnutChartOptions()
            });
        }
        
        // Outstanding Clients Table
        const tableBody = document.querySelector('#billing-outstanding-table tbody');
        tableBody.innerHTML = '';
        if (data.topOutstandingClients) {
            data.topOutstandingClients.forEach(client => {
                const statusClass = client.daysOverdue > 60 ? 'bg-danger' : 
                                   client.daysOverdue > 30 ? 'bg-warning' : 'bg-info';
                tableBody.innerHTML += `
                    <tr>
                        <td><strong>${client.clientName}</strong></td>
                        <td>${formatCurrency(client.totalBilled)}</td>
                        <td>${formatCurrency(client.totalPaid)}</td>
                        <td class="text-danger">${formatCurrency(client.outstanding)}</td>
                        <td>${client.daysOverdue}</td>
                        <td><span class="badge ${statusClass}">${client.daysOverdue > 60 ? 'Kritisch' : client.daysOverdue > 30 ? 'Überfällig' : 'Ausstehend'}</span></td>
                    </tr>
                `;
            });
        }
        
        hideLoading();
    } catch (error) {
        console.error('Error loading billing dashboard:', error);
        hideLoading();
    }
}

// ============================================
// CHART OPTIONS
// ============================================

function getLineChartOptions() {
    return {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                position: 'bottom'
            }
        },
        scales: {
            y: {
                beginAtZero: true,
                grid: {
                    color: '#f1f5f9'
                }
            },
            x: {
                grid: {
                    display: false
                }
            }
        }
    };
}

function getBarChartOptions() {
    return {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                position: 'bottom'
            }
        },
        scales: {
            y: {
                beginAtZero: true,
                grid: {
                    color: '#f1f5f9'
                }
            },
            x: {
                grid: {
                    display: false
                }
            }
        }
    };
}

function getStackedBarChartOptions() {
    return {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                position: 'bottom'
            }
        },
        scales: {
            x: {
                stacked: true,
                grid: {
                    display: false
                }
            },
            y: {
                stacked: true,
                beginAtZero: true,
                grid: {
                    color: '#f1f5f9'
                }
            }
        }
    };
}

function getDoughnutChartOptions() {
    return {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                position: 'bottom'
            }
        }
    };
}

// ============================================
// UTILITY FUNCTIONS
// ============================================

function destroyChart(chartId) {
    if (charts[chartId]) {
        charts[chartId].destroy();
        delete charts[chartId];
    }
}

function formatCurrency(value) {
    return new Intl.NumberFormat('de-DE', {
        style: 'currency',
        currency: 'EUR',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
    }).format(value);
}

function formatCurrencyShort(value) {
    if (value >= 1000000) {
        return (value / 1000000).toFixed(1) + 'M €';
    } else if (value >= 1000) {
        return (value / 1000).toFixed(1) + 'K €';
    }
    return formatCurrency(value);
}

function formatKpiValue(value, unit) {
    if (unit === 'EUR') {
        return formatCurrency(value);
    } else if (unit === '%') {
        return value + '%';
    }
    return value + ' ' + unit;
}

function getPerformanceLabel(score) {
    if (score >= 80) return 'Ausgezeichnet';
    if (score >= 60) return 'Gut';
    return 'Verbesserungsbedarf';
}

function showLoading() {
    document.getElementById('loading-overlay').style.display = 'flex';
}

function hideLoading() {
    document.getElementById('loading-overlay').style.display = 'none';
}
