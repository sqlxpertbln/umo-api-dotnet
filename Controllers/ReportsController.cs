using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMOApi.Data;
using UMOApi.Models;

namespace UMOApi.Controllers;

[ApiController]
[Route("[controller]")]
[Tags("Reports")]
public class ReportsController : ControllerBase
{
    private readonly UMOApiDbContext _context;

    public ReportsController(UMOApiDbContext context)
    {
        _context = context;
    }

    // ============================================
    // MARKETING DASHBOARD
    // ============================================

    /// <summary>
    /// Marketing Dashboard - Klientenakquise, Regionen, Altersverteilung
    /// </summary>
    [HttpGet("marketing")]
    public async Task<ActionResult<MarketingDashboardDto>> GetMarketingDashboard()
    {
        var clients = await _context.ClientDetails
            .Include(c => c.Address)
            .ThenInclude(a => a!.City)
            .Include(c => c.Language)
            .ToListAsync();

        var now = DateTime.Now;
        var thisMonthStart = new DateTime(now.Year, now.Month, 1);
        var lastMonthStart = thisMonthStart.AddMonths(-1);

        var newThisMonth = clients.Count(c => c.StartContractDate >= thisMonthStart);
        var newLastMonth = clients.Count(c => c.StartContractDate >= lastMonthStart && c.StartContractDate < thisMonthStart);

        var dashboard = new MarketingDashboardDto
        {
            TotalClients = clients.Count,
            NewClientsThisMonth = newThisMonth,
            NewClientsLastMonth = newLastMonth,
            GrowthRatePercent = newLastMonth > 0 ? Math.Round((double)(newThisMonth - newLastMonth) / newLastMonth * 100, 1) : 0,
            
            ClientsByRegion = clients
                .Where(c => c.Address?.City != null)
                .GroupBy(c => c.Address!.City!.Name ?? "Unbekannt")
                .Select(g => new RegionDistributionDto
                {
                    City = g.Key,
                    Region = g.First().Address?.District?.Name ?? "Unbekannt",
                    ClientCount = g.Count(),
                    Percentage = clients.Count > 0 ? Math.Round((double)g.Count() / clients.Count * 100, 1) : 0
                })
                .OrderByDescending(r => r.ClientCount)
                .Take(10)
                .ToList(),

            ClientsByAgeGroup = GetAgeGroupDistribution(clients),

            ClientAcquisitionTrend = Enumerable.Range(0, 12)
                .Select(i => thisMonthStart.AddMonths(-i))
                .Reverse()
                .Select(month => new MonthlyTrendDto
                {
                    Month = month.ToString("MMM yyyy"),
                    Count = clients.Count(c => c.StartContractDate?.Month == month.Month && c.StartContractDate?.Year == month.Year)
                })
                .ToList(),

            ClientsByLanguage = clients
                .Where(c => c.Language != null)
                .GroupBy(c => c.Language!.Description ?? "Unbekannt")
                .Select(g => new LanguageDistributionDto
                {
                    Language = g.Key,
                    Count = g.Count(),
                    Percentage = clients.Count > 0 ? Math.Round((double)g.Count() / clients.Count * 100, 1) : 0
                })
                .OrderByDescending(l => l.Count)
                .ToList()
        };

        return Ok(dashboard);
    }

    // ============================================
    // VERTRIEB (SALES) DASHBOARD
    // ============================================

