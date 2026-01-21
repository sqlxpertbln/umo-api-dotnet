using Microsoft.EntityFrameworkCore;
using UMOApi.Models;

namespace UMOApi.Data;

/// <summary>
/// Database context for the UMO API application.
/// </summary>
public class UMOApiDbContext : DbContext
{
    public UMOApiDbContext(DbContextOptions<UMOApiDbContext> options) : base(options)
    {
    }

    // Client-related entities
    public DbSet<Client> Clients { get; set; }
    public DbSet<ClientDetails> ClientDetails { get; set; }
    public DbSet<ClientStatus> ClientStatuses { get; set; }
    public DbSet<ClientStatusHistory> ClientStatusHistories { get; set; }
    public DbSet<ClientPhone> ClientPhones { get; set; }
    public DbSet<ClientNotation> ClientNotations { get; set; }
    public DbSet<ClientFeature> ClientFeatures { get; set; }
    public DbSet<ClientCost> ClientCosts { get; set; }
    public DbSet<ClientDisease> ClientDiseases { get; set; }
    public DbSet<ClientMedecin> ClientMedecins { get; set; }

    // Address-related entities
    public DbSet<Address> Addresses { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Country> Countries { get; set; }

    // Device-related entities
    public DbSet<Device> Devices { get; set; }
    public DbSet<DeviceDetails> DeviceDetails { get; set; }

    // Provider-related entities
    public DbSet<DirectProvider> DirectProviders { get; set; }
    public DbSet<DirectProviderDetails> DirectProviderDetails { get; set; }
    public DbSet<ProfessionalProvider> ProfessionalProviders { get; set; }
    public DbSet<ProfessionalProviderDetails> ProfessionalProviderDetails { get; set; }
    public DbSet<ProfessionalProviderClientLink> ProfessionalProviderClientLinks { get; set; }

    // System-related entities
    public DbSet<SystemEntry> SystemEntries { get; set; }
    public DbSet<Tarif> Tarifs { get; set; }
    public DbSet<VatTax> VatTaxes { get; set; }
    public DbSet<User> Users { get; set; }

    // Marketing & CRM entities
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Lead> Leads { get; set; }
    public DbSet<LeadActivity> LeadActivities { get; set; }

    // Sales entities
    public DbSet<SalesOpportunity> SalesOpportunities { get; set; }
    public DbSet<SalesOrder> SalesOrders { get; set; }
    public DbSet<SalesOrderItem> SalesOrderItems { get; set; }

    // ERP entities
    public DbSet<Article> Articles { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<Medication> Medications { get; set; }

    // Billing & Finance entities
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<CostCenter> CostCenters { get; set; }

    // Service Hub / Emergency entities
    public DbSet<EmergencyDevice> EmergencyDevices { get; set; }
    public DbSet<EmergencyContact> EmergencyContacts { get; set; }
    public DbSet<EmergencyAlert> EmergencyAlerts { get; set; }
    public DbSet<Dispatcher> Dispatchers { get; set; }
    public DbSet<CallLog> CallLogs { get; set; }
    public DbSet<SipConfiguration> SipConfigurations { get; set; }
    public DbSet<DispatcherShift> DispatcherShifts { get; set; }
    public DbSet<EmergencyChainAction> EmergencyChainActions { get; set; }
    
    // Client Medications (Medikamentenliste für Hausnotruf)
    public DbSet<ClientMedication> ClientMedications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure composite keys and indexes
        modelBuilder.Entity<Client>()
            .HasIndex(c => new { c.MandantId, c.Nummer })
            .IsUnique();

        modelBuilder.Entity<ClientDetails>()
            .HasIndex(c => new { c.MandantId, c.Nummer })
            .IsUnique();

        modelBuilder.Entity<Device>()
            .HasIndex(d => new { d.MandantId, d.SerialNumber });

        modelBuilder.Entity<SystemEntry>()
            .HasIndex(s => new { s.MandantId, s.Type, s.Code });

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // Configure ClientDetails relationships
        modelBuilder.Entity<ClientDetails>()
            .HasMany(c => c.Costs)
            .WithOne(c => c.Client)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ClientDetails>()
            .HasMany(c => c.StatusHistory)
            .WithOne(c => c.Client)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ClientDetails>()
            .HasMany(c => c.Phones)
            .WithOne(c => c.Client)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ClientDetails>()
            .HasMany(c => c.Notations)
            .WithOne(c => c.Client)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ClientDetails>()
            .HasMany(c => c.Features)
            .WithOne(c => c.Client)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ClientDetails>()
            .HasMany(c => c.Diseases)
            .WithOne(c => c.Client)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ClientDetails>()
            .HasMany(c => c.Medicin)
            .WithOne(c => c.Client)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Countries
        modelBuilder.Entity<Country>().HasData(
            new Country { Id = 1, Name = "Deutschland", IsoCode = "DE", PhoneCode = "+49" },
            new Country { Id = 2, Name = "Österreich", IsoCode = "AT", PhoneCode = "+43" },
            new Country { Id = 3, Name = "Schweiz", IsoCode = "CH", PhoneCode = "+41" },
            new Country { Id = 4, Name = "Luxemburg", IsoCode = "LU", PhoneCode = "+352" }
        );

        // Seed Districts
        modelBuilder.Entity<District>().HasData(
            new District { Id = 1, MandantId = 1, Name = "Bayern", Code = "BY", CountryId = 1 },
            new District { Id = 2, MandantId = 1, Name = "Baden-Württemberg", Code = "BW", CountryId = 1 },
            new District { Id = 3, MandantId = 1, Name = "Nordrhein-Westfalen", Code = "NW", CountryId = 1 },
            new District { Id = 4, MandantId = 1, Name = "Wien", Code = "W", CountryId = 2 }
        );

        // Seed Cities
        modelBuilder.Entity<City>().HasData(
            new City { Id = 1, MandantId = 1, Name = "München", ZipCode = "80331", DistrictId = 1, CountryId = 1 },
            new City { Id = 2, MandantId = 1, Name = "Stuttgart", ZipCode = "70173", DistrictId = 2, CountryId = 1 },
            new City { Id = 3, MandantId = 1, Name = "Köln", ZipCode = "50667", DistrictId = 3, CountryId = 1 },
            new City { Id = 4, MandantId = 1, Name = "Wien", ZipCode = "1010", DistrictId = 4, CountryId = 2 }
        );

        // Seed System Entries - Titles
        modelBuilder.Entity<SystemEntry>().HasData(
            new SystemEntry { Id = 1, MandantId = 1, Type = "T", Description = "Herr", Code = "MR", SortOrder = 1, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 2, MandantId = 1, Type = "T", Description = "Frau", Code = "MRS", SortOrder = 2, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 3, MandantId = 1, Type = "T", Description = "Divers", Code = "DIV", SortOrder = 3, IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed System Entries - Prefixes
        modelBuilder.Entity<SystemEntry>().HasData(
            new SystemEntry { Id = 4, MandantId = 1, Type = "P", Description = "Dr.", Code = "DR", SortOrder = 1, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 5, MandantId = 1, Type = "P", Description = "Prof.", Code = "PROF", SortOrder = 2, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 6, MandantId = 1, Type = "P", Description = "Prof. Dr.", Code = "PROFDR", SortOrder = 3, IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed System Entries - Status
        modelBuilder.Entity<SystemEntry>().HasData(
            new SystemEntry { Id = 7, MandantId = 1, Type = "S", Description = "Aktiv", Code = "ACTIVE", SortOrder = 1, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 8, MandantId = 1, Type = "S", Description = "Inaktiv", Code = "INACTIVE", SortOrder = 2, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 9, MandantId = 1, Type = "S", Description = "Pausiert", Code = "PAUSED", SortOrder = 3, IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed System Entries - Marital Status
        modelBuilder.Entity<SystemEntry>().HasData(
            new SystemEntry { Id = 10, MandantId = 1, Type = "M", Description = "Ledig", Code = "SINGLE", SortOrder = 1, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 11, MandantId = 1, Type = "M", Description = "Verheiratet", Code = "MARRIED", SortOrder = 2, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 12, MandantId = 1, Type = "M", Description = "Geschieden", Code = "DIVORCED", SortOrder = 3, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 13, MandantId = 1, Type = "M", Description = "Verwitwet", Code = "WIDOWED", SortOrder = 4, IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed System Entries - Invoice Method
        modelBuilder.Entity<SystemEntry>().HasData(
            new SystemEntry { Id = 14, MandantId = 1, Type = "I", Description = "Rechnung", Code = "INVOICE", SortOrder = 1, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 15, MandantId = 1, Type = "I", Description = "Lastschrift", Code = "DEBIT", SortOrder = 2, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 16, MandantId = 1, Type = "I", Description = "Bar", Code = "CASH", SortOrder = 3, IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed System Entries - Priority
        modelBuilder.Entity<SystemEntry>().HasData(
            new SystemEntry { Id = 17, MandantId = 1, Type = "PR", Description = "Niedrig", Code = "LOW", SortOrder = 1, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 18, MandantId = 1, Type = "PR", Description = "Normal", Code = "NORMAL", SortOrder = 2, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 19, MandantId = 1, Type = "PR", Description = "Hoch", Code = "HIGH", SortOrder = 3, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 20, MandantId = 1, Type = "PR", Description = "Dringend", Code = "URGENT", SortOrder = 4, IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed System Entries - Language
        modelBuilder.Entity<SystemEntry>().HasData(
            new SystemEntry { Id = 21, MandantId = 1, Type = "L", Description = "Deutsch", Code = "DE", SortOrder = 1, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 22, MandantId = 1, Type = "L", Description = "Englisch", Code = "EN", SortOrder = 2, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 23, MandantId = 1, Type = "L", Description = "Französisch", Code = "FR", SortOrder = 3, IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed System Entries - Reason
        modelBuilder.Entity<SystemEntry>().HasData(
            new SystemEntry { Id = 24, MandantId = 1, Type = "R", Description = "Pflegebedarf", Code = "CARE", SortOrder = 1, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 25, MandantId = 1, Type = "R", Description = "Rehabilitation", Code = "REHAB", SortOrder = 2, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 26, MandantId = 1, Type = "R", Description = "Therapie", Code = "THERAPY", SortOrder = 3, IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed System Entries - Insurance Class
        modelBuilder.Entity<SystemEntry>().HasData(
            new SystemEntry { Id = 27, MandantId = 1, Type = "IC", Description = "Gesetzlich", Code = "PUBLIC", SortOrder = 1, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 28, MandantId = 1, Type = "IC", Description = "Privat", Code = "PRIVATE", SortOrder = 2, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 29, MandantId = 1, Type = "IC", Description = "Selbstzahler", Code = "SELF", SortOrder = 3, IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed System Entries - Insurance Care
        modelBuilder.Entity<SystemEntry>().HasData(
            new SystemEntry { Id = 30, MandantId = 1, Type = "ICR", Description = "Pflegegrad 1", Code = "PG1", SortOrder = 1, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 31, MandantId = 1, Type = "ICR", Description = "Pflegegrad 2", Code = "PG2", SortOrder = 2, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 32, MandantId = 1, Type = "ICR", Description = "Pflegegrad 3", Code = "PG3", SortOrder = 3, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 33, MandantId = 1, Type = "ICR", Description = "Pflegegrad 4", Code = "PG4", SortOrder = 4, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 34, MandantId = 1, Type = "ICR", Description = "Pflegegrad 5", Code = "PG5", SortOrder = 5, IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed System Entries - Financial Group
        modelBuilder.Entity<SystemEntry>().HasData(
            new SystemEntry { Id = 35, MandantId = 1, Type = "FG", Description = "Gruppe A", Code = "GA", SortOrder = 1, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 36, MandantId = 1, Type = "FG", Description = "Gruppe B", Code = "GB", SortOrder = 2, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 37, MandantId = 1, Type = "FG", Description = "Gruppe C", Code = "GC", SortOrder = 3, IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed VAT Taxes
        modelBuilder.Entity<VatTax>().HasData(
            new VatTax { Id = 1, MandantId = 1, Name = "Standard", Percentage = 19.0m, Code = "STD", IsActive = true, CreateDate = DateTime.UtcNow },
            new VatTax { Id = 2, MandantId = 1, Name = "Ermäßigt", Percentage = 7.0m, Code = "RED", IsActive = true, CreateDate = DateTime.UtcNow },
            new VatTax { Id = 3, MandantId = 1, Name = "Steuerfrei", Percentage = 0.0m, Code = "FREE", IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed Tarifs
        modelBuilder.Entity<Tarif>().HasData(
            new Tarif { Id = 1, MandantId = 1, Name = "Basis-Tarif", Description = "Grundlegende Pflegeleistungen", BasePrice = 100.00m, VatTaxId = 3, TotalPrice = 100.00m, IsActive = true, CreateDate = DateTime.UtcNow },
            new Tarif { Id = 2, MandantId = 1, Name = "Premium-Tarif", Description = "Erweiterte Pflegeleistungen", BasePrice = 200.00m, VatTaxId = 3, TotalPrice = 200.00m, IsActive = true, CreateDate = DateTime.UtcNow },
            new Tarif { Id = 3, MandantId = 1, Name = "Intensiv-Tarif", Description = "Intensive Pflegeleistungen", BasePrice = 350.00m, VatTaxId = 3, TotalPrice = 350.00m, IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed default admin user (password: admin123)
        modelBuilder.Entity<User>().HasData(
            new User 
            { 
                Id = 1, 
                MandantId = 1, 
                Username = "admin", 
                PasswordHash = "AQAAAAIAAYagAAAAELBzKvJvYQQwG5GqKqLvPxQYl5CvxNFj0VBmJqPqYqPqYqPqYqPqYqPqYqPqYqPqYg==", // admin123
                Email = "admin@umoapi.local",
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin",
                IsActive = true,
                CreateDate = DateTime.UtcNow
            }
        );

        // Seed sample addresses
        modelBuilder.Entity<Address>().HasData(
            new Address { Id = 1, MandantId = 1, Street = "Hauptstraße", HouseNumber = "1", ZipCode = "80331", CityId = 1, DistrictId = 1, CountryId = 1, CreateDate = DateTime.UtcNow },
            new Address { Id = 2, MandantId = 1, Street = "Königstraße", HouseNumber = "50", ZipCode = "70173", CityId = 2, DistrictId = 2, CountryId = 1, CreateDate = DateTime.UtcNow }
        );

        // Seed sample clients
        modelBuilder.Entity<ClientDetails>().HasData(
            new ClientDetails 
            { 
                Id = 1, 
                MandantId = 1, 
                Nummer = 1001, 
                TitelId = 1, 
                FirstName = "Max", 
                LastName = "Mustermann", 
                Sex = "M", 
                BirthDay = new DateTime(1960, 5, 15),
                StatusId = 7,
                StartContractDate = new DateTime(2024, 1, 1),
                PolicyNumber = "POL-001",
                MaritalStatusId = 11,
                LanguageId = 21,
                PriorityId = 18,
                InvoiceMethodId = 14,
                ClassificationId = 27,
                AddressId = 1,
                TarifId = 1,
                CreateId = "admin",
                CreateDate = DateTime.UtcNow
            },
            new ClientDetails 
            { 
                Id = 2, 
                MandantId = 1, 
                Nummer = 1002, 
                TitelId = 2, 
                FirstName = "Erika", 
                LastName = "Musterfrau", 
                Sex = "F", 
                BirthDay = new DateTime(1955, 8, 22),
                StatusId = 7,
                StartContractDate = new DateTime(2024, 2, 1),
                PolicyNumber = "POL-002",
                MaritalStatusId = 13,
                LanguageId = 21,
                PriorityId = 19,
                InvoiceMethodId = 15,
                ClassificationId = 28,
                AddressId = 2,
                TarifId = 2,
                CreateId = "admin",
                CreateDate = DateTime.UtcNow
            }
        );

        // Seed sample devices
        modelBuilder.Entity<DeviceDetails>().HasData(
            new DeviceDetails
            {
                Id = 1,
                MandantId = 1,
                DeviceType = "Rollstuhl",
                SerialNumber = "RS-001-2024",
                Description = "Elektrischer Rollstuhl",
                Status = "Verfügbar",
                Manufacturer = "Ottobock",
                Model = "B500",
                PurchaseDate = new DateTime(2024, 1, 15),
                AssignedClientId = 1,
                CreateId = "admin",
                CreateDate = DateTime.UtcNow
            },
            new DeviceDetails
            {
                Id = 2,
                MandantId = 1,
                DeviceType = "Pflegebett",
                SerialNumber = "PB-002-2024",
                Description = "Elektrisches Pflegebett",
                Status = "In Benutzung",
                Manufacturer = "Burmeier",
                Model = "Regia",
                PurchaseDate = new DateTime(2024, 2, 1),
                AssignedClientId = 2,
                CreateId = "admin",
                CreateDate = DateTime.UtcNow
            }
        );

        // Seed sample professional providers
        modelBuilder.Entity<ProfessionalProviderDetails>().HasData(
            new ProfessionalProviderDetails
            {
                Id = 1,
                MandantId = 1,
                Name = "Dr. Schmidt",
                FirstName = "Hans",
                LastName = "Schmidt",
                Specialty = "Allgemeinmedizin",
                Street = "Arztstraße",
                HouseNumber = "10",
                ZipCode = "80331",
                City = "München",
                Country = "Deutschland",
                Phone = "+49 89 12345678",
                Email = "dr.schmidt@praxis.de",
                Status = "Aktiv",
                LicenseNumber = "DE-BY-12345",
                CreateId = "admin",
                CreateDate = DateTime.UtcNow
            },
            new ProfessionalProviderDetails
            {
                Id = 2,
                MandantId = 1,
                Name = "Dr. Müller",
                FirstName = "Anna",
                LastName = "Müller",
                Specialty = "Geriatrie",
                Street = "Klinikweg",
                HouseNumber = "5",
                ZipCode = "70173",
                City = "Stuttgart",
                Country = "Deutschland",
                Phone = "+49 711 87654321",
                Email = "dr.mueller@klinik.de",
                Status = "Aktiv",
                LicenseNumber = "DE-BW-67890",
                CreateId = "admin",
                CreateDate = DateTime.UtcNow
            }
        );

        // Seed sample direct providers
        modelBuilder.Entity<DirectProviderDetails>().HasData(
            new DirectProviderDetails
            {
                Id = 1,
                MandantId = 1,
                Name = "Pflegedienst Sonnenschein",
                FirstName = "Maria",
                LastName = "Huber",
                Street = "Pflegeweg",
                HouseNumber = "20",
                ZipCode = "80331",
                City = "München",
                Country = "Deutschland",
                Phone = "+49 89 11111111",
                Email = "info@sonnenschein-pflege.de",
                Status = "Aktiv",
                CreateId = "admin",
                CreateDate = DateTime.UtcNow
            },
            new DirectProviderDetails
            {
                Id = 2,
                MandantId = 1,
                Name = "Caritas Pflegedienst",
                FirstName = "Thomas",
                LastName = "Weber",
                Street = "Kirchstraße",
                HouseNumber = "15",
                ZipCode = "70173",
                City = "Stuttgart",
                Country = "Deutschland",
                Phone = "+49 711 22222222",
                Email = "info@caritas-pflege.de",
                Status = "Aktiv",
                CreateId = "admin",
                CreateDate = DateTime.UtcNow
            },
            new DirectProviderDetails
            {
                Id = 3,
                MandantId = 1,
                Name = "AWO Pflegedienst Köln",
                FirstName = "Sabine",
                LastName = "Klein",
                Street = "Domstraße",
                HouseNumber = "8",
                ZipCode = "50667",
                City = "Köln",
                Country = "Deutschland",
                Phone = "+49 221 33333333",
                Email = "info@awo-koeln.de",
                Status = "Aktiv",
                CreateId = "admin",
                CreateDate = DateTime.UtcNow
            }
        );

        // Seed additional Tarifs for better report data
        modelBuilder.Entity<Tarif>().HasData(
            new Tarif { Id = 4, MandantId = 1, Name = "Kompakt-Tarif", Description = "Kompakte Pflegeleistungen", BasePrice = 150.00m, VatTaxId = 3, TotalPrice = 150.00m, IsActive = true, CreateDate = DateTime.UtcNow },
            new Tarif { Id = 5, MandantId = 1, Name = "Familien-Tarif", Description = "Familienfreundliche Pflegeleistungen", BasePrice = 280.00m, VatTaxId = 3, TotalPrice = 280.00m, IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed additional addresses for more clients
        modelBuilder.Entity<Address>().HasData(
            new Address { Id = 3, MandantId = 1, Street = "Domstraße", HouseNumber = "25", ZipCode = "50667", CityId = 3, DistrictId = 3, CountryId = 1, CreateDate = DateTime.UtcNow },
            new Address { Id = 4, MandantId = 1, Street = "Ringstraße", HouseNumber = "12", ZipCode = "1010", CityId = 4, DistrictId = 4, CountryId = 2, CreateDate = DateTime.UtcNow },
            new Address { Id = 5, MandantId = 1, Street = "Marienplatz", HouseNumber = "3", ZipCode = "80331", CityId = 1, DistrictId = 1, CountryId = 1, CreateDate = DateTime.UtcNow },
            new Address { Id = 6, MandantId = 1, Street = "Schlossallee", HouseNumber = "100", ZipCode = "70173", CityId = 2, DistrictId = 2, CountryId = 1, CreateDate = DateTime.UtcNow },
            new Address { Id = 7, MandantId = 1, Street = "Rheinufer", HouseNumber = "45", ZipCode = "50667", CityId = 3, DistrictId = 3, CountryId = 1, CreateDate = DateTime.UtcNow },
            new Address { Id = 8, MandantId = 1, Street = "Stephansplatz", HouseNumber = "7", ZipCode = "1010", CityId = 4, DistrictId = 4, CountryId = 2, CreateDate = DateTime.UtcNow }
        );

        // Seed additional clients for meaningful reports
        modelBuilder.Entity<ClientDetails>().HasData(
            new ClientDetails 
            { 
                Id = 3, MandantId = 1, Nummer = 1003, TitelId = 1, FirstName = "Hans", LastName = "Meier", Sex = "M", 
                BirthDay = new DateTime(1945, 3, 10), StatusId = 7, StartContractDate = new DateTime(2024, 3, 15),
                PolicyNumber = "POL-003", MaritalStatusId = 11, LanguageId = 21, PriorityId = 17, InvoiceMethodId = 14,
                ClassificationId = 27, AddressId = 3, TarifId = 3, DirectProviderId = 3, CreateId = "admin", CreateDate = DateTime.UtcNow
            },
            new ClientDetails 
            { 
                Id = 4, MandantId = 1, Nummer = 1004, TitelId = 2, FirstName = "Helga", LastName = "Schulz", Sex = "F", 
                BirthDay = new DateTime(1938, 7, 22), StatusId = 7, StartContractDate = new DateTime(2024, 4, 1),
                PolicyNumber = "POL-004", MaritalStatusId = 13, LanguageId = 21, PriorityId = 20, InvoiceMethodId = 15,
                ClassificationId = 28, AddressId = 4, TarifId = 3, DirectProviderId = 1, CreateId = "admin", CreateDate = DateTime.UtcNow
            },
            new ClientDetails 
            { 
                Id = 5, MandantId = 1, Nummer = 1005, TitelId = 1, FirstName = "Werner", LastName = "Fischer", Sex = "M", 
                BirthDay = new DateTime(1952, 11, 5), StatusId = 7, StartContractDate = new DateTime(2024, 5, 10),
                PolicyNumber = "POL-005", MaritalStatusId = 11, LanguageId = 22, PriorityId = 18, InvoiceMethodId = 14,
                ClassificationId = 27, AddressId = 5, TarifId = 2, DirectProviderId = 1, CreateId = "admin", CreateDate = DateTime.UtcNow
            },
            new ClientDetails 
            { 
                Id = 6, MandantId = 1, Nummer = 1006, TitelId = 2, FirstName = "Ingrid", LastName = "Wagner", Sex = "F", 
                BirthDay = new DateTime(1948, 2, 28), StatusId = 7, StartContractDate = new DateTime(2024, 6, 1),
                PolicyNumber = "POL-006", MaritalStatusId = 12, LanguageId = 21, PriorityId = 19, InvoiceMethodId = 16,
                ClassificationId = 29, AddressId = 6, TarifId = 1, DirectProviderId = 2, CreateId = "admin", CreateDate = DateTime.UtcNow
            },
            new ClientDetails 
            { 
                Id = 7, MandantId = 1, Nummer = 1007, TitelId = 1, FirstName = "Klaus", LastName = "Becker", Sex = "M", 
                BirthDay = new DateTime(1940, 9, 15), StatusId = 8, StartContractDate = new DateTime(2023, 1, 1),
                PolicyNumber = "POL-007", MaritalStatusId = 13, LanguageId = 21, PriorityId = 17, InvoiceMethodId = 14,
                ClassificationId = 27, AddressId = 7, TarifId = 1, DirectProviderId = 3, CreateId = "admin", CreateDate = DateTime.UtcNow
            },
            new ClientDetails 
            { 
                Id = 8, MandantId = 1, Nummer = 1008, TitelId = 2, FirstName = "Gerda", LastName = "Hoffmann", Sex = "F", 
                BirthDay = new DateTime(1935, 12, 1), StatusId = 7, StartContractDate = new DateTime(2023, 6, 15),
                PolicyNumber = "POL-008", MaritalStatusId = 13, LanguageId = 21, PriorityId = 20, InvoiceMethodId = 15,
                ClassificationId = 28, AddressId = 8, TarifId = 3, DirectProviderId = 2, CreateId = "admin", CreateDate = DateTime.UtcNow
            },
            new ClientDetails 
            { 
                Id = 9, MandantId = 1, Nummer = 1009, TitelId = 1, FirstName = "Friedrich", LastName = "Zimmermann", Sex = "M", 
                BirthDay = new DateTime(1958, 4, 20), StatusId = 7, StartContractDate = new DateTime(2024, 7, 1),
                PolicyNumber = "POL-009", MaritalStatusId = 11, LanguageId = 23, PriorityId = 18, InvoiceMethodId = 14,
                ClassificationId = 27, AddressId = 1, TarifId = 2, DirectProviderId = 1, CreateId = "admin", CreateDate = DateTime.UtcNow
            },
            new ClientDetails 
            { 
                Id = 10, MandantId = 1, Nummer = 1010, TitelId = 2, FirstName = "Ursula", LastName = "Braun", Sex = "F", 
                BirthDay = new DateTime(1942, 6, 8), StatusId = 9, StartContractDate = new DateTime(2023, 9, 1),
                PolicyNumber = "POL-010", MaritalStatusId = 12, LanguageId = 21, PriorityId = 17, InvoiceMethodId = 16,
                ClassificationId = 29, AddressId = 2, TarifId = 1, DirectProviderId = 2, CreateId = "admin", CreateDate = DateTime.UtcNow
            }
        );

        // Seed additional devices
        modelBuilder.Entity<DeviceDetails>().HasData(
            new DeviceDetails
            {
                Id = 3, MandantId = 1, DeviceType = "Rollator", SerialNumber = "RO-003-2024",
                Description = "Leichtgewicht-Rollator", Status = "In Benutzung", Manufacturer = "Topro",
                Model = "Troja", PurchaseDate = new DateTime(2024, 3, 1), AssignedClientId = 3,
                CreateId = "admin", CreateDate = DateTime.UtcNow
            },
            new DeviceDetails
            {
                Id = 4, MandantId = 1, DeviceType = "Pflegebett", SerialNumber = "PB-004-2024",
                Description = "Niedrigbett mit Aufstehhilfe", Status = "In Benutzung", Manufacturer = "Völker",
                Model = "S962", PurchaseDate = new DateTime(2024, 4, 15), AssignedClientId = 4,
                CreateId = "admin", CreateDate = DateTime.UtcNow
            },
            new DeviceDetails
            {
                Id = 5, MandantId = 1, DeviceType = "Sauerstoffgerät", SerialNumber = "O2-005-2024",
                Description = "Mobiler Sauerstoffkonzentrator", Status = "In Benutzung", Manufacturer = "Philips",
                Model = "SimplyGo", PurchaseDate = new DateTime(2024, 5, 1), AssignedClientId = 5,
                CreateId = "admin", CreateDate = DateTime.UtcNow
            },
            new DeviceDetails
            {
                Id = 6, MandantId = 1, DeviceType = "Rollstuhl", SerialNumber = "RS-006-2024",
                Description = "Faltbarer Standardrollstuhl", Status = "Verfügbar", Manufacturer = "Bischoff & Bischoff",
                Model = "S-Eco 2", PurchaseDate = new DateTime(2024, 1, 20), AssignedClientId = null,
                CreateId = "admin", CreateDate = DateTime.UtcNow
            },
            new DeviceDetails
            {
                Id = 7, MandantId = 1, DeviceType = "Badewannenlift", SerialNumber = "BL-007-2024",
                Description = "Elektrischer Badewannenlift", Status = "In Benutzung", Manufacturer = "Aquatec",
                Model = "Orca", PurchaseDate = new DateTime(2024, 6, 1), AssignedClientId = 6,
                CreateId = "admin", CreateDate = DateTime.UtcNow
            },
            new DeviceDetails
            {
                Id = 8, MandantId = 1, DeviceType = "Patientenlifter", SerialNumber = "PL-008-2024",
                Description = "Mobiler Patientenlifter", Status = "In Benutzung", Manufacturer = "Arjo",
                Model = "Maxi Move", PurchaseDate = new DateTime(2023, 11, 15), AssignedClientId = 8,
                CreateId = "admin", CreateDate = DateTime.UtcNow
            },
            new DeviceDetails
            {
                Id = 9, MandantId = 1, DeviceType = "Rollator", SerialNumber = "RO-009-2023",
                Description = "Indoor-Rollator", Status = "Defekt", Manufacturer = "Russka",
                Model = "Vital", PurchaseDate = new DateTime(2023, 5, 1), AssignedClientId = null,
                CreateId = "admin", CreateDate = DateTime.UtcNow
            },
            new DeviceDetails
            {
                Id = 10, MandantId = 1, DeviceType = "Pflegebett", SerialNumber = "PB-010-2024",
                Description = "Pflegebett mit Seitengitter", Status = "Verfügbar", Manufacturer = "Stiegelmeyer",
                Model = "Puro", PurchaseDate = new DateTime(2024, 7, 1), AssignedClientId = null,
                CreateId = "admin", CreateDate = DateTime.UtcNow
            }
        );

        // Seed Client Status entries for reports
        modelBuilder.Entity<ClientStatus>().HasData(
            new ClientStatus { Id = 1, MandantId = 1, Name = "Aktiv", Code = "ACTIVE", IsActive = true },
            new ClientStatus { Id = 2, MandantId = 1, Name = "Inaktiv", Code = "INACTIVE", IsActive = true },
            new ClientStatus { Id = 3, MandantId = 1, Name = "Pausiert", Code = "PAUSED", IsActive = true },
            new ClientStatus { Id = 4, MandantId = 1, Name = "Gekündigt", Code = "CANCELLED", IsActive = true }
        );

        // Seed Priority entries for reports
        modelBuilder.Entity<SystemEntry>().HasData(
            new SystemEntry { Id = 38, MandantId = 1, Type = "PRIO", Description = "Niedrig", Code = "LOW", SortOrder = 1, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 39, MandantId = 1, Type = "PRIO", Description = "Normal", Code = "NORMAL", SortOrder = 2, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 40, MandantId = 1, Type = "PRIO", Description = "Hoch", Code = "HIGH", SortOrder = 3, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 41, MandantId = 1, Type = "PRIO", Description = "Dringend", Code = "URGENT", SortOrder = 4, IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed Languages for reports
        modelBuilder.Entity<SystemEntry>().HasData(
            new SystemEntry { Id = 42, MandantId = 1, Type = "LANG", Description = "Türkisch", Code = "TR", SortOrder = 4, IsActive = true, CreateDate = DateTime.UtcNow },
            new SystemEntry { Id = 43, MandantId = 1, Type = "LANG", Description = "Russisch", Code = "RU", SortOrder = 5, IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed Marketing Campaigns
        modelBuilder.Entity<Campaign>().HasData(
            new Campaign { Id = 1, MandantId = 1, Name = "Frühjahrs-Pflegekampagne 2024", Type = "Email", Status = "Completed", Description = "E-Mail-Kampagne für Pflegedienste", StartDate = new DateTime(2024, 3, 1), EndDate = new DateTime(2024, 4, 30), Budget = 5000, ActualCost = 4200, TargetLeads = 100, ActualLeads = 87, ConvertedLeads = 23, TargetAudience = "Senioren 65+", Channel = "Email", CreateDate = DateTime.UtcNow },
            new Campaign { Id = 2, MandantId = 1, Name = "Social Media Awareness", Type = "Social", Status = "Active", Description = "Facebook & Instagram Kampagne", StartDate = new DateTime(2024, 6, 1), EndDate = new DateTime(2024, 12, 31), Budget = 12000, ActualCost = 6500, TargetLeads = 200, ActualLeads = 134, ConvertedLeads = 31, TargetAudience = "Angehörige", Channel = "Social Media", CreateDate = DateTime.UtcNow },
            new Campaign { Id = 3, MandantId = 1, Name = "Messe Altenpflege 2024", Type = "Event", Status = "Completed", Description = "Messestand auf der Altenpflege-Messe", StartDate = new DateTime(2024, 4, 23), EndDate = new DateTime(2024, 4, 25), Budget = 15000, ActualCost = 14200, TargetLeads = 50, ActualLeads = 67, ConvertedLeads = 18, TargetAudience = "Fachpublikum", Channel = "Event", CreateDate = DateTime.UtcNow },
            new Campaign { Id = 4, MandantId = 1, Name = "Empfehlungsprogramm", Type = "Referral", Status = "Active", Description = "Kunden-werben-Kunden Programm", StartDate = new DateTime(2024, 1, 1), EndDate = new DateTime(2024, 12, 31), Budget = 8000, ActualCost = 3200, TargetLeads = 80, ActualLeads = 45, ConvertedLeads = 28, TargetAudience = "Bestandskunden", Channel = "Referral", CreateDate = DateTime.UtcNow },
            new Campaign { Id = 5, MandantId = 1, Name = "Google Ads Herbst", Type = "Digital", Status = "Active", Description = "Google Ads für Pflegeberatung", StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2024, 11, 30), Budget = 6000, ActualCost = 2100, TargetLeads = 120, ActualLeads = 56, ConvertedLeads = 12, TargetAudience = "Pflegebedürftige", Channel = "Google", CreateDate = DateTime.UtcNow }
        );

        // Seed Leads
        modelBuilder.Entity<Lead>().HasData(
            new Lead { Id = 1, MandantId = 1, FirstName = "Thomas", LastName = "Becker", Email = "t.becker@email.de", Phone = "+49 171 1234567", Company = null, Source = "Website", Status = "Qualified", QualificationScore = "Hot", Score = 85, EstimatedValue = 2400, FirstContactDate = new DateTime(2024, 7, 15), LastContactDate = new DateTime(2024, 8, 1), CampaignId = 2, AssignedToName = "Maria Schmidt", CreateDate = DateTime.UtcNow },
            new Lead { Id = 2, MandantId = 1, FirstName = "Sabine", LastName = "Koch", Email = "s.koch@gmx.de", Phone = "+49 172 2345678", Company = "Pflegeheim Sonnenschein", Source = "Referral", Status = "Proposal", QualificationScore = "Hot", Score = 92, EstimatedValue = 8500, FirstContactDate = new DateTime(2024, 6, 20), LastContactDate = new DateTime(2024, 7, 28), CampaignId = 4, AssignedToName = "Peter Müller", CreateDate = DateTime.UtcNow },
            new Lead { Id = 3, MandantId = 1, FirstName = "Michael", LastName = "Wagner", Email = "m.wagner@web.de", Phone = "+49 173 3456789", Company = null, Source = "Campaign", Status = "Contacted", QualificationScore = "Warm", Score = 65, EstimatedValue = 1800, FirstContactDate = new DateTime(2024, 8, 5), LastContactDate = new DateTime(2024, 8, 10), CampaignId = 1, AssignedToName = "Maria Schmidt", CreateDate = DateTime.UtcNow },
            new Lead { Id = 4, MandantId = 1, FirstName = "Claudia", LastName = "Hoffmann", Email = "c.hoffmann@outlook.de", Phone = "+49 174 4567890", Company = null, Source = "Website", Status = "New", QualificationScore = "Cold", Score = 35, EstimatedValue = 1200, FirstContactDate = new DateTime(2024, 8, 12), LastContactDate = null, CampaignId = 5, AssignedToName = "Anna Weber", CreateDate = DateTime.UtcNow },
            new Lead { Id = 5, MandantId = 1, FirstName = "Stefan", LastName = "Richter", Email = "s.richter@t-online.de", Phone = "+49 175 5678901", Company = "Seniorenresidenz Am Park", Source = "Event", Status = "Won", QualificationScore = "Hot", Score = 98, EstimatedValue = 12000, FirstContactDate = new DateTime(2024, 4, 23), LastContactDate = new DateTime(2024, 5, 15), ConvertedClientId = 5, ConversionDate = new DateTime(2024, 5, 20), CampaignId = 3, AssignedToName = "Peter Müller", CreateDate = DateTime.UtcNow },
            new Lead { Id = 6, MandantId = 1, FirstName = "Petra", LastName = "Schulz", Email = "p.schulz@email.de", Phone = "+49 176 6789012", Company = null, Source = "Website", Status = "Negotiation", QualificationScore = "Hot", Score = 88, EstimatedValue = 3600, FirstContactDate = new DateTime(2024, 7, 1), LastContactDate = new DateTime(2024, 8, 8), CampaignId = 2, AssignedToName = "Maria Schmidt", CreateDate = DateTime.UtcNow },
            new Lead { Id = 7, MandantId = 1, FirstName = "Andreas", LastName = "Klein", Email = "a.klein@gmail.com", Phone = "+49 177 7890123", Company = null, Source = "Referral", Status = "Lost", QualificationScore = "Warm", Score = 55, EstimatedValue = 2000, FirstContactDate = new DateTime(2024, 5, 10), LastContactDate = new DateTime(2024, 6, 15), CampaignId = 4, AssignedToName = "Anna Weber", CreateDate = DateTime.UtcNow },
            new Lead { Id = 8, MandantId = 1, FirstName = "Monika", LastName = "Wolf", Email = "m.wolf@yahoo.de", Phone = "+49 178 8901234", Company = "Ambulanter Pflegedienst Herz", Source = "Cold Call", Status = "Qualified", QualificationScore = "Warm", Score = 72, EstimatedValue = 5500, FirstContactDate = new DateTime(2024, 7, 20), LastContactDate = new DateTime(2024, 8, 5), CampaignId = null, AssignedToName = "Peter Müller", CreateDate = DateTime.UtcNow }
        );

        // Seed Lead Activities
        modelBuilder.Entity<LeadActivity>().HasData(
            new LeadActivity { Id = 1, LeadId = 1, MandantId = 1, Type = "Call", Description = "Erstgespräch - Interesse an Pflegebett", ActivityDate = new DateTime(2024, 7, 16), Outcome = "Positive", Duration = 25, PerformedBy = "Maria Schmidt" },
            new LeadActivity { Id = 2, LeadId = 1, MandantId = 1, Type = "Email", Description = "Produktinformationen gesendet", ActivityDate = new DateTime(2024, 7, 18), Outcome = "Neutral", Duration = 10, PerformedBy = "Maria Schmidt" },
            new LeadActivity { Id = 3, LeadId = 2, MandantId = 1, Type = "Meeting", Description = "Vor-Ort-Termin im Pflegeheim", ActivityDate = new DateTime(2024, 7, 5), Outcome = "Positive", Duration = 90, PerformedBy = "Peter Müller" },
            new LeadActivity { Id = 4, LeadId = 2, MandantId = 1, Type = "Proposal", Description = "Angebot für 10 Pflegebetten erstellt", ActivityDate = new DateTime(2024, 7, 15), Outcome = "Positive", Duration = 60, PerformedBy = "Peter Müller" },
            new LeadActivity { Id = 5, LeadId = 5, MandantId = 1, Type = "Demo", Description = "Produktvorführung auf der Messe", ActivityDate = new DateTime(2024, 4, 24), Outcome = "Positive", Duration = 45, PerformedBy = "Peter Müller" },
            new LeadActivity { Id = 6, LeadId = 6, MandantId = 1, Type = "Call", Description = "Nachfassgespräch - Preisverhandlung", ActivityDate = new DateTime(2024, 8, 8), Outcome = "Positive", Duration = 30, PerformedBy = "Maria Schmidt" }
        );

        // Seed Sales Opportunities
        modelBuilder.Entity<SalesOpportunity>().HasData(
            new SalesOpportunity { Id = 1, MandantId = 1, Name = "Pflegeheim Sonnenschein - Betten", LeadId = 2, Stage = "Proposal", Amount = 8500, Probability = 75, ExpectedCloseDate = new DateTime(2024, 9, 15), ProductInterest = "Pflegebetten", AssignedToName = "Peter Müller", CreateDate = DateTime.UtcNow },
            new SalesOpportunity { Id = 2, MandantId = 1, Name = "Becker - Pflegebett", LeadId = 1, Stage = "Negotiation", Amount = 2400, Probability = 80, ExpectedCloseDate = new DateTime(2024, 8, 30), ProductInterest = "Pflegebett Einzeln", AssignedToName = "Maria Schmidt", CreateDate = DateTime.UtcNow },
            new SalesOpportunity { Id = 3, MandantId = 1, Name = "Schulz - Komplett-Ausstattung", LeadId = 6, Stage = "Negotiation", Amount = 3600, Probability = 70, ExpectedCloseDate = new DateTime(2024, 9, 1), ProductInterest = "Pflegebett + Rollator", AssignedToName = "Maria Schmidt", CreateDate = DateTime.UtcNow },
            new SalesOpportunity { Id = 4, MandantId = 1, Name = "Ambulanter Dienst Herz - Geräte", LeadId = 8, Stage = "Qualification", Amount = 5500, Probability = 40, ExpectedCloseDate = new DateTime(2024, 10, 15), ProductInterest = "Diverse Geräte", AssignedToName = "Peter Müller", CreateDate = DateTime.UtcNow },
            new SalesOpportunity { Id = 5, MandantId = 1, Name = "Seniorenresidenz - Abschluss", LeadId = 5, Stage = "Closed Won", Amount = 12000, Probability = 100, ExpectedCloseDate = new DateTime(2024, 5, 20), ActualCloseDate = new DateTime(2024, 5, 20), ProductInterest = "Großauftrag", AssignedToName = "Peter Müller", CreateDate = DateTime.UtcNow }
        );

        // Seed Articles
        modelBuilder.Entity<Article>().HasData(
            new Article { Id = 1, MandantId = 1, ArticleNumber = "PB-001", Name = "Pflegebett Standard", Description = "Elektrisch verstellbares Pflegebett", Category = "Device", SubCategory = "Betten", Unit = "Stück", PurchasePrice = 800, SalesPrice = 1200, VatRate = 19, MinStock = 5, MaxStock = 20, ReorderPoint = 8, Supplier = "Stiegelmeyer", IsActive = true, CreateDate = DateTime.UtcNow },
            new Article { Id = 2, MandantId = 1, ArticleNumber = "PB-002", Name = "Pflegebett Premium", Description = "Premium Pflegebett mit Seitengitter", Category = "Device", SubCategory = "Betten", Unit = "Stück", PurchasePrice = 1200, SalesPrice = 1800, VatRate = 19, MinStock = 3, MaxStock = 15, ReorderPoint = 5, Supplier = "Stiegelmeyer", IsActive = true, CreateDate = DateTime.UtcNow },
            new Article { Id = 3, MandantId = 1, ArticleNumber = "RO-001", Name = "Rollator Standard", Description = "Leichtgewicht-Rollator", Category = "Device", SubCategory = "Mobilität", Unit = "Stück", PurchasePrice = 80, SalesPrice = 150, VatRate = 19, MinStock = 10, MaxStock = 50, ReorderPoint = 15, Supplier = "Russka", IsActive = true, CreateDate = DateTime.UtcNow },
            new Article { Id = 4, MandantId = 1, ArticleNumber = "RS-001", Name = "Rollstuhl Standard", Description = "Faltbarer Rollstuhl", Category = "Device", SubCategory = "Mobilität", Unit = "Stück", PurchasePrice = 200, SalesPrice = 350, VatRate = 19, MinStock = 5, MaxStock = 25, ReorderPoint = 8, Supplier = "Bischoff & Bischoff", IsActive = true, CreateDate = DateTime.UtcNow },
            new Article { Id = 5, MandantId = 1, ArticleNumber = "NA-001", Name = "Notrufarmband", Description = "GPS-Notrufarmband mit Sturzerkennung", Category = "Device", SubCategory = "Sicherheit", Unit = "Stück", PurchasePrice = 150, SalesPrice = 280, VatRate = 19, MinStock = 20, MaxStock = 100, ReorderPoint = 30, Supplier = "Tunstall", IsActive = true, CreateDate = DateTime.UtcNow },
            new Article { Id = 6, MandantId = 1, ArticleNumber = "MED-001", Name = "Ibuprofen 400mg", Description = "Schmerzmittel 50 Tabletten", Category = "Medication", SubCategory = "Schmerzmittel", Unit = "Packung", PurchasePrice = 3.50m, SalesPrice = 6.99m, VatRate = 19, MinStock = 50, MaxStock = 200, ReorderPoint = 80, Supplier = "Ratiopharm", RequiresPrescription = false, IsActive = true, CreateDate = DateTime.UtcNow },
            new Article { Id = 7, MandantId = 1, ArticleNumber = "MED-002", Name = "Metformin 500mg", Description = "Diabetes-Medikament 100 Tabletten", Category = "Medication", SubCategory = "Diabetes", Unit = "Packung", PurchasePrice = 8.00m, SalesPrice = 15.99m, VatRate = 19, MinStock = 30, MaxStock = 100, ReorderPoint = 50, Supplier = "Hexal", RequiresPrescription = true, IsActive = true, CreateDate = DateTime.UtcNow },
            new Article { Id = 8, MandantId = 1, ArticleNumber = "SUP-001", Name = "Einmalhandschuhe L", Description = "Nitril-Handschuhe Größe L, 100 Stück", Category = "Supply", SubCategory = "Hygiene", Unit = "Box", PurchasePrice = 8.00m, SalesPrice = 14.99m, VatRate = 19, MinStock = 100, MaxStock = 500, ReorderPoint = 150, Supplier = "Hartmann", IsActive = true, CreateDate = DateTime.UtcNow },
            new Article { Id = 9, MandantId = 1, ArticleNumber = "SUP-002", Name = "Desinfektionsmittel 1L", Description = "Händedesinfektionsmittel", Category = "Supply", SubCategory = "Hygiene", Unit = "Flasche", PurchasePrice = 5.00m, SalesPrice = 9.99m, VatRate = 19, MinStock = 50, MaxStock = 200, ReorderPoint = 80, Supplier = "Schülke", IsActive = true, CreateDate = DateTime.UtcNow },
            new Article { Id = 10, MandantId = 1, ArticleNumber = "SRV-001", Name = "Pflegeberatung Stunde", Description = "Professionelle Pflegeberatung", Category = "Service", SubCategory = "Beratung", Unit = "Stunde", PurchasePrice = 0, SalesPrice = 85, VatRate = 19, MinStock = 0, MaxStock = 0, ReorderPoint = 0, Supplier = null, IsActive = true, CreateDate = DateTime.UtcNow }
        );

        // Seed Inventory
        modelBuilder.Entity<Inventory>().HasData(
            new Inventory { Id = 1, MandantId = 1, ArticleId = 1, WarehouseLocation = "Lager A-01", CurrentStock = 12, ReservedStock = 2, AvailableStock = 10, LastStockTake = new DateTime(2024, 7, 1), LastMovementDate = new DateTime(2024, 8, 5) },
            new Inventory { Id = 2, MandantId = 1, ArticleId = 2, WarehouseLocation = "Lager A-02", CurrentStock = 8, ReservedStock = 1, AvailableStock = 7, LastStockTake = new DateTime(2024, 7, 1), LastMovementDate = new DateTime(2024, 8, 3) },
            new Inventory { Id = 3, MandantId = 1, ArticleId = 3, WarehouseLocation = "Lager B-01", CurrentStock = 35, ReservedStock = 5, AvailableStock = 30, LastStockTake = new DateTime(2024, 7, 1), LastMovementDate = new DateTime(2024, 8, 10) },
            new Inventory { Id = 4, MandantId = 1, ArticleId = 4, WarehouseLocation = "Lager B-02", CurrentStock = 18, ReservedStock = 3, AvailableStock = 15, LastStockTake = new DateTime(2024, 7, 1), LastMovementDate = new DateTime(2024, 8, 8) },
            new Inventory { Id = 5, MandantId = 1, ArticleId = 5, WarehouseLocation = "Lager C-01", CurrentStock = 45, ReservedStock = 10, AvailableStock = 35, LastStockTake = new DateTime(2024, 7, 1), LastMovementDate = new DateTime(2024, 8, 12) },
            new Inventory { Id = 6, MandantId = 1, ArticleId = 6, WarehouseLocation = "Lager D-01", CurrentStock = 120, ReservedStock = 0, AvailableStock = 120, BatchNumber = "LOT-2024-001", ExpiryDate = new DateTime(2026, 3, 15), LastStockTake = new DateTime(2024, 7, 1), LastMovementDate = new DateTime(2024, 8, 1) },
            new Inventory { Id = 7, MandantId = 1, ArticleId = 7, WarehouseLocation = "Lager D-02", CurrentStock = 65, ReservedStock = 0, AvailableStock = 65, BatchNumber = "LOT-2024-002", ExpiryDate = new DateTime(2025, 12, 31), LastStockTake = new DateTime(2024, 7, 1), LastMovementDate = new DateTime(2024, 7, 25) },
            new Inventory { Id = 8, MandantId = 1, ArticleId = 8, WarehouseLocation = "Lager E-01", CurrentStock = 250, ReservedStock = 20, AvailableStock = 230, LastStockTake = new DateTime(2024, 7, 1), LastMovementDate = new DateTime(2024, 8, 11) },
            new Inventory { Id = 9, MandantId = 1, ArticleId = 9, WarehouseLocation = "Lager E-02", CurrentStock = 95, ReservedStock = 5, AvailableStock = 90, LastStockTake = new DateTime(2024, 7, 1), LastMovementDate = new DateTime(2024, 8, 9) }
        );

        // Seed Invoices
        modelBuilder.Entity<Invoice>().HasData(
            new Invoice { Id = 1, MandantId = 1, InvoiceNumber = "INV-2024-001", ClientId = 1, Status = "Paid", InvoiceDate = new DateTime(2024, 1, 15), DueDate = new DateTime(2024, 2, 15), PaidDate = new DateTime(2024, 2, 10), SubTotal = 1200, Tax = 228, TotalAmount = 1428, PaidAmount = 1428, OutstandingAmount = 0, PaymentMethod = "Bank Transfer", CreateDate = DateTime.UtcNow },
            new Invoice { Id = 2, MandantId = 1, InvoiceNumber = "INV-2024-002", ClientId = 2, Status = "Paid", InvoiceDate = new DateTime(2024, 2, 1), DueDate = new DateTime(2024, 3, 1), PaidDate = new DateTime(2024, 2, 28), SubTotal = 2400, Tax = 456, TotalAmount = 2856, PaidAmount = 2856, OutstandingAmount = 0, PaymentMethod = "Direct Debit", CreateDate = DateTime.UtcNow },
            new Invoice { Id = 3, MandantId = 1, InvoiceNumber = "INV-2024-003", ClientId = 3, Status = "Overdue", InvoiceDate = new DateTime(2024, 5, 15), DueDate = new DateTime(2024, 6, 15), SubTotal = 850, Tax = 161.50m, TotalAmount = 1011.50m, PaidAmount = 0, OutstandingAmount = 1011.50m, ReminderCount = 2, LastReminderDate = new DateTime(2024, 7, 15), CreateDate = DateTime.UtcNow },
            new Invoice { Id = 4, MandantId = 1, InvoiceNumber = "INV-2024-004", ClientId = 4, Status = "Sent", InvoiceDate = new DateTime(2024, 7, 1), DueDate = new DateTime(2024, 8, 1), SubTotal = 1800, Tax = 342, TotalAmount = 2142, PaidAmount = 0, OutstandingAmount = 2142, CreateDate = DateTime.UtcNow },
            new Invoice { Id = 5, MandantId = 1, InvoiceNumber = "INV-2024-005", ClientId = 5, Status = "Paid", InvoiceDate = new DateTime(2024, 6, 1), DueDate = new DateTime(2024, 7, 1), PaidDate = new DateTime(2024, 6, 25), SubTotal = 3500, Tax = 665, TotalAmount = 4165, PaidAmount = 4165, OutstandingAmount = 0, PaymentMethod = "Bank Transfer", CreateDate = DateTime.UtcNow },
            new Invoice { Id = 6, MandantId = 1, InvoiceNumber = "INV-2024-006", ClientId = 6, Status = "Paid", InvoiceDate = new DateTime(2024, 7, 15), DueDate = new DateTime(2024, 8, 15), PaidDate = new DateTime(2024, 8, 1), SubTotal = 950, Tax = 180.50m, TotalAmount = 1130.50m, PaidAmount = 1130.50m, OutstandingAmount = 0, PaymentMethod = "Credit Card", CreateDate = DateTime.UtcNow },
            new Invoice { Id = 7, MandantId = 1, InvoiceNumber = "INV-2024-007", ClientId = 7, Status = "Overdue", InvoiceDate = new DateTime(2024, 4, 1), DueDate = new DateTime(2024, 5, 1), SubTotal = 1500, Tax = 285, TotalAmount = 1785, PaidAmount = 500, OutstandingAmount = 1285, ReminderCount = 3, LastReminderDate = new DateTime(2024, 8, 1), CreateDate = DateTime.UtcNow },
            new Invoice { Id = 8, MandantId = 1, InvoiceNumber = "INV-2024-008", ClientId = 8, Status = "Draft", InvoiceDate = new DateTime(2024, 8, 10), DueDate = new DateTime(2024, 9, 10), SubTotal = 2200, Tax = 418, TotalAmount = 2618, PaidAmount = 0, OutstandingAmount = 2618, CreateDate = DateTime.UtcNow }
        );

        // Seed Payments
        modelBuilder.Entity<Payment>().HasData(
            new Payment { Id = 1, MandantId = 1, InvoiceId = 1, ClientId = 1, PaymentNumber = "PAY-2024-001", Amount = 1428, PaymentDate = new DateTime(2024, 2, 10), PaymentMethod = "Bank Transfer", Reference = "Überweisung", Status = "Completed", CreateDate = DateTime.UtcNow },
            new Payment { Id = 2, MandantId = 1, InvoiceId = 2, ClientId = 2, PaymentNumber = "PAY-2024-002", Amount = 2856, PaymentDate = new DateTime(2024, 2, 28), PaymentMethod = "Direct Debit", Reference = "Lastschrift", Status = "Completed", CreateDate = DateTime.UtcNow },
            new Payment { Id = 3, MandantId = 1, InvoiceId = 5, ClientId = 5, PaymentNumber = "PAY-2024-003", Amount = 4165, PaymentDate = new DateTime(2024, 6, 25), PaymentMethod = "Bank Transfer", Reference = "Überweisung", Status = "Completed", CreateDate = DateTime.UtcNow },
            new Payment { Id = 4, MandantId = 1, InvoiceId = 6, ClientId = 6, PaymentNumber = "PAY-2024-004", Amount = 1130.50m, PaymentDate = new DateTime(2024, 8, 1), PaymentMethod = "Credit Card", Reference = "Kreditkarte", Status = "Completed", CreateDate = DateTime.UtcNow },
            new Payment { Id = 5, MandantId = 1, InvoiceId = 7, ClientId = 7, PaymentNumber = "PAY-2024-005", Amount = 500, PaymentDate = new DateTime(2024, 5, 15), PaymentMethod = "Bank Transfer", Reference = "Teilzahlung", Status = "Completed", CreateDate = DateTime.UtcNow }
        );

        // Seed Cost Centers
        modelBuilder.Entity<CostCenter>().HasData(
            new CostCenter { Id = 1, MandantId = 1, Code = "CC-100", Name = "Vertrieb", Description = "Vertriebsabteilung", Department = "Sales", Budget = 50000, ActualSpend = 32000, IsActive = true },
            new CostCenter { Id = 2, MandantId = 1, Code = "CC-200", Name = "Marketing", Description = "Marketingabteilung", Department = "Marketing", Budget = 40000, ActualSpend = 28500, IsActive = true },
            new CostCenter { Id = 3, MandantId = 1, Code = "CC-300", Name = "Lager & Logistik", Description = "Lagerverwaltung", Department = "Operations", Budget = 25000, ActualSpend = 18000, IsActive = true },
            new CostCenter { Id = 4, MandantId = 1, Code = "CC-400", Name = "Verwaltung", Description = "Allgemeine Verwaltung", Department = "Admin", Budget = 35000, ActualSpend = 29000, IsActive = true },
            new CostCenter { Id = 5, MandantId = 1, Code = "CC-500", Name = "IT", Description = "IT-Abteilung", Department = "IT", Budget = 30000, ActualSpend = 22000, IsActive = true }
        );

        // Seed Medications
        modelBuilder.Entity<Medication>().HasData(
            new Medication { Id = 1, MandantId = 1, Name = "Ibuprofen 400mg", GenericName = "Ibuprofen", Manufacturer = "Ratiopharm", DosageForm = "Tablet", Strength = "400mg", PackageSize = "50 Stück", Price = 6.99m, RequiresPrescription = false, StorageConditions = "Raumtemperatur", ArticleId = 6, IsActive = true },
            new Medication { Id = 2, MandantId = 1, Name = "Metformin 500mg", GenericName = "Metformin", Manufacturer = "Hexal", DosageForm = "Tablet", Strength = "500mg", PackageSize = "100 Stück", Price = 15.99m, RequiresPrescription = true, StorageConditions = "Raumtemperatur", ArticleId = 7, IsActive = true },
            new Medication { Id = 3, MandantId = 1, Name = "Omeprazol 20mg", GenericName = "Omeprazol", Manufacturer = "Stada", DosageForm = "Capsule", Strength = "20mg", PackageSize = "30 Stück", Price = 12.50m, RequiresPrescription = true, StorageConditions = "Raumtemperatur", IsActive = true },
            new Medication { Id = 4, MandantId = 1, Name = "Ramipril 5mg", GenericName = "Ramipril", Manufacturer = "1A Pharma", DosageForm = "Tablet", Strength = "5mg", PackageSize = "100 Stück", Price = 18.99m, RequiresPrescription = true, StorageConditions = "Raumtemperatur", IsActive = true },
            new Medication { Id = 5, MandantId = 1, Name = "Insulin Lantus", GenericName = "Insulin glargin", Manufacturer = "Sanofi", DosageForm = "Injection", Strength = "100 E/ml", PackageSize = "5 Pens", Price = 89.99m, RequiresPrescription = true, StorageConditions = "Kühlschrank 2-8°C", IsActive = true }
        );
    }
}

