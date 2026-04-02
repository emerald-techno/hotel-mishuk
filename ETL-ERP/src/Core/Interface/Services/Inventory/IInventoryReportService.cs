using Domain.ViewModel.Inventory;
using Domain.ViewModel.Report;

namespace Interface.Services.Inventory;

public interface IInventoryReportService
{
    Task<string> GetStockReportHtml(StockVm vm);
    Task<string> GetIssueReportHtml(IssueReportVM vm);
    Task<string> GetDepartmentWiseStockReportHtml(DepartmentStockVm vm);
    Task<DepartmentStockVm?> GetItemStockByDptId(long dptId, long itemId);
    Task<string> GetStockRegisterReportHtml(StockRegisterVm vm);
    Task<string> GetDepartmentWiseConsumeReportHtml(ConsumeReportVm vm);
    Task<string> GetOrderDueReportHtml(InvOrderDueReportVm vm);
    Task<string> GetDepartmentStockRegisterReportHtml(StockRegisterVm vm);
    Task<string> GetReceiveReportHtml(ReceiveReportVM vm);
    Task<string> GetDepartmentWiseStockReportWithAmountHtml(StockRegisterVm vm);
    Task<string> ItemConsumptionReportHtml(ItemConsumptionReportVm vm);
    Task<object> GetItemAverageAmountJsonData(long itemId);
}

