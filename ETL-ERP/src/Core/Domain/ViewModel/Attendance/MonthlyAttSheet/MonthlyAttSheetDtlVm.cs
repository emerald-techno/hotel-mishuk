using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Attendance.MonthlyAttSheet
{
    public class MonthlyAttSheetDtlVm
    {
        public long Id { get; set; }
        public short PresentDays { get; set; }
        public short AbsentDays { get; set; }
        public short LeaveDays { get; set; }
        public short UnPaidLeaveDays { get; set; }
        public short OffDays { get; set; }
        public short Holidays { get; set; }
        public short LateDays { get; set; }
        public short PayDays { get; set; }
        public short TotalDays { get; set; } = 30;

        [StringLength(120)]
        public string Remarks { get; set; }

        //------------------------FK-----------------
        public long SheetId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmpDesignation { get; set; }
        public string EmpDesignationCode { get; set; }
        public string EmpDepartment { get; set; }
        public string EmpDepartmentCode { get; set; }
    }
}
