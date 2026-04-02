using Domain.Entities.Restaurant;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.FoodCategory;
using Domain.ViewModel.Restaurant.FoodIngredient;
using Interface.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services.Restaurant
{
	public interface IFoodIngredientService :IService<RsFoodIngredient>
	{
		Task<bool> FoodIngredientAddAsync(FoodIngredientVM vm);

		Task<DataTablePagination<FoodIngredientSearchVM, FoodIngredientSearchVM>>
			SearchAsync(DataTablePagination<FoodIngredientSearchVM, FoodIngredientSearchVM> model);
	}
}
