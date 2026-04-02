using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.OnlineBooking;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IOnlineBookingService : IService<HtOnlineBooking>
{
    Task<(bool, long)> OnlineBookingEntryAsync(OnlineBookingVm vm);
    Task<string> GetOnlineBookingCode();

    Task<DataTablePagination<OnlineBookingSearchVm, OnlineBookingSearchVm>>
        SearchAsync(DataTablePagination<OnlineBookingSearchVm, OnlineBookingSearchVm> model);
    public Task<string> GetOnlineBookingBillByIdAsyncHtml(long id);
    public Task<OnlineBookingVm> GetBookingBillByIdAsync(long id);
    Task<List<OnlineBookingVm>> GetUpcomingPendingBooking();

}
