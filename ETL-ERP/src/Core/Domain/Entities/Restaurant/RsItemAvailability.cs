using Domain.Entities.Identity;

namespace Domain.Entities.HotelManagement;

public class RsItemAvailability
{
    public long Id { get; set; }
    public DayOfWeek Day { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public DateTime ActionDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }

    
    // --- Fk ---

    public long FoodId { get; set; }
    public RsFoodItem Food { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
}
