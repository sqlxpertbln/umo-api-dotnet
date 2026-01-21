using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMOApi.Models;

/// <summary>
/// Represents a client in the system with basic information.
/// </summary>
public class Client
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    public int? Nummer { get; set; }
    
    [MaxLength(100)]
    public string? FirstName { get; set; }
    
    [MaxLength(100)]
    public string? LastName { get; set; }
    
    [MaxLength(10)]
    public string? Sex { get; set; }
    
    public DateTime? BirthDay { get; set; }
    
    public byte[]? Memo { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; }
    
    public DateTime? StartContractDate { get; set; }
    
    [MaxLength(50)]
    public string? PolicyNumber { get; set; }
    
    [MaxLength(500)]
    public string? Note { get; set; }
    
    [MaxLength(200)]
    public string? FreeFeld { get; set; }
    
    [MaxLength(100)]
    public string? BooleanOptions { get; set; }
    
    // === Hausnotruf-Einstellungen ===
    
    /// <summary>
    /// Erlaubt die Aufzeichnung von Notruf-Gesprächen für Dokumentationszwecke
    /// </summary>
    public bool AllowCallRecording { get; set; } = false;
    
    /// <summary>
    /// Einverständniserklärung für Aufzeichnung wurde unterschrieben
    /// </summary>
    public bool RecordingConsentSigned { get; set; } = false;
    
    /// <summary>
    /// Datum der Einverständniserklärung
    /// </summary>
    public DateTime? RecordingConsentDate { get; set; }
    
    /// <summary>
    /// Aufbewahrungsdauer für Aufzeichnungen in Tagen (Standard: 90 Tage)
    /// </summary>
    public int RecordingRetentionDays { get; set; } = 90;
}

/// <summary>
/// Represents detailed information about a client including all related entities.
/// </summary>
public class ClientDetails
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    public int? Nummer { get; set; }
    
    public int? TitelId { get; set; }
    [ForeignKey("TitelId")]
    public virtual SystemEntry? Titel { get; set; }
    
    public int? PrefixId { get; set; }
    [ForeignKey("PrefixId")]
    public virtual SystemEntry? Prefix { get; set; }
    
    [MaxLength(50)]
    public string? SocialSecurityNumber { get; set; }
    
    [MaxLength(100)]
    public string? FirstName { get; set; }
    
    [MaxLength(100)]
    public string? LastName { get; set; }
    
    [MaxLength(10)]
    public string? Sex { get; set; }
    
    public DateTime? BirthDay { get; set; }
    
    public byte[]? Memo { get; set; }
    
    public int? StatusId { get; set; }
    [ForeignKey("StatusId")]
    public virtual SystemEntry? Status { get; set; }
    
    public DateTime? StartContractDate { get; set; }
    
    public DateTime? EndContractDate { get; set; }
    
    [MaxLength(50)]
    public string? PolicyNumber { get; set; }
    
    [MaxLength(500)]
    public string? Note { get; set; }
    
    [MaxLength(200)]
    public string? FreeFeld { get; set; }
    
    [MaxLength(100)]
    public string? BooleanOptions { get; set; }
    
    [MaxLength(200)]
    public string? MedecinPlace { get; set; }
    
    public int? Bill { get; set; }
    
    [MaxLength(50)]
    public string? BankNumber { get; set; }
    
    [MaxLength(100)]
    public string? BankName { get; set; }
    
    [MaxLength(50)]
    public string? PostBankNumber { get; set; }
    
    [MaxLength(100)]
    public string? InsuresCare { get; set; }
    
    public int? ClassificationId { get; set; }
    [ForeignKey("ClassificationId")]
    public virtual SystemEntry? Classification { get; set; }
    
    public int? InvoiceMethodId { get; set; }
    [ForeignKey("InvoiceMethodId")]
    public virtual SystemEntry? InvoiceMethod { get; set; }
    
    public int? ReasonId { get; set; }
    [ForeignKey("ReasonId")]
    public virtual SystemEntry? Reason { get; set; }
    
    public int? PriorityId { get; set; }
    [ForeignKey("PriorityId")]
    public virtual SystemEntry? Priority { get; set; }
    
    public int? MaritalStatusId { get; set; }
    [ForeignKey("MaritalStatusId")]
    public virtual SystemEntry? MaritalStatus { get; set; }
    
    public int? LanguageId { get; set; }
    [ForeignKey("LanguageId")]
    public virtual SystemEntry? Language { get; set; }
    
    public int? FinancialGroupId { get; set; }
    [ForeignKey("FinancialGroupId")]
    public virtual SystemEntry? FinancialGroup { get; set; }
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? UpdateId { get; set; }
    
    public DateTime? UpdateDate { get; set; }
    
    // Navigation properties
    public int? AddressId { get; set; }
    [ForeignKey("AddressId")]
    public virtual Address? Address { get; set; }
    
    public int? TarifId { get; set; }
    [ForeignKey("TarifId")]
    public virtual Tarif? Tarif { get; set; }
    
    public int? DirectProviderId { get; set; }
    [ForeignKey("DirectProviderId")]
    public virtual DirectProviderDetails? DirectProvider { get; set; }
    
    public virtual ICollection<ClientMedecin>? Medicin { get; set; }
    public virtual ICollection<ClientDisease>? Diseases { get; set; }
    public virtual ICollection<ClientCost>? Costs { get; set; }
    public virtual ICollection<ClientStatusHistory>? StatusHistory { get; set; }
    public virtual ICollection<ClientPhone>? Phones { get; set; }
    public virtual ICollection<ClientNotation>? Notations { get; set; }
    public virtual ICollection<ClientFeature>? Features { get; set; }
}

