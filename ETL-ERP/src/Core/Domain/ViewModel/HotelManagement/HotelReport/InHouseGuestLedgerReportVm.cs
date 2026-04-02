namespace Domain.ViewModel.HotelManagement.HotelReport;

public class InHouseGuestLedgerReportVm
{
    public string StrQueryDate { get; set; }
    public long BookingId { get; set; }
    public string BookingNo { get; set; } = string.Empty;
    public string GuestName { get; set; } = string.Empty;
    public string GuestMobile { get; set; } = string.Empty;
    public string RoomNo { get; set; } = string.Empty;
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public double RackRate { get; set; }
    public double RoomRentCharged { get; set; }
    public double FnbCharged { get; set; }
    public double RoomTotalTariff { get; set; }
    public double BookingTotalCharged { get; set; }
    public double BookingPaid { get; set; }
    public double BookingDue { get; set; }
    
    public bool IsPirnt = false;
}