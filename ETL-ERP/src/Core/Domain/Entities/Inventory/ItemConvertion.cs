using Domain.Entities.Identity;
using Domain.ModelInterface;

namespace Domain.Entities.Inventory;

public class ItemConvertion : IAuditable
{
	public long Id { get; set; }
	public double Quantity { get; set; } = 1;
	public double ConvertedQuantity { get; set; } = 0;
	public bool IsActive { get; set; } = true;
	public DateTime? DisableDate { get; set; }
	public DateTime ActionDate { get; set; }
	public DateTime? UpdateDate { get; set; }
	public bool IsDeleted { get; set; } = false;

	//-----FK----------

	public long ItemId { get; set; }
	public ItemInfo Item { get; set; }
	public long UnitId { get; set; }
	public UnitInfo Unit { get; set; }
	public long? ConvertedUnitId { get; set; }
	public UnitInfo ConvertedUnit { get; set; }
	public long ActionById { get; set; }
	public ApplicationUser ActionBy { get; set; }
	public long? UpdatedById { get; set; }
	public ApplicationUser UpdatedBy { get; set; }
}
