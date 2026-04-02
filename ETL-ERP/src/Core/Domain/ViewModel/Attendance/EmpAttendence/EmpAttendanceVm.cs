using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Attendance.EmpAttendence
{
    public class EmpAttendanceVm
    {
        public int Id { get; set; }


        [DisplayName("Attendance Date: ")]
        public DateTime AttendDate { get; set; }

        [Required(ErrorMessage = "Attendance Date is required")]
        public string AttendDateStr { get; set; }

        [DisplayName("In Time: ")]
        public DateTime? InTime { get; set; }


        [Required(ErrorMessage = "Attendance Time is required")]
        public string AttendTimeStr { get; set; }



        [DisplayName("Out Time: ")]
        public DateTime? OutTime { get; set; }
        [DisplayName("Lunch Out: ")]
        public DateTime? LunchOut { get; set; }
        [DisplayName("Lunch In: ")]
        public DateTime? LunchIn { get; set; }

        [StringLength(1)]
        [DisplayName("Status: ")]
        public string Status { get; set; } // Present = P, Absent = A, Late = L


        [StringLength(1)]
        [DisplayName("Status Details: ")]
        public string StatusDtl { get; set; }
        public bool IsManual { get; set; }
        [DisplayName("Manual Entry Date: ")]
        public DateTime? ManualEntrydate { get; set; }

        public bool IsLunch { get; set; }

        public IEnumerable<SelectListItem> EmployeetLookUp { get; set; }

        //--------------------------------------------
        public long EmployeeId { get; set; }
        public long ManualEntryById { get; set; }
    }
}
