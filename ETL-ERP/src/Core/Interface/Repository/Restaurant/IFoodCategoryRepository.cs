using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.FoodCategory;
using Interface.Base;

namespace Interface.Repository.Restaurant;

public interface IFoodCategoryRepository : IRepository<RsFoodCategory>
{
    Task<DataTablePagination<FoodCategorySearchVm, FoodCategorySearchVm>>
       SearchAsync(DataTablePagination<FoodCategorySearchVm, FoodCategorySearchVm> vm);
}