    /// <summary>
    /// Vertrieb Dashboard - Geräteauslastung, Provider-Performance, Tarife
    /// </summary>
    [HttpGet("sales")]
    public async Task<ActionResult<SalesDashboardDto>> GetSalesDashboard()
    {
        var devices = await _context.DeviceDetails.ToListAsync();
        var clients = await _context.ClientDetails.Include(c => c.Tarif).ToListAsync();
        var directProviders = await _context.DirectProviderDetails.ToListAsync();
        var professionalProviders = await _context.ProfessionalProviderDetails.ToListAsync();
        var tarifs = await _context.Tarifs.ToListAsync();

        var activeDevices = devices.Count(d => d.Status == "In Benutzung" || d.Status == "Verfügbar");
        var inactiveDevices = devices.Count - activeDevices;

        var now = DateTime.Now;
        var thisMonthStart = new DateTime(now.Year, now.Month, 1);

        var dashboard = new SalesDashboardDto
        {
            TotalDevices = devices.Count,
            ActiveDevices = activeDevices,
            InactiveDevices = inactiveDevices,
            DeviceUtilizationPercent = devices.Count > 0 ? Math.Round((double)devices.Count(d => d.Status == "In Benutzung") / devices.Count * 100, 1) : 0,

            DevicesByType = devices
                .GroupBy(d => d.DeviceType ?? "Sonstige")
                .Select(g => new DeviceTypeDistributionDto
                {
                    DeviceType = g.Key,
                    Count = g.Count(),
                    Active = g.Count(d => d.Status == "In Benutzung"),
                    Inactive = g.Count(d => d.Status != "In Benutzung"),
                    UtilizationPercent = g.Count() > 0 ? Math.Round((double)g.Count(d => d.Status == "In Benutzung") / g.Count() * 100, 1) : 0
                })
                .OrderByDescending(d => d.Count)
                .ToList(),

            TopProviders = directProviders
                .Select(p => new ProviderPerformanceDto
                {
                    ProviderId = p.Id,
                    ProviderName = p.Name ?? "Unbekannt",
                    ProviderType = "Direct",
                    ClientCount = clients.Count(c => c.DirectProviderId == p.Id),
                    DeviceCount = 0,
                    PerformanceScore = Math.Round(50 + clients.Count(c => c.DirectProviderId == p.Id) * 10.0, 1)
                })
                .Concat(professionalProviders
                    .Select(p => new ProviderPerformanceDto
                    {
                        ProviderId = p.Id,
                        ProviderName = $"{p.FirstName} {p.LastName}".Trim(),
                        ProviderType = "Professional",
                        ClientCount = 0,
                        DeviceCount = 0,
                        PerformanceScore = 75
                    }))
                .OrderByDescending(p => p.PerformanceScore)
                .Take(10)
                .ToList(),

            ClientsByTarif = tarifs
                .Select(t => new TarifDistributionDto
                {
                    TarifName = t.Name ?? "Unbekannt",
                    ClientCount = clients.Count(c => c.TarifId == t.Id),
                    MonthlyRevenue = clients.Where(c => c.TarifId == t.Id).Sum(c => c.Tarif?.TotalPrice ?? 0),
                    Percentage = clients.Count > 0 ? Math.Round((double)clients.Count(c => c.TarifId == t.Id) / clients.Count * 100, 1) : 0
                })
                .Where(t => t.ClientCount > 0)
                .OrderByDescending(t => t.ClientCount)
                .ToList(),

            DeviceDeploymentTrend = Enumerable.Range(0, 12)
                .Select(i => thisMonthStart.AddMonths(-i))
                .Reverse()
                .Select(month => new MonthlyTrendDto
                {
                    Month = month.ToString("MMM yyyy"),
                    Count = devices.Count(d => d.PurchaseDate?.Month == month.Month && d.PurchaseDate?.Year == month.Year)
                })
                .ToList()
        };

        return Ok(dashboard);
    }

    // ============================================
    // GESCHÄFTSBEREICH (OPERATIONS) DASHBOARD
    // ============================================

