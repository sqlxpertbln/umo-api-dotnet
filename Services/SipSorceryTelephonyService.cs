using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SIPSorcery.SIP;
using SIPSorcery.SIP.App;

namespace UMOApi.Services
{
    /// <summary>
    /// Telefonie-Service basierend auf SIPSorcery für serverseitige SIP-Anrufe
    /// </summary>
    public class SipSorceryTelephonyService : IDisposable
    {
        private readonly ILogger<SipSorceryTelephonyService> _logger;
        private readonly SIPTransport _sipTransport;
        private SIPRegistrationUserAgent? _regUserAgent;
        private bool _isRegistered = false;
        private bool _disposed = false;

        // Sipgate Konfiguration
        private readonly string _sipServer = "sipgate.de";
        private readonly string _sipUsername = "3938564e0";
        private readonly string _sipPassword = "ihFauejmdjkb";
        private readonly int _sipPort = 5060;

        public bool IsRegistered => _isRegistered;

        public SipSorceryTelephonyService(ILogger<SipSorceryTelephonyService> logger)
        {
            _logger = logger;
            _sipTransport = new SIPTransport();
        }

        /// <summary>
        /// Initialisiert den SIP-Transport und registriert sich bei Sipgate
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Initialisiere SIP-Transport...");

                // SIP-Transport starten
                _sipTransport.AddSIPChannel(new SIPUDPChannel(new IPEndPoint(IPAddress.Any, 0)));

                // Bei Sipgate registrieren
                var sipUri = SIPURI.ParseSIPURI($"sip:{_sipUsername}@{_sipServer}");
                var regUri = SIPURI.ParseSIPURI($"sip:{_sipServer}");

                _regUserAgent = new SIPRegistrationUserAgent(
                    _sipTransport,
                    _sipUsername,
                    _sipPassword,
                    _sipServer,
                    120); // Registrierung alle 120 Sekunden erneuern

                _regUserAgent.RegistrationSuccessful += (uri, response) =>
                {
                    _isRegistered = true;
                    _logger.LogInformation("SIP-Registrierung bei {Server} erfolgreich", _sipServer);
                };

                _regUserAgent.RegistrationFailed += (uri, response, message) =>
                {
                    _isRegistered = false;
                    _logger.LogError("SIP-Registrierung fehlgeschlagen: {Message}", message);
                };

                _regUserAgent.RegistrationRemoved += (uri, response) =>
                {
                    _isRegistered = false;
                    _logger.LogInformation("SIP-Registrierung entfernt");
                };

                _regUserAgent.Start();

                // Warten auf Registrierung (max 10 Sekunden)
                for (int i = 0; i < 20 && !_isRegistered; i++)
                {
                    await Task.Delay(500);
                }

                if (_isRegistered)
                {
                    _logger.LogInformation("SIP-Service erfolgreich initialisiert und registriert");
                }
                else
                {
                    _logger.LogWarning("SIP-Registrierung noch nicht abgeschlossen");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler bei der SIP-Initialisierung");
                throw;
            }
        }

