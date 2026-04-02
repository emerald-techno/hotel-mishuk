using System.ComponentModel.DataAnnotations;
using Domain.Entities.HR;
using Domain.Entities.Identity;

namespace Domain.Entities.Payroll
{
    public class MonthlyAttendanceSheetDtl
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
        public short TotalDays { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }

        //-------------FK---------------
        public long SheetId { get; set; }
        public MonthlyAttendanceSheetMst Sheet { get; set; }

        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public ApplicationUser ActionBy { get; set; }
        public long ActionById { get; set; }
        public DateTime ActionDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
