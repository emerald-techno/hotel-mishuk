namespace Domain.ViewModel.HotelManagement.HotelReport;

public class CancelReportVm
{
    public long BookingId { get; set; }
    public string BookingNo { get; set; }
    public DateTime BookingDate { get; set; }
    public long GuestId { get; set; }
    public string GuestName { get; set; }
    public string Company { get; set; }
    public string Country { get; set; }
    public int TotalGuest { get; set; }
    public DateTime CancelDate { get; set; }
    public string CancelBy { get; set; }
    public string CancelReason { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public string RoomNo { get; set; }
    public string CategoryName { get; set; }
    public double NetRent { get; set; }
    public float NoOfNights { get; set; }
    public double RoomRate { get; set; }
    public string FromDateStr { get; set; }
    public string ToDateStr { get; set; }
    public string BookingFilter { get; set; }
    public string GuestFilter { get; set; }
    public string CompanyFilter { get; set; }
}
