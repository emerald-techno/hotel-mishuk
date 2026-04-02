using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Inventory.Transaction;

public class DirectReceiveReportVm
{
    public long TranId { get; set; }
    public string TranNo { get; set; }
    public long TranById { get; set; }
    public string TranByName { get; set; }
    public DateTime TranDate { get; set; }
    public long? SupplierId { get; set; }
    public string SupplierName { get; set; }
    public string SupplierMobile { get; set; }
    public double ReceiveAmount { get; set; }
    public double PaidAmount { get; set; }
    public double DueAmount { get; set; }
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }
    public IEnumerable<SelectListItem> SupplierLookUp { get; set; }
}