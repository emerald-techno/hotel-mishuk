using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Entities.Inventory;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.Transaction;
using Interface.Repository.Accounts;
using Interface.Repository.Common;
using Interface.Repository.Inventory;
using Interface.Services.Accounts;
using Interface.Services.Admin;
using Interface.Services.Inventory;
using Interface.Services.Notification;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;
using DU = Domain.Utility;

namespace Services.Inventory;

public class TranService : BaseService<TranMst>, ITranService
{
    #region CONFIG

    private ITranRepository Repository { get; }
    private IOrderRepository _iOrderRepository { get; }
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IMapper _iMapper;
    private readonly IAutoCodeRepository _autoCodeRepository;
    private readonly IRequsitionInfoRepository _iRequsitionInfoRepository;
    private readonly ITranDtlRepository _iTranDtlRepository;
    private readonly IInventoryBillPaymentRepository _iInventoryBillPaymentRepository;
    private readonly ISetCurrencyService _iSetCurrencyService;
    private readonly ISetFincYearService _iSetFincYearService;

    private readonly IAccHeadRepository _iAccHeadRepository;
    private readonly IAccLedgerService _iAccLedgerService;
    private readonly IAccTranMstRepository _iAccTranMstRepository;
    private readonly IAccTranDtlRepository _iAccTranDtlRepository;

    private readonly INtfNotificationMsgService _iNtfMsgService;

