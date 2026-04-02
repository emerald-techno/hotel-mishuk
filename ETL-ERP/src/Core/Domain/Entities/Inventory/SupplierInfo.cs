using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Inventory;

public class SupplierInfo : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string SupplierName { get; set; }

    [Required]
    [StringLength(40)]
    public string SupplierCode { get; set; }

    [StringLength(40)]
    public string Mobile { get; set; }

    [StringLength(40)]
    public string Phone { get; set; }

    [StringLength(40)]
    public string Email { get; set; }

    [StringLength(180)]
    public string Address { get; set; }

    [StringLength(120)]
    public string PhotoDoc { get; set; }

    [StringLength(350)]
    public string Remarks { get; set; } = string.Empty;


    public DateTime ActionDate { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public DateTime? UpdateDate { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}
