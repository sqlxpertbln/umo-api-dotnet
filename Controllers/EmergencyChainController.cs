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

    // ==================== ANGEHÖRIGE INFORMIEREN ====================

    /// <summary>
    /// Informiert alle Notfallkontakte (Angehörige) per SMS und/oder Anruf
    /// </summary>
    [HttpPost("alerts/{alertId}/notify-family")]
    [ProducesResponseType(typeof(NotificationResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationResultDto>> NotifyFamily(int alertId, [FromQuery] int? dispatcherId = null)
    {
        var alert = await _context.EmergencyAlerts
            .Include(a => a.Client)
            .FirstOrDefaultAsync(a => a.Id == alertId);

        if (alert == null)
            return NotFound(new { message = "Alarm nicht gefunden" });

        // Notfallkontakte des Klienten laden
        var contacts = await _context.EmergencyContacts
            .Where(c => c.ClientId == alert.ClientId)
            .OrderBy(c => c.Priority)
            .ToListAsync();

        var results = new List<ContactNotificationResult>();
        int successCount = 0;

        foreach (var contact in contacts)
        {
            var result = new ContactNotificationResult
            {
                ContactId = contact.Id,
                ContactName = $"{contact.FirstName} {contact.LastName}",
                Relationship = contact.Relationship
            };

            // SMS senden wenn aktiviert
            if (contact.NotifyBySms && !string.IsNullOrEmpty(contact.MobileNumber))
            {
                var smsResult = await _sipgateService.SendSmsAsync(
                    "s0", // SMS-ID
                    contact.MobileNumber,
                    $"NOTRUF: {alert.Client?.FirstName} {alert.Client?.LastName} hat einen Notruf ausgelöst. " +
                    $"Alarmtyp: {alert.AlertType}. Bitte kontaktieren Sie die Leitstelle."
                );
                result.SmsSent = smsResult;
                result.SmsMessage = smsResult ? "SMS gesendet" : "SMS fehlgeschlagen";
            }

            // Anruf initiieren wenn aktiviert
            if (contact.NotifyByCall && !string.IsNullOrEmpty(contact.PhoneNumber))
            {
                var callResult = await _sipgateService.InitiateCallAsync("e0", contact.PhoneNumber);
                result.CallInitiated = callResult.Success;
                result.CallMessage = callResult.Message;
            }

            // Aktion protokollieren
            var action = new EmergencyChainAction
            {
                EmergencyAlertId = alertId,
                ActionType = contact.NotifyBySms ? "SmsFamily" : "CallFamily",
                DispatcherId = dispatcherId,
                Target = $"{contact.FirstName} {contact.LastName}",
                TargetPhone = contact.MobileNumber ?? contact.PhoneNumber ?? "",
                Result = (result.SmsSent || result.CallInitiated) ? "Success" : "Failed",
                Notes = $"Beziehung: {contact.Relationship}"
            };
            _context.EmergencyChainActions.Add(action);

            if (result.SmsSent || result.CallInitiated)
                successCount++;

            results.Add(result);
        }

        // Alert aktualisieren
        alert.EmergencyChainStep = "ContactingFamily";
        alert.FamilyNotifiedTime = DateTime.UtcNow;
        alert.FamilyContactsNotified = successCount;
        alert.ContactsNotified = successCount > 0;

        await _context.SaveChangesAsync();

        return Ok(new NotificationResultDto
        {
            AlertId = alertId,
            TotalContacts = contacts.Count,
            SuccessfulNotifications = successCount,
            Results = results,
            Message = $"{successCount} von {contacts.Count} Kontakten benachrichtigt"
        });
    }

    // ==================== ARZT INFORMIEREN ====================

    /// <summary>
    /// Informiert den Hausarzt des Klienten
    /// </summary>
    [HttpPost("alerts/{alertId}/notify-doctor")]
    [ProducesResponseType(typeof(DoctorNotificationResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorNotificationResultDto>> NotifyDoctor(int alertId, [FromQuery] int? dispatcherId = null)
    {
        var alert = await _context.EmergencyAlerts
            .Include(a => a.Client)
            .FirstOrDefaultAsync(a => a.Id == alertId);

        if (alert == null)
            return NotFound(new { message = "Alarm nicht gefunden" });

        // Hausarzt des Klienten finden
        var doctor = await _context.ProfessionalProviderDetails
            .Where(p => p.Specialty == "Hausarzt" || p.Specialty == "Allgemeinmedizin")
            .FirstOrDefaultAsync();

        if (doctor == null)
        {
            return Ok(new DoctorNotificationResultDto
            {
                AlertId = alertId,
                Success = false,
                Message = "Kein Hausarzt für diesen Klienten hinterlegt"
            });
        }

        // Arzt anrufen
        var callResult = await _sipgateService.InitiateCallAsync("e0", doctor.Phone ?? "");

        // Aktion protokollieren
        var action = new EmergencyChainAction
        {
            EmergencyAlertId = alertId,
            ActionType = "CallDoctor",
            DispatcherId = dispatcherId,
            Target = $"Dr. {doctor.FirstName} {doctor.LastName}",
            TargetPhone = doctor.Phone ?? "",
            Result = callResult.Success ? "Success" : "Failed",
            Notes = $"Fachrichtung: {doctor.Specialty}"
        };
        _context.EmergencyChainActions.Add(action);

        // Alert aktualisieren
        alert.EmergencyChainStep = "ContactingDoctor";
        alert.DoctorNotifiedTime = DateTime.UtcNow;
        alert.DoctorNotified = $"Dr. {doctor.FirstName} {doctor.LastName}";

        await _context.SaveChangesAsync();

        return Ok(new DoctorNotificationResultDto
        {
            AlertId = alertId,
            DoctorName = $"Dr. {doctor.FirstName} {doctor.LastName}",
            DoctorPhone = doctor.Phone ?? "",
            Success = callResult.Success,
            Message = callResult.Success ? "Arzt wird angerufen" : "Anruf fehlgeschlagen"
        });
    }

    // ==================== RETTUNGSDIENST ALARMIEREN ====================

    /// <summary>
    /// Alarmiert den Rettungsdienst und übergibt die Medikamentenliste
    /// </summary>
    [HttpPost("alerts/{alertId}/call-ambulance")]
    [ProducesResponseType(typeof(AmbulanceCallResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AmbulanceCallResultDto>> CallAmbulance(
        int alertId, 
        [FromBody] CallAmbulanceRequest request)
    {
        var alert = await _context.EmergencyAlerts
            .Include(a => a.Client)
            .FirstOrDefaultAsync(a => a.Id == alertId);

        if (alert == null)
            return NotFound(new { message = "Alarm nicht gefunden" });

        // Medikamentenliste des Klienten laden
        var medications = await _context.ClientMedications
            .Where(m => m.ClientId == alert.ClientId && m.IsActive)
            .OrderBy(m => m.Priority)
            .ToListAsync();

        // Medikamentenliste als Text formatieren
        var medicationListText = FormatMedicationList(medications);

        // Klient-Adresse laden
        var clientDetails = await _context.ClientDetails
            .Include(c => c.Address)
            .FirstOrDefaultAsync(c => c.Id == alert.ClientId);

        // Rettungsdienst anrufen (112 oder konfigurierte Nummer)
        var ambulanceNumber = request.AmbulanceNumber ?? "112";
        var callResult = await _sipgateService.InitiateCallAsync("e0", ambulanceNumber);

        // Aktion protokollieren
        var action = new EmergencyChainAction
        {
            EmergencyAlertId = alertId,
            ActionType = "CallAmbulance",
            DispatcherId = request.DispatcherId,
            Target = "Rettungsdienst",
            TargetPhone = ambulanceNumber,
            Result = callResult.Success ? "Success" : "Failed",
            MedicationListProvided = true,
            MedicationListContent = medicationListText,
            Notes = $"Alarmtyp: {alert.AlertType}, Priorität: {alert.Priority}"
        };
        _context.EmergencyChainActions.Add(action);

        // Alert aktualisieren
        alert.EmergencyChainStep = "ContactingAmbulance";
        alert.AmbulanceCalled = true;
        alert.AmbulanceCalledTime = DateTime.UtcNow;
        alert.MedicationListProvided = true;
        alert.MedicationListProvidedTime = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(request.IncidentNumber))
            alert.AmbulanceIncidentNumber = request.IncidentNumber;

        await _context.SaveChangesAsync();

        return Ok(new AmbulanceCallResultDto
        {
            AlertId = alertId,
            Success = callResult.Success,
            MedicationList = medications.Select(m => new MedicationDto
            {
                Name = m.MedicationName,
                Dosage = m.Dosage ?? "",
                Frequency = m.Frequency ?? "",
                EmergencyNotes = m.EmergencyNotes ?? "",
                Category = m.Category ?? ""
            }).ToList(),
            MedicationListText = medicationListText,
            ClientAddress = clientDetails?.Address != null 
                ? $"{clientDetails.Address.Street}, {clientDetails.Address.ZipCode} {clientDetails.Address.City}"
                : "Adresse nicht verfügbar",
            Message = callResult.Success 
                ? "Rettungsdienst wird alarmiert, Medikamentenliste bereit" 
                : "Anruf fehlgeschlagen"
        });
    }

    // ==================== MEDIKAMENTENLISTE ABRUFEN ====================

    /// <summary>
    /// Ruft die Medikamentenliste eines Klienten ab
    /// </summary>
    [HttpGet("clients/{clientId}/medications")]
    [ProducesResponseType(typeof(ClientMedicationListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientMedicationListDto>> GetClientMedications(int clientId)
    {
        var client = await _context.ClientDetails
            .FirstOrDefaultAsync(c => c.Id == clientId);

        if (client == null)
            return NotFound(new { message = "Klient nicht gefunden" });

        var medications = await _context.ClientMedications
            .Where(m => m.ClientId == clientId && m.IsActive)
            .OrderBy(m => m.Priority)
            .ToListAsync();

        return Ok(new ClientMedicationListDto
        {
            ClientId = clientId,
            ClientName = $"{client.FirstName} {client.LastName}",
            Medications = medications.Select(m => new MedicationDto
            {
                Id = m.Id,
                Name = m.MedicationName,
                ActiveIngredient = m.ActiveIngredient ?? "",
                Dosage = m.Dosage ?? "",
                Frequency = m.Frequency ?? "",
                TimeOfDay = m.TimeOfDay ?? "",
                PrescribedBy = m.PrescribedBy ?? "",
                EmergencyNotes = m.EmergencyNotes ?? "",
                Category = m.Category ?? "",
                Priority = m.Priority
            }).ToList(),
            FormattedList = FormatMedicationList(medications)
        });
    }

    /// <summary>
    /// Fügt ein Medikament zur Medikamentenliste eines Klienten hinzu
    /// </summary>
    [HttpPost("clients/{clientId}/medications")]
    [ProducesResponseType(typeof(ClientMedication), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientMedication>> AddClientMedication(int clientId, [FromBody] AddMedicationRequest request)
    {
        var client = await _context.ClientDetails.FindAsync(clientId);
        if (client == null)
            return NotFound(new { message = "Klient nicht gefunden" });

        var medication = new ClientMedication
        {
            ClientId = clientId,
            MandantId = client.MandantId,
            MedicationName = request.MedicationName,
            ActiveIngredient = request.ActiveIngredient,
            Dosage = request.Dosage,
            Frequency = request.Frequency,
            TimeOfDay = request.TimeOfDay,
            PrescribedBy = request.PrescribedBy,
            PrescribedDate = request.PrescribedDate,
            EmergencyNotes = request.EmergencyNotes,
            Category = request.Category,
            Priority = request.Priority,
            IsActive = true
        };

        _context.ClientMedications.Add(medication);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetClientMedications), new { clientId }, medication);
    }

    // ==================== KONFERENZSCHALTUNG / MAKELN ====================

    /// <summary>
    /// Startet eine Konferenzschaltung für den Notruf
    /// </summary>
    [HttpPost("alerts/{alertId}/start-conference")]
    [ProducesResponseType(typeof(ConferenceResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConferenceResultDto>> StartConference(int alertId, [FromQuery] int? dispatcherId = null)
    {
        var alert = await _context.EmergencyAlerts
            .Include(a => a.Client)
            .FirstOrDefaultAsync(a => a.Id == alertId);

        if (alert == null)
            return NotFound(new { message = "Alarm nicht gefunden" });

        // Konferenz starten
        alert.ConferenceActive = true;
        alert.EmergencyChainStep = "InConference";
        alert.ConferenceParticipants = "Disponent";

        // Aktion protokollieren
        var action = new EmergencyChainAction
        {
            EmergencyAlertId = alertId,
            ActionType = "StartConference",
            DispatcherId = dispatcherId,
            Target = "Konferenzschaltung",
            Result = "Success",
            Notes = "Konferenzschaltung gestartet"
        };
        _context.EmergencyChainActions.Add(action);

        await _context.SaveChangesAsync();

        return Ok(new ConferenceResultDto
        {
            AlertId = alertId,
            ConferenceActive = true,
            Participants = new List<string> { "Disponent" },
            Message = "Konferenzschaltung gestartet"
        });
    }

    /// <summary>
    /// Fügt einen Teilnehmer zur Konferenzschaltung hinzu (Makeln)
    /// </summary>
    [HttpPost("alerts/{alertId}/add-to-conference")]
    [ProducesResponseType(typeof(ConferenceResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConferenceResultDto>> AddToConference(
        int alertId, 
        [FromBody] AddToConferenceRequest request)
    {
        var alert = await _context.EmergencyAlerts.FindAsync(alertId);
        if (alert == null)
            return NotFound(new { message = "Alarm nicht gefunden" });

        if (!alert.ConferenceActive)
            return BadRequest(new { message = "Keine aktive Konferenzschaltung" });

        // Teilnehmer anrufen und zur Konferenz hinzufügen
        var callResult = await _sipgateService.InitiateCallAsync("e0", request.PhoneNumber);

        // Teilnehmer zur Liste hinzufügen
        var participants = alert.ConferenceParticipants.Split(',').ToList();
        participants.Add(request.ParticipantName);
        alert.ConferenceParticipants = string.Join(",", participants);

        // Aktion protokollieren
        var action = new EmergencyChainAction
        {
            EmergencyAlertId = alertId,
            ActionType = "AddToConference",
            DispatcherId = request.DispatcherId,
            Target = request.ParticipantName,
            TargetPhone = request.PhoneNumber,
            Result = callResult.Success ? "Success" : "Failed",
            Notes = $"Rolle: {request.Role}"
        };
        _context.EmergencyChainActions.Add(action);

        await _context.SaveChangesAsync();

        return Ok(new ConferenceResultDto
        {
            AlertId = alertId,
            ConferenceActive = true,
            Participants = participants,
            Message = callResult.Success 
                ? $"{request.ParticipantName} wird zur Konferenz hinzugefügt"
                : "Anruf fehlgeschlagen"
        });
    }

    /// <summary>
    /// Beendet die Konferenzschaltung
    /// </summary>
    [HttpPost("alerts/{alertId}/end-conference")]
    [ProducesResponseType(typeof(ConferenceResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ConferenceResultDto>> EndConference(int alertId, [FromQuery] int? dispatcherId = null)
    {
        var alert = await _context.EmergencyAlerts.FindAsync(alertId);
        if (alert == null)
            return NotFound(new { message = "Alarm nicht gefunden" });

        alert.ConferenceActive = false;

        // Aktion protokollieren
        var action = new EmergencyChainAction
        {
            EmergencyAlertId = alertId,
            ActionType = "EndConference",
            DispatcherId = dispatcherId,
            Target = "Konferenzschaltung",
            Result = "Success",
            Notes = $"Teilnehmer: {alert.ConferenceParticipants}"
        };
        _context.EmergencyChainActions.Add(action);

        await _context.SaveChangesAsync();

        return Ok(new ConferenceResultDto
        {
            AlertId = alertId,
            ConferenceActive = false,
            Participants = new List<string>(),
            Message = "Konferenzschaltung beendet"
        });
    }

    // ==================== NOTFALLKETTE STATUS ====================

    /// <summary>
    /// Ruft den aktuellen Status der Notfallkette ab
    /// </summary>
    [HttpGet("alerts/{alertId}/chain-status")]
    [ProducesResponseType(typeof(EmergencyChainFullStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmergencyChainFullStatusDto>> GetChainStatus(int alertId)
    {
        var alert = await _context.EmergencyAlerts
            .Include(a => a.Client)
            .Include(a => a.EmergencyDevice)
            .Include(a => a.ChainActions)
                .ThenInclude(ca => ca.Dispatcher)
            .FirstOrDefaultAsync(a => a.Id == alertId);

        if (alert == null)
            return NotFound(new { message = "Alarm nicht gefunden" });

        var clientInfo = await GetClientEmergencyInfo(alert.ClientId);

        return Ok(new EmergencyChainFullStatusDto
        {
            AlertId = alert.Id,
            AlertType = alert.AlertType,
            Priority = alert.Priority,
            Status = alert.Status,
            CurrentStep = alert.EmergencyChainStep,
            AlertTime = alert.AlertTime,
            
            // Notfallketten-Status
            FamilyNotified = alert.ContactsNotified,
            FamilyNotifiedTime = alert.FamilyNotifiedTime,
            FamilyContactsNotified = alert.FamilyContactsNotified,
            
            DoctorNotified = !string.IsNullOrEmpty(alert.DoctorNotified),
            DoctorNotifiedTime = alert.DoctorNotifiedTime,
            DoctorName = alert.DoctorNotified,
            
            AmbulanceCalled = alert.AmbulanceCalled,
            AmbulanceCalledTime = alert.AmbulanceCalledTime,
            AmbulanceIncidentNumber = alert.AmbulanceIncidentNumber,
            
            MedicationListProvided = alert.MedicationListProvided,
            MedicationListProvidedTime = alert.MedicationListProvidedTime,
            
            ConferenceActive = alert.ConferenceActive,
            ConferenceParticipants = alert.ConferenceParticipants?.Split(',').ToList() ?? new List<string>(),
            
            // Klient-Info
            ClientInfo = clientInfo,
            
            // Aktionen-Protokoll
            Actions = alert.ChainActions.OrderBy(a => a.ActionTime).Select(a => new ChainActionDto
            {
                Id = a.Id,
                ActionType = a.ActionType,
                ActionTime = a.ActionTime,
                Target = a.Target,
                TargetPhone = a.TargetPhone,
                Result = a.Result,
                Notes = a.Notes,
                DispatcherName = a.Dispatcher != null ? $"{a.Dispatcher.FirstName} {a.Dispatcher.LastName}" : null
            }).ToList()
        });
    }

    // ==================== HILFSMETHODEN ====================

    private async Task<ClientEmergencyInfoDto?> GetClientEmergencyInfo(int? clientId)
    {
        if (clientId == null) return null;

        var client = await _context.ClientDetails
            .Include(c => c.Address)
            .FirstOrDefaultAsync(c => c.Id == clientId);

        if (client == null) return null;

        var medications = await _context.ClientMedications
            .Where(m => m.ClientId == clientId && m.IsActive)
            .OrderBy(m => m.Priority)
            .ToListAsync();

        var contacts = await _context.EmergencyContacts
            .Where(c => c.ClientId == clientId)
            .OrderBy(c => c.Priority)
            .ToListAsync();

        var diseases = await _context.ClientDiseases
            .Where(d => d.ClientId == clientId)
            .ToListAsync();

        return new ClientEmergencyInfoDto
        {
            ClientId = client.Id,
            Name = $"{client.FirstName} {client.LastName}",
            BirthDate = client.BirthDay,
            Address = client.Address != null 
                ? $"{client.Address.Street}, {client.Address.ZipCode} {client.Address.City}"
                : null,
            Medications = medications.Select(m => new MedicationDto
            {
                Name = m.MedicationName,
                Dosage = m.Dosage ?? "",
                Frequency = m.Frequency ?? "",
                EmergencyNotes = m.EmergencyNotes ?? "",
                Category = m.Category ?? ""
            }).ToList(),
            EmergencyContacts = contacts.Select(c => new EmergencyContactInfoDto
            {
                Name = $"{c.FirstName} {c.LastName}",
                Relationship = c.Relationship,
                Phone = c.PhoneNumber,
                Mobile = c.MobileNumber,
                Priority = c.Priority,
                HasKey = c.HasKey
            }).ToList(),
            Diseases = diseases.Select(d => d.DiseaseName ?? "").ToList(),
            MedicalNotes = client.Note
        };
    }

    private string FormatMedicationList(List<ClientMedication> medications)
    {
        if (!medications.Any())
            return "Keine Medikamente hinterlegt";

        var lines = new List<string>
        {
            "=== MEDIKAMENTENLISTE ===",
            ""
        };

        foreach (var med in medications)
        {
            lines.Add($"• {med.MedicationName}");
            if (!string.IsNullOrEmpty(med.Dosage))
                lines.Add($"  Dosierung: {med.Dosage}");
            if (!string.IsNullOrEmpty(med.Frequency))
                lines.Add($"  Einnahme: {med.Frequency}");
            if (!string.IsNullOrEmpty(med.EmergencyNotes))
                lines.Add($"  ⚠️ WICHTIG: {med.EmergencyNotes}");
            lines.Add("");
        }

        return string.Join("\n", lines);
    }
}

// ==================== DTOs ====================

public class EmergencyChainStatusDto
{
    public int AlertId { get; set; }
    public string CurrentStep { get; set; } = string.Empty;
    public ClientEmergencyInfoDto? ClientInfo { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class NotificationResultDto
{
    public int AlertId { get; set; }
    public int TotalContacts { get; set; }
    public int SuccessfulNotifications { get; set; }
    public List<ContactNotificationResult> Results { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class ContactNotificationResult
{
    public int ContactId { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public bool SmsSent { get; set; }
    public string? SmsMessage { get; set; }
    public bool CallInitiated { get; set; }
    public string? CallMessage { get; set; }
}

public class DoctorNotificationResultDto
{
    public int AlertId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorPhone { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class CallAmbulanceRequest
{
    public int? DispatcherId { get; set; }
    public string? AmbulanceNumber { get; set; }
    public string? IncidentNumber { get; set; }
}

public class AmbulanceCallResultDto
{
    public int AlertId { get; set; }
    public bool Success { get; set; }
    public List<MedicationDto> MedicationList { get; set; } = new();
    public string MedicationListText { get; set; } = string.Empty;
    public string ClientAddress { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class ClientMedicationListDto
{
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public List<MedicationDto> Medications { get; set; } = new();
    public string FormattedList { get; set; } = string.Empty;
}

public class MedicationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ActiveIngredient { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string TimeOfDay { get; set; } = string.Empty;
    public string PrescribedBy { get; set; } = string.Empty;
    public string EmergencyNotes { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Priority { get; set; }
}

public class AddMedicationRequest
{
    public string MedicationName { get; set; } = string.Empty;
    public string? ActiveIngredient { get; set; }
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public string? TimeOfDay { get; set; }
    public string? PrescribedBy { get; set; }
    public DateTime? PrescribedDate { get; set; }
    public string? EmergencyNotes { get; set; }
    public string? Category { get; set; }
    public int Priority { get; set; } = 5;
}

public class ConferenceResultDto
{
    public int AlertId { get; set; }
    public bool ConferenceActive { get; set; }
    public List<string> Participants { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class AddToConferenceRequest
{
    public int? DispatcherId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // Family, Doctor, Ambulance
}

public class EmergencyChainFullStatusDto
{
    public int AlertId { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public DateTime AlertTime { get; set; }
    
    public bool FamilyNotified { get; set; }
    public DateTime? FamilyNotifiedTime { get; set; }
    public int FamilyContactsNotified { get; set; }
    
    public bool DoctorNotified { get; set; }
    public DateTime? DoctorNotifiedTime { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    
    public bool AmbulanceCalled { get; set; }
    public DateTime? AmbulanceCalledTime { get; set; }
    public string AmbulanceIncidentNumber { get; set; } = string.Empty;
    
    public bool MedicationListProvided { get; set; }
    public DateTime? MedicationListProvidedTime { get; set; }
    
    public bool ConferenceActive { get; set; }
    public List<string> ConferenceParticipants { get; set; } = new();
    
    public ClientEmergencyInfoDto? ClientInfo { get; set; }
    public List<ChainActionDto> Actions { get; set; } = new();
}

public class ClientEmergencyInfoDto
{
    public int ClientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string? Address { get; set; }
    public List<MedicationDto> Medications { get; set; } = new();
    public List<EmergencyContactInfoDto> EmergencyContacts { get; set; } = new();
    public List<string> Diseases { get; set; } = new();
    public string? MedicalNotes { get; set; }
}

public class EmergencyContactInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public int Priority { get; set; }
    public bool HasKey { get; set; }
}

public class ChainActionDto
{
    public int Id { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public DateTime ActionTime { get; set; }
    public string Target { get; set; } = string.Empty;
    public string TargetPhone { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string? DispatcherName { get; set; }
}
