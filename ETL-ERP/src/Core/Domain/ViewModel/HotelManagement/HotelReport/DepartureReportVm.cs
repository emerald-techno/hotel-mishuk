namespace Domain.ViewModel.HotelManagement.HotelReport;

public class DepartureReportVm
{
    public string StrQueryDate { get; set; }
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }
    public DateTime CheckedOutDateTime { get; set; }
    public DateTime CheckedOutDate { get; set; }
    public TimeSpan CheckedOutTime { get; set; }
    public DateTime CheckInDateTime { get; set; }
    public string RoomNo { get; set; }
    public string RoomCategory { get; set; }
    public string GuestName { get; set; }
    public string GuestMobile { get; set; }
    public string CountryName { get; set; }
    public string CompanyName { get; set; }
    public string BillNumber { get; set; }
    public long? BillId { get; set; }

    public string BookingNo { get; set; }
    public long BookingId { get; set; }
    public string Remarks { get; set; }
    public int Adult { get; set; }
    public int Child { get; set; }
    public double RoomRent { get; set; }
}
