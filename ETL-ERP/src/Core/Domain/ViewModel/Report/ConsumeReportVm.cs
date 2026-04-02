using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Report;

public class ConsumeReportVm
{
    public long CategoryId { get; set; }
    public long ItemId { get; set; }
    public long UnitId { get; set; }
    public string CategoryName { get; set; }
    public string ItemName { get; set; }
    public string UnitName { get; set; }
    public double ConsumeQty { get; set; }
    public int? StockStatus { get; set; }
    public IEnumerable<SelectListItem> CategoryLookUp { get; set; }
    public IEnumerable<SelectListItem> ItemLookUp { get; set; }
    public IEnumerable<SelectListItem> DepartmentLookUp { get; set; }
    public IEnumerable<SelectListItem> StockStatusLookUp { get; set; }

    public long DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public long IssueRoomId { get; set; }
    public string IssueRoomNo { get; set; }
    public string Remarks { get; set; }
    public long TranById { get; set; }
    public string TranByName { get; set; }
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }
    public string TranNo { get; set; }
    public DateTime TranDate { get; set; }
    public string StrTranDate { get; set; }
}