/// <summary>
/// Represents the status of a client.
/// </summary>
public class ClientStatus
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(100)]
    public string? Name { get; set; }
    
    [MaxLength(50)]
    public string? Code { get; set; }
    
    [MaxLength(100)]
    public string? Description { get; set; }
    
    public int? BooleanOptions { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? UpdateId { get; set; }
    
    public DateTime? UpdateDate { get; set; }
}

/// <summary>
/// Represents the status history of a client.
/// </summary>
public class ClientStatusHistory
{
    [Key]
    public int Id { get; set; }
    
    public int ClientId { get; set; }
    [ForeignKey("ClientId")]
    public virtual ClientDetails? Client { get; set; }
    
    public int MandantId { get; set; }
    
    public int? StatusId { get; set; }
    
    [MaxLength(100)]
    public string? Description { get; set; }
    
    public DateTime? ChangeDate { get; set; }
    
    [MaxLength(50)]
    public string? ChangedBy { get; set; }
}

/// <summary>
/// Represents a phone number associated with a client.
/// </summary>
public class ClientPhone
{
    [Key]
    public int Id { get; set; }
    
    public int ClientId { get; set; }
    [ForeignKey("ClientId")]
    public virtual ClientDetails? Client { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(50)]
    public string? PhoneType { get; set; }
    
    [MaxLength(50)]
    public string? PhoneNumber { get; set; }
    
    public bool? IsPrimary { get; set; }
}

/// <summary>
/// Represents a notation/note associated with a client.
/// </summary>
public class ClientNotation
{
    [Key]
    public int Id { get; set; }
    
    public int ClientId { get; set; }
    [ForeignKey("ClientId")]
    public virtual ClientDetails? Client { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(500)]
    public string? Text { get; set; }
    
    public DateTime? CreatedDate { get; set; }
    
    [MaxLength(50)]
    public string? CreatedBy { get; set; }
}

/// <summary>
/// Represents a feature associated with a client.
/// </summary>
public class ClientFeature
{
    [Key]
    public int Id { get; set; }
    
