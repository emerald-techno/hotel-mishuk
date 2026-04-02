using AutoMapper;
using Domain.ViewModel.Inventory;
using Domain.ViewModel.Report;
using Interface.Repository.Inventory;
using Interface.Services.Inventory;
using Interface.UnitOfWork;

namespace Services.Inventory;

public class InventoryReportService : IInventoryReportService
{
    #region Config
    private IInventoryReportRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public InventoryReportService(IInventoryReportRepository repository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork)
    {
        _iRepository = repository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }

    #endregion

    #region GetStockReportHtml

    public async Task<string> GetStockReportHtml(StockVm vm)
    {
        string fullHtml = await _iRepository.StockDetailsReportHtml(vm);
        return fullHtml;
    }

    #endregion

    #region GetStockRegisterReportHtml

    public async Task<string> GetStockRegisterReportHtml(StockRegisterVm vm)
    {
        string fullHtml = await _iRepository.StockRegisterReportHtml(vm);
        return fullHtml;
    }

    #endregion

    #region GetDepartmentStockRegisterReportHtml

    public async Task<string> GetDepartmentStockRegisterReportHtml(StockRegisterVm vm)
    {
        string fullHtml = await _iRepository.DepartmentStockRegisterReportHtml(vm);
        return fullHtml;
    }

    #endregion

    #region GetIssueReportHtml

    public async Task<string> GetIssueReportHtml(IssueReportVM vm)
    {
        string fullHtml = await _iRepository.IssueReportHtml(vm);
        return fullHtml;
    }

    #endregion

    #region GetReceiveReportHtml
    public async Task<string> GetReceiveReportHtml(ReceiveReportVM vm)
    {
        string fullHtml = await _iRepository.ReceiveReportHtml(vm);
        return fullHtml;
    }
    #endregion

    #region GetDepartmentWiseStockReportHtml

    public async Task<string> GetDepartmentWiseStockReportHtml(DepartmentStockVm vm)
    {
        string fullHtml = await _iRepository.DepartmentWiseStockReportHtml(vm);
        return fullHtml;
    }

    #endregion

    #region GetDepartmentWiseStockReportWithAmountHtml

    public async Task<string> GetDepartmentWiseStockReportWithAmountHtml(StockRegisterVm vm)
    {
        string fullHtml = await _iRepository.DepartmentStockRegisterReportWithAmountHtml(vm);
        return fullHtml;
    }

    #endregion

    #region GetItemStockByDptId

    public async Task<DepartmentStockVm?> GetItemStockByDptId(long dptId, long itemId)
    {
        var model = new DepartmentStockVm { DepartmentId = dptId, ItemId = itemId };

        var dataList = await _iRepository.DepartmentWiseStockReport(model);
        if (!(dataList?.Count > 0))
            return null;

        var data = dataList.FirstOrDefault();
        return data;
    }

    #endregion

    #region GetDepartmentWiseConsumeReportHtml

    public async Task<string> GetDepartmentWiseConsumeReportHtml(ConsumeReportVm vm)
    {
        string fullHtml = await _iRepository.ConsumeReportHtml(vm);
        return fullHtml;
    }

    #endregion

    #region GetOrderDueReportHtml

    public async Task<string> GetOrderDueReportHtml(InvOrderDueReportVm vm)
    {
        string fullHtml = await _iRepository.InvOrderDueReportHtml(vm);
        return fullHtml;
    }

    #endregion

    #region ItemConsumptionReportHtml

    public async Task<string> ItemConsumptionReportHtml(ItemConsumptionReportVm vm)
    {
        string fullHtml = await _iRepository.ItemConsumptionReportHtml(vm);
        return fullHtml;
    }

    #endregion

    #region GetStockJsonData
    public async Task<object> GetItemAverageAmountJsonData(long itemId)
    {
        var data = await _iRepository.GetItemReceiveInfo(new ReceiveReportVM
        {
            ItemId = itemId
        });
        if (data == null || !data.Any())
            return null;

        var totalAmount = data.Sum(x => x.TotalAmount);
        var totalQty = data.Sum(x => x.ItemQty);

        var averageAmount = totalQty > 0
            ? Math.Round(totalAmount / totalQty, 2)
            : 0;

        var unitId = data.First().UnitId;

        return new
        {
            AverageAmount = averageAmount,
            UnitId = unitId
        };
    }
    #endregion
}
