using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.FoodCategory;
using Interface.Repository.Restaurant;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;

namespace Services.Restaurant;

public class FoodCategoryService : BaseService<RsFoodCategory>, IFoodCategoryService
{
    #region Config
    private IFoodCategoryRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public FoodCategoryService(IFoodCategoryRepository iRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }

    #endregion

    public async Task<DataTablePagination<FoodCategorySearchVm, FoodCategorySearchVm>> SearchAsync(DataTablePagination<FoodCategorySearchVm, FoodCategorySearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }

    public async Task<bool> FoodCategoryAddAsync(FoodCategoryVm vm)
    {
        var foodCategoryModel = _iMapper.Map<RsFoodCategory>(vm);

        foodCategoryModel.ActionById = CurrentUserId;
        foodCategoryModel.ActionDate = Utility.GetBdDateTimeNow();
        foodCategoryModel.IsActive = true;

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iRepository.AddAsync(foodCategoryModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return false; }
        ts.Complete();
        return true;
    }
}
