using Domain.Enums.AppEnums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.HotelReport;

public class ReservationReportVm
{
    public long BookingId { get; set; }
    public long BookingRoomId { get; set; }
    public short? RoomId { get; set; }
    public short? CategoryId { get; set; }
    public string BookingNo { get; set; }
    public DateTime BookingDate { get; set; }
    public string FromDateStr { get; set; }
    public string ToDateStr { get; set; }
    public short? BookingRoomAssignStatusId { get; set; }
    public long GuestId { get; set; }
    public string GuestName { get; set; }
    public string GuestMobile { get; set; }
    public string GuestEmail { get; set; }
    public string Company { get; set; }
    public string Country { get; set; }
    public decimal TotalGuest { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public float RoomDiscountPercent { get; set; }
    public float FoodDiscountPercent { get; set; }
    public string RoomNo { get; set; }
    public string CategoryName { get; set; }
    public double NetRent { get; set; }
    public double Discount { get; set; }
    public double AdvancePayment { get; set; }
    public float NoOfNights { get; set; }
    public double RoomRate { get; set; }
    public string Remarks { get; set; }
    public string BookingFilter { get; set; }
    public string RoomFilter { get; set; }
    public string GuestFilter { get; set; }
    public string UserName { get; set; }
    public BookingConfirmEnum BookingConfirmStatus { get; set; }
    public IEnumerable<SelectListItem>? BookingRoomAssignLookUp { get; set; }
    public IEnumerable<SelectListItem>? BookingRoomLookUp { get; set; }
}
