using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Leave.EmpLeaveApplication
{
    public class EmpLeaveReportVm
    {
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmpDesignation { get; set; }
        public string EmpDepartment { get; set; }
        public DateTime EmpJoinDate { get; set; }
        public int MedicalLeave { get; set; }
        public int CasualLeave { get; set; }
        public int MaturnityLeave { get; set; }
        public int EarnLeave { get; set; }
        public int LeaveWithoutPay { get; set; }
        public int StudyLeave { get; set; }
        public int TotalLeave { get; set; }
        public string FormDateStr { get; set; }
        public string ToDateStr { get; set; }
        public long? EmpDepatmentId { get; set; }
        public long? EmpDesignationId { get; set; }
        public IEnumerable<SelectListItem> DepatmentLookup { get; set; }
        public IEnumerable<SelectListItem> DesignationLookup { get; set; }
    }

    public class EmpLeaveTypeLeaveReport
    {
        public string LeaveDateStr { get; set; }
        public string DayStr { get; set; }
        public DateTime LeaveDate { get; set; }
    }
}
