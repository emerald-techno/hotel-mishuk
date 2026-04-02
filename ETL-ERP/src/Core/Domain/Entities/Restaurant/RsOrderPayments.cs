using Domain.Entities.Accounting;
using Domain.Entities.Identity;
using Domain.Enums.AppEnums;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class RsOrderPayments : IAuditable
{
    public long Id { get; set; }
    public DateTime PaidDate { get; set; }
    public double PaidAmount { get; set; } = 0;
    public PayModeEnum PayMode { get; set; }

    [StringLength(30)]
    public string TransactionNo { get; set; }

    [StringLength(30)]
    public string ChequeNo { get; set; }

    [StringLength(30)]
    public string AccountNo { get; set; }

    [StringLength(120)]
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }
    public RsOrderPaymentTypeEnum PaymentType { get; set; } // 0 = Receive, 1 = Refund
    public DateTime ReportDate { get; set; }

    [StringLength(250)]
    public string AuditRemarks { get; set; }
    public DateTime? AuditDate { get; set; }
    public bool IsAdvance { get; set; }

    // --- Fk ---

    public long OrderId { get; set; }
    public RsFoodOrder Order { get; set; }
    public long? BankId { get; set; }
    public long? BillDtlId { get; set; }
    public HtBillingDetail BillDtl { get; set; }
    public long? PayModeDetailId { get; set; }
    public PayModeDetail PayModeDetail { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
    public long? AuditById { get; set; }
    public ApplicationUser AuditBy { get; set; }
}
