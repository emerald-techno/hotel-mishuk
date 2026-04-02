using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.HotelReport;

public class AdvanceReportVm
{
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public DateTime? ActualCheckInTime { get; set; }
    public DateTime? ActualCheckOutTime { get; set; }
    public string BookingType { get; set; }
    public long BookingId { get; set; }
    public string BookingNo { get; set; }
    public DateTime BookingDate { get; set; }
    public string BookingDateStr { get; set; }
    public string RoomNoList { get; set; }
    public long GuestId { get; set; }
    public string GuestName { get; set; }
    public long CompanyId { get; set; }
    public string CompanyName { get; set; }
    public string MrNoList { get; set; }
    public decimal Amount { get; set; }
    public decimal NetRent { get; set; }
    public string QType { get; set; } // RR = Room Reservation,
                                      // RA = Room Advance,
                                      // HR = Hall Reservation,
                                      // HA = Hall Advance

    public bool IsPirnt = false;
    public IEnumerable<SelectListItem> GuestLookUp { get; set; }
    public IEnumerable<SelectListItem> CompanyLookUp { get; set; }
    public IEnumerable<SelectListItem> AdvanceTypeLookUp { get; set; }
}
