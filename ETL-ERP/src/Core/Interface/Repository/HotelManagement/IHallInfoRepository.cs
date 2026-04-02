using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomInfo;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IHallInfoRepository : IRepository<HtHallInfo>
{
    Task<DataTablePagination<HtHallInfoSearchVm, HtHallInfoSearchVm>>
       SearchAsync(DataTablePagination<HtHallInfoSearchVm, HtHallInfoSearchVm> vm);
}