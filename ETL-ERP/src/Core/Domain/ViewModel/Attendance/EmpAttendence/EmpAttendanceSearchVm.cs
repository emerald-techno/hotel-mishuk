using Domain.ModelInterface;

namespace Domain.ViewModel.Attendance.EmpAttendence
{
    public class EmpAttendanceSearchVm : IDataTableSearch
    {
        public int Id { get; set; }
        public DateTime AttendDate { get; set; }
        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }
        public DateTime? LunchOut { get; set; }
        public DateTime? LunchIn { get; set; }
        public string Status { get; set; }
        public string StatusDtl { get; set; }
        public bool IsManual { get; set; }
        public DateTime? ManualEntrydate { get; set; }

        public DateTime? FormDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string FormDateStr { get; set; }
        public string ToDateStr { get; set; }

        //----------------------------------------------------
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmpDepartment { get; set; }
        public string EmpDesignation { get; set; }

        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
        long IDataTableSearch.Id { get; set; }
    }
}
