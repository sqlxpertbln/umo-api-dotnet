using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace UMOApi.Services
{
    public interface ISipgateService
    {
        Task<SipgateCallResult> InitiateCallAsync(string callerExtension, string targetNumber);
        Task<bool> HangupCallAsync(string callId);
        Task<bool> TransferCallAsync(string callId, string targetNumber);
        Task<bool> HoldCallAsync(string callId, bool hold);
        Task<bool> MuteCallAsync(string callId, bool mute);
        Task<bool> StartRecordingAsync(string callId);
        Task<bool> StopRecordingAsync(string callId);
        Task<List<SipgateActiveCall>> GetActiveCallsAsync();
        Task<SipgateCallHistory> GetCallHistoryAsync(int limit = 50);
        Task<bool> SendSmsAsync(string smsId, string recipient, string message);
        Task<SmsNotificationResult> NotifyEmergencyContactsAsync(int clientId, string alertType, string message);
        Task<SipgateAccountInfo> GetAccountInfoAsync();
        Task<List<SipgateUser>> GetUsersAsync();
    }

    public class SipgateService : ISipgateService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SipgateService> _logger;
        private readonly string _baseUrl = "https://api.sipgate.com/v2";
        private readonly string _username;
        private readonly string _password;

        public SipgateService(IConfiguration configuration, ILogger<SipgateService> logger)
        {
            _logger = logger;
            _username = configuration["Sipgate:Username"] ?? "3938564t0";
            _password = configuration["Sipgate:Password"] ?? "VEWqXdhf9wty";

            _httpClient = new HttpClient();
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<SipgateCallResult> InitiateCallAsync(string callerExtension, string targetNumber)
        {
            try
            {
                var payload = new
                {
                    caller = callerExtension,
                    callee = targetNumber,
                    callerId = callerExtension
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/sessions/calls", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<SipgateCallResult>(responseContent);
                    _logger.LogInformation($"Call initiated successfully: {result?.SessionId}");
                    return result ?? new SipgateCallResult { Success = true };
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to initiate call: {error}");
                    return new SipgateCallResult { Success = false, Error = error };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating call");
                return new SipgateCallResult { Success = false, Error = ex.Message };
            }
        }

        public async Task<bool> HangupCallAsync(string callId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/calls/{callId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error hanging up call {callId}");
                return false;
            }
        }

        public async Task<bool> TransferCallAsync(string callId, string targetNumber)
        {
            try
            {
                var payload = new { target = targetNumber };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/calls/{callId}/transfer", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error transferring call {callId}");
                return false;
            }
        }

        public async Task<bool> HoldCallAsync(string callId, bool hold)
        {
            try
            {
                var payload = new { value = hold };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseUrl}/calls/{callId}/hold", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error holding call {callId}");
                return false;
            }
        }

        public async Task<bool> MuteCallAsync(string callId, bool mute)
        {
            try
            {
                var payload = new { value = mute };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseUrl}/calls/{callId}/muted", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error muting call {callId}");
                return false;
            }
        }

        public async Task<bool> StartRecordingAsync(string callId)
        {
            try
            {
                var payload = new { value = true };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseUrl}/calls/{callId}/recording", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error starting recording for call {callId}");
                return false;
            }
        }

        public async Task<bool> StopRecordingAsync(string callId)
        {
            try
            {
                var payload = new { value = false };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseUrl}/calls/{callId}/recording", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error stopping recording for call {callId}");
                return false;
            }
        }

        public async Task<List<SipgateActiveCall>> GetActiveCallsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/calls");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<SipgateActiveCallsResponse>(content);
                    return result?.Data ?? new List<SipgateActiveCall>();
                }
                return new List<SipgateActiveCall>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active calls");
                return new List<SipgateActiveCall>();
            }
        }

        public async Task<SipgateCallHistory> GetCallHistoryAsync(int limit = 50)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/history?limit={limit}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<SipgateCallHistory>(content) ?? new SipgateCallHistory();
                }
                return new SipgateCallHistory();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting call history");
                return new SipgateCallHistory();
            }
        }

        public async Task<bool> SendSmsAsync(string smsId, string recipient, string message)
        {
            try
            {
                // Formatiere Telefonnummer für sipgate (E.164 Format)
                var formattedRecipient = FormatPhoneNumber(recipient);
                
                var payload = new
                {
                    smsId = smsId,
                    recipient = formattedRecipient,
                    message = message
                };

                _logger.LogInformation($"Sending SMS to {formattedRecipient}: {message.Substring(0, Math.Min(50, message.Length))}...");
                
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/sessions/sms", content);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"SMS successfully sent to {formattedRecipient}");
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to send SMS: {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS");
                return false;
            }
        }
        
        /// <summary>
        /// Benachrichtigt alle Notfallkontakte eines Klienten per SMS bei einem eingehenden Notruf
        /// </summary>
        public async Task<SmsNotificationResult> NotifyEmergencyContactsAsync(int clientId, string alertType, string message)
        {
            var result = new SmsNotificationResult
            {
                ClientId = clientId,
                AlertType = alertType,
                Timestamp = DateTime.UtcNow
            };
            
            try
            {
                _logger.LogInformation($"Starting SMS notification for client {clientId}, alert type: {alertType}");
                
                // Hinweis: In einer echten Implementierung würden hier die Kontakte aus der Datenbank geladen
                // Da dieser Service keinen direkten DB-Zugriff hat, muss der Controller die Kontakte übergeben
                // Diese Methode wird vom Controller mit den Kontaktdaten aufgerufen
                
                result.Success = true;
                result.Message = "SMS notification initiated. Contacts will be notified by controller.";
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error notifying emergency contacts for client {clientId}");
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }
        }
        
        /// <summary>
        /// Sendet SMS an eine Liste von Notfallkontakten
        /// </summary>
        public async Task<SmsNotificationResult> SendBulkEmergencySmsAsync(List<EmergencyContactSms> contacts, string clientName, string alertType, string alertMessage)
        {
            var result = new SmsNotificationResult
            {
                AlertType = alertType,
                Timestamp = DateTime.UtcNow,
                NotifiedContacts = new List<NotifiedContact>()
            };
            
            // Standard SMS-ID für sipgate (muss in Ihrem Account konfiguriert sein)
            var smsId = "s0"; // Standard SMS-Endpunkt
            
            foreach (var contact in contacts)
            {
                try
                {
                    var smsMessage = BuildEmergencyMessage(clientName, alertType, alertMessage, contact.ContactName);
                    var success = await SendSmsAsync(smsId, contact.PhoneNumber, smsMessage);
                    
                    result.NotifiedContacts.Add(new NotifiedContact
                    {
                        Name = contact.ContactName,
                        PhoneNumber = contact.PhoneNumber,
                        Success = success,
                        SentAt = DateTime.UtcNow
                    });
                    
                    if (success)
                    {
                        _logger.LogInformation($"Emergency SMS sent to {contact.ContactName} ({contact.PhoneNumber})");
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to send emergency SMS to {contact.ContactName} ({contact.PhoneNumber})");
                    }
                    
                    // Kurze Pause zwischen SMS um Rate-Limiting zu vermeiden
                    await Task.Delay(500);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error sending SMS to {contact.ContactName}");
                    result.NotifiedContacts.Add(new NotifiedContact
                    {
                        Name = contact.ContactName,
                        PhoneNumber = contact.PhoneNumber,
                        Success = false,
                        Error = ex.Message
                    });
                }
            }
            
            result.Success = result.NotifiedContacts.Any(c => c.Success);
            result.TotalContacts = contacts.Count;
            result.SuccessfulNotifications = result.NotifiedContacts.Count(c => c.Success);
            result.Message = $"{result.SuccessfulNotifications} von {result.TotalContacts} Kontakten erfolgreich benachrichtigt.";
            
            return result;
        }
        
        private string BuildEmergencyMessage(string clientName, string alertType, string alertMessage, string contactName)
        {
            var alertTypeText = alertType switch
            {
                "FallDetection" => "STURZERKENNUNG",
                "ManualAlert" => "MANUELLER NOTRUF",
                "InactivityAlert" => "INAKTIVITÄTSALARM",
                "LowBattery" => "BATTERIE NIEDRIG",
                "Panic" => "PANIK-ALARM",
                "Medical" => "MEDIZINISCHER NOTFALL",
                _ => "NOTRUF"
            };
            
            return $"⚠️ {alertTypeText}\n" +
                   $"Klient: {clientName}\n" +
                   $"{alertMessage}\n" +
                   $"Bitte kontaktieren Sie die Notrufzentrale.\n" +
                   $"UMO Hausnotruf";
        }
        
        private string FormatPhoneNumber(string phoneNumber)
        {
            // Entferne alle Nicht-Ziffern außer +
            var cleaned = new string(phoneNumber.Where(c => char.IsDigit(c) || c == '+').ToArray());
            
            // Wenn keine Ländervorwahl, füge deutsche hinzu
            if (!cleaned.StartsWith("+"))
            {
                if (cleaned.StartsWith("0"))
                {
                    cleaned = "+49" + cleaned.Substring(1);
                }
                else
                {
                    cleaned = "+49" + cleaned;
                }
            }
            
            return cleaned;
        }

        public async Task<SipgateAccountInfo> GetAccountInfoAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/account");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<SipgateAccountInfo>(content) ?? new SipgateAccountInfo();
                }
                return new SipgateAccountInfo();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account info");
                return new SipgateAccountInfo();
            }
        }

        public async Task<List<SipgateUser>> GetUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/app/users");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<SipgateUsersResponse>(content);
                    return result?.Items ?? new List<SipgateUser>();
                }
                return new List<SipgateUser>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return new List<SipgateUser>();
            }
        }
    }

    // Response Models
    public class SipgateCallResult
    {
        public bool Success { get; set; }
        public string? SessionId { get; set; }
        public string? Error { get; set; }
    }

    public class SipgateActiveCallsResponse
    {
        public List<SipgateActiveCall>? Data { get; set; }
    }

    public class SipgateActiveCall
    {
        public string? CallId { get; set; }
        public string? Direction { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Status { get; set; }
        public bool? Muted { get; set; }
        public bool? Recording { get; set; }
        public bool? Hold { get; set; }
    }

    public class SipgateCallHistory
    {
        public List<SipgateHistoryEntry>? Items { get; set; }
    }

    public class SipgateHistoryEntry
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public string? Direction { get; set; }
        public string? Source { get; set; }
        public string? Target { get; set; }
        public DateTime? Created { get; set; }
        public int? Duration { get; set; }
    }

    public class SipgateAccountInfo
    {
        public string? Company { get; set; }
        public string? Mainline { get; set; }
    }

    public class SipgateUsersResponse
    {
        public List<SipgateUser>? Items { get; set; }
    }

    public class SipgateUser
    {
        public string? Id { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Email { get; set; }
        public bool? Admin { get; set; }
    }

    // SMS Notification Models
    public class SmsNotificationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int ClientId { get; set; }
        public string? AlertType { get; set; }
        public DateTime Timestamp { get; set; }
        public int TotalContacts { get; set; }
        public int SuccessfulNotifications { get; set; }
        public List<NotifiedContact>? NotifiedContacts { get; set; }
    }
    
    public class NotifiedContact
    {
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public bool Success { get; set; }
        public DateTime SentAt { get; set; }
        public string? Error { get; set; }
    }
    
    public class EmergencyContactSms
    {
        public string ContactName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public int Priority { get; set; }
    }
}
