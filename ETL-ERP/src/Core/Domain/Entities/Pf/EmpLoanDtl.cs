using Domain.Entities.Identity;
using Domain.Entities.Payroll;

namespace Domain.Entities.Pf;

public class EmpLoanDtl
{
    public long Id { get; set; }
    public DateTime InsDate { get; set; }
    public double InsAmount { get; set; }
    public double InterestAmount { get; set; }
    public double LoanAmount { get; set; }
    public double WaiverAmount { get; set; } = 0;
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
    public double PaidAmount { get; set; } = 0;
    public string Remarks { get; set; }  
    public short Serial { get; set; } // Default = 1
    public bool IsDeleted { get; set; }
    public long LoanId { get; set; }
    public EmpLoanMst Loan { get; set; }
    public long? PrDtlId { get; set; }
    public PrSalaryDtl PrDtl { get; set; }
    public long? PaidSetById { get; set; }
    public ApplicationUser PaidSetBy { get; set; }
}
