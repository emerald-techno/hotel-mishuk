using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Report;

public class InvOrderDueReportVm
{
    public long OrderId { get; set; }    
    public string OrderNo { get; set; }
    public DateTime OrderDate { get; set; }
    public long SupplierId { get; set; }
    public string SupplierName { get; set; }
    public string MrrNoList { get; set; }
    public double OrderAmount { get; set; }
    public double MrrAmount { get; set; }
    public double PaidAmount { get; set; }
    public double DueAmount { get; set; }
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }
    public IEnumerable<SelectListItem> OrderLookUp { get; set; }
    public IEnumerable<SelectListItem> SupplierLookUp { get; set; }
}