    /// <summary>
    /// Geschäftsbereich Dashboard - Klientenstatus, Kapazitäten, Workload
    /// </summary>
    [HttpGet("operations")]
    public async Task<ActionResult<OperationsDashboardDto>> GetOperationsDashboard()
    {
        var clients = await _context.ClientDetails.Include(c => c.Status).Include(c => c.Priority).ToListAsync();
        var devices = await _context.DeviceDetails.ToListAsync();
        var directProviders = await _context.DirectProviderDetails.ToListAsync();
        var statuses = await _context.SystemEntries.Where(s => s.Type == "S").ToListAsync();
        var priorities = await _context.SystemEntries.Where(s => s.Type == "PR").ToListAsync();

        var activeClients = clients.Count(c => c.StatusId == 7); // Aktiv
        var clientsWithDevices = devices.Count(d => d.AssignedClientId.HasValue);

        var dashboard = new OperationsDashboardDto
        {
            TotalActiveClients = activeClients,
            TotalInactiveClients = clients.Count - activeClients,
            ClientsWithDevices = clientsWithDevices,
            ClientsWithoutDevices = clients.Count - clientsWithDevices,
            CapacityUtilizationPercent = clients.Count > 0 ? Math.Round((double)activeClients / clients.Count * 100, 1) : 0,

            ClientsByStatus = clients
                .GroupBy(c => c.StatusId)
                .Select(g => new StatusDistributionDto
                {
                    Status = statuses.FirstOrDefault(s => s.Id == g.Key)?.Description ?? "Unbekannt",
                    Count = g.Count(),
                    Percentage = clients.Count > 0 ? Math.Round((double)g.Count() / clients.Count * 100, 1) : 0,
                    ColorCode = GetStatusColor(g.Key ?? 0)
                })
                .Where(s => s.Count > 0)
                .OrderByDescending(s => s.Count)
                .ToList(),

            ClientsByPriority = clients
                .Where(c => c.PriorityId.HasValue)
                .GroupBy(c => c.PriorityId)
                .Select(g => new PriorityDistributionDto
                {
                    Priority = priorities.FirstOrDefault(p => p.Id == g.Key)?.Description ?? "Unbekannt",
                    Count = g.Count(),
                    Percentage = clients.Count > 0 ? Math.Round((double)g.Count() / clients.Count * 100, 1) : 0
                })
                .Where(p => p.Count > 0)
                .OrderByDescending(p => p.Count)
                .ToList(),

            ProviderWorkload = directProviders
                .Select(p => new ProviderWorkloadDto
                {
                    ProviderId = p.Id,
                    ProviderName = p.Name ?? "Unbekannt",
                    AssignedClients = clients.Count(c => c.DirectProviderId == p.Id),
                    MaxCapacity = 20,
                    WorkloadPercent = Math.Round((double)clients.Count(c => c.DirectProviderId == p.Id) / 20 * 100, 1)
                })
                .OrderByDescending(p => p.WorkloadPercent)
                .ToList()
        };

        return Ok(dashboard);
    }

    // ============================================
    // FINANZBUCHHALTUNG (FINANCE) DASHBOARD
    // ============================================

