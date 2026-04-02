using AutoMapper;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using Domain.Entities.Accounting;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.ViewModel.Inventory;
using Domain.ViewModel.Report;
using Interface.Repository.Accounts;
using Interface.Repository.Inventory;
using Interface.Services.Admin;
using Persistence.DapperModel;
using DU = Domain.Utility;

namespace Repository.Inventory;

public class InventoryReportRepository : IInventoryReportRepository
{
    #region Config

    private readonly IMapper _iMapper;
    private readonly IApplicationReadDbConnection _iReadDbConnection;
    private readonly IAccTranMstRepository _AccTranRepository;
    private readonly ISetCountryService _iSetCountryService;
    private readonly IAccLedgerRepository _iAccLedgerRepository;

    public InventoryReportRepository(IMapper iMapper, IApplicationReadDbConnection iReadDbConnection, IAccTranMstRepository accTranRepository, ISetCountryService iSetCountryService, IAccLedgerRepository iAccLedgerRepository)
    {
        _iMapper = iMapper;
        _iReadDbConnection = iReadDbConnection;
        _AccTranRepository = accTranRepository;
        _iSetCountryService = iSetCountryService;
        _iAccLedgerRepository = iAccLedgerRepository;
    }

    #endregion

    #region InventoryStockReport

    public async Task<List<StockVm>> GetInventoryStockInfo(StockVm vm)
    {
        var categoryIdQuery = vm.CategoryId > 0 ? $"and ci.Id = {vm.CategoryId}" : "";
        var itemIdQuery = vm.ItemId > 0 ? $"and ii.Id = {vm.ItemId}" : "";

        var query = $@"select d.CategoryType,d.CategoryId,d.CategoryName,d.ItemId,d.ItemName,d.UnitId,d.UnitName,sum(isnull(d.RcvQty,0)) TotalRcvQty,sum(isnull(d.IssQty,0)) TotalIssueQty
            ,sum(isnull(d.RcvQty,0)) - sum(isnull(d.IssQty,0)) Stock,sum(isnull(d.RcvAmount,0)) TotalRcvAmount,sum(isnull(d.IssueAmount,0)) TotalIssueAmount
            ,sum(isnull(d.RcvAmount,0)) - sum(isnull(d.IssueAmount,0)) StockAmount,case when (sum(isnull(d.RcvQty,0)) - sum(isnull(d.IssQty,0))) > 0 and  
            sum(isnull(d.RcvAmount,0)) - sum(isnull(d.IssueAmount,0)) > 0 then round((sum(isnull(d.RcvAmount,0)) - sum(isnull(d.IssueAmount,0))) / (sum(isnull(d.RcvQty,0)) - sum(isnull(d.IssQty,0))),2)
            else 0 end UnitPrice
            from (
            select ci.CategoryType,ci.Id CategoryId, ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName UnitName,td.ItemQty RcvQty,0 IssQty,td.ItemQty * td.UnitPrice RcvAmount,0 IssueAmount
            from TranDtls td
            inner join ItemInfos ii ON ii.Id = td.ItemId
            inner join CategoryInfos ci on ci.Id = ii.CategoryId
            inner join TranMsts tm ON tm.Id = td.TranMstId
            inner join UnitInfos ui on ui.Id = ii.UnitId
            where tm.IsDeleted = 0 and tm.TranType in ('R','O','E') {categoryIdQuery} {itemIdQuery}
            union all
            select ci.CategoryType,ci.Id CategoryId,ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName,0 RcvQty,td.ItemQty IssQty,0 RcvAmount,td.ItemQty * td.UnitPrice IssueAmount
            from TranDtls td
            inner join ItemInfos ii ON ii.Id = td.ItemId
            inner join CategoryInfos ci on ci.Id = ii.CategoryId
            inner join TranMsts tm ON tm.Id = td.TranMstId
            inner join UnitInfos ui on ui.Id = ii.UnitId
            where tm.IsDeleted = 0 and tm.TranType in ('I','U') {categoryIdQuery} {itemIdQuery}
            ) d group by d.CategoryType,d.CategoryId,d.CategoryName,d.ItemId,d.ItemName,d.UnitId,d.UnitName";

        var data = await _iReadDbConnection.QueryAsync<StockVm>(query);
        return data.ToList();
    }

    #endregion

    #region InventoryStockRegisterReport

    public async Task<List<StockRegisterVm>> GetStockRegisterInfo(StockRegisterVm vm)
    {
        var categoryIdQuery = vm.CategoryId > 0 ? $"and g.CategoryId = {vm.CategoryId}" : "";
        var itemIdQuery = vm.ItemId > 0 ? $"and g.ItemId = {vm.ItemId}" : "";
        vm.StrFromDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrFromDate;
        vm.StrToDate = (string.IsNullOrEmpty(vm.StrToDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrToDate;
        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)).ToString("dd/MMM/yyyy");

        var query = $@"select g.CategoryId, g.CategoryName,g.ItemId,g.ItemName,g.UnitId,g.UnitName,sum(isnull(g.OpeningQty,0)) OpeningQty,sum(isnull(g.OpeningAmount,0)) OpeningAmount,sum(isnull(g.ReceiveQty,0)) ReceiveQty
        ,sum(isnull(g.ReceiveAmount,0)) ReceiveAmount,sum(isnull(g.IssueQty,0)) IssueQty,sum(isnull(g.IssueAmount,0)) IssueAmount,sum(isnull(g.ReceiveReturnQty,0)) ReceiveReturnQty
        ,sum(isnull(g.ReceiveReturnAmount,0)) ReceiveReturnAmount,sum(isnull(g.IssueReturnQty,0)) IssueReturnQty,sum(isnull(g.IssueReturnAmount,0)) IssueReturnAmount
        ,sum(isnull(g.OpeningQty,0)) + sum(isnull(g.ReceiveQty,0)) + sum(isnull(g.IssueReturnQty,0)) TotalInQty
        ,sum(isnull(g.OpeningAmount,0)) + sum(isnull(g.ReceiveAmount,0)) + sum(isnull(g.IssueReturnAmount,0)) TotalInAmount
        ,sum(isnull(g.IssueQty,0)) + sum(isnull(g.ReceiveReturnQty,0)) TotalOutQty
        ,sum(isnull(g.IssueAmount,0)) + sum(isnull(g.ReceiveReturnAmount,0)) TotalOutAmount
        ,(sum(isnull(g.OpeningQty,0)) + sum(isnull(g.ReceiveQty,0)) + sum(isnull(g.IssueReturnQty,0))) - (sum(isnull(g.IssueQty,0)) + sum(isnull(g.ReceiveReturnQty,0))) ClosingQty
        ,(sum(isnull(g.OpeningAmount,0)) + sum(isnull(g.ReceiveAmount,0)) + sum(isnull(g.IssueReturnAmount,0))) - (sum(isnull(g.IssueAmount,0)) + sum(isnull(g.ReceiveReturnAmount,0))) ClosingAmount
        from (
        select d.CategoryId, d.CategoryName,d.ItemId,d.ItemName,d.UnitId,d.UnitName,sum(isnull(d.RcvQty,0)) - sum(isnull(d.IssQty,0)) OpeningQty,sum(isnull(d.RcvAmount,0)) - sum(isnull(d.IssueAmount,0)) OpeningAmount
        ,0 ReceiveQty,0 ReceiveAmount,0 IssueQty,0 IssueAmount,0 ReceiveReturnQty,0 ReceiveReturnAmount,0 IssueReturnQty,0 IssueReturnAmount,0 ClosingQty,0 ClosingAmount
        from (
        select ci.Id CategoryId, ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName UnitName,td.ItemQty RcvQty,0 IssQty,td.ItemQty * td.UnitPrice RcvAmount,0 IssueAmount
        from TranDtls td
        inner join ItemInfos ii ON ii.Id = td.ItemId
        inner join CategoryInfos ci on ci.Id = ii.CategoryId
        inner join TranMsts tm ON tm.Id = td.TranMstId
        inner join UnitInfos ui on ui.Id = ii.UnitId
        where tm.IsDeleted = 0 and tm.TranType in ('R','O','E') and tm.TranDate < '{fromDate}' and tm.IssueDeptId is null 
        union all
        select ci.Id CategoryId,ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName,0 RcvQty,td.ItemQty IssQty,0 RcvAmount,td.ItemQty * td.UnitPrice IssueAmount
        from TranDtls td
        inner join ItemInfos ii ON ii.Id = td.ItemId
        inner join CategoryInfos ci on ci.Id = ii.CategoryId
        inner join TranMsts tm ON tm.Id = td.TranMstId
        inner join UnitInfos ui on ui.Id = ii.UnitId
        where tm.IsDeleted = 0 and tm.TranType in ('I','U') and tm.TranDate < '{fromDate}'
        )d group by d.CategoryId, d.CategoryName,d.ItemId,d.ItemName,d.UnitId,d.UnitName
        union all
        select ci.Id CategoryId, ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName UnitName,0 OpeningQty,0 OpeningAmount,isnull(sum(td.ItemQty),0) ReceiveQty,isnull(sum(td.ItemQty * td.UnitPrice),0) ReceiveAmount
        ,0 IssueQty,0 IssueAmount,0 ReceiveReturnQty,0 ReceiveReturnAmount,0 IssueReturnQty,0 IssueReturnAmount,0 ClosingQty,0 ClosingAmount
        from TranDtls td
        inner join ItemInfos ii ON ii.Id = td.ItemId
        inner join CategoryInfos ci on ci.Id = ii.CategoryId
        inner join TranMsts tm ON tm.Id = td.TranMstId
        inner join UnitInfos ui on ui.Id = ii.UnitId
        where tm.IsDeleted = 0 and tm.TranType = 'R' and tm.TranDate between '{fromDate}' and '{toDate}'
        group by ci.Id, ci.CategoryName,ii.Id,ii.ItemName,ui.Id,ui.UnitName
        union all
        select ci.Id CategoryId, ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName UnitName,0 OpeningQty,0 OpeningAmount,0 ReceiveQty,0 ReceiveAmount
        ,0 IssueQty,0 IssueAmount,0 ReceiveReturnQty,0 ReceiveReturnAmount,isnull(sum(td.ItemQty),0) IssueReturnQty,isnull(sum(td.ItemQty * td.UnitPrice),0) IssueReturnAmount,0 ClosingQty,0 ClosingAmount
        from TranDtls td
        inner join ItemInfos ii ON ii.Id = td.ItemId
        inner join CategoryInfos ci on ci.Id = ii.CategoryId
        inner join TranMsts tm ON tm.Id = td.TranMstId
        inner join UnitInfos ui on ui.Id = ii.UnitId
        where tm.IsDeleted = 0 and tm.TranType = 'E' and tm.TranDate between '{fromDate}' and '{toDate}'
        group by ci.Id, ci.CategoryName,ii.Id,ii.ItemName,ui.Id,ui.UnitName
        union all
        select ci.Id CategoryId, ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName UnitName,0 OpeningQty,0 OpeningAmount,0 ReceiveQty,0 ReceiveAmount
        ,isnull(sum(td.ItemQty),0) IssueQty,isnull(sum(td.ItemQty * td.UnitPrice),0)  IssueAmount,0 ReceiveReturnQty,0 ReceiveReturnAmount,0 IssueReturnQty,0 IssueReturnAmount,0 ClosingQty,0 ClosingAmount
        from TranDtls td
        inner join ItemInfos ii ON ii.Id = td.ItemId
        inner join CategoryInfos ci on ci.Id = ii.CategoryId
        inner join TranMsts tm ON tm.Id = td.TranMstId
        inner join UnitInfos ui on ui.Id = ii.UnitId
        where tm.IsDeleted = 0 and tm.TranType = 'I' and tm.TranDate between '{fromDate}' and '{toDate}'
        group by ci.Id, ci.CategoryName,ii.Id,ii.ItemName,ui.Id,ui.UnitName
        union all
        select ci.Id CategoryId, ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName UnitName,0 OpeningQty,0 OpeningAmount,0 ReceiveQty,0 ReceiveAmount
        ,0 IssueQty,0 IssueAmount,isnull(sum(td.ItemQty),0) ReceiveReturnQty,isnull(sum(td.ItemQty * td.UnitPrice),0) ReceiveReturnAmount,0 IssueReturnQty,0 IssueReturnAmount,0 ClosingQty,0 ClosingAmount
        from TranDtls td
        inner join ItemInfos ii ON ii.Id = td.ItemId
        inner join CategoryInfos ci on ci.Id = ii.CategoryId
        inner join TranMsts tm ON tm.Id = td.TranMstId
        inner join UnitInfos ui on ui.Id = ii.UnitId
        where tm.IsDeleted = 0 and tm.TranType = 'U' and tm.TranDate between '{fromDate}' and '{toDate}'
        group by ci.Id, ci.CategoryName,ii.Id,ii.ItemName,ui.Id,ui.UnitName
        ) g where 1=1 {categoryIdQuery} {itemIdQuery}
        group by g.CategoryId, g.CategoryName,g.ItemId,g.ItemName,g.UnitId,g.UnitName order by g.CategoryName,g.ItemName ";

        var data = await _iReadDbConnection.QueryAsync<StockRegisterVm>(query);
        return data.ToList();
    }

