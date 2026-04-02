using System.Transactions;
using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.Issue;
using Domain.ViewModel.Inventory.RequsitionInfo;
using Domain.ViewModel.Report;
using Interface.Repository.Common;
using Interface.Repository.Inventory;
using Interface.Services.Inventory;
using Interface.Services.Notification;
using Interface.UnitOfWork;
using Services.Base;
using DU = Domain.Utility;

namespace Services.Inventory;

public class RequsitionInfoService : BaseService<RequsitionInfo>, IRequsitionInfoService
{

    #region CONFIG

    private readonly IRequsitionInfoRepository _iRepository;
    private readonly IRequsitionInfoDtlRepository _iRequsitionInfoDtlRepository;
    private readonly IItemInfoService _iItemInfoService;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IAutoCodeRepository _iAutoCodeRepository;
    private readonly IMapper _iMapper;
    private readonly IInventoryReportRepository _iReportRepository;
    private readonly ITranRepository _iTranRepository;

    private readonly INtfNotificationMsgService _iNtfMsgService;

    public RequsitionInfoService(IRequsitionInfoRepository iRepository,
        IMapper iMapper,
        IRequsitionInfoDtlRepository iRequsitionInfoDtlRepository,
        IItemInfoService iItemInfoService,
        IUnitOfWork iUnitOfWork,
        IAutoCodeRepository iAutoCodeRepository,
        IInventoryReportRepository iReportRepository,
        ITranRepository iTranRepository,
        INtfNotificationMsgService iNtfMsgService) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iRequsitionInfoDtlRepository = iRequsitionInfoDtlRepository;
        _iItemInfoService = iItemInfoService;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iAutoCodeRepository = iAutoCodeRepository;
        _iReportRepository = iReportRepository;
        _iTranRepository = iTranRepository;
        _iNtfMsgService = iNtfMsgService;
    }

    #endregion

    public async Task<RequsitionInfoVm> GetRequsitionInfoDataAsync(long id)
    {
        var data = await _iRepository.GetRequsitionInfoByIdAsync(id);
        return data;
    }

    public async Task<bool> AddAsync(RequsitionInfoVm vm)
    {
        var requsitionModel = _iMapper.Map<RequsitionInfo>(vm);

        if (!(requsitionModel.DeptId > 0))
            throw new Exception("Please add depertment information...!!");

        requsitionModel.ReqDate = (DateTime)(!string.IsNullOrEmpty(vm.ReqDateStr) ? DU.Utility.ConvertStrToDate(vm.ReqDateStr) : vm.ReqDate);
        requsitionModel.Status = (short)RequisitionStatusEnum.FRESH;
        requsitionModel.IsStatus = (short)RequisitionIssueStatusEnum.NOT;
        requsitionModel.SubmitById = CurrentUserId;
        requsitionModel.ActionById = CurrentUserId;
        requsitionModel.ActionDate = DU.Utility.GetBdDateTimeNow();

        if (requsitionModel.RequsitionInfoDtls != null && requsitionModel.RequsitionInfoDtls.Count() > 0)
        {
            var i = 0;
            foreach (var item in requsitionModel.RequsitionInfoDtls)
            {
                item.ActionById = CurrentUserId;
                item.ActionDate = DU.Utility.GetBdDateTimeNow();
                item.SlNo = i++;
            }

        }

        await _iRepository.AddAsync(requsitionModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) return false;

        return true;
    }


    public async Task<DataTablePagination<RequsitionInfoSearchVm, RequsitionInfoSearchVm>>
        SearchAsync(DataTablePagination<RequsitionInfoSearchVm, RequsitionInfoSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }

    public async Task<bool> AddOrUpdate(RequsitionInfoVm vm)
    {
        var requsitionModel = _iMapper.Map<RequsitionInfo>(vm);

        requsitionModel.ReqDate = (DateTime)(!string.IsNullOrEmpty(vm.ReqDateStr) ? DU.Utility.ConvertStrToDate(vm.ReqDateStr) : vm.ReqDate);

        if (requsitionModel == null || requsitionModel.RequsitionInfoDtls == null || requsitionModel.RequsitionInfoDtls.Count <= 0)
        {
            throw new Exception("Sorry! No Item details Found! Please Add Item First!");
        }

        var reqDetails = requsitionModel.RequsitionInfoDtls;

        List<RequsitionInfoDtl> addableItems = null;
        List<RequsitionInfoDtl> updateableItems = null;
        List<RequsitionInfoDtl> deletableItems = null;

        addableItems = reqDetails.Where(c => c.Id == 0).ToList();

        var changeableItems = reqDetails.Where(c => c.Id > 0).ToList();
        var currentItemIds = changeableItems.Select(c => c.Id);

        var existingDetails = _iRequsitionInfoDtlRepository.Get(c => c.ReqId == requsitionModel.Id).ToList();

        if (existingDetails != null)
        {
            updateableItems = existingDetails.Where(c => currentItemIds.Contains(c.Id)).ToList();
            deletableItems = existingDetails.Where(c => !currentItemIds.Contains(c.Id)).ToList();

            if (updateableItems.Count > 0)
            {
                foreach (var item in updateableItems)
                {
                    var filterData = reqDetails.Where(c => c.Id == item.Id).FirstOrDefault();

                    item.ReqQty = filterData.ReqQty;
                }
            }
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        requsitionModel.RequsitionInfoDtls = null;
        _iRepository.Update(requsitionModel);

        if (addableItems?.Count > 0)
        {
            addableItems.ForEach(c => c.ReqId = requsitionModel.Id);
            addableItems.ForEach(c => c.ActionById = CurrentUserId);
            addableItems.ForEach(c => c.ActionDate = DU.Utility.GetBdDateTimeNow());
            await _iRequsitionInfoDtlRepository.AddRangeAsync(addableItems);
        }

        if (updateableItems?.Count > 0)
        {
            await _iRequsitionInfoDtlRepository.UpdateRangeAsync(updateableItems);
        }

        if (deletableItems?.Count > 0)
        {
            _iRequsitionInfoDtlRepository.RemoveRange(deletableItems);
        }

        var isExecuted = _iUnitOfWork.Complete();
        if (!isExecuted) { return false; }

        ts.Complete();
        return true;
    }

    public async Task<bool> ReviewUpdate(RequsitionApprovalVm vm, string ntfLink)
    {

        if (vm.Id == 0 || vm.ApprovalDtls == null || vm.ApprovalDtls.Count <= 0)
        {
            throw new Exception("Sorry! No Item details Found! ");
        }

        var requsitionModel = _iRepository.GetById(vm.Id);
        requsitionModel.Status = vm.Status;

        if (requsitionModel.Status == (short)RequisitionStatusEnum.APPROVED)
        {
            requsitionModel.ApprovedById = CurrentUserId;
            requsitionModel.ApprovedDate = DU.Utility.GetBdDateTimeNow();
        }

        List<RequsitionInfoDtl> updateableItems = null;
        List<RequsitionInfoDtl> deletableItems = null;

        var existingDetails = _iRequsitionInfoDtlRepository.Get(c => c.ReqId == requsitionModel.Id).ToList();


        if (existingDetails?.Count > 0)
        {
            foreach (var item in existingDetails)
            {
                var filterExistData = vm.ApprovalDtls.FirstOrDefault(c => c.Id == item.Id);

                item.AprReqQty = filterExistData.AprReqQty;
            }

            var updateItemList = existingDetails.Where(c => c.AprReqQty > 0).ToList();
            var deleteItemList = existingDetails.Where(c => c.AprReqQty <= 0).ToList();

            updateableItems = _iMapper.Map<List<RequsitionInfoDtl>>(updateItemList);
            deletableItems = _iMapper.Map<List<RequsitionInfoDtl>>(deleteItemList);
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        _iRepository.Update(requsitionModel);

        if (updateableItems?.Count > 0)
        {
            _iRequsitionInfoDtlRepository.UpdateRange(updateableItems);
        }

        if (deletableItems?.Count > 0 && requsitionModel.Status == (short)RequisitionStatusEnum.APPROVED)
        {
            _iRequsitionInfoDtlRepository.RemoveRange(deletableItems);
        }

        var isExecuted = _iUnitOfWork.Complete();
        if (!isExecuted) { return false; }

        string ntfMsgHtml = @$"<a href='{ntfLink}' target='_blank'><b>Requsition {requsitionModel.ReqNo}-({requsitionModel.ReqDate.ToString("dd/MM/yyyy")}) Review Approved</b></a>";
        string emailMsg = $@"Requsition {requsitionModel.ReqNo}-({requsitionModel.ReqDate.ToString("dd/MM/yyyy")}) Review Approved";

        var ntfGenerated = await _iNtfMsgService.GenerateNtf(NotificationEventCode.RequsitionApproveNtf, ntfMsgHtml, emailMsg);

        ts.Complete();
        return true;

    }

    public async Task<string> GetRequsitionInfoNo()
    {
        var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.RequsitionInfos.ToString(), "ReqNo", "REQ", 6);
        return data;
    }

    public dynamic GetItemsByRequsitionId(long id)
    {

        var requsitionItemIds = _iRepository.GetFirstOrDefault(c => c.Id == id, i => i.RequsitionInfoDtls).RequsitionInfoDtls.Select(c => c.ItemId);

        var dynamicData = _iItemInfoService.Get(c => requsitionItemIds.Contains(c.Id)).Select(c => new { c.Id, c.ItemName });

        return dynamicData;

    }

    public dynamic GetRequsitionByDptId(long dptId)
    {
        var requisitionData = _iRepository.Get(x => x.DeptId == dptId
        && (x.IsStatus == (short)RequisitionIssueStatusEnum.PARTIAL || x.IsStatus == (short)RequisitionIssueStatusEnum.NOT)
        && !x.IsDeleted && x.ApprovedById > 0).Select(c => new { c.Id, Name = $"{DU.Utility.GetDate(c.ReqDate)}-({c.ReqNo})" }).ToList();
        return requisitionData;
    }
    public dynamic GetRequsitionByEmpId(long empId)
    {
        var requisitionData = _iRepository.Get(x => x.ReqById == empId
        && (x.IsStatus == (short)RequisitionIssueStatusEnum.PARTIAL || x.IsStatus == (short)RequisitionIssueStatusEnum.NOT)
        && !x.IsDeleted && x.ApprovedById > 0).Select(c => new { c.Id, Name = $"{DU.Utility.GetDate(c.ReqDate)}-({c.ReqNo})" }).ToList();
        return requisitionData;
    }

    public async Task<List<IssueDtlVm>> GetIssueItemByReqId(long reqId)
    {
        if (reqId > 0 is false)
            throw new ArgumentException("Requsition Not Found...!");

        var requsition = await _iRepository.GetRequsitionInfoByIdAsync(reqId);

        if (requsition == null)
            throw new Exception("Requsition Not Found...!");

        var itemList = new List<IssueDtlVm>();

        if (requsition.RequsitionInfoDtls != null && requsition.RequsitionInfoDtls.Count > 0)
        {
            foreach (var issueItem in requsition.RequsitionInfoDtls)
            {
                var model = new IssueDtlVm();

                model.ItemId = issueItem.ItemId;
                model.ItemName = issueItem.ItemName;
                model.ItemUnitId = issueItem.ItemUnitId;
                model.ItemUnitName = issueItem.ItemUnitName;
                model.ApproveQty = issueItem.AprReqQty ?? 0;
                model.IssueQty = issueItem.IssueQty ?? 0;

                var alreadyIssuedItemList = (await _iTranRepository.GetAsync(x => x.ReqMstId == issueItem.ReqId, d => d.TranDtls)).SelectMany(x => x.TranDtls).ToList();
                var itemIssuedQty = alreadyIssuedItemList.Count > 0 ? alreadyIssuedItemList.Where(x => x.ItemId == model.ItemId).Sum(c => c.ItemQty) : 0;

                model.AlreadyIssuedQty = itemIssuedQty;

                var currentStock = await _iReportRepository.GetInventoryStockInfo(new StockVm { ItemId = issueItem.ItemId });
                model.Stock = currentStock != null && currentStock.Count > 0 ? currentStock.FirstOrDefault().Stock : 0;
                model.UnitPrice = currentStock != null && currentStock.Count > 0 ? currentStock.FirstOrDefault().UnitPrice : 0;

                itemList.Add(model);
            }
        }

        return itemList;
    }

    #region RequsitionHtml

    public async Task<string> GetRequsitionByIdAsyncHtml(long id)
    {
        try
        {
            var data = await GetRequsitionInfoDataAsync(id);

            var fullHtml = "";
            //fullHtml += "<div><p><u>Bill To :</u></p></div>";
            fullHtml += "<div style='padding-top:5px'>";
            fullHtml += $@"<table class='master-table'>
                                <tbody>
                                    <tr>
                                        <td style='width:12%;'><b>Requsition By</b></td>
                                        <td style='width:38%'>: {data.SubmitByName}</td>
                                        <td style='width:12%;'><b>Requsition No</b></td>
                                        <td style='width:38%'>: {data.ReqNo}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Department</b></td>
                                        <td style='width:38%'>: {data.Departement}</td>
                                        <td style='width:12%;'><b>Requsition Date</b></td>
                                        <td style='width:38%'>: {data.ReqDate.ToString("dd-MMM-yyyy")}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Requsition For</b></td>
                                        <td style='width:38%'>: {data.RequsitionFor}</td>
                                        <td style='width:12%;'><b>Priority</b></td>
                                        <td style='width:38%'>: {data.PriorityText}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Remarks</b></td>
                                        <td style='width:38%;'>: {data.Remarks}</td>
                                        <td style='width:12%;'><b>Req. Status</b></td>
                                        <td style='width:38%'>: {data.StatusText}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Issue Status</b></td>
                                        <td style='width:38%'>: {data.IsStatusText}</td>
                                    </tr>
                                </tbody>
                            </table>";

            fullHtml += "</div>";

            //Order Item
            fullHtml += "<div style='padding-top:15px;'><p>Item List :</p></div>";
            fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center;margin-top:5px;font-size:11px'>";

            fullHtml += "<thead>";
            fullHtml += "<tr>";
            fullHtml += "<th style='width:5%'>Sl No</th>";
            fullHtml += "<th style='width:15%'>Item Name</th>";
            fullHtml += "<th style='width:10%'>Unit</th>";
            fullHtml += "<th style='width:10%'>Requsition Qty</th>";
            fullHtml += "<th style='width:10%'>Approve Qty</th>";

            if (data.IsStatus != (short)RequisitionIssueStatusEnum.NOT)
            {
                fullHtml += "<th style='width:10%'>Issue Qty</th>";
            }

            fullHtml += "<th style='width:10%'>Stock</th>";
            fullHtml += "<th style='width:10%'>Remarks</th>";
            fullHtml += "</tr>";

            fullHtml += "</thead>";

            fullHtml += "<tbody>";

            if (data.RequsitionInfoDtls.Count > 0)
            {
                foreach (var (item, i) in data.RequsitionInfoDtls.GetItemWithIndex())
                {

                    fullHtml += "<tr>";

                    fullHtml += $@"<td class='text-center'>{i + 1} </td>";
                    fullHtml += $@"<td class='text-start'>{item.ItemName}</td>";
                    fullHtml += $@"<td class='text-start'>{item.ItemUnitName}</td>";
                    fullHtml += $@"<td class='text-center'>{item.ReqQty}</td>";
                    fullHtml += $@"<td class='text-center'>{item.AprReqQty ?? 0}</td>";

                    if (data.IsStatus != (short)RequisitionIssueStatusEnum.NOT)
                    {
                        fullHtml += $@"<td class='text-center'>{item.IssueQty ?? 0}</td>";
                    }

                    fullHtml += $@"<td class='text-center'>{item.Stock}</td>";
                    fullHtml += $@"<td class='text-start'>{item.Remarks}</td>";

                    fullHtml += "</tr>";


                }
            }

            fullHtml += "</tbody>";
            fullHtml += "</table>";

            return fullHtml;
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    #endregion
}