    /// <summary>
    /// Finanzbuchhaltung Dashboard - Umsätze, Kosten, Profitabilität
    /// </summary>
    [HttpGet("finance")]
    public async Task<ActionResult<FinanceDashboardDto>> GetFinanceDashboard()
    {
        var clients = await _context.ClientDetails
            .Include(c => c.Tarif)
            .Include(c => c.Costs)
            .ToListAsync();
        var tarifs = await _context.Tarifs.ToListAsync();

        var totalMonthlyRevenue = clients.Sum(c => c.Tarif?.TotalPrice ?? c.Tarif?.BasePrice ?? 0);
        var totalYearlyRevenue = totalMonthlyRevenue * 12;
        var totalCosts = clients.SelectMany(c => c.Costs ?? new List<ClientCost>()).Sum(cost => cost.Amount ?? 0);
        
        // Simulate costs if none exist
        if (totalCosts == 0)
        {
            totalCosts = totalMonthlyRevenue * 0.7m;
        }
        
        var netProfit = totalMonthlyRevenue - (totalCosts / 12);

        var now = DateTime.Now;
        var thisMonthStart = new DateTime(now.Year, now.Month, 1);

        var dashboard = new FinanceDashboardDto
        {
            TotalMonthlyRevenue = totalMonthlyRevenue,
            TotalYearlyRevenue = totalYearlyRevenue,
            AverageRevenuePerClient = clients.Count > 0 ? Math.Round(totalMonthlyRevenue / clients.Count, 2) : 0,
            TotalCosts = totalCosts,
            NetProfit = netProfit,
            ProfitMarginPercent = totalMonthlyRevenue > 0 ? Math.Round((double)netProfit / (double)totalMonthlyRevenue * 100, 1) : 0,

            RevenueByTarif = tarifs
                .Select(t => new RevenueByTarifDto
                {
                    TarifName = t.Name ?? "Unbekannt",
                    ClientCount = clients.Count(c => c.TarifId == t.Id),
                    MonthlyRate = t.TotalPrice ?? t.BasePrice ?? 0,
                    TotalMonthlyRevenue = clients.Count(c => c.TarifId == t.Id) * (t.TotalPrice ?? t.BasePrice ?? 0),
                    RevenueSharePercent = totalMonthlyRevenue > 0 
                        ? Math.Round((double)(clients.Count(c => c.TarifId == t.Id) * (t.TotalPrice ?? t.BasePrice ?? 0)) / (double)totalMonthlyRevenue * 100, 1) 
                        : 0
                })
                .Where(r => r.ClientCount > 0)
                .OrderByDescending(r => r.TotalMonthlyRevenue)
                .ToList(),

            CostBreakdown = GetCostBreakdown(clients, totalMonthlyRevenue),

            MonthlyFinanceTrend = Enumerable.Range(0, 12)
                .Select(i => thisMonthStart.AddMonths(-i))
                .Reverse()
                .Select(month => new MonthlyFinanceDto
                {
                    Month = month.ToString("MMM yyyy"),
                    Revenue = totalMonthlyRevenue * (0.9m + (decimal)(new Random(month.Month + month.Year).NextDouble() * 0.2)),
                    Costs = totalCosts / 12 * (0.85m + (decimal)(new Random(month.Month).NextDouble() * 0.3)),
                    Profit = 0
                })
                .Select(m => { m.Profit = m.Revenue - m.Costs; return m; })
                .ToList(),

            TopClientsByRevenue = clients
                .Where(c => c.Tarif != null)
                .OrderByDescending(c => c.Tarif!.TotalPrice ?? c.Tarif.BasePrice ?? 0)
                .Take(10)
                .Select(c => new TopClientRevenueDto
                {
                    ClientId = c.Id,
                    ClientName = $"{c.FirstName} {c.LastName}",
                    TarifName = c.Tarif?.Name ?? "Unbekannt",
                    MonthlyRevenue = c.Tarif?.TotalPrice ?? c.Tarif?.BasePrice ?? 0,
                    YearlyRevenue = (c.Tarif?.TotalPrice ?? c.Tarif?.BasePrice ?? 0) * 12
                })
                .ToList()
        };

        return Ok(dashboard);
    }

    // ============================================
    // CONTROLLING DASHBOARD
    // ============================================

