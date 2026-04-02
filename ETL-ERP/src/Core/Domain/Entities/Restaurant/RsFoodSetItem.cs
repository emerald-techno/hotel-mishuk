using Domain.Entities.Identity;
using Domain.ModelInterface;

namespace Domain.Entities.HotelManagement;

public class RsFoodSetItem : IAuditable
{
    public long Id { get; set; }
    public double Quantity { get; set; } = 0;
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsEnable { get; set; }
    public bool IsDeleted { get; set; }

    // --- Fk ---

    public long FoodItemId { get; set; }
    public RsFoodItem FoodItem { get; set; }
    public long SetFoodItemId { get; set; }
    public RsFoodItem SetFoodItem { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
