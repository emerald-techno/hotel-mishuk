using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomDayAudit;
using Interface.Repository.HotelManagement;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.HotelManagement;

public class RoomDayAuditRepository : BaseRepository<HtRoomDayAudit>, IRoomDayAuditRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public RoomDayAuditRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }
    #endregion

    #region GetRoomAuditByBusinessDate

    public async Task<List<RoomDayAuditVm>> GetRoomAuditByBusinessDate(DateTime businessDate)
    {
        var dataResult = await Context.HtRoomDayAudits
                            .Include(r => r.Room)
                            .Include(c => c.RoomCategory)
                            .Include(br => br.BookingRoom)
                                .ThenInclude(b => b.Booking)
                            .AsNoTracking()
                            .Where(c => c.BusinessDate.Date == businessDate.Date)
                            .ToListAsync();

        var auditList = _iMapper.Map<List<RoomDayAuditVm>>(dataResult);

        var bookingIds = dataResult.Select(x => x.BookingRoom.BookingId).ToList();

        var guestList = Context.HtBookingGuests
            .Where(x => bookingIds.Contains(x.BookingId))
            .Include(g => g.Guest)
            .ToList();

        if (auditList.Count > 0)
        {
            foreach (var audit in auditList)
            {
                var filterData = dataResult.Where(c => c.Id == audit.Id).FirstOrDefault();
                var bookingGuest = guestList.FirstOrDefault(x => x.IsMain && x.BookingId == filterData.BookingRoom.BookingId);

                audit.RoomNo = filterData.Room.RoomNo;
                audit.RoomCategoryName = filterData.RoomCategory.CategoryName;
                audit.BookingNo = filterData.BookingRoom.Booking.BookingNo;
                audit.BookingId = filterData.BookingRoom.Booking.Id;
                audit.BookingDate = filterData.BookingRoom.Booking.BookingDate;
                audit.GuestName = $"{bookingGuest.Guest?.Salutation} {bookingGuest.Guest?.FirstName} {bookingGuest.Guest?.LastName}";
                audit.GuestMobile = $"{bookingGuest.Guest?.Mobile}";
            }
        }

        return auditList;
    }

    #endregion

    #region IsAudited
    public async Task<AuditStatusVm> IsAudited(DateTime businessDate)
    {
        var targetDate = businessDate.Date;

        var roomAudits = await Context.HtRoomDayAudits
            .Where(r => r.BusinessDate == targetDate).ToListAsync();

        var isAllRoomCharged = roomAudits.All(r => r.IsCharged);

        var isRoomAuditGenerated = Context.HtRoomDayAudits.Any(r => r.BusinessDate == targetDate);

        var allFrontOfficePaymentsAudited = await Context.HtBookingPayments
            .Where(p => p.PaidDate.Date == targetDate).AllAsync(x => x.AuditById > 0 && x.AuditDate != null);

        var allExtraServiceAudited = await Context.HtBillingDetails
            .Where(o => o.Service.ServiceCode != HtServiceCode.RoomRent
            && o.Service.ServiceCode != HtServiceCode.FoodService
            && o.Service.ServiceCode != HtServiceCode.HallRent && o.ServiceDate != null && o.ServiceDate.Value.Date == targetDate)
            .AllAsync(o => o.AuditById > 0 && o.AuditDate != null);

        var isAllRestRevenueAudited = await Context.RsFoodOrders
            .Where(i => i.OrderDate.Date == targetDate)
            .AllAsync(i => i.AuditById > 0 && i.AuditDate != null);

        var allRestaurantPaymentsAudited = await Context.RsOrderPayments
            .Where(p => p.PaidDate.Date == targetDate)
            .AllAsync(x => x.AuditById > 0 && x.AuditDate != null);

        var isAllBanquetAudited = await Context.HtBookingHalls
            .Where(b => b.BookingDate.Date == targetDate)
            .AllAsync(b => b.AuditDate != null);

        var isAllAudited = isAllRoomCharged
                            && allFrontOfficePaymentsAudited
                            && allExtraServiceAudited
                            && isAllRestRevenueAudited
                            && allRestaurantPaymentsAudited
                            && isAllBanquetAudited;

        var model = new AuditStatusVm();
        model.IsAllRoomCharged = isRoomAuditGenerated && isAllRoomCharged ? isAllRoomCharged : false;
        model.IsFoPaymentAudited = allFrontOfficePaymentsAudited;
        model.IsExtraServiceAudited = allExtraServiceAudited;
        model.IsFoodOrderAudited = isAllRestRevenueAudited;
        model.IsRestPaymentAudited = allRestaurantPaymentsAudited;
        model.IsBanquetAudited = isAllBanquetAudited;
        model.IsAllAudited = isAllAudited;
        model.IsRoomAuditGenerated = isRoomAuditGenerated;

        return model;
    }
    #endregion

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }

    #endregion
}
