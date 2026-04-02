using AutoMapper;
using Domain.Entities.Restaurant;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.FoodIngredient;
using Interface.Repository.Restaurant;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;

namespace Services.Restaurant;

public class FoodIngredientService : BaseService<RsFoodIngredient>, IFoodIngredientService
{
    #region Config
    private IFoodIngredientRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public FoodIngredientService(IFoodIngredientRepository iRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }

    #endregion


    #region Create
    public async Task<bool> FoodIngredientAddAsync(FoodIngredientVM vm)
    {
        var foodIngredientModel = _iMapper.Map<RsFoodIngredient>(vm);

        foodIngredientModel.ActionById = CurrentUserId;
        foodIngredientModel.ActionDate = Utility.GetBdDateTimeNow();
        foodIngredientModel.IsActive = true;

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iRepository.AddAsync(foodIngredientModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return false; }
        ts.Complete();
        return true;
    }
    #endregion

    #region Search
    public async Task<DataTablePagination<FoodIngredientSearchVM, FoodIngredientSearchVM>> SearchAsync(DataTablePagination<FoodIngredientSearchVM, FoodIngredientSearchVM> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }
    #endregion
}