    #endregion

    #region InventoryDepartmentStockRegisterReport

    public async Task<List<StockRegisterVm>> GetDepartmentStockRegisterInfo(StockRegisterVm vm)
    {
        var categoryIdQuery = vm.CategoryId > 0 ? $"and g.CategoryId = {vm.CategoryId}" : "";
        var itemIdQuery = vm.ItemId > 0 ? $"and g.ItemId = {vm.ItemId}" : "";
        var departmentQuery = (vm.DepartmentId > 0) ? $" and g.DepartmentId = {vm.DepartmentId}" : "";

        vm.StrFromDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrFromDate;
        vm.StrToDate = (string.IsNullOrEmpty(vm.StrToDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrToDate;
        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)).ToString("dd/MMM/yyyy");

        var query = $@"select g.DepartmentId,g.DepartmentName, g.CategoryId, g.CategoryName,g.ItemId,g.ItemName,g.UnitId,g.UnitName,sum(isnull(g.OpeningQty,0)) OpeningQty,sum(isnull(g.OpeningAmount,0)) OpeningAmount,sum(isnull(g.ReceiveQty,0)) ReceiveQty
        ,sum(isnull(g.ReceiveAmount,0)) ReceiveAmount,sum(isnull(g.IssueQty,0)) IssueQty,sum(isnull(g.IssueAmount,0)) IssueAmount,sum(isnull(g.IssueReturnQty,0)) LeftOverQty,sum(isnull(g.IssueReturnAmount,0)) LeftOverQtyAmount
        ,sum(isnull(g.OpeningQty,0)) + sum(isnull(g.ReceiveQty,0)) + sum(isnull(g.IssueReturnQty,0)) TotalInQty
        ,sum(isnull(g.OpeningAmount,0)) + sum(isnull(g.ReceiveAmount,0)) + sum(isnull(g.IssueReturnAmount,0)) TotalInAmount
        ,sum(isnull(g.IssueQty,0)) + sum(isnull(g.ReceiveReturnQty,0)) TotalOutQty
        ,sum(isnull(g.IssueAmount,0)) + sum(isnull(g.ReceiveReturnAmount,0)) TotalOutAmount
        ,(sum(isnull(g.OpeningQty,0)) + sum(isnull(g.ReceiveQty,0)) + sum(isnull(g.IssueReturnQty,0))) - (sum(isnull(g.IssueQty,0)) + sum(isnull(g.ReceiveReturnQty,0))) ClosingQty
        ,(sum(isnull(g.OpeningAmount,0)) + sum(isnull(g.ReceiveAmount,0)) + sum(isnull(g.IssueReturnAmount,0))) - (sum(isnull(g.IssueAmount,0)) + sum(isnull(g.ReceiveReturnAmount,0))) ClosingAmount
        from (
        select d.CategoryId, d.CategoryName,d.ItemId,d.ItemName,d.UnitId,d.UnitName,sum(isnull(d.RcvQty,0)) - sum(isnull(d.IssQty,0)) OpeningQty,sum(isnull(d.RcvAmount,0)) - sum(isnull(d.IssueAmount,0)) OpeningAmount
        ,0 ReceiveQty,0 ReceiveAmount,0 IssueQty,0 IssueAmount,0 ReceiveReturnQty,0 ReceiveReturnAmount,0 IssueReturnQty,0 IssueReturnAmount,0 ClosingQty,0 ClosingAmount,d.DepartmentId,d.DepartmentName
        from (
        select ci.Id CategoryId, ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName UnitName,td.ItemQty RcvQty,0 IssQty,td.ItemQty * td.UnitPrice RcvAmount,0 IssueAmount,d.Id DepartmentId,d.Name DepartmentName
        from TranDtls td
        inner join ItemInfos ii ON ii.Id = td.ItemId
        inner join CategoryInfos ci on ci.Id = ii.CategoryId
        inner join TranMsts tm ON tm.Id = td.TranMstId
        inner join UnitInfos ui on ui.Id = ii.UnitId
        inner join Departments d on d.Id = tm.IssueDeptId
        where tm.IsDeleted = 0 and tm.TranType in ('I','L') and tm.TranDate < '{fromDate}'
        union all
        select ci.Id CategoryId, ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName UnitName,td.ItemQty RcvQty,0 IssQty,td.ItemQty * td.UnitPrice RcvAmount,0 IssueAmount,d.Id DepartmentId,d.Name DepartmentName
        from TranDtls td
        inner join ItemInfos ii ON ii.Id = td.ItemId
        inner join CategoryInfos ci on ci.Id = ii.CategoryId
        inner join TranMsts tm ON tm.Id = td.TranMstId
        inner join UnitInfos ui on ui.Id = ii.UnitId
        inner join Departments d on d.Id = tm.IssueDeptId
        where tm.IsDeleted = 0 and tm.TranType in ('O')
        union all
        select ci.Id CategoryId,ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName,0 RcvQty,td.ItemQty IssQty,0 RcvAmount,td.ItemQty * td.UnitPrice IssueAmount,d.Id DepartmentId,d.Name DepartmentName
        from TranDtls td
        inner join ItemInfos ii ON ii.Id = td.ItemId
        inner join CategoryInfos ci on ci.Id = ii.CategoryId
        inner join TranMsts tm ON tm.Id = td.TranMstId
        inner join UnitInfos ui on ui.Id = ii.UnitId
        inner join Departments d on d.Id = tm.IssueDeptId
        where tm.IsDeleted = 0 and tm.TranType in ('C') and tm.TranDate < '{fromDate}'
        )d group by d.CategoryId, d.CategoryName,d.ItemId,d.ItemName,d.UnitId,d.UnitName,d.DepartmentId,d.DepartmentName
        union all
        select ci.Id CategoryId, ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName UnitName,0 OpeningQty,0 OpeningAmount,isnull(sum(td.ItemQty),0) ReceiveQty,isnull(sum(td.ItemQty * td.UnitPrice),0) ReceiveAmount
        ,0 IssueQty,0 IssueAmount,0 ReceiveReturnQty,0 ReceiveReturnAmount,0 IssueReturnQty,0 IssueReturnAmount,0 ClosingQty,0 ClosingAmount,d.Id DepartmentId,d.Name DepartmentName
        from TranDtls td
        inner join ItemInfos ii ON ii.Id = td.ItemId
        inner join CategoryInfos ci on ci.Id = ii.CategoryId
        inner join TranMsts tm ON tm.Id = td.TranMstId
        inner join UnitInfos ui on ui.Id = ii.UnitId
        inner join Departments d on d.Id = tm.IssueDeptId
        where tm.IsDeleted = 0 and tm.TranType = 'I' and tm.TranDate between '{fromDate}' and '{toDate}' 
        group by ci.Id, ci.CategoryName,ii.Id,ii.ItemName,ui.Id,ui.UnitName,d.Id,d.Name
        union all
        select ci.Id CategoryId, ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName UnitName,0 OpeningQty,0 OpeningAmount,0 ReceiveQty,0 ReceiveAmount
        ,0 IssueQty,0 IssueAmount,0 ReceiveReturnQty,0 ReceiveReturnAmount,isnull(sum(td.ItemQty),0) IssueReturnQty,isnull(sum(td.ItemQty * td.UnitPrice),0) IssueReturnAmount,0 ClosingQty,0 ClosingAmount
        ,d.Id DepartmentId,d.Name DepartmentName
        from TranDtls td
        inner join ItemInfos ii ON ii.Id = td.ItemId
        inner join CategoryInfos ci on ci.Id = ii.CategoryId
        inner join TranMsts tm ON tm.Id = td.TranMstId
        inner join UnitInfos ui on ui.Id = ii.UnitId
        inner join Departments d on d.Id = tm.IssueDeptId
        where tm.IsDeleted = 0 and tm.TranType = 'L' and tm.TranDate between '{fromDate}' and '{toDate}' 
        group by ci.Id, ci.CategoryName,ii.Id,ii.ItemName,ui.Id,ui.UnitName,d.Id,d.Name
        union all
        select ci.Id CategoryId, ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName UnitName,0 OpeningQty,0 OpeningAmount,0 ReceiveQty,0 ReceiveAmount
        ,isnull(sum(td.ItemQty),0) IssueQty,isnull(sum(td.ItemQty * td.UnitPrice),0)  IssueAmount,0 ReceiveReturnQty,0 ReceiveReturnAmount,0 IssueReturnQty,0 IssueReturnAmount,0 ClosingQty,0 ClosingAmount
        ,d.Id DepartmentId,d.Name DepartmentName
        from TranDtls td
        inner join ItemInfos ii ON ii.Id = td.ItemId
        inner join CategoryInfos ci on ci.Id = ii.CategoryId
        inner join TranMsts tm ON tm.Id = td.TranMstId
        inner join UnitInfos ui on ui.Id = ii.UnitId
        inner join Departments d on d.Id = tm.IssueDeptId
        where tm.IsDeleted = 0 and tm.TranType = 'C' and tm.TranDate between '{fromDate}' and '{toDate}' 
        group by ci.Id, ci.CategoryName,ii.Id,ii.ItemName,ui.Id,ui.UnitName,d.Id,d.Name
        ) g where 1=1 {categoryIdQuery} {itemIdQuery} {departmentQuery}
        group by g.CategoryId, g.CategoryName,g.ItemId,g.ItemName,g.UnitId,g.UnitName,g.DepartmentId,g.DepartmentName order by g.CategoryName,g.ItemName";

        var data = await _iReadDbConnection.QueryAsync<StockRegisterVm>(query);
        return data.ToList();
    }

