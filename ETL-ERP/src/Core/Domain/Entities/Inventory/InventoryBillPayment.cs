using Domain.Entities.Accounting;
using Domain.Entities.Identity;
using Domain.Enums.AppEnums;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Inventory;

public class InventoryBillPayment : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    public string BillNo { get; set; }
    public DateTime BillDate { get; set; }
    public double BillAmount { get; set; }
    public PayModeEnum PayMode { get; set; }
    public string BillFileUrl { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string ApprovedRemarks { get; set; }
    public bool IsAdvance { get; set; }
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }
    public long? OrderMstId { get; set; }
    public virtual OrderMst OrderMst { get; set; }
    public long? ReceiveId { get; set; }
    public virtual TranMst Receive { get; set; }
    public long BillById { get; set; }
    public virtual ApplicationUser BillBy { get; set; }
    public long? SupplierId { get; set; }
    public virtual SupplierInfo Supplier { get; set; }
    public long? PayModeDetailId { get; set; }
    public PayModeDetail PayModeDetail { get; set; }
    public long? ApprovedById { get; set; }
    public virtual ApplicationUser ApprovedBy { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
    public DateTime ReportDate { get; set; }
}
