using Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class RsCustomerType
{
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string TypeName { get; set; } // Hotel, Online, Walk-in, Employee

    [Required]
    [StringLength(30)]
    public string TypeCode { get; set; }
    public double DiscountPercent { get; set; } = 0;
    public DateTime ActionDate { get; set; }
    public bool IsDeleted { get; set; }

    // --- Fk ---

    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
}
