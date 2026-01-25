// =================================================================================================
// APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Domain Layer)
// This class is part of the Domain Layer in Clean Architecture. It represents the CallLog entity.
//
// META-DATA:
//   - Layer: Domain (Entity)
//   - Responsibility: Define the structure and properties of a CallLog.
// =================================================================================================

namespace UMOApi.Models;

public class CallLog
{
    public int Id { get; set; }
    public string SipgateCallId { get; set; }
    public string Direction { get; set; }
    public string CallerNumber { get; set; }
    public string CalleeNumber { get; set; }
    public string Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? ClientId { get; set; }
    public Client? Client { get; set; }
    public int? DispatcherId { get; set; }
    public Dispatcher? Dispatcher { get; set; }
}
