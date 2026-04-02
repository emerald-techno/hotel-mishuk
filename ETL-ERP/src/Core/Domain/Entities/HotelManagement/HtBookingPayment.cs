using Domain.Entities.Accounting;
using Domain.Entities.Identity;
using Domain.Enums.AppEnums;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class HtBookingPayment : IAuditable
{
    public long Id { get; set; }
    public DateTime PaidDate { get; set; }
    public double PaidAmount { get; set; }
    public PayModeEnum PayMode { get; set; }

    [StringLength(30)]
    public string TransactionNo { get; set; }

    [StringLength(30)]
    public string ChequeNo { get; set; }

    [StringLength(30)]
    public string AccountNo { get; set; }

    [StringLength(250)]
    public string Description { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? ReportDate { get; set; }

    [StringLength(250)]
    public string AuditRemarks { get; set; }
    public DateTime? AuditDate { get; set; }
    public bool IsAdvance { get; set; }

    // --- Fk ---

    public long BookingId { get; set; }
    public HtBookingService Booking { get; set; }
    public long? BillingId { get; set; }
    public HtBilling Billing { get; set; }
    public long? BankId { get; set; }
    public long? PayModeDetailId { get; set; }
    public PayModeDetail PayModeDetail { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
    public long? AuditById { get; set; }
    public ApplicationUser AuditBy { get; set; }
}
