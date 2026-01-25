using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using UMOApi.Data;
using UMOApi.Services;

// =================================================================================================
// APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION
// This file represents the entry point of the transformed core application.
// It configures services, database, and the HTTP pipeline based on the App Fabric's
// architectural guidelines (Clean Architecture, .NET 8, EF Core).
//
// META-DATA:
//   - Transformation-Target: .NET 8 Web API
//   - Architecture: Clean Architecture (inferred)
//   - Database-Provider: Entity Framework Core (Azure SQL / SQLite)
//   - Core-Functionality: UMO API for Emergency Call Center Management
// =================================================================================================

var builder = WebApplication.CreateBuilder(args);

// =================================================================================================
// STAGE 2.1: SERVICE REGISTRATION (Dependency Injection)
// All services and dependencies are registered here. This follows the IServiceCollection pattern
// and allows for loose coupling and testability, key principles of Clean Architecture.
// =================================================================================================

// Add services to the container.
builder.Services.AddControllers();

// Configure Database - Azure SQL or SQLite fallback
// This section demonstrates a dynamic database provider selection, a key feature for
// flexible deployment environments (local vs. cloud).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains("database.windows.net"))
{
    // Azure SQL Server - Production Environment
    builder.Services.AddDbContext<UMOApiDbContext>(options =>
        options.UseSqlServer(connectionString));
    Console.WriteLine("Using Azure SQL Database");
}
else
{
    // SQLite fallback for local development
    builder.Services.AddDbContext<UMOApiDbContext>(options =>
        options.UseSqlite("Data Source=UMOApi.db"));
    Console.WriteLine("Using SQLite Database");
}

// Register sipgate service for VoIP integration
// This service encapsulates the logic for interacting with the Sipgate API.
builder.Services.AddSingleton<ISipgateService, SipgateService>();

// Register SIPSorcery telephony service for advanced telephony features
builder.Services.AddSingleton<SipSorceryTelephonyService>();

// Configure CORS for web client
// Allows the frontend application to communicate with the API.
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
    
    // Enable XML comments for better documentation in Swagger UI
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add Application Insights for monitoring in Azure
var appInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
if (!string.IsNullOrEmpty(appInsightsConnectionString))
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = appInsightsConnectionString;
    });
    Console.WriteLine("Application Insights enabled");
}

// =================================================================================================
// STAGE 2.2: HTTP REQUEST PIPELINE CONFIGURATION
// The middleware pipeline is configured here. The order of middleware is crucial for
// security, performance, and functionality.
// =================================================================================================

var app = builder.Build();

// Ensure database is created and seeded on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UMOApiDbContext>();
    
    // For SQL Server, use migrations; for SQLite, use EnsureCreated
    var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(dbConnectionString) && dbConnectionString.Contains("database.windows.net"))
    {
        // Apply migrations for Azure SQL
        context.Database.Migrate();
    }
    else
    {
        // Ensure database is created for SQLite
        context.Database.EnsureCreated();
    }
    
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

// Serve static files for the web client (if any)
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

// Fallback to index.html for Single Page Application (SPA) routing
app.MapFallbackToFile("index.html");

// =================================================================================================
// STAGE 3: DEPLOYMENT AUTOMATION (Aspire Orchestration)
// The app.Run() command starts the application. In an Aspire project, this would be
// orchestrated by the AppHost, which manages the lifecycle of all services.
// =================================================================================================

app.Run();

// =================================================================================================
// STAGE 1: TEMPLATE & MOCKUP (Seed Data)
// This method provides the initial data for the application, acting as a functional mockup.
// In a real App Fabric scenario, this data could be derived from a JSON template.
// =================================================================================================
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
            ClientId = null,
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
            ClientId = null,
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
            ClientId = null,
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
            ClientId = null,
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
            ClientId = null,
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
            ClientId = null,
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
            ClientId = null,
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
            ClientId = null,
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
            EmergencyDeviceId = null,
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
            EmergencyDeviceId = null,
            ClientId = null,
            CallerNumber = "+4930123456002",
            AlertTime = DateTime.UtcNow.AddMinutes(-10),
            AcknowledgedTime = DateTime.UtcNow.AddMinutes(-9),
            AcknowledgedByDispatcherId = 2,
            Notes = "Klient meldet Unwohlsein. Angehörige werden kontaktiert.",
            HeartRate = 95
        }
    };
    context.EmergencyAlerts.AddRange(alerts);
    await context.SaveChangesAsync();
}
