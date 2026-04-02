using Domain.ModelInterface;

namespace Domain.ViewModel.Attendance.DutyShift;

public class DutyShiftSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string ShiftName { get; set; }
    public string ShiftCode { get; set; }
    public string ShiftType { get; set; } // P=Permanent, D=Duty.. Default=P
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string StartTimeStr { get; set; }
    public string EndTimeStr { get; set; }
    public string PayType { get; set; } // M=Monthly, S=ShiftWise.. Default=M
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}