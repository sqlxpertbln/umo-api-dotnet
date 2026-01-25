// =================================================================================================
// APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Domain Layer)
// This class is part of the Domain Layer in Clean Architecture. It represents the EmergencyContact entity.
//
// META-DATA:
//   - Layer: Domain (Entity)
//   - Responsibility: Define the structure and properties of an EmergencyContact.
// =================================================================================================

namespace UMOApi.Models;

public class EmergencyContact
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Relationship { get; set; }
    public string PhoneNumber { get; set; }
    public int Priority { get; set; }
    public int ClientId { get; set; }
    public Client Client { get; set; }
}
