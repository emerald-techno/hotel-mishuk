using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class RsFoodItem : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string ItemName { get; set; } // Bengali, Indian, Chinese

    [Required]
    [StringLength(50)]
    public string ItemCode { get; set; }

    [StringLength(120)]
    public string PhotoUrl { get; set; }

    [StringLength(300)]
    public string Description { get; set; }
    public double VAT { get; set; } = 0;
    public double Rate { get; set; } = 0;
    public double OfferRate { get; set; } = 0;
    public double NetRate { get; set; } = 0;

    [StringLength(120)]
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsSetMenuItem { get; set; } = false;
    public bool IsDeleted { get; set; }

    // --- Fk ---

    public long CategoryId { get; set; }
    public RsFoodCategory Category { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
