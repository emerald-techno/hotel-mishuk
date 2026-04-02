using Domain.Entities.HR;

namespace Domain.Entities.Pf
{
    public class PfFundDtl
    {
        public long Id { get; set; }
        public double EmpCon { get; set; }
        public double CompCon { get; set; }
        public double Interest { get; set; }
        public double TotalAmount { get; set; }
        public double? EmpSalary { get; set; }
        public bool IsDeleted { get; set; }

        //---FK---
        public long FundMstId { get; set; }
        public PfFundMst FundMst { get; set; }
        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }

    }
}