    #endregion

    #region StockReporthtml
    public async Task<string> StockDetailsReportHtml(StockVm vm)
    {
        var model = await GetInventoryStockInfo(vm);


        if (vm.StockStatus == (int)StockStatusEnum.InStock)
            model = model.Where(x => x.Stock > 0).ToList();
        else if (vm.StockStatus == (int)StockStatusEnum.OutOfStock)
            model = model.Where(x => x.Stock <= 0).ToList();

        var fullHtml = "";

        fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
        fullHtml += "<thead>";
        fullHtml += "<tr style='height:26px;font-size:12px;'>";
        fullHtml += "<th style='width:20px;text-align:center;'>Serial</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Category</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Item</th>";
        fullHtml += "<th style='width:60px;text-align:center;'>Unit</th>";
        fullHtml += "<th style='width:80px;text-align:right;'>Stock (Qty)</th>";
        fullHtml += "<th style='width:80px;text-align:right;'>Stock Value (tk)</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        int i = 0;
        foreach (var item in model)
        {
            fullHtml += "<tr style='height:22px;'>";
            fullHtml += $@"<td class='text-center'>{++i}</td>";
            fullHtml += $@"<td class='text-center'>{item.CategoryName}</td>";
            fullHtml += $@"<td class='text-center'>{item.ItemName}</td>";
            fullHtml += $@"<td class='text-center'>{item.UnitName}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.Stock:N2}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.StockAmount:N2}</td>";
            fullHtml += "</tr>";
        }
        fullHtml += "</tbody>";

        fullHtml += $@"<tfoot><tr style='height:25px;font-weight;bold;'><td colspan='4' style='text-align:right;'> Total : </td>
            <td style='text-align:right;'></td><td style='text-align:right;'>{model.Sum(o => o.StockAmount):N2}</td>
            </tr></tfoot>";

        fullHtml += "</table>";
        return fullHtml;
    }
    #endregion

    #region IssueReport

    public async Task<List<IssueReportVM>> GetItemIssueInfo(IssueReportVM vm)
    {
        var categoryIdQuery = vm.CategoryId > 0 ? $"and ci.Id = {vm.CategoryId}" : "";
        var itemIdQuery = vm.ItemId > 0 ? $"and ii.Id = {vm.ItemId}" : "";
        var deptIdQuery = vm.DepartmentId > 0 ? $"and dp.Id = {vm.DepartmentId}" : "";
        string fromDateQuery = (!string.IsNullOrEmpty(vm.StrFromDate)) ? $" and convert(date,tm.TranDate) >= '{Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)):dd/MMM/yyyy}'" : "";
        string toDateQuery = (!string.IsNullOrEmpty(vm.StrToDate)) ? $" and convert(date,tm.TranDate) <= '{Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)):dd/MMM/yyyy}'" : "";

        var query = $@"select tm.TranNo,tm.Id IssueId,convert(date,tm.TranDate)TranDate,ci.Id CategoryId, ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName UnitName
        ,dp.Id DepartmentId,dp.Name DepartmentName,ui.Id TranById,u.Id TranById,u.FullName TranByName,rq.Id ReqId,rq.ReqNo
        ,td.ItemQty IssueQty,isnull(ir.ReturnQty,0) ReturnQty,td.ItemQty - isnull(ir.ReturnQty,0) ActualIssueQty
        from TranDtls td
        inner join ItemInfos ii ON ii.Id = td.ItemId
        inner join CategoryInfos ci on ci.Id = ii.CategoryId
        inner join TranMsts tm ON tm.Id = td.TranMstId
        inner join UnitInfos ui on ui.Id = ii.UnitId
        inner join Departments dp on dp.Id = tm.IssueDeptId
        left join AspNetUsers u on u.Id = tm.TranById
        left join RequsitionInfos rq on rq.Id = tm.ReqMstId
        left join (
        select ii.Id,tm.RefTranId,sum(td.ItemQty) ReturnQty
        from TranDtls td
        inner join TranMsts tm ON tm.Id = td.TranMstId
        inner join ItemInfos ii ON ii.Id = td.ItemId
        where tm.TranType = 'E'
        group by ii.id,tm.RefTranId
        ) ir on ir.Id = ii.id and ir.RefTranId = tm.Id
        where tm.IsDeleted = 0 and td.IsDeleted =0 and tm.TranType = 'I' {categoryIdQuery} {itemIdQuery} {deptIdQuery} {fromDateQuery} {toDateQuery}
        --and convert(date,tm.TranDate) >= '01/Jan/2024' and convert(date,tm.TranDate) <= '31/Dec/2024'
        order by tm.TranDate,ci.CategoryName,ii.ItemName";

        var data = await _iReadDbConnection.QueryAsync<IssueReportVM>(query);
        return data.ToList();
    }

    #endregion

    #region IssueReportHtml
    public async Task<string> IssueReportHtml(IssueReportVM vm)
    {
        var model = await GetItemIssueInfo(vm);

        var fullHtml = "";

        fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
        fullHtml += "<thead>";
        fullHtml += "<tr style='height:26px;font-size:12px;'>";
        fullHtml += "<th style='width:20px;text-align:center;'>Serial</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Department</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Issue No</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Issue Date</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Category</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Item</th>";
        fullHtml += "<th style='width:60px;text-align:center;'>Unit</th>";
        fullHtml += "<th style='width:80px;text-align:right;'>Issue Qty</th>";
        fullHtml += "<th style='width:80px;text-align:right;'>Return Qty</th>";
        fullHtml += "<th style='width:80px;text-align:right;'>Actual Issue Qty</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        int i = 0;
        foreach (var item in model)
        {
            fullHtml += "<tr style='height:22px;'>";
            fullHtml += $@"<td class='text-center'>{++i}</td>";
            fullHtml += $@"<td class='text-center'>{item.DepartmentName}</td>";
            fullHtml += $@"<td class='text-center'>{item.TranNo}</td>";
            fullHtml += $@"<td class='text-center'>{item.TranDate:dd/MMM/yy}</td>";
            fullHtml += $@"<td class='text-center'>{item.CategoryName}</td>";
            fullHtml += $@"<td class='text-center'>{item.ItemName}</td>";
            fullHtml += $@"<td class='text-center'>{item.UnitName}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.IssueQty:N2}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.ReturnQty:N2}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.ActualIssueQty:N2}</td>";
            fullHtml += "</tr>";
        }
        fullHtml += "</tbody>";

        fullHtml += $@"<tfoot><tr style='height:25px;font-weight;bold;'><td colspan='7' style='text-align:right;'> Total : </td>
            <td style='text-align:right;'>{model.Sum(o => o.IssueQty):N2}</td><td style='text-align:right;'>{model.Sum(o => o.ReturnQty):N2}</td>
            <td style='text-align:right;'>{model.Sum(o => o.ActualIssueQty):N2}</td>
            </tr></tfoot>";

        fullHtml += "</table>";
        return fullHtml;
    }
    #endregion

    #region GetItemReceiveInfo