    public TranService(ITranRepository iRepository,
        IOrderRepository iOrderRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork,
        IAutoCodeRepository autoCodeRepository,
        IRequsitionInfoRepository iRequsitionInfoRepository,
        ITranDtlRepository iTranDtlRepository,
        IInventoryBillPaymentRepository iInventoryBillPaymentRepository,
        INtfNotificationMsgService iNtfMsgService,
        ISetCurrencyService iSetCurrencyService,
        ISetFincYearService iSetFincYearService,
        IAccLedgerService iAccLedgerService,
        IAccTranMstRepository iAccTranMstRepository,
        IAccHeadRepository iAccHeadRepository,
        IAccTranDtlRepository iAccTranDtlRepository) : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iOrderRepository = iOrderRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _autoCodeRepository = autoCodeRepository;
        _iRequsitionInfoRepository = iRequsitionInfoRepository;
        _iTranDtlRepository = iTranDtlRepository;
        _iInventoryBillPaymentRepository = iInventoryBillPaymentRepository;
        _iNtfMsgService = iNtfMsgService;
        _iSetCurrencyService = iSetCurrencyService;
        _iSetFincYearService = iSetFincYearService;
        _iAccLedgerService = iAccLedgerService;
        _iAccTranMstRepository = iAccTranMstRepository;
        _iAccHeadRepository = iAccHeadRepository;
        _iAccTranDtlRepository = iAccTranDtlRepository;
    }

    #endregion

    public async Task<DataTablePagination<TransactionSearchVm, TransactionSearchVm>>
        SearchAsync(DataTablePagination<TransactionSearchVm, TransactionSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    public async Task<DataTablePagination<TransactionSearchVm, TransactionSearchVm>>
        SearchLOAsync(DataTablePagination<TransactionSearchVm, TransactionSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    public async Task<bool> AddAsync(TransactionVm vm)
    {
        var transactionModel = _iMapper.Map<TranMst>(vm);

        transactionModel.TranDate = (DateTime)(!string.IsNullOrEmpty(vm.TranDateStr) ? Utility.ConvertStrToDate(vm.TranDateStr) : vm.TranDate);
        transactionModel.TranById = CurrentUserId;
        transactionModel.ActionById = CurrentUserId;
        transactionModel.ActionDate = Utility.GetBdDateTimeNow();

        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        var reportDate = Utility.GenerateReportDate(transactionModel.TranDate.Add(currentTime));
        transactionModel.ReportDate = reportDate;

        transactionModel.TranDtls = transactionModel.TranDtls.Where(x => x.ItemQty > 0).ToList();

        if (transactionModel.TranDtls != null && transactionModel.TranDtls.Count() > 0)
        {
            var i = 0;
            foreach (var item in transactionModel.TranDtls)
            {
                item.ActionById = CurrentUserId;
                item.ActionDate = Utility.GetBdDateTimeNow();
                item.SlNo = i++;
            }
        }
        else
        {
            throw new Exception("Sorry! No Details Found!");
        }

        var orderUpdate = false;

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (transactionModel.TranType == TransType.Receive)
        {
            if (transactionModel.OrderId > 0)
            {
                var order = await _iOrderRepository.GetFirstOrDefaultAsync(c => c.Id == transactionModel.OrderId);
                order.ReceiveStatus = vm.ReceiveStatus;

                await _iOrderRepository.UpdateAsync(order);
                orderUpdate = await _iUnitOfWork.CompleteAsync();
            }
        }

        await Repository.AddAsync(transactionModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();

        if (transactionModel.TranType == TransType.Receive && transactionModel.OrderId > 0)
        {
            if (!isAdded || !orderUpdate)
                return false;
        }

        ts.Complete();
        return true;
    }

    //public async Task<string> GetTransAutoCode(string codeType, string tranType)
    //{
    //    var code = await _autoCodeRepository.GetTransactionCode(codeType, tranType);
    //    return code;
    //}

    public async Task<string> GetTransAutoCode(string transType)
    {
        var type = transType == TransType.Receive ? "RCV" : transType == TransType.Issue ? "ISU" : transType == TransType.Consumption ? "CSM" : transType == TransType.LeftOver ? "LOR" : "OPN";
        var data = await _autoCodeRepository.GetMaxAutoCode(TableEnum.TranMsts.ToString(), "TranNo", $"{type}/{DateTime.Now.ToString("yy")}/", 5);
        return data;
    }

    public async Task<TransactionVm> GetTransInfoDataAsync(long id)
    {
        var data = await Repository.GetTransByIdAsync(id);
        return data;
    }

    public List<TransactionDtlVm> GetReceiveDetailsByOrderIdAsync(long id)
    {
        var dataList = Repository.Get(c => c.OrderId == id && c.TranType == TransType.Receive, i => i.TranDtls);
        var result = dataList.SelectMany(c => c.TranDtls).ToList();
        var mapResult = _iMapper.Map<List<TransactionDtlVm>>(result);
        return mapResult;
    }

    #region DirectReceiveEntry

    public async Task<(bool, long)> DirectReceiveEntryAsync(TransactionVm vm)
    {
        var transactionModel = _iMapper.Map<TranMst>(vm);

        transactionModel.TranDate = (DateTime)(!string.IsNullOrEmpty(vm.TranDateStr) ? Utility.ConvertStrToDate(vm.TranDateStr) : vm.TranDate);
        transactionModel.TranById = CurrentUserId;
        transactionModel.ActionById = CurrentUserId;
        transactionModel.ActionDate = Utility.GetBdDateTimeNow();

        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        var reportDate = Utility.GenerateReportDate(transactionModel.TranDate.Add(currentTime));
        transactionModel.ReportDate = reportDate;

        if (transactionModel.TranType != TransType.Receive)
            throw new Exception("Sorry! This is not receive type transaction...!");

        transactionModel.TranDtls = transactionModel.TranDtls.Where(x => x.ItemQty > 0).ToList();

        if (transactionModel.TranDtls != null && transactionModel.TranDtls.Count() > 0)
        {
            var i = 0;
            foreach (var item in transactionModel.TranDtls)
            {
                item.ActionById = CurrentUserId;
                item.ActionDate = Utility.GetBdDateTimeNow();
                item.SlNo = i++;
            }
        }
        else
        {
            throw new Exception("Sorry! No Details Found!");
        }

        var totalQty = transactionModel.TranDtls.Sum(x => x.ItemQty * x.UnitPrice);

        InventoryBillPayment paymentModel = null;
        AccTranMst paymentVoucher = null;

        if (vm.IsReceiveAutoPay)
        {
            if (!(transactionModel.SupplierId > 0))
                throw new Exception("Sorry! No Supplier Found!");

            paymentModel = new InventoryBillPayment();

            paymentModel.BillNo = await GetBillNo();
            paymentModel.BillDate = Utility.GetBdDateTimeNow();
            paymentModel.BillById = CurrentUserId;
            paymentModel.ActionById = CurrentUserId;
            paymentModel.ActionDate = Utility.GetBdDateTimeNow();

            paymentModel.SupplierId = transactionModel.SupplierId;

            paymentModel.PayMode = PayModeEnum.Cash;
            paymentModel.BillAmount = totalQty;

            var paymentReportDate = Utility.GenerateReportDate(paymentModel.BillDate.Add(currentTime));
            transactionModel.ReportDate = paymentReportDate;

            paymentModel.Remarks = $"{vm.TranNo} Payment. Date:{paymentModel.BillDate.ToString("dd/mm/yyyy")}";

            paymentVoucher = await GetInventoryReceivePaymentQuickVoucher(paymentModel, vm);//for quick voucher system
            if (paymentVoucher == null)
                throw new Exception("Somthing Went Wrong Creating Voucher..!!");
        }

        RequsitionInfo autoReqModel = null;

        if (vm.IsReqAuto)
        {
            autoReqModel = await GetAutoReqForReceive(transactionModel); //for auto requsition system
            if (autoReqModel == null)
                throw new Exception("Auto Requsition Create Failed..!!");
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await Repository.AddAsync(transactionModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();

        if (paymentModel != null)
        {
            paymentModel.ReceiveId = transactionModel.Id;
            await _iInventoryBillPaymentRepository.AddAsync(paymentModel);
            await _iUnitOfWork.CompleteAsync();

            paymentVoucher.InvPaymentId = paymentModel.Id;
            await _iAccTranMstRepository.AddAsync(paymentVoucher);
            await _iUnitOfWork.CompleteAsync();
        }

        if (autoReqModel != null)
        {
            await _iRequsitionInfoRepository.AddAsync(autoReqModel);
            var isRequsitionAdded = _iUnitOfWork.Complete();
            if (!isRequsitionAdded) { return (false, 0); }
        }

        ts.Complete();
        return (true, transactionModel.Id);
    }

    #endregion

    #region GetReceiveQuickVoucher

    private async Task<AccTranMst> GetInventoryReceivePaymentQuickVoucher(InventoryBillPayment payment, TransactionVm transactionVM)
    {
        var model = new AccTranMst();
        model.VcDate = payment.BillDate;
        model.VcType = VoucherType.JournalVoucher;
        model.SubVacType = VoucherType.JournalVoucher;
        model.VcNo = await _autoCodeRepository.GetVoucherAutoNo(VoucherTypeCode.JournalVoucher, model.VcDate);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.FinYearId = (await _iSetFincYearService.GetFincYearByDate(model.VcDate)).Id;
        model.Narration = $"Inventory purchase payment. Receive No:{transactionVM.TranNo}";
        model.ActionById = CurrentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();
        model.IsAuto = true;

        var accTranList = new List<AccTranDtl>();

        if (payment != null)
        {

            //var drLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.HotelMisuk);
            //var crLedgerList  = transactionVM.TranDtls.GroupBy(i => new { i.LedgerId,i.HeadCode})
            //      .Select(g => new { LedgerId = g.Key.LedgerId, Amount = g.Sum(x => x.ItemQty * x.UnitPrice),HeadCode = g.Key.HeadCode})
            //      .ToList();

            //var crLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.RMPurchase);
            //if (crLedger == null)
            //    throw new Exception("No Ledger Found Against Room Advance Receive..!!");

            var crLedgerList = transactionVM.TranDtls.GroupBy(i => new { i.LedgerId })
                  .Select(g => new { LedgerId = g.Key.LedgerId, Amount = g.Sum(x => x.ItemQty * x.UnitPrice) })
                  .ToList();

            //var mishukHead = _iAccHeadRepository.GetFirstOrDefault(x => x.HeadCode == AccHeadCode.HotelMisukHead);
            //if (mishukHead == null)
            //    throw new Exception("No Mishuk Head Found !!");

            //var mishulLagederList = await _iAccLedgerService.GetAsync(x => x.HeadId == mishukHead.Id);

            //if (drLedger == null)
            //    throw new Exception("No Debit Ledger Found !!");

            for (int i = 0; i < crLedgerList.Count; i++)
            {
                if (Convert.ToDouble(crLedgerList[i].Amount) > 0)
                {
                    //var drLadger = mishulLagederList.FirstOrDefault(x=>x.LedgerCode == crLedgerList[i].HeadCode);

                    //if (drLadger == null)
                    //{
                    //    drLadger = mishulLagederList.FirstOrDefault(x => x.LedgerCode == AccLadgerCode.HotelMisuk);
                    //}

                    var modelDtl = new AccTranDtl();
                    modelDtl.AmountDr = Convert.ToDouble(crLedgerList[i].Amount);
                    modelDtl.AmountCr = Convert.ToDouble(crLedgerList[i].Amount);
                    modelDtl.LedgerDrId = crLedgerList[i].LedgerId;
                    modelDtl.LedgerCrId = transactionVM.LedgerId;
                    modelDtl.ActionById = CurrentUserId;
                    modelDtl.ActionDate = Utility.GetBdDateTimeNow();
                    accTranList.Add(modelDtl);
                }
            }

            //modelDtl.AmountDr = payment.BillAmount;
            //modelDtl.AmountCr = payment.BillAmount;
            //modelDtl.LedgerDrId = drLedger.Id;
            //modelDtl.LedgerCrId = crLedger.Id;
            //modelDtl.ActionById = CurrentUserId;
            //modelDtl.ActionDate = Utility.GetBdDateTimeNow();
            //accTranList.Add(modelDtl);

            model.AccTranDtls = accTranList;
        }

        model.AccAccountId = transactionVM.LedgerId;
        model.TotalAmount = model.AccTranDtls.Sum(c => c.AmountDr);

        return model;
    }

    #endregion

    #region IssueEntry

    public async Task<bool> IssueEntryAsync(TransactionVm vm)
    {
        var transactionModel = _iMapper.Map<TranMst>(vm);

        if (!(transactionModel.ReqMstId > 0))
            throw new Exception("No requsition information found...!!");

        transactionModel.TranDate = (DateTime)(!string.IsNullOrEmpty(vm.TranDateStr) ? Utility.ConvertStrToDate(vm.TranDateStr) : vm.TranDate);
        transactionModel.TranById = CurrentUserId;
        transactionModel.ActionById = CurrentUserId;
        transactionModel.ActionDate = Utility.GetBdDateTimeNow();

        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        var reportDate = Utility.GenerateReportDate(transactionModel.TranDate.Add(currentTime));
        transactionModel.ReportDate = reportDate;

        transactionModel.TranDtls = transactionModel.TranDtls.Where(x => x.ItemQty > 0).ToList();

        var alreadyIssuedItemList = (await Repository.GetAsync(x => x.ReqMstId == transactionModel.ReqMstId, d => d.TranDtls)).SelectMany(x => x.TranDtls).ToList();

        var requsition = await _iRequsitionInfoRepository.GetFirstOrDefaultAsync(c => c.Id == transactionModel.ReqMstId, d => d.RequsitionInfoDtls);
        var requsitionItemList = requsition.RequsitionInfoDtls.ToList();

        if (transactionModel.TranDtls != null && transactionModel.TranDtls.Count() > 0)
        {
            foreach (var (item, i) in transactionModel.TranDtls.GetItemWithIndex())
            {
                item.ActionById = CurrentUserId;
                item.ActionDate = Utility.GetBdDateTimeNow();
                item.SlNo = (i + 1);
            }
        }
        else
        {
            throw new Exception("Sorry! No Details Found!");
        }

        #region RequsitionIssueStatusUpdate

        bool isFullIssue = true;

        if (requsitionItemList.Count > 0)
        {
            foreach (var reqItem in requsitionItemList)
            {
                var itemIssuedQty = alreadyIssuedItemList.Count > 0 ? alreadyIssuedItemList.Where(x => x.ItemId == reqItem.ItemId).Sum(c => c.ItemQty) : 0;

                var currentIssueItem = transactionModel.TranDtls.FirstOrDefault(x => x.ItemId == reqItem.ItemId);
                var currentIssueItemQty = currentIssueItem != null ? currentIssueItem.ItemQty : 0;
                var totalIssueQty = itemIssuedQty + currentIssueItemQty;

                reqItem.IssueQty = totalIssueQty;

                if (totalIssueQty < reqItem.AprReqQty)
                    isFullIssue = false;
            }

            requsition.IsStatus = !isFullIssue ? (short)RequisitionIssueStatusEnum.PARTIAL : (short)RequisitionIssueStatusEnum.FULL;
        }

        #endregion

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await Repository.AddAsync(transactionModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();

        ts.Complete();
        return true;
    }

    #endregion

    #region AddOpening

    public async Task<long> AddOpening(TransactionVm vm)
    {
        if (vm == null)
            throw new Exception("No Opening Information found");

        if (vm.TranType != TransType.Opening)
            throw new Exception("This is not opening entry");

        var transactionModel = _iMapper.Map<TranMst>(vm);
        transactionModel.TranNo = await GetTransAutoCode(vm.TranType);
        transactionModel.TranDate = DateTime.Now;
        transactionModel.TranById = CurrentUserId;
        transactionModel.ActionById = CurrentUserId;
        transactionModel.ActionDate = Utility.GetBdDateTimeNow();

        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        var reportDate = Utility.GenerateReportDate(transactionModel.TranDate.Add(currentTime));
        transactionModel.ReportDate = reportDate;

        transactionModel.TranDtls = transactionModel.TranDtls.Where(x => x.ItemQty > 0).ToList();

        if (transactionModel.TranDtls != null && transactionModel.TranDtls.Count() > 0)
        {
            var i = 0;
            foreach (var item in transactionModel.TranDtls)
            {
                item.ActionById = CurrentUserId;
                item.ActionDate = Utility.GetBdDateTimeNow();
                item.SlNo = i++;
            }
        }
        else
        {
            throw new Exception("Sorry! No Details Found!");
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await Repository.AddAsync(transactionModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();

        if (!isAdded)
        {
            return 0;
        }

        ts.Complete();
        return transactionModel.Id;
    }

    #endregion

    #region UpdateOpening

    public async Task<long> UpdateOpening(TransactionVm vm)
    {
        if (vm == null && vm.Id > 0 is false)
            throw new Exception("No Opening Information found");

        if (vm.TranType != TransType.Opening)
            throw new Exception("This is not opening entry");

        var existingOpening = await Repository.GetFirstOrDefaultAsync(x => x.Id == vm.Id && x.TranType == TransType.Opening && !x.IsDeleted);

        if (vm.TranDtls == null && vm.TranDtls.Count > 0 is false)
            throw new Exception("No Inventory Items Found...!");

        List<TranDtl> addableList = null;
        List<TranDtl> updateableList = null;
        List<TranDtl> deletableList = null;

        if (vm.TranDtls.Count > 0)
        {
            var dataListForAdd = vm.TranDtls.Where(c => c.Id == 0).ToList();

            var updatableItemIds = vm.TranDtls?.Where(c => c.Id > 0).Select(c => c.Id).ToList();
            updateableList = (await _iTranDtlRepository.GetAsync(c => updatableItemIds.Contains(c.Id))).ToList();

            if (updateableList?.Count > 0)
            {
                foreach (var updateDtl in updateableList)
                {
                    var filterData = vm.TranDtls.Where(c => c.Id == updateDtl.Id).FirstOrDefault();

                    updateDtl.UpdateDate = DateTime.Now;
                    updateDtl.UpdatedById = CurrentUserId;
                }

            }

            var oldIds = updateableList?.Select(c => c.Id).ToList();

            deletableList = (await _iTranDtlRepository.GetAsync(c => c.TranMstId == vm.Id && !oldIds.Contains(c.Id))).ToList();

            if (dataListForAdd?.Count > 0)
            {
                addableList = _iMapper.Map<List<TranDtl>>(dataListForAdd);

                foreach (var (v, i) in addableList.GetItemWithIndex())
                {
                    v.TranMstId = existingOpening.Id;
                    v.ActionDate = DateTime.Now;
                    v.ActionById = CurrentUserId;
                }
            }
        }



        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await Repository.UpdateAsync(existingOpening);

        if (addableList?.Count > 0)
        {
            _iTranDtlRepository.AddRange(addableList);
        }

        if (updateableList?.Count > 0)
        {
            _iTranDtlRepository.UpdateRange(updateableList);
        }

        if (deletableList?.Count > 0)
        {
            _iTranDtlRepository.RemoveRange(deletableList);
        }

        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded)
        {
            return 0;
        }
        ts.Complete();
        return existingOpening.Id;
    }

    #endregion

    #region GetOpeningData

    public async Task<TransactionVm?> GetInventoryOpeningDataAsync()
    {
        var data = await Repository.GetOpeningData();
        if (data == null)
            return null;

        return data;
    }

    public async Task<TransactionVm?> GetInventoryOpeningDataByDeptIdAsync(long id)
    {
        var data = await Repository.GetOpeningDataByDept(id);
        if (data == null)
            return null;

        return data;
    }

    #endregion

    #region ReceiveHtml

    public async Task<string> GetReceiveByIdAsyncHtml(long id)
    {
        try
        {
            var data = await GetTransInfoDataAsync(id);

            var fullHtml = "";
            fullHtml += "<div style='padding-top:5px'>";
            fullHtml += $@"<table class='master-table'>
                                <tbody>
                                    <tr>
                                        <td style='width:12%;'><b>Receive By</b></td>
                                        <td style='width:38%'>: {data.TranByName}</td>
                                        <td style='width:12%;'><b>Receive No</b></td>
                                        <td style='width:38%'>: {data.TranNo}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Supplier</b></td>
                                        <td style='width:38%'>: {data.SupplierName}<br/>
                                            {data.SupplierMobile}
                                        </td>
                                        <td style='width:12%;'><b>Receive Date</b></td>
                                        <td style='width:38%'>: {data.TranDate.ToString("dd-MMM-yyyy")}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Remarks</b></td>
                                        <td style='width:38%;'>: {data.Remarks}</td>
                                        <td style='width:12%;'><b>Order</b></td>
                                        <td style='width:38%'>: {data.OrderNo} <br/>
                                            {data.OrderDate?.ToString("dd-MMM-yyyy")}
                                        </td>
                                    </tr>
                                </tbody>
                            </table>";

            fullHtml += "</div>";

            //Receive Item
            fullHtml += "<div style='padding-top:15px;'><p>Item List :</p></div>";
            fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center;margin-top:5px;font-size:11px'>";

            fullHtml += "<thead>";
            fullHtml += "<tr>";
            fullHtml += "<th style='width:5%'>Sl No</th>";
            fullHtml += "<th style='width:15%'>Item Name</th>";
            fullHtml += "<th style='width:15%'>Unit</th>";
            fullHtml += "<th style='width:10%'>Order Qty</th>";
            fullHtml += "<th style='width:10%'>Receive Qty</th>";
            fullHtml += "<th style='width:10%'>Unit Price</th>";
            fullHtml += "<th style='width:10%'>Rcv. Total</th>";
            fullHtml += "<th style='width:10%'>Stock</th>";
            fullHtml += "<th style='width:15%'>Remarks</th>";
            fullHtml += "</tr>";

            fullHtml += "</thead>";

            fullHtml += "<tbody>";

            if (data.TranDtls.Count > 0)
            {
                foreach (var (item, i) in data.TranDtls.GetItemWithIndex())
                {
                    fullHtml += "<tr>";

                    fullHtml += $@"<td class='text-center'>{i + 1} </td>";
                    fullHtml += $@"<td class='text-start'>{item.ItemName}</td>";
                    fullHtml += $@"<td class='text-start'>{item.ItemUnitName}</td>";
                    fullHtml += $@"<td class='text-center'>{item.OrderQty}</td>";
                    fullHtml += $@"<td class='text-center'>{item.ItemQty}</td>";
                    fullHtml += $@"<td class='text-center'>{item.UnitPrice}</td>";

                    var itemTotal = item.UnitPrice * item.ItemQty ?? 0;

                    fullHtml += $@"<td class='text-end'>{itemTotal:N2}</td>";
                    fullHtml += $@"<td class='text-center'>{item.Stock}</td>";
                    fullHtml += $@"<td class='text-end'>{item.Remarks}</td>";

                    fullHtml += "</tr>";
                }

                var netTotal = data.TranDtls.Sum(x => x.ItemQty * x.UnitPrice) ?? 0;

                fullHtml += "<tr>";
                fullHtml += $@"<td colspan='6' class='text-end'><b>Total</b></td>";
                fullHtml += $@"<td class='text-end'><b>{netTotal.ToString("N2")}</b></td>";
                fullHtml += $@"<td colspan='2'></td>";
                fullHtml += "</tr>";
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

    #region IssueHtml

    public async Task<string> GetIssueByIdAsyncHtml(long id)
    {
        try
        {
            var data = await GetTransInfoDataAsync(id);

            var fullHtml = "";
            fullHtml += "<div style='padding-top:5px'>";
            fullHtml += $@"<table class='master-table'>
                                <tbody>
                                    <tr>
                                        <td style='width:12%;'><b>Issue By</b></td>
                                        <td style='width:38%'>: {data.TranByName}</td>
                                        <td style='width:12%;'><b>Issue No</b></td>
                                        <td style='width:38%'>: {data.TranNo}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Issue Department</b></td>
                                        <td style='width:38%'>: {data.IssueDeptName}</td>
                                        <td style='width:12%;'><b>Issue Date</b></td>
                                        <td style='width:38%'>: {data.TranDate.ToString("dd-MMM-yyyy")}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Issue Employee</b></td>
                                        <td style='width:38%;'>: {data.IssueEmpName}</td>
                                        <td style='width:12%;'><b>Requsition</b></td>
                                        <td style='width:38%'>: {data.ReqNo}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Remarks</b></td>
                                        <td style='width:38%;'>: {data.Remarks}</td>
                                        <td style='width:12%;'><b>Requsition Date</b></td>
                                        <td style='width:38%'>:
                                            {data.ReqDate?.ToString("dd-MMM-yyyy")}
                                        </td>
                                    </tr>
                                </tbody>
                            </table>";

            fullHtml += "</div>";

            //Receive Item
            fullHtml += "<div style='padding-top:15px;'><p>Item List :</p></div>";
            fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center;margin-top:5px;font-size:11px'>";

            fullHtml += "<thead>";
            fullHtml += "<tr>";
            fullHtml += "<th style='width:5%'>Sl No</th>";
            fullHtml += "<th style='width:15%'>Item Name</th>";
            fullHtml += "<th style='width:15%'>Unit</th>";
            fullHtml += "<th style='width:10%'>Issue Qty</th>";
            fullHtml += "<th style='width:10%'>Stock</th>";
            fullHtml += "<th style='width:15%'>Remarks</th>";
            fullHtml += "</tr>";

            fullHtml += "</thead>";

            fullHtml += "<tbody>";

            if (data.TranDtls.Count > 0)
            {
                foreach (var (item, i) in data.TranDtls.GetItemWithIndex())
                {
                    fullHtml += "<tr>";

                    fullHtml += $@"<td class='text-center'>{i + 1} </td>";
                    fullHtml += $@"<td class='text-start'>{item.ItemName}</td>";
                    fullHtml += $@"<td class='text-start'>{item.ItemUnitName}</td>";
                    fullHtml += $@"<td class='text-center'>{item.ItemQty}</td>";
                    fullHtml += $@"<td class='text-center'>{item.Stock}</td>";
                    fullHtml += $@"<td class='text-end'>{item.Remarks}</td>";

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

    #region BillCode
    public async Task<string> GetBillNo()
    {
        var data = await _autoCodeRepository.GetMaxAutoCode(TableEnum.InventoryBillPayments.ToString(), "BillNo", "INV", 6);
        return data;
    }
    #endregion

    #region DirectReceiveReportHtml
    public async Task<string> DirectReceiveReportHtml(DirectReceiveReportVm vm, bool isPrint = false)
    {
        var model = await Repository.GetDirectReceiveReportData(vm);

        var fullHtml = "";

        fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
        fullHtml += "<thead>";
        fullHtml += "<tr style='height:26px;font-size:12px;'>";
        fullHtml += "<th style='width:20px;text-align:center;'>Serial</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Receive</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Supplier</th>";
        fullHtml += "<th style='width:50px;text-align:center;'>Receive Amount</th>";
        fullHtml += "<th style='width:60px;text-align:center;'>Paid Amount</th>";
        fullHtml += "<th style='width:80px;text-align:right;'>Due Amount</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        int i = 0;
        foreach (var item in model)
        {

            string tranNo = !isPrint ? $"<a target='_blank' href='../Receive/Details/{item.TranId}'>{item.TranNo}</a>" : $"{item.TranNo}";

            fullHtml += "<tr style='height:22px;'>";
            fullHtml += $@"<td class='text-center'>{++i}</td>";
            fullHtml += $@"<td class='text-center'><b>{tranNo}</b><br />{DU.Utility.ConvertDateToStr(item.TranDate)}</td>";
            fullHtml += $@"<td class='text-center'>{item.SupplierName}</td>";
            fullHtml += $@"<td class='text-center'>{item.ReceiveAmount:N2}</td>";
            fullHtml += $@"<td class='text-center'>{item.PaidAmount:N2}</td>";
            fullHtml += $@"<td style='text-align:right;'>{item.DueAmount:N2}</td>";
            fullHtml += "</tr>";
        }
        fullHtml += "</tbody>";

        fullHtml += $@"<tfoot><tr style='height:25px;font-weight;bold;'><td colspan='3' style='text-align:right;'> Total : </td>
            <td style='text-align:right;'>{model.Sum(o => o.ReceiveAmount):N2}</td>
            <td style='text-align:right;'>{model.Sum(o => o.PaidAmount):N2}</td>
            <td style='text-align:right;'>{model.Sum(o => o.DueAmount):N2}</td>
            </tr></tfoot>";

        fullHtml += "</table>";
        return fullHtml;
    }
    #endregion

    #region AutoRequsition

    private async Task<RequsitionInfo> GetAutoReqForReceive(TranMst receive)
    {
        var requsitionModel = new RequsitionInfo();

        requsitionModel.ReqDate = receive.TranDate;
        requsitionModel.ReqNo = await GetRequsitionInfoNo();
        requsitionModel.Status = (short)RequisitionStatusEnum.FRESH;
        requsitionModel.IsStatus = (short)RequisitionIssueStatusEnum.NOT;
        requsitionModel.SubmitById = CurrentUserId;
        requsitionModel.ActionById = CurrentUserId;
        requsitionModel.ActionDate = DU.Utility.GetBdDateTimeNow();
        requsitionModel.Priority = "H";

        //requsitionModel.DeptId = receive.IssueDeptId;
        requsitionModel.Remarks = @$"Auto Requsition From Receive - {receive.TranNo}";

        var dtlList = new List<RequsitionInfoDtl>();

        if (receive.TranDtls != null && receive.TranDtls.Count() > 0)
        {
            var i = 0;
            foreach (var item in receive.TranDtls)
            {
                var dtlModel = new RequsitionInfoDtl();

                dtlModel.ItemId = item.ItemId;
                dtlModel.ItemUnitId = item.ItemUnitId;
                dtlModel.ReqQty = item.ItemQty;
                dtlModel.Stock = item.Stock;
                dtlModel.ActionById = CurrentUserId;
                dtlModel.ActionDate = DU.Utility.GetBdDateTimeNow();
                dtlModel.SlNo = i++;

                dtlList.Add(dtlModel);
            }

            requsitionModel.RequsitionInfoDtls = dtlList;
        }

        return requsitionModel;
    }

    private async Task<string> GetRequsitionInfoNo()
    {
        var data = await _autoCodeRepository.GetMaxAutoCode(TableEnum.RequsitionInfos.ToString(), "ReqNo", "REQ", 6);
        return data;
    }

    #endregion

    #region Update

    public async Task<bool> AddOrUpdate(TransactionVm vm)
    {
        var transactionModel = _iMapper.Map<TranMst>(vm);

        transactionModel.TranDate = (DateTime)(!string.IsNullOrEmpty(vm.TranDateStr) ? DU.Utility.ConvertStrToDate(vm.TranDateStr) : vm.TranDate);
        transactionModel.UpdatedById = CurrentUserId;
        transactionModel.UpdateDate = Utility.GetBdDateTimeNow();

        TimeSpan currentTime = DateTime.Now.TimeOfDay;

        if (transactionModel == null || transactionModel.TranDtls == null || transactionModel.TranDtls.Count <= 0)
        {
            throw new Exception("Sorry! No Item details Found! Please Add Item First!");
        }

        var tranDetails = transactionModel.TranDtls;

        List<TranDtl> addableItems = null;
        List<TranDtl> updateableItems = null;
        List<TranDtl> deletableItems = null;

        addableItems = tranDetails.Where(c => c.Id == 0).ToList();

        var changeableItems = tranDetails.Where(c => c.Id > 0).ToList();
        var currentItemIds = changeableItems.Select(c => c.Id);

        var existingDetails = await _iTranDtlRepository.GetAsync(c => c.TranMstId == transactionModel.Id);

        if (existingDetails != null)
        {
            updateableItems = existingDetails.Where(c => currentItemIds.Contains(c.Id)).ToList();
            deletableItems = existingDetails.Where(c => !currentItemIds.Contains(c.Id)).ToList();

            if (updateableItems.Count > 0)
            {
                foreach (var item in updateableItems)
                {
                    var filterData = tranDetails.Where(c => c.Id == item.Id).FirstOrDefault();

                    item.ItemQty = filterData.ItemQty;
                    item.UnitPrice = filterData.UnitPrice;
                }
            }
        }

        var totalQty = (addableItems?.Sum(x => x.ItemQty * x.UnitPrice) + updateableItems?.Sum(x => x.ItemQty * x.UnitPrice));

        #region PaymentUpdate

        List<InventoryBillPayment> paymentDeletableItems = null;
        List<AccTranMst> deleteVcList = null;

        var tranPaymentList = await _iInventoryBillPaymentRepository.GetAsync(x => x.ReceiveId == transactionModel.Id && !x.IsDeleted);
        var tarnPaymentIds = tranPaymentList.Select(x => x.Id).ToList();
        paymentDeletableItems = tranPaymentList.ToList();

        var voucherList = await _iAccTranMstRepository.GetAsync(x => tarnPaymentIds.Contains((long)x.InvPaymentId) && !x.IsDeleted && x.IsAuto, d => d.AccTranDtls);
        deleteVcList = voucherList.ToList();

        InventoryBillPayment paymentModel = null;
        AccTranMst paymentVoucher = null;

        if (vm.IsReceiveAutoPay)
        {
            if (!(transactionModel.SupplierId > 0))
                throw new Exception("Sorry! No Supplier Found!");

            paymentModel = new InventoryBillPayment();

            paymentModel.BillNo = await GetBillNo();
            paymentModel.BillDate = transactionModel.TranDate;
            paymentModel.BillById = CurrentUserId;
            paymentModel.ActionById = CurrentUserId;
            paymentModel.ActionDate = Utility.GetBdDateTimeNow();

            paymentModel.SupplierId = transactionModel.SupplierId;

            paymentModel.PayMode = PayModeEnum.Cash;
            paymentModel.BillAmount = totalQty ?? 0;

            var paymentReportDate = Utility.GenerateReportDate(paymentModel.BillDate.Add(currentTime));
            transactionModel.ReportDate = paymentReportDate;

            paymentModel.Remarks = $"{vm.TranNo} Payment. Date:{paymentModel.BillDate.ToString("dd/mm/yyyy")}";

            paymentVoucher = await GetInventoryReceivePaymentQuickVoucher(paymentModel, vm);//for quick voucher system
            if (paymentVoucher == null)
                throw new Exception("Somthing Went Wrong Creating Voucher..!!");
        }

        #endregion

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        transactionModel.TranDtls = null;
        Repository.Update(transactionModel);

        if (addableItems?.Count > 0)
        {
            addableItems.ForEach(c => c.TranMstId = transactionModel.Id);
            addableItems.ForEach(c => c.ActionById = CurrentUserId);
            addableItems.ForEach(c => c.ActionDate = DU.Utility.GetBdDateTimeNow());
            await _iTranDtlRepository.AddRangeAsync(addableItems);
        }

        if (updateableItems?.Count > 0)
        {
            await _iTranDtlRepository.UpdateRangeAsync(updateableItems);
        }

        if (deletableItems?.Count > 0)
        {
            _iTranDtlRepository.RemoveRange(deletableItems);
        }

        if (deleteVcList?.Count > 0)
        {
            var dtlList = deleteVcList.SelectMany(x => x.AccTranDtls).ToList();
            _iAccTranDtlRepository.RemoveRange(dtlList);
            await _iUnitOfWork.CompleteAsync();

            deleteVcList.ForEach(x => x.AccTranDtls = null);

            _iAccTranMstRepository.RemoveRange(deleteVcList);
            await _iUnitOfWork.CompleteAsync();
        }

        if (paymentDeletableItems?.Count > 0)
        {
            _iInventoryBillPaymentRepository.RemoveRange(paymentDeletableItems);
            await _iUnitOfWork.CompleteAsync();
        }

        if (paymentModel != null)
        {
            paymentModel.ReceiveId = transactionModel.Id;
            await _iInventoryBillPaymentRepository.AddAsync(paymentModel);
            await _iUnitOfWork.CompleteAsync();
        }

        if (paymentVoucher != null)
        {
            paymentVoucher.InvPaymentId = paymentModel.Id;
            await _iAccTranMstRepository.AddAsync(paymentVoucher);
            //var isVoucherAdded = await _iUnitOfWork.CompleteAsync();
        }

        var isExecuted = _iUnitOfWork.Complete();
        if (!isExecuted) { return false; }

        ts.Complete();
        return true;
    }

    #endregion
}
