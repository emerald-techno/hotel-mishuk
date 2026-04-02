namespace Domain.ViewModel.HotelManagement.HotelReport;

public class DailyInHouseGuestVm
{
    public string StrReportDate { get; set; }
    public List<RoomWiseGuestVm> GuestList { get; set; }
    public double DayUse { get; set; } = 0;
    public double HalfDayUse { get; set; } = 0;
    public double FullDayUse { get; set; } = 0;
    public double CBF { get; set; } = 0;

    public bool IsPirnt = false;
    public string HalfDayRoomNo { get; set; } = "";
    public string DayUseRoomNo { get; set; } = "";
    public string FullDayUseRoomNo { get; set; } = "";
    public double NoShowRoom { get; set; } = 0;
    public string NoShowRoomNo { get; set; } = "";
    public bool IsTodayCbfAdded { get; set; }
}

