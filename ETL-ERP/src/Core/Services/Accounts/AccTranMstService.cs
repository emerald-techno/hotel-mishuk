using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccTranMst;
using Interface.Repository.Accounts;
using Interface.Repository.Common;
using Interface.Services.Accounts;
using Interface.Services.Admin;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;
using DU = Domain.Utility;

namespace Services.Accounts;

public class AccTranMstService : BaseService<AccTranMst>, IAccTranMstService
{
    #region Config
    private IAccTranMstRepository Repository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IAutoCodeRepository _iAutoCodeRepository;
    private readonly IAccountReportRepository _iAccountReportRepository;
    private readonly IAccTranDtlRepository _iAccTranDtlRepository;
    private readonly IAccTranFileRepository _iAccTranFileRepository;
    private readonly IAccTranNoteRepository _iAccTranNoteRepository;
    private readonly IAccLedgerRepository _iAccLedgerRepository;

    private readonly ISetCurrencyService _iSetCurrencyService;
    private readonly ISetFincYearService _iSetFincYearService;

    public AccTranMstService(
        IAccTranMstRepository iRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork,
        IAutoCodeRepository iAutoCodeRepository,
        IAccountReportRepository iAccountReportRepository,
        IAccTranDtlRepository iAccTranDtlRepository,
        IAccLedgerRepository iAccLedgerRepository,
        ISetCurrencyService iSetCurrencyService,
        ISetFincYearService iSetFincYearService,
        IAccTranFileRepository iAccTranFileRepository,
        IAccTranNoteRepository iAccTranNoteRepository)
        : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iAutoCodeRepository = iAutoCodeRepository;
        _iAccountReportRepository = iAccountReportRepository;
        _iAccTranDtlRepository = iAccTranDtlRepository;
        _iAccLedgerRepository = iAccLedgerRepository;
        _iSetCurrencyService = iSetCurrencyService;
        _iSetFincYearService = iSetFincYearService;
        _iAccTranFileRepository = iAccTranFileRepository;
        _iAccTranNoteRepository = iAccTranNoteRepository;
    }
    #endregion

    #region Search

    public async Task<DataTablePagination<AccTranMstSearchVm, AccTranMstSearchVm>> SearchAsync(DataTablePagination<AccTranMstSearchVm, AccTranMstSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    #endregion

    #region AddAccountTransaction

    public async Task<long> AddAccTran(AccTranMstVm vm)
    {
        var tranModel = _iMapper.Map<AccTranMst>(vm);

        tranModel.VcDate = (DateTime)(!string.IsNullOrEmpty(vm.VcDateStr) ? DU.Utility.ConvertStrToDate(vm.VcDateStr) : DateTime.Now);

        tranModel.ActionById = CurrentUserId;
        tranModel.ActionDate = DU.Utility.GetBdDateTimeNow();

        if (tranModel.VcType == VoucherType.JournalVoucher)
        {
            tranModel.VcNo = await GetVoucherAutoCode(VoucherTypeCode.JournalVoucher, tranModel.VcDate);
        }
        else if (tranModel.VcType == VoucherType.OpeningVoucher)
        {
            tranModel.VcNo = await GetVoucherAutoCode(VoucherTypeCode.OpeningVoucher, tranModel.VcDate);
        }
        else if (tranModel.VcType == VoucherType.BankDebitVoucher)
        {
            tranModel.VcNo = await GetVoucherAutoCode(VoucherTypeCode.BankDebitVoucher, tranModel.VcDate);
        }
        else if (tranModel.VcType == VoucherType.BankCreditVoucher)
        {
            tranModel.VcNo = await GetVoucherAutoCode(VoucherTypeCode.BankCreditVoucher, tranModel.VcDate);
        }
        else if (tranModel.VcType == VoucherType.CashCreditVoucher)
        {
            tranModel.VcNo = await GetVoucherAutoCode(VoucherTypeCode.CashCreditVoucher, tranModel.VcDate);
        }
        else if (tranModel.VcType == VoucherType.CashDebitVoucher)
        {
            tranModel.VcNo = await GetVoucherAutoCode(VoucherTypeCode.CashDebitVoucher, tranModel.VcDate);
        }

        if (tranModel.AccTranDtls == null && tranModel.AccTranDtls.Count > 0 is false)
            throw new Exception("No Ladger Found...!");

        if (tranModel.AccTranDtls.Count > 0)
        {
            foreach (var dtl in tranModel.AccTranDtls)
            {
                dtl.ActionById = CurrentUserId;
                dtl.ActionDate = DU.Utility.GetBdDateTimeNow();
            }
        }

        await Repository.AddAsync(tranModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded)
        {
            return 0;
        }

        return tranModel.Id;
    }

    #endregion

    #region GetJournalData

    public async Task<AccTranMstVm> GetJournalDataAsync(long id)
    {
        var data = await Repository.GetJournalDetails(id);
        return data;
    }

    #endregion

    #region GetVoucherAutoCode

    public async Task<string> GetVoucherAutoCode(string voucherTypeCode, DateTime vcDate)
    {
        var data = await _iAutoCodeRepository.GetVoucherAutoNo($"{voucherTypeCode}", vcDate);
        return data;
    }

    #endregion

    #region GetLedgerReportHtml

    public async Task<string> GetLedgerReportHtml(AccReportVm vm, bool isPrint)
    {
        string fullHtml = await _iAccountReportRepository.GetLedgerReportHtml(vm, isPrint);
        return fullHtml;
    }

    #endregion

    #region GetTrialBalanceHtml

    public async Task<string> GetTrialBalanceHtml(AccReportVm vm)
    {
        string fullHtml = await _iAccountReportRepository.GetTrialBalanceHtml(vm);
        return fullHtml;
    }

    #endregion

    #region GetHeadWiseBalanceHtml

    public async Task<string> GetHeadWiseBalanceHtml(AccReportVm vm)
    {
        string fullHtml = await _iAccountReportRepository.GetHeadWiseBalanceHtml(vm);
        return fullHtml;
    }

    #endregion

    #region GetVoucherDetaliHtml

    public async Task<string> GetVoucherDetaliHtml(long id)
    {
        string fullHtml = await _iAccountReportRepository.GetVoucherDetailHtml(id);
        return fullHtml;
    }

    #endregion

    #region GetReceiptAndPaymentsReportHtml

    public async Task<string> GetReceiptAndPaymentsReportHtml(AccReportVm vm)
    {
        string fullHtml = await _iAccountReportRepository.GetReceiptAndPaymentsHtml(vm);
        return fullHtml;
    }

    #endregion

    #region GetProfitOrLossStatementHtml

    public async Task<string> GetProfitOrLossStatementHtml(AccReportVm vm)
    {
        string fullHtml = await _iAccountReportRepository.GetProfitOrLossStatementHtml(vm);
        return fullHtml;
    }

    #endregion

    #region UpdateOpeningTransaction

    public async Task<long> UpdateOpeningTransaction(AccTranMstVm vm)
    {
        if (vm == null && vm.Id > 0 is false)
            throw new Exception("No Voucher Information found");

        if (vm.VcType != VoucherType.OpeningVoucher)
            throw new Exception("This is not opening voucher");

        var existingVc = await Repository.GetFirstOrDefaultAsync(x => x.Id == vm.Id && x.FinYearId == vm.FinYearId && x.VcType == VoucherType.OpeningVoucher && !x.IsDeleted);

        if (vm.AccTranDtls == null && vm.AccTranDtls.Count > 0 is false)
            throw new Exception("No Ladger Found...!");

        List<AccTranDtl> addableList = null;
        List<AccTranDtl> updateableList = null;
        List<AccTranDtl> deletableList = null;

        if (vm.AccTranDtls.Count > 0)
        {
            var dataListForAdd = vm.AccTranDtls.Where(c => c.Id == 0).ToList();

            var updatableItemIds = vm.AccTranDtls?.Where(c => c.Id > 0).Select(c => c.Id).ToList();
            updateableList = (await _iAccTranDtlRepository.GetAsync(c => updatableItemIds.Contains(c.Id))).ToList();

            if (updateableList?.Count > 0)
            {
                foreach (var updateDtl in updateableList)
                {
                    var filterData = vm.AccTranDtls.Where(c => c.Id == updateDtl.Id).FirstOrDefault();

                    updateDtl.UpdateDate = DateTime.Now;
                    updateDtl.UpdatedById = CurrentUserId;
                }

            }

            var oldIds = updateableList?.Select(c => c.Id).ToList();

            deletableList = (await _iAccTranDtlRepository.GetAsync(c => c.TranMstId == vm.Id && !oldIds.Contains(c.Id))).ToList();

            if (dataListForAdd?.Count > 0)
            {
                addableList = _iMapper.Map<List<AccTranDtl>>(dataListForAdd);

                foreach (var (v, i) in addableList.GetItemWithIndex())
                {
                    v.TranMstId = existingVc.Id;
                    v.ActionDate = DateTime.Now;
                    v.ActionById = CurrentUserId;
                }
            }
        }



        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await Repository.UpdateAsync(existingVc);

        if (addableList?.Count > 0)
        {
            _iAccTranDtlRepository.AddRange(addableList);
        }

        if (updateableList?.Count > 0)
        {
            _iAccTranDtlRepository.UpdateRange(updateableList);
        }

        if (deletableList?.Count > 0)
        {
            _iAccTranDtlRepository.RemoveRange(deletableList);
        }

        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded)
        {
            return 0;
        }
        ts.Complete();
        return existingVc.Id;
    }

    #endregion

    #region GetJournalData

    public async Task<AccTranMstVm?> GetOpeningDataAsync(long finYearId)
    {
        if (finYearId > 0 is false)
            throw new Exception("Financial Year Information Not Found...!!");

        var data = await Repository.GetOpeningDetails(finYearId);
        if (data == null)
            return null;

        return data;
    }

    #endregion


    public async Task<AccTranMstVm> GetClosingDataAsync(long finYearId)
    {
        if (finYearId > 0 is false)
            throw new Exception("Last Financial Year Closing Data Not Found...!!");

        var data = await Repository.GetClosingDetails(finYearId);
        if (data == null)
            return null;

        return data;
    }

    #region AddQuickVc

    public async Task<long> AddQuickVc(QuickVoucherVm vm)
    {
        var tranModel = new AccTranMst();

        tranModel.VcType = VoucherType.JournalVoucher;
        tranModel.SubVacType = VoucherType.JournalVoucher;

        tranModel.VcDate = (DateTime)(!string.IsNullOrEmpty(vm.VcDateStr) ? DU.Utility.ConvertStrToDate(vm.VcDateStr) : DateTime.Now);
        tranModel.VcNo = await GetVoucherAutoCode(VoucherTypeCode.JournalVoucher, tranModel.VcDate);
        tranModel.ActionById = CurrentUserId;
        tranModel.ActionDate = DU.Utility.GetBdDateTimeNow();
        tranModel.Narration = vm.Narration;

        tranModel.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        tranModel.FinYearId = (await _iSetFincYearService.GetFincYearByDate(tranModel.VcDate)).Id;

        if (vm.MishukLedgerId == null && vm.MishukLedgerId == 0)
            throw new Exception("Ladger For Mishuk Not Found...!!");

        tranModel.AccAccountId = vm.MishukLedgerId;


        var vcDtl = new AccTranDtl();

        vcDtl.ActionById = CurrentUserId;
        vcDtl.ActionDate = DU.Utility.GetBdDateTimeNow();

        var dtlList = new List<AccTranDtl>();

        if (vm.DrCr == "D")
        {
            vcDtl.LedgerCrId = vm.LedgerId;
            vcDtl.AmountCr = vm.Amount;
            vcDtl.LedgerDrId = vm.MishukLedgerId;// misukLedger.Id;
            vcDtl.AmountDr = vm.Amount;
        }
        else if (vm.DrCr == "C")
        {
            vcDtl.LedgerCrId = vm.MishukLedgerId;// misukLedger.Id;
            vcDtl.AmountCr = vm.Amount;
            vcDtl.LedgerDrId = vm.LedgerId;
            vcDtl.AmountDr = vm.Amount;
        }

        dtlList.Add(vcDtl);

        tranModel.AccTranDtls = dtlList;

        tranModel.TotalAmount = tranModel.AccTranDtls.Sum(x => x.AmountDr);

        await Repository.AddAsync(tranModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded)
        {
            return 0;
        }

        return tranModel.Id;
    }

    #endregion

    #region GetQuickLedgerReportHtml

    public async Task<string> GetQuickLedgerReportHtml(AccReportVm vm, bool isPrint)
    {

        string fullHtml = await _iAccountReportRepository.GetQuickViewLedgerReportHtml(vm, isPrint);
        return fullHtml;
    }

    #endregion

    #region GetQuickVoucherByVcId
    //quickVoucherType
    public async Task<QuickVoucherVm> GetQuickVoucherByVcId(long vcId)
    {
        if (vcId < 0)
            throw new Exception("Voucher Information Not Found...!!");

        var voucher = await Repository.GetFirstOrDefaultAsync(x => x.Id == vcId, d => d.AccTranDtls);

        if (voucher == null)
            throw new Exception("Voucher Information Not Found...!!");

        if (voucher != null && voucher.AccTranDtls.Count > 0)
        {
            var vcDtl = voucher.AccTranDtls.FirstOrDefault();

            var misukLedger = await _iAccLedgerRepository.GetFirstOrDefaultAsync(x => x.LedgerCode == AccLadgerCode.HotelMisuk);
            var restaurantLedger = await _iAccLedgerRepository.GetFirstOrDefaultAsync(x => x.LedgerCode == AccLadgerCode.RestaurantLedger);
            var amariResortLedger = await _iAccLedgerRepository.GetFirstOrDefaultAsync(x => x.LedgerCode == AccLadgerCode.AmariResortLedger);
            var staffKitchLedger = await _iAccLedgerRepository.GetFirstOrDefaultAsync(x => x.LedgerCode == AccLadgerCode.StaffKitchenLedger);

            //if(type == "R")
            //    misukLedger = await _iAccLedgerRepository.GetFirstOrDefaultAsync(x => x.LedgerCode == AccLadgerCode.RestaurantLedger);
            //if (type == "A")
            //    misukLedger = await _iAccLedgerRepository.GetFirstOrDefaultAsync(x => x.LedgerCode == AccLadgerCode.AmariResortLedger);
            //if (type == "S")
            //    misukLedger = await _iAccLedgerRepository.GetFirstOrDefaultAsync(x => x.LedgerCode == AccLadgerCode.StaffKitchenLedger);


            //if (misukLedger == null)
            //    throw new Exception("Ladger For QuckVoucher Not Found...!!");

            var quickVm = new QuickVoucherVm();

            quickVm.VcNo = voucher.VcNo;
            quickVm.VcDate = voucher.VcDate;
            quickVm.VcDateStr = voucher.VcDate.ToString("dd/MM/yyyy");
            quickVm.Narration = voucher.Narration;
            quickVm.Amount = vcDtl.AmountDr;

            if (vcDtl.LedgerDrId == misukLedger.Id || vcDtl.LedgerDrId == restaurantLedger.Id || vcDtl.LedgerDrId == amariResortLedger.Id || vcDtl.LedgerDrId == staffKitchLedger.Id)
            {
                quickVm.DrCr = "D";// "C";
                quickVm.LedgerId = (long)vcDtl.LedgerCrId;
                quickVm.MishukLedgerId = (long)vcDtl.LedgerDrId;
            }
            else if (vcDtl.LedgerCrId == misukLedger.Id || vcDtl.LedgerCrId == restaurantLedger.Id || vcDtl.LedgerCrId == amariResortLedger.Id || vcDtl.LedgerCrId == staffKitchLedger.Id)
            {
                quickVm.DrCr = "C";// "D";
                quickVm.LedgerId = (long)vcDtl.LedgerDrId;
                quickVm.MishukLedgerId = (long)vcDtl.LedgerCrId;
            }

            return quickVm;
        }
        else
        {
            return null;

        }
    }

    #endregion

    #region UpdateQuickVc

    public async Task<bool> UpdateQuickVc(QuickVoucherVm vm)
    {
        if (vm == null && !(vm.VcId > 0))
            throw new Exception("Voucher Information Is Not Correct...!!");

        var tranModel = await Repository.GetFirstOrDefaultAsync(x => x.Id == vm.VcId && !x.IsDeleted, d => d.AccTranDtls);

        tranModel.VcDate = (DateTime)(!string.IsNullOrEmpty(vm.VcDateStr) ? DU.Utility.ConvertStrToDate(vm.VcDateStr) : tranModel.VcDate);
        tranModel.Narration = vm.Narration;

        // var misukLedger = await _iAccLedgerRepository.GetFirstOrDefaultAsync(x => x.LedgerCode == AccLadgerCode.HotelMisuk);
        //if (misukLedger == null)
        //    throw new Exception("Ladger For Mishuk Not Found...!!");
        if (vm.MishukLedgerId == null || vm.MishukLedgerId == 0)
            throw new Exception("Ladger For Mishuk Not Found...!!");

        if (tranModel.AccTranDtls.Count > 0)
        {
            foreach (var vcDtl in tranModel.AccTranDtls)
            {
                //if (vm.DrCr == "D")
                //{
                //    vcDtl.LedgerDrId = vm.LedgerId;
                //    vcDtl.AmountCr = vm.Amount;
                //    vcDtl.LedgerCrId = vm.MishukLedgerId; //misukLedger.Id;
                //    vcDtl.AmountDr = vm.Amount;
                //}
                //else if (vm.DrCr == "C")
                //{
                //    vcDtl.LedgerDrId = vm.MishukLedgerId;// misukLedger.Id;
                //    vcDtl.AmountCr = vm.Amount;
                //    vcDtl.LedgerCrId = vm.LedgerId;
                //    vcDtl.AmountDr = vm.Amount;
                //}
                if (vm.DrCr == "C")
                {
                    vcDtl.LedgerDrId = vm.LedgerId;
                    vcDtl.AmountCr = vm.Amount;
                    vcDtl.LedgerCrId = vm.MishukLedgerId; //misukLedger.Id;
                    vcDtl.AmountDr = vm.Amount;
                }
                else if (vm.DrCr == "D")
                {
                    vcDtl.LedgerDrId = vm.MishukLedgerId;// misukLedger.Id;
                    vcDtl.AmountCr = vm.Amount;
                    vcDtl.LedgerCrId = vm.LedgerId;
                    vcDtl.AmountDr = vm.Amount;
                }
            }
        }

        tranModel.TotalAmount = tranModel.AccTranDtls.Sum(x => x.AmountDr);

        await Repository.UpdateAsync(tranModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded)
        {
            return false;
        }

        return true;
    }

    #endregion

    #region Voucher Remove

    public async Task<bool> VoucherRemoveAsync(long voucherId)
    {
        var voucher = await Repository.GetFirstOrDefaultAsync(x => x.Id == voucherId && !x.IsDeleted, o => o.AccTranDtls);
        var accTranDtls = voucher.AccTranDtls;

        var file = await _iAccTranFileRepository.GetAsync(x => x.TranMstId == voucher.Id);
        var note = await _iAccTranNoteRepository.GetAsync(x => x.TranMstId == voucher.Id);

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (file != null && file.Count > 0)
        {
            _iAccTranFileRepository.RemoveRange(file);
        }

        if (note != null && note.Count > 0)
        {
            _iAccTranNoteRepository.RemoveRange(note);
        }

        if (accTranDtls != null && accTranDtls.Count > 0)
        {
            _iAccTranDtlRepository.RemoveRange(accTranDtls);
        }

        Repository.Remove(voucher);

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }


    #endregion
}
