// =================================================================================================
// APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Presentation Layer)
// This controller is part of the Presentation Layer in Clean Architecture. It handles HTTP requests
// related to ServiceHub entities and translates them into application-level commands or queries.
//
// META-DATA:
//   - Layer: Presentation (API Controller)
//   - Responsibility: Expose ServiceHub-related functionality via a RESTful API.
// =================================================================================================

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
            try
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
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Dashboard Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                // Return empty dashboard on error instead of 500
                return Ok(new ServiceHubDashboardDto
                {
                    ActiveAlerts = 0,
                    CriticalAlerts = 0,
                    OnlineDispatchers = 0,
                    TotalDispatchers = 0,
                    ActiveCalls = 0,
                    OnlineDevices = 0,
                    TotalDevices = 0,
                    CallsToday = 0,
                    AlertsToday = 0,
                    AverageResponseTime = 0,
                    RecentAlerts = new List<AlertSummaryDto>(),
                    ActiveCallsList = new List<CallSummaryDto>(),
                    DispatcherStatuses = new List<DispatcherStatusDto>()
                });
            }
        }

        private async Task<double> CalculateAverageResponseTimeAsync()
        {
            var alerts = await _context.EmergencyAlerts
                .Where(a => a.AcknowledgedTime.HasValue)
                .ToListAsync();

            if (!alerts.Any())
            {
                return 0;
            }

            return alerts.Average(a => (a.AcknowledgedTime.Value - a.AlertTime).TotalSeconds);
        }

        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalSeconds < 60)
            {
                return $"{(int)timeSpan.TotalSeconds}s ago";
            }
            if (timeSpan.TotalMinutes < 60)
            {
                return $"{(int)timeSpan.TotalMinutes}m ago";
            }
            if (timeSpan.TotalHours < 24)
            {
                return $"{(int)timeSpan.TotalHours}h ago";
            }
            return $"{(int)timeSpan.TotalDays}d ago";
        }
    }
}
