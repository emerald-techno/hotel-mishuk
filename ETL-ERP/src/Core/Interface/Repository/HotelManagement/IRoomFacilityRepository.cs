using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Interface.Base;
using Domain.ViewModel.HotelManagement.RoomFacility;

namespace Interface.Repository.HotelManagement;

public interface IRoomFacilityRepository : IRepository<HtRoomFacility>
{
    Task<DataTablePagination<HtRoomFacilitySearchVm, HtRoomFacilitySearchVm>>
       SearchAsync(DataTablePagination<HtRoomFacilitySearchVm, HtRoomFacilitySearchVm> vm);
}