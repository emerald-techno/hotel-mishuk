using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Restaurant.FoodCategory;

public class FoodCategorySearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string CategoryName { get; set; }
    public string PhotoUrl { get; set; }
    public bool IsActive { get; set; }
    public long? ParentCategoryId { get; set; }
    public string ParentCategoryName { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }

    //---------------------------------------
    public IEnumerable<SelectListItem> FoodCategoryLookUp { get; set; }
}
