using Domain.Enums.AppEnums;
using Domain.ViewModel.HotelManagement.RoomBooking;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.Booking;

public class CheckInServiceVm
{
    public long Id { get; set; }
    public string BookingNo { get; set; }
    public DateTime BookingDate { get; set; }
    public string BookingDateStr { get; set; }
    public DateTime CheckInTime { get; set; }
    public string CheckInTimeStr { get; set; }
    public DateTime CheckOutTime { get; set; }
    public string CheckOutTimeStr { get; set; }
    public string VisitPurpose { get; set; }
    public string BookingType { get; set; }
    public string Remarks { get; set; }
    public BookingServiceStatusEnum BookingStatus { get; set; }
    public PaymentStatusEnum PaymentStatus { get; set; }
    public double Rent { get; set; }
    public double ServiceCharge { get; set; }
    public double Vat { get; set; }
    public double Tax { get; set; }
    public double Discount { get; set; }
    public double NetRent { get; set; }
    public double TotalGuest { get; set; }
    public double Adult { get; set; }
    public double Child { get; set; }
    public IEnumerable<SelectListItem> PayModeLookUp { get; set; }
    public virtual ICollection<HtCheckInGuestVm> CheckInGuestVms { get; set; }
    public virtual ICollection<HtCheckInRoomVm> CheckInRoomVms { get; set; }
    public virtual HtBookingPaymentVm PaymentVm { get; set; }
}

public class HtCheckInGuestVm
{
    public long BookingId { get; set; }
    public long GuestId { get; set; }
    public bool IsMain { get; set; }
}

public class HtCheckInRoomVm
{
    public long BookingId { get; set; }
    public long RoomCategoryId { get; set; }
    public long RoomId { get; set; }
    public long? ComplementaryId { get; set; }
    public DateTime? ActualCheckInTime { get; set; }
    public DateTime? ActualCheckOutTime { get; set; }
    public double Adult { get; set; }
    public double Child {get; set; }
}