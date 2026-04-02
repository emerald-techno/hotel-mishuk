using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.FoodItem;
using Interface.Repository.Common;
using Interface.Repository.Restaurant;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;

namespace Services.Restaurant;

public class FoodItemService : BaseService<RsFoodItem>, IFoodItemService
{
    #region Config
    private IFoodItemRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IAutoCodeRepository _iAutoCodeRepository;
    private readonly IFoodSetItemRepository _iFoodSetItemRepository;

    public FoodItemService(IFoodItemRepository iRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork,
        IAutoCodeRepository iAutoCodeRepository,
        IFoodSetItemRepository iFoodSetItemRepository) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iAutoCodeRepository = iAutoCodeRepository;
        _iFoodSetItemRepository = iFoodSetItemRepository;
    }

    #endregion

    #region Search
    public async Task<DataTablePagination<FoodItemSearchVm, FoodItemSearchVm>> SearchAsync(DataTablePagination<FoodItemSearchVm, FoodItemSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }
    #endregion

    #region FoodAdd
    public async Task<bool> FoodAddAsync(FoodItemVm vm)
    {
        var foodCategoryModel = _iMapper.Map<RsFoodItem>(vm);
        //foodCategoryModel.ItemCode = await GetFoodItemCode();
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
    #endregion

    #region SetItemCopyAdd
    public async Task<bool> SetItemCopyAddAsync(FoodItemVm vm)
    {
        if (vm == null)
            throw new ArgumentNullException(nameof(vm));

        var model = _iMapper.Map<RsFoodItem>(vm);
        model.Id = 0;
        model.ActionById = CurrentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();
        model.IsActive = true;

        var setItemList = new List<RsFoodSetItem>();

        if (vm.FoodSetItemVms != null && vm.IsSetMenuItem)
        {
            foreach (var item in vm.FoodSetItemVms)
            {
                var setItem = new RsFoodSetItem();
                setItem.SetFoodItemId = item.SetFoodItemId;
                setItem.Quantity = item.Quantity;
                setItem.ActionById = CurrentUserId;
                setItem.ActionDate = Utility.GetBdDateTimeNow();
                setItem.IsEnable = true;

                setItemList.Add(setItem);
            }
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iRepository.AddAsync(model);
        await _iUnitOfWork.CompleteAsync();

        setItemList.ForEach(x => x.FoodItemId = model.Id);

        if (setItemList.Any())
        {
            await _iFoodSetItemRepository.AddRangeAsync(setItemList);
        }
        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return false; }
        ts.Complete();
        return true;
    }
    #endregion

    #region GetFoodByCategory
    public async Task<List<FoodItemVm>> GetFoodByCategoryIdAsync(long? categoryId = 0)
    {
        var foodList = categoryId > 0 ? await _iRepository.GetAsync(x => x.CategoryId == categoryId && x.IsActive && !x.IsDeleted, c => c.Category)
            : await _iRepository.GetAsync(x => x.IsActive && !x.IsDeleted, c => c.Category);

        var result = _iMapper.Map<List<FoodItemVm>>(foodList);

        if (result.Count > 0)
        {
            foreach (var foodItem in result)
            {
                var filterData = foodList.FirstOrDefault(c => c.Id == foodItem.Id);

                foodItem.CategoryName = filterData?.Category.CategoryName;
            }
        }

        return result;
    }
    #endregion

    #region FoodItemDetails
    public async Task<FoodItemVm> FoodDetails(long id)
    {
        var model = await _iRepository.FoodDetails(id);
        return model;
    }
    #endregion

    #region GetSetMenuItems
    public async Task<List<FoodSetItemVm>> GetSetMenuItems(long id)
    {
        var model = await _iRepository.GetSetMenuItems(id);
        return model;
    }
    #endregion

    #region GetBookingCode

    public async Task<string> GetFoodItemCode()
    {
        var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.RsFoodItems.ToString(), "ItemCode", "FIM", 6);
        return data;
    }

    #endregion

    #region GetFoodByCategory
    public async Task<List<FoodItemVm>> GetFoodByNameOrCodeAsync(string value)
    {
        var foodList = !string.IsNullOrEmpty(value) ? await _iRepository.GetAsync(x => (x.ItemCode.ToLower().Contains(value) || x.ItemName.ToLower().Contains(value)) && x.IsActive && !x.IsDeleted, c => c.Category)
            : await _iRepository.GetAsync(x => x.IsActive && !x.IsDeleted, c => c.Category);

        var result = _iMapper.Map<List<FoodItemVm>>(foodList);

        if (result.Count > 0)
        {
            foreach (var foodItem in result)
            {
                var filterData = foodList.FirstOrDefault(c => c.Id == foodItem.Id);

                foodItem.CategoryName = filterData?.Category.CategoryName;
            }
        }

        return result;
    }
    #endregion

    #region SetFoodItemAdd
    public async Task<bool> SetFoodItemAddAsync(FoodSetItemVm vm)
    {
        var foodSetModel = _iMapper.Map<RsFoodSetItem>(vm);

        foodSetModel.ActionById = CurrentUserId;
        foodSetModel.ActionDate = Utility.GetBdDateTimeNow();
        foodSetModel.IsEnable = true;

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iFoodSetItemRepository.AddAsync(foodSetModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return false; }
        ts.Complete();
        return true;
    }
    #endregion

    #region SetFoodItemActive/Inactive
    public async Task<bool> SetFoodItemActiveInActiveAsync(long foodItemId, bool status)
    {
        var data = _iFoodSetItemRepository.GetById(foodItemId);
        if (data == null)
            throw new Exception("No Set Item Found...!!");

        data.IsEnable = status;

        await _iFoodSetItemRepository.UpdateAsync(data);
        var isUpdate = await _iUnitOfWork.CompleteAsync();
        if (!isUpdate) { return false; }
        return true;
    }
    #endregion

    #region DeleteSetItem
    public async Task<bool> DeleteSetItem(long foodItemId)
    {
        var data = _iFoodSetItemRepository.GetById(foodItemId);
        if (data == null)
            throw new Exception("No Set Item Found...!!");

        data.IsDeleted = true;

        await _iFoodSetItemRepository.UpdateAsync(data);
        var isUpdate = await _iUnitOfWork.CompleteAsync();
        if (!isUpdate) { return false; }
        return true;
    }
    #endregion

    //public async Task<bool> IsFoodItemInOrderAsync()
    //{
    //    return await _iRepository.IsFoodItemInOrderAsync();
    //}
}
