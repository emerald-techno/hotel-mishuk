using Domain.Entities.Identity;
using Domain.Entities.Inventory;
using Domain.ModelInterface;

namespace Domain.Entities.HouseKeeping;

public class HkTaskRate : IAuditable
{
    public long Id { get; set; }
    public double Rate { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    // --- Fk ---

    public long ItemId { get; set; }
    public ItemInfo Item { get; set; }
    public long TaskId { get; set; }
    public HkTaskName Task { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
