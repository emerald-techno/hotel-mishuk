using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Payroll.SetSalaryGrade;

public class SetSalaryGradeVm
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Grade name is required")]
    [StringLength(50, ErrorMessage = "Grade name can not be more than 50 characters")]
    public string GradeName { get; set; }
    public double StartingBasic { get; set; }
    public double MaxAmount { get; set; }

    [StringLength(120, ErrorMessage = "Remarks can not be more than 120 characters")]
    public string Remarks { get; set; }

    //-----------------------------------------
}
