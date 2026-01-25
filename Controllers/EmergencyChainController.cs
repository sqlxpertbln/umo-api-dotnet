// =================================================================================================
// APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Presentation Layer)
// This controller is part of the Presentation Layer in Clean Architecture. It handles HTTP requests
// related to EmergencyChain entities and translates them into application-level commands or queries.
//
// META-DATA:
//   - Layer: Presentation (API Controller)
//   - Responsibility: Expose EmergencyChain-related functionality via a RESTful API.
// =================================================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMOApi.Data;
using UMOApi.Models;
using UMOApi.Services;

namespace UMOApi.Controllers;

/// <summary>
/// Controller für die Notfallkette - Hausnotruf-Workflow
/// </summary>
[ApiController]
[Route("[controller]")]
[Tags("Notfallkette")]
public class EmergencyChainController : ControllerBase
{
    private readonly UMOApiDbContext _context;
    private readonly SipgateService _sipgateService;
    private readonly ILogger<EmergencyChainController> _logger;

    public EmergencyChainController(
        UMOApiDbContext context, 
        SipgateService sipgateService,
        ILogger<EmergencyChainController> logger)
    {
        _context = context;
        _sipgateService = sipgateService;
        _logger = logger;
    }

    // ==================== NOTFALLKETTE STARTEN ====================

    /// <summary>
    /// Startet die Notfallkette für einen Alarm
    /// </summary>
    [HttpPost("alerts/{alertId}/start-chain")]
    [ProducesResponseType(typeof(EmergencyChainStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmergencyChainStatusDto>> StartEmergencyChain(int alertId)
    {
        var alert = await _context.EmergencyAlerts
            .Include(a => a.Client)
            .Include(a => a.EmergencyDevice)
            .FirstOrDefaultAsync(a => a.Id == alertId);

        if (alert == null)
            return NotFound(new { message = "Alarm nicht gefunden" });

        // Notfallkette starten
        alert.EmergencyChainStep = "Initial";
        alert.Status = "InProgress";
        
        await _context.SaveChangesAsync();

        // Klient-Daten laden
        var clientInfo = await GetClientEmergencyInfo(alert.ClientId);

        return Ok(new EmergencyChainStatusDto
        {
            AlertId = alert.Id,
            CurrentStep = alert.EmergencyChainStep,
            ClientInfo = clientInfo,
            Message = "Notfallkette gestartet"
        });
    }

    private async Task<ClientEmergencyInfoDto> GetClientEmergencyInfo(int clientId)
    {
        var client = await _context.ClientDetails
            .Include(c => c.Address)
            .ThenInclude(a => a.City)
            .FirstOrDefaultAsync(c => c.Id == clientId);

        if (client == null)
        {
            return new ClientEmergencyInfoDto { Name = "Unknown" };
        }

        return new ClientEmergencyInfoDto
        {
            ClientId = client.Id,
            Name = $"{client.FirstName} {client.LastName}",
            Address = $"{client.Address.Street} {client.Address.HouseNumber}, {client.Address.ZipCode} {client.Address.City.Name}",
            PhoneNumber = client.Phones.FirstOrDefault(p => p.IsPrimary)?.PhoneNumber ?? "N/A",
            DateOfBirth = client.BirthDay
        };
    }
}
