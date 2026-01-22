using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UMOApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    ArticleNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    SubCategory = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "TEXT", nullable: true),
                    SalesPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                    VatRate = table.Column<decimal>(type: "TEXT", nullable: true),
                    MinStock = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxStock = table.Column<int>(type: "INTEGER", nullable: true),
                    ReorderPoint = table.Column<int>(type: "INTEGER", nullable: true),
                    Supplier = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SupplierArticleNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresPrescription = table.Column<bool>(type: "INTEGER", nullable: false),
                    StorageConditions = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Budget = table.Column<decimal>(type: "TEXT", nullable: true),
                    ActualCost = table.Column<decimal>(type: "TEXT", nullable: true),
                    TargetLeads = table.Column<int>(type: "INTEGER", nullable: true),
                    ActualLeads = table.Column<int>(type: "INTEGER", nullable: true),
                    ConvertedLeads = table.Column<int>(type: "INTEGER", nullable: true),
                    TargetAudience = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Channel = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nummer = table.Column<int>(type: "INTEGER", nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Sex = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    BirthDay = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Memo = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    StartContractDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PolicyNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    FreeFeld = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    BooleanOptions = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AllowCallRecording = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecordingConsentSigned = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecordingConsentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RecordingRetentionDays = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    BooleanOptions = table.Column<int>(type: "INTEGER", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CostCenters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Department = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Budget = table.Column<decimal>(type: "TEXT", nullable: true),
                    ActualSpend = table.Column<decimal>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCenters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsoCode = table.Column<string>(type: "TEXT", maxLength: 5, nullable: true),
                    PhoneCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    DeviceType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SerialNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Manufacturer = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    WarrantyEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "TEXT", nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AssignedClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectProviderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Street = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    HouseNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    ZipCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Mobile = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Fax = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Website = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectProviderDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dispatchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Extension = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    SipExtension = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastActivity = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CurrentCallCount = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalCallsHandled = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispatchers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfessionalProviderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Specialty = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Street = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    HouseNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    ZipCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Mobile = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Fax = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Website = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    LicenseNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Qualifications = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalProviderDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SipConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SipServer = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    WebSocketUrl = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SipUsername = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SipPassword = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SipDomain = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SipPort = table.Column<int>(type: "INTEGER", nullable: false),
                    UseTls = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    WebhookUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SipConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    BooleanOptions = table.Column<int>(type: "INTEGER", nullable: true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VatTaxes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Percentage = table.Column<decimal>(type: "TEXT", nullable: true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VatTaxes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    ArticleId = table.Column<int>(type: "INTEGER", nullable: false),
                    WarehouseLocation = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CurrentStock = table.Column<int>(type: "INTEGER", nullable: false),
                    ReservedStock = table.Column<int>(type: "INTEGER", nullable: false),
                    AvailableStock = table.Column<int>(type: "INTEGER", nullable: false),
                    BatchNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastStockTake = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastMovementDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventories_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Medications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    GenericName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Manufacturer = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DosageForm = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Strength = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    PackageSize = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Price = table.Column<decimal>(type: "TEXT", nullable: true),
                    RequiresPrescription = table.Column<bool>(type: "INTEGER", nullable: false),
                    StorageConditions = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Contraindications = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ArticleId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medications_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    ArticleId = table.Column<int>(type: "INTEGER", nullable: false),
                    MovementType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Reference = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FromLocation = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ToLocation = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MovementDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PerformedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockMovements_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Company = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    QualificationScore = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Score = table.Column<int>(type: "INTEGER", nullable: true),
                    EstimatedValue = table.Column<decimal>(type: "TEXT", nullable: true),
                    FirstContactDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastContactDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpectedCloseDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CampaignId = table.Column<int>(type: "INTEGER", nullable: true),
                    AssignedToUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    AssignedToName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ConvertedClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    ConversionDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leads_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmergencyContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Relationship = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    MobileNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAvailable24h = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    HasKey = table.Column<bool>(type: "INTEGER", nullable: false),
                    NotifyBySms = table.Column<bool>(type: "INTEGER", nullable: false),
                    NotifyByCall = table.Column<bool>(type: "INTEGER", nullable: false),
                    NotifyByEmail = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmergencyContacts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmergencyDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DeviceType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SerialNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    SipIdentifier = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    LastHeartbeat = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastAlertTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsOnline = table.Column<bool>(type: "INTEGER", nullable: false),
                    BatteryLevel = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmergencyDevices_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    CountryId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DispatcherShifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DispatcherId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShiftStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ShiftEnd = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ShiftType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatcherShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DispatcherShifts_Dispatchers_DispatcherId",
                        column: x => x.DispatcherId,
                        principalTable: "Dispatchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tarifs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    BasePrice = table.Column<decimal>(type: "TEXT", nullable: true),
                    VatTaxId = table.Column<int>(type: "INTEGER", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarifs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tarifs_VatTaxes_VatTaxId",
                        column: x => x.VatTaxId,
                        principalTable: "VatTaxes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LeadActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LeadId = table.Column<int>(type: "INTEGER", nullable: false),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ActivityDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Outcome = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Duration = table.Column<int>(type: "INTEGER", nullable: true),
                    PerformedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    NextFollowUp = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadActivities_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesOpportunities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LeadId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    Stage = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: true),
                    Probability = table.Column<int>(type: "INTEGER", nullable: true),
                    ExpectedCloseDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ActualCloseDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ProductInterest = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CompetitorInfo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AssignedToName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOpportunities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOpportunities_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmergencyAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlertType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    EmergencyDeviceId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    CallerNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    AlertTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AcknowledgedTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ResolvedTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AcknowledgedByDispatcherId = table.Column<int>(type: "INTEGER", nullable: true),
                    ResolvedByDispatcherId = table.Column<int>(type: "INTEGER", nullable: true),
                    Resolution = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: true),
                    Longitude = table.Column<double>(type: "REAL", nullable: true),
                    HeartRate = table.Column<int>(type: "INTEGER", nullable: true),
                    AmbulanceCalled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ContactsNotified = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmergencyChainStep = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FamilyNotifiedTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FamilyContactsNotified = table.Column<int>(type: "INTEGER", nullable: false),
                    DoctorNotifiedTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DoctorNotified = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    AmbulanceCalledTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AmbulanceIncidentNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    MedicationListProvided = table.Column<bool>(type: "INTEGER", nullable: false),
                    MedicationListProvidedTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ConferenceActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ConferenceParticipants = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    AmbulanceEta = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmergencyAlerts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmergencyAlerts_Dispatchers_AcknowledgedByDispatcherId",
                        column: x => x.AcknowledgedByDispatcherId,
                        principalTable: "Dispatchers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmergencyAlerts_Dispatchers_ResolvedByDispatcherId",
                        column: x => x.ResolvedByDispatcherId,
                        principalTable: "Dispatchers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmergencyAlerts_EmergencyDevices_EmergencyDeviceId",
                        column: x => x.EmergencyDeviceId,
                        principalTable: "EmergencyDevices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ZipCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    DistrictId = table.Column<int>(type: "INTEGER", nullable: true),
                    CountryId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cities_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CallLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SipgateCallId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Direction = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CallerNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CalleeNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DispatcherId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    EmergencyAlertId = table.Column<int>(type: "INTEGER", nullable: true),
                    EmergencyContactId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ConnectTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DurationSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    EndReason = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    IsRecorded = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecordingUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CallType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RecordingAllowed = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecordingStartedManually = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecordingFileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    RecordingDurationSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    Transcription = table.Column<string>(type: "TEXT", maxLength: 10000, nullable: false),
                    CallSummary = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CallCategory = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    WasEscalated = table.Column<bool>(type: "INTEGER", nullable: false),
                    EscalatedTo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RequiresFollowUp = table.Column<bool>(type: "INTEGER", nullable: false),
                    FollowUpDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FollowUpCompleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CallLogs_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CallLogs_Dispatchers_DispatcherId",
                        column: x => x.DispatcherId,
                        principalTable: "Dispatchers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CallLogs_EmergencyAlerts_EmergencyAlertId",
                        column: x => x.EmergencyAlertId,
                        principalTable: "EmergencyAlerts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CallLogs_EmergencyContacts_EmergencyContactId",
                        column: x => x.EmergencyContactId,
                        principalTable: "EmergencyContacts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmergencyChainActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmergencyAlertId = table.Column<int>(type: "INTEGER", nullable: false),
                    ActionType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ActionTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DispatcherId = table.Column<int>(type: "INTEGER", nullable: true),
                    Target = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TargetPhone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Result = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DurationSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    MedicationListProvided = table.Column<bool>(type: "INTEGER", nullable: false),
                    MedicationListContent = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    SipgateCallId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyChainActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmergencyChainActions_Dispatchers_DispatcherId",
                        column: x => x.DispatcherId,
                        principalTable: "Dispatchers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmergencyChainActions_EmergencyAlerts_EmergencyAlertId",
                        column: x => x.EmergencyAlertId,
                        principalTable: "EmergencyAlerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Street = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    HouseNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    ZipCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    CityId = table.Column<int>(type: "INTEGER", nullable: true),
                    DistrictId = table.Column<int>(type: "INTEGER", nullable: true),
                    CountryId = table.Column<int>(type: "INTEGER", nullable: true),
                    AdditionalInfo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Addresses_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Addresses_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nummer = table.Column<int>(type: "INTEGER", nullable: true),
                    TitelId = table.Column<int>(type: "INTEGER", nullable: true),
                    PrefixId = table.Column<int>(type: "INTEGER", nullable: true),
                    SocialSecurityNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Sex = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    BirthDay = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Memo = table.Column<byte[]>(type: "BLOB", nullable: true),
                    StatusId = table.Column<int>(type: "INTEGER", nullable: true),
                    StartContractDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndContractDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PolicyNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    FreeFeld = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    BooleanOptions = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MedecinPlace = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Bill = table.Column<int>(type: "INTEGER", nullable: true),
                    BankNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    BankName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PostBankNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    InsuresCare = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ClassificationId = table.Column<int>(type: "INTEGER", nullable: true),
                    InvoiceMethodId = table.Column<int>(type: "INTEGER", nullable: true),
                    ReasonId = table.Column<int>(type: "INTEGER", nullable: true),
                    PriorityId = table.Column<int>(type: "INTEGER", nullable: true),
                    MaritalStatusId = table.Column<int>(type: "INTEGER", nullable: true),
                    LanguageId = table.Column<int>(type: "INTEGER", nullable: true),
                    FinancialGroupId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AddressId = table.Column<int>(type: "INTEGER", nullable: true),
                    TarifId = table.Column<int>(type: "INTEGER", nullable: true),
                    DirectProviderId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientDetails_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientDetails_DirectProviderDetails_DirectProviderId",
                        column: x => x.DirectProviderId,
                        principalTable: "DirectProviderDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientDetails_SystemEntries_ClassificationId",
                        column: x => x.ClassificationId,
                        principalTable: "SystemEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientDetails_SystemEntries_FinancialGroupId",
                        column: x => x.FinancialGroupId,
                        principalTable: "SystemEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientDetails_SystemEntries_InvoiceMethodId",
                        column: x => x.InvoiceMethodId,
                        principalTable: "SystemEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientDetails_SystemEntries_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "SystemEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientDetails_SystemEntries_MaritalStatusId",
                        column: x => x.MaritalStatusId,
                        principalTable: "SystemEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientDetails_SystemEntries_PrefixId",
                        column: x => x.PrefixId,
                        principalTable: "SystemEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientDetails_SystemEntries_PriorityId",
                        column: x => x.PriorityId,
                        principalTable: "SystemEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientDetails_SystemEntries_ReasonId",
                        column: x => x.ReasonId,
                        principalTable: "SystemEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientDetails_SystemEntries_StatusId",
                        column: x => x.StatusId,
                        principalTable: "SystemEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientDetails_SystemEntries_TitelId",
                        column: x => x.TitelId,
                        principalTable: "SystemEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientDetails_Tarifs_TarifId",
                        column: x => x.TarifId,
                        principalTable: "Tarifs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DirectProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AddressId = table.Column<int>(type: "INTEGER", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectProviders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectProviders_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProfessionalProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Specialty = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AddressId = table.Column<int>(type: "INTEGER", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    LicenseNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalProviders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessionalProviders_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientCosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    CostType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientCosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientCosts_ClientDetails_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientDiseases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    DiseaseName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IcdCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DiagnosisDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientDiseases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientDiseases_ClientDetails_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    FeatureName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    FeatureValue = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientFeatures_ClientDetails_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientMedications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    MedicationName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ActiveIngredient = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Dosage = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Frequency = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TimeOfDay = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PrescribedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PrescribedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EmergencyNotes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientMedications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientMedications_ClientDetails_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientNotations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientNotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientNotations_ClientDetails_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientPhones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    PhoneType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientPhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientPhones_ClientDetails_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusId = table.Column<int>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ChangeDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ChangedBy = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientStatusHistories_ClientDetails_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    DeviceType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SerialNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Manufacturer = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    WarrantyEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AssignedClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_ClientDetails_AssignedClientId",
                        column: x => x.AssignedClientId,
                        principalTable: "ClientDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    SalesOrderId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PaidDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SubTotal = table.Column<decimal>(type: "TEXT", nullable: true),
                    Tax = table.Column<decimal>(type: "TEXT", nullable: true),
                    Discount = table.Column<decimal>(type: "TEXT", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    PaidAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    OutstandingAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ReminderCount = table.Column<int>(type: "INTEGER", nullable: true),
                    LastReminderDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_ClientDetails_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SalesOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    OpportunityId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SubTotal = table.Column<decimal>(type: "TEXT", nullable: true),
                    Tax = table.Column<decimal>(type: "TEXT", nullable: true),
                    Discount = table.Column<decimal>(type: "TEXT", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    PaymentTerms = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrders_ClientDetails_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientMedecins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfessionalProviderId = table.Column<int>(type: "INTEGER", nullable: true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    AssignedDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientMedecins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientMedecins_ClientDetails_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientMedecins_ProfessionalProviders_ProfessionalProviderId",
                        column: x => x.ProfessionalProviderId,
                        principalTable: "ProfessionalProviders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProfessionalProviderClientLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProfessionalProviderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    LinkDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalProviderClientLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessionalProviderClientLinks_ProfessionalProviders_ProfessionalProviderId",
                        column: x => x.ProfessionalProviderId,
                        principalTable: "ProfessionalProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ArticleId = table.Column<int>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    VatRate = table.Column<decimal>(type: "TEXT", nullable: true),
                    Discount = table.Column<decimal>(type: "TEXT", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    ServicePeriod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MandantId = table.Column<int>(type: "INTEGER", nullable: false),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    PaymentNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Reference = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SalesOrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ArticleId = table.Column<int>(type: "INTEGER", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    Discount = table.Column<decimal>(type: "TEXT", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrderItems_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesOrderItems_SalesOrders_SalesOrderId",
                        column: x => x.SalesOrderId,
                        principalTable: "SalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Articles",
                columns: new[] { "Id", "ArticleNumber", "Category", "CreateDate", "CreateId", "Description", "IsActive", "MandantId", "MaxStock", "MinStock", "Name", "PurchasePrice", "ReorderPoint", "RequiresPrescription", "SalesPrice", "StorageConditions", "SubCategory", "Supplier", "SupplierArticleNumber", "Unit", "VatRate" },
                values: new object[,]
                {
                    { 1, "PB-001", "Device", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2360), null, "Elektrisch verstellbares Pflegebett", true, 1, 20, 5, "Pflegebett Standard", 800m, 8, false, 1200m, null, "Betten", "Stiegelmeyer", null, "Stück", 19m },
                    { 2, "PB-002", "Device", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2366), null, "Premium Pflegebett mit Seitengitter", true, 1, 15, 3, "Pflegebett Premium", 1200m, 5, false, 1800m, null, "Betten", "Stiegelmeyer", null, "Stück", 19m },
                    { 3, "RO-001", "Device", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2391), null, "Leichtgewicht-Rollator", true, 1, 50, 10, "Rollator Standard", 80m, 15, false, 150m, null, "Mobilität", "Russka", null, "Stück", 19m },
                    { 4, "RS-001", "Device", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2396), null, "Faltbarer Rollstuhl", true, 1, 25, 5, "Rollstuhl Standard", 200m, 8, false, 350m, null, "Mobilität", "Bischoff & Bischoff", null, "Stück", 19m },
                    { 5, "NA-001", "Device", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2400), null, "GPS-Notrufarmband mit Sturzerkennung", true, 1, 100, 20, "Notrufarmband", 150m, 30, false, 280m, null, "Sicherheit", "Tunstall", null, "Stück", 19m },
                    { 6, "MED-001", "Medication", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2406), null, "Schmerzmittel 50 Tabletten", true, 1, 200, 50, "Ibuprofen 400mg", 3.50m, 80, false, 6.99m, null, "Schmerzmittel", "Ratiopharm", null, "Packung", 19m },
                    { 7, "MED-002", "Medication", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2410), null, "Diabetes-Medikament 100 Tabletten", true, 1, 100, 30, "Metformin 500mg", 8.00m, 50, true, 15.99m, null, "Diabetes", "Hexal", null, "Packung", 19m },
                    { 8, "SUP-001", "Supply", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2415), null, "Nitril-Handschuhe Größe L, 100 Stück", true, 1, 500, 100, "Einmalhandschuhe L", 8.00m, 150, false, 14.99m, null, "Hygiene", "Hartmann", null, "Box", 19m },
                    { 9, "SUP-002", "Supply", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2420), null, "Händedesinfektionsmittel", true, 1, 200, 50, "Desinfektionsmittel 1L", 5.00m, 80, false, 9.99m, null, "Hygiene", "Schülke", null, "Flasche", 19m },
                    { 10, "SRV-001", "Service", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2424), null, "Professionelle Pflegeberatung", true, 1, 0, 0, "Pflegeberatung Stunde", 0m, 0, false, 85m, null, "Beratung", null, null, "Stunde", 19m }
                });

            migrationBuilder.InsertData(
                table: "Campaigns",
                columns: new[] { "Id", "ActualCost", "ActualLeads", "Budget", "Channel", "ConvertedLeads", "CreateDate", "CreateId", "Description", "EndDate", "MandantId", "Name", "StartDate", "Status", "TargetAudience", "TargetLeads", "Type" },
                values: new object[,]
                {
                    { 1, 4200m, 87, 5000m, "Email", 23, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1734), null, "E-Mail-Kampagne für Pflegedienste", new DateTime(2024, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Frühjahrs-Pflegekampagne 2024", new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Completed", "Senioren 65+", 100, "Email" },
                    { 2, 6500m, 134, 12000m, "Social Media", 31, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1739), null, "Facebook & Instagram Kampagne", new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Social Media Awareness", new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Active", "Angehörige", 200, "Social" },
                    { 3, 14200m, 67, 15000m, "Event", 18, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1745), null, "Messestand auf der Altenpflege-Messe", new DateTime(2024, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Messe Altenpflege 2024", new DateTime(2024, 4, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Completed", "Fachpublikum", 50, "Event" },
                    { 4, 3200m, 45, 8000m, "Referral", 28, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1749), null, "Kunden-werben-Kunden Programm", new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Empfehlungsprogramm", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Active", "Bestandskunden", 80, "Referral" },
                    { 5, 2100m, 56, 6000m, "Google", 12, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1754), null, "Google Ads für Pflegeberatung", new DateTime(2024, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Google Ads Herbst", new DateTime(2024, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Active", "Pflegebedürftige", 120, "Digital" }
                });

            migrationBuilder.InsertData(
                table: "ClientStatuses",
                columns: new[] { "Id", "BooleanOptions", "Code", "CreateDate", "CreateId", "Description", "IsActive", "MandantId", "Name", "UpdateDate", "UpdateId" },
                values: new object[,]
                {
                    { 1, null, "ACTIVE", null, null, null, true, 1, "Aktiv", null, null },
                    { 2, null, "INACTIVE", null, null, null, true, 1, "Inaktiv", null, null },
                    { 3, null, "PAUSED", null, null, null, true, 1, "Pausiert", null, null },
                    { 4, null, "CANCELLED", null, null, null, true, 1, "Gekündigt", null, null }
                });

            migrationBuilder.InsertData(
                table: "CostCenters",
                columns: new[] { "Id", "ActualSpend", "Budget", "Code", "Department", "Description", "IsActive", "MandantId", "Name" },
                values: new object[,]
                {
                    { 1, 32000m, 50000m, "CC-100", "Sales", "Vertriebsabteilung", true, 1, "Vertrieb" },
                    { 2, 28500m, 40000m, "CC-200", "Marketing", "Marketingabteilung", true, 1, "Marketing" },
                    { 3, 18000m, 25000m, "CC-300", "Operations", "Lagerverwaltung", true, 1, "Lager & Logistik" },
                    { 4, 29000m, 35000m, "CC-400", "Admin", "Allgemeine Verwaltung", true, 1, "Verwaltung" },
                    { 5, 22000m, 30000m, "CC-500", "IT", "IT-Abteilung", true, 1, "IT" }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "IsoCode", "Name", "PhoneCode" },
                values: new object[,]
                {
                    { 1, "DE", "Deutschland", "+49" },
                    { 2, "AT", "Österreich", "+43" },
                    { 3, "CH", "Schweiz", "+41" },
                    { 4, "LU", "Luxemburg", "+352" }
                });

            migrationBuilder.InsertData(
                table: "DeviceDetails",
                columns: new[] { "Id", "AssignedClientId", "CreateDate", "CreateId", "Description", "DeviceType", "Location", "MandantId", "Manufacturer", "Model", "Notes", "PurchaseDate", "PurchasePrice", "SerialNumber", "Status", "UpdateDate", "UpdateId", "WarrantyEndDate" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1078), "admin", "Elektrischer Rollstuhl", "Rollstuhl", null, 1, "Ottobock", "B500", null, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "RS-001-2024", "Verfügbar", null, null, null },
                    { 2, 2, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1085), "admin", "Elektrisches Pflegebett", "Pflegebett", null, 1, "Burmeier", "Regia", null, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PB-002-2024", "In Benutzung", null, null, null },
                    { 3, 3, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1476), "admin", "Leichtgewicht-Rollator", "Rollator", null, 1, "Topro", "Troja", null, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "RO-003-2024", "In Benutzung", null, null, null },
                    { 4, 4, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1479), "admin", "Niedrigbett mit Aufstehhilfe", "Pflegebett", null, 1, "Völker", "S962", null, new DateTime(2024, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PB-004-2024", "In Benutzung", null, null, null },
                    { 5, 5, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1485), "admin", "Mobiler Sauerstoffkonzentrator", "Sauerstoffgerät", null, 1, "Philips", "SimplyGo", null, new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "O2-005-2024", "In Benutzung", null, null, null },
                    { 6, null, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1487), "admin", "Faltbarer Standardrollstuhl", "Rollstuhl", null, 1, "Bischoff & Bischoff", "S-Eco 2", null, new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "RS-006-2024", "Verfügbar", null, null, null },
                    { 7, 6, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1490), "admin", "Elektrischer Badewannenlift", "Badewannenlift", null, 1, "Aquatec", "Orca", null, new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "BL-007-2024", "In Benutzung", null, null, null },
                    { 8, 8, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1493), "admin", "Mobiler Patientenlifter", "Patientenlifter", null, 1, "Arjo", "Maxi Move", null, new DateTime(2023, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PL-008-2024", "In Benutzung", null, null, null },
                    { 9, null, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1495), "admin", "Indoor-Rollator", "Rollator", null, 1, "Russka", "Vital", null, new DateTime(2023, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "RO-009-2023", "Defekt", null, null, null },
                    { 10, null, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1515), "admin", "Pflegebett mit Seitengitter", "Pflegebett", null, 1, "Stiegelmeyer", "Puro", null, new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PB-010-2024", "Verfügbar", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "DirectProviderDetails",
                columns: new[] { "Id", "City", "Country", "CreateDate", "CreateId", "Email", "Fax", "FirstName", "HouseNumber", "LastName", "MandantId", "Mobile", "Name", "Notes", "Phone", "Status", "Street", "UpdateDate", "UpdateId", "Website", "ZipCode" },
                values: new object[,]
                {
                    { 1, "München", "Deutschland", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1236), "admin", "info@sonnenschein-pflege.de", null, "Maria", "20", "Huber", 1, null, "Pflegedienst Sonnenschein", null, "+49 89 11111111", "Aktiv", "Pflegeweg", null, null, null, "80331" },
                    { 2, "Stuttgart", "Deutschland", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1241), "admin", "info@caritas-pflege.de", null, "Thomas", "15", "Weber", 1, null, "Caritas Pflegedienst", null, "+49 711 22222222", "Aktiv", "Kirchstraße", null, null, null, "70173" },
                    { 3, "Köln", "Deutschland", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1244), "admin", "info@awo-koeln.de", null, "Sabine", "8", "Klein", 1, null, "AWO Pflegedienst Köln", null, "+49 221 33333333", "Aktiv", "Domstraße", null, null, null, "50667" }
                });

            migrationBuilder.InsertData(
                table: "Leads",
                columns: new[] { "Id", "AssignedToName", "AssignedToUserId", "CampaignId", "Company", "ConversionDate", "ConvertedClientId", "CreateDate", "CreateId", "Email", "EstimatedValue", "ExpectedCloseDate", "FirstContactDate", "FirstName", "LastContactDate", "LastName", "MandantId", "Notes", "Phone", "QualificationScore", "Score", "Source", "Status" },
                values: new object[] { 8, "Peter Müller", null, null, "Ambulanter Pflegedienst Herz", null, null, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2126), null, "m.wolf@yahoo.de", 5500m, null, new DateTime(2024, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Monika", new DateTime(2024, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Wolf", 1, null, "+49 178 8901234", "Warm", 72, "Cold Call", "Qualified" });

            migrationBuilder.InsertData(
                table: "Medications",
                columns: new[] { "Id", "ArticleId", "Contraindications", "DosageForm", "GenericName", "IsActive", "MandantId", "Manufacturer", "Name", "PackageSize", "Price", "RequiresPrescription", "StorageConditions", "Strength" },
                values: new object[,]
                {
                    { 3, null, null, "Capsule", "Omeprazol", true, 1, "Stada", "Omeprazol 20mg", "30 Stück", 12.50m, true, "Raumtemperatur", "20mg" },
                    { 4, null, null, "Tablet", "Ramipril", true, 1, "1A Pharma", "Ramipril 5mg", "100 Stück", 18.99m, true, "Raumtemperatur", "5mg" },
                    { 5, null, null, "Injection", "Insulin glargin", true, 1, "Sanofi", "Insulin Lantus", "5 Pens", 89.99m, true, "Kühlschrank 2-8°C", "100 E/ml" }
                });

            migrationBuilder.InsertData(
                table: "ProfessionalProviderDetails",
                columns: new[] { "Id", "City", "Country", "CreateDate", "CreateId", "Email", "Fax", "FirstName", "HouseNumber", "LastName", "LicenseNumber", "MandantId", "Mobile", "Name", "Notes", "Phone", "Qualifications", "Specialty", "Status", "Street", "UpdateDate", "UpdateId", "Website", "ZipCode" },
                values: new object[,]
                {
                    { 1, "München", "Deutschland", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1172), "admin", "dr.schmidt@praxis.de", null, "Hans", "10", "Schmidt", "DE-BY-12345", 1, null, "Dr. Schmidt", null, "+49 89 12345678", null, "Allgemeinmedizin", "Aktiv", "Arztstraße", null, null, null, "80331" },
                    { 2, "Stuttgart", "Deutschland", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1177), "admin", "dr.mueller@klinik.de", null, "Anna", "5", "Müller", "DE-BW-67890", 1, null, "Dr. Müller", null, "+49 711 87654321", null, "Geriatrie", "Aktiv", "Klinikweg", null, null, null, "70173" }
                });

            migrationBuilder.InsertData(
                table: "SystemEntries",
                columns: new[] { "Id", "BooleanOptions", "Code", "CreateDate", "CreateId", "Description", "IsActive", "MandantId", "SortOrder", "Type", "UpdateDate", "UpdateId" },
                values: new object[,]
                {
                    { 1, null, "MR", new DateTime(2026, 1, 22, 21, 51, 50, 507, DateTimeKind.Utc).AddTicks(9916), null, "Herr", true, 1, 1, "T", null, null },
                    { 2, null, "MRS", new DateTime(2026, 1, 22, 21, 51, 50, 507, DateTimeKind.Utc).AddTicks(9927), null, "Frau", true, 1, 2, "T", null, null },
                    { 3, null, "DIV", new DateTime(2026, 1, 22, 21, 51, 50, 507, DateTimeKind.Utc).AddTicks(9929), null, "Divers", true, 1, 3, "T", null, null },
                    { 4, null, "DR", new DateTime(2026, 1, 22, 21, 51, 50, 507, DateTimeKind.Utc).AddTicks(9968), null, "Dr.", true, 1, 1, "P", null, null },
                    { 5, null, "PROF", new DateTime(2026, 1, 22, 21, 51, 50, 507, DateTimeKind.Utc).AddTicks(9970), null, "Prof.", true, 1, 2, "P", null, null },
                    { 6, null, "PROFDR", new DateTime(2026, 1, 22, 21, 51, 50, 507, DateTimeKind.Utc).AddTicks(9972), null, "Prof. Dr.", true, 1, 3, "P", null, null },
                    { 7, null, "ACTIVE", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(9), null, "Aktiv", true, 1, 1, "S", null, null },
                    { 8, null, "INACTIVE", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(11), null, "Inaktiv", true, 1, 2, "S", null, null },
                    { 9, null, "PAUSED", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(13), null, "Pausiert", true, 1, 3, "S", null, null },
                    { 10, null, "SINGLE", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(52), null, "Ledig", true, 1, 1, "M", null, null },
                    { 11, null, "MARRIED", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(54), null, "Verheiratet", true, 1, 2, "M", null, null },
                    { 12, null, "DIVORCED", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(56), null, "Geschieden", true, 1, 3, "M", null, null },
                    { 13, null, "WIDOWED", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(58), null, "Verwitwet", true, 1, 4, "M", null, null },
                    { 14, null, "INVOICE", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(90), null, "Rechnung", true, 1, 1, "I", null, null },
                    { 15, null, "DEBIT", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(93), null, "Lastschrift", true, 1, 2, "I", null, null },
                    { 16, null, "CASH", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(95), null, "Bar", true, 1, 3, "I", null, null },
                    { 17, null, "LOW", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(127), null, "Niedrig", true, 1, 1, "PR", null, null },
                    { 18, null, "NORMAL", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(254), null, "Normal", true, 1, 2, "PR", null, null },
                    { 19, null, "HIGH", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(258), null, "Hoch", true, 1, 3, "PR", null, null },
                    { 20, null, "URGENT", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(260), null, "Dringend", true, 1, 4, "PR", null, null },
                    { 21, null, "DE", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(299), null, "Deutsch", true, 1, 1, "L", null, null },
                    { 22, null, "EN", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(301), null, "Englisch", true, 1, 2, "L", null, null },
                    { 23, null, "FR", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(303), null, "Französisch", true, 1, 3, "L", null, null },
                    { 24, null, "CARE", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(375), null, "Pflegebedarf", true, 1, 1, "R", null, null },
                    { 25, null, "REHAB", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(377), null, "Rehabilitation", true, 1, 2, "R", null, null },
                    { 26, null, "THERAPY", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(381), null, "Therapie", true, 1, 3, "R", null, null },
                    { 27, null, "PUBLIC", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(414), null, "Gesetzlich", true, 1, 1, "IC", null, null },
                    { 28, null, "PRIVATE", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(416), null, "Privat", true, 1, 2, "IC", null, null },
                    { 29, null, "SELF", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(418), null, "Selbstzahler", true, 1, 3, "IC", null, null },
                    { 30, null, "PG1", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(456), null, "Pflegegrad 1", true, 1, 1, "ICR", null, null },
                    { 31, null, "PG2", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(467), null, "Pflegegrad 2", true, 1, 2, "ICR", null, null },
                    { 32, null, "PG3", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(469), null, "Pflegegrad 3", true, 1, 3, "ICR", null, null },
                    { 33, null, "PG4", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(471), null, "Pflegegrad 4", true, 1, 4, "ICR", null, null },
                    { 34, null, "PG5", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(473), null, "Pflegegrad 5", true, 1, 5, "ICR", null, null },
                    { 35, null, "GA", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(509), null, "Gruppe A", true, 1, 1, "FG", null, null },
                    { 36, null, "GB", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(511), null, "Gruppe B", true, 1, 2, "FG", null, null },
                    { 37, null, "GC", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(514), null, "Gruppe C", true, 1, 3, "FG", null, null },
                    { 38, null, "LOW", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1625), null, "Niedrig", true, 1, 1, "PRIO", null, null },
                    { 39, null, "NORMAL", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1628), null, "Normal", true, 1, 2, "PRIO", null, null },
                    { 40, null, "HIGH", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1629), null, "Hoch", true, 1, 3, "PRIO", null, null },
                    { 41, null, "URGENT", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1631), null, "Dringend", true, 1, 4, "PRIO", null, null },
                    { 42, null, "TR", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1664), null, "Türkisch", true, 1, 4, "LANG", null, null },
                    { 43, null, "RU", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1666), null, "Russisch", true, 1, 5, "LANG", null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreateDate", "CreateId", "Email", "FirstName", "IsActive", "LastLogin", "LastName", "MandantId", "PasswordHash", "Role", "UpdateDate", "UpdateId", "Username" },
                values: new object[] { 1, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(704), null, "admin@umoapi.local", "Admin", true, null, "User", 1, "AQAAAAIAAYagAAAAELBzKvJvYQQwG5GqKqLvPxQYl5CvxNFj0VBmJqPqYqPqYqPqYqPqYqPqYqPqYqPqYg==", "Admin", null, null, "admin" });

            migrationBuilder.InsertData(
                table: "VatTaxes",
                columns: new[] { "Id", "Code", "CreateDate", "CreateId", "IsActive", "MandantId", "Name", "Percentage" },
                values: new object[,]
                {
                    { 1, "STD", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(577), null, true, 1, "Standard", 19.0m },
                    { 2, "RED", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(580), null, true, 1, "Ermäßigt", 7.0m },
                    { 3, "FREE", new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(582), null, true, 1, "Steuerfrei", 0.0m }
                });

            migrationBuilder.InsertData(
                table: "Districts",
                columns: new[] { "Id", "Code", "CountryId", "MandantId", "Name" },
                values: new object[,]
                {
                    { 1, "BY", 1, 1, "Bayern" },
                    { 2, "BW", 1, 1, "Baden-Württemberg" },
                    { 3, "NW", 1, 1, "Nordrhein-Westfalen" },
                    { 4, "W", 2, 1, "Wien" }
                });

            migrationBuilder.InsertData(
                table: "Inventories",
                columns: new[] { "Id", "ArticleId", "AvailableStock", "BatchNumber", "CurrentStock", "ExpiryDate", "LastMovementDate", "LastStockTake", "MandantId", "ReservedStock", "WarehouseLocation" },
                values: new object[,]
                {
                    { 1, 1, 10, null, 12, null, new DateTime(2024, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Lager A-01" },
                    { 2, 2, 7, null, 8, null, new DateTime(2024, 8, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, "Lager A-02" },
                    { 3, 3, 30, null, 35, null, new DateTime(2024, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, "Lager B-01" },
                    { 4, 4, 15, null, 18, null, new DateTime(2024, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, "Lager B-02" },
                    { 5, 5, 35, null, 45, null, new DateTime(2024, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 10, "Lager C-01" },
                    { 6, 6, 120, "LOT-2024-001", 120, new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, "Lager D-01" },
                    { 7, 7, 65, "LOT-2024-002", 65, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, "Lager D-02" },
                    { 8, 8, 230, null, 250, null, new DateTime(2024, 8, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 20, "Lager E-01" },
                    { 9, 9, 90, null, 95, null, new DateTime(2024, 8, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, "Lager E-02" }
                });

            migrationBuilder.InsertData(
                table: "Leads",
                columns: new[] { "Id", "AssignedToName", "AssignedToUserId", "CampaignId", "Company", "ConversionDate", "ConvertedClientId", "CreateDate", "CreateId", "Email", "EstimatedValue", "ExpectedCloseDate", "FirstContactDate", "FirstName", "LastContactDate", "LastName", "MandantId", "Notes", "Phone", "QualificationScore", "Score", "Source", "Status" },
                values: new object[,]
                {
                    { 1, "Maria Schmidt", null, 2, null, null, null, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1871), null, "t.becker@email.de", 2400m, null, new DateTime(2024, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Thomas", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Becker", 1, null, "+49 171 1234567", "Hot", 85, "Website", "Qualified" },
                    { 2, "Peter Müller", null, 4, "Pflegeheim Sonnenschein", null, null, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1878), null, "s.koch@gmx.de", 8500m, null, new DateTime(2024, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sabine", new DateTime(2024, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Koch", 1, null, "+49 172 2345678", "Hot", 92, "Referral", "Proposal" },
                    { 3, "Maria Schmidt", null, 1, null, null, null, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1882), null, "m.wagner@web.de", 1800m, null, new DateTime(2024, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Michael", new DateTime(2024, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Wagner", 1, null, "+49 173 3456789", "Warm", 65, "Campaign", "Contacted" },
                    { 4, "Anna Weber", null, 5, null, null, null, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1886), null, "c.hoffmann@outlook.de", 1200m, null, new DateTime(2024, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Claudia", null, "Hoffmann", 1, null, "+49 174 4567890", "Cold", 35, "Website", "New" },
                    { 5, "Peter Müller", null, 3, "Seniorenresidenz Am Park", new DateTime(2024, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2101), null, "s.richter@t-online.de", 12000m, null, new DateTime(2024, 4, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Stefan", new DateTime(2024, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Richter", 1, null, "+49 175 5678901", "Hot", 98, "Event", "Won" },
                    { 6, "Maria Schmidt", null, 2, null, null, null, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2105), null, "p.schulz@email.de", 3600m, null, new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Petra", new DateTime(2024, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Schulz", 1, null, "+49 176 6789012", "Hot", 88, "Website", "Negotiation" },
                    { 7, "Anna Weber", null, 4, null, null, null, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2122), null, "a.klein@gmail.com", 2000m, null, new DateTime(2024, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Andreas", new DateTime(2024, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Klein", 1, null, "+49 177 7890123", "Warm", 55, "Referral", "Lost" }
                });

            migrationBuilder.InsertData(
                table: "Medications",
                columns: new[] { "Id", "ArticleId", "Contraindications", "DosageForm", "GenericName", "IsActive", "MandantId", "Manufacturer", "Name", "PackageSize", "Price", "RequiresPrescription", "StorageConditions", "Strength" },
                values: new object[,]
                {
                    { 1, 6, null, "Tablet", "Ibuprofen", true, 1, "Ratiopharm", "Ibuprofen 400mg", "50 Stück", 6.99m, false, "Raumtemperatur", "400mg" },
                    { 2, 7, null, "Tablet", "Metformin", true, 1, "Hexal", "Metformin 500mg", "100 Stück", 15.99m, true, "Raumtemperatur", "500mg" }
                });

            migrationBuilder.InsertData(
                table: "SalesOpportunities",
                columns: new[] { "Id", "ActualCloseDate", "Amount", "AssignedToName", "ClientId", "CompetitorInfo", "CreateDate", "CreateId", "ExpectedCloseDate", "LeadId", "MandantId", "Name", "Notes", "Probability", "ProductInterest", "Stage" },
                values: new object[] { 4, null, 5500m, "Peter Müller", null, null, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2288), null, new DateTime(2024, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 1, "Ambulanter Dienst Herz - Geräte", null, 40, "Diverse Geräte", "Qualification" });

            migrationBuilder.InsertData(
                table: "Tarifs",
                columns: new[] { "Id", "BasePrice", "CreateDate", "CreateId", "Description", "IsActive", "MandantId", "Name", "TotalPrice", "UpdateDate", "UpdateId", "ValidFrom", "ValidTo", "VatTaxId" },
                values: new object[,]
                {
                    { 1, 100.00m, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(648), null, "Grundlegende Pflegeleistungen", true, 1, "Basis-Tarif", 100.00m, null, null, null, null, 3 },
                    { 2, 200.00m, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(652), null, "Erweiterte Pflegeleistungen", true, 1, "Premium-Tarif", 200.00m, null, null, null, null, 3 },
                    { 3, 350.00m, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(656), null, "Intensive Pflegeleistungen", true, 1, "Intensiv-Tarif", 350.00m, null, null, null, null, 3 },
                    { 4, 150.00m, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1278), null, "Kompakte Pflegeleistungen", true, 1, "Kompakt-Tarif", 150.00m, null, null, null, null, 3 },
                    { 5, 280.00m, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1281), null, "Familienfreundliche Pflegeleistungen", true, 1, "Familien-Tarif", 280.00m, null, null, null, null, 3 }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CountryId", "DistrictId", "MandantId", "Name", "ZipCode" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, "München", "80331" },
                    { 2, 1, 2, 1, "Stuttgart", "70173" },
                    { 3, 1, 3, 1, "Köln", "50667" },
                    { 4, 2, 4, 1, "Wien", "1010" }
                });

            migrationBuilder.InsertData(
                table: "LeadActivities",
                columns: new[] { "Id", "ActivityDate", "Description", "Duration", "LeadId", "MandantId", "NextFollowUp", "Outcome", "PerformedBy", "Type" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 7, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Erstgespräch - Interesse an Pflegebett", 25, 1, 1, null, "Positive", "Maria Schmidt", "Call" },
                    { 2, new DateTime(2024, 7, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Produktinformationen gesendet", 10, 1, 1, null, "Neutral", "Maria Schmidt", "Email" },
                    { 3, new DateTime(2024, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vor-Ort-Termin im Pflegeheim", 90, 2, 1, null, "Positive", "Peter Müller", "Meeting" },
                    { 4, new DateTime(2024, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Angebot für 10 Pflegebetten erstellt", 60, 2, 1, null, "Positive", "Peter Müller", "Proposal" },
                    { 5, new DateTime(2024, 4, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Produktvorführung auf der Messe", 45, 5, 1, null, "Positive", "Peter Müller", "Demo" },
                    { 6, new DateTime(2024, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nachfassgespräch - Preisverhandlung", 30, 6, 1, null, "Positive", "Maria Schmidt", "Call" }
                });

            migrationBuilder.InsertData(
                table: "SalesOpportunities",
                columns: new[] { "Id", "ActualCloseDate", "Amount", "AssignedToName", "ClientId", "CompetitorInfo", "CreateDate", "CreateId", "ExpectedCloseDate", "LeadId", "MandantId", "Name", "Notes", "Probability", "ProductInterest", "Stage" },
                values: new object[,]
                {
                    { 1, null, 8500m, "Peter Müller", null, null, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2278), null, new DateTime(2024, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "Pflegeheim Sonnenschein - Betten", null, 75, "Pflegebetten", "Proposal" },
                    { 2, null, 2400m, "Maria Schmidt", null, null, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2282), null, new DateTime(2024, 8, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, "Becker - Pflegebett", null, 80, "Pflegebett Einzeln", "Negotiation" },
                    { 3, null, 3600m, "Maria Schmidt", null, null, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2285), null, new DateTime(2024, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 1, "Schulz - Komplett-Ausstattung", null, 70, "Pflegebett + Rollator", "Negotiation" },
                    { 5, new DateTime(2024, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 12000m, "Peter Müller", null, null, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2292), null, new DateTime(2024, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1, "Seniorenresidenz - Abschluss", null, 100, "Großauftrag", "Closed Won" }
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "AdditionalInfo", "CityId", "CountryId", "CreateDate", "CreateId", "DistrictId", "HouseNumber", "MandantId", "Street", "UpdateDate", "UpdateId", "ZipCode" },
                values: new object[,]
                {
                    { 1, null, 1, 1, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(813), null, 1, "1", 1, "Hauptstraße", null, null, "80331" },
                    { 2, null, 2, 1, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(816), null, 2, "50", 1, "Königstraße", null, null, "70173" },
                    { 3, null, 3, 1, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1313), null, 3, "25", 1, "Domstraße", null, null, "50667" },
                    { 4, null, 4, 2, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1315), null, 4, "12", 1, "Ringstraße", null, null, "1010" },
                    { 5, null, 1, 1, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1317), null, 1, "3", 1, "Marienplatz", null, null, "80331" },
                    { 6, null, 2, 1, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1320), null, 2, "100", 1, "Schlossallee", null, null, "70173" },
                    { 7, null, 3, 1, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1322), null, 3, "45", 1, "Rheinufer", null, null, "50667" },
                    { 8, null, 4, 2, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1324), null, 4, "7", 1, "Stephansplatz", null, null, "1010" }
                });

            migrationBuilder.InsertData(
                table: "ClientDetails",
                columns: new[] { "Id", "AddressId", "BankName", "BankNumber", "Bill", "BirthDay", "BooleanOptions", "ClassificationId", "CreateDate", "CreateId", "DirectProviderId", "EndContractDate", "FinancialGroupId", "FirstName", "FreeFeld", "InsuresCare", "InvoiceMethodId", "LanguageId", "LastName", "MandantId", "MaritalStatusId", "MedecinPlace", "Memo", "Note", "Nummer", "PolicyNumber", "PostBankNumber", "PrefixId", "PriorityId", "ReasonId", "Sex", "SocialSecurityNumber", "StartContractDate", "StatusId", "TarifId", "TitelId", "UpdateDate", "UpdateId" },
                values: new object[,]
                {
                    { 1, 1, null, null, null, new DateTime(1960, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 27, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(892), "admin", null, null, null, "Max", null, null, 14, 21, "Mustermann", 1, 11, null, null, null, 1001, "POL-001", null, null, 18, null, "M", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 1, 1, null, null },
                    { 2, 2, null, null, null, new DateTime(1955, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 28, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(999), "admin", null, null, null, "Erika", null, null, 15, 21, "Musterfrau", 1, 13, null, null, null, 1002, "POL-002", null, null, 19, null, "F", null, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 2, 2, null, null },
                    { 3, 3, null, null, null, new DateTime(1945, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 27, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1363), "admin", 3, null, null, "Hans", null, null, 14, 21, "Meier", 1, 11, null, null, null, 1003, "POL-003", null, null, 17, null, "M", null, new DateTime(2024, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 3, 1, null, null },
                    { 4, 4, null, null, null, new DateTime(1938, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 28, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1368), "admin", 1, null, null, "Helga", null, null, 15, 21, "Schulz", 1, 13, null, null, null, 1004, "POL-004", null, null, 20, null, "F", null, new DateTime(2024, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 3, 2, null, null },
                    { 5, 5, null, null, null, new DateTime(1952, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 27, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1374), "admin", 1, null, null, "Werner", null, null, 14, 22, "Fischer", 1, 11, null, null, null, 1005, "POL-005", null, null, 18, null, "M", null, new DateTime(2024, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 2, 1, null, null },
                    { 6, 6, null, null, null, new DateTime(1948, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 29, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1380), "admin", 2, null, null, "Ingrid", null, null, 16, 21, "Wagner", 1, 12, null, null, null, 1006, "POL-006", null, null, 19, null, "F", null, new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 1, 2, null, null },
                    { 7, 7, null, null, null, new DateTime(1940, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 27, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1385), "admin", 3, null, null, "Klaus", null, null, 14, 21, "Becker", 1, 13, null, null, null, 1007, "POL-007", null, null, 17, null, "M", null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 1, 1, null, null },
                    { 8, 8, null, null, null, new DateTime(1935, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 28, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1391), "admin", 2, null, null, "Gerda", null, null, 15, 21, "Hoffmann", 1, 13, null, null, null, 1008, "POL-008", null, null, 20, null, "F", null, new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 3, 2, null, null },
                    { 9, 1, null, null, null, new DateTime(1958, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 27, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1428), "admin", 1, null, null, "Friedrich", null, null, 14, 23, "Zimmermann", 1, 11, null, null, null, 1009, "POL-009", null, null, 18, null, "M", null, new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 2, 1, null, null },
                    { 10, 2, null, null, null, new DateTime(1942, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 29, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(1433), "admin", 2, null, null, "Ursula", null, null, 16, 21, "Braun", 1, 12, null, null, null, 1010, "POL-010", null, null, 17, null, "F", null, new DateTime(2023, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, 1, 2, null, null }
                });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "Id", "ClientId", "CreateDate", "CreateId", "Discount", "DueDate", "InvoiceDate", "InvoiceNumber", "LastReminderDate", "MandantId", "Notes", "OutstandingAmount", "PaidAmount", "PaidDate", "PaymentMethod", "ReminderCount", "SalesOrderId", "Status", "SubTotal", "Tax", "TotalAmount" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2657), null, null, new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV-2024-001", null, 1, null, 0m, 1428m, new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bank Transfer", null, null, "Paid", 1200m, 228m, 1428m },
                    { 2, 2, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2665), null, null, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV-2024-002", null, 1, null, 0m, 2856m, new DateTime(2024, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Direct Debit", null, null, "Paid", 2400m, 456m, 2856m },
                    { 3, 3, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2671), null, null, new DateTime(2024, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV-2024-003", new DateTime(2024, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, 1011.50m, 0m, null, null, 2, null, "Overdue", 850m, 161.50m, 1011.50m },
                    { 4, 4, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2676), null, null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV-2024-004", null, 1, null, 2142m, 0m, null, null, null, null, "Sent", 1800m, 342m, 2142m },
                    { 5, 5, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2682), null, null, new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV-2024-005", null, 1, null, 0m, 4165m, new DateTime(2024, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bank Transfer", null, null, "Paid", 3500m, 665m, 4165m },
                    { 6, 6, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2689), null, null, new DateTime(2024, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV-2024-006", null, 1, null, 0m, 1130.50m, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Credit Card", null, null, "Paid", 950m, 180.50m, 1130.50m },
                    { 7, 7, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2695), null, null, new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV-2024-007", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, 1285m, 500m, null, null, 3, null, "Overdue", 1500m, 285m, 1785m },
                    { 8, 8, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2703), null, null, new DateTime(2024, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV-2024-008", null, 1, null, 2618m, 0m, null, null, null, null, "Draft", 2200m, 418m, 2618m }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "ClientId", "CreateDate", "CreateId", "InvoiceId", "MandantId", "Notes", "PaymentDate", "PaymentMethod", "PaymentNumber", "Reference", "Status" },
                values: new object[,]
                {
                    { 1, 1428m, 1, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2779), null, 1, 1, null, new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bank Transfer", "PAY-2024-001", "Überweisung", "Completed" },
                    { 2, 2856m, 2, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2783), null, 2, 1, null, new DateTime(2024, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Direct Debit", "PAY-2024-002", "Lastschrift", "Completed" },
                    { 3, 4165m, 5, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2786), null, 5, 1, null, new DateTime(2024, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bank Transfer", "PAY-2024-003", "Überweisung", "Completed" },
                    { 4, 1130.50m, 6, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2789), null, 6, 1, null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Credit Card", "PAY-2024-004", "Kreditkarte", "Completed" },
                    { 5, 500m, 7, new DateTime(2026, 1, 22, 21, 51, 50, 508, DateTimeKind.Utc).AddTicks(2792), null, 7, 1, null, new DateTime(2024, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bank Transfer", "PAY-2024-005", "Teilzahlung", "Completed" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CityId",
                table: "Addresses",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CountryId",
                table: "Addresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_DistrictId",
                table: "Addresses",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_CallLogs_ClientId",
                table: "CallLogs",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_CallLogs_DispatcherId",
                table: "CallLogs",
                column: "DispatcherId");

            migrationBuilder.CreateIndex(
                name: "IX_CallLogs_EmergencyAlertId",
                table: "CallLogs",
                column: "EmergencyAlertId");

            migrationBuilder.CreateIndex(
                name: "IX_CallLogs_EmergencyContactId",
                table: "CallLogs",
                column: "EmergencyContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_DistrictId",
                table: "Cities",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCosts_ClientId",
                table: "ClientCosts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDetails_AddressId",
                table: "ClientDetails",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDetails_ClassificationId",
                table: "ClientDetails",
                column: "ClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDetails_DirectProviderId",
                table: "ClientDetails",
                column: "DirectProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDetails_FinancialGroupId",
                table: "ClientDetails",
                column: "FinancialGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDetails_InvoiceMethodId",
                table: "ClientDetails",
                column: "InvoiceMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDetails_LanguageId",
                table: "ClientDetails",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDetails_MandantId_Nummer",
                table: "ClientDetails",
                columns: new[] { "MandantId", "Nummer" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientDetails_MaritalStatusId",
                table: "ClientDetails",
                column: "MaritalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDetails_PrefixId",
                table: "ClientDetails",
                column: "PrefixId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDetails_PriorityId",
                table: "ClientDetails",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDetails_ReasonId",
                table: "ClientDetails",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDetails_StatusId",
                table: "ClientDetails",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDetails_TarifId",
                table: "ClientDetails",
                column: "TarifId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDetails_TitelId",
                table: "ClientDetails",
                column: "TitelId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDiseases_ClientId",
                table: "ClientDiseases",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientFeatures_ClientId",
                table: "ClientFeatures",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMedecins_ClientId",
                table: "ClientMedecins",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMedecins_ProfessionalProviderId",
                table: "ClientMedecins",
                column: "ProfessionalProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientMedications_ClientId",
                table: "ClientMedications",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientNotations_ClientId",
                table: "ClientNotations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientPhones_ClientId",
                table: "ClientPhones",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_MandantId_Nummer",
                table: "Clients",
                columns: new[] { "MandantId", "Nummer" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientStatusHistories_ClientId",
                table: "ClientStatusHistories",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_AssignedClientId",
                table: "Devices",
                column: "AssignedClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_MandantId_SerialNumber",
                table: "Devices",
                columns: new[] { "MandantId", "SerialNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_DirectProviders_AddressId",
                table: "DirectProviders",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatcherShifts_DispatcherId",
                table: "DispatcherShifts",
                column: "DispatcherId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_CountryId",
                table: "Districts",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyAlerts_AcknowledgedByDispatcherId",
                table: "EmergencyAlerts",
                column: "AcknowledgedByDispatcherId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyAlerts_ClientId",
                table: "EmergencyAlerts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyAlerts_EmergencyDeviceId",
                table: "EmergencyAlerts",
                column: "EmergencyDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyAlerts_ResolvedByDispatcherId",
                table: "EmergencyAlerts",
                column: "ResolvedByDispatcherId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyChainActions_DispatcherId",
                table: "EmergencyChainActions",
                column: "DispatcherId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyChainActions_EmergencyAlertId",
                table: "EmergencyChainActions",
                column: "EmergencyAlertId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContacts_ClientId",
                table: "EmergencyContacts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyDevices_ClientId",
                table: "EmergencyDevices",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_ArticleId",
                table: "Inventories",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_InvoiceId",
                table: "InvoiceItems",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ClientId",
                table: "Invoices",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadActivities_LeadId",
                table: "LeadActivities",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_CampaignId",
                table: "Leads",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_Medications_ArticleId",
                table: "Medications",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InvoiceId",
                table: "Payments",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalProviderClientLinks_ProfessionalProviderId",
                table: "ProfessionalProviderClientLinks",
                column: "ProfessionalProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalProviders_AddressId",
                table: "ProfessionalProviders",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOpportunities_LeadId",
                table: "SalesOpportunities",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderItems_ArticleId",
                table: "SalesOrderItems",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderItems_SalesOrderId",
                table: "SalesOrderItems",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_ClientId",
                table: "SalesOrders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ArticleId",
                table: "StockMovements",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemEntries_MandantId_Type_Code",
                table: "SystemEntries",
                columns: new[] { "MandantId", "Type", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_Tarifs_VatTaxId",
                table: "Tarifs",
                column: "VatTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CallLogs");

            migrationBuilder.DropTable(
                name: "ClientCosts");

            migrationBuilder.DropTable(
                name: "ClientDiseases");

            migrationBuilder.DropTable(
                name: "ClientFeatures");

            migrationBuilder.DropTable(
                name: "ClientMedecins");

            migrationBuilder.DropTable(
                name: "ClientMedications");

            migrationBuilder.DropTable(
                name: "ClientNotations");

            migrationBuilder.DropTable(
                name: "ClientPhones");

            migrationBuilder.DropTable(
                name: "ClientStatuses");

            migrationBuilder.DropTable(
                name: "ClientStatusHistories");

            migrationBuilder.DropTable(
                name: "CostCenters");

            migrationBuilder.DropTable(
                name: "DeviceDetails");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "DirectProviders");

            migrationBuilder.DropTable(
                name: "DispatcherShifts");

            migrationBuilder.DropTable(
                name: "EmergencyChainActions");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "InvoiceItems");

            migrationBuilder.DropTable(
                name: "LeadActivities");

            migrationBuilder.DropTable(
                name: "Medications");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "ProfessionalProviderClientLinks");

            migrationBuilder.DropTable(
                name: "ProfessionalProviderDetails");

            migrationBuilder.DropTable(
                name: "SalesOpportunities");

            migrationBuilder.DropTable(
                name: "SalesOrderItems");

            migrationBuilder.DropTable(
                name: "SipConfigurations");

            migrationBuilder.DropTable(
                name: "StockMovements");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "EmergencyContacts");

            migrationBuilder.DropTable(
                name: "EmergencyAlerts");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "ProfessionalProviders");

            migrationBuilder.DropTable(
                name: "Leads");

            migrationBuilder.DropTable(
                name: "SalesOrders");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Dispatchers");

            migrationBuilder.DropTable(
                name: "EmergencyDevices");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "ClientDetails");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "DirectProviderDetails");

            migrationBuilder.DropTable(
                name: "SystemEntries");

            migrationBuilder.DropTable(
                name: "Tarifs");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "VatTaxes");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
