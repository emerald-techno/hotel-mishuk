using Domain.ViewModel.Payroll.PrSalaryDtl;
using Domain.ViewModel.Payroll.PrSalaryPart;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Payroll.PrSalaryMst;

public class PrSalaryMstVm
{
    public long Id { get; set; }


    [DisplayName("Salary Date: ")]
    public string SalaryDateStr { get; set; }
    public DateTime SalaryDate { get; set; }

    public int Year { get; set; }
    public int Month { get; set; }


    [DisplayName("From Date: ")]
    public string DateFromStr { get; set; }
    public DateTime DateFrom { get; set; }


    [DisplayName("To Date: ")]
    public string DateToStr { get; set; }
    public DateTime DateTo { get; set; }



    [DisplayName("Total Employee: ")]
    public int TotalEmployee { get; set; }

    [DisplayName("Total Salary: ")]
    public double TotalSalary { get; set; }


    [StringLength(120, ErrorMessage = "Remarks can not be more than 120 characters")]
    [DisplayName("Remarks: ")]
    public string Remarks { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public long? ApprovedById { get; set; }
    public long? DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public ICollection<PrSalaryPartVm> EnablePrSalaryParts { get; set; }
    public ICollection<PrSalaryDtlVm> PrSalaryDtls { get; set; }
    public virtual IEnumerable<SelectListItem> DepartmentLookup { get; set; }
}
