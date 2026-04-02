using Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Restaurant.Waiter;

public class WaiterVm
{
    public long Id { get; set; }

    [StringLength(90, ErrorMessage = "Waiter name can not be more than 90 characters")]
    [Required(ErrorMessage = "Waiter name is required")]
    public string Name { get; set; }

    [StringLength(30, ErrorMessage = "Waiter code can not be more than 30 characters")]
    [Required(ErrorMessage = "Waiter code is required")]
    public string Code { get; set; }

    [StringLength(20, ErrorMessage = "Waiter mobile number can not be more than 20 characters")]
    [Required(ErrorMessage = "Waiter mobile number is required")]
    public string Mobile { get; set; }

    [StringLength(35, ErrorMessage = "Waiter email can not be more than 35 characters")]
    public string Email { get; set; }

    public double Salary { get; set; }
    public DateTime? Dob { get; set; }
    public string DobStr { get; set; }
    public DateTime? JoinDate { get; set; }
    public string JoinDateStr { get; set; }
    public DateTime ActionDate { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }

    // --- Fk ---

    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
}