        /// <summary>
        /// Initiiert einen Anruf zwischen zwei Telefonnummern (Click-to-Call)
        /// Der Server ruft zuerst die Caller-Nummer an, dann wird zur Callee-Nummer verbunden
        /// </summary>
        public async Task<CallResult> InitiateCallAsync(string callerNumber, string calleeNumber)
        {
            if (!_isRegistered)
            {
                return new CallResult
                {
                    Success = false,
                    Message = "SIP nicht registriert",
                    SessionId = null
                };
            }

            try
            {
                _logger.LogInformation("Initiiere Anruf von {Caller} zu {Callee}", callerNumber, calleeNumber);

                // Nummer formatieren
                var formattedCallee = FormatPhoneNumber(calleeNumber);
                var callUri = SIPURI.ParseSIPURI($"sip:{formattedCallee}@{_sipServer}");

                // SIP User Agent für den Anruf erstellen
                var userAgent = new SIPUserAgent(_sipTransport, null);
                
                // Anruf-Ergebnis
                var sessionId = Guid.NewGuid().ToString();
                var callSuccess = false;
                var callMessage = "";

                userAgent.ClientCallFailed += (uac, error, response) =>
                {
                    callMessage = $"Anruf fehlgeschlagen: {error}";
                    _logger.LogError("Anruf zu {Callee} fehlgeschlagen: {Error}", calleeNumber, error);
                };

                userAgent.ClientCallAnswered += (uac, response) =>
                {
                    callSuccess = true;
                    callMessage = "Anruf angenommen";
                    _logger.LogInformation("Anruf zu {Callee} wurde angenommen", calleeNumber);
                };

                // Anruf starten (ohne Media für Server-Umgebung)
                var callResult = await userAgent.Call(callUri.ToString(), _sipUsername, _sipPassword, null);

                if (callResult)
                {
                    return new CallResult
                    {
                        Success = true,
                        Message = "Anruf erfolgreich initiiert",
                        SessionId = sessionId
                    };
                }
                else
                {
                    return new CallResult
                    {
                        Success = false,
                        Message = callMessage ?? "Anruf konnte nicht hergestellt werden",
                        SessionId = null
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Initiieren des Anrufs");
                return new CallResult
                {
                    Success = false,
                    Message = $"Fehler: {ex.Message}",
                    SessionId = null
                };
            }
        }

        /// <summary>
        /// Verwendet die Sipgate REST API für Click-to-Call (zuverlässiger für Server-Umgebungen)
        /// </summary>
        public async Task<CallResult> InitiateClickToCallAsync(string callerExtension, string calleeNumber)
        {
            try
            {
                _logger.LogInformation("Initiiere Click-to-Call von Extension {Extension} zu {Callee}", 
                    callerExtension, calleeNumber);

                using var httpClient = new HttpClient();
                
                // Sipgate Personal Access Token
                var token = "token-AP98OS:ac6eb762-5c4f-4ec9-866e-7f2d5a8d71ce";
                var authHeader = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

                var requestBody = new
                {
                    caller = callerExtension, // z.B. "e0" für VoIP-Telefon
                    callee = FormatPhoneNumber(calleeNumber)
                };

                var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://api.sipgate.com/v2/sessions/calls", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Click-to-Call erfolgreich initiiert");
                    return new CallResult
                    {
                        Success = true,
                        Message = "Anruf erfolgreich initiiert",
                        SessionId = responseContent
                    };
                }
                else
                {
                    _logger.LogError("Click-to-Call fehlgeschlagen: {Status} - {Content}", 
                        response.StatusCode, responseContent);
                    return new CallResult
                    {
                        Success = false,
                        Message = $"Fehler: {response.StatusCode} - {responseContent}",
                        SessionId = null
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Click-to-Call");
                return new CallResult
                {
                    Success = false,
                    Message = $"Fehler: {ex.Message}",
                    SessionId = null
                };
            }
        }

        /// <summary>
        /// Sendet eine SMS über die Sipgate API
        /// </summary>
        public async Task<bool> SendSmsAsync(string recipientNumber, string message)
        {
            try
            {
                _logger.LogInformation("Sende SMS an {Recipient}", recipientNumber);

                using var httpClient = new HttpClient();
                
                var token = "token-AP98OS:ac6eb762-5c4f-4ec9-866e-7f2d5a8d71ce";
                var authHeader = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

                var requestBody = new
                {
                    smsId = "s0", // SMS-Extension
                    recipient = FormatPhoneNumber(recipientNumber),
                    message = message
                };

                var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://api.sipgate.com/v2/sessions/sms", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("SMS erfolgreich gesendet an {Recipient}", recipientNumber);
                    return true;
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("SMS senden fehlgeschlagen: {Status} - {Content}", 
                        response.StatusCode, responseContent);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim SMS senden");
                return false;
            }
        }

        /// <summary>
        /// Gibt den aktuellen Registrierungsstatus zurück
        /// </summary>
        public TelephonyStatus GetStatus()
        {
            return new TelephonyStatus
            {
                IsRegistered = _isRegistered,
                SipServer = _sipServer,
                SipUsername = _sipUsername,
                LastUpdate = DateTime.UtcNow
            };
        }

        private string FormatPhoneNumber(string number)
        {
            // Nummer bereinigen
            var cleaned = new string(number.Where(c => char.IsDigit(c) || c == '+').ToArray());
            
            // Wenn die Nummer mit 0 beginnt, deutsche Vorwahl hinzufügen
            if (cleaned.StartsWith("0") && !cleaned.StartsWith("00"))
            {
                cleaned = "+49" + cleaned.Substring(1);
            }
            
            return cleaned;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _regUserAgent?.Stop();
                _sipTransport?.Shutdown();
                _disposed = true;
            }
        }
    }

    public class CallResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? SessionId { get; set; }
    }

    public class TelephonyStatus
    {
        public bool IsRegistered { get; set; }
        public string? SipServer { get; set; }
        public string? SipUsername { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
