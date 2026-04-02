using Domain.ModelInterface;

namespace Domain.ViewModel.Report
{
    public class EmployeePfSummaryReportVm : IDataTableSearch
    {
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmpDesignation { get; set; }
        public string EmpDepartment { get; set; }
        public double TotalEmpCon { get; set; }
        public double TotalCompCon { get; set; }
        public double TotalInterest { get; set; }
        public double TotalPfAmount { get; set; }
        public long Id { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
