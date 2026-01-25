// =================================================================================================
// APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Infrastructure Layer)
// This class is part of the Infrastructure Layer in Clean Architecture. It handles data access
// and communication with the database using Entity Framework Core.
//
// META-DATA:
//   - Layer: Infrastructure (Data Access)
//   - Responsibility: Define the database context, configure entity mappings, and manage data transactions.
// =================================================================================================

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

    // =================================================================================================
    // APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Infrastructure Layer)
    // The DbSets represent the tables in the database. Each DbSet corresponds to an entity in the
    // Domain Layer.
    // =================================================================================================
    public DbSet<Address> Addresses { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<ClientDetails> ClientDetails { get; set; }
    public DbSet<ClientCost> ClientCosts { get; set; }
    public DbSet<ClientDisease> ClientDiseases { get; set; }
    public DbSet<ClientFeature> ClientFeatures { get; set; }
    public DbSet<ClientMedecin> ClientMedecins { get; set; }
    public DbSet<ClientMedication> ClientMedications { get; set; }
    public DbSet<ClientNotation> ClientNotations { get; set; }
    public DbSet<ClientPhone> ClientPhones { get; set; }
    public DbSet<ClientStatus> ClientStatuses { get; set; }
    public DbSet<ClientStatusHistory> ClientStatusHistories { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<DeviceDetails> DeviceDetails { get; set; }
    public DbSet<DirectProvider> DirectProviders { get; set; }
    public DbSet<DirectProviderDetails> DirectProviderDetails { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<ProfessionalProvider> ProfessionalProviders { get; set; }
    public DbSet<ProfessionalProviderDetails> ProfessionalProviderDetails { get; set; }
    public DbSet<ProfessionalProviderClientLink> ProfessionalProviderClientLinks { get; set; }
    public DbSet<SystemEntry> SystemEntries { get; set; }
    public DbSet<Tarif> Tarifs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<VatTax> VatTaxes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // =================================================================================================
        // APP FABRIC - STAGE 2: CORE APPLICATION TRANSFORMATION (Infrastructure Layer)
        // This method is used to configure the database model. It defines relationships, constraints,
        // and other database-specific settings.
        // =================================================================================================

        base.OnModelCreating(modelBuilder);

        // Configure relationships and constraints here
    }
}
