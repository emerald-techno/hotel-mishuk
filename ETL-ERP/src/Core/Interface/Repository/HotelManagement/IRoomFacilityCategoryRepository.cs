using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Interface.Base;
using Domain.ViewModel.HotelManagement.RoomFacilityCategory;

namespace Interface.Repository.HotelManagement;

public interface IRoomFacilityCategoryRepository : IRepository<HtRoomFacilityCategory>
{
    Task<DataTablePagination<HtRoomFacilityCategorySearchVm, HtRoomFacilityCategorySearchVm>>
       SearchAsync(DataTablePagination<HtRoomFacilityCategorySearchVm, HtRoomFacilityCategorySearchVm> vm);
}