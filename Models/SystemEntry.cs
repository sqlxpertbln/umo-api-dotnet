using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMOApi.Models;

/// <summary>
/// Represents a system entry for reference data (titles, prefixes, statuses, etc.).
/// </summary>
public class SystemEntry
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    /// <summary>
    /// Type of the system entry:
    /// T = Title, P = Prefix, S = Status, M = MaritalStatus, 
    /// I = InvoiceMethod, R = Reason, PR = Priority, L = Language,
    /// IC = InsuranceClass, ICR = InsuranceCare, FG = FinancialGroup
    /// </summary>
    [MaxLength(10)]
    public string? Type { get; set; }
    
    [MaxLength(100)]
    public string? Description { get; set; }
    
    public int? BooleanOptions { get; set; }
    
    [MaxLength(50)]
    public string? Code { get; set; }
    
    public int? SortOrder { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? UpdateId { get; set; }
    
    public DateTime? UpdateDate { get; set; }
}

/// <summary>
/// Represents a tariff with VAT tax information.
/// </summary>
public class Tarif
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(100)]
    public string? Name { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public decimal? BasePrice { get; set; }
    
    public int? VatTaxId { get; set; }
    [ForeignKey("VatTaxId")]
    public virtual VatTax? VatTax { get; set; }
    
    public decimal? TotalPrice { get; set; }
    
    public DateTime? ValidFrom { get; set; }
    
    public DateTime? ValidTo { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? UpdateId { get; set; }
    
    public DateTime? UpdateDate { get; set; }
}

/// <summary>
/// Represents VAT tax information.
/// </summary>
public class VatTax
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(100)]
    public string? Name { get; set; }
    
    public decimal? Percentage { get; set; }
    
    [MaxLength(20)]
    public string? Code { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public DateTime? CreateDate { get; set; }
}

/// <summary>
/// Represents a user for authentication.
/// </summary>
public class User
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(100)]
    public string? Username { get; set; }
    
    [MaxLength(256)]
    public string? PasswordHash { get; set; }
    
    [MaxLength(100)]
    public string? Email { get; set; }
    
    [MaxLength(100)]
    public string? FirstName { get; set; }
    
    [MaxLength(100)]
    public string? LastName { get; set; }
    
    [MaxLength(50)]
    public string? Role { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime? LastLogin { get; set; }
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? UpdateId { get; set; }
    
    public DateTime? UpdateDate { get; set; }
}

/// <summary>
/// Represents a login request.
/// </summary>
public class LoginRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

/// <summary>
/// Represents a login response.
/// </summary>
public class LoginResponse
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
    public int? UserId { get; set; }
    public int? MandantId { get; set; }
    public string? Username { get; set; }
    public string? Role { get; set; }
}
