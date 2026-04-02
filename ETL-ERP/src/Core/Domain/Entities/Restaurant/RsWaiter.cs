using Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class RsWaiter
{
    public long Id { get; set; }

    [Required]
    [StringLength(90)]
    public string Name { get; set; }

    [Required]
    [StringLength(30)]
    public string Code { get; set; }

    [Required]
    [StringLength(20)]
    public string Mobile { get; set; }

    public string? Email { get; set; }
    public double? Salary { get; set; }
    public DateTime? Dob { get; set; }
    public DateTime? JoinDate { get; set; }
    public DateTime ActionDate { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }

    // --- Fk ---

    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
}
