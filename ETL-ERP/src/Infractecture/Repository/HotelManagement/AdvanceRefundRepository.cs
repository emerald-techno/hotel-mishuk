using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.ViewModel.HotelManagement.AdvanceRefund;
using Domain.ViewModel.HotelManagement.RefundReport;
using Domain.ViewModel.Report;
using Interface.Repository.HotelManagement;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Persistence.DapperModel;
using Repository.Base;
using DU = Domain.Utility;

namespace Repository.HotelManagement;

public class AdvanceRefundRepository : BaseRepository<HtAdvanceRefund>, IAdvanceRefundRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;
    private readonly IApplicationReadDbConnection _iReadDbConnection;

    public AdvanceRefundRepository(ApplicationDbContext db, IMapper iMapper, IApplicationReadDbConnection iReadDbConnection) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
        _iReadDbConnection = iReadDbConnection;
    }
    #endregion

    #region PrepareAdvanceRefundByBookingId

    public async Task<AdvanceRefundVm> PrepareAdvanceRefundByBookingId(long bookingId)
    {
        if (!(bookingId > 0))
            throw new Exception("Booking Information Is Not Correct..!!");

        var bookingInfo = await Context.HtBookingServices.FirstOrDefaultAsync(x => x.Id == bookingId && !x.IsDeleted);
        if (bookingInfo == null)
            throw new Exception("Booking Information Not Found..!!");

        var model = new AdvanceRefundVm();
        model.BookingId = bookingInfo.Id;
        model.BookingNo = bookingInfo.BookingNo;
        model.BookingDate = bookingInfo.BookingDate;
        model.BookingCheckInDate = bookingInfo.CheckInTime;
        model.BookingCheckOutDate = bookingInfo.CheckOutTime;
        model.BookingNetAmount = bookingInfo.NetRent;

        var paidList = await Context.HtBookingPayments.Where(x => x.BookingId == bookingInfo.Id).ToListAsync();
        var refundList = await Context.HtAdvanceRefunds.Where(x => x.BookingId == bookingInfo.Id).ToListAsync();

        model.PaidAmount = paidList.Sum(x => x.PaidAmount);
        model.AlreadyRefundAmount = refundList.Sum(x => x.RefundAmount);

        return model;
    }

    #endregion

    public async Task<List<RefundReportVm>> HtRefundDataAsync(RefundReportVm vm)
    {
        vm.StrFromDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrFromDate;
        vm.StrToDate = (string.IsNullOrEmpty(vm.StrToDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrToDate;
        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)).ToString("dd/MMM/yyyy");
        string fromDateFilter = (!string.IsNullOrEmpty(vm.StrFromDate)) ? $" and convert(date,ar.RefundDate) >= '{fromDate}'" : "";
        string toDateFilter = (!string.IsNullOrEmpty(vm.StrToDate)) ? $" and convert(date,ar.RefundDate) <= '{toDate}'" : "";
        string guestFilter = (vm.GuestId > 0) ? $" and g.Id = {vm.GuestId}" : "";
        string companyFilter = (vm.CompanyId > 0) ? $" and ci.id = {vm.CompanyId}" : "";

        string query = $@"select bs.id BookingId,bs.BookingNo,convert(date,bs.BookingDate) BookingDate,g.Id GuestId,g.FirstName +' '+ISNULL(g.LastName,'') GuestName, g.Mobile GuestMobile,ci.id CompanyId,ci.Name CompanyName,convert(Date,bs.CheckInTime) EstCheckInTime
            ,convert(date,bs.CheckOutTime) EstCheckOutTime,ar.RefundAmount,convert(date,ar.RefundDate) RefundDate,ar.RefundMode,ar.Description,ar.TransactionNo
            from HtAdvanceRefunds ar
            inner join HtBookingServices bs on bs.Id = ar.BookingId
            left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
            left join HtGuestInfos g on g.Id= bg.GuestId
            left join ClientCompanies ci on ci.Id = g.CompanyId
            where ar.RefundAmount > 0 {fromDateFilter} {toDateFilter} {guestFilter} {companyFilter}
            order by convert(date,ar.RefundDate) desc ";
        try
        {
            var data = await _iReadDbConnection.QueryAsync<RefundReportVm>(query);
            return data.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }


    #endregion
}