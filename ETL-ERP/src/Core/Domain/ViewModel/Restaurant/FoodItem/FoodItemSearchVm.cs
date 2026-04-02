using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Restaurant.FoodItem;

public class FoodItemSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string ItemName { get; set; }
    public string ItemCode { get; set; }
    public string PhotoUrl { get; set; }
    public string Description { get; set; }
    public double VAT { get; set; } = 0;
    public double Rate { get; set; } = 0;
    public double OfferRate { get; set; } = 0;
    public double NetRate { get; set; } = 0;
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsSetMenuItem { get; set; }
    public bool IsDeleted { get; set; }

    // --- Fk ---

    public long CategoryId { get; set; }
    public string CategoryName { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
    public IEnumerable<SelectListItem> FoodCategoryLookUp { get; set; }
}
