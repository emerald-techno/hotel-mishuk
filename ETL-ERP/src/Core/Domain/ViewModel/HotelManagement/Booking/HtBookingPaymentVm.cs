using Domain.Enums.AppEnums;

namespace Domain.ViewModel.HotelManagement.RoomBooking;

public class HtBookingPaymentVm
{
    public long Id { get; set; }
    public double? PaidAmount { get; set; }
    public PayModeEnum? PayMode { get; set; }
    public string TransactionNo { get; set; }
    public string ChequeNo { get; set; }
    public string AccountNo { get; set; }
    public string Description { get; set; }
    public DateTime? PaidDate { get; set; }
    public long? BillingId { get; set; }
    public long? VoucherId { get; set; }
    public string VoucherNo { get; set; }
}

public class BookingPaymentDto
{
    public long BookingId { get; set; }
    public double? PaidAmount { get; set; }
    public PayModeEnum? PayMode { get; set; }
    public string TransactionNo { get; set; }
    public string ChequeNo { get; set; }
    public string AccountNo { get; set; }
    public string Description { get; set; }
    public string PaidDateStr { get; set; }
}

public class DateBreakfastDto
{
    public double BreakfastAmount { get; set; }
    public string BreakfastDateStr { get; set; }
}