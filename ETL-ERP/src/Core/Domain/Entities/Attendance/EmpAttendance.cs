using System.ComponentModel.DataAnnotations;
using Domain.Entities.HR;
using Domain.Entities.Identity;

namespace Domain.Entities.Attendance
{
    public class EmpAttendance
    {
        public long Id { get; set; }
        public DateTime AttendDate { get; set; }
        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }
        public DateTime? LunchOut { get; set; }
        public DateTime? LunchIn { get; set; }

        [Required]
        [StringLength(1)]
        public string Status { get; set; } // P = Present, A = Absent, L = Leave, H = Holiday, O = Offday

        [StringLength(1)]
        public string StatusDtl { get; set; }
        public bool IsManual { get; set; }
        public DateTime? ManualEntrydate { get; set; }
        public short IsNight { get; set; } // 0=DayShift, 1=NightShift

        //------------------FK--------------------
        public Employee Employee { get; set; }
        public long EmployeeId { get; set; }

        public ApplicationUser ManualEntryBy { get; set; }
        public long? ManualEntryById { get; set; }

        public ApplicationUser ActionBy { get; set; }
        public long ActionById { get; set; }
        public DateTime ActionDate { get; set; }

        public bool IsDeleted { get; set; }

    }
}
