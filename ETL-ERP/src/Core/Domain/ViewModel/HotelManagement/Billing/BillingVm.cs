using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.ViewModel.HotelManagement.RoomBooking;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DU = Domain.Utility;

namespace Domain.ViewModel.HotelManagement.Billing;

public class BillingVm
{
    public long Id { get; set; }
    public string BillNumber { get; set; }
    public DateTime BillDate { get; set; }
    public double TotalAmount { get; set; }
    public double Vat { get; set; }
    public double Tax { get; set; }
    public double Discount { get; set; }
    public double SpecialDiscount { get; set; }
    public double PaidAmount { get; set; }
    public double NetAmount { get; set; }
    public BillStatusEnum BillStatus { get; set; } // 0 = Fresh, 1 = Partial Paid, 2 = Full Paid

    [StringLength(150)]
    public string Remarks { get; set; }
    public long BookingId { get; set; }
    public string BookingNo { get; set; }
    public DateTime BookingDate { get; set; }
    public string BookingType { get; set; }
    public BookingServiceStatusEnum BookingStatus { get; set; }
    public long? BookingGuestId { get; set; }
    public string BookingGuestName { get; set; }
    public string BookingGuestMobile { get; set; }
    public string BookingGuestAddress { get; set; }
    public long BillById { get; set; }
    public string BillByName { get; set; }
    public ICollection<BillingDetailVm> BillingDetails { get; set; }
    public List<HtBookingPaymentVm> BillingPayments { get; set; }
    public IEnumerable<SelectListItem> PayModeLookUp { get; set; }
    public IEnumerable<SelectListItem> ServiceLookUp { get; set; }
    public IEnumerable<SelectListItem> BookingRoomLookUp { get; set; }
    public ICollection<SelectListItem> BillServiceLookUp { get; set; }
    public double AdvanceAmount { get; set; }
    public string MrList { get; set; }
    public string PaidDateStr { get; set; }
    public string TransactionNo { get; set; }
    public string CmpRemarks { get; set; }

    public double TotalGuest { get; set; }
    public string BillStatusText { get; set; }
    public string BillStatusClass { get; set; }
    public string DurationText { get; set; }
    public void SetBillStatusInfo()
    {
        if (this.BillStatus == BillStatusEnum.FullPaid)
        {
            this.BillStatusText = "Full Paid";
            this.BillStatusClass = "badge badge-pill badge-success";
        }
        else if (this.BillStatus == BillStatusEnum.PartialPaid)
        {
            this.BillStatusText = "Partial Paid";
            this.BillStatusClass = "badge badge-pill badge-warning";
        }
        else if (this.BillStatus == BillStatusEnum.Fresh)
        {
            this.BillStatusText = "New";
            this.BillStatusClass = "badge badge-pill badge-secondary";
        }

        try
        {
            DateTime checkInDate = Convert.ToDateTime(this.BillingDetails.Min(o => o.BookingRoomCheckInTime)).Date;
            DateTime checkOutDate = Convert.ToDateTime(this.BillingDetails.Max(o => o.BookingRoomCheckOutTime)).Date;
            int nightCount = DU.AppUtility.DaysDiffernce(checkOutDate, checkInDate);
            int dayUseCount = (this.BillingDetails.Where(o => Convert.ToDateTime(o.BookingRoomCheckOutTime).Date == checkOutDate.Date && o.BookingDayStatus == (int)BookingDayStatusEnum.DayUse).ToList().Count > 0) ? 1 : 0;
            int halfDayCount = (this.BillingDetails.Where(o => Convert.ToDateTime(o.BookingRoomCheckOutTime).Date == checkOutDate.Date && o.BookingDayStatus == (int)BookingDayStatusEnum.HalfDay).ToList().Count > 0) ? 1 : 0;
            halfDayCount = (dayUseCount > 0) ? 0 : halfDayCount;

            this.DurationText += (nightCount > 0) ? $"{nightCount} Night " : "";
            this.DurationText += (dayUseCount > 0) ? (string.IsNullOrEmpty(this.DurationText)) ? $"{dayUseCount} Day Use " : $" | {dayUseCount} Day Use " : "";
            this.DurationText += (halfDayCount > 0) ? (string.IsNullOrEmpty(this.DurationText)) ? $"{halfDayCount} Half Day " : $" | {halfDayCount} Half Day " : "";

        }
        catch (Exception ex)
        {
        }
    }
}

public class BillingDetailVm
{
    public long Id { get; set; }
    public double Quantity { get; set; }
    public double Rate { get; set; }
    public double Amount { get; set; }
    public double VAT { get; set; }
    public double Tax { get; set; }
    public double Discount { get; set; }
    public double ExtraBedCharge { get; set; } = 0;
    public double ServiceCharge { get; set; } = 0;
    public double NetAmount { get; set; }
    public string Remarks { get; set; }
    public long BillId { get; set; }
    public DateTime? ServiceDate { get; set; }
    public DateTime? ActionDate { get; set; }
    public long ServiceId { get; set; }
    public string ServiceName { get; set; }
    public string ServiceCode { get; set; }
    public long? BookingRoomId { get; set; }
    public string BookingRoomNo { get; set; }
    public double? BookingRoomRent { get; set; }
    public double? BookingRoomServiceCharge { get; set; }
    public double? BookingRoomDiscount { get; set; }
    public double? BookingRoomExtraBedCharge { get; set; }
    public short? BookingRoomExtraBed { get; set; }
    public DateTime? BookingRoomCheckInTime { get; set; }
    public DateTime? BookingRoomCheckOutTime { get; set; }

    public long? BookingRoomCategoryId { get; set; }
    public string BookingRoomCategoryName { get; set; }


    public string BookingHallName { get; set; }
    public DateTime? BookingHallBookDate { get; set; }
    public int? BookingHallBookShift { get; set; }
    public string HallShiftText => BookingHallBookShift switch { 1 => "Day Shift", 2 => "Night Shift", 3 => "Both Shift", _ => "--" };
    public double? BookingHallRent { get; set; }

    [NotMapped]
    //public int Days { get; set; }
    public double Days { get; set; }
    public int BookingDayStatus { get; set; }

    public long? LedgerId { get; set; }
    public string LedgerName { get; set; }
    public string LedgerCode { get; set; }  


}

public class SaveBillingDetailVm
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public DateTime? ServiceDate { get; set; }
    public double Quantity { get; set; }
    public double Rate { get; set; }
    public double Amount { get; set; }
    public double VAT { get; set; }
    public double ServiceCharge { get; set; }
    public double Tax { get; set; }
    public double Discount { get; set; }
    public double NetAmount { get; set; }
    public string Remarks { get; set; }
    public long BillId { get; set; }
    public long ServiceId { get; set; }
    public long? BookingRoomId { get; set; }

}
