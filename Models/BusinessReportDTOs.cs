namespace UMOApi.Models;

#region Marketing & CRM Report DTOs

/// <summary>
/// Marketing Dashboard Report
/// </summary>
public class MarketingDashboardReport
{
    // KPIs
    public int TotalCampaigns { get; set; }
    public int ActiveCampaigns { get; set; }
    public int TotalLeads { get; set; }
    public int NewLeadsThisMonth { get; set; }
    public decimal TotalMarketingBudget { get; set; }
    public decimal TotalMarketingSpend { get; set; }
    public decimal CostPerLead { get; set; }
    public decimal LeadConversionRate { get; set; }
    
    // Charts Data
    public List<ChartDataItem> LeadsBySource { get; set; } = new();
    public List<ChartDataItem> LeadsByStatus { get; set; } = new();
    public List<ChartDataItem> CampaignPerformance { get; set; } = new();
    public List<TimeSeriesDataItem> LeadTrend { get; set; } = new();
    public List<CampaignSummary> TopCampaigns { get; set; } = new();
}

/// <summary>
/// Lead Qualification Report
/// </summary>
public class LeadQualificationReport
{
    // KPIs
    public int TotalLeads { get; set; }
    public int HotLeads { get; set; }
    public int WarmLeads { get; set; }
    public int ColdLeads { get; set; }
    public decimal AverageLeadScore { get; set; }
    public decimal QualificationRate { get; set; }
    public int AverageDaysToQualify { get; set; }
    
    // Charts Data
    public List<ChartDataItem> LeadsByQualification { get; set; } = new();
    public List<ChartDataItem> LeadScoreDistribution { get; set; } = new();
    public List<TimeSeriesDataItem> QualificationTrend { get; set; } = new();
    public List<LeadSummary> RecentQualifiedLeads { get; set; } = new();
    public List<ChartDataItem> ConversionBySource { get; set; } = new();
}

/// <summary>
/// CRM Activity Report
/// </summary>
public class CRMActivityReport
{
    // KPIs
    public int TotalActivities { get; set; }
    public int ActivitiesThisWeek { get; set; }
    public int CallsMade { get; set; }
    public int EmailsSent { get; set; }
    public int MeetingsHeld { get; set; }
    public decimal AverageResponseTime { get; set; }
    public int OverdueFollowUps { get; set; }
    
    // Charts Data
    public List<ChartDataItem> ActivitiesByType { get; set; } = new();
    public List<ChartDataItem> ActivitiesByOutcome { get; set; } = new();
    public List<TimeSeriesDataItem> ActivityTrend { get; set; } = new();
    public List<UserActivitySummary> ActivityByUser { get; set; } = new();
    public List<UpcomingActivity> UpcomingActivities { get; set; } = new();
}

public class CampaignSummary
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public decimal Budget { get; set; }
    public decimal Spend { get; set; }
    public int Leads { get; set; }
    public int Conversions { get; set; }
    public decimal ROI { get; set; }
}

public class LeadSummary
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Company { get; set; }
    public string? Source { get; set; }
    public string? Status { get; set; }
    public int Score { get; set; }
    public decimal EstimatedValue { get; set; }
    public DateTime? LastContact { get; set; }
}

public class UserActivitySummary
{
    public string? UserName { get; set; }
    public int TotalActivities { get; set; }
    public int Calls { get; set; }
    public int Emails { get; set; }
    public int Meetings { get; set; }
    public int Conversions { get; set; }
}

