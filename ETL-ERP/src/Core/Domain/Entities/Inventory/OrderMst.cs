using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Inventory;

public class OrderMst : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(30)]
    public string OrderNo { get; set; }
    public DateTime OrderDate { get; set; }

    [Required]
    [StringLength(1)]
    public string OrderType { get; set; } // I=Item, S=Service
    public string OrderMode { get; set; } // Cheque, Cash etc

    [StringLength(150)]
    public string Remarks { get; set; }
    public short Status { get; set; } // 0=FRESH, 1 = REVIEW, 2=REJECTED, 3 = APPROVED
    public short ReceiveStatus { get; set; } // 0=NOT, 1=PARTIAL, 2=FULL, 3=FOURCE FULL
    public DateTime? DeliveryDeadline { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? CompleteDate { get; set; }

    [StringLength(250)]
    public string CompleteRemarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    //---- FK -----

    public long? ReqId { get; set; }
    public virtual RequsitionInfo Req { get; set; }
    public long SupplierId { get; set; }
    public virtual SupplierInfo Supplier { get; set; }
    public long? ApprovedById { get; set; }
    public virtual ApplicationUser ApprovedBy { get; set; }
    public long ActionById { get; set; }
    public virtual ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public virtual ApplicationUser UpdatedBy { get; set; }
    public virtual ICollection<OrderDtl> OrderDtls { get; set; }
}