    public async Task<List<ReceiveReportVM>> GetItemReceiveInfo(ReceiveReportVM vm)
    {
        var categoryIdQuery = vm.CategoryId > 0 ? $"and ci.Id = {vm.CategoryId}" : "";
        var itemIdQuery = vm.ItemId > 0 ? $"and ii.Id = {vm.ItemId}" : "";
        string fromDateQuery = (!string.IsNullOrEmpty(vm.StrFromDate)) ? $" and convert(date,tm.TranDate) >= '{Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)):dd/MMM/yyyy}'" : "";
        string toDateQuery = (!string.IsNullOrEmpty(vm.StrToDate)) ? $" and convert(date,tm.TranDate) <= '{Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)):dd/MMM/yyyy}'" : "";
        var ledgerIdQuery = vm.LedgerId > 0 ? $"and tm.LedgerId = {vm.LedgerId}" : "";

        var query = $@"select tm.Id TranId,tm.TranDate,tm.TranNo,tm.LedgerId,si.id SupplierId,si.SupplierName,si.SupplierCode,om.id OrderId,om.OrderNo,om.OrderDate,om.OrderType,ci.Id CategoryId,ci.CategoryName,ci.CategoryCode
        ,ii.Id ItemId,ii.ItemCode,ii.ItemName,td.ItemQty,td.UnitPrice,ui.Id UnitId,ui.UnitName ,round(td.ItemQty * td.UnitPrice,2) TotalAmount 
        from TranDtls td
        inner join UnitInfos ui on ui.Id = td.ItemUnitId
        inner join ItemInfos ii on ii.Id = td.ItemId
        inner join CategoryInfos ci on ci.Id = ii.CategoryId
        inner join TranMsts tm on tm.Id = td.TranMstId
        left join SupplierInfos si on si.Id = tm.SupplierId
        left join OrderMsts om on om.Id = tm.OrderId
        where tm.TranType = 'R' and td.IsDeleted = 0 and tm.IsDeleted = 0 {categoryIdQuery} {itemIdQuery} {fromDateQuery} {toDateQuery} {ledgerIdQuery}
        order by tm.TranDate,tm.Id";

        var data = await _iReadDbConnection.QueryAsync<ReceiveReportVM>(query);
        return data.ToList();
    }

    #endregion

    #region ReceiveReportHtml
    public async Task<string> ReceiveReportHtml(ReceiveReportVM vm)
    {
        var model = await GetItemReceiveInfo(vm);

        AccLedger ledger = null;
        if (vm.LedgerId > 0)
        {
            ledger = _iAccLedgerRepository.GetFirstOrDefault(x => x.Id == vm.LedgerId);
        }

        var fullHtml = "";

        fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
        fullHtml += "<thead>";
        if (vm.LedgerId > 0)
        {
            fullHtml += $@"<tr><th colspan='10'>{ledger?.LedgerName}</th></tr>";
        }
        fullHtml += "<tr style='height:26px;font-size:12px;'>";
        //fullHtml += "<th style='width:20px;text-align:center;'>Serial</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Order No</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Supplier</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Receive No</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Receive Date</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Category</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Item</th>";
        fullHtml += "<th style='width:60px;text-align:center;'>Unit</th>";
        fullHtml += "<th style='width:80px;text-align:right;'>Qty</th>";
        fullHtml += "<th style='width:80px;text-align:right;'>Unit Price</th>";
        fullHtml += "<th style='width:80px;text-align:right;'>Amount</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        //int i = 0;
        long preTranId = 0;
        double totalAmount = 0;
        double totalQty = 0;

        foreach (var (item, i) in model.GetItemWithIndex())
        {
            bool isLastItemOfTranId = (i == model.Count - 1) || model[i + 1].TranId != item.TranId;

            if (preTranId != item.TranId)
            {
                totalAmount = 0; // Reset total for new TranId
                totalQty = 0; // Reset total for new TranId
                int rowCount = model.Where(o => o.TranId == item.TranId).ToList().Count();
                fullHtml += "<tr style='height:22px;'>";
                //fullHtml += $@"<td class='text-center'>{++i}</td>";
                fullHtml += $@"<td class='text-center' rowspan='{rowCount}'>{item.OrderNo}</td>";
                fullHtml += $@"<td class='text-center' rowspan='{rowCount}'>{item.SupplierName}</td>";
                fullHtml += $@"<td class='text-center' rowspan='{rowCount}'>{item.TranNo}</td>";
                fullHtml += $@"<td class='text-center' rowspan='{rowCount}'>{item.TranDate:dd/MMM/yy}</td>";
                fullHtml += $@"<td class='text-center'>{item.CategoryName}</td>";
                fullHtml += $@"<td class='text-center'>{item.ItemName}</td>";
                fullHtml += $@"<td class='text-center'>{item.UnitName}</td>";
                fullHtml += $@"<td style='text-align:right;'>{item.ItemQty:N2}</td>";
                fullHtml += $@"<td style='text-align:right;'>{item.UnitPrice:N2}</td>";
                fullHtml += $@"<td style='text-align:right;'>{item.TotalAmount:N2}</td>";
                fullHtml += "</tr>";
            }
            else
            {
                fullHtml += "<tr style='height:22px;'>";
                //fullHtml += $@"<td class='text-center'>{++i}</td>";
                //fullHtml += $@"<td class='text-center'>{item.OrderNo}</td>";
                //fullHtml += $@"<td class='text-center'>{item.SupplierName}</td>";
                //fullHtml += $@"<td class='text-center'>{item.TranNo}</td>";
                //fullHtml += $@"<td class='text-center'>{item.TranDate:dd/MMM/yy}</td>";
                fullHtml += $@"<td class='text-center'>{item.CategoryName}</td>";
                fullHtml += $@"<td class='text-center'>{item.ItemName}</td>";
                fullHtml += $@"<td class='text-center'>{item.UnitName}</td>";
                fullHtml += $@"<td style='text-align:right;'>{item.ItemQty:N2}</td>";
                fullHtml += $@"<td style='text-align:right;'>{item.UnitPrice:N2}</td>";
                fullHtml += $@"<td style='text-align:right;'>{item.TotalAmount:N2}</td>";
                fullHtml += "</tr>";
            }

            totalAmount += item.TotalAmount;
            totalQty += item.ItemQty;

            if (isLastItemOfTranId)
            {
                // Add total row for the current TranId
                fullHtml += "<tr style='height:22px; background-color: #f0f0f0; font-weight: bold;'>";
                fullHtml += $@"<td colspan='7' style='text-align:right;'>Total:</td>";
                fullHtml += $@"<td style='text-align:right;'>{totalQty:N2}</td>";
                fullHtml += $@"<td style='text-align:right;'></td>";
                fullHtml += $@"<td style='text-align:right;'>{totalAmount:N2}</td>";
                fullHtml += "</tr>";
            }

            preTranId = item.TranId;
        }
        fullHtml += "</tbody>";

        fullHtml += $@"<tfoot><tr style='height:25px;background-color:#f0f0f0;font-weight: bold;'>
            <td colspan='7' style='text-align:right;'> Total : </td>
            <td style='text-align:right;'>{model.Sum(o => o.ItemQty):N2}</td><td style='text-align:right;'></td>
            <td style='text-align:right;'>{model.Sum(o => o.TotalAmount):N2}</td>
            </tr></tfoot>";

        fullHtml += "</table>";
        return fullHtml;
    }
    #endregion

    #region DepartmentWiseStockReport

