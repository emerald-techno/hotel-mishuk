using Domain.Entities.Identity;

namespace Domain.Entities.HotelManagement;

public class HtRoomFacilityMap
{
    public long Id { get; set; }
    public DateTime ActionDate { get; set; }
    public bool IsDeleted { get; set; }

    // --- Fk ---

    public long RoomId { get; set; }
    public HtRoomInfo Room { get; set; }
    public long FacilityId { get; set; }
    public HtRoomFacility Facility { get; set; }
    public long? RoomCategoryId { get; set; }
    public HtRoomCategory RoomCategory { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
}
