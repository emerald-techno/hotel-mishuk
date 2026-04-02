using Domain.Entities.HouseKeeping;
using Domain.Utility.Common;
using Domain.ViewModel.HouseKeeping.Reports;
using Domain.ViewModel.HouseKeeping.RoomAssign;
using Interface.Base;

namespace Interface.Repository.HouseKeeping;

public interface IRoomAssignRepository : IRepository<HkRoomAssign>
{
    Task<DataTablePagination<RoomAssignSearchVm, RoomAssignSearchVm>> 
        AssignRooms(DataTablePagination<RoomAssignSearchVm, RoomAssignSearchVm> vm);

    Task<List<RoomCleaningReportVm>> GetRoomCleaningReportData(RoomCleaningReportVm vm);
}