    public async Task<List<DepartmentStockVm>> DepartmentWiseStockReport(DepartmentStockVm vm)
    {
        var departmentIdQuery = vm.DepartmentId > 0 ? $"and d.DepartmentId = {vm.DepartmentId}" : "";
        var categoryIdQuery = vm.CategoryId > 0 ? $"and d.CategoryId = {vm.CategoryId}" : "";
        var itemIdQuery = vm.ItemId > 0 ? $"and d.ItemId = {vm.ItemId}" : "";

        #region WithoutStockAmount
        //var query = $@"select d.CategoryType,d.CategoryId,d.CategoryName,d.ItemId,d.ItemName,d.UnitId,d.UnitName,sum(isnull(d.RcvQty,0)) TotalRcvQty,sum(isnull(d.IssQty,0)) TotalConsumptionQty
        //    ,sum(isnull(d.RcvQty,0)) - sum(isnull(d.IssQty,0)) Stock,d.DepartmentId,d.DepartmentName
        //    from (
        //    select ci.CategoryType,ci.Id CategoryId, ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName UnitName,td.ItemQty RcvQty,0 IssQty,dp.Id DepartmentId,dp.Name DepartmentName
        //    from TranDtls td
        //    inner join ItemInfos ii ON ii.Id = td.ItemId
        //    inner join CategoryInfos ci on ci.Id = ii.CategoryId
        //    inner join TranMsts tm ON tm.Id = td.TranMstId
        //    inner join UnitInfos ui on ui.Id = ii.UnitId
        //    inner join Departments dp on dp.Id = tm.IssueDeptId
        //    where tm.IsDeleted = 0 and tm.TranType in ('I','O') 
        //    union all
        //    select ci.CategoryType,ci.Id CategoryId,ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName,0 RcvQty,td.ItemQty IssQty,dp.Id DepartmentId,dp.Name DepartmentName
        //    from TranDtls td
        //    inner join ItemInfos ii ON ii.Id = td.ItemId
        //    inner join CategoryInfos ci on ci.Id = ii.CategoryId
        //    inner join TranMsts tm ON tm.Id = td.TranMstId
        //    inner join UnitInfos ui on ui.Id = ii.UnitId
        //    inner join Departments dp on dp.Id = tm.IssueDeptId
        //    where tm.IsDeleted = 0 and tm.TranType in ('E','C')
        //    union all
        //    select ci.CategoryType,ci.Id CategoryId,ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName,0 RcvQty,-td.ItemQty IssQty,dp.Id DepartmentId,dp.Name DepartmentName
        //    from TranDtls td
        //    inner join ItemInfos ii ON ii.Id = td.ItemId
        //    inner join CategoryInfos ci on ci.Id = ii.CategoryId
        //    inner join TranMsts tm ON tm.Id = td.TranMstId
        //    inner join UnitInfos ui on ui.Id = ii.UnitId
        //    inner join Departments dp on dp.Id = tm.IssueDeptId
        //    where tm.IsDeleted = 0 and tm.TranType = 'L'
        //    ) d 
        //    where d.ItemId > 0 {departmentIdQuery} {categoryIdQuery} {itemIdQuery}
        //    group by d.CategoryType,d.CategoryId,d.CategoryName,d.ItemId,d.ItemName,d.UnitId,d.UnitName,d.DepartmentId,d.DepartmentName";
        #endregion

        #region WithStockAmount
        //I=ISSUE,O=OPENING,L=LEFT OVER,C=CONSUMPTION,E=ISSUE RETURN
        var query = $@"select d.CategoryType,d.CategoryId,d.CategoryName,d.ItemId,d.ItemName,d.UnitId,d.UnitName,sum(isnull(d.RcvQty,0)) TotalRcvQty,sum(isnull(d.IssQty,0)) TotalConsumptionQty
            ,sum(isnull(d.RcvQty,0)) - sum(isnull(d.IssQty,0)) Stock,d.DepartmentId,d.DepartmentName
            ,sum(isnull(d.RcvAmount,0)) TotalRcvAmount,sum(isnull(d.IssAmount,0)) TotalConsumptionAmount
            ,sum(isnull(d.RcvAmount,0)) - sum(isnull(d.IssAmount,0)) StockAmount,case when (sum(isnull(d.RcvQty,0)) - sum(isnull(d.IssQty,0))) > 0 and  
            sum(isnull(d.RcvAmount,0)) - sum(isnull(d.IssAmount,0)) > 0 then round((sum(isnull(d.RcvAmount,0)) - sum(isnull(d.IssAmount,0))) / (sum(isnull(d.RcvQty,0)) - sum(isnull(d.IssQty,0))),2)
            else 0 end UnitPrice
            from (
            select ci.CategoryType,ci.Id CategoryId, ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName UnitName,td.ItemQty RcvQty,0 IssQty,dp.Id DepartmentId,dp.Name DepartmentName
            ,td.ItemQty * td.UnitPrice RcvAmount,0 IssAmount
            from TranDtls td
            inner join ItemInfos ii ON ii.Id = td.ItemId
            inner join CategoryInfos ci on ci.Id = ii.CategoryId
            inner join TranMsts tm ON tm.Id = td.TranMstId
            inner join UnitInfos ui on ui.Id = ii.UnitId
            inner join Departments dp on dp.Id = tm.IssueDeptId
            where tm.IsDeleted = 0 and tm.TranType in ('I','O') 
            union all
            select ci.CategoryType,ci.Id CategoryId,ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName,0 RcvQty,td.ItemQty IssQty,dp.Id DepartmentId,dp.Name DepartmentName
            ,0 RcvAmount,td.ItemQty * td.UnitPrice IssAmount
            from TranDtls td
            inner join ItemInfos ii ON ii.Id = td.ItemId
            inner join CategoryInfos ci on ci.Id = ii.CategoryId
            inner join TranMsts tm ON tm.Id = td.TranMstId
            inner join UnitInfos ui on ui.Id = ii.UnitId
            inner join Departments dp on dp.Id = tm.IssueDeptId
            where tm.IsDeleted = 0 and tm.TranType in ('E','C')
            union all
            select ci.CategoryType,ci.Id CategoryId,ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName,0 RcvQty,-td.ItemQty IssQty,dp.Id DepartmentId,dp.Name DepartmentName
            ,0 RcvAmount,td.ItemQty * td.UnitPrice IssAmount
            from TranDtls td
            inner join ItemInfos ii ON ii.Id = td.ItemId
            inner join CategoryInfos ci on ci.Id = ii.CategoryId
            inner join TranMsts tm ON tm.Id = td.TranMstId
            inner join UnitInfos ui on ui.Id = ii.UnitId
            inner join Departments dp on dp.Id = tm.IssueDeptId
            where tm.IsDeleted = 0 and tm.TranType = 'L'
            ) d 
            where d.ItemId > 0 {departmentIdQuery} {categoryIdQuery} {itemIdQuery}
            group by d.CategoryType,d.CategoryId,d.CategoryName,d.ItemId,d.ItemName,d.UnitId,d.UnitName,d.DepartmentId,d.DepartmentName";

        #endregion

        var data = await _iReadDbConnection.QueryAsync<DepartmentStockVm>(query);
        return data.ToList();
    }

    #endregion

    #region DepartmentWiseStockHtml
    public async Task<string> DepartmentWiseStockReportHtml(DepartmentStockVm vm)
    {
        var model = await DepartmentWiseStockReport(vm);

        if (vm.StockStatus == (int)StockStatusEnum.InStock)
            model = model.Where(x => x.Stock > 0).ToList();
        else if (vm.StockStatus == (int)StockStatusEnum.OutOfStock)
            model = model.Where(x => x.Stock <= 0).ToList();

        var departmentName = model.FirstOrDefault()?.DepartmentName;

        var fullHtml = "";

        fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
        fullHtml += "<thead>";
        if (vm.DepartmentId > 0)
        {
            fullHtml += $@"<tr><th colspan='8' style='text-align:center;font-size:14px;'>{departmentName}</th></tr>";
        }
        fullHtml += "<tr style='height:26px;font-size:12px;'>";
        fullHtml += "<th style='width:20px;text-align:center;'>Sl No</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Category</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Item</th>";
        fullHtml += "<th style='width:60px;text-align:center;'>Unit</th>";
        fullHtml += "<th style='width:60px;text-align:center;'>Total Receive</th>";
        fullHtml += "<th style='width:60px;text-align:center;'>Total Consume</th>";
        fullHtml += "<th style='width:80px;text-align:right;'>Stock (Qty)</th>";
        fullHtml += "<th style='width:80px;text-align:right;'>Stock Value (BDT)</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        int i = 0;
        foreach (var item in model)
        {
            var stockAmount = item.Stock > 0 && item.StockAmount > 0 ? item.StockAmount : 0;

            fullHtml += "<tr style='height:22px;'>";
            fullHtml += $@"<td class='text-center'>{++i}</td>";
            fullHtml += $@"<td class='text-center'>{item.CategoryName}</td>";
            fullHtml += $@"<td class='text-center'>{item.ItemName}</td>";
            fullHtml += $@"<td class='text-center'>{item.UnitName}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.TotalRcvQty:N2}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.TotalConsumptionQty:N2}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.Stock:N2}</td>";
            fullHtml += $@"<td style='text-align:right;'>{stockAmount:N2}</td>";
            fullHtml += "</tr>";
        }
        fullHtml += "</tbody>";

        fullHtml += $@"<tfoot><tr style='height:25px;font-weight;bold;'><td colspan='4' style='text-align:right;'><b> Total : </b></td>
            <td style='text-align:right;'><b>{model.Sum(x => x.TotalRcvQty):N2}</b></td>
            <td style='text-align:right;'><b>{model.Sum(x => x.TotalConsumptionQty):N2}</b></td>
            <td style='text-align:right;'><b>{model.Sum(x => x.Stock):N2}</b></td>
            <td style='text-align:right;'><b>{model.Where(x => x.Stock > 0 && x.StockAmount > 0).Sum(x => x.StockAmount):N2}</b></td>
            </tr></tfoot>";

        fullHtml += "</table>";
        return fullHtml;
    }
    #endregion

