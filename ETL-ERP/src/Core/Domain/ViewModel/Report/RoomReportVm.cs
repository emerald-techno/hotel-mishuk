namespace Domain.ViewModel.Report;

public class RoomReportVm
{
    public long RoomId { get; set; }
    public string RoomNo { get; set; }
    public int Status { get; set; } //0 = No Info, 1 = Booked, 2 = Occupied, 3 = Available, 4 = Out Of Order, 5 = VD
    public int CleaningStatus { get; set; } = 0;//0= No Info, 4 = Out Of Order, 5=VD
    public bool IsTodayCheckout { get; set; }
}