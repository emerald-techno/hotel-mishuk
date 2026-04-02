using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;

namespace Domain.ViewModel.HouseKeeping.Dashboard;

public class HkDashboard
{
    public int DirtyRoom { get; set; }
    public int AvailableWorker { get; set; }
    public int TaskCompleted { get; set; }
    public int TaskRemaining { get; set; }
    public ICollection<HtRoomInfo> DirtyRooms { get; set; }
    public ICollection<TaskList> TaskLists { get; set; }
}

public class TaskList
{
    public string HouseKeeperName { get; set; }
    public string RoomNo { get; set; }
    public RoomAssignEnum Status { get; set; }
}