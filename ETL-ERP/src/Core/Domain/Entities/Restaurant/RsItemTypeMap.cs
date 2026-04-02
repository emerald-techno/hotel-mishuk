using Domain.Entities.Identity;

namespace Domain.Entities.HotelManagement;

public class RsItemTypeMap
{
    public long Id { get; set; }
    public DateTime ActionDate { get; set; }
    public bool IsDeleted { get; set; }

    // --- Fk ---
    public long ItemId { get; set; }
    public RsFoodItem Item { get; set; }
    public long TypeId { get; set; }
    public RsFoodType Type { get; set; } // Dinner, Lunch, Breakfast
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
}
