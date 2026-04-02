namespace Domain.ViewModel.HotelManagement.HotelReport;

public class CategoryWiseRoomAvailableReportVm
{
    public string CategoryName { get; set; }
    public string StartDateStr { get; set; }
    public string EndDateStr { get; set; }
    public DateTime BookingDate { get; set; }
    public double TotalRooms { get; set; }
    public double AvailableRooms { get; set; }
    public double VacantRooms { get; set; }
    public double ExpectedArrivals { get; set; }
    public double ExpectedDepartures { get; set; }
    public double InHouseNights { get; set; }
    public double OutOfOrderRooms { get; set; }
    public double OccupancyPct { get; set; }
}
