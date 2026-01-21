using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMOApi.Data;
using UMOApi.Models;
using UMOApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMOApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ServiceHubController : ControllerBase
    {
        private readonly UMOApiDbContext _context;
        private readonly ISipgateService _sipgateService;

        public ServiceHubController(UMOApiDbContext context, ISipgateService sipgateService)
        {
            _context = context;
            _sipgateService = sipgateService;
        }

        // ==================== DASHBOARD ====================

        /// <summary>
        /// Hauptdashboard der Leitstelle mit Übersicht aller aktiven Alarme und Anrufe
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<ServiceHubDashboardDto>> GetDashboard()
        {
            var today = DateTime.UtcNow.Date;

            var activeAlerts = await _context.EmergencyAlerts
                .Where(a => a.Status != "Resolved" && a.Status != "Cancelled")
                .Include(a => a.Client)
                .Include(a => a.EmergencyDevice)
                .OrderByDescending(a => a.AlertTime)
                .Take(20)
                .ToListAsync();

            var activeCalls = await _context.CallLogs
                .Where(c => c.Status == "Ringing" || c.Status == "Connected" || c.Status == "OnHold")
                .Include(c => c.Client)
                .Include(c => c.Dispatcher)
                .OrderByDescending(c => c.StartTime)
                .ToListAsync();

            var dispatchers = await _context.Dispatchers.ToListAsync();

            var dashboard = new ServiceHubDashboardDto
            {
                ActiveAlerts = activeAlerts.Count,
                CriticalAlerts = activeAlerts.Count(a => a.Priority == "Critical"),
                OnlineDispatchers = dispatchers.Count(d => d.Status == "Online" || d.Status == "OnCall"),
                TotalDispatchers = dispatchers.Count,
                ActiveCalls = activeCalls.Count,
                OnlineDevices = await _context.EmergencyDevices.CountAsync(d => d.IsOnline),
                TotalDevices = await _context.EmergencyDevices.CountAsync(),
                CallsToday = await _context.CallLogs.CountAsync(c => c.StartTime.Date == today),
                AlertsToday = await _context.EmergencyAlerts.CountAsync(a => a.AlertTime.Date == today),
                AverageResponseTime = await CalculateAverageResponseTimeAsync(),
                RecentAlerts = activeAlerts.Select(a => new AlertSummaryDto
                {
                    Id = a.Id,
                    AlertType = a.AlertType,
                    Priority = a.Priority,
                    Status = a.Status,
                    ClientName = a.Client != null ? $"{a.Client.FirstName} {a.Client.LastName}" : "Unbekannt",
                    DeviceName = a.EmergencyDevice?.DeviceName ?? "Unbekannt",
                    AlertTime = a.AlertTime,
                    TimeAgo = GetTimeAgo(a.AlertTime),
                    HeartRate = a.HeartRate,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude
                }).ToList(),
                ActiveCallsList = activeCalls.Select(c => new CallSummaryDto
                {
                    Id = c.Id,
                    SipgateCallId = c.SipgateCallId,
                    Direction = c.Direction,
                    CallerNumber = c.CallerNumber,
                    CalleeNumber = c.CalleeNumber,
                    ClientName = c.Client != null ? $"{c.Client.FirstName} {c.Client.LastName}" : "",
                    DispatcherName = c.Dispatcher != null ? $"{c.Dispatcher.FirstName} {c.Dispatcher.LastName}" : "",
                    Status = c.Status,
                    StartTime = c.StartTime,
                    DurationSeconds = (int)(DateTime.UtcNow - c.StartTime).TotalSeconds,
                    CallType = c.CallType
                }).ToList(),
                DispatcherStatuses = dispatchers.Select(d => new DispatcherStatusDto
                {
                    Id = d.Id,
                    Name = $"{d.FirstName} {d.LastName}",
                    Status = d.Status,
                    IsAvailable = d.IsAvailable,
                    CurrentCallCount = d.CurrentCallCount,
                    Extension = d.Extension
                }).ToList()
            };

            return Ok(dashboard);
        }

        // ==================== ALERTS ====================

        /// <summary>
        /// Liste aller Alarme mit Filteroptionen
        /// </summary>
        [HttpGet("alerts")]
        public async Task<ActionResult<List<AlertSummaryDto>>> GetAlerts(
            [FromQuery] string? status = null,
            [FromQuery] string? priority = null,
            [FromQuery] int limit = 50)
        {
            var query = _context.EmergencyAlerts
                .Include(a => a.Client)
                .Include(a => a.EmergencyDevice)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);

            if (!string.IsNullOrEmpty(priority))
                query = query.Where(a => a.Priority == priority);

            var alerts = await query
                .OrderByDescending(a => a.AlertTime)
                .Take(limit)
                .Select(a => new AlertSummaryDto
                {
                    Id = a.Id,
                    AlertType = a.AlertType,
                    Priority = a.Priority,
                    Status = a.Status,
                    ClientName = a.Client != null ? $"{a.Client.FirstName} {a.Client.LastName}" : "Unbekannt",
                    DeviceName = a.EmergencyDevice != null ? a.EmergencyDevice.DeviceName : "Unbekannt",
                    AlertTime = a.AlertTime,
                    HeartRate = a.HeartRate,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude
                })
                .ToListAsync();

            foreach (var alert in alerts)
            {
                alert.TimeAgo = GetTimeAgo(alert.AlertTime);
            }

            return Ok(alerts);
        }

        /// <summary>
        /// Detailansicht eines Alarms mit allen zugehörigen Informationen
        /// </summary>
        [HttpGet("alerts/{id}")]
        public async Task<ActionResult<AlertDetailDto>> GetAlertDetail(int id)
        {
            var alert = await _context.EmergencyAlerts
                .Include(a => a.Client)
                .Include(a => a.EmergencyDevice)
                .Include(a => a.AcknowledgedByDispatcher)
                .Include(a => a.ResolvedByDispatcher)
                .Include(a => a.CallLogs)
                    .ThenInclude(c => c.Dispatcher)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (alert == null)
                return NotFound();

            var emergencyContacts = alert.ClientId.HasValue
                ? await _context.EmergencyContacts
                    .Where(c => c.ClientId == alert.ClientId)
                    .OrderBy(c => c.Priority)
                    .ToListAsync()
                : new List<EmergencyContact>();

            var detail = new AlertDetailDto
            {
                Id = alert.Id,
                AlertType = alert.AlertType,
                Priority = alert.Priority,
                Status = alert.Status,
                AlertTime = alert.AlertTime,
                AcknowledgedTime = alert.AcknowledgedTime,
                ResolvedTime = alert.ResolvedTime,
                Resolution = alert.Resolution,
                Notes = alert.Notes,
                AmbulanceCalled = alert.AmbulanceCalled,
                ContactsNotified = alert.ContactsNotified,
                Latitude = alert.Latitude,
                Longitude = alert.Longitude,
                HeartRate = alert.HeartRate,
                ClientId = alert.ClientId,
                ClientName = alert.Client != null ? $"{alert.Client.FirstName} {alert.Client.LastName}" : "",
                ClientPhone = "", // Phone not in Client model
                ClientBirthDate = alert.Client?.BirthDay,
                DeviceId = alert.EmergencyDeviceId,
                DeviceName = alert.EmergencyDevice?.DeviceName ?? "",
                DeviceType = alert.EmergencyDevice?.DeviceType ?? "",
                AcknowledgedBy = alert.AcknowledgedByDispatcher != null 
                    ? $"{alert.AcknowledgedByDispatcher.FirstName} {alert.AcknowledgedByDispatcher.LastName}" : "",
                ResolvedBy = alert.ResolvedByDispatcher != null 
                    ? $"{alert.ResolvedByDispatcher.FirstName} {alert.ResolvedByDispatcher.LastName}" : "",
                EmergencyContacts = emergencyContacts.Select(c => new EmergencyContactDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Relationship = c.Relationship,
                    PhoneNumber = c.PhoneNumber,
                    MobileNumber = c.MobileNumber,
                    Email = c.Email,
                    Priority = c.Priority,
                    IsAvailable24h = c.IsAvailable24h,
                    HasKey = c.HasKey,
                    Notes = c.Notes
                }).ToList(),
                RelatedCalls = alert.CallLogs.Select(c => new CallSummaryDto
                {
                    Id = c.Id,
                    Direction = c.Direction,
                    CallerNumber = c.CallerNumber,
                    CalleeNumber = c.CalleeNumber,
                    DispatcherName = c.Dispatcher != null ? $"{c.Dispatcher.FirstName} {c.Dispatcher.LastName}" : "",
                    Status = c.Status,
                    StartTime = c.StartTime,
                    DurationSeconds = c.DurationSeconds,
                    CallType = c.CallType
                }).ToList()
            };

            return Ok(detail);
        }

        /// <summary>
        /// Neuen Alarm erstellen (z.B. von Webhook oder manuell)
        /// </summary>
        [HttpPost("alerts")]
        public async Task<ActionResult<AlertSummaryDto>> CreateAlert([FromBody] CreateAlertDto dto)
        {
            // Versuche Client anhand der Telefonnummer zu identifizieren
            Client? client = null;
            if (dto.ClientId.HasValue)
            {
                client = await _context.Clients.FindAsync(dto.ClientId.Value);
            }
            else if (!string.IsNullOrEmpty(dto.CallerNumber))
            {
                // Suche Client anhand der Geräte-Telefonnummer
                var device = await _context.EmergencyDevices
                    .Include(d => d.Client)
                    .FirstOrDefaultAsync(d => d.PhoneNumber == dto.CallerNumber);
                client = device?.Client;
            }

            var alert = new EmergencyAlert
            {
                AlertType = dto.AlertType,
                Priority = dto.Priority,
                Status = "New",
                EmergencyDeviceId = dto.EmergencyDeviceId,
                ClientId = client?.Id ?? dto.ClientId,
                CallerNumber = dto.CallerNumber,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                HeartRate = dto.HeartRate,
                Notes = dto.Notes,
                AlertTime = DateTime.UtcNow
            };

            _context.EmergencyAlerts.Add(alert);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAlertDetail), new { id = alert.Id }, new AlertSummaryDto
            {
                Id = alert.Id,
                AlertType = alert.AlertType,
                Priority = alert.Priority,
                Status = alert.Status,
                ClientName = client != null ? $"{client.FirstName} {client.LastName}" : "Unbekannt",
                AlertTime = alert.AlertTime,
                TimeAgo = "Gerade eben"
            });
        }

        /// <summary>
        /// Alarm aktualisieren (Status ändern, Notizen hinzufügen, etc.)
        /// </summary>
        [HttpPut("alerts/{id}")]
        public async Task<IActionResult> UpdateAlert(int id, [FromBody] UpdateAlertDto dto, [FromQuery] int? dispatcherId = null)
        {
            var alert = await _context.EmergencyAlerts.FindAsync(id);
            if (alert == null)
                return NotFound();

            if (!string.IsNullOrEmpty(dto.Status))
            {
                var oldStatus = alert.Status;
                alert.Status = dto.Status;

                if (dto.Status == "Acknowledged" && oldStatus == "New")
                {
                    alert.AcknowledgedTime = DateTime.UtcNow;
                    alert.AcknowledgedByDispatcherId = dispatcherId;
                }
                else if (dto.Status == "Resolved")
                {
                    alert.ResolvedTime = DateTime.UtcNow;
                    alert.ResolvedByDispatcherId = dispatcherId;
                }
            }

            if (!string.IsNullOrEmpty(dto.Resolution))
                alert.Resolution = dto.Resolution;

            if (!string.IsNullOrEmpty(dto.Notes))
                alert.Notes = string.IsNullOrEmpty(alert.Notes) 
                    ? dto.Notes 
                    : $"{alert.Notes}\n[{DateTime.UtcNow:yyyy-MM-dd HH:mm}] {dto.Notes}";

            if (dto.AmbulanceCalled.HasValue)
                alert.AmbulanceCalled = dto.AmbulanceCalled.Value;

            if (dto.ContactsNotified.HasValue)
                alert.ContactsNotified = dto.ContactsNotified.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ==================== CALLS ====================

        /// <summary>
        /// Anruf initiieren (Click-to-Call)
        /// </summary>
        [HttpPost("calls/initiate")]
        public async Task<ActionResult<CallSummaryDto>> InitiateCall([FromBody] InitiateCallDto dto, [FromQuery] int dispatcherId)
        {
            var dispatcher = await _context.Dispatchers.FindAsync(dispatcherId);
            if (dispatcher == null)
                return BadRequest("Dispatcher nicht gefunden");

            // Anruf über sipgate initiieren
            var result = await _sipgateService.InitiateCallAsync(dispatcher.SipExtension, dto.TargetNumber);

            if (!result.Success)
                return BadRequest($"Anruf konnte nicht initiiert werden: {result.Error}");

            // Call Log erstellen
            var callLog = new CallLog
            {
                SipgateCallId = result.SessionId ?? "",
                Direction = "Outbound",
                CallerNumber = dispatcher.Extension,
                CalleeNumber = dto.TargetNumber,
                DispatcherId = dispatcherId,
                ClientId = dto.ClientId,
                EmergencyContactId = dto.EmergencyContactId,
                EmergencyAlertId = dto.EmergencyAlertId,
                Status = "Ringing",
                StartTime = DateTime.UtcNow,
                CallType = dto.CallType,
                Notes = dto.Notes
            };

            _context.CallLogs.Add(callLog);
            
            // Dispatcher Status aktualisieren
            dispatcher.Status = "OnCall";
            dispatcher.CurrentCallCount++;
            
            await _context.SaveChangesAsync();

            return Ok(new CallSummaryDto
            {
                Id = callLog.Id,
                SipgateCallId = callLog.SipgateCallId,
                Direction = callLog.Direction,
                CallerNumber = callLog.CallerNumber,
                CalleeNumber = callLog.CalleeNumber,
                DispatcherName = $"{dispatcher.FirstName} {dispatcher.LastName}",
                Status = callLog.Status,
                StartTime = callLog.StartTime,
                CallType = callLog.CallType
            });
        }

        /// <summary>
        /// Anruf-Aktion ausführen (Halten, Stummschalten, Weiterleiten, Auflegen)
        /// </summary>
        [HttpPost("calls/{callId}/action")]
        public async Task<IActionResult> CallAction(string callId, [FromBody] CallActionDto dto)
        {
            bool success = false;

            switch (dto.Action.ToLower())
            {
                case "hold":
                    success = await _sipgateService.HoldCallAsync(callId, true);
                    break;
                case "resume":
                    success = await _sipgateService.HoldCallAsync(callId, false);
                    break;
                case "mute":
                    success = await _sipgateService.MuteCallAsync(callId, true);
                    break;
                case "unmute":
                    success = await _sipgateService.MuteCallAsync(callId, false);
                    break;
                case "transfer":
                    success = await _sipgateService.TransferCallAsync(callId, dto.TransferTarget);
                    break;
                case "hangup":
                    success = await _sipgateService.HangupCallAsync(callId);
                    break;
                case "record":
                    success = await _sipgateService.StartRecordingAsync(callId);
                    break;
                case "stoprecord":
                    success = await _sipgateService.StopRecordingAsync(callId);
                    break;
                default:
                    return BadRequest($"Unbekannte Aktion: {dto.Action}");
            }

            if (!success)
                return BadRequest("Aktion konnte nicht ausgeführt werden");

            // Call Log aktualisieren
            var callLog = await _context.CallLogs.FirstOrDefaultAsync(c => c.SipgateCallId == callId);
            if (callLog != null)
            {
                if (dto.Action.ToLower() == "hangup")
                {
                    callLog.Status = "Ended";
                    callLog.EndTime = DateTime.UtcNow;
                    callLog.DurationSeconds = (int)(DateTime.UtcNow - callLog.StartTime).TotalSeconds;
                    callLog.EndReason = "Completed";

                    // Dispatcher Status zurücksetzen
                    if (callLog.DispatcherId.HasValue)
                    {
                        var dispatcher = await _context.Dispatchers.FindAsync(callLog.DispatcherId.Value);
                        if (dispatcher != null)
                        {
                            dispatcher.CurrentCallCount = Math.Max(0, dispatcher.CurrentCallCount - 1);
                            dispatcher.TotalCallsHandled++;
                            if (dispatcher.CurrentCallCount == 0)
                                dispatcher.Status = "Online";
                        }
                    }
                }
                else if (dto.Action.ToLower() == "hold")
                {
                    callLog.Status = "OnHold";
                }
                else if (dto.Action.ToLower() == "resume")
                {
                    callLog.Status = "Connected";
                }
                else if (dto.Action.ToLower() == "record")
                {
                    callLog.IsRecorded = true;
                }

                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true, action = dto.Action });
        }

        /// <summary>
        /// Anrufprotokoll abrufen
        /// </summary>
        [HttpGet("calls/history")]
        public async Task<ActionResult<List<CallLogDto>>> GetCallHistory(
            [FromQuery] int? dispatcherId = null,
            [FromQuery] int? clientId = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] int limit = 50)
        {
            var query = _context.CallLogs
                .Include(c => c.Dispatcher)
                .Include(c => c.Client)
                .AsQueryable();

            if (dispatcherId.HasValue)
                query = query.Where(c => c.DispatcherId == dispatcherId);

            if (clientId.HasValue)
                query = query.Where(c => c.ClientId == clientId);

            if (from.HasValue)
                query = query.Where(c => c.StartTime >= from.Value);

            if (to.HasValue)
                query = query.Where(c => c.StartTime <= to.Value);

            var calls = await query
                .OrderByDescending(c => c.StartTime)
                .Take(limit)
                .Select(c => new CallLogDto
                {
                    Id = c.Id,
                    SipgateCallId = c.SipgateCallId,
                    Direction = c.Direction,
                    CallerNumber = c.CallerNumber,
                    CalleeNumber = c.CalleeNumber,
                    DispatcherName = c.Dispatcher != null ? $"{c.Dispatcher.FirstName} {c.Dispatcher.LastName}" : "",
                    ClientName = c.Client != null ? $"{c.Client.FirstName} {c.Client.LastName}" : "",
                    Status = c.Status,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    DurationSeconds = c.DurationSeconds,
                    CallType = c.CallType,
                    Notes = c.Notes,
                    IsRecorded = c.IsRecorded
                })
                .ToListAsync();

            return Ok(calls);
        }

        // ==================== WEBHOOKS ====================

        /// <summary>
        /// sipgate Webhook für eingehende Anrufe
        /// </summary>
        [HttpPost("webhook/incoming")]
        public async Task<IActionResult> IncomingCallWebhook([FromForm] SipgateWebhookDto webhook)
        {
            if (webhook.Event == "newCall" && webhook.Direction == "in")
            {
                // Prüfe ob es ein Notrufgerät ist
                var device = await _context.EmergencyDevices
                    .Include(d => d.Client)
                    .FirstOrDefaultAsync(d => d.PhoneNumber == webhook.From || d.SipIdentifier == webhook.From);

                if (device != null)
                {
                    // Automatisch Alarm erstellen
                    var alert = new EmergencyAlert
                    {
                        AlertType = "IncomingCall",
                        Priority = "High",
                        Status = "New",
                        EmergencyDeviceId = device.Id,
                        ClientId = device.ClientId,
                        CallerNumber = webhook.From,
                        AlertTime = DateTime.UtcNow
                    };

                    _context.EmergencyAlerts.Add(alert);
                }

                // Call Log erstellen
                var callLog = new CallLog
                {
                    SipgateCallId = webhook.CallId,
                    Direction = "Inbound",
                    CallerNumber = webhook.From,
                    CalleeNumber = webhook.To,
                    ClientId = device?.ClientId,
                    Status = "Ringing",
                    StartTime = DateTime.UtcNow,
                    CallType = device != null ? "Emergency" : "Routine"
                };

                _context.CallLogs.Add(callLog);
                await _context.SaveChangesAsync();
            }
            else if (webhook.Event == "answer")
            {
                var callLog = await _context.CallLogs.FirstOrDefaultAsync(c => c.SipgateCallId == webhook.CallId);
                if (callLog != null)
                {
                    callLog.Status = "Connected";
                    callLog.ConnectTime = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
            else if (webhook.Event == "hangup")
            {
                var callLog = await _context.CallLogs.FirstOrDefaultAsync(c => c.SipgateCallId == webhook.CallId);
                if (callLog != null)
                {
                    callLog.Status = "Ended";
                    callLog.EndTime = DateTime.UtcNow;
                    callLog.EndReason = webhook.Cause ?? "Completed";
                    callLog.DurationSeconds = callLog.ConnectTime.HasValue 
                        ? (int)(DateTime.UtcNow - callLog.ConnectTime.Value).TotalSeconds 
                        : 0;
                    await _context.SaveChangesAsync();
                }
            }

            return Ok();
        }

        // ==================== DEVICES ====================

        /// <summary>
        /// Liste aller Notrufgeräte
        /// </summary>
        [HttpGet("devices")]
        public async Task<ActionResult<List<EmergencyDeviceDto>>> GetDevices([FromQuery] string? status = null)
        {
            var query = _context.EmergencyDevices.Include(d => d.Client).AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(d => d.Status == status);

            var devices = await query
                .OrderBy(d => d.DeviceName)
                .Select(d => new EmergencyDeviceDto
                {
                    Id = d.Id,
                    DeviceName = d.DeviceName,
                    DeviceType = d.DeviceType,
                    Manufacturer = d.Manufacturer,
                    Model = d.Model,
                    SerialNumber = d.SerialNumber,
                    PhoneNumber = d.PhoneNumber,
                    Status = d.Status,
                    IsOnline = d.IsOnline,
                    BatteryLevel = d.BatteryLevel,
                    LastHeartbeat = d.LastHeartbeat,
                    LastAlertTime = d.LastAlertTime,
                    ClientId = d.ClientId,
                    ClientName = d.Client != null ? $"{d.Client.FirstName} {d.Client.LastName}" : ""
                })
                .ToListAsync();

            return Ok(devices);
        }

        /// <summary>
        /// Neues Notrufgerät registrieren
        /// </summary>
        [HttpPost("devices")]
        public async Task<ActionResult<EmergencyDeviceDto>> CreateDevice([FromBody] CreateEmergencyDeviceDto dto)
        {
            var device = new EmergencyDevice
            {
                DeviceName = dto.DeviceName,
                DeviceType = dto.DeviceType,
                Manufacturer = dto.Manufacturer,
                Model = dto.Model,
                SerialNumber = dto.SerialNumber,
                PhoneNumber = dto.PhoneNumber,
                SipIdentifier = dto.SipIdentifier,
                ClientId = dto.ClientId,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            _context.EmergencyDevices.Add(device);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDevices), new { id = device.Id }, new EmergencyDeviceDto
            {
                Id = device.Id,
                DeviceName = device.DeviceName,
                DeviceType = device.DeviceType,
                Status = device.Status
            });
        }

        // ==================== DISPATCHERS ====================

        /// <summary>
        /// Liste aller Disponenten
        /// </summary>
        [HttpGet("dispatchers")]
        public async Task<ActionResult<List<DispatcherDto>>> GetDispatchers()
        {
            var dispatchers = await _context.Dispatchers
                .OrderBy(d => d.LastName)
                .Select(d => new DispatcherDto
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Username = d.Username,
                    Email = d.Email,
                    Extension = d.Extension,
                    Status = d.Status,
                    IsAvailable = d.IsAvailable,
                    CurrentCallCount = d.CurrentCallCount,
                    TotalCallsHandled = d.TotalCallsHandled,
                    Role = d.Role,
                    LastLogin = d.LastLogin
                })
                .ToListAsync();

            return Ok(dispatchers);
        }

        /// <summary>
        /// Dispatcher-Status aktualisieren
        /// </summary>
        [HttpPut("dispatchers/{id}/status")]
        public async Task<IActionResult> UpdateDispatcherStatus(int id, [FromBody] UpdateDispatcherStatusDto dto)
        {
            var dispatcher = await _context.Dispatchers.FindAsync(id);
            if (dispatcher == null)
                return NotFound();

            if (!string.IsNullOrEmpty(dto.Status))
                dispatcher.Status = dto.Status;

            if (dto.IsAvailable.HasValue)
                dispatcher.IsAvailable = dto.IsAvailable.Value;

            dispatcher.LastActivity = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ==================== CONTACTS ====================

        /// <summary>
        /// Notfallkontakte eines Klienten abrufen
        /// </summary>
        [HttpGet("clients/{clientId}/contacts")]
        public async Task<ActionResult<List<EmergencyContactDto>>> GetClientContacts(int clientId)
        {
            var contacts = await _context.EmergencyContacts
                .Where(c => c.ClientId == clientId)
                .OrderBy(c => c.Priority)
                .Select(c => new EmergencyContactDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Relationship = c.Relationship,
                    PhoneNumber = c.PhoneNumber,
                    MobileNumber = c.MobileNumber,
                    Email = c.Email,
                    Priority = c.Priority,
                    IsAvailable24h = c.IsAvailable24h,
                    HasKey = c.HasKey,
                    Notes = c.Notes
                })
                .ToListAsync();

            return Ok(contacts);
        }

        /// <summary>
        /// Neuen Notfallkontakt hinzufügen
        /// </summary>
        [HttpPost("contacts")]
        public async Task<ActionResult<EmergencyContactDto>> CreateContact([FromBody] CreateEmergencyContactDto dto)
        {
            var contact = new EmergencyContact
            {
                ClientId = dto.ClientId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Relationship = dto.Relationship,
                PhoneNumber = dto.PhoneNumber,
                MobileNumber = dto.MobileNumber,
                Email = dto.Email,
                Priority = dto.Priority,
                IsAvailable24h = dto.IsAvailable24h,
                HasKey = dto.HasKey,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.EmergencyContacts.Add(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClientContacts), new { clientId = contact.ClientId }, new EmergencyContactDto
            {
                Id = contact.Id,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Relationship = contact.Relationship,
                PhoneNumber = contact.PhoneNumber
            });
        }

        // ==================== STATISTICS ====================

        /// <summary>
        /// Statistiken für die Leitstelle
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<ServiceHubStatisticsDto>> GetStatistics()
        {
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var stats = new ServiceHubStatisticsDto
            {
                TotalAlertsToday = await _context.EmergencyAlerts.CountAsync(a => a.AlertTime.Date == today),
                TotalAlertsThisWeek = await _context.EmergencyAlerts.CountAsync(a => a.AlertTime >= weekStart),
                TotalAlertsThisMonth = await _context.EmergencyAlerts.CountAsync(a => a.AlertTime >= monthStart),
                TotalCallsToday = await _context.CallLogs.CountAsync(c => c.StartTime.Date == today),
                TotalCallsThisWeek = await _context.CallLogs.CountAsync(c => c.StartTime >= weekStart),
                TotalCallsThisMonth = await _context.CallLogs.CountAsync(c => c.StartTime >= monthStart),
                AverageResponseTimeSeconds = await CalculateAverageResponseTimeAsync(),
                AverageCallDurationSeconds = await _context.CallLogs
                    .Where(c => c.DurationSeconds > 0)
                    .AverageAsync(c => (double?)c.DurationSeconds) ?? 0,
                FalseAlarmCount = await _context.EmergencyAlerts.CountAsync(a => a.Resolution == "FalseAlarm" && a.AlertTime >= monthStart),
                AmbulanceDispatchCount = await _context.EmergencyAlerts.CountAsync(a => a.AmbulanceCalled && a.AlertTime >= monthStart),
                AlertsByType = await _context.EmergencyAlerts
                    .Where(a => a.AlertTime >= monthStart)
                    .GroupBy(a => a.AlertType)
                    .Select(g => new ChartDataPoint { Label = g.Key, Value = g.Count() })
                    .ToListAsync(),
                CallsByDay = await _context.CallLogs
                    .Where(c => c.StartTime >= weekStart)
                    .GroupBy(c => c.StartTime.Date)
                    .Select(g => new ChartDataPoint { Label = g.Key.ToString("ddd"), Value = g.Count() })
                    .ToListAsync()
            };

            return Ok(stats);
        }

        // ==================== WEBRTC CREDENTIALS ====================

        /// <summary>
        /// WebRTC/SIP Credentials für Browser-Telefonie
        /// </summary>
        [HttpGet("webrtc/credentials")]
        public ActionResult<WebRtcCredentialsDto> GetWebRtcCredentials([FromQuery] int dispatcherId)
        {
            // In Produktion würden diese aus der Datenbank/Konfiguration kommen
            return Ok(new WebRtcCredentialsDto
            {
                SipServer = "sipconnect.sipgate.de",
                WebSocketUrl = "wss://sipconnect.sipgate.de",
                Username = "3938564t0",
                Password = "VEWqXdhf9wty",
                Domain = "sipgate.de",
                DisplayName = "Leitstelle"
            });
        }

        // ==================== HELPER METHODS ====================

        private async Task<double> CalculateAverageResponseTimeAsync()
        {
            var acknowledgedAlerts = await _context.EmergencyAlerts
                .Where(a => a.AcknowledgedTime.HasValue)
                .Select(a => new { a.AlertTime, a.AcknowledgedTime })
                .ToListAsync();

            if (!acknowledgedAlerts.Any())
                return 0;

            return acknowledgedAlerts
                .Average(a => (a.AcknowledgedTime!.Value - a.AlertTime).TotalSeconds);
        }

        private static string GetTimeAgo(DateTime dateTime)
        {
            var span = DateTime.UtcNow - dateTime;

            if (span.TotalMinutes < 1)
                return "Gerade eben";
            if (span.TotalMinutes < 60)
                return $"vor {(int)span.TotalMinutes} Min.";
            if (span.TotalHours < 24)
                return $"vor {(int)span.TotalHours} Std.";
            if (span.TotalDays < 7)
                return $"vor {(int)span.TotalDays} Tagen";

            return dateTime.ToString("dd.MM.yyyy HH:mm");
        }
    }
}
