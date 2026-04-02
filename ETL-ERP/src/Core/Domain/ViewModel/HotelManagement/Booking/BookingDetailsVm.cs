using Domain.Enums.AppEnums;
using Domain.ViewModel.HotelManagement.RoomBooking;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ViewModel.HotelManagement.Booking;

public class BookingDetailsVm
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
    public virtual ICollection<HtBookingGuestVm> BookingGuestVms { get; set; }
    public virtual ICollection<HtBookingRoomVm> BookingRoomVms { get; set; }
    public virtual HtBookingPaymentVm? PaymentVm { get; set; }
    public string ActualCheckInTimeStr { get; set; }
    public string ActualCheckOutTimeStr { get; set; }

    [NotMapped]
    public double PaidAmount { get; set; }
    public string BillNumber { get; set; }
    public DateTime BillDate { get; set; }
    public double TotalAmmount { get; set; }
    public double NetAmount { get; set; }
    public BillStatusEnum BillStatus { get; set; } // 0 = Fresh, 1 = Partial Paid, 2 = Full Paid
    public long BookingId { get; set; }
    public string BookingGuestName { get; set; }
    public string BookingGuestMobile { get; set; }
    public string BookingGuestAddress { get; set; }
    public long BillById { get; set; }
    public string BillByName { get; set; }
    public ICollection<BookedBillingDetailVm> BillingDetails { get; set; }
    public IEnumerable<SelectListItem> PayModeLookUp { get; set; }
}

public class BookedBillingDetailVm
{
    public long Id { get; set; }
    public double Quantity { get; set; }
    public double Rate { get; set; }
    public double Amount { get; set; }
    public double VAT { get; set; }
    public double Tax { get; set; }
    public double Discount { get; set; }
    public double NetAmount { get; set; }
    public string Remarks { get; set; }
    public long BillId { get; set; }
    public long ServiceId { get; set; }
    public string ServiceName { get; set; }
    public long? BookingRoomId { get; set; }
    public string BookingRoomNo { get; set; }
    public double? BookingRoomRent { get; set; }
    public double? BookingRoomServiceCharge { get; set; }
    public DateTime? BookingRoomCheckInTime { get; set; }
    public DateTime? BookingRoomCheckOutTime { get; set; }

    [NotMapped]
    public int Days { get; set; }
}