    public int ClientId { get; set; }
    [ForeignKey("ClientId")]
    public virtual ClientDetails? Client { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(100)]
    public string? FeatureName { get; set; }
    
    [MaxLength(200)]
    public string? FeatureValue { get; set; }
}

/// <summary>
/// Represents a cost associated with a client.
/// </summary>
public class ClientCost
{
    [Key]
    public int Id { get; set; }
    
    public int ClientId { get; set; }
    [ForeignKey("ClientId")]
    public virtual ClientDetails? Client { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(50)]
    public string? CostType { get; set; } // OneTime or Regular
    
    [MaxLength(100)]
    public string? Description { get; set; }
    
    public decimal? Amount { get; set; }
    
    public DateTime? Date { get; set; }
}

/// <summary>
/// Represents a disease associated with a client.
/// </summary>
public class ClientDisease
{
    [Key]
    public int Id { get; set; }
    
    public int ClientId { get; set; }
    [ForeignKey("ClientId")]
    public virtual ClientDetails? Client { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(100)]
    public string? DiseaseName { get; set; }
    
    [MaxLength(50)]
    public string? IcdCode { get; set; }
    
    public DateTime? DiagnosisDate { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
}

/// <summary>
/// Represents a medication for a client (Medikamentenliste für Hausnotruf)
/// </summary>
public class ClientMedication
{
    [Key]
    public int Id { get; set; }
    
    public int ClientId { get; set; }
    [ForeignKey("ClientId")]
    public virtual ClientDetails? Client { get; set; }
    
    public int MandantId { get; set; }
    
    /// <summary>
    /// Name des Medikaments
    /// </summary>
    [MaxLength(200)]
    public string MedicationName { get; set; } = string.Empty;
    
    /// <summary>
    /// Wirkstoff
    /// </summary>
    [MaxLength(200)]
    public string? ActiveIngredient { get; set; }
    
    /// <summary>
    /// Dosierung (z.B. "100mg")
    /// </summary>
    [MaxLength(100)]
    public string? Dosage { get; set; }
    
    /// <summary>
    /// Einnahmefrequenz (z.B. "2x täglich", "morgens")
    /// </summary>
    [MaxLength(100)]
    public string? Frequency { get; set; }
    
    /// <summary>
    /// Einnahmezeit (z.B. "08:00, 20:00")
    /// </summary>
    [MaxLength(100)]
    public string? TimeOfDay { get; set; }
    
    /// <summary>
    /// Verschreibender Arzt
    /// </summary>
    [MaxLength(200)]
    public string? PrescribedBy { get; set; }
    
    /// <summary>
    /// Verschreibungsdatum
    /// </summary>
    public DateTime? PrescribedDate { get; set; }
    
    /// <summary>
    /// Wichtige Hinweise für Rettungsdienst (z.B. "Blutverduenner", "Wechselwirkungen")
    /// </summary>
    [MaxLength(500)]
    public string? EmergencyNotes { get; set; }
    
    /// <summary>
    /// Ist das Medikament aktuell aktiv?
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Priorität für Notfall-Übergabe (1 = höchste)
    /// </summary>
    public int Priority { get; set; } = 5;
    
    /// <summary>
    /// Kategorie (z.B. "Herzmedikament", "Blutverduenner", "Schmerzmittel")
    /// </summary>
    [MaxLength(100)]
    public string? Category { get; set; }
}

/// <summary>
/// Represents a medical professional associated with a client.
/// </summary>
public class ClientMedecin
{
    [Key]
    public int Id { get; set; }
    
    public int ClientId { get; set; }
    [ForeignKey("ClientId")]
    public virtual ClientDetails? Client { get; set; }
    
    public int? ProfessionalProviderId { get; set; }
    [ForeignKey("ProfessionalProviderId")]
    public virtual ProfessionalProvider? ProfessionalProvider { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(50)]
    public string? Role { get; set; }
    
    public DateTime? AssignedDate { get; set; }
}
