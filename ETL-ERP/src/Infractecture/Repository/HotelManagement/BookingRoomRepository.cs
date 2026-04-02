using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.ViewModel.HotelManagement.RoomBooking;
using Interface.Repository.HotelManagement;
using Persistence.ContextModel;
using Persistence.DapperModel;
using Repository.Base;

namespace Repository.HotelManagement;

public class BookingRoomRepository : BaseRepository<HtBookingRoom>, IBookingRoomRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;
    private readonly IApplicationReadDbConnection _iReadDbConnection;

    public BookingRoomRepository(ApplicationDbContext db, IMapper iMapper, IApplicationReadDbConnection iReadDbConnection) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
        _iReadDbConnection = iReadDbConnection;
    }
    #endregion

    #region GetBookingRoomByBookingId

    public async Task<List<HtBookingRoomDto>> GetBookedRoom(long? bookingId = null)
    {
        string bookingIdFilter = (bookingId > 0) ? $" and BR.BookingId = {bookingId}" : "";

        var query = $@"select BR.*, RC.CategoryName as RoomCategoryName, RM.RoomNo as RoomNo, RM.CleaningStatus as CleaningStatus
                    , c.Title as ComplementaryName, RA.IsCharged
                    from HtBookingRooms BR
                    inner join HtRoomCategories RC on RC.Id = BR.RoomCategoryId
                    inner join HtRoomInfos RM on RM.Id = BR.RoomId
                    left join HtRoomDayAudits RA on RA.BookingRoomId = BR.Id 
                    left join HtComplementaries C on C.Id = BR.ComplementaryId
                    where BR.IsDeleted = 0 {bookingIdFilter}";

        var data = await _iReadDbConnection.QueryAsync<HtBookingRoomDto>(query);
        return data.DistinctBy(x => x.Id).ToList();
    }

    #endregion

    #region GetBookedRoomInfoByRoomId

    public async Task<BookingRoomInfoDto> GetBookedRoomInfoByRoomIdAsync(long roomId)
    {
        if (!(roomId > 0))
            return null;

        //commented by tawkir: 09/03/2025
        //var query = $@"select bs.Id BookingId, bs.BookingDate, br.CheckInTime, br.CheckOutTime, br.TotalGuest, 
        //                CONCAT(gi.Salutation, ' ', gi.FirstName, ' ', gi.LastName) GuestName, gi.Mobile GuestMobile 
        //                from HtBookingRooms br
        //                INNER JOIN HtBookingServices bs on bs.Id = br.BookingId
        //                INNER JOIN HtBookingGuests bg on bg.BookingId = bs.Id and bg.IsMain = 1
        //                INNER JOIN HtGuestInfos gi on gi.Id = bg.GuestId
        //                where bs.BookingStatus = 2 and br.RoomId = {roomId}";

        var query = $@"select bs.Id BookingId, bs.BookingDate, br.CheckInTime, br.CheckOutTime, br.TotalGuest, 
                        CONCAT(gi.Salutation, ' ', gi.FirstName, ' ', gi.LastName) GuestName, gi.Mobile GuestMobile 
                        from HtBookingRooms br
                        INNER JOIN HtBookingServices bs on bs.Id = br.BookingId
                        INNER JOIN HtBookingGuests bg on bg.BookingId = bs.Id and bg.IsMain = 1
                        INNER JOIN HtGuestInfos gi on gi.Id = bg.GuestId
                        where bs.BookingStatus = 2 and br.ActualCheckInTime IS NOT NULL and br.ActualCheckOutTime IS NULL and br.RoomId = {roomId}";

        var data = await _iReadDbConnection.QueryFirstOrDefaultAsync<BookingRoomInfoDto>(query);
        return data;
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }

    #endregion
}