    #region StockRegisterReportHtml
    public async Task<string> StockRegisterReportHtml(StockRegisterVm vm)
    {
        var model = await GetStockRegisterInfo(vm);

        if (vm.StockStatus == (int)StockStatusEnum.InStock)
            model = model.Where(x => x.ClosingQty >= 0.5).ToList();
        else if (vm.StockStatus == (int)StockStatusEnum.OutOfStock)
            model = model.Where(x => x.ClosingQty < 0.5).ToList();

        var fullHtml = "";

        fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
        fullHtml += "<thead>";
        fullHtml += "<tr style='height:26px;font-size:12px;'>";
        fullHtml += "<th rowspan='3' style='width:20px;text-align:center;'>Serial</th>";
        fullHtml += "<th rowspan='3' style='width:100px;text-align:center;'>Category</th>";
        fullHtml += "<th rowspan='3' style='width:100px;text-align:center;'>Item</th>";
        fullHtml += "<th rowspan='3' style='width:60px;text-align:center;'>Unit</th>";
        fullHtml += "<th colspan='6' style='text-align:center;'>Stock In</th>";
        fullHtml += "<th colspan='4' style='text-align:center;'>Stock Out</th>";
        fullHtml += "<th colspan='2' style='text-align:center;'>Closing Stock</th>";
        fullHtml += "</tr>";
        fullHtml += "<tr style='height:26px;font-size:12px;'>";
        fullHtml += "<th colspan='2' style='text-align:center;'>Opening</th>";
        fullHtml += "<th colspan='2' style='text-align:center;'>Receive</th>";
        fullHtml += "<th colspan='2' style='text-align:center;'>Issue Return</th>";
        fullHtml += "<th colspan='2' style='text-align:center;'>Issue</th>";
        fullHtml += "<th colspan='2' style='text-align:center;'>Rcv. Return</th>";
        fullHtml += "<th rowspan='2' style='text-align:center;'>Qty</th>";
        fullHtml += "<th rowspan='2' style='text-align:center;'>Amount</th>";
        fullHtml += "</tr>";
        fullHtml += "<tr style='height:26px;font-size:12px;'>";
        fullHtml += "<th style='text-align:center;'>Qty</th><th style='text-align:center;'>Amount</th>";
        fullHtml += "<th style='text-align:center;'>Qty</th><th style='text-align:center;'>Amount</th>";
        fullHtml += "<th style='text-align:center;'>Qty</th><th style='text-align:center;'>Amount</th>";
        fullHtml += "<th style='text-align:center;'>Qty</th><th style='text-align:center;'>Amount</th>";
        fullHtml += "<th style='text-align:center;'>Qty</th><th style='text-align:center;'>Amount</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        int i = 0;
        foreach (var item in model)
        {
            fullHtml += "<tr style='height:22px;'>";
            fullHtml += $@"<td class='text-center'>{++i}</td>";
            fullHtml += $@"<td class='text-center'>{item.CategoryName}</td>";
            fullHtml += $@"<td class='text-center'>{item.ItemName}</td>";
            fullHtml += $@"<td class='text-center'>{item.UnitName}</td>";

            fullHtml += $@"<td style='text-align:right;'>{item.OpeningQty:N2}</td>";
            var openingAmount = item.OpeningQty > 0 && item.OpeningAmount > 0 ? item.OpeningAmount : 0;
            fullHtml += $@"<td style='text-align:right;'>{openingAmount:N2}</td>";

            fullHtml += $@"<td style='text-align:right;'>{item.ReceiveQty:N2}</td>";
            var receiveAmount = item.ReceiveQty > 0 && item.ReceiveAmount > 0 ? item.ReceiveAmount : 0;
            fullHtml += $@"<td style='text-align:right;'>{receiveAmount:N2}</td>";

            fullHtml += $@"<td style='text-align:right;'>{item.IssueReturnQty:N2}</td>";
            var issueReturnAmount = item.IssueReturnQty > 0 && item.IssueReturnAmount > 0 ? item.IssueReturnAmount : 0;
            fullHtml += $@"<td style='text-align:right;'>{issueReturnAmount:N2}</td>";

            fullHtml += $@"<td style='text-align:right;'>{item.IssueQty:N2}</td>";
            var issueAmount = item.IssueQty > 0 && item.IssueAmount > 0 ? item.IssueAmount : 0;
            fullHtml += $@"<td style='text-align:right;'>{issueAmount:N2}</td>";

            fullHtml += $@"<td style='text-align:right;'>{item.ReceiveReturnQty:N2}</td>";
            var receiveReturnAmount = item.ReceiveReturnQty > 0 && item.ReceiveReturnAmount > 0 ? item.ReceiveReturnAmount : 0;
            fullHtml += $@"<td style='text-align:right;'>{receiveReturnAmount:N2}</td>";

            fullHtml += $@"<td style='text-align:right;'>{item.ClosingQty:F2}</td>";
            var closingAmount = item.ClosingQty > 0 && item.ClosingAmount > 0 ? item.ClosingAmount : 0;
            fullHtml += $@"<td style='text-align:right;'>{closingAmount:N2}</td>";
            fullHtml += "</tr>";
        }
        fullHtml += "</tbody>";

        fullHtml += $@"<tfoot><tr style='height:25px;font-weight;bold;'>
            <td colspan='4' style='text-align:right;'><b> Total : </b></td>
            <td style='text-align:right;'><b>{model.Sum(o => o.OpeningQty):N2}</b></td>
            <td style='text-align:right;'><b>{model.Where(x => x.OpeningQty > 0 && x.OpeningAmount > 0).Sum(o => o.OpeningAmount):N2}</b></td>
            <td style='text-align:right;'><b>{model.Sum(o => o.ReceiveQty):N2}</b></td>
            <td style='text-align:right;'><b>{model.Where(x => x.ReceiveQty > 0 && x.ReceiveAmount > 0).Sum(o => o.ReceiveAmount):N2}</b></td>
            <td style='text-align:right;'><b>{model.Sum(o => o.IssueReturnQty):N2}</b></td>
            <td style='text-align:right;'><b>{model.Where(x => x.IssueReturnQty > 0 && x.IssueReturnAmount > 0).Sum(o => o.IssueReturnAmount):N2}</b></td>
            <td style='text-align:right;'><b>{model.Sum(o => o.IssueQty):N2}</b></td>
            <td style='text-align:right;'><b>{model.Where(x => x.IssueQty > 0 && x.IssueAmount > 0).Sum(o => o.IssueAmount):N2}</b></td>
            <td style='text-align:right;'><b>{model.Sum(o => o.ReceiveReturnQty):N2}</b></td>
            <td style='text-align:right;'><b>{model.Where(x => x.ReceiveReturnQty > 0 && x.ReceiveReturnAmount > 0).Sum(o => o.ReceiveReturnAmount):N2}</b></td>
            <td style='text-align:right;'><b>{model.Sum(o => o.ClosingQty):N2}</b></td>
            <td style='text-align:right;'><b>{model.Where(x => x.ClosingQty > 0 && x.ClosingAmount > 0).Sum(o => o.ClosingAmount):N2}</b></td>
            </tr></tfoot>";

        fullHtml += "</table>";
        return fullHtml;
    }
    #endregion

    #region DpartmentStockRegisterReportHtml
    public async Task<string> DepartmentStockRegisterReportHtml(StockRegisterVm vm)
    {
        var model = await GetDepartmentStockRegisterInfo(vm);

        var fullHtml = "";

        fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
        fullHtml += "<thead>";
        fullHtml += "<tr style='height:26px;font-size:12px;'>";
        fullHtml += "<th rowspan='2' style='width:30px;text-align:center;'>Serial</th>";
        fullHtml += "<th rowspan='2' style='width:180px;text-align:center;'>Category</th>";
        fullHtml += "<th rowspan='2' style='width:300px;text-align:center;'>Item</th>";
        fullHtml += "<th rowspan='2' style='width:80px;text-align:center;'>Unit</th>";
        fullHtml += "<th colspan='3' style='text-align:center;'>Stock In</th>";
        fullHtml += "<th colspan='1' style='text-align:center;'>Stock Out</th>";
        fullHtml += "<th colspan='1' style='text-align:center;'>Closing Stock</th>";
        fullHtml += "</tr>";
        fullHtml += "<tr style='height:26px;font-size:12px;'>";
        fullHtml += "<th style='text-align:center;'>Opening Qty</th>";
        fullHtml += "<th style='text-align:center;'>Receive Qty</th>";
        fullHtml += "<th style='text-align:center;'>Left Over Qty</th>";
        fullHtml += "<th style='text-align:center;'>Consumption Qty</th>";
        fullHtml += "<th style='text-align:center;'>Qty</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        int i = 0;
        foreach (var item in model)
        {
            fullHtml += "<tr style='height:22px;'>";
            fullHtml += $@"<td class='text-center'>{++i}</td>";
            fullHtml += $@"<td class='text-center'>{item.CategoryName}</td>";
            fullHtml += $@"<td class='text-center'>{item.ItemName}</td>";
            fullHtml += $@"<td class='text-center'>{item.UnitName}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.OpeningQty:N2}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.ReceiveQty:N2}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.LeftOverQty:N2}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.IssueQty:N2}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.ClosingQty:N2}</td>";
            fullHtml += "</tr>";
        }
        fullHtml += "</tbody>";

        fullHtml += $@"<tfoot><tr style='height:25px;font-weight;bold;'>
            <td colspan='4' style='text-align:right;'> Total : </td>
            <td style='text-align:right;'>{model.Sum(o => o.OpeningQty):N2}</td>
            <td style='text-align:right;'>{model.Sum(o => o.ReceiveQty):N2}</td>
            <td style='text-align:right;'>{model.Sum(o => o.LeftOverQty):N2}</td>
            <td style='text-align:right;'>{model.Sum(o => o.IssueQty):N2}</td>
            <td style='text-align:right;'>{model.Sum(o => o.ClosingQty):N2}</td>
            </tr></tfoot>";

        fullHtml += "</table>";
        return fullHtml;
    }
    #endregion

