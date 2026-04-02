using Domain.Enums.AppEnums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.AdvanceRefund;

public class AdvanceRefundVm
{
    public long Id { get; set; }
    public DateTime RefundDate { get; set; }
    public double RefundAmount { get; set; }
    public PayModeEnum RefundMode { get; set; }
    public string TransactionNo { get; set; }
    public string ChequeNo { get; set; }
    public string AccountNo { get; set; }
    public string Description { get; set; }

    // --- Fk ---

    public long BookingId { get; set; }
    public string BookingNo { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime BookingCheckInDate { get; set; }
    public DateTime BookingCheckOutDate { get; set; }
    public double BookingNetAmount { get; set; }
    public double PaidAmount { get; set; }
    public double AlreadyRefundAmount { get; set; }
    public IEnumerable<SelectListItem> BookingServiceLookUp { get; set; }
}
