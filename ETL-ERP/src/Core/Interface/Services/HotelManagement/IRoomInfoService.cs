using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomInfo;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IRoomInfoService : IService<HtRoomInfo>
{
    Task<bool> RoomAddAsync(HtRoomInfoVm vm);
    Task<bool> RoomUpdateAsync(HtRoomInfoVm vm);

    Task<DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm>>
        SearchAsync(DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm> model);
    Task<dynamic> GetAvailableRoomByCategoryId(long categoryId, DateTime checkInDate, DateTime checkOutDate);

    Task<DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm>>
        HouseKeeperViewSearchAsync(DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm> model);
    Task<DayWiseRoomInfoVm> GetDayWiseRoomInfo(long roomId, DateTime selectedDate);
    Task<string> HkRoomViewPrintHtml(DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm> model);
    Task<dynamic> GetAvailableRoomByDateRange(DateTime checkInDate, DateTime checkOutDate);
    Task<bool> CheckRoomIsAvaliable(long roomId, DateTime checkInDate, DateTime checkOutDate, long? sameBooKId = null);
    Task<bool> RoomCleanStatusUpdate(long roomId, int cleanStatus);
    Task<bool> RoomAvailabilityStatusUpdate(long roomId, int availabilityStatus);
    Task<dynamic> GetAvailableRooms(DateTime checkInDate, DateTime checkOutDate, long? categoryId);
    Task<bool> MultipleRoomStatusUpdate(List<long> roomIds, int? cleanStatus);
    Task<bool> MultipleRoomMakeOOO(List<long> roomIds, string remarks);
    Task<bool> MultipleRoomMakeAvailable(List<long> roomIds);
}
