using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMOApi.Data;
using UMOApi.Models;

namespace UMOApi.Controllers;

/// <summary>
/// Controller for business reports across Marketing, CRM, Sales, ERP, and Finance.
/// </summary>
[ApiController]
[Route("[controller]")]
public class BusinessReportsController : ControllerBase
{
    private readonly UMOApiDbContext _context;

    public BusinessReportsController(UMOApiDbContext context)
    {
        _context = context;
    }

    #region Marketing & CRM Reports

    /// <summary>
    /// Get Marketing Dashboard Report
    /// </summary>
    [HttpGet("marketing/dashboard")]
    public async Task<ActionResult<MarketingDashboardReport>> GetMarketingDashboard()
    {
        var campaigns = await _context.Campaigns.ToListAsync();
        var leads = await _context.Leads.Include(l => l.Campaign).ToListAsync();
        
        var report = new MarketingDashboardReport
        {
            TotalCampaigns = campaigns.Count,
            ActiveCampaigns = campaigns.Count(c => c.Status == "Active"),
            TotalLeads = leads.Count,
            NewLeadsThisMonth = leads.Count(l => l.CreateDate?.Month == DateTime.Now.Month && l.CreateDate?.Year == DateTime.Now.Year),
            TotalMarketingBudget = campaigns.Sum(c => c.Budget ?? 0),
            TotalMarketingSpend = campaigns.Sum(c => c.ActualCost ?? 0),
            CostPerLead = leads.Count > 0 ? campaigns.Sum(c => c.ActualCost ?? 0) / leads.Count : 0,
            LeadConversionRate = leads.Count > 0 ? (decimal)leads.Count(l => l.Status == "Won") / leads.Count * 100 : 0,
            
            LeadsBySource = leads.GroupBy(l => l.Source ?? "Unknown")
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .OrderByDescending(x => x.Value)
                .ToList(),
            
            LeadsByStatus = leads.GroupBy(l => l.Status ?? "Unknown")
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .ToList(),
            
            CampaignPerformance = campaigns.Select(c => new ChartDataItem
            {
                Label = c.Name ?? "Unknown",
                Value = c.ActualLeads ?? 0
            }).OrderByDescending(x => x.Value).Take(5).ToList(),
            
            LeadTrend = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .Select(date => new TimeSeriesDataItem
                {
                    Period = date.ToString("MMM yyyy"),
                    Value = leads.Count(l => l.CreateDate?.Month == date.Month && l.CreateDate?.Year == date.Year)
                })
                .Reverse()
                .ToList(),
            
            TopCampaigns = campaigns.OrderByDescending(c => c.ConvertedLeads)
                .Take(5)
                .Select(c => new CampaignSummary
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    Status = c.Status,
                    Budget = c.Budget ?? 0,
                    Spend = c.ActualCost ?? 0,
                    Leads = c.ActualLeads ?? 0,
                    Conversions = c.ConvertedLeads ?? 0,
                    ROI = c.ActualCost > 0 ? ((c.ConvertedLeads ?? 0) * 1000 - (c.ActualCost ?? 0)) / (c.ActualCost ?? 1) * 100 : 0
                })
                .ToList()
        };

