using Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class RsFoodType
{
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string TypeName { get; set; } // Breakfast, Lunch, Dinner

    [StringLength(120)]
    public string PhotoUrl { get; set; }
    public DateTime ActionDate { get; set; }
    public bool IsDeleted { get; set; }

    // --- Fk ---

    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
}
