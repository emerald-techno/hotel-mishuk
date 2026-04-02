using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Inventory;

public class TranDtl : IAuditable
{
    public long Id { get; set; }
    public long SlNo { get; set; } = 1;
    public double ItemQty { get; set; }
    public double Stock { get; set; }
    public double OrderQty { get; set; }
    public double UnitPrice { get; set; } = 0;
    public bool IsQcPass { get; set; }

    [StringLength(350)]
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    // ---- FK ----

    public long TranMstId { get; set; }
    public virtual TranMst TranMst { get; set; }
    public long ItemId { get; set; }
    public virtual ItemInfo Item { get; set; }
    public long ItemUnitId { get; set; }
    public virtual UnitInfo ItemUnit { get; set; }
    public long? OrderDtlId { get; set; }
    public virtual OrderDtl OrderDtl { get; set; }
    public long? ReqDtlId { get; set; }
    public virtual RequsitionInfoDtl ReqDtl { get; set; }
    public long ActionById { get; set; }
    public virtual ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public virtual ApplicationUser UpdatedBy { get; set; }
}
