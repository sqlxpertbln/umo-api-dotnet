using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMOApi.Models;

#region Marketing & CRM Models

/// <summary>
/// Represents a marketing campaign.
/// </summary>
public class Campaign
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(200)]
    public string? Name { get; set; }
    
    [MaxLength(50)]
    public string? Type { get; set; } // Email, Social, Print, Event, Referral
    
    [MaxLength(50)]
    public string? Status { get; set; } // Planned, Active, Paused, Completed
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public decimal? Budget { get; set; }
    
    public decimal? ActualCost { get; set; }
    
    public int? TargetLeads { get; set; }
    
    public int? ActualLeads { get; set; }
    
    public int? ConvertedLeads { get; set; }
    
    [MaxLength(100)]
    public string? TargetAudience { get; set; }
    
    [MaxLength(100)]
    public string? Channel { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public virtual ICollection<Lead>? Leads { get; set; }
}

/// <summary>
/// Represents a lead/prospect in the CRM system.
/// </summary>
public class Lead
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(100)]
    public string? FirstName { get; set; }
    
    [MaxLength(100)]
    public string? LastName { get; set; }
    
    [MaxLength(200)]
    public string? Email { get; set; }
    
    [MaxLength(50)]
    public string? Phone { get; set; }
    
    [MaxLength(200)]
    public string? Company { get; set; }
    
    [MaxLength(50)]
    public string? Source { get; set; } // Website, Referral, Campaign, Cold Call, Event
    
    [MaxLength(50)]
    public string? Status { get; set; } // New, Contacted, Qualified, Proposal, Negotiation, Won, Lost
    
    [MaxLength(50)]
    public string? QualificationScore { get; set; } // Hot, Warm, Cold
    
    public int? Score { get; set; } // 0-100 Lead Score
    
    public decimal? EstimatedValue { get; set; }
    
    public DateTime? FirstContactDate { get; set; }
    
    public DateTime? LastContactDate { get; set; }
    
    public DateTime? ExpectedCloseDate { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public int? CampaignId { get; set; }
    [ForeignKey("CampaignId")]
    public virtual Campaign? Campaign { get; set; }
    
    public int? AssignedToUserId { get; set; }
    
    [MaxLength(100)]
    public string? AssignedToName { get; set; }
    
    public int? ConvertedClientId { get; set; }
    
    public DateTime? ConversionDate { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public virtual ICollection<LeadActivity>? Activities { get; set; }
}

/// <summary>
/// Represents an activity/interaction with a lead.
/// </summary>
public class LeadActivity
{
    [Key]
    public int Id { get; set; }
    
    public int LeadId { get; set; }
    [ForeignKey("LeadId")]
    public virtual Lead? Lead { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(50)]
    public string? Type { get; set; } // Call, Email, Meeting, Demo, Proposal
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public DateTime? ActivityDate { get; set; }
    
    [MaxLength(50)]
    public string? Outcome { get; set; } // Positive, Neutral, Negative
    
    public int? Duration { get; set; } // in minutes
    
    [MaxLength(100)]
    public string? PerformedBy { get; set; }
    
    public DateTime? NextFollowUp { get; set; }
}

#endregion

#region Sales Models

/// <summary>
/// Represents a sales opportunity.
/// </summary>
public class SalesOpportunity
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(200)]
    public string? Name { get; set; }
    
    public int? LeadId { get; set; }
    [ForeignKey("LeadId")]
    public virtual Lead? Lead { get; set; }
    
    public int? ClientId { get; set; }
    
    [MaxLength(50)]
    public string? Stage { get; set; } // Qualification, Needs Analysis, Proposal, Negotiation, Closed Won, Closed Lost
    
    public decimal? Amount { get; set; }
    
    public int? Probability { get; set; } // 0-100%
    
    public DateTime? ExpectedCloseDate { get; set; }
    
    public DateTime? ActualCloseDate { get; set; }
    
    [MaxLength(100)]
    public string? ProductInterest { get; set; }
    
    [MaxLength(100)]
    public string? CompetitorInfo { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    [MaxLength(100)]
    public string? AssignedToName { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
}

/// <summary>
/// Represents a sales order/contract.
/// </summary>
public class SalesOrder
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(50)]
    public string? OrderNumber { get; set; }
    
    public int? ClientId { get; set; }
    [ForeignKey("ClientId")]
    public virtual ClientDetails? Client { get; set; }
    
    public int? OpportunityId { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; } // Draft, Pending, Approved, Fulfilled, Cancelled
    
    public DateTime? OrderDate { get; set; }
    
    public DateTime? DeliveryDate { get; set; }
    
    public decimal? SubTotal { get; set; }
    
    public decimal? Tax { get; set; }
    
    public decimal? Discount { get; set; }
    
    public decimal? TotalAmount { get; set; }
    
    [MaxLength(50)]
    public string? PaymentTerms { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public virtual ICollection<SalesOrderItem>? Items { get; set; }
}

