using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.GuestInfo;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IGuestInfoRepository : IRepository<HtGuestInfo>
{
    Task<DataTablePagination<HtGuestInfoSearchVm, HtGuestInfoSearchVm>>
       SearchAsync(DataTablePagination<HtGuestInfoSearchVm, HtGuestInfoSearchVm> vm);
    Task<HtGuestInfoVm> GetGuestByIdAsync(long id);

    Task<HtGuestDetailsVm> GetGuestDetailsByIdAsync(long id);
}