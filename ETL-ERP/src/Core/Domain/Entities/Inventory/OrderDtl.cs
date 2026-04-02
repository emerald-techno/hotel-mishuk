using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Inventory;

public class OrderDtl : IAuditable
{
    public long Id { get; set; }
    public double OrderQty { get; set; }
    public double AprOrderQty { get; set; }
    public double ActualAmount { get; set; }
    public double Stock { get; set; }
    public double Rate { get; set; }
    public long SlNo { get; set; } = 1;

    [StringLength(350)]
    public string Remarks { get; set; }
    public DateTime? LastOrderDate { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    //---- FK ----      

    public long OrderId { get; set; }
    public virtual OrderMst Order { get; set; }
    public long ItemId { get; set; }
    public virtual ItemInfo Item { get; set; }
    public long ItemUnitId { get; set; }
    public virtual UnitInfo ItemUnit { get; set; }
    public long? LastOrderId { get; set; }
    public virtual OrderMst LastOrder { get; set; }
    public long ActionById { get; set; }
    public virtual ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public virtual ApplicationUser UpdatedBy { get; set; }
}