public class UpcomingActivity
{
    public int Id { get; set; }
    public string? Type { get; set; }
    public string? LeadName { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string? AssignedTo { get; set; }
}

#endregion

#region Sales Report DTOs

/// <summary>
/// Sales Dashboard Report
/// </summary>
public class SalesDashboardReport
{
    // KPIs
    public decimal TotalRevenue { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal RevenueLastMonth { get; set; }
    public decimal RevenueGrowth { get; set; }
    public int TotalOrders { get; set; }
    public int OrdersThisMonth { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal WinRate { get; set; }
    
    // Pipeline
    public decimal PipelineValue { get; set; }
    public int OpenOpportunities { get; set; }
    public decimal WeightedPipeline { get; set; }
    
    // Charts Data
    public List<TimeSeriesDataItem> RevenueTrend { get; set; } = new();
    public List<ChartDataItem> RevenueByProduct { get; set; } = new();
    public List<ChartDataItem> SalesByRegion { get; set; } = new();
    public List<ChartDataItem> PipelineByStage { get; set; } = new();
    public List<SalesRepPerformance> TopSalesReps { get; set; } = new();
    public List<OrderSummary> RecentOrders { get; set; } = new();
}

/// <summary>
/// Sales Pipeline Report
/// </summary>
public class SalesPipelineReport
{
    // KPIs
    public decimal TotalPipelineValue { get; set; }
    public decimal WeightedValue { get; set; }
    public int TotalOpportunities { get; set; }
    public decimal AverageOpportunitySize { get; set; }
    public int AverageSalesCycle { get; set; }
    public decimal ForecastedRevenue { get; set; }
    
    // Charts Data
    public List<PipelineStage> PipelineStages { get; set; } = new();
    public List<ChartDataItem> OpportunitiesByStage { get; set; } = new();
    public List<TimeSeriesDataItem> PipelineTrend { get; set; } = new();
    public List<OpportunitySummary> TopOpportunities { get; set; } = new();
    public List<ChartDataItem> WinLossAnalysis { get; set; } = new();
}

public class SalesRepPerformance
{
    public string? Name { get; set; }
    public decimal Revenue { get; set; }
    public int Deals { get; set; }
    public decimal WinRate { get; set; }
    public decimal AverageDealSize { get; set; }
    public decimal Target { get; set; }
    public decimal Achievement { get; set; }
}

public class OrderSummary
{
    public int Id { get; set; }
    public string? OrderNumber { get; set; }
    public string? ClientName { get; set; }
    public decimal Amount { get; set; }
    public string? Status { get; set; }
    public DateTime? OrderDate { get; set; }
}

public class PipelineStage
{
    public string? Name { get; set; }
    public int Count { get; set; }
    public decimal Value { get; set; }
    public decimal WeightedValue { get; set; }
    public int AverageDays { get; set; }
}

public class OpportunitySummary
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ClientName { get; set; }
    public decimal Amount { get; set; }
    public int Probability { get; set; }
    public string? Stage { get; set; }
    public DateTime? ExpectedClose { get; set; }
    public string? Owner { get; set; }
}

#endregion

#region ERP Report DTOs

/// <summary>
/// ERP Inventory Dashboard Report
/// </summary>
public class ERPInventoryReport
{
    // KPIs
    public int TotalArticles { get; set; }
    public int ActiveArticles { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public int LowStockItems { get; set; }
    public int OutOfStockItems { get; set; }
    public int ExpiringItems { get; set; }
    public decimal InventoryTurnover { get; set; }
    
    // Charts Data
    public List<ChartDataItem> StockByCategory { get; set; } = new();
    public List<ChartDataItem> ValueByCategory { get; set; } = new();
    public List<TimeSeriesDataItem> StockMovementTrend { get; set; } = new();
    public List<ArticleStockSummary> LowStockAlerts { get; set; } = new();
    public List<ArticleStockSummary> TopMovingItems { get; set; } = new();
    public List<ChartDataItem> StockByWarehouse { get; set; } = new();
}

/// <summary>
/// ERP Device Management Report
/// </summary>
public class ERPDeviceReport
{
    // KPIs
    public int TotalDevices { get; set; }
    public int ActiveDevices { get; set; }
    public int AssignedDevices { get; set; }
    public int AvailableDevices { get; set; }
    public int DevicesInMaintenance { get; set; }
    public int WarrantyExpiringSoon { get; set; }
    public decimal TotalDeviceValue { get; set; }
    
    // Charts Data
    public List<ChartDataItem> DevicesByType { get; set; } = new();
    public List<ChartDataItem> DevicesByStatus { get; set; } = new();
    public List<ChartDataItem> DevicesByManufacturer { get; set; } = new();
    public List<TimeSeriesDataItem> DeviceAcquisitionTrend { get; set; } = new();
    public List<DeviceSummary> RecentDevices { get; set; } = new();
    public List<DeviceSummary> WarrantyAlerts { get; set; } = new();
}

/// <summary>
/// ERP Medication Report
/// </summary>
public class ERPMedicationReport
{
    // KPIs
    public int TotalMedications { get; set; }
    public int ActiveMedications { get; set; }
    public int PrescriptionRequired { get; set; }
    public int LowStockMedications { get; set; }
    public int ExpiringMedications { get; set; }
    public decimal TotalMedicationValue { get; set; }
    
