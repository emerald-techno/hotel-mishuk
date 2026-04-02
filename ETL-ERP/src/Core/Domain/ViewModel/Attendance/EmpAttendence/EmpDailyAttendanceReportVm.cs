using Domain.Entities.Identity;
using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Attendance.EmpAttendence
{
    public class EmpDailyAttendanceReportVm : IDataTableSearch
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public bool Present { get; set; }
        public bool Absent { get; set; }
        public bool Leave { get; set; }
        public bool Holiday { get; set; }
        public bool OffDay { get; set; }
        public bool Late { get; set; }
        public bool NoEntry { get; set; }
        public DateTime? InTime { get; set; }
        public string InTimeStr { get; set; }
        public DateTime? OutTime { get; set; }
        public string OutTimeStr { get; set; }
        public DateTime? LunchOut { get; set; }
        public DateTime? LunchIn { get; set; }
        public DateTime AttendDate { get; set; }
        public string AttendDateStr { get; set; }
        public string FormDateStr { get; set; }
        public string SearchDateStr { get; set; }
        public string ToDateStr { get; set; }
        public string Status { get; set; } // P = Present, A = Absent, L = Leave, H = Holiday, O = Offday
        public string StatusText => Status switch { "P" => "Present", "A" => "Absent", "L" => "Leave", "H" => "Holiday", "O" => "Off Day", "N" => "No Entry", _ => "--" };

        public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }
        public IEnumerable<SelectListItem> DesignationLookUp { get; set; }
        public IEnumerable<SelectListItem> DepartmentLookUp { get; set; }

        //----------------------FK------------------------- 

        public long DesignationId { get; set; }
        //public string DesignationName { get; set; }
        public long? DepartmentId { get; set; }
        //public string DepartmentName { get; set; }
        public string EmpDepartment { get; set; }
        public string EmpDesignation { get; set; }
        public string Mobile { get; set; }
        public ApplicationUser ActionBy { get; set; }
        public long ActionById { get; set; }
        public DateTime ActionDate { get; set; }
        public bool IsDeleted { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
        public bool IsDayWise { get; set; } = false;

    }
}
