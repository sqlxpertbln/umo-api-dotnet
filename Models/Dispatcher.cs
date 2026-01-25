// =================================================================================================
// APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Domain Layer)
// This class is part of the Domain Layer in Clean Architecture. It represents the Dispatcher entity.
//
// META-DATA:
//   - Layer: Domain (Entity)
//   - Responsibility: Define the structure and properties of a Dispatcher.
// =================================================================================================

namespace UMOApi.Models;

public class Dispatcher
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public bool IsAvailable { get; set; }
    public int CurrentCallCount { get; set; }
    public string Extension { get; set; }
}
