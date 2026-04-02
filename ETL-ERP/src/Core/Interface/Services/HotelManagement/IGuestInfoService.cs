using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.GuestInfo;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IGuestInfoService : IService<HtGuestInfo>
{
    Task<(bool, long)> GuestEntry(HtGuestInfoVm vm);

    Task<DataTablePagination<HtGuestInfoSearchVm, HtGuestInfoSearchVm>>
        SearchAsync(DataTablePagination<HtGuestInfoSearchVm, HtGuestInfoSearchVm> model);

    Task<string> GuestPrintHtml(long id);

    Task<HtGuestDetailsVm> GetGuestDetails(long id);
    Task<(bool, long)> GuestUpdateAsync(HtGuestInfoVm vm);
}