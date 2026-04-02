using Domain.Entities.HR;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class RsCustomer : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    public string CustomerCode { get; set; }

    [Required]
    [StringLength(20)]
    public string Salutation { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; }

    [StringLength(100)]
    public string LastName { get; set; }

    [Required]
    [StringLength(35)]
    public string Mobile { get; set; }

    [StringLength(40)]
    public string Email { get; set; }
    public DateTime? Dob { get; set; }

    [StringLength(1)]
    public string Gender { get; set; } // M=Male,F=Female

    [StringLength(150)]
    public string Address { get; set; }

    [StringLength(120)]
    public string PhotoUrl { get; set; }
    
    [StringLength(150)]
    public string Note { get; set; }

    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    // --- Fk ---
    public long? CompanyId { get; set; }
    public ClientCompany Company { get; set; }
    public long? EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public long? GuestId { get; set; }
    public HtGuestInfo Guest { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}