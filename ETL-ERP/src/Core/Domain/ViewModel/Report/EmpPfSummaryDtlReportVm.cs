using Domain.ViewModel.Pf.PfFund;

namespace Domain.ViewModel.Report
{
    public class EmpPfSummaryDtlReportVm
    {
        public DateTime OpenningDate { get; set; }
        public DateTime PfStartDate { get; set; }
        public double EmpCon { get; set; }
        public double CompCon { get; set; }
        public double Interest { get; set; }
        public double TotalAmount { get; set; }
        public string Remarks { get; set; }

        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmpDesignation { get; set; }
        public string EmpDepartment { get; set; }
        public ICollection<PfFundDtlVm> PfFundDtlVms { get; set; }
    }
}
