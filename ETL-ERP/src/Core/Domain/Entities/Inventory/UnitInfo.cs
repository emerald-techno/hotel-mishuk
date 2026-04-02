using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Inventory;

public class UnitInfo : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(40)]
    public string UnitName { get; set; }

    [Required]
    [StringLength(40)]
    public string UnitCode { get; set; }

    [StringLength(120)]
    public string Remarks { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime ActionDate { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public DateTime? UpdateDate { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
