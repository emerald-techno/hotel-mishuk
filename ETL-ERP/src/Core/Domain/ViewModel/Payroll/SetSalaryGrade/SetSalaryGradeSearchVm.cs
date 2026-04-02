namespace Domain.ViewModel.Payroll.SetSalaryGrade;

public class SetSalaryGradeSearchVm
{
    public long Id { get; set; }
    public string GradeName { get; set; }
    public double StartingBasic { get; set; }
    public double MaxAmount { get; set; }
    public string Remarks { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}
