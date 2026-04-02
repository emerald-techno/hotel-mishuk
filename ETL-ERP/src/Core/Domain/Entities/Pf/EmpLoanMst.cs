using Domain.Entities.HR;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Pf;

public class EmpLoanMst : IAuditable
{
    public long Id { get; set; }
    public DateTime LoanPassDate { get; set; }
    public DateTime LoanPayDate { get; set; }
    public double LoanAmount { get; set; }
    public double InterestRate { get; set; }
    public DateTime FirstInsDate { get; set; } // First Installment Date

    [StringLength(120)]
    public string LoanFileUrl { get; set; }
    public double InsAmount { get; set; }
    public short Status { get; set; } // 0=Running , 1=Complete, 2=Held


    [StringLength(350)]
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    /* ----------------- FK -------------------*/

    public long EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
    public ICollection<EmpLoanDtl> EmpLoanDtls { get; set; }

}
