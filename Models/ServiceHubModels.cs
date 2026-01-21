using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMOApi.Models
{
    // ==================== EMERGENCY DEVICES ====================
    
    /// <summary>
    /// Notrufgerät (Apple Watch, Hausnotruf-Gerät, etc.)
    /// </summary>
    public class EmergencyDevice
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string DeviceName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string DeviceType { get; set; } = string.Empty; // AppleWatch, Hausnotruf, Smartphone, etc.
        
        [StringLength(50)]
        public string Manufacturer { get; set; } = string.Empty; // Apple, Tunstall, Bosch, etc.
        
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string SerialNumber { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty; // Rufnummer des Geräts
        
        [StringLength(50)]
        public string SipIdentifier { get; set; } = string.Empty; // SIP-ID für Identifikation
        
        public int? ClientId { get; set; } // Zugeordneter Klient
        public Client? Client { get; set; }
        
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Inactive, Maintenance
        
        public DateTime? LastHeartbeat { get; set; } // Letztes Lebenszeichen
        
        public DateTime? LastAlertTime { get; set; } // Letzter Alarm
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public bool IsOnline { get; set; } = false;
        
        public int BatteryLevel { get; set; } = 100; // Akkustand in %
        
        // Navigation
        public ICollection<EmergencyAlert> Alerts { get; set; } = new List<EmergencyAlert>();
    }
    
    // ==================== EMERGENCY CONTACTS ====================
    
    /// <summary>
    /// Notfallkontakt (Angehörige, Ärzte, etc.)
    /// </summary>
    public class EmergencyContact
    {
        [Key]
        public int Id { get; set; }
        
        public int? ClientId { get; set; } // Nullable für Seed-Daten
        public Client? Client { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Relationship { get; set; } = string.Empty; // Sohn, Tochter, Nachbar, Arzt, etc.
        
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string MobileNumber { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        public int Priority { get; set; } = 1; // Reihenfolge der Kontaktierung
        
        public bool IsAvailable24h { get; set; } = false;
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        public bool HasKey { get; set; } = false; // Hat Schlüssel zur Wohnung
        
        public bool NotifyBySms { get; set; } = true; // Bei Notruf per SMS benachrichtigen
        
        public bool NotifyByCall { get; set; } = false; // Bei Notruf anrufen
        
        public bool NotifyByEmail { get; set; } = false; // Bei Notruf per E-Mail benachrichtigen
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    
    // ==================== EMERGENCY ALERTS ====================
    
    /// <summary>
    /// Notruf/Alarm
    /// </summary>
    public class EmergencyAlert
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string AlertType { get; set; } = string.Empty; // FallDetection, ManualAlert, Inactivity, Medical, Fire
        
        [Required]
        [StringLength(20)]
        public string Priority { get; set; } = "High"; // Critical, High, Medium, Low
        
        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "New"; // New, Acknowledged, InProgress, Resolved, Escalated, Cancelled
        
        public int? EmergencyDeviceId { get; set; }
        public EmergencyDevice? EmergencyDevice { get; set; }
        
        public int? ClientId { get; set; }
        public Client? Client { get; set; }
        
        [StringLength(20)]
        public string CallerNumber { get; set; } = string.Empty;
        
        public DateTime AlertTime { get; set; } = DateTime.UtcNow;
        
        public DateTime? AcknowledgedTime { get; set; }
        
        public DateTime? ResolvedTime { get; set; }
        
        public int? AcknowledgedByDispatcherId { get; set; }
        public Dispatcher? AcknowledgedByDispatcher { get; set; }
        
        public int? ResolvedByDispatcherId { get; set; }
        public Dispatcher? ResolvedByDispatcher { get; set; }
        
        [StringLength(50)]
        public string Resolution { get; set; } = string.Empty; // FalseAlarm, MedicalAssistance, Ambulance, Police, Fire, ContactNotified
        
        [StringLength(2000)]
        public string Notes { get; set; } = string.Empty;
        
        // GPS-Koordinaten (falls verfügbar, z.B. von Apple Watch)
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        
        // Vitalzeichen (falls verfügbar)
        public int? HeartRate { get; set; }
        
        public bool AmbulanceCalled { get; set; } = false;
        
        public bool ContactsNotified { get; set; } = false;
        
        // === NOTFALLKETTE STATUS ===
        
        /// <summary>
        /// Aktueller Schritt in der Notfallkette
        /// </summary>
        [StringLength(50)]
        public string EmergencyChainStep { get; set; } = "Initial"; // Initial, ContactingFamily, ContactingDoctor, ContactingAmbulance, InConference, Resolved
        
        /// <summary>
        /// Zeitpunkt wann Angehörige informiert wurden
        /// </summary>
        public DateTime? FamilyNotifiedTime { get; set; }
        
        /// <summary>
        /// Anzahl der benachrichtigten Angehörigen
        /// </summary>
        public int FamilyContactsNotified { get; set; } = 0;
        
        /// <summary>
        /// Zeitpunkt wann Arzt informiert wurde
        /// </summary>
        public DateTime? DoctorNotifiedTime { get; set; }
        
        /// <summary>
        /// Name des informierten Arztes
        /// </summary>
        [StringLength(200)]
        public string DoctorNotified { get; set; } = string.Empty;
        
        /// <summary>
        /// Zeitpunkt wann Rettungsdienst alarmiert wurde
        /// </summary>
        public DateTime? AmbulanceCalledTime { get; set; }
        
        /// <summary>
        /// Rettungsdienst-Einsatznummer
        /// </summary>
        [StringLength(50)]
        public string AmbulanceIncidentNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Medikamentenliste wurde an Rettungsdienst übergeben
        /// </summary>
        public bool MedicationListProvided { get; set; } = false;
        
        /// <summary>
        /// Zeitpunkt der Medikamentenlisten-Übergabe
        /// </summary>
        public DateTime? MedicationListProvidedTime { get; set; }
        
        /// <summary>
        /// Konferenzschaltung aktiv (Makeln)
        /// </summary>
        public bool ConferenceActive { get; set; } = false;
        
        /// <summary>
        /// Teilnehmer der Konferenzschaltung
        /// </summary>
        [StringLength(500)]
        public string ConferenceParticipants { get; set; } = string.Empty;
        
        /// <summary>
        /// Geschätzte Ankunftszeit des Rettungsdienstes
        /// </summary>
        public DateTime? AmbulanceEta { get; set; }
        
        // Navigation
        public ICollection<CallLog> CallLogs { get; set; } = new List<CallLog>();
        public ICollection<EmergencyChainAction> ChainActions { get; set; } = new List<EmergencyChainAction>();
    }
    
    // ==================== DISPATCHERS ====================
    
    /// <summary>
    /// Disponent/Agent in der Leitstelle
    /// </summary>
    public class Dispatcher
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Extension { get; set; } = string.Empty; // Nebenstelle
        
        [StringLength(20)]
        public string SipExtension { get; set; } = string.Empty; // SIP-Nebenstelle
        
        [StringLength(20)]
        public string Status { get; set; } = "Offline"; // Online, Offline, OnCall, Break, Away
        
        public bool IsAvailable { get; set; } = false;
        
        public DateTime? LastLogin { get; set; }
        
        public DateTime? LastActivity { get; set; }
        
        public int CurrentCallCount { get; set; } = 0;
        
        public int TotalCallsHandled { get; set; } = 0;
        
        [StringLength(50)]
        public string Role { get; set; } = "Agent"; // Agent, Supervisor, Admin
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation
        public ICollection<CallLog> CallLogs { get; set; } = new List<CallLog>();
    }
    
    // ==================== CALL LOG ====================
    
    /// <summary>
    /// Anrufprotokoll
    /// </summary>
    public class CallLog
    {
        [Key]
        public int Id { get; set; }
        
        [StringLength(100)]
        public string SipgateCallId { get; set; } = string.Empty; // sipgate Call-ID
        
        [Required]
        [StringLength(20)]
        public string Direction { get; set; } = string.Empty; // Inbound, Outbound
        
        [StringLength(20)]
        public string CallerNumber { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string CalleeNumber { get; set; } = string.Empty;
        
        public int? DispatcherId { get; set; }
        public Dispatcher? Dispatcher { get; set; }
        
        public int? ClientId { get; set; }
        public Client? Client { get; set; }
        
        public int? EmergencyAlertId { get; set; }
        public EmergencyAlert? EmergencyAlert { get; set; }
        
        public int? EmergencyContactId { get; set; }
        public EmergencyContact? EmergencyContact { get; set; }
        
        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Ringing"; // Ringing, Connected, OnHold, Transferred, Ended, Missed, Voicemail
        
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        
        public DateTime? ConnectTime { get; set; }
        
        public DateTime? EndTime { get; set; }
        
        public int DurationSeconds { get; set; } = 0;
        
        [StringLength(50)]
        public string EndReason { get; set; } = string.Empty; // Completed, NoAnswer, Busy, Failed, Cancelled
        
        [StringLength(2000)]
        public string Notes { get; set; } = string.Empty;
        
        public bool IsRecorded { get; set; } = false;
        
        [StringLength(500)]
        public string RecordingUrl { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string CallType { get; set; } = string.Empty; // Emergency, Callback, Routine, Test
        
        // === Erweiterte Aufzeichnungsfelder ===
        
        /// <summary>
        /// Aufzeichnung war erlaubt (basierend auf Klient-Einstellung)
        /// </summary>
        public bool RecordingAllowed { get; set; } = false;
        
        /// <summary>
        /// Aufzeichnung wurde vom Disponenten manuell gestartet
        /// </summary>
        public bool RecordingStartedManually { get; set; } = false;
        
        /// <summary>
        /// Dateigröße der Aufzeichnung in Bytes
        /// </summary>
        public long RecordingFileSize { get; set; } = 0;
        
        /// <summary>
        /// Dauer der Aufzeichnung in Sekunden
        /// </summary>
        public int RecordingDurationSeconds { get; set; } = 0;
        
        /// <summary>
        /// Transkription des Gesprächs (optional, für Dokumentation)
        /// </summary>
        [StringLength(10000)]
        public string Transcription { get; set; } = string.Empty;
        
        /// <summary>
        /// Zusammenfassung des Gesprächs
        /// </summary>
        [StringLength(2000)]
        public string CallSummary { get; set; } = string.Empty;
        
        /// <summary>
        /// Kategorisierung des Anrufs nach Abschluss
        /// </summary>
        [StringLength(50)]
        public string CallCategory { get; set; } = string.Empty; // Emergency, Medical, Technical, Social, Test
        
        /// <summary>
        /// Priorität des Anrufs
        /// </summary>
        [StringLength(20)]
        public string Priority { get; set; } = "Normal"; // Critical, High, Normal, Low
        
        /// <summary>
        /// Wurde der Anruf eskaliert?
        /// </summary>
        public bool WasEscalated { get; set; } = false;
        
        /// <summary>
        /// An wen wurde eskaliert?
        /// </summary>
        [StringLength(100)]
        public string EscalatedTo { get; set; } = string.Empty;
        
        /// <summary>
        /// Follow-up erforderlich?
        /// </summary>
        public bool RequiresFollowUp { get; set; } = false;
        
        /// <summary>
        /// Follow-up Datum
        /// </summary>
        public DateTime? FollowUpDate { get; set; }
        
        /// <summary>
        /// Follow-up erledigt?
        /// </summary>
        public bool FollowUpCompleted { get; set; } = false;
    }
    
    // ==================== SIP CONFIGURATION ====================
    
    /// <summary>
    /// SIP/VoIP Konfiguration
    /// </summary>
    public class SipConfiguration
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string SipServer { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string WebSocketUrl { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string SipUsername { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string SipPassword { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string SipDomain { get; set; } = string.Empty;
        
        public int SipPort { get; set; } = 5060;
        
        public bool UseTls { get; set; } = true;
        
        public bool IsActive { get; set; } = true;
        
        [StringLength(500)]
        public string WebhookUrl { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    
    // ==================== SHIFT/SCHICHT ====================
    
    /// <summary>
    /// Schichtplan für Disponenten
    /// </summary>
    public class DispatcherShift
    {
        [Key]
        public int Id { get; set; }
        
        public int DispatcherId { get; set; }
        public Dispatcher? Dispatcher { get; set; }
        
        public DateTime ShiftStart { get; set; }
        
        public DateTime ShiftEnd { get; set; }
        
        [StringLength(50)]
        public string ShiftType { get; set; } = string.Empty; // Morning, Afternoon, Night, OnCall
        
        [StringLength(200)]
        public string Notes { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
    }
    
    // ==================== EMERGENCY CHAIN ACTION ====================
    
    /// <summary>
    /// Protokolliert jeden Schritt in der Notfallkette
    /// </summary>
    public class EmergencyChainAction
    {
        [Key]
        public int Id { get; set; }
        
        public int EmergencyAlertId { get; set; }
        public EmergencyAlert? EmergencyAlert { get; set; }
        
        /// <summary>
        /// Art der Aktion
        /// </summary>
        [Required]
        [StringLength(50)]
        public string ActionType { get; set; } = string.Empty; // CallFamily, SmsFamily, CallDoctor, CallAmbulance, ProvideMedList, StartConference, AddToConference, EndConference
        
        /// <summary>
        /// Zeitpunkt der Aktion
        /// </summary>
        public DateTime ActionTime { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Durchgeführt von (Disponent)
        /// </summary>
        public int? DispatcherId { get; set; }
        public Dispatcher? Dispatcher { get; set; }
        
        /// <summary>
        /// Ziel der Aktion (Name/Telefonnummer)
        /// </summary>
        [StringLength(200)]
        public string Target { get; set; } = string.Empty;
        
        /// <summary>
        /// Telefonnummer des Ziels
        /// </summary>
        [StringLength(20)]
        public string TargetPhone { get; set; } = string.Empty;
        
        /// <summary>
        /// Ergebnis der Aktion
        /// </summary>
        [StringLength(50)]
        public string Result { get; set; } = string.Empty; // Success, NoAnswer, Busy, Failed, Voicemail
        
        /// <summary>
        /// Dauer des Anrufs in Sekunden (falls Anruf)
        /// </summary>
        public int DurationSeconds { get; set; } = 0;
        
        /// <summary>
        /// Notizen zur Aktion
        /// </summary>
        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;
        
        /// <summary>
        /// Wurde Medikamentenliste übergeben?
        /// </summary>
        public bool MedicationListProvided { get; set; } = false;
        
        /// <summary>
        /// Inhalt der übergebenen Medikamentenliste
        /// </summary>
        [StringLength(5000)]
        public string MedicationListContent { get; set; } = string.Empty;
        
        /// <summary>
        /// Sipgate Call-ID (falls Anruf)
        /// </summary>
        [StringLength(100)]
        public string SipgateCallId { get; set; } = string.Empty;
    }
}
