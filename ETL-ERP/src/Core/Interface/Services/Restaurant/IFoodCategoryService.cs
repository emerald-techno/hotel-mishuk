using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.FoodCategory;
using Interface.Base;

namespace Interface.Services.Restaurant;

public interface IFoodCategoryService : IService<RsFoodCategory>
{
    Task<bool> FoodCategoryAddAsync(FoodCategoryVm vm);

    Task<DataTablePagination<FoodCategorySearchVm, FoodCategorySearchVm>>
        SearchAsync(DataTablePagination<FoodCategorySearchVm, FoodCategorySearchVm> model);
}