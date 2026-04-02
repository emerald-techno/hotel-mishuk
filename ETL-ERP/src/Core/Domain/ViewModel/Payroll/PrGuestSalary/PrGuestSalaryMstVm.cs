using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Payroll.PrGuestSalary;

public class PrGuestSalaryMstVm
{
    public long Id { get; set; }
    public short Year { get; set; }
    public short Month { get; set; }

    [StringLength(120)]
    public string Remarks { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string ApprovedRemarks { get; set; }
    public long? ApprovedById { get; set; }
    public string ApprovedByFullName { get; set; }
    public long EmployeeId { get; set; }
    public IEnumerable<SelectListItem> EmployeeLookup { get; set; }
    public IEnumerable<SelectListItem> YearLookup { get; set; }
    public IEnumerable<SelectListItem> MonthLookup { get; set; }
    public ICollection<PrGuestSalaryDtlVm> PrGuestSalaryDtls { get; set; }

}
