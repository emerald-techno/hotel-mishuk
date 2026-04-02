using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.FloorInfo;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IFloorInfoService : IService<HtFloorInfo>
{
    Task<DataTablePagination<HtFloorInfoSearchVm, HtFloorInfoSearchVm>>
        SearchAsync(DataTablePagination<HtFloorInfoSearchVm, HtFloorInfoSearchVm> model);
}
