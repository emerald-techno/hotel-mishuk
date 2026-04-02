using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomFacilityCategory;
using Domain.ViewModel.HotelManagement.RoomInfo;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IRoomFacilityCategoryService : IService<HtRoomFacilityCategory>
{
    Task<DataTablePagination<HtRoomFacilityCategorySearchVm, HtRoomFacilityCategorySearchVm>>
        SearchAsync(DataTablePagination<HtRoomFacilityCategorySearchVm, HtRoomFacilityCategorySearchVm> model);
    Task<List<SaveRoomFacilityCategoryVm>> GetCategoryWithFacility();
}