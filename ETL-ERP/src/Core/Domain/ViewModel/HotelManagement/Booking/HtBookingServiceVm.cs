using Domain.Enums.AppEnums;
using Domain.ViewModel.HotelManagement.Billing;
using Domain.ViewModel.HotelManagement.Booking;
using Domain.ViewModel.Restaurant.FoodOrder;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ViewModel.HotelManagement.RoomBooking;

public class HtBookingServiceVm
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
    public double SpecialDiscount { get; set; }
    public double NetRent { get; set; }
    public double TotalGuest { get; set; }
    public double Adult { get; set; }
    public double Child { get; set; }
    public long? OnlineBookingId { get; set; }
    public string OnlineBookingNumber { get; set; }
    public long? ActionById { get; set; }
    public string ActionByName { get; set; }
    public BookingConfirmEnum BookingConfirmStatus { get; set; }
    public IEnumerable<SelectListItem> PayModeLookUp { get; set; }
    public IEnumerable<SelectListItem> ComplementaryLookUp { get; set; }
    public IEnumerable<SelectListItem> OnlineBookingLookUp { get; set; }
    public IEnumerable<SelectListItem> HallBookingShiftLookUp { get; set; }
    public IEnumerable<SelectListItem> CompanyLookUp { get; set; }
    public IEnumerable<SelectListItem> GenderLookUp { get; set; }
    public IEnumerable<SelectListItem> CountryLookUp { get; set; }
    public IEnumerable<CustomSelectListItems> CountryWithCodeLookUp { get; set; }
    public IEnumerable<SelectListItem> DistrictLookUp { get; set; }
    public virtual ICollection<HtBookingGuestVm> BookingGuestVms { get; set; }
    public virtual ICollection<HtBookingRoomVm> BookingRoomVms { get; set; }
    public virtual ICollection<HtBookingHallVm> BookingHallVms { get; set; }
    public virtual HtBookingPaymentVm? PaymentVm { get; set; }
    public virtual ICollection<HtBookingPaymentVm> BookingPayments { get; set; }
    public string ActualCheckInTimeStr { get; set; }
    public string ActualCheckOutTimeStr { get; set; }
    public string PaidDateStr { get; set; }

    [NotMapped]
    public double PaidAmount { get; set; }

    [NotMapped]
    public bool IsRefund { get; set; }

    [NotMapped]
    public double RefundAmount { get; set; }

    [NotMapped]
    public bool IsBillGenerated { get; set; }
    [NotMapped]
    public List<string> VDRooms { get; set; } = new();
    [NotMapped]
    public List<string> RoomsWithExtraService { get; set; } = new();

    [NotMapped]
    public long? BillId { get; set; }

    [NotMapped]
    public virtual BillingVm? BookingBill { get; set; }

    public virtual List<BillingDetailVm> Services { get; set; }
    public virtual ICollection<FoodOrderVm> FoodOrders { get; set; }
}