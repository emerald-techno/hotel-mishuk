using Domain.Entities.Accounting;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Inventory;

public class CategoryInfo : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string CategoryName { get; set; }

    [Required]
    [StringLength(40)]
    public string CategoryCode { get; set; }

    [Required]
    [StringLength(1)]
    public string CategoryType { get; set; }

    [StringLength(120)]
    public string PhotoDocUrl { get; set; }

    [StringLength(350)]
    public string Remarks { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }

    public long? LedgerId { get; set; }

    public AccLedger Ledger { get; set; }
}
