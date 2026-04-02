using Domain.Enums.AppEnums;
using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Inventory.BillPayment;

public class InventoryBillPaymentSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string BillNo { get; set; }
    public string BillDate { get; set; }
    public long OrderMstId { get; set; }
    public string OrderMstNo { get; set; }
    public string OrderDate { get; set; }
    public long? SupplierId { get; set; }
    public string SupplierName { get; set; }
    public double BillAmount { get; set; }
    public PayModeEnum PayMode { get; set; }
    public string BillByName { get; set; }
    public string ApprovedByName { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
    public IEnumerable<SelectListItem> OrderLookUp { get; set; }
    public IEnumerable<SelectListItem> SupplierLookUp { get; set; }
}