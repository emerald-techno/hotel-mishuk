namespace Domain.ViewModel.Inventory;

public class ItemConsumptionReportVm
{
    public string FoodName { get; set; }
    public int? FoodId { get; set; }
    public double? TotalSold { get; set; }
    public string ItemName { get; set; }
    public int ItemId { get; set; }
    public double? IngQty { get; set; }
    public double TotalIngQy { get; set; }
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }
    public string UnitName { get; set; }



}
