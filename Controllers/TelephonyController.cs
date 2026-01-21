using Microsoft.AspNetCore.Mvc;
using UMOApi.Services;

namespace UMOApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TelephonyController : ControllerBase
    {
        private readonly SipSorceryTelephonyService _telephonyService;
        private readonly ILogger<TelephonyController> _logger;

        public TelephonyController(SipSorceryTelephonyService telephonyService, ILogger<TelephonyController> logger)
        {
            _telephonyService = telephonyService;
            _logger = logger;
        }

        /// <summary>
        /// Gibt den aktuellen Telefonie-Status zurück
        /// </summary>
        [HttpGet("status")]
        public ActionResult<TelephonyStatus> GetStatus()
        {
            return Ok(_telephonyService.GetStatus());
        }

        /// <summary>
        /// Initiiert einen Click-to-Call Anruf über die Sipgate API
        /// Der Anruf wird von der angegebenen Extension (z.B. e0 für VoIP-Telefon) zur Zielnummer hergestellt
        /// </summary>
        [HttpPost("call")]
        public async Task<ActionResult<CallResult>> InitiateCall([FromBody] CallRequest request)
        {
            if (string.IsNullOrEmpty(request.CalleeNumber))
            {
                return BadRequest(new CallResult 
                { 
                    Success = false, 
                    Message = "Zielnummer ist erforderlich" 
                });
            }

            var callerExtension = request.CallerExtension ?? "e0"; // Standard: VoIP-Telefon
            var result = await _telephonyService.InitiateClickToCallAsync(callerExtension, request.CalleeNumber);
            
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Initiiert einen direkten SIP-Anruf (für Server-zu-Server Kommunikation)
        /// </summary>
        [HttpPost("sip-call")]
        public async Task<ActionResult<CallResult>> InitiateSipCall([FromBody] SipCallRequest request)
        {
            if (string.IsNullOrEmpty(request.CalleeNumber))
            {
                return BadRequest(new CallResult 
                { 
                    Success = false, 
                    Message = "Zielnummer ist erforderlich" 
                });
            }

            var result = await _telephonyService.InitiateCallAsync(
                request.CallerNumber ?? "", 
                request.CalleeNumber);
            
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Sendet eine SMS über die Sipgate API
        /// </summary>
        [HttpPost("sms")]
        public async Task<ActionResult> SendSms([FromBody] SmsRequest request)
        {
            if (string.IsNullOrEmpty(request.RecipientNumber) || string.IsNullOrEmpty(request.Message))
            {
                return BadRequest(new { success = false, message = "Empfängernummer und Nachricht sind erforderlich" });
            }

            var success = await _telephonyService.SendSmsAsync(request.RecipientNumber, request.Message);
            
            if (success)
            {
                return Ok(new { success = true, message = "SMS erfolgreich gesendet" });
            }
            else
            {
                return StatusCode(500, new { success = false, message = "SMS konnte nicht gesendet werden" });
            }
        }

        /// <summary>
        /// Ruft einen Notfallkontakt an (für Alarm-Benachrichtigung)
        /// </summary>
        [HttpPost("emergency-call")]
        public async Task<ActionResult<CallResult>> EmergencyCall([FromBody] EmergencyCallRequest request)
        {
            if (string.IsNullOrEmpty(request.ContactNumber))
            {
                return BadRequest(new CallResult 
                { 
                    Success = false, 
                    Message = "Kontaktnummer ist erforderlich" 
                });
            }

            _logger.LogWarning("Notfall-Anruf initiiert zu {Number} für Alarm {AlarmId}", 
                request.ContactNumber, request.AlarmId);

            // Click-to-Call verwenden
            var result = await _telephonyService.InitiateClickToCallAsync("e0", request.ContactNumber);
            
            return result.Success ? Ok(result) : StatusCode(500, result);
        }
    }

    public class CallRequest
    {
        public string? CallerExtension { get; set; }
        public string CalleeNumber { get; set; } = "";
    }

    public class SipCallRequest
    {
        public string? CallerNumber { get; set; }
        public string CalleeNumber { get; set; } = "";
    }

    public class SmsRequest
    {
        public string RecipientNumber { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public class EmergencyCallRequest
    {
        public int? AlarmId { get; set; }
        public string ContactNumber { get; set; } = "";
        public string? ContactName { get; set; }
    }
}
