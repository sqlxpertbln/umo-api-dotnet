// =================================================================================================
// APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Presentation Layer)
// This controller is part of the Presentation Layer in Clean Architecture. It handles HTTP requests
// related to Business Intelligence and Reporting, translating them into application-level queries.
//
// META-DATA:
//   - Layer: Presentation (API Controller)
//   - Responsibility: Expose business intelligence and reporting functionality via a RESTful API.
// =================================================================================================

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

    #endregion
}
