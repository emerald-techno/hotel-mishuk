using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomInfo;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IHallInfoService : IService<HtHallInfo>
{
    Task<bool> HallAddAsync(HtHallInfoVm vm);

    Task<DataTablePagination<HtHallInfoSearchVm, HtHallInfoSearchVm>>
        SearchAsync(DataTablePagination<HtHallInfoSearchVm, HtHallInfoSearchVm> model);
}