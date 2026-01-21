namespace UMOApi.Models;

/// <summary>
/// Generic paginated response wrapper.
/// </summary>
/// <typeparam name="T">Type of items in the result.</typeparam>
public class PaginatedResponse<T>
{
    public long Count { get; set; }
    public long Start { get; set; }
    public long PageCount { get; set; }
    public IEnumerable<T>? Results { get; set; }
    public string? Error { get; set; }
}

/// <summary>
/// DTO for client list items.
/// </summary>
public class ClientListDto
{
    public int Id { get; set; }
    public int MandantId { get; set; }
    public int? Nummer { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Sex { get; set; }
    public DateTime? BirthDay { get; set; }
    public string? Status { get; set; }
    public DateTime? StartContractDate { get; set; }
    public string? PolicyNumber { get; set; }
    public string? Note { get; set; }
    public string? FreeFeld { get; set; }
    public string? BooleanOptions { get; set; }
}

/// <summary>
/// DTO for client details.
/// </summary>
public class ClientDetailsDto
{
    public int Id { get; set; }
    public int MandantId { get; set; }
    public int? Nummer { get; set; }
    public string? Titel { get; set; }
    public string? Prefix { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Sex { get; set; }
    public DateTime? BirthDay { get; set; }
    public byte[]? Memo { get; set; }
    public string? Status { get; set; }
    public DateTime? StartContractDate { get; set; }
    public DateTime? EndContractDate { get; set; }
    public string? PolicyNumber { get; set; }
    public string? Note { get; set; }
    public string? FreeFeld { get; set; }
    public string? BooleanOptions { get; set; }
    public string? MedecinPlace { get; set; }
    public int? Bill { get; set; }
    public string? BankNumber { get; set; }
    public string? BankName { get; set; }
    public string? PostBankNumber { get; set; }
    public string? InsuresCare { get; set; }
    public string? Classification { get; set; }
    public string? InvoiceMethod { get; set; }
    public string? Reason { get; set; }
    public string? Priority { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Language { get; set; }
    public string? FinancialGroup { get; set; }
    public string? CreateId { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? UpdateId { get; set; }
    public DateTime? UpdateDate { get; set; }
    public AddressDto? Address { get; set; }
    public TarifDto? Tarif { get; set; }
    public List<MedecinDto>? Medicin { get; set; }
    public List<DiseaseDto>? Diseases { get; set; }
    public List<CostDto>? OneTimeCost { get; set; }
    public List<CostDto>? RegularCost { get; set; }
    public List<StatusHistoryDto>? StatusHistory { get; set; }
    public List<PhoneDto>? Phones { get; set; }
    public List<NotationDto>? Notations { get; set; }
    public List<FeatureDto>? Features { get; set; }
}

/// <summary>
/// DTO for creating/updating client details.
/// </summary>
public class ClientDetailsCreateDto
{
    public int MandantId { get; set; }
    public int? Nummer { get; set; }
    public int? TitelId { get; set; }
    public int? PrefixId { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Sex { get; set; }
    public DateTime? BirthDay { get; set; }
    public byte[]? Memo { get; set; }
    public int? StatusId { get; set; }
    public DateTime? StartContractDate { get; set; }
    public DateTime? EndContractDate { get; set; }
    public string? PolicyNumber { get; set; }
    public string? Note { get; set; }
    public string? FreeFeld { get; set; }
    public string? BooleanOptions { get; set; }
    public string? MedecinPlace { get; set; }
    public int? Bill { get; set; }
    public string? BankNumber { get; set; }
    public string? BankName { get; set; }
    public string? PostBankNumber { get; set; }
    public string? InsuresCare { get; set; }
    public int? ClassificationId { get; set; }
    public int? InvoiceMethodId { get; set; }
    public int? ReasonId { get; set; }
    public int? PriorityId { get; set; }
    public int? MaritalStatusId { get; set; }
    public int? LanguageId { get; set; }
    public int? FinancialGroupId { get; set; }
    public int? AddressId { get; set; }
    public int? TarifId { get; set; }
}

/// <summary>
/// DTO for address information.
/// </summary>
public class AddressDto
{
    public int Id { get; set; }
    public int MandantId { get; set; }
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Country { get; set; }
    public string? AdditionalInfo { get; set; }
}

/// <summary>
/// DTO for creating/updating address.
/// </summary>
public class AddressCreateDto
{
    public int MandantId { get; set; }
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public string? ZipCode { get; set; }
    public int? CityId { get; set; }
    public int? DistrictId { get; set; }
    public int? CountryId { get; set; }
    public string? AdditionalInfo { get; set; }
}

/// <summary>
/// DTO for tariff information.
/// </summary>
public class TarifDto
{
    public int Id { get; set; }
    public int MandantId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? BasePrice { get; set; }
    public decimal? VatPercentage { get; set; }
    public decimal? TotalPrice { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for creating/updating tariff.
/// </summary>
public class TarifCreateDto
{
    public int MandantId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? BasePrice { get; set; }
    public int? VatTaxId { get; set; }
    public decimal? VatPercentage { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for medical professional information.
/// </summary>
public class MedecinDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Specialty { get; set; }
    public string? Role { get; set; }
}

/// <summary>
/// DTO for disease information.
/// </summary>
public class DiseaseDto
{
    public int Id { get; set; }
    public string? DiseaseName { get; set; }
    public string? IcdCode { get; set; }
    public DateTime? DiagnosisDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for cost information.
/// </summary>
public class CostDto
{
    public int Id { get; set; }
    public string? CostType { get; set; }
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? Date { get; set; }
}

/// <summary>
/// DTO for status history information.
/// </summary>
public class StatusHistoryDto
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public DateTime? ChangeDate { get; set; }
    public string? ChangedBy { get; set; }
}

/// <summary>
/// DTO for phone information.
/// </summary>
public class PhoneDto
{
    public int Id { get; set; }
    public string? PhoneType { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsPrimary { get; set; }
}

/// <summary>
/// DTO for notation information.
/// </summary>
public class NotationDto
{
    public int Id { get; set; }
    public string? Text { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
}

/// <summary>
/// DTO for feature information.
/// </summary>
public class FeatureDto
{
    public int Id { get; set; }
    public string? FeatureName { get; set; }
    public string? FeatureValue { get; set; }
}

/// <summary>
/// DTO for device list items.
/// </summary>
public class DeviceListDto
{
    public int Id { get; set; }
    public int MandantId { get; set; }
    public string? DeviceType { get; set; }
    public string? SerialNumber { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
}

/// <summary>
/// DTO for device details.
/// </summary>
public class DeviceDetailsDto
{
    public int Id { get; set; }
    public int MandantId { get; set; }
    public string? DeviceType { get; set; }
    public string? SerialNumber { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public int? AssignedClientId { get; set; }
    public string? CreateId { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? UpdateId { get; set; }
    public DateTime? UpdateDate { get; set; }
}

/// <summary>
/// DTO for creating/updating device.
/// </summary>
public class DeviceCreateDto
{
    public int MandantId { get; set; }
    public string? DeviceType { get; set; }
    public string? SerialNumber { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public int? AssignedClientId { get; set; }
}

/// <summary>
/// DTO for direct provider list items.
/// </summary>
public class DirectProviderListDto
{
    public int Id { get; set; }
    public int MandantId { get; set; }
    public string? Name { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// DTO for direct provider details.
/// </summary>
public class DirectProviderDetailsDto
{
    public int Id { get; set; }
    public int MandantId { get; set; }
    public string? Name { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public string? CreateId { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? UpdateId { get; set; }
    public DateTime? UpdateDate { get; set; }
}

/// <summary>
/// DTO for creating/updating direct provider.
/// </summary>
public class DirectProviderCreateDto
{
    public int MandantId { get; set; }
    public string? Name { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for professional provider list items.
/// </summary>
public class ProfessionalProviderListDto
{
    public int Id { get; set; }
    public int MandantId { get; set; }
    public string? Name { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Specialty { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// DTO for professional provider details.
/// </summary>
public class ProfessionalProviderDetailsDto
{
    public int Id { get; set; }
    public int MandantId { get; set; }
    public string? Name { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Specialty { get; set; }
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Status { get; set; }
    public string? LicenseNumber { get; set; }
    public string? Notes { get; set; }
    public string? Qualifications { get; set; }
    public string? CreateId { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? UpdateId { get; set; }
    public DateTime? UpdateDate { get; set; }
}

/// <summary>
/// DTO for creating/updating professional provider.
/// </summary>
public class ProfessionalProviderCreateDto
{
    public int MandantId { get; set; }
    public string? Name { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Specialty { get; set; }
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Status { get; set; }
    public string? LicenseNumber { get; set; }
    public string? Notes { get; set; }
    public string? Qualifications { get; set; }
}

/// <summary>
/// DTO for linking professional provider to client.
/// </summary>
public class ProfessionalProviderLinkDto
{
    public int ProfessionalProviderId { get; set; }
    public int ClientId { get; set; }
    public int MandantId { get; set; }
    public string? Role { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for system entry (reference data).
/// </summary>
public class SystemEntryDto
{
    public int Id { get; set; }
    public int MandantId { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public int? BooleanOptions { get; set; }
    public string? Code { get; set; }
    public int? SortOrder { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for creating/updating system entry.
/// </summary>
public class SystemEntryCreateDto
{
    public int MandantId { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
    public int? BooleanOptions { get; set; }
    public string? Code { get; set; }
    public int? SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO for city information.
/// </summary>
public class CityDto
{
    public int Id { get; set; }
    public int MandantId { get; set; }
    public string? Name { get; set; }
    public string? ZipCode { get; set; }
    public string? District { get; set; }
    public string? Country { get; set; }
}

/// <summary>
/// DTO for district information.
/// </summary>
public class DistrictDto
{
    public int Id { get; set; }
    public int MandantId { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Country { get; set; }
}

/// <summary>
/// DTO for country information.
/// </summary>
public class CountryDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? IsoCode { get; set; }
    public string? PhoneCode { get; set; }
}
