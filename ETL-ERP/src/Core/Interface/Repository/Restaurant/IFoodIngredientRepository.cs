using Domain.Entities.Restaurant;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.Customer;
using Domain.ViewModel.Restaurant.FoodIngredient;
using Interface.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repository.Restaurant
{
	public interface IFoodIngredientRepository :IRepository<RsFoodIngredient>
	{
		Task<DataTablePagination<FoodIngredientSearchVM, FoodIngredientSearchVM>>
	   SearchAsync(DataTablePagination<FoodIngredientSearchVM, FoodIngredientSearchVM> vm);
	}
}
