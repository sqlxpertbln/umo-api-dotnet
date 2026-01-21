namespace UMOApi.Models;

// ============================================
// MARKETING REPORTS
// ============================================

public class MarketingDashboardDto
{
    public int TotalClients { get; set; }
    public int NewClientsThisMonth { get; set; }
    public int NewClientsLastMonth { get; set; }
    public double GrowthRatePercent { get; set; }
    public List<RegionDistributionDto> ClientsByRegion { get; set; } = new();
    public List<AgeGroupDistributionDto> ClientsByAgeGroup { get; set; } = new();
    public List<MonthlyTrendDto> ClientAcquisitionTrend { get; set; } = new();
    public List<LanguageDistributionDto> ClientsByLanguage { get; set; } = new();
}

public class RegionDistributionDto
{
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int ClientCount { get; set; }
    public double Percentage { get; set; }
}

public class AgeGroupDistributionDto
{
    public string AgeGroup { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class LanguageDistributionDto
{
    public string Language { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

// ============================================
// VERTRIEB (SALES) REPORTS
// ============================================

public class SalesDashboardDto
{
    public int TotalDevices { get; set; }
    public int ActiveDevices { get; set; }
    public int InactiveDevices { get; set; }
    public double DeviceUtilizationPercent { get; set; }
    public List<DeviceTypeDistributionDto> DevicesByType { get; set; } = new();
    public List<ProviderPerformanceDto> TopProviders { get; set; } = new();
    public List<TarifDistributionDto> ClientsByTarif { get; set; } = new();
    public List<MonthlyTrendDto> DeviceDeploymentTrend { get; set; } = new();
}

public class DeviceTypeDistributionDto
{
    public string DeviceType { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Active { get; set; }
    public int Inactive { get; set; }
    public double UtilizationPercent { get; set; }
}

public class ProviderPerformanceDto
{
    public int ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string ProviderType { get; set; } = string.Empty;
    public int ClientCount { get; set; }
    public int DeviceCount { get; set; }
    public double PerformanceScore { get; set; }
}

public class TarifDistributionDto
{
    public string TarifName { get; set; } = string.Empty;
    public int ClientCount { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public double Percentage { get; set; }
}

// ============================================
// GESCHÄFTSBEREICH (OPERATIONS) REPORTS
// ============================================

public class OperationsDashboardDto
{
    public int TotalActiveClients { get; set; }
    public int TotalInactiveClients { get; set; }
    public int ClientsWithDevices { get; set; }
    public int ClientsWithoutDevices { get; set; }
    public double CapacityUtilizationPercent { get; set; }
    public List<StatusDistributionDto> ClientsByStatus { get; set; } = new();
    public List<PriorityDistributionDto> ClientsByPriority { get; set; } = new();
    public List<ProviderWorkloadDto> ProviderWorkload { get; set; } = new();
    public List<MonthlyTrendDto> StatusChangeTrend { get; set; } = new();
}

public class StatusDistributionDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
    public string ColorCode { get; set; } = string.Empty;
}

public class PriorityDistributionDto
{
    public string Priority { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class ProviderWorkloadDto
{
    public int ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public int AssignedClients { get; set; }
    public int MaxCapacity { get; set; }
    public double WorkloadPercent { get; set; }
}

// ============================================
// FINANZBUCHHALTUNG (FINANCE) REPORTS
// ============================================

public class FinanceDashboardDto
{
    public decimal TotalMonthlyRevenue { get; set; }
    public decimal TotalYearlyRevenue { get; set; }
    public decimal AverageRevenuePerClient { get; set; }
    public decimal TotalCosts { get; set; }
    public decimal NetProfit { get; set; }
    public double ProfitMarginPercent { get; set; }
    public List<RevenueByTarifDto> RevenueByTarif { get; set; } = new();
    public List<CostBreakdownDto> CostBreakdown { get; set; } = new();
    public List<MonthlyFinanceDto> MonthlyFinanceTrend { get; set; } = new();
    public List<TopClientRevenueDto> TopClientsByRevenue { get; set; } = new();
}

public class RevenueByTarifDto
{
    public string TarifName { get; set; } = string.Empty;
    public int ClientCount { get; set; }
    public decimal MonthlyRate { get; set; }
    public decimal TotalMonthlyRevenue { get; set; }
    public double RevenueSharePercent { get; set; }
}

public class CostBreakdownDto
{
    public string CostCategory { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public double Percentage { get; set; }
}

public class MonthlyFinanceDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Costs { get; set; }
    public decimal Profit { get; set; }
}

public class TopClientRevenueDto
{
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string TarifName { get; set; } = string.Empty;
    public decimal MonthlyRevenue { get; set; }
    public decimal YearlyRevenue { get; set; }
}

// ============================================
// CONTROLLING REPORTS
// ============================================

public class ControllingDashboardDto
{
    public List<KpiDto> KeyPerformanceIndicators { get; set; } = new();
    public List<YearOverYearComparisonDto> YearOverYearComparison { get; set; } = new();
    public List<EfficiencyMetricDto> EfficiencyMetrics { get; set; } = new();
    public List<TrendAnalysisDto> TrendAnalysis { get; set; } = new();
    public List<BenchmarkDto> Benchmarks { get; set; } = new();
}

public class KpiDto
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal CurrentValue { get; set; }
    public decimal TargetValue { get; set; }
    public decimal PreviousValue { get; set; }
    public double ChangePercent { get; set; }
    public string Status { get; set; } = string.Empty; // "good", "warning", "critical"
    public string Unit { get; set; } = string.Empty;
}

public class YearOverYearComparisonDto
{
    public string Metric { get; set; } = string.Empty;
    public decimal CurrentYear { get; set; }
    public decimal PreviousYear { get; set; }
    public double ChangePercent { get; set; }
}

public class EfficiencyMetricDto
{
    public string MetricName { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Trend { get; set; } = string.Empty; // "up", "down", "stable"
}

public class TrendAnalysisDto
{
    public string Period { get; set; } = string.Empty;
    public decimal Clients { get; set; }
    public decimal Revenue { get; set; }
    public decimal Devices { get; set; }
    public decimal Efficiency { get; set; }
}

public class BenchmarkDto
{
    public string Category { get; set; } = string.Empty;
    public decimal ActualValue { get; set; }
    public decimal BenchmarkValue { get; set; }
    public double PerformancePercent { get; set; }
}

// ============================================
// RECHNUNGSWESEN / ABRECHNUNGEN (BILLING) REPORTS
// ============================================

public class BillingDashboardDto
{
    public decimal TotalBilled { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalOutstanding { get; set; }
    public int InvoiceCount { get; set; }
    public int OverdueInvoices { get; set; }
    public double CollectionRatePercent { get; set; }
    public List<InvoiceMethodDistributionDto> InvoicesByMethod { get; set; } = new();
    public List<PaymentStatusDto> PaymentStatus { get; set; } = new();
    public List<AgingReportDto> AgingReport { get; set; } = new();
    public List<MonthlyBillingDto> MonthlyBillingTrend { get; set; } = new();
    public List<ClientBillingDto> TopOutstandingClients { get; set; } = new();
}

public class InvoiceMethodDistributionDto
{
    public string Method { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
    public double Percentage { get; set; }
}

public class PaymentStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Amount { get; set; }
    public double Percentage { get; set; }
}

public class AgingReportDto
{
    public string AgingBucket { get; set; } = string.Empty; // "Current", "1-30 days", "31-60 days", etc.
    public int InvoiceCount { get; set; }
    public decimal Amount { get; set; }
    public double Percentage { get; set; }
}

public class MonthlyBillingDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Billed { get; set; }
    public decimal Collected { get; set; }
    public decimal Outstanding { get; set; }
}

public class ClientBillingDto
{
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public decimal TotalBilled { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Outstanding { get; set; }
    public int DaysOverdue { get; set; }
}

// ============================================
// SHARED / COMMON
// ============================================

public class MonthlyTrendDto
{
    public string Month { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Value { get; set; }
}

public class ReportSummaryDto
{
    public string ReportName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string Period { get; set; } = string.Empty;
    public Dictionary<string, object> KeyMetrics { get; set; } = new();
}
