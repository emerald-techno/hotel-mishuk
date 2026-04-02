using Domain.Enums.AppEnums;

namespace Domain.ViewModel.HotelManagement.HotelReport;

public class BookingArrivalVm
{
    public long BookingId { get; set; }
    public string BookingNo { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public BookingServiceStatusEnum BookingStatus { get; set; }
    public PaymentStatusEnum PaymentStatus { get; set; }
    public double NetRent { get; set; }
    public string GuestName { get; set; }
    public string GuestMobile { get; set; }
    public string RoomList { get; set; }
    public string Remarks { get; set; }
    public string StrQueryDate { get; set; }
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }
    public double AdvanceAmount { get; set; }
    public int RoomCount { get; set; }
}
