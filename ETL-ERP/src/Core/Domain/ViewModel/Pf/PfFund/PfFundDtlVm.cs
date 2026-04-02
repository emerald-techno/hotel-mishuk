namespace Domain.ViewModel.Pf.PfFund
{
    public class PfFundDtlVm
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
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmpDesignation { get; set; }
        public string EmpDepartment { get; set; }
    }
}
