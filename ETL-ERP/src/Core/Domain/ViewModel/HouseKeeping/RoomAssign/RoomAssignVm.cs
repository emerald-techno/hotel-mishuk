using Domain.Enums.AppEnums;
using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HouseKeeping.RoomAssign;

public class RoomAssignSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public RoomAssignEnum Status { get; set; }
    public long HouseKeeperId { get; set; }
    public string HouseKeeperName { get; set; }
    public long RoomId { get; set; }
    public string RoomNo { get; set; }
    public string RoomCategoryName { get; set; }
    public string RoomFloorName { get; set; }
    public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }

    //DataTable
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}

public class RoomAssignVm
{
    public long Id { get; set; }
    public RoomAssignEnum Status { get; set; }
    public long HouseKeeperId { get; set; }
    public string HouseKeeperName { get; set; }
    public long RoomId { get; set; }
    public string RoomNo { get; set; }
    public string RoomCategoryName { get; set; }
    public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }
    public ICollection<RoomAssignVm> RoomAssignVms { get; set; }
}

public class SingleRoomAssignVm
{
    public long HouseKeeperId { get; set; }
    public long RoomId { get; set; }
}