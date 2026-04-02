using Domain.Entities.HotelManagement;
using Domain.Entities.Inventory;
using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Restaurant.FoodIngredient
{
	public class FoodIngredientSearchVM :IDataTableSearch
	{
        public int Id { get; set; }
        public double Quantity { get; set; } = 0;
        public double Price { get; set; } = 0;
        public double Amount { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime? DisableDate { get; set; }
        public DateTime ActionDate { get; set; } = DateTime.Now;
        public DateTime? UpdateDate { get; set; }
        public long? ActionById { get; set; }
        public bool IsDeleted { get; set; } = false;
        //----FK----

        public long FoodItemId { get; set; }
        public RsFoodOrder FoodItem { get; set; }
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public long UnitId { get; set; }
        public UnitInfo Unit { get; set; }

        public IEnumerable<SelectListItem> FoodItemLookUp { get; set; }
        public IEnumerable<SelectListItem> ItemLookUp { get; set; }
        public IEnumerable<SelectListItem> UnitLookUp { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
        long IDataTableSearch.Id { get; set; }
    }
}
