using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomCategory;
using Domain.ViewModel.Website;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IRoomCategoryRepository : IRepository<HtRoomCategory>
{
    Task<DataTablePagination<HtRoomCategorySearchVm, HtRoomCategorySearchVm>>
       SearchAsync(DataTablePagination<HtRoomCategorySearchVm, HtRoomCategorySearchVm> vm);
    Task<List<RoomFacilityVm>> GetRoomFacilityData();
    Task<List<HtRoomCategoryVm>> GetRoomCategoryPublicData();
}
