// =================================================================================================
// APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Domain Layer)
// This class is part of the Domain Layer in Clean Architecture. It represents the Alert entity.
//
// META-DATA:
//   - Layer: Domain (Entity)
//   - Responsibility: Define the structure and properties of an Alert.
// =================================================================================================

namespace UMOApi.Models;

public class EmergencyAlert
{
    public int Id { get; set; }
    public string AlertType { get; set; }
    public string Status { get; set; }
    public DateTime AlertTime { get; set; }
    public int? ClientId { get; set; }
    public Client? Client { get; set; }
    public int? EmergencyDeviceId { get; set; }
    public EmergencyDevice? EmergencyDevice { get; set; }
    public string? EmergencyChainStep { get; set; }
}