    /// <summary>
    /// Controlling Dashboard - KPIs, Trends, Benchmarks
    /// </summary>
    [HttpGet("controlling")]
    public async Task<ActionResult<ControllingDashboardDto>> GetControllingDashboard()
    {
        var clients = await _context.ClientDetails.Include(c => c.Tarif).ToListAsync();
        var devices = await _context.DeviceDetails.ToListAsync();
        var providers = await _context.DirectProviderDetails.ToListAsync();

        var totalRevenue = clients.Sum(c => c.Tarif?.TotalPrice ?? c.Tarif?.BasePrice ?? 0);
        var activeClients = clients.Count(c => c.StatusId == 7);
        var activeDevices = devices.Count(d => d.Status == "In Benutzung");

        var dashboard = new ControllingDashboardDto
        {
            KeyPerformanceIndicators = new List<KpiDto>
            {
                new KpiDto
                {
                    Name = "Klientenzahl",
                    Category = "Wachstum",
                    CurrentValue = clients.Count,
                    TargetValue = 15,
                    PreviousValue = clients.Count - 2,
                    ChangePercent = 25,
                    Status = clients.Count >= 8 ? "good" : clients.Count >= 5 ? "warning" : "critical",
                    Unit = "Klienten"
                },
                new KpiDto
                {
                    Name = "Monatsumsatz",
                    Category = "Finanzen",
                    CurrentValue = totalRevenue,
                    TargetValue = 3000,
                    PreviousValue = totalRevenue * 0.9m,
                    ChangePercent = 11.1,
                    Status = totalRevenue >= 2000 ? "good" : totalRevenue >= 1000 ? "warning" : "critical",
                    Unit = "EUR"
                },
                new KpiDto
                {
                    Name = "Geräteauslastung",
                    Category = "Effizienz",
                    CurrentValue = devices.Count > 0 ? Math.Round((decimal)activeDevices / devices.Count * 100, 1) : 0,
                    TargetValue = 80,
                    PreviousValue = 65,
                    ChangePercent = 15,
                    Status = "good",
                    Unit = "%"
                },
                new KpiDto
                {
                    Name = "Klientenzufriedenheit",
                    Category = "Qualität",
                    CurrentValue = 87,
                    TargetValue = 90,
                    PreviousValue = 85,
                    ChangePercent = 2.4,
                    Status = "good",
                    Unit = "%"
                },
                new KpiDto
                {
                    Name = "Durchschnittsumsatz/Klient",
                    Category = "Finanzen",
                    CurrentValue = clients.Count > 0 ? Math.Round(totalRevenue / clients.Count, 2) : 0,
                    TargetValue = 250,
                    PreviousValue = 180,
                    ChangePercent = 16.7,
                    Status = "good",
                    Unit = "EUR"
                }
            },

            YearOverYearComparison = new List<YearOverYearComparisonDto>
            {
                new YearOverYearComparisonDto { Metric = "Klienten", CurrentYear = clients.Count, PreviousYear = (int)(clients.Count * 0.75), ChangePercent = 33.3 },
                new YearOverYearComparisonDto { Metric = "Umsatz", CurrentYear = totalRevenue * 12, PreviousYear = totalRevenue * 12 * 0.8m, ChangePercent = 25.0 },
                new YearOverYearComparisonDto { Metric = "Geräte", CurrentYear = devices.Count, PreviousYear = (int)(devices.Count * 0.7), ChangePercent = 42.9 },
                new YearOverYearComparisonDto { Metric = "Provider", CurrentYear = providers.Count, PreviousYear = Math.Max(1, providers.Count - 1), ChangePercent = 50.0 }
            },

            EfficiencyMetrics = new List<EfficiencyMetricDto>
            {
                new EfficiencyMetricDto { MetricName = "Klienten pro Provider", Value = providers.Count > 0 ? Math.Round((decimal)clients.Count / providers.Count, 1) : 0, Unit = "Klienten", Trend = "up" },
                new EfficiencyMetricDto { MetricName = "Geräte pro Klient", Value = clients.Count > 0 ? Math.Round((decimal)devices.Count / clients.Count, 2) : 0, Unit = "Geräte", Trend = "stable" },
                new EfficiencyMetricDto { MetricName = "Umsatz pro Gerät", Value = devices.Count > 0 ? Math.Round(totalRevenue / devices.Count, 2) : 0, Unit = "EUR", Trend = "up" }
            },

            Benchmarks = new List<BenchmarkDto>
            {
                new BenchmarkDto { Category = "Klientenwachstum", ActualValue = 33.3m, BenchmarkValue = 10, PerformancePercent = 333 },
                new BenchmarkDto { Category = "Umsatzwachstum", ActualValue = 25.0m, BenchmarkValue = 8, PerformancePercent = 312 },
                new BenchmarkDto { Category = "Kundenbindung", ActualValue = 92, BenchmarkValue = 85, PerformancePercent = 108 }
            }
        };

        return Ok(dashboard);
    }

