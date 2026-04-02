using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Report;

public class RsTransectionReportVm
{
    public long RsPaymentId { get; set; }
    public long OrderId { get; set; }
    public string OrderNo { get; set; }
    public DateTime? OrderDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public string CustomerName { get; set; }
    public string Mobile { get; set; }
    public string CustomerType { get; set; }
    public string WaiterName { get; set; }
    public int? RoomId { get; set; }
    public string RoomNo { get; set; }
    public string TableNo { get; set; }
    public long? BillDtlId { get; set; }
    public int PayMode { get; set; }
    public string Remarks { get; set; }

    public double OrderAmount { get; set; }
    public double Discount { get; set; }
    public double ServiceCharge { get; set; }
    public double VAT { get; set; }
    public double NetAmount { get; set; }
    public double PaidAmount { get; set; }


    public string AuditRemarks { get; set; }
    public DateTime? AuditDate { get; set; }
    public long? AuditById { get; set; }
    public string AuditBy { get; set; }

    // Report Filters
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }

    public long? CustomerId { get; set; }
    public long? CustomerTypeId { get; set; }

    public IEnumerable<SelectListItem> CustomerLookUp { get; set; }
    public IEnumerable<SelectListItem> CustomerTypeLookUp { get; set; }
}
