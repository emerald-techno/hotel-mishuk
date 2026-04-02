using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Restaurant.FoodIngredient;

public class FoodIngredientVM
{
    public int Id { get; set; }
    public double Quantity { get; set; } = 0;
    public double Price { get; set; } = 0;
    public double Amount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime? DisableDate { get; set; }
    public DateTime ActionDate { get; set; } = DateTime.Now;
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; } = false;
    public long? ActionById { get; set; }
    public bool IsAjaxPost { get; set; }

    //----FK----

    public long FoodItemId { get; set; }
    public string FoodItemName { get; set; }
    public long ItemId { get; set; }
    public string ItemName { get; set; }
    public long UnitId { get; set; }
    public string UnitName { get; set; }

    public IEnumerable<SelectListItem> FoodItemLookUp { get; set; }
    public IEnumerable<SelectListItem> ItemLookUp { get; set; }
    public IEnumerable<SelectListItem> UnitLookUp { get; set; }
}
