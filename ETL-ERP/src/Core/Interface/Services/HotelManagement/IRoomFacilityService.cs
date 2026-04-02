using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomFacility;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IRoomFacilityService : IService<HtRoomFacility>
{
    Task<DataTablePagination<HtRoomFacilitySearchVm, HtRoomFacilitySearchVm>>
        SearchAsync(DataTablePagination<HtRoomFacilitySearchVm, HtRoomFacilitySearchVm> model);
}
