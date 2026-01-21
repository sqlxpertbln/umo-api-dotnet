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
    }
}
