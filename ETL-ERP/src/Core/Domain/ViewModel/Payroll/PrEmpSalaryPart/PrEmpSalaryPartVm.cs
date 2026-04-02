using Domain.ViewModel.Payroll.PrSalaryPart;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Payroll.PrEmpSalaryPart;

public class PrEmpSalaryPartVm
{
    public long Id { get; set; }


    [StringLength(1)]
    [DisplayName("Part Type: ")]
    public string PartType { get; set; }
    public string PartTypeText => PartType switch { "A" => "Addition", "D" => "Deduction", _ => "--" };


    [StringLength(1)]
    [DisplayName("Value Type: ")]
    public string ValueType { get; set; }
    public string ValueTypeText => ValueType switch { "P" => "Percent", "A" => "Amount", _ => "--" };



    [DisplayName("Value: ")]
    public double Value { get; set; }



    [StringLength(120, ErrorMessage = "Remarks can not be more than 120 characters")]
    [DisplayName("Remarks: ")]
    public string Remarks { get; set; }

    //-------------------FK----------------------
    public long SalaryPartId { get; set; }
    public string SalaryPartName { get; set; }
    public string EmpPartName { get; set; }
    public string EmpPartCode { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string EmployeeCode { get; set; }
    public string EmpPhotoUrl { get; set; }

    public ICollection<PrSalaryPartVm> PrSalaryParts { get; set; }
    public ICollection<PrEmpSalaryPartVm> PrEmpSalaryParts { get; set; }

    public IEnumerable<SelectListItem> EmployeeLookup { get; set; }
    public IEnumerable<SelectListItem> ValueTypeLookUp { get; set; }
    public IEnumerable<SelectListItem> PartTypeLookUp { get; set; }
    public IEnumerable<SelectListItem> SalaryPartLookUp { get; set; }


}
