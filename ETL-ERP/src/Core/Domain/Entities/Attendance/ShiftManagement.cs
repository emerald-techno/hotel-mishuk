using Domain.Entities.HR;
using Domain.Entities.Identity;
using Domain.ModelInterface;

namespace Domain.Entities.Attendance;

public class ShiftManagement : IAuditable
{
    public long Id { get; set; }
    public DateTime Month { get; set; } // first day of month, only show month and year
    public DateTime StartDate { get; set; } // only date
    public DateTime EndDate { get; set; } // only date
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    // --- Fk ---

    public long PermanentShiftId { get; set; }
    public DutyShift PermanentShift { get; set; }
    public long DutyShiftId { get; set; }
    public DutyShift DutyShift { get; set; }
    public long? EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
