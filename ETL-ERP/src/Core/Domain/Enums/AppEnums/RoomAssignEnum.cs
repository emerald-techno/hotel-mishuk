using System.ComponentModel;

namespace Domain.Enums.AppEnums;

// 0=Assigned, 1=Running, 2 =Completed, 3=Cancel 

public enum RoomAssignEnum
{
    Assigned,
    Running,
    Completed,
    Cancel
}

public enum RoomStatusHstEnum
{
    Booked = 1,
    Occupied = 2,
    Available = 3,
    OutOfOrder = 4,
    VD = 5
}

