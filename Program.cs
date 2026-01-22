using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using UMOApi.Data;
using UMOApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure SQLite Database
builder.Services.AddDbContext<UMOApiDbContext>(options =>
    options.UseSqlite("Data Source=UMOApi.db"));

// Register sipgate service
builder.Services.AddSingleton<ISipgateService, SipgateService>();

// Register SIPSorcery telephony service
builder.Services.AddSingleton<SipSorceryTelephonyService>();

// Configure CORS for web client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "UMO API",
        Version = "v1",
        Description = "API for managing clients, devices, providers, emergency services, and system entries in the UMO system. Includes Service Hub for VoIP telephony via sipgate."
    });
    
    // Enable XML comments for better documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UMOApiDbContext>();
    // Ensure database is created (without deleting existing data)
    context.Database.EnsureCreated();
    
    // Seed Service Hub data
    await SeedServiceHubDataAsync(context);
    
    // Initialize SIPSorcery telephony service
    try
    {
        var telephonyService = scope.ServiceProvider.GetRequiredService<SipSorceryTelephonyService>();
        await telephonyService.InitializeAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Telefonie-Service konnte nicht initialisiert werden: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UMO API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAll");

app.UseAuthorization();

// Serve static files for the web client
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

// Fallback to index.html for SPA routing
app.MapFallbackToFile("index.html");

app.Run();

// Seed method for Service Hub data
async Task SeedServiceHubDataAsync(UMOApiDbContext context)
{
    // Check if already seeded
    if (await context.Dispatchers.AnyAsync())
        return;

    // Add Dispatchers
    var dispatchers = new[]
    {
        new UMOApi.Models.Dispatcher
        {
            FirstName = "Maria",
            LastName = "Schmidt",
            Username = "mschmidt",
            Email = "m.schmidt@leitstelle.de",
            Extension = "101",
            SipExtension = "e0",
            Status = "Online",
            IsAvailable = true,
            Role = "Agent",
            TotalCallsHandled = 245
        },
        new UMOApi.Models.Dispatcher
        {
            FirstName = "Thomas",
            LastName = "Weber",
            Username = "tweber",
            Email = "t.weber@leitstelle.de",
            Extension = "102",
            SipExtension = "e1",
            Status = "Online",
            IsAvailable = true,
            Role = "Agent",
            TotalCallsHandled = 189
        },
        new UMOApi.Models.Dispatcher
        {
            FirstName = "Sandra",
            LastName = "Müller",
            Username = "smueller",
            Email = "s.mueller@leitstelle.de",
            Extension = "103",
            SipExtension = "e2",
            Status = "Break",
            IsAvailable = false,
            Role = "Supervisor",
            TotalCallsHandled = 312
        }
    };
    context.Dispatchers.AddRange(dispatchers);
    await context.SaveChangesAsync();

    // Add SIP Configuration
    var sipConfig = new UMOApi.Models.SipConfiguration
    {
        Name = "sipgate Hauptanschluss",
        SipServer = "sipconnect.sipgate.de",
        WebSocketUrl = "wss://sipconnect.sipgate.de",
        SipUsername = "3938564t0",
        SipPassword = "VEWqXdhf9wty",
        SipDomain = "sipgate.de",
        SipPort = 5060,
        UseTls = true,
        IsActive = true
    };
    context.SipConfigurations.Add(sipConfig);
    await context.SaveChangesAsync();

    // Add Emergency Devices
    var devices = new[]
    {
        new UMOApi.Models.EmergencyDevice
        {
            DeviceName = "Apple Watch - Müller",
            DeviceType = "AppleWatch",
            Manufacturer = "Apple",
            Model = "Watch Series 9",
            SerialNumber = "AW-2024-001",
            PhoneNumber = "+4930123456001",
            SipIdentifier = "aw001",
            ClientId = null, // Wird später zugewiesen
            Status = "Active",
            IsOnline = true,
            BatteryLevel = 85,
            LastHeartbeat = DateTime.UtcNow.AddMinutes(-5)
        },
        new UMOApi.Models.EmergencyDevice
        {
            DeviceName = "Hausnotruf - Schmidt",
            DeviceType = "Hausnotruf",
            Manufacturer = "Tunstall",
            Model = "Lifeline Vi+",
            SerialNumber = "TN-2024-002",
            PhoneNumber = "+4930123456002",
            SipIdentifier = "hn002",
            ClientId = null, // Wird später zugewiesen
            Status = "Active",
            IsOnline = true,
            BatteryLevel = 100,
            LastHeartbeat = DateTime.UtcNow.AddMinutes(-2)
        },
        new UMOApi.Models.EmergencyDevice
        {
            DeviceName = "Mobiler Notruf - Weber",
            DeviceType = "MobilNotruf",
            Manufacturer = "Bosch",
            Model = "Smartlife Care",
            SerialNumber = "BS-2024-003",
            PhoneNumber = "+4930123456003",
            SipIdentifier = "mn003",
            ClientId = null, // Wird später zugewiesen
            Status = "Active",
            IsOnline = false,
            BatteryLevel = 45,
            LastHeartbeat = DateTime.UtcNow.AddHours(-2)
        },
        new UMOApi.Models.EmergencyDevice
        {
            DeviceName = "Apple Watch - Fischer",
            DeviceType = "AppleWatch",
            Manufacturer = "Apple",
            Model = "Watch Ultra 2",
            SerialNumber = "AW-2024-004",
            PhoneNumber = "+4930123456004",
            SipIdentifier = "aw004",
            ClientId = null, // Wird später zugewiesen
            Status = "Active",
            IsOnline = true,
            BatteryLevel = 92,
            LastHeartbeat = DateTime.UtcNow.AddMinutes(-1)
        }
    };
    context.EmergencyDevices.AddRange(devices);
    await context.SaveChangesAsync();

    // Add Emergency Contacts
    var contacts = new[]
    {
        new UMOApi.Models.EmergencyContact
        {
            ClientId = null, // Wird später zugewiesen
            FirstName = "Peter",
            LastName = "Müller",
            Relationship = "Sohn",
            PhoneNumber = "+4930111222333",
            MobileNumber = "+49171111222333",
            Email = "peter.mueller@email.de",
            Priority = 1,
            IsAvailable24h = true,
            HasKey = true,
            Notes = "Erreichbar auch nachts"
        },
        new UMOApi.Models.EmergencyContact
        {
            ClientId = null, // Wird später zugewiesen
            FirstName = "Anna",
            LastName = "Müller",
            Relationship = "Tochter",
            PhoneNumber = "+4930222333444",
            MobileNumber = "+49172222333444",
            Email = "anna.mueller@email.de",
            Priority = 2,
            IsAvailable24h = false,
            HasKey = true,
            Notes = "Tagsüber erreichbar 8-18 Uhr"
        },
        new UMOApi.Models.EmergencyContact
        {
            ClientId = null, // Wird später zugewiesen
            FirstName = "Klaus",
            LastName = "Schmidt",
            Relationship = "Ehemann",
            PhoneNumber = "+4930333444555",
            MobileNumber = "+49173333444555",
            Email = "klaus.schmidt@email.de",
            Priority = 1,
            IsAvailable24h = true,
            HasKey = true,
            Notes = "Lebt im gleichen Haushalt"
        },
        new UMOApi.Models.EmergencyContact
        {
            ClientId = null, // Wird später zugewiesen
            FirstName = "Dr. med.",
            LastName = "Hoffmann",
            Relationship = "Hausarzt",
            PhoneNumber = "+4930444555666",
            MobileNumber = "",
            Email = "praxis@dr-hoffmann.de",
            Priority = 3,
            IsAvailable24h = false,
            HasKey = false,
            Notes = "Praxiszeiten Mo-Fr 8-17 Uhr"
        }
    };
    context.EmergencyContacts.AddRange(contacts);
    await context.SaveChangesAsync();

    // Add some sample alerts
    var alerts = new[]
    {
        new UMOApi.Models.EmergencyAlert
        {
            AlertType = "FallDetection",
            Priority = "High",
            Status = "Resolved",
            EmergencyDeviceId = null, // Wird später zugewiesen
            ClientId = null,
            CallerNumber = "+4930123456001",
            AlertTime = DateTime.UtcNow.AddHours(-3),
            AcknowledgedTime = DateTime.UtcNow.AddHours(-3).AddMinutes(1),
            ResolvedTime = DateTime.UtcNow.AddHours(-3).AddMinutes(15),
            AcknowledgedByDispatcherId = 1,
            ResolvedByDispatcherId = 1,
            Resolution = "FalseAlarm",
            Notes = "Klient hat versehentlich Alarm ausgelöst. Rückruf bestätigt, dass alles in Ordnung ist.",
            HeartRate = 78
        },
        new UMOApi.Models.EmergencyAlert
        {
            AlertType = "ManualAlert",
            Priority = "Critical",
            Status = "InProgress",
            EmergencyDeviceId = null, // Wird später zugewiesen
            ClientId = null,
            CallerNumber = "+4930123456002",
            AlertTime = DateTime.UtcNow.AddMinutes(-10),
            AcknowledgedTime = DateTime.UtcNow.AddMinutes(-9),
            AcknowledgedByDispatcherId = 2,
            Notes = "Klient meldet Unwohlsein. Angehörige werden kontaktiert.",
            HeartRate = 95,
            ContactsNotified = true
        },
        new UMOApi.Models.EmergencyAlert
        {
            AlertType = "FallDetection",
            Priority = "High",
            Status = "New",
            EmergencyDeviceId = null, // Wird später zugewiesen
            ClientId = null,
            CallerNumber = "+4930123456004",
            AlertTime = DateTime.UtcNow.AddMinutes(-2),
            Latitude = 52.5200,
            Longitude = 13.4050,
            HeartRate = 110,
            Notes = "Automatische Sturzerkennung durch Apple Watch"
        }
    };
    context.EmergencyAlerts.AddRange(alerts);
    await context.SaveChangesAsync();

    // Add sample call logs
    var callLogs = new[]
    {
        new UMOApi.Models.CallLog
        {
            SipgateCallId = "call-001",
            Direction = "Inbound",
            CallerNumber = "+4930123456001",
            CalleeNumber = "101",
            DispatcherId = null, // Wird später zugewiesen
            ClientId = null,
            EmergencyAlertId = null,
            Status = "Ended",
            StartTime = DateTime.UtcNow.AddHours(-3),
            ConnectTime = DateTime.UtcNow.AddHours(-3).AddSeconds(5),
            EndTime = DateTime.UtcNow.AddHours(-3).AddMinutes(5),
            DurationSeconds = 295,
            EndReason = "Completed",
            CallType = "Emergency",
            Notes = "Sturzerkennung - Fehlalarm bestätigt"
        },
        new UMOApi.Models.CallLog
        {
            SipgateCallId = "call-002",
            Direction = "Outbound",
            CallerNumber = "101",
            CalleeNumber = "+4930111222333",
            DispatcherId = null, // Wird später zugewiesen
            ClientId = null,
            EmergencyContactId = null,
            EmergencyAlertId = null,
            Status = "Ended",
            StartTime = DateTime.UtcNow.AddHours(-3).AddMinutes(6),
            ConnectTime = DateTime.UtcNow.AddHours(-3).AddMinutes(6).AddSeconds(10),
            EndTime = DateTime.UtcNow.AddHours(-3).AddMinutes(10),
            DurationSeconds = 230,
            EndReason = "Completed",
            CallType = "Callback",
            Notes = "Rückruf an Sohn - Entwarnung gegeben"
        },
        new UMOApi.Models.CallLog
        {
            SipgateCallId = "call-003",
            Direction = "Inbound",
            CallerNumber = "+4930123456002",
            CalleeNumber = "102",
            DispatcherId = null, // Wird später zugewiesen
            ClientId = null,
            EmergencyAlertId = null,
            Status = "Connected",
            StartTime = DateTime.UtcNow.AddMinutes(-10),
            ConnectTime = DateTime.UtcNow.AddMinutes(-9).AddSeconds(30),
            CallType = "Emergency",
            Notes = "Aktiver Notruf - Klient am Telefon"
        }
    };
    context.CallLogs.AddRange(callLogs);

    await context.SaveChangesAsync();
}
