using Domain.Entities.HouseKeeping;
using Domain.Utility.Common;
using Domain.ViewModel.HouseKeeping.Reports;
using Domain.ViewModel.HouseKeeping.RoomAssign;
using Interface.Base;

namespace Interface.Services.HouseKeeping;

public interface IRoomAssignService : IService<HkRoomAssign>
{
    Task<bool> AddOrUpdate(RoomAssignVm vm);
    Task<bool> AddAssignRoom(RoomAssignVm vm);
    Task<bool> RemoveAssignRoom(RoomAssignVm vm);

    Task<DataTablePagination<RoomAssignSearchVm, RoomAssignSearchVm>>
        GetAssignRooms(DataTablePagination<RoomAssignSearchVm, RoomAssignSearchVm> model);
    Task<bool> AssignSingleRoom(SingleRoomAssignVm vm);
    Task<bool> AssignMultipleRoom(List<long> roomIds, int houseKeeperId, bool isClean);
    Task<string> GetRoomCleaningReportHtml(RoomCleaningReportVm vm);
}