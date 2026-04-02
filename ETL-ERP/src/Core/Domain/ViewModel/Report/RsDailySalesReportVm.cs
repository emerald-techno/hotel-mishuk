using Domain.Enums.AppEnums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Report;

public class RsDailySalesReportVm
{
    public long OrderId { get; set; }
    public string OrderNo { get; set; }
    public string WaiterName { get; set; }
    public string TableNo { get; set; }
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? BookingDate { get; set; }
    public long CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string Mobile { get; set; }
    public long CustomerTypeId { get; set; }
    public string CustomerType { get; set; }
    public long? RoomId { get; set; }
    public string RoomNo { get; set; }
    public long? BookingId { get; set; }
    public string BookingNo { get; set; }
    public double OrderAmount { get; set; }
    public double PaidAmount { get; set; }
    public double VAT { get; set; }
    public double TAX { get; set; }
    public double ServiceCharge { get; set; }
    public double Discount { get; set; }
    public double NetAmount { get; set; }
    public double DueAmount { get; set; }
    public double NetOrderAmount { get; set; }
    public DateTime? ReservationDate { get; set; }
    public string OrderDesc { get; set; }

    public string AuditRemarks { get; set; }
    public DateTime? AuditDate { get; set; }
    public long? AuditById { get; set; }
    public string AuditBy { get; set; }

    public RsOrderPaymentStatusEnum PaymentStatus { get; set; }
    public IEnumerable<SelectListItem> CustomerLookUp { get; set; }
    public IEnumerable<SelectListItem> CustomerTypeLookUp { get; set; }
}