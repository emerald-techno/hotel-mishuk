namespace Domain.ViewModel.Payroll.PrArrearMst;

public class PrArrearDtlVm
{
    public long Id { get; set; }
    public double Amount { get; set; }
    public string Remarks { get; set; }
    public long ArrearId { get; set; }
    public string ArrearFor { get; set; } //N=Normal,S=SALARY,L=LEAVE
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
}
