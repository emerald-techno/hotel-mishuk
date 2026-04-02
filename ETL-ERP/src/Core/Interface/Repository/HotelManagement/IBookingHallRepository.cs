using Domain.Entities.HotelManagement;
using Domain.ViewModel.HotelManagement.Booking;
using Domain.ViewModel.HotelManagement.HotelReport;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IBookingHallRepository : IRepository<HtBookingHall>
{
    Task<List<HtBookingHallVm>> GetBookedHall(long? bookingId = null);
    Task<List<BookingHallReportVm>> BookingHallReportDataAsync(BookingHallReportVm vm);
}