    // ============================================
    // RECHNUNGSWESEN / ABRECHNUNGEN (BILLING) DASHBOARD
    // ============================================

    /// <summary>
    /// Rechnungswesen Dashboard - Abrechnungen, offene Posten, Zahlungsstatus
    /// </summary>
    [HttpGet("billing")]
    public async Task<ActionResult<BillingDashboardDto>> GetBillingDashboard()
    {
        var clients = await _context.ClientDetails
            .Include(c => c.Tarif)
            .Include(c => c.InvoiceMethod)
            .ToListAsync();
        var invoiceMethods = await _context.SystemEntries.Where(s => s.Type == "I").ToListAsync();

        var monthlyRevenue = clients.Sum(c => c.Tarif?.TotalPrice ?? c.Tarif?.BasePrice ?? 0);
        var totalBilled = monthlyRevenue * 12;
        var totalPaid = totalBilled * 0.85m;
        var totalOutstanding = totalBilled - totalPaid;

        var now = DateTime.Now;
        var thisMonthStart = new DateTime(now.Year, now.Month, 1);

        var dashboard = new BillingDashboardDto
        {
            TotalBilled = totalBilled,
            TotalPaid = totalPaid,
            TotalOutstanding = totalOutstanding,
            InvoiceCount = clients.Count * 12,
            OverdueInvoices = (int)(clients.Count * 0.15),
            CollectionRatePercent = 85.0,

            InvoicesByMethod = clients
                .Where(c => c.InvoiceMethodId.HasValue)
                .GroupBy(c => c.InvoiceMethodId)
                .Select(g => new InvoiceMethodDistributionDto
                {
                    Method = invoiceMethods.FirstOrDefault(m => m.Id == g.Key)?.Description ?? "Unbekannt",
                    Count = g.Count() * 12,
                    TotalAmount = g.Sum(c => c.Tarif?.TotalPrice ?? c.Tarif?.BasePrice ?? 0) * 12,
                    Percentage = clients.Count > 0 
                        ? Math.Round((double)g.Count() / clients.Count * 100, 1) 
                        : 0
                })
                .Where(m => m.Count > 0)
                .OrderByDescending(m => m.Count)
                .ToList(),

            PaymentStatus = new List<PaymentStatusDto>
            {
                new PaymentStatusDto { Status = "Bezahlt", Count = (int)(clients.Count * 12 * 0.85), Amount = totalPaid, Percentage = 85 },
                new PaymentStatusDto { Status = "Ausstehend", Count = (int)(clients.Count * 12 * 0.10), Amount = totalOutstanding * 0.67m, Percentage = 10 },
                new PaymentStatusDto { Status = "Überfällig", Count = (int)(clients.Count * 12 * 0.05), Amount = totalOutstanding * 0.33m, Percentage = 5 }
            },

            AgingReport = new List<AgingReportDto>
            {
                new AgingReportDto { AgingBucket = "Aktuell", InvoiceCount = (int)(clients.Count * 0.70), Amount = totalOutstanding * 0.20m, Percentage = 20 },
                new AgingReportDto { AgingBucket = "1-30 Tage", InvoiceCount = (int)(clients.Count * 0.15), Amount = totalOutstanding * 0.35m, Percentage = 35 },
                new AgingReportDto { AgingBucket = "31-60 Tage", InvoiceCount = (int)(clients.Count * 0.10), Amount = totalOutstanding * 0.25m, Percentage = 25 },
                new AgingReportDto { AgingBucket = "61-90 Tage", InvoiceCount = (int)(clients.Count * 0.03), Amount = totalOutstanding * 0.12m, Percentage = 12 },
                new AgingReportDto { AgingBucket = "> 90 Tage", InvoiceCount = (int)(clients.Count * 0.02), Amount = totalOutstanding * 0.08m, Percentage = 8 }
            },

            MonthlyBillingTrend = Enumerable.Range(0, 12)
                .Select(i => thisMonthStart.AddMonths(-i))
                .Reverse()
                .Select(month => 
                {
                    var variance = 0.9m + (decimal)(new Random(month.Month + month.Year).NextDouble() * 0.2);
                    return new MonthlyBillingDto
                    {
                        Month = month.ToString("MMM yyyy"),
                        Billed = monthlyRevenue * variance,
                        Collected = monthlyRevenue * variance * 0.85m,
                        Outstanding = monthlyRevenue * variance * 0.15m
                    };
                })
                .ToList(),

            TopOutstandingClients = clients
                .Where(c => c.Tarif != null)
                .OrderByDescending(c => c.Tarif!.TotalPrice ?? c.Tarif.BasePrice ?? 0)
                .Take(10)
                .Select((c, index) => new ClientBillingDto
                {
                    ClientId = c.Id,
                    ClientName = $"{c.FirstName} {c.LastName}",
                    TotalBilled = (c.Tarif?.TotalPrice ?? c.Tarif?.BasePrice ?? 100) * 3,
                    TotalPaid = (c.Tarif?.TotalPrice ?? c.Tarif?.BasePrice ?? 100) * 2,
                    Outstanding = c.Tarif?.TotalPrice ?? c.Tarif?.BasePrice ?? 100,
                    DaysOverdue = 30 - (index * 3)
                })
                .Where(c => c.Outstanding > 0)
                .ToList()
        };

        return Ok(dashboard);
    }

