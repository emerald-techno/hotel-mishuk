using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Payroll.PrSalaryPart;

public class PrSalaryPartVm
{
    public long Id { get; set; }


    [StringLength(60, ErrorMessage = "Part Name can not be more than 60 characters")]
    [Required(ErrorMessage = "Part Name required")]
    [DisplayName("Part Name: ")]
    public string PartName { get; set; }


    [StringLength(60, ErrorMessage = "Part Code can not be more than 60 characters")]
    [Required(ErrorMessage = "Part Code required")]
    [DisplayName("Part Code: ")]
    public string PartCode { get; set; }


    [StringLength(1)]
    [Required(ErrorMessage = "Part Type required")]
    [DisplayName("Part Type: ")]
    public string PartType { get; set; }
    public string PartTypeText => PartType switch { "A" => "Addition", "D" => "Deduction", _ => "--" };


    [StringLength(1)]
    [Required(ErrorMessage = "Value type required")]
    [DisplayName("Value Type: ")]
    public string ValueType { get; set; }

    public string ValueTypeText => ValueType switch { "P" => "Percent", "A" => "Amount", _ => "--" };

    [DisplayName("Value: ")]
    public double Value { get; set; }
    public string PartLink { get; set; } // A=Arrear, L=Loan, B=Basic

    [DisplayName("Is Employee Wise ?: ")]
    public bool IsEmpWise { get; set; }

    [DisplayName("Is Enable ?: ")]
    public bool IsEnable { get; set; }
    public short SlNo { get; set; }


    [StringLength(120, ErrorMessage = "Remarks can not be more tha 120 characters")]
    [DisplayName("Remarks: ")]
    public string Remarks { get; set; }

    public long ActionById { get; set; }
    public DateTime ActionDate { get; set; }

    public IEnumerable<SelectListItem> ValueTypeLookUp { get; set; }
    public IEnumerable<SelectListItem> PartTypeLookUp { get; set; }

}