/// <summary>
/// Represents an item in a sales order.
/// </summary>
public class SalesOrderItem
{
    [Key]
    public int Id { get; set; }
    
    public int SalesOrderId { get; set; }
    [ForeignKey("SalesOrderId")]
    public virtual SalesOrder? SalesOrder { get; set; }
    
    public int? ArticleId { get; set; }
    [ForeignKey("ArticleId")]
    public virtual Article? Article { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal? Discount { get; set; }
    
    public decimal TotalPrice { get; set; }
    
    [MaxLength(200)]
    public string? Description { get; set; }
}

#endregion

#region ERP Models

/// <summary>
/// Represents an article/product in the ERP system.
/// </summary>
public class Article
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(50)]
    public string? ArticleNumber { get; set; }
    
    [MaxLength(200)]
    public string? Name { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(50)]
    public string? Category { get; set; } // Device, Medication, Supply, Service
    
    [MaxLength(50)]
    public string? SubCategory { get; set; }
    
    [MaxLength(50)]
    public string? Unit { get; set; } // Piece, Box, Package, Hour
    
    public decimal? PurchasePrice { get; set; }
    
    public decimal? SalesPrice { get; set; }
    
    public decimal? VatRate { get; set; }
    
    public int? MinStock { get; set; }
    
    public int? MaxStock { get; set; }
    
    public int? ReorderPoint { get; set; }
    
    [MaxLength(100)]
    public string? Supplier { get; set; }
    
    [MaxLength(50)]
    public string? SupplierArticleNumber { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool RequiresPrescription { get; set; }
    
    [MaxLength(50)]
    public string? StorageConditions { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
}

/// <summary>
/// Represents inventory/stock information.
/// </summary>
public class Inventory
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    public int ArticleId { get; set; }
    [ForeignKey("ArticleId")]
    public virtual Article? Article { get; set; }
    
    [MaxLength(100)]
    public string? WarehouseLocation { get; set; }
    
    public int CurrentStock { get; set; }
    
    public int ReservedStock { get; set; }
    
    public int AvailableStock { get; set; }
    
    [MaxLength(50)]
    public string? BatchNumber { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
    
    public DateTime? LastStockTake { get; set; }
    
    public DateTime? LastMovementDate { get; set; }
}

/// <summary>
/// Represents a stock movement/transaction.
/// </summary>
public class StockMovement
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    public int ArticleId { get; set; }
    [ForeignKey("ArticleId")]
    public virtual Article? Article { get; set; }
    
    [MaxLength(50)]
    public string? MovementType { get; set; } // In, Out, Transfer, Adjustment
    
    public int Quantity { get; set; }
    
    [MaxLength(50)]
    public string? Reference { get; set; } // Order number, delivery note, etc.
    
    [MaxLength(200)]
    public string? Reason { get; set; }
    
    [MaxLength(100)]
    public string? FromLocation { get; set; }
    
    [MaxLength(100)]
    public string? ToLocation { get; set; }
    
    public DateTime MovementDate { get; set; }
    
    [MaxLength(100)]
    public string? PerformedBy { get; set; }
}

