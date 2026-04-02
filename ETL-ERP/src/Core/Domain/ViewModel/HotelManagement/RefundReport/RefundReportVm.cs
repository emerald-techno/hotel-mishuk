using Domain.Enums.AppEnums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.RefundReport
{
    public class RefundReportVm
    {
        public long? BookingId { get; set; }
        public string BookingNo { get; set; }
        public DateTime? BookingDate { get; set; }
        public long? GuestId { get; set; }
        public string GuestName { get; set; }
        public string GuestMobile { get; set; }
        public long? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateTime? EstCheckInTime { get; set; }
        public DateTime? EstCheckOutTime { get; set; }
        public DateTime? RefundDate { get; set; }
        public double RefundAmount { get; set; }
        public short RefundMode { get; set; }
        public string TransactionNo { get; set; }
        public string Description { get; set; }

        // --- Query ---
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public IEnumerable<SelectListItem> GuestLookUp { get; set; }
        public IEnumerable<SelectListItem> CompayLookUp { get; set; }
    }
}
