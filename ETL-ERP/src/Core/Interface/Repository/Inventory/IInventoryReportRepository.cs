using Domain.ViewModel.Inventory;
using Domain.ViewModel.Report;

namespace Interface.Repository.Inventory
{
    public interface IInventoryReportRepository
    {
        Task<List<StockVm>> GetInventoryStockInfo(StockVm vm);
        Task<string> StockDetailsReportHtml(StockVm vm);
        Task<string> StockRegisterReportHtml(StockRegisterVm vm);
        Task<string> DepartmentStockRegisterReportHtml(StockRegisterVm vm);

        Task<List<IssueReportVM>> GetItemIssueInfo(IssueReportVM vm);
        Task<string> IssueReportHtml(IssueReportVM vm);

        Task<List<DepartmentStockVm>> DepartmentWiseStockReport(DepartmentStockVm vm);
        Task<string> DepartmentWiseStockReportHtml(DepartmentStockVm vm);

        Task<List<ConsumeReportVm>> GetItemConsumeInfo(ConsumeReportVm vm);
        Task<string> ConsumeReportHtml(ConsumeReportVm vm);

        Task<List<InvOrderDueReportVm>> GetInvOrderDueReport(InvOrderDueReportVm vm);
        Task<string> InvOrderDueReportHtml(InvOrderDueReportVm vm, bool isPrint = false);

        Task<string> ReceiveReportHtml(ReceiveReportVM vm);
        Task<string> DepartmentStockRegisterReportWithAmountHtml(StockRegisterVm vm);

        Task<string> ItemConsumptionReportHtml(ItemConsumptionReportVm vm, bool isPrint = false);
        Task<List<ReceiveReportVM>> GetItemReceiveInfo(ReceiveReportVM vm);
    }
}
