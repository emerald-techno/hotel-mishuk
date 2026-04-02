namespace Domain.ViewModel.HotelManagement.HotelReport;

public class RoomWiseGuestVm
{
    public long BookingRoomId { get; set; }
    public string BookingRoomNo { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public string GuestName { get; set; }
    public string GuestMobile { get; set; }
    public long? ComplementaryId { get; set; }
    public string ComplementaryName { get; set; }
    public double TotalGuest { get; set; }
}
