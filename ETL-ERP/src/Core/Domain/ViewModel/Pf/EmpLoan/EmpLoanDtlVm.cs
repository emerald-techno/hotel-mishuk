namespace Domain.ViewModel.Pf.EmpLoan;

public class EmpLoanDtlVm
{
    public long Id { get; set; }
    public DateTime InsDate { get; set; }
    public string InsDateStr { get; set; }
    public double InsAmount { get; set; }
    public double InterestAmount { get; set; }
    public double LoanAmount { get; set; }
    public bool IsPaid { get; set; }
    public short Serial { get; set; } // Default = 1
    public bool IsDeleted { get; set; }
    public long LoanId { get; set; }
    public long? PrDtlId { get; set; }
}
