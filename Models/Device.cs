using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMOApi.Models;

/// <summary>
/// Represents a device in the system.
/// </summary>
public class Device
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(100)]
    public string? DeviceType { get; set; }
    
    [MaxLength(100)]
    public string? SerialNumber { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; }
    
    [MaxLength(100)]
    public string? Manufacturer { get; set; }
    
    [MaxLength(100)]
    public string? Model { get; set; }
    
    public DateTime? PurchaseDate { get; set; }
    
    public DateTime? WarrantyEndDate { get; set; }
    
    public int? AssignedClientId { get; set; }
    [ForeignKey("AssignedClientId")]
    public virtual ClientDetails? AssignedClient { get; set; }
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? UpdateId { get; set; }
    
    public DateTime? UpdateDate { get; set; }
}

/// <summary>
/// Represents detailed device information.
/// </summary>
public class DeviceDetails
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(100)]
    public string? DeviceType { get; set; }
    
    [MaxLength(100)]
    public string? SerialNumber { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; }
    
    [MaxLength(100)]
    public string? Manufacturer { get; set; }
    
    [MaxLength(100)]
    public string? Model { get; set; }
    
    public DateTime? PurchaseDate { get; set; }
    
    public DateTime? WarrantyEndDate { get; set; }
    
    public decimal? PurchasePrice { get; set; }
    
    [MaxLength(200)]
    public string? Location { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public int? AssignedClientId { get; set; }
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? UpdateId { get; set; }
    
    public DateTime? UpdateDate { get; set; }
}
