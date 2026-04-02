using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.HotelReport;

public class BookingServiceReportVm
{
    public long RoomCategoryId { get; set; }
    public string RoomCategory { get; set; }
    public long RoomId { get; set; }
    public string RoomNo { get; set; }
    public long FloorId { get; set; }
    public string FloorName { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public DateTime? ActualCheckInTime { get; set; }
    public DateTime? ActualCheckOutTime { get; set; }
    public decimal Rent { get; set; }
    public decimal ServiceCharge { get; set; }
    public decimal NetRent { get; set; }
    public int Adult { get; set; }
    public int Child { get; set; }
    public long BookingId { get; set; }
    public DateTime? BookingDate { get; set; }
    public string BookingNo { get; set; }
    public short BookingStatus { get; set; }
    public string BookingType { get; set; }
    public decimal PaidAmount { get; set; }
    public string BookingStatusName { get; set; }
    public short PaymentStatus { get; set; }
    public string PaymentStatusName { get; set; }
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }
    public IEnumerable<SelectListItem> RoomCategoryLookUp { get; set; }
    public IEnumerable<SelectListItem> RoomLookUp { get; set; }
    public IEnumerable<SelectListItem> FloorLookUp { get; set; }
    public IEnumerable<SelectListItem> BookingReportStatusLookUp { get; set; }

    public int BookingById { get; set; }
    public string BookingBy { get; set; }
}
