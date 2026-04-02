using Domain.Enums.AppEnums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Report;

public class StockVm
{
    public long CategoryId { get; set; }
    public long ItemId { get; set; }
    public long UnitId { get; set; }
    public string CategoryType { get; set; }
    public string CategoryTypeName => CategoryType switch { "I" => "Item", "S" => "Service", _ => "--" };
    public string CategoryName { get; set; }
    public string ItemName { get; set; }
    public string UnitName { get; set; }
    public double TotalRcvQty { get; set; }
    public double TotalIssueQty { get; set; }
    public double Stock { get; set; }
    public int? StockStatus { get; set; }
    public double TotalRcvAmount { get; set; }
    public double TotalIssueAmount { get; set; }
    public double StockAmount { get; set; }
    public double UnitPrice { get; set; }
    public IEnumerable<SelectListItem> CategoryLookUp { get; set; }
    public IEnumerable<SelectListItem> StockStatusLookUp { get; set; }
    public IEnumerable<SelectListItem> ItemLookUp { get; set; }
}
public class DepartmentStockVm
{
    public long CategoryId { get; set; }
    public string CategoryName { get; set; }
    public long ItemId { get; set; }
    public string ItemName { get; set; }
    public long UnitId { get; set; }
    public string UnitName { get; set; }
    public double UnitPrice { get; set; }
    public long DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public string CategoryType { get; set; }
    public string CategoryTypeName => CategoryType switch { "I" => "Item", "S" => "Service", _ => "--" };   
    public double TotalRcvQty { get; set; }
    public double TotalConsumptionQty { get; set; }
    public double Stock { get; set; }
    public int? StockStatus { get; set; }
    public double TotalRcvAmount { get; set; }
    public double TotalConsumptionAmount { get; set; }
    public double StockAmount { get; set; }
    public IEnumerable<SelectListItem> DepartmentLookUp { get; set; }
    public IEnumerable<SelectListItem> CategoryLookUp { get; set; }
    public IEnumerable<SelectListItem> StockStatusLookUp { get; set; }
    public IEnumerable<SelectListItem> ItemLookUp { get; set; }
}
