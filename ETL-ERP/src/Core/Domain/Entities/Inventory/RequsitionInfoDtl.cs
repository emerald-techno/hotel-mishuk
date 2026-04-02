using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Inventory;

public class RequsitionInfoDtl : IAuditable
{
    public long Id { get; set; }
    public double ReqQty { get; set; }
    public double Stock { get; set; }
    public double? AprReqQty { get; set; }
    public double IssueQty { get; set; }
    public long SlNo { get; set; }

    [StringLength(150)]
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    //---- FK ----      

    public long ReqId { get; set; }
    public virtual RequsitionInfo Req { get; set; }
    public long ItemId { get; set; }
    public virtual ItemInfo Item { get; set; }
    public long ItemUnitId { get; set; }
    public virtual UnitInfo ItemUnit { get; set; }
    public long? LastReqId { get; set; }
    public virtual RequsitionInfo LastReq { get; set; }
    public long ActionById { get; set; }
    public virtual ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public virtual ApplicationUser UpdatedBy { get; set; }
}
