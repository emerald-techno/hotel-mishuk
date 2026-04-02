namespace Domain.ViewModel.HotelManagement.HotelReport;

public class RoomChangeReportVm
{
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }
    public DateTime ChangeDate { get; set; }

    public string BookingNo { get; set; }
    public long BookingId { get; set; }

    public string GuestName { get; set; }
    public string GuestMobile { get; set; }

    public string OldRoomNo { get; set; }
    public string OldCategoryName { get; set; }

    public string NewRoomNo { get; set; }
    public string NewCategoryName { get; set; }

    public string SubmittedBy { get; set; }
    public string Reason { get; set; }
    public string Remarks { get; set; }
}
