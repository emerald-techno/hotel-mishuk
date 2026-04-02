using Domain.Entities.HotelManagement;
using Domain.ViewModel.HotelManagement.RoomBooking;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IBookingRoomRepository : IRepository<HtBookingRoom>
{
    Task<List<HtBookingRoomDto>> GetBookedRoom(long? bookingId = null);
    Task<BookingRoomInfoDto> GetBookedRoomInfoByRoomIdAsync(long roomId);
}
