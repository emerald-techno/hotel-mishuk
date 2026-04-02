namespace Domain.ViewModel.Payroll.PrGuestSalary;

public class PrGuestSalaryDtlVm
{
    public long Id { get; set; }
    public double Amount { get; set; }
    public string Remarks { get; set; }
    public long GuestSalaryMstId { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; }

}
