using System;
using System.Collections.Generic;

namespace UMOApi.Models
{
    // ==================== HELPER CLASSES ====================
    
    public class ChartDataPoint
    {
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
        public string? Type { get; set; }
        public string? Date { get; set; }
        public int? Count { get; set; }
    }
    
    // ==================== DASHBOARD DTOs ====================
    
    public class ServiceHubDashboardDto
    {
        public int ActiveAlerts { get; set; }
        public int CriticalAlerts { get; set; }
        public int OnlineDispatchers { get; set; }
        public int TotalDispatchers { get; set; }
        public int ActiveCalls { get; set; }
        public int OnlineDevices { get; set; }
        public int TotalDevices { get; set; }
        public int CallsToday { get; set; }
        public int AlertsToday { get; set; }
        public double AverageResponseTime { get; set; } // in Sekunden
        public List<AlertSummaryDto> RecentAlerts { get; set; } = new();
        public List<CallSummaryDto> ActiveCallsList { get; set; } = new();
        public List<DispatcherStatusDto> DispatcherStatuses { get; set; } = new();
    }
    
    public class AlertSummaryDto
    {
        public int Id { get; set; }
        public string AlertType { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public DateTime AlertTime { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
        public int? HeartRate { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
    
    public class CallSummaryDto
    {
        public int Id { get; set; }
        public string SipgateCallId { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
        public string CallerNumber { get; set; } = string.Empty;
        public string CalleeNumber { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string DispatcherName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public int DurationSeconds { get; set; }
        public string CallType { get; set; } = string.Empty;
    }
    
    public class DispatcherStatusDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public int CurrentCallCount { get; set; }
        public string Extension { get; set; } = string.Empty;
    }
    
    // ==================== ALERT DTOs ====================
    
    public class CreateAlertDto
    {
        public string AlertType { get; set; } = string.Empty;
        public string Priority { get; set; } = "High";
        public int? EmergencyDeviceId { get; set; }
        public int? ClientId { get; set; }
        public string CallerNumber { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? HeartRate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
    
    public class UpdateAlertDto
    {
        public string Status { get; set; } = string.Empty;
        public string Resolution { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool? AmbulanceCalled { get; set; }
        public bool? ContactsNotified { get; set; }
    }
    
    public class AlertDetailDto
    {
        public int Id { get; set; }
        public string AlertType { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime AlertTime { get; set; }
        public DateTime? AcknowledgedTime { get; set; }
        public DateTime? ResolvedTime { get; set; }
        public string Resolution { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool AmbulanceCalled { get; set; }
        public bool ContactsNotified { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? HeartRate { get; set; }
        
        // Client Info
        public int? ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientAddress { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public DateTime? ClientBirthDate { get; set; }
        public string ClientMedicalNotes { get; set; } = string.Empty;
        
        // Device Info
        public int? DeviceId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        
        // Dispatcher Info
        public string AcknowledgedBy { get; set; } = string.Empty;
        public string ResolvedBy { get; set; } = string.Empty;
        
        // Related Data
        public List<EmergencyContactDto> EmergencyContacts { get; set; } = new();
        public List<CallSummaryDto> RelatedCalls { get; set; } = new();
    }
    
    // ==================== DEVICE DTOs ====================
    
    public class EmergencyDeviceDto
    {
        public int Id { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public int BatteryLevel { get; set; }
        public DateTime? LastHeartbeat { get; set; }
        public DateTime? LastAlertTime { get; set; }
        public int? ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
    }
    
    public class CreateEmergencyDeviceDto
    {
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string SipIdentifier { get; set; } = string.Empty;
        public int? ClientId { get; set; }
    }
    
    // ==================== CONTACT DTOs ====================
    
    public class EmergencyContactDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Relationship { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Priority { get; set; }
        public bool IsAvailable24h { get; set; }
        public bool HasKey { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
    
    public class CreateEmergencyContactDto
    {
        public int ClientId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Priority { get; set; } = 1;
        public bool IsAvailable24h { get; set; }
        public bool HasKey { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
    
    // ==================== DISPATCHER DTOs ====================
    
    public class DispatcherDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public int CurrentCallCount { get; set; }
        public int TotalCallsHandled { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime? LastLogin { get; set; }
    }
    
    public class UpdateDispatcherStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public bool? IsAvailable { get; set; }
    }
    
    // ==================== CALL DTOs ====================
    
    public class InitiateCallDto
    {
        public string TargetNumber { get; set; } = string.Empty;
        public int? ClientId { get; set; }
        public int? EmergencyContactId { get; set; }
        public int? EmergencyAlertId { get; set; }
        public string CallType { get; set; } = "Routine";
        public string Notes { get; set; } = string.Empty;
    }
    
    public class CallActionDto
    {
        public string Action { get; set; } = string.Empty; // Hold, Resume, Mute, Unmute, Transfer, Hangup
        public string TransferTarget { get; set; } = string.Empty;
    }
    
    public class CallLogDto
    {
        public int Id { get; set; }
        public string SipgateCallId { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
        public string CallerNumber { get; set; } = string.Empty;
        public string CalleeNumber { get; set; } = string.Empty;
        public string DispatcherName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int DurationSeconds { get; set; }
        public string CallType { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool IsRecorded { get; set; }
    }
    
    // ==================== WEBHOOK DTOs ====================
    
    public class SipgateWebhookDto
    {
        public string Event { get; set; } = string.Empty; // newCall, answer, hangup
        public string CallId { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Cause { get; set; } = string.Empty;
        public string AnsweringNumber { get; set; } = string.Empty;
    }
    
    // ==================== STATISTICS DTOs ====================
    
    public class ServiceHubStatisticsDto
    {
        public int TotalAlertsToday { get; set; }
        public int TotalAlertsThisWeek { get; set; }
        public int TotalAlertsThisMonth { get; set; }
        public int TotalCallsToday { get; set; }
        public int TotalCallsThisWeek { get; set; }
        public int TotalCallsThisMonth { get; set; }
        public double AverageResponseTimeSeconds { get; set; }
        public double AverageCallDurationSeconds { get; set; }
        public int FalseAlarmCount { get; set; }
        public int AmbulanceDispatchCount { get; set; }
        public List<ChartDataPoint> AlertsByType { get; set; } = new();
        public List<ChartDataPoint> AlertsByHour { get; set; } = new();
        public List<ChartDataPoint> CallsByDay { get; set; } = new();
        public List<DispatcherPerformanceDto> DispatcherPerformance { get; set; } = new();
    }
    
    public class DispatcherPerformanceDto
    {
        public int DispatcherId { get; set; }
        public string DispatcherName { get; set; } = string.Empty;
        public int CallsHandled { get; set; }
        public int AlertsResolved { get; set; }
        public double AverageResponseTime { get; set; }
        public double AverageCallDuration { get; set; }
    }
    
    // ==================== SIP CONFIGURATION DTOs ====================
    
    public class SipConfigurationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SipServer { get; set; } = string.Empty;
        public string WebSocketUrl { get; set; } = string.Empty;
        public string SipUsername { get; set; } = string.Empty;
        public string SipDomain { get; set; } = string.Empty;
        public int SipPort { get; set; }
        public bool UseTls { get; set; }
        public bool IsActive { get; set; }
    }
    
    public class WebRtcCredentialsDto
    {
        public string SipServer { get; set; } = string.Empty;
        public string WebSocketUrl { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }

    // ==================== SMS NOTIFICATION DTOs ====================
    
    public class SmsNotificationResultDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int AlertId { get; set; }
        public int ClientId { get; set; }
        public string? AlertType { get; set; }
        public DateTime Timestamp { get; set; }
        public int TotalContacts { get; set; }
        public int SuccessfulNotifications { get; set; }
        public List<NotifiedContactDto> NotifiedContacts { get; set; } = new();
    }
    
    public class NotifiedContactDto
    {
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public bool Success { get; set; }
        public DateTime SentAt { get; set; }
        public string? Error { get; set; }
    }
    
    public class SendSmsDto
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
    
    public class SmsSendResultDto
    {
        public bool Success { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
}