    #region DpartmentStockRegisterReportHtml
    public async Task<string> DepartmentStockRegisterReportWithAmountHtml(StockRegisterVm vm)
    {
        var model = await GetDepartmentStockRegisterInfo(vm);

        var departmentName = model.FirstOrDefault()?.DepartmentName;

        if (vm.StockStatus == (int)StockStatusEnum.InStock)
            model = model.Where(x => x.ClosingQty > 0).ToList();
        else if (vm.StockStatus == (int)StockStatusEnum.OutOfStock)
            model = model.Where(x => x.ClosingQty <= 0).ToList();

        var fullHtml = "";

        fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
        fullHtml += "<thead>";

        if (vm.DepartmentId > 0)
        {
            fullHtml += $@"<tr><th colspan='14' style='text-align:center;font-size:14px;'>{departmentName}</th></tr>";
        }

        fullHtml += "<tr style='height:26px;font-size:12px;'>";
        fullHtml += "<th rowspan='3' style='width:30px;text-align:center;'>Serial</th>";
        fullHtml += "<th rowspan='3' style='width:180px;text-align:center;'>Category</th>";
        fullHtml += "<th rowspan='3' style='width:300px;text-align:center;'>Item</th>";
        fullHtml += "<th rowspan='3' style='width:80px;text-align:center;'>Unit</th>";
        fullHtml += "<th colspan='6' style='text-align:center;'>Stock In</th>";
        fullHtml += "<th colspan='2' style='text-align:center;'>Stock Out</th>";
        fullHtml += "<th colspan='2' style='text-align:center;'>Closing Stock</th>";
        fullHtml += "</tr>";
        fullHtml += "<tr style='height:26px;font-size:12px;'>";
        fullHtml += "<th colspan='2' style='text-align:center;'>Opening Qty</th>";
        fullHtml += "<th colspan='2' style='text-align:center;'>Receive Qty</th>";
        fullHtml += "<th colspan='2' style='text-align:center;'>Left Over Qty</th>";
        fullHtml += "<th colspan='2' style='text-align:center;'>Consumption Qty</th>";
        fullHtml += "<th colspan='2' style='text-align:center;'>Qty</th>";
        fullHtml += "</tr>";
        fullHtml += "<tr style='height:26px;font-size:12px;'>";
        fullHtml += "<th style='text-align:center;'>Qty</th><th style='text-align:center;'>Amount</th>";
        fullHtml += "<th style='text-align:center;'>Qty</th><th style='text-align:center;'>Amount</th>";
        fullHtml += "<th style='text-align:center;'>Qty</th><th style='text-align:center;'>Amount</th>";
        fullHtml += "<th style='text-align:center;'>Qty</th><th style='text-align:center;'>Amount</th>";
        fullHtml += "<th style='text-align:center;'>Qty</th><th style='text-align:center;'>Amount</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        int i = 0;
        foreach (var item in model)
        {
            fullHtml += "<tr style='height:22px;'>";
            fullHtml += $@"<td class='text-center'>{++i}</td>";
            fullHtml += $@"<td class='text-center'>{item.CategoryName}</td>";
            fullHtml += $@"<td class='text-center'>{item.ItemName}</td>";
            fullHtml += $@"<td class='text-center'>{item.UnitName}</td>";

            fullHtml += $@"<td style='text-align:right;'>{item.OpeningQty:N2}</td>";
            var openingAmount = item.OpeningQty > 0 && item.OpeningAmount > 0 ? item.OpeningAmount : 0;
            fullHtml += $@"<td style='text-align:right;'>{openingAmount:N2}</td>";

            fullHtml += $@"<td style='text-align:right;'>{item.ReceiveQty:N2}</td>";
            var receiveAmount = item.ReceiveQty > 0 && item.ReceiveAmount > 0 ? item.ReceiveAmount : 0;
            fullHtml += $@"<td style='text-align:right;'>{receiveAmount:N2}</td>";

            fullHtml += $@"<td style='text-align:right;'>{item.LeftOverQty:N2}</td>";
            var leftOverAmount = item.LeftOverQty > 0 && item.LeftOverAmount > 0 ? item.LeftOverAmount : 0;
            fullHtml += $@"<td style='text-align:right;'>{leftOverAmount:N2}</td>";

            fullHtml += $@"<td style='text-align:right;'>{item.IssueQty:N2}</td>";
            var issueAmount = item.IssueQty > 0 && item.IssueAmount > 0 ? item.IssueAmount : 0;
            fullHtml += $@"<td style='text-align:right;'>{issueAmount:N2}</td>";

            fullHtml += $@"<td style='text-align:right;'>{item.ClosingQty:N2}</td>";
            var closingAmount = item.ClosingQty > 0 && item.ClosingAmount > 0 ? item.ClosingAmount : 0;
            fullHtml += $@"<td style='text-align:right;'>{closingAmount:N2}</td>";

            fullHtml += "</tr>";
        }
        fullHtml += "</tbody>";

        fullHtml += $@"<tfoot><tr style='height:25px;font-weight;bold;'>
            <td colspan='4' style='text-align:right;'> Total : </td>
            <td style='text-align:right;'><b>{model.Sum(o => o.OpeningQty):N2}</b></td>
            <td style='text-align:right;'><b>{model.Where(x => x.OpeningQty > 0 && x.OpeningAmount > 0).Sum(o => o.OpeningAmount):N2}</b></td>
            <td style='text-align:right;'><b>{model.Sum(o => o.ReceiveQty):N2}</b></td>
            <td style='text-align:right;'><b>{model.Where(x => x.ReceiveQty > 0 && x.ReceiveAmount > 0).Sum(o => o.ReceiveAmount):N2}</b></td>
            <td style='text-align:right;'><b>{model.Sum(o => o.LeftOverQty):N2}</b></td>
            <td style='text-align:right;'><b>{model.Where(x => x.LeftOverQty > 0 && x.LeftOverAmount > 0).Sum(o => o.LeftOverAmount):N2}</b></td>
            <td style='text-align:right;'><b>{model.Sum(o => o.IssueQty):N2}</b></td>
            <td style='text-align:right;'><b>{model.Where(x => x.IssueQty > 0 && x.IssueAmount > 0).Sum(o => o.IssueAmount):N2}</b></td>
            <td style='text-align:right;'><b>{model.Sum(o => o.ClosingQty):N2}</b></td>
            <td style='text-align:right;'><b>{model.Where(x => x.ClosingQty > 0 && x.ClosingAmount > 0).Sum(o => o.ClosingAmount):N2}</b></td>
            </tr></tfoot>";

        fullHtml += "</table>";
        return fullHtml;
    }
    #endregion

    #region ConsumeReport

    public async Task<List<ConsumeReportVm>> GetItemConsumeInfo(ConsumeReportVm vm)
    {
        var categoryIdQuery = vm.CategoryId > 0 ? $"and ci.Id = {vm.CategoryId}" : "";
        var itemIdQuery = vm.ItemId > 0 ? $"and ii.Id = {vm.ItemId}" : "";
        var deptIdQuery = vm.DepartmentId > 0 ? $"and dp.Id = {vm.DepartmentId}" : "";
        string fromDateQuery = (!string.IsNullOrEmpty(vm.StrFromDate)) ? $" and convert(date,tm.TranDate) >= '{Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)):dd/MMM/yyyy}'" : "";
        string toDateQuery = (!string.IsNullOrEmpty(vm.StrToDate)) ? $" and convert(date,tm.TranDate) <= '{Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)):dd/MMM/yyyy}'" : "";

        var query = $@"select tm.TranNo,tm.Id ConsumeId,convert(date,tm.TranDate)TranDate,ci.Id CategoryId, ci.CategoryName,ii.Id ItemId,ii.ItemName,ui.Id UnitId,ui.UnitName UnitName
        ,dp.Id DepartmentId,dp.Name DepartmentName,hr.Id IssueRoomId,hr.RoomNo IssueRoomNo,u.Id TranById,u.FullName TranByName,td.ItemQty ConsumeQty, td.Remarks Remarks
        from TranDtls td
        inner join ItemInfos ii ON ii.Id = td.ItemId
        inner join CategoryInfos ci on ci.Id = ii.CategoryId
        inner join TranMsts tm ON tm.Id = td.TranMstId
        inner join UnitInfos ui on ui.Id = ii.UnitId
        inner join Departments dp on dp.Id = tm.IssueDeptId
        left join HtRoomInfos hr on hr.Id = tm.IssueRoomId
        left join AspNetUsers u on u.Id = tm.TranById
        left join RequsitionInfos rq on rq.Id = tm.ReqMstId
        where tm.IsDeleted = 0 and td.IsDeleted =0 and tm.TranType = 'C' {categoryIdQuery} {itemIdQuery} {deptIdQuery} {fromDateQuery} {toDateQuery}
        order by tm.TranDate,ci.CategoryName,ii.ItemName";

        var data = await _iReadDbConnection.QueryAsync<ConsumeReportVm>(query);
        return data.ToList();
    }

    #endregion

    #region ConsumeReportHtml
    public async Task<string> ConsumeReportHtml(ConsumeReportVm vm)
    {
        var model = await GetItemConsumeInfo(vm);


        //if (vm.StockStatus == (int)StockStatusEnum.InStock)
        //    model = model.Where(x => x.Stock > 0).ToList();
        //else if (vm.StockStatus == (int)StockStatusEnum.OutOfStock)
        //    model = model.Where(x => x.Stock <= 0).ToList();

        var deptName = vm.DepartmentId > 0 ? model.FirstOrDefault(x => x.DepartmentId == vm.DepartmentId)?.DepartmentName : "All Department";

        var fullHtml = "";

        fullHtml += "<table class='table table-striped table-bordered table-md ' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
        fullHtml += "<thead>";

        #region DeptName and Date
        fullHtml += "<tr style='height:30px;'>";
        fullHtml += $@"<th colspan='10' style='text-align:center;font-size: 12px;'>{deptName} <br/> From Date : {vm.StrFromDate} || To Date : {vm.StrToDate}</th>";
        fullHtml += "</tr>";
        #endregion

        fullHtml += "<tr style='height:26px;font-size:12px;'>";
        fullHtml += "<th style='width:20px;text-align:center;'>Serial</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Department</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Consume No</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Consume Date</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Category</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Item</th>";
        fullHtml += "<th style='width:60px;text-align:center;'>Unit</th>";
        fullHtml += "<th style='width:80px;text-align:right;'>Consume Qty</th>";
        fullHtml += "<th style='width:80px;text-align:right;'>Consume By</th>";
        fullHtml += "<th style='width:80px;text-align:right;'>Remarks</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        int i = 0;
        foreach (var item in model)
        {
            fullHtml += "<tr style='height:22px;'>";
            fullHtml += $@"<td class='text-center'>{++i}</td>";
            fullHtml += $@"<td class='text-center'>{item.DepartmentName}</td>";
            fullHtml += $@"<td class='text-center'>{item.TranNo}</td>";
            fullHtml += $@"<td class='text-center'>{item.TranDate:dd/MMM/yy}</td>";
            fullHtml += $@"<td class='text-center'>{item.CategoryName}</td>";
            fullHtml += $@"<td class='text-center'>{item.ItemName}</td>";
            fullHtml += $@"<td class='text-center'>{item.UnitName}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.ConsumeQty:N2}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.TranByName}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.Remarks}</td>";
            fullHtml += "</tr>";
        }
        fullHtml += "</tbody>";

        fullHtml += $@"<tfoot>
                        <tr style='height:25px;font-weight;bold;'>
                            <td colspan='7' style='text-align:right;'> Total : </td>
                            <td style='text-align:right;'>{model.Sum(o => o.ConsumeQty):N2}</td>
                            <td style='text-align:right;' colspan='2'></td>
                        </tr>
                    </tfoot>";

        fullHtml += "</table>";
        return fullHtml;
    }
    #endregion

    #region InvOrderDueReport

