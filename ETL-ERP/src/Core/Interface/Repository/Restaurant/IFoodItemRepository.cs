using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.FoodItem;
using Interface.Base;
using Microsoft.AspNetCore.Mvc;

namespace Interface.Repository.Restaurant;

public interface IFoodItemRepository : IRepository<RsFoodItem>
{
    Task<DataTablePagination<FoodItemSearchVm, FoodItemSearchVm>>
       SearchAsync(DataTablePagination<FoodItemSearchVm, FoodItemSearchVm> vm);
    Task<FoodItemVm> FoodDetails(long id);
    Task<List<FoodSetItemVm>> GetSetMenuItems(long id);

    //    Task<bool> IsFoodItemInOrderAsync();
}