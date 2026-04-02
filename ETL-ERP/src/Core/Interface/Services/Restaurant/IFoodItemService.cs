using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.FoodItem;
using Interface.Base;

namespace Interface.Services.Restaurant;

public interface IFoodItemService : IService<RsFoodItem>
{
    Task<bool> FoodAddAsync(FoodItemVm vm);

    Task<DataTablePagination<FoodItemSearchVm, FoodItemSearchVm>>
        SearchAsync(DataTablePagination<FoodItemSearchVm, FoodItemSearchVm> model);
    Task<List<FoodItemVm>> GetFoodByCategoryIdAsync(long? categoryId = 0);
    Task<string> GetFoodItemCode();
    Task<FoodItemVm> FoodDetails(long id);
    Task<List<FoodSetItemVm>> GetSetMenuItems(long id);
    Task<List<FoodItemVm>> GetFoodByNameOrCodeAsync(string value);
    Task<bool> SetFoodItemAddAsync(FoodSetItemVm vm);
    Task<bool> SetFoodItemActiveInActiveAsync(long foodItemId, bool status);
    Task<bool> DeleteSetItem(long foodItemId);
    Task<bool> SetItemCopyAddAsync(FoodItemVm vm);
    //Task<bool> IsFoodItemInOrderAsync();
}
