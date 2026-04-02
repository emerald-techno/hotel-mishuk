using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.ViewModel.HotelManagement.Booking;
using Domain.ViewModel.HotelManagement.HotelReport;
using Interface.Repository.HotelManagement;
using Persistence.ContextModel;
using Persistence.DapperModel;
using Repository.Base;
using DU = Domain.Utility;

namespace Repository.HotelManagement;

public class BookingHallRepository : BaseRepository<HtBookingHall>, IBookingHallRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;
    private readonly IApplicationReadDbConnection _iReadDbConnection;

    public BookingHallRepository(ApplicationDbContext db, IMapper iMapper, IApplicationReadDbConnection iReadDbConnection) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
        _iReadDbConnection = iReadDbConnection;
    }
    #endregion

    #region GetBookingRoomByBookingId

    public async Task<List<HtBookingHallVm>> GetBookedHall(long? bookingId = null)
    {
        string bookingIdFilter = (bookingId > 0) ? $" and bh.BookingId = {bookingId}" : "";

        var query = $@"select bh.*,h.HallName from HtBookingHalls bh
                    inner join HtHallInfos h on h.Id = bh.HallId
                    where bh.IsDeleted = 0 {bookingIdFilter}
                    order by bh.BookingDate";

        var data = await _iReadDbConnection.QueryAsync<HtBookingHallVm>(query);
        return data.ToList();
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }

    #endregion

    #region HKSalesReportDataAsync

    public async Task<List<BookingHallReportVm>> BookingHallReportDataAsync(BookingHallReportVm vm)
    {
        DateTime fromDate = string.IsNullOrEmpty(vm.StrFromDate) ? DateTime.Today : Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate));
        DateTime toDate = string.IsNullOrEmpty(vm.StrToDate) ? DateTime.Today : Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate));

        string dateFilter = $"and CONVERT(date,bh.BookingDate) between '{fromDate}' and '{toDate}'";

        string query = $@"
                        select bs.Id BookingId,bs.BookingNo, bs.BookingDate, bh.Id BookingHallId, bh.BookingDate PartyDate, bh.HallShift, h.HallName, 
                        bh.HallRent, bh.Rent, bh.ServiceCharge, bh.Vat, bh.Discount, bh.Discount, bh.NetRent, bh.TotalPerson,
                        bh.AuditById, bh.AuditDate, bh.AuditRemarks, g.Salutation + ' ' + g.FirstName + ' '+ ISNULL(g.LastName,'') GuestName, g.Mobile

                        from HtBookingHalls bh
                        inner join HtHallInfos h on h.Id = bh.HallId
                        inner join HtBookingServices bs on bs.Id = bh.BookingId
                        inner join HtBookingGuests bg on bg.BookingId = bs.Id and bg.IsMain = 1
                        inner join HtGuestInfos g on g.Id = bg.GuestId

                        where 1=1 {dateFilter};";

        var data = await _iReadDbConnection.QueryAsync<BookingHallReportVm>(query);
        return data.ToList();
    }

    #endregion
}
