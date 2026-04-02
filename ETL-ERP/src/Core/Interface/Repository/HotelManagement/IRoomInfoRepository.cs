using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomInfo;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IRoomInfoRepository : IRepository<HtRoomInfo>
{
    Task<DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm>>
       SearchAsync(DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm> vm);

    Task<DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm>>
        HouseKeeperViewSearchAsync(DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm> vm);

    Task<DayWiseRoomInfoVm> GetDayWiseRoomInfo(long roomId, DateTime selectedDate);

    Task<DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm>> 
        HouseKeeperViewSearchAllAsync(DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm> vm);
}
