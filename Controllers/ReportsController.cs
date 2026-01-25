// =================================================================================================
// APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Presentation Layer)
// This controller is part of the Presentation Layer in Clean Architecture. It handles HTTP requests
// related to Reporting and translates them into application-level queries.
//
// META-DATA:
//   - Layer: Presentation (API Controller)
//   - Responsibility: Expose Reporting functionality via a RESTful API.
// =================================================================================================

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

    private List<AgeGroupDistributionDto> GetAgeGroupDistribution(List<ClientDetails> clients)
    {
        var ageGroups = new List<AgeGroupDistributionDto>
        {
            new AgeGroupDistributionDto { AgeGroup = "< 60" },
            new AgeGroupDistributionDto { AgeGroup = "60-69" },
            new AgeGroupDistributionDto { AgeGroup = "70-79" },
            new AgeGroupDistributionDto { AgeGroup = "80-89" },
            new AgeGroupDistributionDto { AgeGroup = ">= 90" }
        };

        foreach (var client in clients)
        {
            if (client.BirthDay.HasValue)
            {
                var age = DateTime.Today.Year - client.BirthDay.Value.Year;
                if (client.BirthDay.Value.Date > DateTime.Today.AddYears(-age)) age--;

                if (age < 60) ageGroups[0].Count++;
                else if (age < 70) ageGroups[1].Count++;
                else if (age < 80) ageGroups[2].Count++;
                else if (age < 90) ageGroups[3].Count++;
                else ageGroups[4].Count++;
            }
        }

        foreach (var group in ageGroups)
        {
            group.Percentage = clients.Count > 0 ? Math.Round((double)group.Count / clients.Count * 100, 1) : 0;
        }

        return ageGroups;
    }
}