/// <summary>
/// Represents a medication in the system.
/// </summary>
public class Medication
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(200)]
    public string? Name { get; set; }
    
    [MaxLength(200)]
    public string? GenericName { get; set; }
    
    [MaxLength(100)]
    public string? Manufacturer { get; set; }
    
    [MaxLength(50)]
    public string? DosageForm { get; set; } // Tablet, Capsule, Liquid, Injection
    
    [MaxLength(50)]
    public string? Strength { get; set; }
    
    [MaxLength(50)]
    public string? PackageSize { get; set; }
    
    public decimal? Price { get; set; }
    
    public bool RequiresPrescription { get; set; }
    
    [MaxLength(50)]
    public string? StorageConditions { get; set; }
    
    [MaxLength(500)]
    public string? Contraindications { get; set; }
    
    public int? ArticleId { get; set; }
    [ForeignKey("ArticleId")]
    public virtual Article? Article { get; set; }
    
    public bool IsActive { get; set; } = true;
}

#endregion

#region Billing & Finance Models

/// <summary>
/// Represents an invoice.
/// </summary>
public class Invoice
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(50)]
    public string? InvoiceNumber { get; set; }
    
    public int? ClientId { get; set; }
    [ForeignKey("ClientId")]
    public virtual ClientDetails? Client { get; set; }
    
    public int? SalesOrderId { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; } // Draft, Sent, Paid, Overdue, Cancelled
    
    public DateTime? InvoiceDate { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public DateTime? PaidDate { get; set; }
    
    public decimal? SubTotal { get; set; }
    
    public decimal? Tax { get; set; }
    
    public decimal? Discount { get; set; }
    
    public decimal? TotalAmount { get; set; }
    
    public decimal? PaidAmount { get; set; }
    
    public decimal? OutstandingAmount { get; set; }
    
    [MaxLength(50)]
    public string? PaymentMethod { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public int? ReminderCount { get; set; }
    
    public DateTime? LastReminderDate { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
    
    public virtual ICollection<InvoiceItem>? Items { get; set; }
    public virtual ICollection<Payment>? Payments { get; set; }
}

/// <summary>
/// Represents an item on an invoice.
/// </summary>
public class InvoiceItem
{
    [Key]
    public int Id { get; set; }
    
    public int InvoiceId { get; set; }
    [ForeignKey("InvoiceId")]
    public virtual Invoice? Invoice { get; set; }
    
    public int? ArticleId { get; set; }
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public decimal? VatRate { get; set; }
    
    public decimal? Discount { get; set; }
    
    public decimal TotalPrice { get; set; }
    
    [MaxLength(50)]
    public string? ServicePeriod { get; set; }
}

/// <summary>
/// Represents a payment received.
/// </summary>
public class Payment
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    public int? InvoiceId { get; set; }
    [ForeignKey("InvoiceId")]
    public virtual Invoice? Invoice { get; set; }
    
    public int? ClientId { get; set; }
    
    [MaxLength(50)]
    public string? PaymentNumber { get; set; }
    
    public decimal Amount { get; set; }
    
    public DateTime PaymentDate { get; set; }
    
    [MaxLength(50)]
    public string? PaymentMethod { get; set; } // Bank Transfer, Credit Card, Cash, Direct Debit
    
    [MaxLength(100)]
    public string? Reference { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; } // Pending, Completed, Failed, Refunded
    
    [MaxLength(200)]
    public string? Notes { get; set; }
    
    public DateTime? CreateDate { get; set; }
    
    [MaxLength(50)]
    public string? CreateId { get; set; }
}

/// <summary>
/// Represents a cost center for financial tracking.
/// </summary>
public class CostCenter
{
    [Key]
    public int Id { get; set; }
    
    public int MandantId { get; set; }
    
    [MaxLength(50)]
    public string? Code { get; set; }
    
    [MaxLength(200)]
    public string? Name { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(100)]
    public string? Department { get; set; }
    
    public decimal? Budget { get; set; }
    
    public decimal? ActualSpend { get; set; }
    
    public bool IsActive { get; set; } = true;
}

#endregion
