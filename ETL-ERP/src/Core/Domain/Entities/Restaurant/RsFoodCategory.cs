using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class RsFoodCategory : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string CategoryName { get; set; } // Bengali, Indian, Chinese

    [StringLength(120)]
    public string PhotoUrl { get; set; } = string.Empty;

    [StringLength(120)]
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }

    // --- Fk ---
    public long? ParentCategoryId { get; set; }
    public RsFoodCategory ParentCategory { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
