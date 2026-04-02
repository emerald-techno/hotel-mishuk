using Domain.Entities.HotelManagement;
using Domain.Entities.Identity;
using Domain.Entities.Inventory;
using Domain.ModelInterface;

namespace Domain.Entities.Restaurant;

public class RsFoodIngredientsHst : IAuditable
{
	public long Id { get; set; }
	public double Quantity { get; set; } = 0;
	public double Price { get; set; } = 0;
	public double Amount { get; set; } = 0;
	public double SysPrice { get; set; }
	public double SysAmount { get; set; }
	public bool IsActive { get; set; } = true;
	public DateTime? DisableDate { get; set; }
	public DateTime ActionDate { get; set; }
	public DateTime? UpdateDate { get; set; }
	public bool IsDeleted { get; set; }

	//----FK---------

	public long OrderItemId { get; set; }
	public RsFoodOrderItem OrderItem { get; set; }
	public long FoodItemId { get; set; }
	public RsFoodItem FoodItem { get; set; }
	public long ItemId { get; set; }
	public ItemInfo Item { get; set; }
	public long UnitId { get; set; }
	public UnitInfo Unit { get; set; }
	public long ActionById { get; set; }
	public ApplicationUser ActionBy { get; set; }
	public long? UpdatedById { get; set; }
	public ApplicationUser UpdatedBy { get; set; }
}
