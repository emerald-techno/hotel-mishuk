using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Attendance.EmpAttendence
{
    public class EmpAttendanceReportVm
    {
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmpDepartment { get; set; }
        public string EmpDesignation { get; set; }
        public long EmpPresent { get; set; }
        public long EmpAbsent { get; set; }
        public long EmpLeave { get; set; }
        public long EmpHoliday { get; set; }
        public long EmpOffDay { get; set; }
        public long EmpLateDay { get; set; }
        public string FormDateStr { get; set; }
        public string ToDateStr { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Status { get; set; }
        public IEnumerable<SelectListItem> YearLookUp { get; set; }
        public IEnumerable<SelectListItem> MonthLookUp { get; set; }
        public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }
    }
}
