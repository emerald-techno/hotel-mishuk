using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.FloorInfo;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IFloorInfoRepository : IRepository<HtFloorInfo>
{
    Task<DataTablePagination<HtFloorInfoSearchVm, HtFloorInfoSearchVm>>
       SearchAsync(DataTablePagination<HtFloorInfoSearchVm, HtFloorInfoSearchVm> vm);
}
