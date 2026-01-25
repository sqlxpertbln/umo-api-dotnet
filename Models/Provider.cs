// =================================================================================================
// APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Domain Layer)
// This class is part of the Domain Layer in Clean Architecture. It represents the Provider entity.
//
// META-DATA:
//   - Layer: Domain (Entity)
//   - Responsibility: Define the structure and properties of a Provider.
// =================================================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMOApi.Models;

/// <summary>
/// Represents a direct provider in the system.
/// </summary>
public class DirectProvider
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(200)]
    public string? Name { get; set; }
    
    [MaxLength(100)]
    public string? FirstName { get; set; }
    
    [MaxLength(100)]
    public string? LastName { get; set; }
    
    public int? AddressId { get; set; }
    [ForeignKey("AddressId")]
    public virtual Address? Address { get; set; }
    
    [MaxLength(50)]
    public string? Phone { get; set; }
    
    [MaxLength(100)]
    public string? Email { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? UpdateId { get; set; }
    
    public DateTime? UpdateDate { get; set; }
}

/// <summary>
/// Represents detailed direct provider information.
/// </summary>
public class DirectProviderDetails
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(200)]
    public string? Name { get; set; }
    
    [MaxLength(100)]
    public string? FirstName { get; set; }
    
    [MaxLength(100)]
    public string? LastName { get; set; }
    
    [MaxLength(200)]
    public string? Street { get; set; }
    
    [MaxLength(20)]
    public string? HouseNumber { get; set; }
    
    [MaxLength(20)]
    public string? ZipCode { get; set; }
    
    [MaxLength(100)]
    public string? City { get; set; }
    
    [MaxLength(100)]
    public string? Country { get; set; }
    
    [MaxLength(50)]
    public string? Phone { get; set; }
    
    [MaxLength(50)]
    public string? Mobile { get; set; }
    
    [MaxLength(50)]
    public string? Fax { get; set; }
    
    [MaxLength(100)]
    public string? Email { get; set; }
    
    [MaxLength(200)]
    public string? Website { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? UpdateId { get; set; }
    
    public DateTime? UpdateDate { get; set; }
}

/// <summary>
/// Represents a professional provider (e.g., doctor, therapist).
/// </summary>
public class ProfessionalProvider
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(200)]
    public string? Name { get; set; }
    
    [MaxLength(100)]
    public string? FirstName { get; set; }
    
    [MaxLength(100)]
    public string? LastName { get; set; }
    
    [MaxLength(100)]
    public string? Specialty { get; set; }
    
    public int? AddressId { get; set; }
    [ForeignKey("AddressId")]
    public virtual Address? Address { get; set; }
    
    [MaxLength(50)]
    public string? Phone { get; set; }
    
    [MaxLength(100)]
    public string? Email { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; }
    
    [MaxLength(50)]
    public string? LicenseNumber { get; set; }
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? UpdateId { get; set; }
    
    public DateTime? UpdateDate { get; set; }
}

/// <summary>
/// Represents detailed professional provider information.
/// </summary>
public class ProfessionalProviderDetails
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(200)]
    public string? Name { get; set; }
    
    [MaxLength(100)]
    public string? FirstName { get; set; }
    
    [MaxLength(100)]
    public string? LastName { get; set; }
    
    [MaxLength(100)]
    public string? Specialty { get; set; }
    
    [MaxLength(200)]
    public string? Street { get; set; }
    
    [MaxLength(20)]
    public string? HouseNumber { get; set; }
    
    [MaxLength(20)]
    public string? ZipCode { get; set; }
    
    [MaxLength(100)]
    public string? City { get; set; }
    
    [MaxLength(100)]
    public string? Country { get; set; }
    
    [MaxLength(50)]
    public string? Phone { get; set; }
    
    [MaxLength(50)]
    public string? Mobile { get; set; }
    
    [MaxLength(50)]
    public string? Fax { get; set; }
    
    [MaxLength(100)]
    public string? Email { get; set; }
    
    [MaxLength(200)]
    public string? Website { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; }
    
    [MaxLength(50)]
    public string? LicenseNumber { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    [MaxLength(500)]
    public string? Qualifications { get; set; }
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? UpdateId { get; set; }
    
    public DateTime? UpdateDate { get; set; }
}

/// <summary>
/// Represents the link between a professional provider and a client.
/// </summary>
public class ProfessionalProviderClientLink
{
    [Key]
    public int Id { get; set; }
    
    public int ProfessionalProviderId { get; set; }
    [ForeignKey("ProfessionalProviderId")]
    public virtual ProfessionalProvider? ProfessionalProvider { get; set; }
    
    public int ClientId { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(50)]
    public string? Role { get; set; }
    
    public DateTime? LinkDate { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
}
