using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.OnlineBooking;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IOnlineBookingRepository : IRepository<HtOnlineBooking>
{
    Task<DataTablePagination<OnlineBookingSearchVm, OnlineBookingSearchVm>>
       SearchAsync(DataTablePagination<OnlineBookingSearchVm, OnlineBookingSearchVm> vm);


    public Task<HtOnlineBooking> GetOnlineBookingByIdAsync(long id);
}