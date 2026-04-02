using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Inventory.ItemInfo;

public class ItemInfoSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string ItemName { get; set; }
    public string ItemCode { get; set; }
    public double ReorderQty { get; set; }
    public double MaxQty { get; set; }
    public double MinQty { get; set; }
    public double AlertQty { get; set; }
    public long CategoryId { get; set; }
    public string CategoryType { get; set; }
    public string CategoryName { get; set; }
    public long UnitId { get; set; }
    public string UnitName { get; set; }
    public short LeadDay { get; set; }
    public string Remarks { get; set; }
    public bool IsDeleted { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public string PhotoDocUrl { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
    public IEnumerable<SelectListItem> CategoryLookUp { get; set; }
    public IEnumerable<SelectListItem> UnitLookUp { get; set; }
}