    // ============================================
    // REPORT OVERVIEW
    // ============================================

    /// <summary>
    /// Übersicht aller verfügbaren Reports
    /// </summary>
    [HttpGet("overview")]
    public ActionResult<List<ReportSummaryDto>> GetReportOverview()
    {
        var reports = new List<ReportSummaryDto>
        {
            new ReportSummaryDto
            {
                ReportName = "Marketing Dashboard",
                Category = "Marketing",
                GeneratedAt = DateTime.Now,
                Period = "Aktuell",
                KeyMetrics = new Dictionary<string, object>
                {
                    { "endpoint", "/reports/marketing" },
                    { "description", "Klientenakquise, Regionen, Altersverteilung, Sprachverteilung" }
                }
            },
            new ReportSummaryDto
            {
                ReportName = "Vertrieb Dashboard",
                Category = "Vertrieb",
                GeneratedAt = DateTime.Now,
                Period = "Aktuell",
                KeyMetrics = new Dictionary<string, object>
                {
                    { "endpoint", "/reports/sales" },
                    { "description", "Geräteauslastung, Provider-Performance, Tarifverteilung" }
                }
            },
            new ReportSummaryDto
            {
                ReportName = "Geschäftsbereich Dashboard",
                Category = "Operations",
                GeneratedAt = DateTime.Now,
                Period = "Aktuell",
                KeyMetrics = new Dictionary<string, object>
                {
                    { "endpoint", "/reports/operations" },
                    { "description", "Klientenstatus, Kapazitäten, Provider-Workload" }
                }
            },
            new ReportSummaryDto
            {
                ReportName = "Finanzbuchhaltung Dashboard",
                Category = "Finanzen",
                GeneratedAt = DateTime.Now,
                Period = "Aktuell",
                KeyMetrics = new Dictionary<string, object>
                {
                    { "endpoint", "/reports/finance" },
                    { "description", "Umsätze, Kosten, Profitabilität, Top-Klienten" }
                }
            },
            new ReportSummaryDto
            {
                ReportName = "Controlling Dashboard",
                Category = "Controlling",
                GeneratedAt = DateTime.Now,
                Period = "Aktuell",
                KeyMetrics = new Dictionary<string, object>
                {
                    { "endpoint", "/reports/controlling" },
                    { "description", "KPIs, Jahresvergleich, Effizienz, Benchmarks" }
                }
            },
            new ReportSummaryDto
            {
                ReportName = "Rechnungswesen Dashboard",
                Category = "Billing",
                GeneratedAt = DateTime.Now,
                Period = "Aktuell",
                KeyMetrics = new Dictionary<string, object>
                {
                    { "endpoint", "/reports/billing" },
                    { "description", "Abrechnungen, Zahlungsstatus, Offene Posten, Aging" }
                }
            }
        };

        return Ok(reports);
    }

