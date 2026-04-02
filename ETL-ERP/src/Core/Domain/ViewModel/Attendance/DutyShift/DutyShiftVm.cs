using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Attendance.DutyShift;

public class DutyShiftVm
{
    public long Id { get; set; }

    [StringLength(50, ErrorMessage = "Shift name can not be more than 50 characters")]
    [Required(ErrorMessage = "Shift name is required")]
    public string ShiftName { get; set; }

    [StringLength(20, ErrorMessage = "Shift code can not be more than 20 characters")]
    [Required(ErrorMessage = "Shift code is required")]
    public string ShiftCode { get; set; }

    [StringLength(1, ErrorMessage = "Shift type can not be more than 1 characters")]
    [Required(ErrorMessage = "Shift type is required")]
    public string ShiftType { get; set; } // P=Permanent, D=Duty.. Default=P
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    [StringLength(1, ErrorMessage = "Pay type can not be more than 50 characters")]
    [Required(ErrorMessage = "Pay type is required")]
    public string PayType { get; set; } // M=Monthly, S=ShiftWise.. Default=M

    public IEnumerable<SelectListItem> ShiftTypeLookUp { get; set; }
    public IEnumerable<SelectListItem> PayTypeLookUp { get; set; }
}