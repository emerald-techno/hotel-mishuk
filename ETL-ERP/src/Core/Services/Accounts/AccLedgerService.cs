using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccLedger;
using Interface.Repository.Accounts;
using Interface.Repository.Common;
using Interface.Repository.HotelManagement;
using Interface.Services.Accounts;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Base;
using System.Transactions;
using DU = Domain.Utility;

namespace Services.Accounts;

public class AccLedgerService : BaseService<AccLedger>, IAccLedgerService
{
    #region Config
    private IAccLedgerRepository Repository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IAutoCodeRepository _iAutoCodeRepository;
    private readonly IAccHeadRepository _iAccHeadRepository;
    private readonly IGuestInfoRepository _iGuestInfoRepository;

    public AccLedgerService(IAccLedgerRepository iRepository,
        IMapper iMapper, IUnitOfWork iUnitOfWork,
        IAutoCodeRepository iAutoCodeRepository,
        IAccHeadRepository iAccHeadRepository,
        IGuestInfoRepository iGuestInfoRepository)
        : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iAutoCodeRepository = iAutoCodeRepository;
        _iAccHeadRepository = iAccHeadRepository;
        _iGuestInfoRepository = iGuestInfoRepository;
    }
    #endregion

    #region Search

    public async Task<DataTablePagination<AccLedgerSearchVm, AccLedgerSearchVm>> SearchAsync(DataTablePagination<AccLedgerSearchVm, AccLedgerSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    #endregion

    #region GetAccLedgerCode

    public async Task<string> GetAccLedgerCode(long headId)
    {
        var data = await _iAutoCodeRepository.GetAccLedgerCode(headId);
        return data;
    }

    #endregion

    #region GetAccLedgerGroupCode

    public async Task<long> GetAccLedgerGroupCode(long? headId)
    {
        var data = await _iAutoCodeRepository.GetAccLedgerGroupCode(headId);
        return data;
    }

    #endregion

    #region CheckIsLedgerFoundInHead
    public async Task<bool> IsLedgerFoundInHead(long headId)
    {
        var count = await Repository.GetTotalLedgerByHeadId(headId);
        return count > 0;
    }
    #endregion

    #region MishukLedgerSelectList
    public async Task<IEnumerable<SelectListItem>> GetMishukLedgerSelectListItems(bool isDefaultSelectAdd = true)
    {
        var items = new List<SelectListItem>();
        if (isDefaultSelectAdd) items.Add(new SelectListItem { Value = "", Text = "---Select---" });

        var mishukHead = _iAccHeadRepository.GetFirstOrDefault(x => x.HeadCode == AccHeadCode.HotelMisukHead);
        if (mishukHead == null)
            throw new Exception("No Mishuk Head Found !!");

        var mishulLagederList = await base.GetAsync(x => x.HeadId == mishukHead.Id);
        items.AddRange(mishulLagederList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.LedgerCode}_{c.LedgerName}" }));
        return items;
    }
    #endregion

    #region AddGuestLedger

    public async Task<long> GetGuestLedgerId(long guestId)
    {
        var guestInfo = _iGuestInfoRepository.GetFirstOrDefault(x => x.Id == guestId);
        if (guestInfo == null)
            throw new Exception("Guest Not Found !!");

        if (guestInfo.DueLedgerId > 0)
            return guestInfo.DueLedgerId ?? 0;
        
        var guestDueHead = _iAccHeadRepository.GetFirstOrDefault(x => x.HeadCode == AccHeadCode.GuestDueHead);
        if (guestDueHead == null)
            throw new Exception("Guest Due Head Not Found !!");

        var ledgerModel = new AccLedger();
        ledgerModel.HeadId = guestDueHead.Id;
        ledgerModel.LedgerName = $"Due From {guestInfo.Salutation} {guestInfo.FirstName} {guestInfo.LastName}-({guestInfo.Mobile})";

        var ledgerCode = await _iAutoCodeRepository.GetAccLedgerCode(ledgerModel.HeadId);
        ledgerModel.LedgerCode = ledgerCode;

        var ledgerGroupCode = await _iAutoCodeRepository.GetAccLedgerGroupCode(ledgerModel.HeadId);
        ledgerModel.LedgerGroupCode = ledgerGroupCode;

        ledgerModel.ActionById = CurrentUserId;
        ledgerModel.ActionDate = DU.Utility.GetBdDateTimeNow();

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await Repository.AddAsync(ledgerModel);
        await _iUnitOfWork.CompleteAsync();

        guestInfo.DueLedgerId = ledgerModel.Id;
        _iGuestInfoRepository.Update(guestInfo);

        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return 0; }

        ts.Complete();
        return ledgerModel.Id;
    }

    #endregion
}
