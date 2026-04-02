using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Entities.Restaurant;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.ItemConversion;
using Domain.ViewModel.Restaurant.FoodItem;
using Interface.Repository.Inventory;
using Interface.Repository.Restaurant;
using Interface.Services.Inventory;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;

namespace Services.Inventory;

public class ItemConvertionService :BaseService<ItemConvertion>, IItemConvertionService
{
    #region Config
    private IItemConvertionRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public ItemConvertionService(IItemConvertionRepository iRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }

    #endregion


    #region Create

    public async Task<bool> AddAsync(ItemConvertionVm vm)
    {
        var Model = _iMapper.Map<ItemConvertion>(vm);

        Model.ActionById = CurrentUserId;
        Model.ActionDate = Utility.GetBdDateTimeNow();
        Model.IsActive = true;

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iRepository.AddAsync(Model);
        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return false; }
        ts.Complete();
        return true;
    }
    #endregion


    #region Search
    public async Task<DataTablePagination<ItemConvertionSearchVm, ItemConvertionSearchVm>> SearchAsync(DataTablePagination<ItemConvertionSearchVm, ItemConvertionSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }
    #endregion
}
