using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Payroll.PrArrearMst;

public class PrArrearMstVm
{
    public long Id { get; set; }
    public short Year { get; set; }
    public short Month { get; set; }
    public string Remarks { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string ApprovedRemarks { get; set; }

    //-----------------------------------------
    public long? ApprovedById { get; set; }
    public string ApprovedByFullName { get; set; }
    public IEnumerable<SelectListItem> EmployeeLookup { get; set; }
    public IEnumerable<SelectListItem> YearLookup { get; set; }
    public IEnumerable<SelectListItem> MonthLookup { get; set; }
    public ICollection<PrArrearDtlVm> PrArrearDtls { get; set; }
}
