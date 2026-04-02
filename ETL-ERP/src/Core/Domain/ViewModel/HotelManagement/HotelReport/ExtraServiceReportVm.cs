using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.HotelReport;

public class ExtraServiceReportVm
{
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }
    public long? ServiceId { get; set; }
    public string ServiceName { get; set; }
    public string ServiceCode { get; set; }
    public DateTime ServiceDate { get; set; }
    public string RoomNo { get; set; }
    public string BookingNo { get; set; }
    public long BookingId { get; set; }
    public string BillNo { get; set; }
    public long BillId { get; set; }
    public long? BillDtlId { get; set; }
    public double Amount { get; set; }
    public double Rate { get; set; }
    public double Discount { get; set; }
    public double ServiceCharge { get; set; }
    public double VAT { get; set; }
    public double Quantity { get; set; }
    public double TotalAmount { get; set; }

    public string AuditRemarks { get; set; }
    public DateTime? AuditDate { get; set; }
    public long? AuditById { get; set; }
    public string AuditBy { get; set; }

    public IEnumerable<SelectListItem> ServiceLookUp { get; set; }
}
