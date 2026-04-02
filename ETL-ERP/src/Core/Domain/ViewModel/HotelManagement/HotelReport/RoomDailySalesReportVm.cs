namespace Domain.ViewModel.HotelManagement.HotelReport;

public class RoomDailySalesReportVm
{
    public long BookingRoomId { get; set; }
    public string BookingRoomNo { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public string GuestName { get; set; }
    public string GuestMobile { get; set; }
    public double RoomRate { get; set; }
    public double Discount { get; set; }
    public double NetSale { get; set; }
}