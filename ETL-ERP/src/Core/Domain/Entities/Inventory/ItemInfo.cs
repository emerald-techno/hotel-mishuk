using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Inventory;

public class ItemInfo : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(90)]
    public string ItemName { get; set; }

    [Required]
    [StringLength(30)]
    public string ItemCode { get; set; }
    public double ReorderQty { get; set; }
    public double MaxQty { get; set; }
    public double MinQty { get; set; }
    public double AlertQty { get; set; }
    public short LeadDay { get; set; }

    [StringLength(120)]
    public string PhotoDocUrl { get; set; }

    [StringLength(350)]
    public string Remarks { get; set; }

    public long UnitId { get; set; }
    public virtual UnitInfo Unit { get; set; }
    public long CategoryId { get; set; }
    public virtual CategoryInfo Category { get; set; }

    public DateTime ActionDate { get; set; }
    public long ActionById { get; set; }
    public DateTime? UpdateDate { get; set; }
    public long? UpdatedById { get; set; }
    public bool IsDeleted { get; set; }

    public virtual ApplicationUser ActionBy { get; set; }
    public virtual ApplicationUser UpdatedBy { get; set; }
}