    public async Task<List<InvOrderDueReportVm>> GetInvOrderDueReport(InvOrderDueReportVm vm)
    {
        var orderIdQuery = vm.OrderId > 0 ? $"and o.Id = {vm.OrderId}" : "";
        var supplierIdQuery = vm.SupplierId > 0 ? $"and o.SupplierId = {vm.SupplierId}" : "";
        string fromDateQuery = (!string.IsNullOrEmpty(vm.StrFromDate)) ? $" and convert(date,o.OrderDate) >= '{Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)):dd/MMM/yyyy}'" : "";
        string toDateQuery = (!string.IsNullOrEmpty(vm.StrToDate)) ? $" and convert(date,o.OrderDate) <= '{Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)):dd/MMM/yyyy}'" : "";

        var query = $@"select o.OrderId,o.OrderNo,o.OrderDate,o.SupplierId,o.SupplierName
                    ,(string_agg(cast(m.TranNo as nvarchar(max)), ',') within group (order by m.TranNo)) MrrNoList
                    ,max(o.OrderAmount) OrderAmount,sum(d.ItemQty * d.UnitPrice) MrrAmount,max(isnull(p.PaidAmount,0)) PaidAmount
                    ,sum(d.ItemQty * d.UnitPrice) - max(isnull(p.PaidAmount,0)) DueAmount
                    from TranDtls d
                    inner join TranMsts m on m.Id = d.TranMstId
                    inner join (
                    select om.Id OrderId,convert(date,om.OrderDate) OrderDate,om.OrderNo,s.Id SupplierId,s.SupplierName,sum(od.AprOrderQty * od.Rate) OrderAmount
                    from OrderDtls od
                    inner join OrderMsts om on om.Id = od.OrderId
                    inner join SupplierInfos s on s.Id = om.SupplierId
                    group by om.Id,om.OrderDate,om.OrderNo,s.Id,s.SupplierName
                    ) o on o.OrderId = m.OrderId
                    left join (
                    select OrderMstId OrderId,sum(b.BillAmount) PaidAmount from InventoryBillPayments b group by OrderMstId
                    ) p on p.OrderId = o.OrderId
                    where m.TranType = 'R' {orderIdQuery} {supplierIdQuery} {fromDateQuery} {toDateQuery}
                    group by o.OrderId,o.OrderNo,o.OrderDate,o.SupplierId,o.SupplierName
                    having sum(d.ItemQty * d.UnitPrice) - max(isnull(p.PaidAmount,0))  > 0
";

        var data = await _iReadDbConnection.QueryAsync<InvOrderDueReportVm>(query);
        return data.ToList();
    }

    #endregion

    #region InvOrderDueReportHtml
    public async Task<string> InvOrderDueReportHtml(InvOrderDueReportVm vm, bool isPrint = false)
    {
        var model = await GetInvOrderDueReport(vm);

        var fullHtml = "";

        fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
        fullHtml += "<thead>";
        fullHtml += "<tr style='height:26px;font-size:12px;'>";
        fullHtml += "<th style='width:20px;text-align:center;'>Serial</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Order</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Supplier</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Receive List</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Order Amount</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Receive Amount</th>";
        fullHtml += "<th style='width:60px;text-align:center;'>Paid Amount</th>";
        fullHtml += "<th style='width:80px;text-align:right;'>Due Amount</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        int i = 0;
        foreach (var item in model)
        {

            string orderNo = !isPrint ? $"<a target='_blank' href='../Order/Details/{item.OrderId}'>{item.OrderNo}</a>" : $"{item.OrderNo}";

            fullHtml += "<tr style='height:22px;'>";
            fullHtml += $@"<td class='text-center'>{++i}</td>";
            fullHtml += $@"<td class='text-center'><b>{orderNo}</b><br />{DU.Utility.ConvertDateToStr(item.OrderDate)}</td>";
            fullHtml += $@"<td class='text-center'>{item.SupplierName}</td>";
            fullHtml += $@"<td class='text-center'>{item.MrrNoList}</td>";
            fullHtml += $@"<td class='text-center'>{item.OrderAmount:N2}</td>";
            fullHtml += $@"<td class='text-center'>{item.MrrAmount:N2}</td>";
            fullHtml += $@"<td class='text-center'>{item.PaidAmount:N2}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.DueAmount:N2}</td>";
            fullHtml += "</tr>";
        }
        fullHtml += "</tbody>";

        fullHtml += $@"<tfoot><tr style='height:25px;font-weight;bold;'><td colspan='4' style='text-align:right;'> Total : </td>
            <td style='text-align:right;'>{model.Sum(o => o.OrderAmount):N2}</td>
            <td style='text-align:right;'>{model.Sum(o => o.MrrAmount):N2}</td>
            <td style='text-align:right;'>{model.Sum(o => o.PaidAmount):N2}</td>
            <td style='text-align:right;'>{model.Sum(o => o.DueAmount):N2}</td>
            </tr></tfoot>";

        fullHtml += "</table>";
        return fullHtml;
    }
    #endregion

    #region ItemConsumptionReportData

    public async Task<List<ItemConsumptionReportVm>> ItemConsumptionReportData(ItemConsumptionReportVm vm)
    {

        string fromDateQuery = (!string.IsNullOrEmpty(vm.StrFromDate)) ? $" and convert(date,ro.OrderDate) >= '{Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)):dd/MMM/yyyy}'" : "";
        string toDateQuery = (!string.IsNullOrEmpty(vm.StrToDate)) ? $" and convert(date,ro.OrderDate) <= '{Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)):dd/MMM/yyyy}'" : "";

        //string dateFilter = !string.IsNullOrEmpty(vm.StrFromDate) ? $" where convert(date,ro.OrderDate) between '{vm.StrFromDate}' and '{vm.StrToDate}'" : "";

        var query = $@"select d.FoodName,d.FoodId,d.TotalSold,ing.ItemName,ing.ItemId,ing.IngQty,IngQty * d.TotalSold TotalIngQy,ing.UnitName
            from (
            select fi.ItemName FoodName,fi.Id FoodId,sum(oi.Quantity) TotalSold
            from RsFoodOrderItems oi
            inner join RsFoodOrders ro on ro.Id = oi.OrderId
            inner join RsFoodItems fi on fi.Id = oi.FoodId
            where ro.IsDeleted = 0 and oi.IsDeleted = 0 {fromDateQuery} {toDateQuery} 
            group by fi.ItemName,fi.Id 
            ) d 
            inner join (
            select fi.Id FoodId,fi.ItemName FooName ,ii.Id ItemId,ii.ItemName,rg.Quantity IngQty,ui.UnitName
            from RsFoodItems fi 
            inner join RsFoodIngredients rg on rg.FoodItemId = fi.Id
            inner join ItemInfos ii on ii.Id = rg.ItemId
            inner join UnitInfos ui on ui.id = rg.UnitId
            where rg.IsDeleted = 0 
            ) ing on ing.FoodId = d.FoodId
            order by d.FoodName";

        var data = await _iReadDbConnection.QueryAsync<ItemConsumptionReportVm>(query);
        return data.ToList();
    }

    #endregion

    #region ItemConsumptionReportHtml
    public async Task<string> ItemConsumptionReportHtml(ItemConsumptionReportVm vm, bool isPrint = false)
    {
        var model = await ItemConsumptionReportData(vm);
        var foodItemList = model.Select(o => new { o.FoodId, o.FoodName, o.TotalSold }).Distinct().ToList();

        var fullHtml = "";

        fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
        fullHtml += "<thead>";
        fullHtml += "<tr style='height:26px;font-size:12px;'>";
        fullHtml += "<th style='width:20px;text-align:center;'>Serial</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Food Name</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Total Sold</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Ing. Name</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Unit</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Ing. Qty (Per Item)</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Total Ing. Qy</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        int sl = 0;

        foreach (var item in foodItemList)
        {
            var ingradientsList = model.Where(o => o.FoodId == item.FoodId).ToList();
            if (ingradientsList.Count > 0)
            {
                int count = ingradientsList.Count;
                for (int i = 0; i < ingradientsList.Count; i++)
                {
                    var ingItem = ingradientsList[i];
                    if (i == 0)
                    {
                        fullHtml += "<tr style='height:22px;'>";
                        fullHtml += $@"<td rowspan='{count}' class='text-center'>{++sl}</td>";
                        fullHtml += $@"<td rowspan='{count}' class='text-center'><b>{item.FoodName}</b></td>";
                        fullHtml += $@"<td rowspan='{count}' class='text-center'>{item.TotalSold}</td>";
                        fullHtml += $@"<td class='text-left'>{ingItem.ItemName}</td>";
                        fullHtml += $@"<td class='text-center'>{ingItem.UnitName}</td>";
                        fullHtml += $@"<td class='text-center'>{ingItem.IngQty:N2}</td>";
                        fullHtml += $@"<td class='text-center'>{ingItem.TotalIngQy:N2}</td>";
                        fullHtml += "</tr>";
                    }
                    else
                    {
                        fullHtml += "<tr style='height:22px;'>";
                        fullHtml += $@"<td class='text-left'>{ingItem.ItemName}</td>";
                        fullHtml += $@"<td class='text-center'>{ingItem.UnitName}</td>";
                        fullHtml += $@"<td class='text-center'>{ingItem.IngQty:N2}</td>";
                        fullHtml += $@"<td class='text-center'>{ingItem.TotalIngQy:N2}</td>";
                        fullHtml += "</tr>";
                    }
                }
            }
        }

        fullHtml += "</tbody>";
        fullHtml += "</table>";


        //raw materials usages

        try
        {
            var summaryList = model.GroupBy(x => new { x.ItemId, x.ItemName, x.UnitName })
               .Select(g => new
               {
                   g.Key.ItemId,
                   g.Key.ItemName,
                   g.Key.UnitName,
                   TotalIngQy = g.Sum(x => x.TotalIngQy)
               })
               .ToList();

            fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-top:35px;margin-top:35px;repeat-header:yes;' border='1'>";
            fullHtml += "<thead>";
            fullHtml += "<tr><td colspan='4'>Raw Materials Usages Summary</td></tr>";
            fullHtml += "<tr style='height:26px;font-size:12px;'>";
            fullHtml += "<th style='width:20px;text-align:center;'>Serial</th>";
            fullHtml += "<th style='width:50px;text-align:center;'>Ing. Name</th>";
            fullHtml += "<th style='width:50px;text-align:center;'>Unit</th>";
            fullHtml += "<th style='width:50px;text-align:center;'>Total Ing. Qy</th>";
            fullHtml += "</tr>";
            fullHtml += "</thead>";
            fullHtml += "<tbody>";
            int s = 0;
            foreach (var item in summaryList)
            {
                fullHtml += "<tr style='height:22px;'>";
                fullHtml += $@"<td class='text-left'>{++s}</td>";
                fullHtml += $@"<td class='text-left'>{item.ItemName}</td>";
                fullHtml += $@"<td class='text-center'>{item.UnitName}</td>";
                fullHtml += $@"<td class='text-center'>{item.TotalIngQy:N2}</td>";
                fullHtml += "</tr>";
            }
            fullHtml += "</tbody>";
            fullHtml += "</table>";
        }
        catch (Exception)
        {

        }



        return fullHtml;
    }
    #endregion

}
