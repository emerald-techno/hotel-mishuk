using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Attendance.ShiftManagement;

public class ShiftManagementSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public DateTime Month { get; set; } // first day of month, only show month and year
    public string MonthStr { get; set; }
    public DateTime StartDate { get; set; } // only date
    public DateTime EndDate { get; set; } // only date

    // --- Fk ---

    public long PermanentShiftId { get; set; }
    public string PermanentShiftName { get; set; }
    public long DutyShiftId { get; set; }
    public string DutyShiftName { get; set; }
    public long? EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public short? SelectYear { get; set; }
    public short? SelectMonth { get; set; }
    public IEnumerable<SelectListItem> PermanentShiftLookUp { get; set; }
    public IEnumerable<SelectListItem> DutyShiftLookUp { get; set; }
    public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}
