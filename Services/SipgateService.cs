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
                var payload = new
                {
                    smsId = smsId,
                    recipient = recipient,
                    message = message
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/sessions/sms", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS");
                return false;
            }
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
}
