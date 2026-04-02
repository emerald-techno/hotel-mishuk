using Domain.Entities.HotelManagement;
using Domain.Entities.HR;
using Domain.Entities.Identity;
using Domain.Enums.AppEnums;
using Domain.ModelInterface;

namespace Domain.Entities.HouseKeeping;

public class HkRoomAssign : IAuditable
{
    public long Id { get; set; }
    public RoomAssignEnum Status { get; set; } // // 0=Assigned, 1=Running, 2 =Completed, 3=Cancel 
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    // --- Fk ---

    public long HouseKeeperId { get; set; }
    public Employee HouseKeeper { get; set; }
    public long RoomId { get; set; }
    public HtRoomInfo Room { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}