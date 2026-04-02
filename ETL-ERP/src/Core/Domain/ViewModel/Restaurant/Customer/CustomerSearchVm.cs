using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Restaurant.Customer;

public class CustomerSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string CustomerCode { get; set; }
    public string Salutation { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
    public DateTime Dob { get; set; }
    public string Gender { get; set; } // M=Male,F=Female
    public string Address { get; set; }
    public string PhotoUrl { get; set; }
    public string Note { get; set; }

    // --- Fk ---
    public long? CompanyId { get; set; }
    public string CompanyName { get; set; }
    public long? EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public long? GuestId { get; set; }
    public string FullName { get; set; }
    public string GuestName { get; set; }
    public IEnumerable<SelectListItem> SalutationLookUp { get; set; }
    public IEnumerable<SelectListItem> GenderLookUp { get; set; }
    public IEnumerable<SelectListItem> GuestLookUp { get; set; }
    public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }
    public IEnumerable<SelectListItem> CompanyLookUp { get; set; }

    // --- Datatable ---

    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}
