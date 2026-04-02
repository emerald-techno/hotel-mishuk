using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Leave.EmpLeaveApplication
{
    public class EmpLeaveBalanceReportVm
    {
        public long EmployeeId { get; set; }
        public int SelectYear { get; set; }
        public string EmployeeName { get; set; }
        public string EmpDesignation { get; set; }
        public string EmpDepartment { get; set; }
        public string EmpPhotoUrl { get; set; }
        public string Code { get; set; }
        public DateTime JoinDate { get; set; }
        public int EmployeeStatus { get; set; } // 1 = Permanent, 2 = Contractual, 3 = Adhoc, 4 = Guest
        public string EmployeeStatusText => EmployeeStatus switch { 1 => "Permanent", 2 => "Contractual", 3 => "Adhoc", 4 => "Guest", _ => "--" };
        public string Gender { get; set; } //M = Male, F = Female
        public string GenderText => Gender switch { "M" => "Male", "F" => "Female", _ => "--" };
        public ICollection<LeaveBalanceVm> LeaveBalanceVms { get; set; }
        public IEnumerable<SelectListItem> YearLookUp { get; set; }
        public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }
    }

    public class LeaveBalanceVm
    {
        public long LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public int LeaveBalance { get; set; }
        public int CurrentYearBalance { get; set; }
        public int LeaveTaken { get; set; }
        public int LeaveRemain { get; set; }
        public int CarryForwardBalance { get; set; }
    }
}
