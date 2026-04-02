using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Attendance.ShiftManagement;

public class ShiftManagementVm
{
    public long Id { get; set; }
    public DateTime Month { get; set; } // first day of month, only show month and year
    public DateTime StartDate { get; set; } // only date
    public string StartDateStr { get; set; }
    public DateTime EndDate { get; set; } // only date
    public string EndDateStr { get; set; }

    public short? SelectYear { get; set; }
    public short? SelectMonth { get; set; }

    // --- Fk ---

    public long PermanentShiftId { get; set; }
    public string PermanentShiftName { get; set; }
    public long DutyShiftId { get; set; }
    public string DutyShiftName { get; set; }
    public long? EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public IEnumerable<SelectListItem> PermanentShiftLookUp { get; set; }
    public IEnumerable<SelectListItem> DutyShiftLookUp { get; set; }
    public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }
    public ICollection<ShiftManagementVm> ShiftManagementVms { get; set; }
}