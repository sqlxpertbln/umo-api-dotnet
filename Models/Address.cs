// =================================================================================================
// APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Domain Layer)
// This class is part of the Domain Layer in Clean Architecture. It represents the Address entity.
//
// META-DATA:
//   - Layer: Domain (Entity)
//   - Responsibility: Define the structure and properties of an Address.
// =================================================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMOApi.Models;

/// <summary>
/// Represents an address in the system.
/// </summary>
public class Address
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(200)]
    public string? Street { get; set; }
    
    [MaxLength(20)]
    public string? HouseNumber { get; set; }
    
    [MaxLength(20)]
    public string? ZipCode { get; set; }
    
    public int? CityId { get; set; }
    [ForeignKey("CityId")]
    public virtual City? City { get; set; }
    
    public int? DistrictId { get; set; }
    [ForeignKey("DistrictId")]
    public virtual District? District { get; set; }
    
    public int? CountryId { get; set; }
    [ForeignKey("CountryId")]
    public virtual Country? Country { get; set; }
    
    [MaxLength(100)]
    public string? AdditionalInfo { get; set; }
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? UpdateId { get; set; }
    
    public DateTime? UpdateDate { get; set; }
}

/// <summary>
/// Represents a city.
/// </summary>
public class City
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(100)]
    public string? Name { get; set; }
    
    [MaxLength(20)]
    public string? ZipCode { get; set; }
    
    public int? DistrictId { get; set; }
    [ForeignKey("DistrictId")]
    public virtual District? District { get; set; }
    
    public int? CountryId { get; set; }
    [ForeignKey("CountryId")]
    public virtual Country? Country { get; set; }
}

/// <summary>
/// Represents a district.
/// </summary>
public class District
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(100)]
    public string? Name { get; set; }
    
    [MaxLength(10)]
    public string? Code { get; set; }
    
    public int? CountryId { get; set; }
    [ForeignKey("CountryId")]
    public virtual Country? Country { get; set; }
}

/// <summary>
/// Represents a country.
/// </summary>
public class Country
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(100)]
    public string? Name { get; set; }
    
    [MaxLength(5)]
    public string? IsoCode { get; set; }
    
    [MaxLength(10)]
    public string? PhoneCode { get; set; }
}