        return Ok(report);
    }

    /// <summary>
    /// Get Lead Qualification Report
    /// </summary>
    [HttpGet("marketing/lead-qualification")]
    public async Task<ActionResult<LeadQualificationReport>> GetLeadQualificationReport()
    {
        var leads = await _context.Leads.Include(l => l.Activities).ToListAsync();
        
        var report = new LeadQualificationReport
        {
            TotalLeads = leads.Count,
            HotLeads = leads.Count(l => l.QualificationScore == "Hot"),
            WarmLeads = leads.Count(l => l.QualificationScore == "Warm"),
            ColdLeads = leads.Count(l => l.QualificationScore == "Cold"),
            AverageLeadScore = leads.Count > 0 ? (decimal)leads.Average(l => l.Score ?? 0) : 0,
            QualificationRate = leads.Count > 0 ? (decimal)leads.Count(l => l.Status == "Qualified" || l.Status == "Won") / leads.Count * 100 : 0,
            AverageDaysToQualify = 14, // Simulated
            
            LeadsByQualification = new List<ChartDataItem>
            {
                new() { Label = "Hot", Value = leads.Count(l => l.QualificationScore == "Hot"), Color = "#dc3545" },
                new() { Label = "Warm", Value = leads.Count(l => l.QualificationScore == "Warm"), Color = "#ffc107" },
                new() { Label = "Cold", Value = leads.Count(l => l.QualificationScore == "Cold"), Color = "#17a2b8" }
            },
            
            LeadScoreDistribution = new List<ChartDataItem>
            {
                new() { Label = "0-25", Value = leads.Count(l => l.Score <= 25) },
                new() { Label = "26-50", Value = leads.Count(l => l.Score > 25 && l.Score <= 50) },
                new() { Label = "51-75", Value = leads.Count(l => l.Score > 50 && l.Score <= 75) },
                new() { Label = "76-100", Value = leads.Count(l => l.Score > 75) }
            },
            
            ConversionBySource = leads.GroupBy(l => l.Source ?? "Unknown")
                .Select(g => new ChartDataItem
                {
                    Label = g.Key,
                    Value = g.Count(l => l.Status == "Won"),
                    Percentage = g.Count() > 0 ? (decimal)g.Count(l => l.Status == "Won") / g.Count() * 100 : 0
                })
                .ToList(),
            
            RecentQualifiedLeads = leads.Where(l => l.Status == "Qualified" || l.QualificationScore == "Hot")
                .OrderByDescending(l => l.LastContactDate)
                .Take(10)
                .Select(l => new LeadSummary
                {
                    Id = l.Id,
                    Name = $"{l.FirstName} {l.LastName}",
                    Company = l.Company,
                    Source = l.Source,
                    Status = l.Status,
                    Score = l.Score ?? 0,
                    EstimatedValue = l.EstimatedValue ?? 0,
                    LastContact = l.LastContactDate
                })
                .ToList()
        };

        return Ok(report);
    }

    /// <summary>
    /// Get CRM Activity Report
    /// </summary>
    [HttpGet("crm/activities")]
    public async Task<ActionResult<CRMActivityReport>> GetCRMActivityReport()
    {
        var activities = await _context.LeadActivities.Include(a => a.Lead).ToListAsync();
        var weekAgo = DateTime.Now.AddDays(-7);
        
        var report = new CRMActivityReport
        {
            TotalActivities = activities.Count,
            ActivitiesThisWeek = activities.Count(a => a.ActivityDate >= weekAgo),
            CallsMade = activities.Count(a => a.Type == "Call"),
            EmailsSent = activities.Count(a => a.Type == "Email"),
            MeetingsHeld = activities.Count(a => a.Type == "Meeting"),
            AverageResponseTime = 2.5m, // Simulated
            OverdueFollowUps = activities.Count(a => a.NextFollowUp < DateTime.Now),
            
            ActivitiesByType = activities.GroupBy(a => a.Type ?? "Unknown")
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .ToList(),
            
            ActivitiesByOutcome = activities.GroupBy(a => a.Outcome ?? "Unknown")
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .ToList(),
            
            ActivityTrend = Enumerable.Range(0, 7)
                .Select(i => DateTime.Now.AddDays(-i))
                .Select(date => new TimeSeriesDataItem
                {
                    Period = date.ToString("ddd"),
                    Value = activities.Count(a => a.ActivityDate?.Date == date.Date)
                })
                .Reverse()
                .ToList(),
            
            ActivityByUser = activities.GroupBy(a => a.PerformedBy ?? "Unknown")
                .Select(g => new UserActivitySummary
                {
                    UserName = g.Key,
                    TotalActivities = g.Count(),
                    Calls = g.Count(a => a.Type == "Call"),
                    Emails = g.Count(a => a.Type == "Email"),
                    Meetings = g.Count(a => a.Type == "Meeting"),
                    Conversions = g.Count(a => a.Outcome == "Positive")
                })
                .OrderByDescending(x => x.TotalActivities)
                .ToList(),
            
            UpcomingActivities = activities.Where(a => a.NextFollowUp > DateTime.Now)
                .OrderBy(a => a.NextFollowUp)
                .Take(10)
                .Select(a => new UpcomingActivity
                {
                    Id = a.Id,
                    Type = a.Type,
                    LeadName = a.Lead != null ? $"{a.Lead.FirstName} {a.Lead.LastName}" : "Unknown",
                    Description = a.Description,
                    DueDate = a.NextFollowUp,
                    AssignedTo = a.PerformedBy
                })
                .ToList()
        };

        return Ok(report);
    }

    #endregion

    #region Sales Reports

    /// <summary>
    /// Get Sales Dashboard Report
    /// </summary>
    [HttpGet("sales/dashboard")]
    public async Task<ActionResult<SalesDashboardReport>> GetSalesDashboard()
    {
        var opportunities = await _context.SalesOpportunities.Include(o => o.Lead).ToListAsync();
        var orders = await _context.SalesOrders.Include(o => o.Client).ToListAsync();
        var invoices = await _context.Invoices.Where(i => i.Status == "Paid").ToListAsync();
        
        var thisMonth = DateTime.Now.Month;
        var lastMonth = DateTime.Now.AddMonths(-1).Month;
        var thisYear = DateTime.Now.Year;
        
        var revenueThisMonth = invoices.Where(i => i.PaidDate?.Month == thisMonth && i.PaidDate?.Year == thisYear).Sum(i => i.TotalAmount ?? 0);
        var revenueLastMonth = invoices.Where(i => i.PaidDate?.Month == lastMonth).Sum(i => i.TotalAmount ?? 0);
        
        var report = new SalesDashboardReport
        {
            TotalRevenue = invoices.Sum(i => i.TotalAmount ?? 0),
            RevenueThisMonth = revenueThisMonth,
            RevenueLastMonth = revenueLastMonth,
            RevenueGrowth = revenueLastMonth > 0 ? (revenueThisMonth - revenueLastMonth) / revenueLastMonth * 100 : 0,
            TotalOrders = orders.Count,
            OrdersThisMonth = orders.Count(o => o.OrderDate?.Month == thisMonth && o.OrderDate?.Year == thisYear),
            AverageOrderValue = orders.Count > 0 ? orders.Average(o => o.TotalAmount ?? 0) : 0,
            WinRate = opportunities.Count > 0 ? (decimal)opportunities.Count(o => o.Stage == "Closed Won") / opportunities.Count(o => o.Stage?.StartsWith("Closed") == true || o.Stage == "Closed Won" || o.Stage == "Closed Lost") * 100 : 0,
            
            PipelineValue = opportunities.Where(o => o.Stage != "Closed Won" && o.Stage != "Closed Lost").Sum(o => o.Amount ?? 0),
            OpenOpportunities = opportunities.Count(o => o.Stage != "Closed Won" && o.Stage != "Closed Lost"),
            WeightedPipeline = opportunities.Where(o => o.Stage != "Closed Won" && o.Stage != "Closed Lost").Sum(o => (o.Amount ?? 0) * (o.Probability ?? 0) / 100),
            
            RevenueTrend = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .Select(date => new TimeSeriesDataItem
                {
                    Period = date.ToString("MMM"),
                    Value = invoices.Where(inv => inv.PaidDate?.Month == date.Month && inv.PaidDate?.Year == date.Year).Sum(inv => inv.TotalAmount ?? 0)
                })
                .Reverse()
                .ToList(),
            
            PipelineByStage = opportunities.Where(o => o.Stage != "Closed Won" && o.Stage != "Closed Lost")
                .GroupBy(o => o.Stage ?? "Unknown")
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Sum(o => o.Amount ?? 0) })
                .ToList(),
            
            TopSalesReps = opportunities.GroupBy(o => o.AssignedToName ?? "Unknown")
                .Select(g => new SalesRepPerformance
                {
                    Name = g.Key,
                    Revenue = g.Where(o => o.Stage == "Closed Won").Sum(o => o.Amount ?? 0),
                    Deals = g.Count(o => o.Stage == "Closed Won"),
                    WinRate = g.Count() > 0 ? (decimal)g.Count(o => o.Stage == "Closed Won") / g.Count() * 100 : 0,
                    AverageDealSize = g.Count(o => o.Stage == "Closed Won") > 0 ? g.Where(o => o.Stage == "Closed Won").Average(o => o.Amount ?? 0) : 0,
                    Target = 50000,
                    Achievement = g.Where(o => o.Stage == "Closed Won").Sum(o => o.Amount ?? 0) / 50000 * 100
                })
                .OrderByDescending(x => x.Revenue)
                .ToList(),
            
            RecentOrders = orders.OrderByDescending(o => o.OrderDate)
                .Take(10)
                .Select(o => new OrderSummary
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    ClientName = o.Client != null ? $"{o.Client.FirstName} {o.Client.LastName}" : "Unknown",
                    Amount = o.TotalAmount ?? 0,
                    Status = o.Status,
                    OrderDate = o.OrderDate
                })
                .ToList()
        };

        return Ok(report);
    }

    /// <summary>
    /// Get Sales Pipeline Report
    /// </summary>
    [HttpGet("sales/pipeline")]
    public async Task<ActionResult<SalesPipelineReport>> GetSalesPipelineReport()
    {
        var opportunities = await _context.SalesOpportunities.Include(o => o.Lead).ToListAsync();
        var openOpportunities = opportunities.Where(o => o.Stage != "Closed Won" && o.Stage != "Closed Lost").ToList();
        
        var report = new SalesPipelineReport
        {
            TotalPipelineValue = openOpportunities.Sum(o => o.Amount ?? 0),
            WeightedValue = openOpportunities.Sum(o => (o.Amount ?? 0) * (o.Probability ?? 0) / 100),
            TotalOpportunities = openOpportunities.Count,
            AverageOpportunitySize = openOpportunities.Count > 0 ? openOpportunities.Average(o => o.Amount ?? 0) : 0,
            AverageSalesCycle = 45, // Simulated
            ForecastedRevenue = openOpportunities.Where(o => o.ExpectedCloseDate?.Month == DateTime.Now.Month).Sum(o => (o.Amount ?? 0) * (o.Probability ?? 0) / 100),
            
            PipelineStages = new List<PipelineStage>
            {
                new() { Name = "Qualification", Count = openOpportunities.Count(o => o.Stage == "Qualification"), Value = openOpportunities.Where(o => o.Stage == "Qualification").Sum(o => o.Amount ?? 0), WeightedValue = openOpportunities.Where(o => o.Stage == "Qualification").Sum(o => (o.Amount ?? 0) * 0.2m), AverageDays = 7 },
                new() { Name = "Needs Analysis", Count = openOpportunities.Count(o => o.Stage == "Needs Analysis"), Value = openOpportunities.Where(o => o.Stage == "Needs Analysis").Sum(o => o.Amount ?? 0), WeightedValue = openOpportunities.Where(o => o.Stage == "Needs Analysis").Sum(o => (o.Amount ?? 0) * 0.4m), AverageDays = 14 },
                new() { Name = "Proposal", Count = openOpportunities.Count(o => o.Stage == "Proposal"), Value = openOpportunities.Where(o => o.Stage == "Proposal").Sum(o => o.Amount ?? 0), WeightedValue = openOpportunities.Where(o => o.Stage == "Proposal").Sum(o => (o.Amount ?? 0) * 0.6m), AverageDays = 21 },
                new() { Name = "Negotiation", Count = openOpportunities.Count(o => o.Stage == "Negotiation"), Value = openOpportunities.Where(o => o.Stage == "Negotiation").Sum(o => o.Amount ?? 0), WeightedValue = openOpportunities.Where(o => o.Stage == "Negotiation").Sum(o => (o.Amount ?? 0) * 0.8m), AverageDays = 10 }
            },
            
            OpportunitiesByStage = openOpportunities.GroupBy(o => o.Stage ?? "Unknown")
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .ToList(),
            
            WinLossAnalysis = new List<ChartDataItem>
            {
                new() { Label = "Won", Value = opportunities.Count(o => o.Stage == "Closed Won"), Color = "#28a745" },
                new() { Label = "Lost", Value = opportunities.Count(o => o.Stage == "Closed Lost"), Color = "#dc3545" },
                new() { Label = "Open", Value = openOpportunities.Count, Color = "#ffc107" }
            },
            
            TopOpportunities = openOpportunities.OrderByDescending(o => o.Amount)
                .Take(10)
                .Select(o => new OpportunitySummary
                {
                    Id = o.Id,
                    Name = o.Name,
                    ClientName = o.Lead != null ? $"{o.Lead.FirstName} {o.Lead.LastName}" : "Unknown",
                    Amount = o.Amount ?? 0,
                    Probability = o.Probability ?? 0,
                    Stage = o.Stage,
                    ExpectedClose = o.ExpectedCloseDate,
                    Owner = o.AssignedToName
                })
                .ToList()
        };

        return Ok(report);
    }

    #endregion

    #region ERP Reports

    /// <summary>
    /// Get ERP Inventory Report
    /// </summary>
    [HttpGet("erp/inventory")]
    public async Task<ActionResult<ERPInventoryReport>> GetERPInventoryReport()
    {
        var articles = await _context.Articles.ToListAsync();
        var inventory = await _context.Inventories.Include(i => i.Article).ToListAsync();
        
        var report = new ERPInventoryReport
        {
            TotalArticles = articles.Count,
            ActiveArticles = articles.Count(a => a.IsActive),
            TotalInventoryValue = inventory.Sum(i => i.CurrentStock * (i.Article?.PurchasePrice ?? 0)),
            LowStockItems = inventory.Count(i => i.CurrentStock <= (i.Article?.ReorderPoint ?? 0) && i.CurrentStock > 0),
            OutOfStockItems = inventory.Count(i => i.CurrentStock == 0),
            ExpiringItems = inventory.Count(i => i.ExpiryDate != null && i.ExpiryDate <= DateTime.Now.AddMonths(3)),
            InventoryTurnover = 4.2m, // Simulated
            
            StockByCategory = articles.GroupBy(a => a.Category ?? "Unknown")
                .Select(g => new ChartDataItem
                {
                    Label = g.Key,
                    Value = inventory.Where(i => g.Select(a => a.Id).Contains(i.ArticleId)).Sum(i => i.CurrentStock)
                })
                .ToList(),
            
            ValueByCategory = articles.GroupBy(a => a.Category ?? "Unknown")
                .Select(g => new ChartDataItem
                {
                    Label = g.Key,
                    Value = inventory.Where(i => g.Select(a => a.Id).Contains(i.ArticleId)).Sum(i => i.CurrentStock * (i.Article?.PurchasePrice ?? 0))
                })
                .ToList(),
            
            LowStockAlerts = inventory.Where(i => i.CurrentStock <= (i.Article?.ReorderPoint ?? 0))
                .OrderBy(i => i.CurrentStock)
                .Take(10)
                .Select(i => new ArticleStockSummary
                {
                    Id = i.ArticleId,
                    ArticleNumber = i.Article?.ArticleNumber,
                    Name = i.Article?.Name,
                    Category = i.Article?.Category,
                    CurrentStock = i.CurrentStock,
                    MinStock = i.Article?.MinStock ?? 0,
                    ReorderPoint = i.Article?.ReorderPoint ?? 0,
                    Value = i.CurrentStock * (i.Article?.PurchasePrice ?? 0),
                    Status = i.CurrentStock == 0 ? "Out of Stock" : "Low Stock"
                })
                .ToList(),
            
            TopMovingItems = inventory.OrderByDescending(i => i.CurrentStock)
                .Take(10)
                .Select(i => new ArticleStockSummary
                {
                    Id = i.ArticleId,
                    ArticleNumber = i.Article?.ArticleNumber,
                    Name = i.Article?.Name,
                    Category = i.Article?.Category,
                    CurrentStock = i.CurrentStock,
                    MinStock = i.Article?.MinStock ?? 0,
                    ReorderPoint = i.Article?.ReorderPoint ?? 0,
                    Value = i.CurrentStock * (i.Article?.PurchasePrice ?? 0),
                    Status = "In Stock"
                })
                .ToList(),
            
            StockByWarehouse = inventory.GroupBy(i => i.WarehouseLocation ?? "Unknown")
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Sum(i => i.CurrentStock) })
                .ToList()
        };

        return Ok(report);
    }

    /// <summary>
    /// Get ERP Device Report
    /// </summary>
    [HttpGet("erp/devices")]
    public async Task<ActionResult<ERPDeviceReport>> GetERPDeviceReport()
    {
        var devices = await _context.DeviceDetails.ToListAsync();
        
        var report = new ERPDeviceReport
        {
            TotalDevices = devices.Count,
            ActiveDevices = devices.Count(d => d.Status != "Defekt" && d.Status != "Ausgemustert"),
            AssignedDevices = devices.Count(d => d.AssignedClientId != null),
            AvailableDevices = devices.Count(d => d.AssignedClientId == null && d.Status == "Verfügbar"),
            DevicesInMaintenance = devices.Count(d => d.Status == "In Wartung" || d.Status == "Defekt"),
            WarrantyExpiringSoon = devices.Count(d => d.WarrantyEndDate != null && d.WarrantyEndDate <= DateTime.Now.AddMonths(3)),
            TotalDeviceValue = devices.Sum(d => d.PurchasePrice ?? 0),
            
            DevicesByType = devices.GroupBy(d => d.DeviceType ?? "Unknown")
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .OrderByDescending(x => x.Value)
                .ToList(),
            
            DevicesByStatus = devices.GroupBy(d => d.Status ?? "Unknown")
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .ToList(),
            
            DevicesByManufacturer = devices.GroupBy(d => d.Manufacturer ?? "Unknown")
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .ToList(),
            
            RecentDevices = devices.OrderByDescending(d => d.PurchaseDate)
                .Take(10)
                .Select(d => new DeviceSummary
                {
                    Id = d.Id,
                    SerialNumber = d.SerialNumber,
                    DeviceType = d.DeviceType,
                    Manufacturer = d.Manufacturer,
                    Model = d.Model,
                    Status = d.Status,
                    WarrantyEnd = d.WarrantyEndDate,
                    Value = d.PurchasePrice
                })
                .ToList(),
            
            WarrantyAlerts = devices.Where(d => d.WarrantyEndDate != null && d.WarrantyEndDate <= DateTime.Now.AddMonths(6))
                .OrderBy(d => d.WarrantyEndDate)
                .Take(10)
                .Select(d => new DeviceSummary
                {
                    Id = d.Id,
                    SerialNumber = d.SerialNumber,
                    DeviceType = d.DeviceType,
                    Manufacturer = d.Manufacturer,
                    Model = d.Model,
                    Status = d.Status,
                    WarrantyEnd = d.WarrantyEndDate,
                    Value = d.PurchasePrice
                })
                .ToList()
        };

        return Ok(report);
    }

    /// <summary>
    /// Get ERP Medication Report
    /// </summary>
    [HttpGet("erp/medications")]
    public async Task<ActionResult<ERPMedicationReport>> GetERPMedicationReport()
    {
        var medications = await _context.Medications.ToListAsync();
        var inventory = await _context.Inventories.Include(i => i.Article).Where(i => i.Article != null && i.Article.Category == "Medication").ToListAsync();
        
        var report = new ERPMedicationReport
        {
            TotalMedications = medications.Count,
            ActiveMedications = medications.Count(m => m.IsActive),
            PrescriptionRequired = medications.Count(m => m.RequiresPrescription),
            LowStockMedications = inventory.Count(i => i.CurrentStock <= (i.Article?.ReorderPoint ?? 0)),
            ExpiringMedications = inventory.Count(i => i.ExpiryDate != null && i.ExpiryDate <= DateTime.Now.AddMonths(6)),
            TotalMedicationValue = inventory.Sum(i => i.CurrentStock * (i.Article?.PurchasePrice ?? 0)),
            
            MedicationsByForm = medications.GroupBy(m => m.DosageForm ?? "Unknown")
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .ToList(),
            
            MedicationsByManufacturer = medications.GroupBy(m => m.Manufacturer ?? "Unknown")
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .ToList(),
            
            ExpiryAlerts = inventory.Where(i => i.ExpiryDate != null)
                .OrderBy(i => i.ExpiryDate)
                .Take(10)
                .Select(i => new MedicationSummary
                {
                    Id = i.ArticleId,
                    Name = i.Article?.Name,
                    Stock = i.CurrentStock,
                    ExpiryDate = i.ExpiryDate
                })
                .ToList(),
            
            TopPrescribed = medications.Take(10)
                .Select(m => new MedicationSummary
                {
                    Id = m.Id,
                    Name = m.Name,
                    GenericName = m.GenericName,
                    DosageForm = m.DosageForm,
                    Strength = m.Strength,
                    RequiresPrescription = m.RequiresPrescription
                })
                .ToList()
        };

        return Ok(report);
    }

    #endregion

    #region Billing & Finance Reports

    /// <summary>
    /// Get Billing Dashboard Report
    /// </summary>
    [HttpGet("billing/dashboard")]
    public async Task<ActionResult<BillingDashboardReport>> GetBillingDashboard()
    {
        var invoices = await _context.Invoices.Include(i => i.Client).ToListAsync();
        var payments = await _context.Payments.ToListAsync();
        
        var report = new BillingDashboardReport
        {
            TotalInvoiced = invoices.Sum(i => i.TotalAmount ?? 0),
            TotalPaid = invoices.Sum(i => i.PaidAmount ?? 0),
            TotalOutstanding = invoices.Sum(i => i.OutstandingAmount ?? 0),
            OverdueAmount = invoices.Where(i => i.Status == "Overdue").Sum(i => i.OutstandingAmount ?? 0),
            TotalInvoices = invoices.Count,
            PaidInvoices = invoices.Count(i => i.Status == "Paid"),
            OverdueInvoices = invoices.Count(i => i.Status == "Overdue"),
            CollectionRate = invoices.Sum(i => i.TotalAmount ?? 0) > 0 ? invoices.Sum(i => i.PaidAmount ?? 0) / invoices.Sum(i => i.TotalAmount ?? 0) * 100 : 0,
            AverageDaysToPayment = 18, // Simulated
            
            InvoicesByStatus = invoices.GroupBy(i => i.Status ?? "Unknown")
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Count() })
                .ToList(),
            
            RevenueTrend = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .Select(date => new TimeSeriesDataItem
                {
                    Period = date.ToString("MMM"),
                    Value = invoices.Where(inv => inv.InvoiceDate?.Month == date.Month && inv.InvoiceDate?.Year == date.Year).Sum(inv => inv.TotalAmount ?? 0)
                })
                .Reverse()
                .ToList(),
            
            PaymentTrend = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .Select(date => new TimeSeriesDataItem
                {
                    Period = date.ToString("MMM"),
                    Value = payments.Where(p => p.PaymentDate.Month == date.Month && p.PaymentDate.Year == date.Year).Sum(p => p.Amount)
                })
                .Reverse()
                .ToList(),
            
            PaymentMethods = payments.GroupBy(p => p.PaymentMethod ?? "Unknown")
                .Select(g => new ChartDataItem { Label = g.Key, Value = g.Sum(p => p.Amount) })
                .ToList(),
            
            AgingReport = new List<AgingBucket>
            {
                new() { Period = "Current", Count = invoices.Count(i => i.DueDate >= DateTime.Now), Amount = invoices.Where(i => i.DueDate >= DateTime.Now).Sum(i => i.OutstandingAmount ?? 0) },
                new() { Period = "1-30 Days", Count = invoices.Count(i => i.DueDate < DateTime.Now && i.DueDate >= DateTime.Now.AddDays(-30)), Amount = invoices.Where(i => i.DueDate < DateTime.Now && i.DueDate >= DateTime.Now.AddDays(-30)).Sum(i => i.OutstandingAmount ?? 0) },
                new() { Period = "31-60 Days", Count = invoices.Count(i => i.DueDate < DateTime.Now.AddDays(-30) && i.DueDate >= DateTime.Now.AddDays(-60)), Amount = invoices.Where(i => i.DueDate < DateTime.Now.AddDays(-30) && i.DueDate >= DateTime.Now.AddDays(-60)).Sum(i => i.OutstandingAmount ?? 0) },
                new() { Period = "61-90 Days", Count = invoices.Count(i => i.DueDate < DateTime.Now.AddDays(-60) && i.DueDate >= DateTime.Now.AddDays(-90)), Amount = invoices.Where(i => i.DueDate < DateTime.Now.AddDays(-60) && i.DueDate >= DateTime.Now.AddDays(-90)).Sum(i => i.OutstandingAmount ?? 0) },
                new() { Period = "90+ Days", Count = invoices.Count(i => i.DueDate < DateTime.Now.AddDays(-90)), Amount = invoices.Where(i => i.DueDate < DateTime.Now.AddDays(-90)).Sum(i => i.OutstandingAmount ?? 0) }
            },
            
            OverdueInvoicesList = invoices.Where(i => i.Status == "Overdue")
                .OrderByDescending(i => i.OutstandingAmount)
                .Take(10)
                .Select(i => new InvoiceSummary
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    ClientName = i.Client != null ? $"{i.Client.FirstName} {i.Client.LastName}" : "Unknown",
                    Amount = i.TotalAmount ?? 0,
                    Outstanding = i.OutstandingAmount ?? 0,
                    DueDate = i.DueDate,
                    DaysOverdue = i.DueDate.HasValue ? (int)(DateTime.Now - i.DueDate.Value).TotalDays : 0,
                    Status = i.Status
                })
                .ToList()
        };

        return Ok(report);
    }

    /// <summary>
    /// Get Financial Overview Report
    /// </summary>
    [HttpGet("finance/overview")]
    public async Task<ActionResult<FinancialOverviewReport>> GetFinancialOverview()
    {
        var invoices = await _context.Invoices.Where(i => i.Status == "Paid").ToListAsync();
        var costCenters = await _context.CostCenters.ToListAsync();
        
        var totalRevenue = invoices.Sum(i => i.TotalAmount ?? 0);
        var totalCosts = costCenters.Sum(c => c.ActualSpend ?? 0);
        
        var report = new FinancialOverviewReport
        {
            TotalRevenue = totalRevenue,
            TotalCosts = totalCosts,
            GrossProfit = totalRevenue - totalCosts,
            GrossMargin = totalRevenue > 0 ? (totalRevenue - totalCosts) / totalRevenue * 100 : 0,
            NetProfit = (totalRevenue - totalCosts) * 0.75m, // After tax estimate
            NetMargin = totalRevenue > 0 ? ((totalRevenue - totalCosts) * 0.75m) / totalRevenue * 100 : 0,
            CashFlow = totalRevenue - totalCosts,
            RevenueYoY = 12.5m, // Simulated
            ProfitYoY = 8.3m, // Simulated
            
            RevenueCostTrend = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .Select(date => new TimeSeriesDataItem
                {
                    Period = date.ToString("MMM"),
                    Value = invoices.Where(inv => inv.PaidDate?.Month == date.Month && inv.PaidDate?.Year == date.Year).Sum(inv => inv.TotalAmount ?? 0),
                    Value2 = costCenters.Sum(c => (c.ActualSpend ?? 0) / 12) // Monthly average
                })
                .Reverse()
                .ToList(),
            
            CostBreakdown = costCenters.Select(c => new ChartDataItem
            {
                Label = c.Name,
                Value = c.ActualSpend ?? 0
            }).ToList(),
            
            CostCenterPerformance = costCenters.Select(c => new CostCenterSummary
            {
                Code = c.Code,
                Name = c.Name,
                Department = c.Department,
                Budget = c.Budget ?? 0,
                Actual = c.ActualSpend ?? 0,
                Variance = (c.Budget ?? 0) - (c.ActualSpend ?? 0),
                VariancePercent = c.Budget > 0 ? ((c.Budget ?? 0) - (c.ActualSpend ?? 0)) / (c.Budget ?? 1) * 100 : 0
            }).ToList(),
            
            MonthlyOverview = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .Select(date => new MonthlyFinancialSummary
                {
                    Month = date.ToString("MMM yyyy"),
                    Revenue = invoices.Where(inv => inv.PaidDate?.Month == date.Month && inv.PaidDate?.Year == date.Year).Sum(inv => inv.TotalAmount ?? 0),
                    Costs = costCenters.Sum(c => (c.ActualSpend ?? 0) / 12),
                    Profit = invoices.Where(inv => inv.PaidDate?.Month == date.Month && inv.PaidDate?.Year == date.Year).Sum(inv => inv.TotalAmount ?? 0) - costCenters.Sum(c => (c.ActualSpend ?? 0) / 12),
                    Margin = 0
                })
                .Reverse()
                .ToList()
        };

        return Ok(report);
    }

    /// <summary>
    /// Get Accounts Receivable Report
    /// </summary>
    [HttpGet("finance/receivables")]
    public async Task<ActionResult<AccountsReceivableReport>> GetAccountsReceivableReport()
    {
        var invoices = await _context.Invoices.Include(i => i.Client).Where(i => i.OutstandingAmount > 0).ToListAsync();
        
        var report = new AccountsReceivableReport
        {
            TotalReceivables = invoices.Sum(i => i.OutstandingAmount ?? 0),
            Current = invoices.Where(i => i.DueDate >= DateTime.Now).Sum(i => i.OutstandingAmount ?? 0),
            Days30 = invoices.Where(i => i.DueDate < DateTime.Now && i.DueDate >= DateTime.Now.AddDays(-30)).Sum(i => i.OutstandingAmount ?? 0),
            Days60 = invoices.Where(i => i.DueDate < DateTime.Now.AddDays(-30) && i.DueDate >= DateTime.Now.AddDays(-60)).Sum(i => i.OutstandingAmount ?? 0),
            Days90Plus = invoices.Where(i => i.DueDate < DateTime.Now.AddDays(-60)).Sum(i => i.OutstandingAmount ?? 0),
            DSO = 25, // Simulated
            CustomersWithOverdue = invoices.Where(i => i.DueDate < DateTime.Now).Select(i => i.ClientId).Distinct().Count(),
            
            AgingAnalysis = new List<AgingBucket>
            {
                new() { Period = "Current", Count = invoices.Count(i => i.DueDate >= DateTime.Now), Amount = invoices.Where(i => i.DueDate >= DateTime.Now).Sum(i => i.OutstandingAmount ?? 0), Percentage = invoices.Sum(i => i.OutstandingAmount ?? 0) > 0 ? invoices.Where(i => i.DueDate >= DateTime.Now).Sum(i => i.OutstandingAmount ?? 0) / invoices.Sum(i => i.OutstandingAmount ?? 0) * 100 : 0 },
                new() { Period = "1-30 Days", Count = invoices.Count(i => i.DueDate < DateTime.Now && i.DueDate >= DateTime.Now.AddDays(-30)), Amount = invoices.Where(i => i.DueDate < DateTime.Now && i.DueDate >= DateTime.Now.AddDays(-30)).Sum(i => i.OutstandingAmount ?? 0) },
                new() { Period = "31-60 Days", Count = invoices.Count(i => i.DueDate < DateTime.Now.AddDays(-30) && i.DueDate >= DateTime.Now.AddDays(-60)), Amount = invoices.Where(i => i.DueDate < DateTime.Now.AddDays(-30) && i.DueDate >= DateTime.Now.AddDays(-60)).Sum(i => i.OutstandingAmount ?? 0) },
                new() { Period = "61-90 Days", Count = invoices.Count(i => i.DueDate < DateTime.Now.AddDays(-60) && i.DueDate >= DateTime.Now.AddDays(-90)), Amount = invoices.Where(i => i.DueDate < DateTime.Now.AddDays(-60) && i.DueDate >= DateTime.Now.AddDays(-90)).Sum(i => i.OutstandingAmount ?? 0) },
                new() { Period = "90+ Days", Count = invoices.Count(i => i.DueDate < DateTime.Now.AddDays(-90)), Amount = invoices.Where(i => i.DueDate < DateTime.Now.AddDays(-90)).Sum(i => i.OutstandingAmount ?? 0) }
            },
            
            TopDebtors = invoices.GroupBy(i => i.ClientId)
                .Select(g => new ClientReceivableSummary
                {
                    ClientId = g.Key ?? 0,
                    ClientName = g.First().Client != null ? $"{g.First().Client.FirstName} {g.First().Client.LastName}" : "Unknown",
                    TotalOutstanding = g.Sum(i => i.OutstandingAmount ?? 0),
                    Current = g.Where(i => i.DueDate >= DateTime.Now).Sum(i => i.OutstandingAmount ?? 0),
                    Overdue = g.Where(i => i.DueDate < DateTime.Now).Sum(i => i.OutstandingAmount ?? 0),
                    OpenInvoices = g.Count(),
                    OldestInvoice = g.Min(i => i.InvoiceDate)
                })
                .OrderByDescending(x => x.TotalOutstanding)
                .Take(10)
                .ToList()
        };

        return Ok(report);
    }

    #endregion
}