    // Charts Data
    public List<ChartDataItem> MedicationsByForm { get; set; } = new();
    public List<ChartDataItem> MedicationsByManufacturer { get; set; } = new();
    public List<MedicationSummary> ExpiryAlerts { get; set; } = new();
    public List<MedicationSummary> TopPrescribed { get; set; } = new();
    public List<TimeSeriesDataItem> UsageTrend { get; set; } = new();
}

public class ArticleStockSummary
{
    public int Id { get; set; }
    public string? ArticleNumber { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public int CurrentStock { get; set; }
    public int MinStock { get; set; }
    public int ReorderPoint { get; set; }
    public decimal Value { get; set; }
    public string? Status { get; set; }
}

public class DeviceSummary
{
    public int Id { get; set; }
    public string? SerialNumber { get; set; }
    public string? DeviceType { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? Status { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime? WarrantyEnd { get; set; }
    public decimal? Value { get; set; }
}

public class MedicationSummary
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? GenericName { get; set; }
    public string? DosageForm { get; set; }
    public string? Strength { get; set; }
    public int Stock { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool RequiresPrescription { get; set; }
}

#endregion

#region Billing & Finance Report DTOs

/// <summary>
/// Billing Dashboard Report
/// </summary>
public class BillingDashboardReport
{
    // KPIs
    public decimal TotalInvoiced { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalOutstanding { get; set; }
    public decimal OverdueAmount { get; set; }
    public int TotalInvoices { get; set; }
    public int PaidInvoices { get; set; }
    public int OverdueInvoices { get; set; }
    public decimal CollectionRate { get; set; }
    public int AverageDaysToPayment { get; set; }
    
    // Charts Data
    public List<ChartDataItem> InvoicesByStatus { get; set; } = new();
    public List<TimeSeriesDataItem> RevenueTrend { get; set; } = new();
    public List<TimeSeriesDataItem> PaymentTrend { get; set; } = new();
    public List<ChartDataItem> PaymentMethods { get; set; } = new();
    public List<AgingBucket> AgingReport { get; set; } = new();
    public List<InvoiceSummary> OverdueInvoicesList { get; set; } = new();
}

/// <summary>
/// Financial Overview Report
/// </summary>
public class FinancialOverviewReport
{
    // KPIs
    public decimal TotalRevenue { get; set; }
    public decimal TotalCosts { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal GrossMargin { get; set; }
    public decimal NetProfit { get; set; }
    public decimal NetMargin { get; set; }
    public decimal CashFlow { get; set; }
    
    // Year over Year
    public decimal RevenueYoY { get; set; }
    public decimal ProfitYoY { get; set; }
    
    // Charts Data
    public List<TimeSeriesDataItem> RevenueCostTrend { get; set; } = new();
    public List<ChartDataItem> RevenueByService { get; set; } = new();
    public List<ChartDataItem> CostBreakdown { get; set; } = new();
    public List<CostCenterSummary> CostCenterPerformance { get; set; } = new();
    public List<MonthlyFinancialSummary> MonthlyOverview { get; set; } = new();
}

/// <summary>
/// Accounts Receivable Report
/// </summary>
public class AccountsReceivableReport
{
    // KPIs
    public decimal TotalReceivables { get; set; }
    public decimal Current { get; set; }
    public decimal Days30 { get; set; }
    public decimal Days60 { get; set; }
    public decimal Days90Plus { get; set; }
    public decimal DSO { get; set; } // Days Sales Outstanding
    public int CustomersWithOverdue { get; set; }
    
    // Charts Data
    public List<AgingBucket> AgingAnalysis { get; set; } = new();
    public List<ChartDataItem> ReceivablesByClient { get; set; } = new();
    public List<TimeSeriesDataItem> CollectionTrend { get; set; } = new();
    public List<ClientReceivableSummary> TopDebtors { get; set; } = new();
}

public class AgingBucket
{
    public string? Period { get; set; }
    public int Count { get; set; }
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}

public class InvoiceSummary
{
    public int Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? ClientName { get; set; }
    public decimal Amount { get; set; }
    public decimal Outstanding { get; set; }
    public DateTime? DueDate { get; set; }
    public int DaysOverdue { get; set; }
    public string? Status { get; set; }
}

public class CostCenterSummary
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Department { get; set; }
    public decimal Budget { get; set; }
    public decimal Actual { get; set; }
    public decimal Variance { get; set; }
    public decimal VariancePercent { get; set; }
}

public class MonthlyFinancialSummary
{
    public string? Month { get; set; }
    public decimal Revenue { get; set; }
    public decimal Costs { get; set; }
    public decimal Profit { get; set; }
    public decimal Margin { get; set; }
}

public class ClientReceivableSummary
{
    public int ClientId { get; set; }
    public string? ClientName { get; set; }
    public decimal TotalOutstanding { get; set; }
    public decimal Current { get; set; }
    public decimal Overdue { get; set; }
    public int OpenInvoices { get; set; }
    public DateTime? OldestInvoice { get; set; }
}

#endregion

#region Shared DTOs

public class ChartDataItem
{
    public string? Label { get; set; }
    public decimal Value { get; set; }
    public string? Color { get; set; }
    public decimal? Percentage { get; set; }
}

public class TimeSeriesDataItem
{
    public string? Period { get; set; }
    public decimal Value { get; set; }
    public decimal? Value2 { get; set; }
    public decimal? Value3 { get; set; }
}

#endregion
