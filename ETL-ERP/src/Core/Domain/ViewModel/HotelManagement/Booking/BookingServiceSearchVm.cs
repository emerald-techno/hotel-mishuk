using Domain.Enums.AppEnums;
using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.Booking;

public class BookingServiceSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string BookingNo { get; set; }
    public DateTime BookingDate { get; set; }
    public string BookingType { get; set; } // R = Room, H = Hall
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public DateTime? CancelDate { get; set; }
    public BookingServiceStatusEnum BookingStatus { get; set; }
    public PaymentStatusEnum? PaymentStatus { get; set; }
    public BookingConfirmEnum BookingConfirmStatus { get; set; }
    public double NetRent { get; set; }
    public double TotalGuest { get; set; }
    public string GuestName { get; set; }
    public string GuestMobile { get; set; }
    public bool OnlyUnPaid { get; set; }
    public long? OnlineBookingId { get; set; }
    public string OnlineBookingNumber { get; set; }
    public string OnlineGuestMobile { get; set; }
    public string FormDateStr { get; set; }
    public string ToDateStr { get; set; }
    public string RoomList { get; set; }

    // -- DataTable Propertry

    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
    public IEnumerable<SelectListItem> BookingServiceStatusLookUp { get; set; }
    public IEnumerable<SelectListItem> PaymentStatusLookUp { get; set; }
    public IEnumerable<SelectListItem> OnlineBookingLookUp { get; set; }
}

public class BookingServiceSearchDto
{
    public long Id { get; set; }
    public string BookingNo { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime? CancelDate { get; set; }
    public string BookingType { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public BookingServiceStatusEnum BookingStatus { get; set; }
    public PaymentStatusEnum PaymentStatus { get; set; }
    public BookingConfirmEnum BookingConfirmStatus { get; set; }
    public double NetRent { get; set; }
    public double TotalGuest { get; set; }
    public string GuestName { get; set; }
    public string GuestMobile { get; set; }
    public long? OnlineBookingId { get; set; }
    public string OnlineBookingNumber { get; set; }
    public string OnlineGuestMobile { get; set; }
}