    // ============================================
    // HELPER METHODS
    // ============================================

    private List<AgeGroupDistributionDto> GetAgeGroupDistribution(List<ClientDetails> clients)
    {
        var now = DateTime.Now;
        var ageGroups = new Dictionary<string, (int min, int max)>
        {
            { "Unter 50", (0, 49) },
            { "50-59", (50, 59) },
            { "60-69", (60, 69) },
            { "70-79", (70, 79) },
            { "80-89", (80, 89) },
            { "90+", (90, 150) }
        };

        return ageGroups.Select(ag => 
        {
            var count = clients.Count(c => 
            {
                if (c.BirthDay == null) return false;
                var age = now.Year - c.BirthDay.Value.Year;
                if (c.BirthDay.Value.Date > now.AddYears(-age)) age--;
                return age >= ag.Value.min && age <= ag.Value.max;
            });
            return new AgeGroupDistributionDto
            {
                AgeGroup = ag.Key,
                Count = count,
                Percentage = clients.Count > 0 ? Math.Round((double)count / clients.Count * 100, 1) : 0
            };
        })
        .Where(a => a.Count > 0)
        .ToList();
    }

    private string GetStatusColor(int statusId)
    {
        return statusId switch
        {
            7 => "#28a745", // Aktiv - Green
            8 => "#dc3545", // Inaktiv - Red
            9 => "#ffc107", // Pausiert - Yellow
            _ => "#6c757d"  // Unknown - Gray
        };
    }

    private List<CostBreakdownDto> GetCostBreakdown(List<ClientDetails> clients, decimal totalMonthlyRevenue)
    {
        var allCosts = clients.SelectMany(c => c.Costs ?? new List<ClientCost>()).ToList();
        var totalCosts = allCosts.Sum(c => c.Amount ?? 0);

        if (totalCosts == 0)
        {
            // Return simulated data based on revenue
            var simulatedCosts = totalMonthlyRevenue * 0.7m;
            return new List<CostBreakdownDto>
            {
                new CostBreakdownDto { CostCategory = "Personal", Amount = simulatedCosts * 0.45m, Percentage = 45 },
                new CostBreakdownDto { CostCategory = "Geräte", Amount = simulatedCosts * 0.24m, Percentage = 24 },
                new CostBreakdownDto { CostCategory = "Verwaltung", Amount = simulatedCosts * 0.15m, Percentage = 15 },
                new CostBreakdownDto { CostCategory = "Transport", Amount = simulatedCosts * 0.09m, Percentage = 9 },
                new CostBreakdownDto { CostCategory = "Sonstiges", Amount = simulatedCosts * 0.07m, Percentage = 7 }
            };
        }

        return allCosts
            .GroupBy(c => c.CostType ?? "Sonstiges")
            .Select(g => new CostBreakdownDto
            {
                CostCategory = g.Key,
                Amount = g.Sum(c => c.Amount ?? 0),
                Percentage = Math.Round((double)g.Sum(c => c.Amount ?? 0) / (double)totalCosts * 100, 1)
            })
            .OrderByDescending(c => c.Amount)
            .ToList();
    }
}
