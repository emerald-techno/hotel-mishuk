using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Billing;
using Domain.ViewModel.HotelManagement.Booking;
using Domain.ViewModel.HotelManagement.FD_PaymentTranReport;
using Domain.ViewModel.HotelManagement.HotelReport;
using Domain.ViewModel.HotelManagement.MoneyReceipt;
using Domain.ViewModel.HotelManagement.OnlineBooking;
using Domain.ViewModel.HotelManagement.RoomBooking;
using Domain.ViewModel.Restaurant.FoodOrder;
using Domain.ViewModel.Website;
using Interface.Repository.Accounts;
using Interface.Repository.HotelManagement;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Persistence.DapperModel;
using Repository.Base;
using DU = Domain.Utility;

namespace Repository.HotelManagement;

public class BookingServiceRepository : BaseRepository<HtBookingService>, IBookingServiceRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;
    private readonly IApplicationReadDbConnection _iReadDbConnection;
    private readonly IAccTranMstRepository _iAccTranMstRepository;

    public BookingServiceRepository(ApplicationDbContext db, IMapper iMapper,
        IApplicationReadDbConnection iReadDbConnection,
        IAccTranMstRepository iAccTranMstRepository) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
        _iReadDbConnection = iReadDbConnection;
        _iAccTranMstRepository = iAccTranMstRepository;
    }
    #endregion

    #region Search
    public async Task<DataTablePagination<BookingServiceSearchVm, BookingServiceSearchVm>> SearchAsync(DataTablePagination<BookingServiceSearchVm, BookingServiceSearchVm> vm)
    {
        var bookingServices = Context.HtBookingServices.AsNoTracking();
        var bookingGuests = Context.HtBookingGuests.AsNoTracking();
        var guestInfos = Context.HtGuestInfos.AsNoTracking();
        var bookingRooms = Context.HtBookingRooms.AsNoTracking();
        var onlineBookings = Context.HtOnlineBookings.AsNoTracking();

        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search booking not found");

        var searchResult = await (from booking in bookingServices

                                  let onlineBooking = booking.OnlineBookingId == null ? null :
                                  (from onlineBooking in onlineBookings
                                   where onlineBooking.Id == booking.OnlineBookingId
                                   select new OnlineBookingDto
                                   {
                                       OnlineBookingNumber = onlineBooking.OnlineBookingNumber,
                                       OnlineBookingDate = onlineBooking.OnlineBookingDate,
                                       GuestName = onlineBooking.GuestName,
                                       GuestMobile = onlineBooking.GuestMobile
                                   }).First()

                                  join bookingGuest in bookingGuests
                                  on booking.Id equals bookingGuest.BookingId
                                  join guest in guestInfos
                                  on bookingGuest.GuestId equals guest.Id
                                  where bookingGuest.IsMain
                                  select new BookingServiceSearchDto
                                  {
                                      Id = booking.Id,
                                      BookingNo = booking.BookingNo,
                                      BookingDate = booking.BookingDate,
                                      BookingType = booking.BookingType,
                                      CheckInTime = booking.CheckInTime,
                                      CheckOutTime = booking.CheckOutTime,
                                      BookingStatus = booking.BookingStatus,
                                      PaymentStatus = booking.PaymentStatus,
                                      NetRent = booking.NetRent,
                                      TotalGuest = booking.TotalGuest,
                                      GuestName = $"{guest.Salutation} {guest.FirstName} {guest.LastName}",
                                      GuestMobile = guest.Mobile,
                                      OnlineBookingId = booking.OnlineBookingId,
                                      OnlineBookingNumber = onlineBooking.OnlineBookingNumber,
                                      OnlineGuestMobile = onlineBooking.GuestMobile,
                                      CancelDate = booking.CancelDate,
                                      BookingConfirmStatus = booking.BookingConfirmStatus
                                  }).ToListAsync();


        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.GuestName.ToLower().Contains(value) || c.BookingNo.ToLower().Contains(value)).ToList();
        }

        if (!string.IsNullOrEmpty(model.BookingType))
        {
            searchResult = searchResult.Where(c => c.BookingType == model.BookingType).ToList();
        }

        if (model.OnlineBookingId > 0)
        {
            searchResult = searchResult.Where(c => c.OnlineBookingId == model.OnlineBookingId).ToList();
        }

        if (model.OnlyUnPaid)
        {
            searchResult = searchResult.Where(c => c.PaymentStatus == PaymentStatusEnum.PartialPayment || c.PaymentStatus == PaymentStatusEnum.Pending || c.BookingStatus == BookingServiceStatusEnum.Booked || c.BookingStatus == BookingServiceStatusEnum.CheckIn).ToList();
        }

        if (!string.IsNullOrEmpty(model.BookingNo))
        {
            searchResult = searchResult.Where(c => c.BookingNo.ToLower().Contains(model.BookingNo.ToLower())).ToList();
        }

        if (!string.IsNullOrEmpty(model.GuestName))
        {
            searchResult = searchResult.Where(c => c.GuestName.ToLower().Contains(model.GuestName.ToLower())).ToList();
        }

        if (!string.IsNullOrEmpty(model.GuestMobile))
        {
            searchResult = searchResult.Where(c => c.GuestMobile.ToLower().Contains(model.GuestMobile.ToLower())).ToList();
        }

        if (model.BookingStatus > 0)
        {
            searchResult = searchResult.Where(c => c.BookingStatus == model.BookingStatus).ToList();
        }

        if (model.PaymentStatus != null && (int)model.PaymentStatus < 10)
        {
            searchResult = searchResult.Where(c => c.PaymentStatus == model.PaymentStatus).ToList();
        }

        if (!string.IsNullOrEmpty(model.FormDateStr))
        {
            var formDate = (DateTime)(!string.IsNullOrEmpty(model.FormDateStr) ? Utility.ConvertStrToDate(model.FormDateStr) : model.BookingDate);
            searchResult = searchResult.Where(c => c.BookingDate.Date >= formDate.Date).ToList();
        }

        if (!string.IsNullOrEmpty(model.ToDateStr))
        {
            var toDate = (DateTime)(!string.IsNullOrEmpty(model.ToDateStr) ? Utility.ConvertStrToDate(model.ToDateStr) : model.BookingDate);
            searchResult = searchResult.Where(c => c.BookingDate.Date <= toDate.Date).ToList();
        }

        var totalRecords = searchResult.Count();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = searchResult.OrderByDescending(c => c.BookingDate).ThenByDescending(x => x.BookingNo)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToList();

            vm.data = _iMapper.Map<List<BookingServiceSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;

                var roomList = bookingRooms.Include(r => r.Room).Where(x => x.BookingId == searchDto.Id).Select(x => x.Room.RoomNo).ToList();

                searchDto.RoomList = string.Join(", ", roomList);
            }
        }
        return vm;
    }

    #endregion

    #region GetBookingByIdAsync

    public async Task<HtBookingServiceVm> GetBookingByIdAsync(long id)
    {
        var dataModel = await Context.HtBookingServices
                                .Include(x => x.ActionBy)
                                  .AsNoTracking()
                                .FirstOrDefaultAsync(c => c.Id == id && c.BookingType == BookingType.Room && !c.IsDeleted);


        if (dataModel == null) throw new Exception("Booking Not Found...!!");

        var model = _iMapper.Map<HtBookingServiceVm>(dataModel);
        model.ActionByName = dataModel.ActionBy.FullName;

        var paidList = Context.HtBookingPayments.Where(c => c.BookingId == dataModel.Id).ToList();
        model.PaidAmount = paidList.Sum(x => x.PaidAmount);
        model.BookingPayments = _iMapper.Map<List<HtBookingPaymentVm>>(paidList);

        var paymentIds = paidList.Select(x => x.Id).ToList();

        var refundList = Context.HtAdvanceRefunds.Where(c => c.BookingId == dataModel.Id).ToList();
        model.RefundAmount = refundList.Sum(x => x.RefundAmount);

        model.BookingPayments = _iMapper.Map<List<HtBookingPaymentVm>>(paidList);

        var voucherList = await _iAccTranMstRepository.GetAsync(x => paymentIds.Contains(x.PaymentId.Value));

        if (voucherList.Count > 0)
        {
            foreach (var payment in model.BookingPayments)
            {
                var voucher = voucherList.FirstOrDefault(v => v.PaymentId == payment.Id);
                if (voucher != null)
                {
                    payment.VoucherNo = voucher.VcNo;
                    payment.VoucherId = voucher.Id;
                }
            }

        }

        var foodBillList = await Context.RsFoodOrders
            .Include(c => c.CustomerType)
            .Include(r => r.Room)
            .Include(c => c.CustomerType)
            .Where(x => x.BookingId == dataModel.Id
                    && x.OrderStatus != RsOrderStatusEnum.Canceled)
            .ToListAsync();

        var foodBillIds = foodBillList.Select(x => x.Id);

        var frontOfficePaidOrderIds = await Context.RsOrderPayments
                                                    .Where(x => foodBillIds.Contains(x.OrderId)
                                                             && x.BillDtl != null)
                                                    .Select(x => x.OrderId)
                                                    .Distinct()
                                                    .ToListAsync();

        foodBillList = foodBillList.Where(x => x.CustomerType.TypeCode == RsCustomerTypeCode.Hotel).ToList();

        foodBillList = foodBillList.Where(x => x.PaymentStatus != RsOrderPaymentStatusEnum.FullPayment || frontOfficePaidOrderIds.Contains(x.Id)).ToList();

        model.FoodOrders = _iMapper.Map<List<FoodOrderVm>>(foodBillList);

        if (model.FoodOrders.Count > 0)
        {
            foreach (var searchDto in model.FoodOrders)
            {
                var filterData = foodBillList.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.RoomNo = filterData?.Room?.RoomNo;
            }
        }

        #region billInfo

        var bookingBill = await Context.HtBillings
            .Include(d => d.BillingDetails)
            .ThenInclude(s => s.Service)
            .FirstOrDefaultAsync(x => x.BookingId == dataModel.Id && !x.IsDeleted);

        var billServices = await Context.HtServices.Where(x => !x.IsDeleted).ToListAsync();

        if (bookingBill != null)
        {
            var roomRentalService = billServices.FirstOrDefault(x => x.ServiceCode == HtServiceCode.RoomRent);
            if (roomRentalService == null)
                throw new Exception("Room Rent Service Not Found...!!");

            var bookingRoomList = await Context.HtBookingRooms
                    .Include(r => r.Room)
                    .Where(x => x.BookingId == model.Id && !x.IsDeleted)
                    .ToListAsync();

            #region RoomList

            if (model.BookingStatus != BookingServiceStatusEnum.CheckOut)
            {
                bookingRoomList = bookingRoomList.Where(x => x.ActualCheckInTime != null && x.ActualCheckOutTime == null).ToList();

                foreach (var bookingRoom in bookingRoomList)
                {
                    var detailModel = new HtBillingDetail();

                    double days = AppUtility.DaysDiffernceOnlyDate(bookingRoom.CheckOutTime, (DateTime)bookingRoom.ActualCheckInTime);

                    if (bookingRoom.BookingDayStatus == (int)BookingDayStatusEnum.HalfDay)
                    {
                        days = (days + 0.5);
                    }
                    else if (bookingRoom.BookingDayStatus == (int)BookingDayStatusEnum.DayUse)
                    {
                        days = (days + 1);
                    }

                    detailModel.Quantity = days;
                    detailModel.BookingRoomId = bookingRoom.Id;
                    detailModel.ServiceId = roomRentalService.Id;
                    detailModel.ActionDate = Utility.GetBdDateTimeNow();
                    detailModel.Rate = bookingRoom.Room.Rent;
                    detailModel.Amount = bookingRoom.Rent;
                    detailModel.VAT = bookingRoom.Vat;
                    detailModel.Tax = bookingRoom.Tax;
                    detailModel.Discount = bookingRoom.Discount;
                    detailModel.ServiceCharge = bookingRoom.ServiceCharge;
                    detailModel.ExtraBedCharge = bookingRoom.ExtraBedCharge;
                    detailModel.NetAmount = bookingRoom.NetRent;
                    detailModel.Remarks = $"Bill For Room No. {bookingRoom.Room.RoomNo}";

                    bookingBill.BillingDetails.Add(detailModel);
                }
            }

            #endregion

            model.BillId = bookingBill.Id;
            model.IsBillGenerated = true;
            model.BookingBill = _iMapper.Map<BillingVm>(bookingBill);

            #region FoodService

            var bookingUnpaidFoodList = await Context.RsFoodOrders.Include(c => c.CustomerType)
                .Where(x => x.BookingId == bookingBill.BookingId
            && x.PaymentStatus != RsOrderPaymentStatusEnum.FullPayment
            && x.OrderStatus != RsOrderStatusEnum.Canceled).ToListAsync();

            var foodService = billServices.FirstOrDefault(x => x.ServiceCode == HtServiceCode.FoodService);
            if (foodService == null)
                throw new Exception("Food Service Not Found...!!");

            foreach (var bookingRoom in bookingRoomList)
            {
                var detailModel = new BillingDetailVm();

                var anyUnpaidFoodBillList = bookingUnpaidFoodList.Where(x => x.RoomId == bookingRoom.RoomId).ToList();

                anyUnpaidFoodBillList = anyUnpaidFoodBillList.Where(x => x.CustomerType.TypeCode == RsCustomerTypeCode.Hotel
                || x.CustomerType.TypeCode == RsCustomerTypeCode.WalkIn).ToList();

                if (anyUnpaidFoodBillList == null || !(anyUnpaidFoodBillList.Count > 0))
                    continue;

                detailModel.BookingRoomId = bookingRoom.Id;
                detailModel.ServiceId = foodService.Id;
                detailModel.Quantity = anyUnpaidFoodBillList.Count();
                detailModel.ServiceName = foodService.ServiceName;
                detailModel.Amount = anyUnpaidFoodBillList.Sum(x => x.OrderAmount);
                detailModel.VAT = anyUnpaidFoodBillList.Sum(x => x.VAT);
                detailModel.Tax = anyUnpaidFoodBillList.Sum(x => x.TAX);
                detailModel.Discount = anyUnpaidFoodBillList.Sum(x => x.Discount);
                detailModel.ServiceCharge = anyUnpaidFoodBillList.Sum(x => x.ServiceCharge);

                var unpaidIds = anyUnpaidFoodBillList.Select(x => x.Id).ToList();
                var rsPaidList = await Context.RsOrderPayments.Where(c => unpaidIds.Contains(c.OrderId) && c.PaymentType == RsOrderPaymentTypeEnum.Receive && !c.IsDeleted).ToListAsync();
                var alreadyPaidAmount = rsPaidList.Sum(x => x.PaidAmount);

                detailModel.NetAmount = anyUnpaidFoodBillList.Sum(x => x.NetAmount) - (alreadyPaidAmount);

                detailModel.Remarks = $"Food Service Bill For Room No. {bookingRoom.Room.RoomNo}";

                model.BookingBill.BillingDetails.Add(detailModel);
            }

            #endregion



            if (model.BookingBill.BillingDetails.Count > 0)
            {
                foreach (var item in model.BookingBill.BillingDetails)
                {
                    var filterData = bookingBill.BillingDetails.FirstOrDefault(x => x.Id == item.Id);

                    item.ServiceName = filterData?.Service?.ServiceName;
                    item.ServiceCode = filterData?.Service?.ServiceCode;
                    item.BookingRoomNo = filterData?.BookingRoom?.Room?.RoomNo;

                    if (item.ServiceId == roomRentalService.Id)
                    {
                        item.ServiceName = roomRentalService.ServiceName;
                        item.ServiceCode = roomRentalService.ServiceCode;
                    }

                    if (item.ServiceId == foodService.Id)
                    {
                        item.ServiceName = foodService.ServiceName;
                        item.ServiceCode = foodService.ServiceCode;
                    }

                    if (model.Services == null)
                        model.Services = new List<BillingDetailVm>();

                    if (item.ServiceCode != HtServiceCode.RoomRent && item.ServiceCode != HtServiceCode.FoodService)
                    {
                        model.Services.Add(item);
                    }
                }
            }
        }
        else
        {
            model.IsBillGenerated = false;
        }

        #endregion

        return model;
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }

    #endregion

    #region Booking Details
    public async Task<HtBookingServiceVm> GetBookingDetailsAsync(long id)
    {
        var dataModel = await Context.HtBookingServices
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (dataModel == null) throw new Exception("Booking Not Found...!!");

        var model = _iMapper.Map<HtBookingServiceVm>(dataModel);

        var paidList = Context.HtBookingPayments.Where(c => c.BookingId == dataModel.Id).ToList();
        model.PaidAmount = paidList.Sum(x => x.PaidAmount);

        return model;
    }
    #endregion

    #region GetHallBookingByIdAsync

    public async Task<HtBookingServiceVm> GetHallBookingByIdAsync(long id)
    {
        var dataModel = await Context.HtBookingServices
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.BookingType == BookingType.Hall && !c.IsDeleted);

        if (dataModel == null) throw new Exception("Hall Booking Not Found...!!");

        var model = _iMapper.Map<HtBookingServiceVm>(dataModel);

        var hallList = Context.HtBookingHalls
            .Include(h => h.Hall)
            .Where(x => x.BookingId == dataModel.Id)
            .ToList();

        model.BookingHallVms = _iMapper.Map<List<HtBookingHallVm>>(hallList);

        if (model.BookingHallVms.Count > 0)
        {
            foreach (var hall in model.BookingHallVms)
            {
                var filterData = hallList.FirstOrDefault(x => x.HallId == hall.HallId);

                hall.HallName = $"{filterData.Hall.HallName}";
            }
        }

        var guestList = Context.HtBookingGuests.Where(x => x.BookingId == dataModel.Id)
            .Include(g => g.Guest)
            .ToList();

        model.BookingGuestVms = _iMapper.Map<List<HtBookingGuestVm>>(guestList);

        if (model.BookingGuestVms.Count > 0)
        {
            foreach (var guest in model.BookingGuestVms)
            {
                var filterData = guestList.FirstOrDefault(x => x.GuestId == guest.GuestId);

                guest.GuestName = $"{filterData.Guest?.Salutation} {filterData.Guest?.FirstName} {filterData.Guest?.LastName}";
                guest.GuestMobile = $"{filterData.Guest?.Mobile}";
            }
        }

        var paidList = Context.HtBookingPayments.Where(c => c.BookingId == dataModel.Id).ToList();
        model.PaidAmount = paidList.Sum(x => x.PaidAmount);

        var refundList = Context.HtAdvanceRefunds.Where(c => c.BookingId == dataModel.Id).ToList();
        model.RefundAmount = refundList.Sum(x => x.RefundAmount);

        var bookingBill = await Context.HtBillings.FirstOrDefaultAsync(x => x.BookingId == dataModel.Id && !x.IsDeleted);
        if (bookingBill != null)
            model.IsBillGenerated = true;
        else
            model.IsBillGenerated = false;

        model.BookingPayments = _iMapper.Map<List<HtBookingPaymentVm>>(paidList);

        return model;
    }

    #endregion

    #region GetDashboardData
    public async Task<IndexRoomVm> GetDashboardDataAsync()
    {
        var bookingServices = Context.HtBookingServices.AsNoTracking();
        var roomService = Context.HtRoomInfos.AsNoTracking();
        var guestService = Context.HtGuestInfos.AsNoTracking();

        var model = new IndexRoomVm();
        var roomInfoList = await roomService.Where(x => !x.IsDeleted && x.IsActive).ToListAsync();

        var guestInfoCount = await guestService.Where(x => !x.IsDeleted).CountAsync();
        var todayCheckingList = await bookingServices.Where(x => x.CheckInTime.Date == DateTime.Today.Date).CountAsync();
        var todayCheckOutList = await bookingServices.Where(x => x.CheckOutTime.Date == DateTime.Today.Date).CountAsync();

        model.GuestCount = guestInfoCount;
        model.TodayCheckId = todayCheckingList;
        model.TodayCheckOut = todayCheckOutList;
        model.Rooms = roomInfoList.OrderBy(x => x.RoomNo).ToList();
        model.AvailableCount = roomInfoList.Where(x => x.BookingStatus == BookingStatusEnum.Available).Count();
        model.BookedCount = roomInfoList.Where(x => x.BookingStatus == BookingStatusEnum.Booked).Count();
        model.OccupiedCount = roomInfoList.Where(x => x.CleaningStatus == CleaningStatusEnum.O).Count();

        return model;
    }
    #endregion

    #region TodayArrival

    public async Task<List<BookingArrivalVm>> TodayArrivalDataAsync(BookingArrivalVm vm)
    {
        var queryDate = (DateTime)(!string.IsNullOrEmpty(vm.StrQueryDate) ? Utility.ConvertStrToDate(vm.StrQueryDate) : DateTime.Today);

        var bookingServices = Context.HtBookingServices.AsNoTracking();

        var bookingRooms = Context.HtBookingRooms
            .Include(x => x.Room)
            .AsNoTracking();

        var bookingGusets = Context.HtBookingGuests
            .Include(x => x.Guest)
            .AsNoTracking();

        var bookingPayments = Context.HtBookingPayments.AsNoTracking();

        var todayBookingList = await bookingServices.Where(x => x.CheckInTime.Date <= queryDate.Date && x.CheckOutTime.Date > queryDate.Date && x.BookingType == BookingType.Room && (x.BookingStatus == BookingServiceStatusEnum.Booked || x.BookingStatus == BookingServiceStatusEnum.Canceled || x.BookingStatus == BookingServiceStatusEnum.CheckIn)).ToListAsync();

        var dataResult = new List<BookingArrivalVm>();

        if (todayBookingList.Count > 0)
        {
            foreach (var booking in todayBookingList)
            {
                var model = new BookingArrivalVm();
                var guest = bookingGusets.FirstOrDefault(x => x.BookingId == booking.Id && x.IsMain);
                if (guest == null)
                    throw new Exception("No Contact Person Found For Booking..!");

                var bookingRoomList = bookingRooms.Where(x => x.BookingId == booking.Id && !x.IsDeleted).ToList();
                if (!(bookingRoomList.Count > 0))
                    throw new Exception("No Rooms Found For Booking..!");

                var roomList = bookingRoomList.Where(x => x.CheckInTime.Date == queryDate.Date && x.ActualCheckInTime == null).Select(x => x.Room).ToList();

                if (!(roomList.Count > 0))
                    continue;

                model.BookingId = booking.Id;
                model.BookingNo = booking.BookingNo;
                model.BookingDate = booking.BookingDate;
                model.CheckInTime = booking.CheckInTime;
                model.CheckOutTime = booking.CheckOutTime;
                model.BookingStatus = booking.BookingStatus;
                model.PaymentStatus = booking.PaymentStatus;
                model.NetRent = booking.NetRent;
                model.GuestName = $"{guest.Guest.Salutation} {guest.Guest.FirstName} {guest.Guest.LastName}";
                model.GuestMobile = guest.Guest.Mobile;
                model.RoomList = string.Join(",", roomList.Select(s => s.RoomNo).ToList());
                model.RoomCount = roomList.Count;
                model.Remarks = booking.Remarks;

                var advancePaymentList = bookingPayments.Where(x => x.BookingId == booking.Id && !x.IsDeleted && x.PaidDate.Date <= DateTime.Today.Date).ToList();

                model.AdvanceAmount = advancePaymentList.Sum(x => x.PaidAmount);

                if (booking.BookingStatus == BookingServiceStatusEnum.Canceled && !(model.AdvanceAmount > 0))
                    continue;

                dataResult.Add(model);
            }
        }
        dataResult = dataResult.OrderBy(o => o.Remarks).ToList();
        return dataResult;
    }

    #endregion

    #region TodayExpectedCheckOut

    public async Task<List<BookingArrivalVm>> TodayExpectedCheckOutDataAsync(BookingArrivalVm vm)
    {
        vm.StrQueryDate = string.IsNullOrEmpty(vm.StrQueryDate) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrQueryDate;

        DateTime fromDate = string.IsNullOrEmpty(vm.StrFromDate) ? DateTime.Today : Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate));

        DateTime toDate = string.IsNullOrEmpty(vm.StrToDate) ? DateTime.Today : Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate));

        string query = $@" 
                            DECLARE @FromDate DATE = @pFromDate;
                            DECLARE @ToDate   DATE = @pToDate;

                            SELECT 
                            b.Id AS BookingId,
                            b.BookingNo,
                            b.BookingDate,
                            b.Remarks,
                            b.CheckInTime,
                            b.CheckOutTime,
                            b.BookingStatus,
                            b.PaymentStatus,
                            b.NetRent,
                            (g.Salutation + ' ' + g.FirstName + ' ' + isnull(g.LastName,'')) AS GuestName,
                            g.Mobile AS GuestMobile,
                            STUFF((
                                SELECT ',' + r.RoomNo
                                FROM HtBookingRooms br2
                                INNER JOIN HtRoomInfos r ON r.Id = br2.RoomId
                                WHERE br2.BookingId = b.Id 
                                  AND br2.IsDeleted = 0
                                  AND CAST(br2.CheckOutTime AS DATE) BETWEEN @FromDate AND @ToDate
                                  AND br2.ActualCheckInTime IS NOT NULL 
                                  AND br2.ActualCheckOutTime IS NULL
                                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'') AS RoomList
                        FROM HtBookingRooms br
                        INNER JOIN HtBookingServices b ON b.Id = br.BookingId
                        INNER JOIN HtBookingGuests bg ON bg.BookingId = b.Id AND bg.IsMain = 1
                        INNER JOIN HtGuestInfos g ON g.Id = bg.GuestId
                        WHERE CAST(br.CheckOutTime AS DATE) BETWEEN @FromDate AND @ToDate
                          AND br.ActualCheckInTime IS NOT NULL
                          AND br.ActualCheckOutTime IS NULL
                          AND b.BookingType = 'R'   -- (Room type, adjust enum value)
                          AND b.BookingStatus = 2   -- (CheckIn status, adjust enum value)
                        GROUP BY 
                            b.Id, b.BookingNo, b.BookingDate, b.Remarks, 
                            b.CheckInTime, b.CheckOutTime, b.BookingStatus, 
                            b.PaymentStatus, b.NetRent, g.Salutation, g.FirstName, g.LastName, g.Mobile;";

        var data = await _iReadDbConnection.QueryAsync<BookingArrivalVm>(query, new { pFromDate = vm.StrFromDate, pToDate = vm.StrToDate });
        return data.ToList();
    }

    #endregion

    #region TodayDepartureDataAsync

    public async Task<List<DepartureReportVm>> TodayDepartureDataAsync(DepartureReportVm vm)
    {
        vm.StrQueryDate = string.IsNullOrEmpty(vm.StrQueryDate) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrQueryDate;

        DateTime fromDate = string.IsNullOrEmpty(vm.StrFromDate) ? DateTime.Today : Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate));

        DateTime toDate = string.IsNullOrEmpty(vm.StrToDate) ? DateTime.Today : Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate));

        string query = $@" 
                            DECLARE @FromDate DATE = @pFromDate;
                            DECLARE @ToDate   DATE = @pToDate;

                        SELECT 
                            br.ActualCheckOutTime AS CheckedOutDateTime,
                            CAST(br.ActualCheckOutTime AS DATE) AS CheckedOutDate,
                            CAST(br.ActualCheckOutTime AS TIME) AS CheckedOutTime,
                            br.ActualCheckInTime AS CheckInDateTime,
                            r.RoomNo,
                            rc.CategoryName AS RoomCategory,
                            (g.Salutation + ' ' + g.FirstName + ' ' + isnull(g.LastName,'')) AS GuestName,
                            g.Mobile AS GuestMobile,
                            n.Name AS CountryName,
                            c.Name AS CompanyName,
                            b.BillNumber,
                            b.Id BillId,
                            bs.BookingNo,
                            bs.Id as BookingId,
                            bs.Adult,
                            bs.Child,
                            br.RoomRent,
                            bs.Remarks
                        FROM HtBookingRooms br
                        INNER JOIN HtRoomInfos r ON r.Id = br.RoomId
                        INNER JOIN HtRoomCategories rc ON rc.Id = r.RoomCategoryId
                        INNER JOIN HtBookingServices bs ON bs.Id = br.BookingId
                        INNER JOIN HtBookingGuests bg ON bg.BookingId = bs.Id AND bg.IsMain = 1
                        INNER JOIN HtGuestInfos g ON g.Id = bg.GuestId
                        INNER JOIN SetCountries n ON n.Id = g.CountryId
                        LEFT JOIN ClientCompanies c ON c.Id = g.CompanyId
                        LEFT JOIN HtBillings b ON b.BookingId = bs.Id
                        WHERE br.ActualCheckOutTime IS NOT NULL
                            AND CAST(br.ActualCheckOutTime AS DATE) BETWEEN @FromDate AND @ToDate
                            AND bs.BookingType = 'R'   -- Room type
                            AND bs.BookingStatus = 3   -- Checked-Out status (adjust enum value)
                        ORDER BY br.ActualCheckOutTime DESC;";

        //var data = await _iReadDbConnection.QueryAsync<DepartureReportVm>(query);
        var data = await _iReadDbConnection.QueryAsync<DepartureReportVm>(query, new { pFromDate = vm.StrFromDate, pToDate = vm.StrToDate });
        return data.ToList();
    }

    #endregion

    #region InHouseGuest

    public async Task<List<RoomWiseGuestVm>> InHouseGuestDataAsync()
    {
        var bookingServices = Context.HtBookingServices.AsNoTracking();

        var bookingRooms = Context.HtBookingRooms
            .Include(x => x.Room)
            .Include(x => x.Complementary)
            .AsNoTracking();

        var bookingGusets = Context.HtBookingGuests
            .Include(x => x.Guest)
            .AsNoTracking();

        var inHouseBookRoomList = await bookingServices
            .Join(bookingRooms,
                  service => service.Id,
                  room => room.BookingId,
                  (service, room) => new { Service = service, Room = room })
            .Where(x => x.Service.BookingStatus == BookingServiceStatusEnum.CheckIn && x.Service.BookingType == BookingType.Room)
            .Select(x => x.Room)
            .ToListAsync();

        var dataResult = new List<RoomWiseGuestVm>();

        if (inHouseBookRoomList.Count > 0)
        {
            foreach (var room in inHouseBookRoomList)
            {
                var model = new RoomWiseGuestVm();

                var guest = bookingGusets.FirstOrDefault(x => x.BookingId == room.BookingId && x.IsMain);
                if (guest == null)
                    throw new Exception("No Contact Person Found For Booking..!");

                if (room.ActualCheckOutTime != null)
                    continue;

                model.BookingRoomId = (long)room.RoomId;
                model.BookingRoomNo = room.Room.RoomNo;
                model.TotalGuest = room.TotalGuest;
                model.CheckInTime = room.CheckInTime;
                model.CheckOutTime = room.CheckOutTime;
                model.GuestName = $"{guest.Guest.Salutation} {guest.Guest.FirstName} {guest.Guest.LastName}";
                model.GuestMobile = guest.Guest.Mobile;
                model.ComplementaryId = room.ComplementaryId;
                model.ComplementaryName = room?.Complementary?.Title;

                if (room.ActualCheckInTime != null)
                {
                    dataResult.Add(model);
                }
            }
        }

        return dataResult;
    }

    #endregion

    #region RoomDailySales

    public async Task<List<RoomDailySalesReportVm>> RoomDailySalesDataAsync()
    {
        var bookingServices = Context.HtBookingServices.AsNoTracking();

        var bookingRooms = Context.HtBookingRooms
            .Include(x => x.Room)
            .AsNoTracking();

        var bookingGusets = Context.HtBookingGuests
            .Include(x => x.Guest)
            .AsNoTracking();

        var occupiedBookingList = await bookingServices
            .Join(bookingRooms,
                  service => service.Id,
                  room => room.BookingId,
                  (service, room) => new { Service = service, Room = room })
            .Where(x => x.Service.BookingStatus == BookingServiceStatusEnum.CheckIn && x.Service.BookingType == BookingType.Room)
            .Select(x => x.Room)
            .ToListAsync();

        var dataResult = new List<RoomDailySalesReportVm>();

        if (occupiedBookingList.Count > 0)
        {
            foreach (var ocRoom in occupiedBookingList)
            {
                var model = new RoomDailySalesReportVm();

                var guest = bookingGusets.FirstOrDefault(x => x.BookingId == ocRoom.BookingId && x.IsMain);
                if (guest == null)
                    throw new Exception("No Contact Person Found For Booking..!");

                model.BookingRoomId = (long)ocRoom.RoomId;
                model.BookingRoomNo = ocRoom.Room.RoomNo;
                model.CheckInTime = ocRoom.CheckInTime;
                model.CheckOutTime = ocRoom.CheckOutTime;
                model.GuestName = $"{guest.Guest.Salutation} {guest.Guest.FirstName} {guest.Guest.LastName}";
                model.GuestMobile = guest.Guest.Mobile;
                model.RoomRate = ocRoom.Room.TotalRent;
                model.Discount = ocRoom.Discount;
                model.NetSale = (model.RoomRate - model.Discount);

                dataResult.Add(model);
            }
        }

        return dataResult;
    }

    #endregion

    #region Daily In House Guest List
    public async Task<List<RoomWiseGuestVm>> DailyInHouseGuestReportData(DailyInHouseGuestVm vm)
    {
        vm.StrReportDate = (string.IsNullOrEmpty(vm.StrReportDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrReportDate;
        var reportDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrReportDate)).ToString("dd/MMM/yyyy");

        string query = $@"select br.Id BookingRoomId, bs.Id BookingId,bs.BookingNo,bs.BookingDate,br.TotalGuest,br.ComplementaryId,hc.Title ComplementaryName,ri.Id RoomId,ri.RoomNo BookingRoomNo,rc.Id CategoryId,rc.CategoryName
        ,gi.Id GuestId,isnull(gi.Salutation,'') + ' '+ isnull(gi.FirstName,'') + ' '+isnull(gi.LastName,'') GuestName,bs.BookingStatus,gi.Mobile GuestMobile,case when bs.BookingStatus = 1 then 'Booked' when bs.BookingStatus = 2 then 'CheckIn' when 
        bs.BookingStatus = 3 then 'CheckOut' when bs.BookingStatus = 4 then 'NoShow' when bs.BookingStatus = 9 then 'Canceled' else convert(varchar,bs.BookingStatus) end BookingStatusText,bs.BookingType
        ,br.ActualCheckInTime CheckInTime,case when br.ActualCheckOutTime is null then br.CheckOutTime else br.ActualCheckOutTime end CheckOutTime
        from HtBookingServices bs
        inner join HtBookingRooms br on br.BookingId = bs.Id
        inner join HtBookingGuests bg on bg.BookingId = bs.Id
        inner join HtGuestInfos gi on gi.Id = bg.GuestId
        inner join HtRoomInfos ri on ri.Id = br.RoomId
        inner join HtRoomCategories rc on rc.Id = ri.RoomCategoryId
        left join HtComplementaries hc on hc.id = br.ComplementaryId 
        where br.ActualCheckInTime is not null 
        and convert(date,br.ActualCheckInTime) <= '{reportDate}' and (br.ActualCheckOutTime is null  or convert(date,ActualCheckOutTime) > '{reportDate}')
        order by ri.RoomNo ";

        var data = await _iReadDbConnection.QueryAsync<RoomWiseGuestVm>(query);
        return data.ToList();
    }
    public async Task<DailyInHouseGuestVm> DailyInHouseRoomStatusReportData(DailyInHouseGuestVm vm)
    {
        vm.StrReportDate = (string.IsNullOrEmpty(vm.StrReportDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrReportDate;
        var reportDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrReportDate)).ToString("dd/MMM/yyyy");

        string query = $@"select isnull(sum(case when br.BookingDayStatus = 1 then 1 else 0 end),0) HalfDayUse
        ,isnull(sum(case when br.BookingDayStatus = 2 and convert(date,br.ActualCheckInTime) = convert(date,br.ActualCheckOutTime) then 1 else 0 end),0) DayUse
        ,isnull(sum(case when br.BookingDayStatus = 2 and convert(date,br.ActualCheckInTime) != convert(date,br.ActualCheckOutTime) then 1 else 0 end),0) FullDayUse
        ,STUFF((
        select distinct ',' + cast(ri1.RoomNo as varchar(max))
        from HtBookingServices bs1
        inner join HtBookingRooms br1 on br1.BookingId = bs1.Id
        inner join HtRoomInfos ri1 on ri1.Id = br1.RoomId
        where br1.BookingDayStatus = 1 AND convert(date,br1.ActualCheckOutTime) = '{reportDate}'
        for xml path(''), type
        ).value('.', 'nvarchar(max)'), 1, 1, '') AS HalfDayRoomNo
        ,STUFF((
        select distinct ',' + cast(ri1.RoomNo as varchar(max))
        from HtBookingServices bs1
        inner join HtBookingRooms br1 on br1.BookingId = bs1.Id
        inner join HtRoomInfos ri1 on ri1.Id = br1.RoomId
        where br1.BookingDayStatus = 2  and convert(date,br1.ActualCheckInTime) = convert(date,br1.ActualCheckOutTime) and convert(date,br1.ActualCheckOutTime) = '{reportDate}'
        for xml path(''), type
        ).value('.', 'nvarchar(max)'), 1, 1, '') AS DayUseRoomNo
        ,STUFF((
        select distinct ',' + cast(ri1.RoomNo as varchar(max))
        from HtBookingServices bs1
        inner join HtBookingRooms br1 on br1.BookingId = bs1.Id
        inner join HtRoomInfos ri1 on ri1.Id = br1.RoomId
        where br1.BookingDayStatus = 2  and convert(date,br1.ActualCheckInTime) != convert(date,br1.ActualCheckOutTime) and convert(date,br1.ActualCheckOutTime) = '{reportDate}'
        for xml path(''), type
        ).value('.', 'nvarchar(max)'), 1, 1, '') AS FullDayUseRoomNo
        from HtBookingServices bs
        inner join HtBookingRooms br on br.BookingId = bs.Id
        inner join HtBookingGuests bg on bg.BookingId = bs.Id --and bg.IsMain = 'true'
        inner join HtGuestInfos gi on gi.Id = bg.GuestId
        inner join HtRoomInfos ri on ri.Id = br.RoomId
        inner join HtRoomCategories rc on rc.Id = ri.RoomCategoryId
        where br.BookingDayStatus in (1,2) and convert(date,br.ActualCheckOutTime) = '{reportDate}'";

        var data = await _iReadDbConnection.QueryAsync<DailyInHouseGuestVm>(query);
        return data.FirstOrDefault();
    }
    public async Task<DailyInHouseGuestVm> DailyInHouseNoShowRoomReportData(DailyInHouseGuestVm vm, DailyInHouseGuestVm data)
    {
        vm.StrReportDate = (string.IsNullOrEmpty(vm.StrReportDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrReportDate;
        var reportDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrReportDate)).ToString("dd/MMM/yyyy");

        string query = $@"select count(br.RoomId) NoShowRoom,(string_agg(cast(ri.RoomNo as nvarchar(max)), ', ') within group (order by ri.RoomNo)) NoShowRoomNo
        from HtBookingServices bs
        inner join HtBookingRooms br on br.BookingId = bs.Id
        inner join HtRoomInfos ri on ri.Id = br.RoomId
        where bs.BookingStatus = {(int)BookingServiceStatusEnum.NoShow} and convert(date,br.CheckInTime) = '{reportDate}'";

        var dataNoShow = await _iReadDbConnection.QueryAsync<DailyInHouseGuestVm>(query);
        if (dataNoShow.FirstOrDefault() != null)
        {
            data.NoShowRoom = dataNoShow.FirstOrDefault().NoShowRoom;
            data.NoShowRoomNo = dataNoShow.FirstOrDefault().NoShowRoomNo;
        }
        return data;
    }
    public async Task<string> DailyInHouseGuestReportHtml(DailyInHouseGuestVm vm)
    {
        try
        {
            string fullHtml = "";
            DailyInHouseGuestVm data = new DailyInHouseGuestVm();
            data = await DailyInHouseRoomStatusReportData(vm);
            data = await DailyInHouseNoShowRoomReportData(vm, data);
            data.GuestList = await DailyInHouseGuestReportData(vm);
            data.CBF = data.GuestList.Where(x => x.ComplementaryId > 0).Sum(x => x.TotalGuest);

            var reportDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrReportDate));
            var todayCbf = await Context.HtDateBreakfasts.FirstOrDefaultAsync(x => x.BreakfastDate.Date == reportDate.Date && !x.IsDeleted);

            string todayCbfText = todayCbf != null ? $"{todayCbf.BreakfastAmount}" : "";

            // === Header Section ===
            if (vm.IsPirnt)
            {
                fullHtml += $@"<div style='text-align:center;margin-bottom:20px;'>
        
                                <p style='margin:4px 0;color:#555;font-size:14px;'>
                                    <b>Date: </b> {vm.StrReportDate}
                                </p>
                                <hr style='border:1px solid #ddd;width:80%;margin:10px auto;' />
                            </div>";
            }
            fullHtml += "<table class='table table-bordered report-table mb-3 w-100' id='print_table' style='width:100%; padding-bottom:10px; repeat-header:yes;' border='1'>";

            fullHtml += "<thead>";
            fullHtml += "<tr style='height:30px;'>";
            fullHtml += $@"<th style='width:5%;'>SL No</th>
                           <th style='width:10%; text-align:center;'>Room No</th>
                           <th style='width:25%; text-align:center;'>Name Of Guest</th>
                           <th style='width:5%; text-align:center;'>Pax</th>
                           <th style='width:15%; text-align:center;'>C/In Date</th>
                           <th style='width:15%; text-align:center;'>C/Out Date</th>
                           <th style='width:15%; text-align:center;'>Mobile</th>
                           <th style='width:10%; text-align:center;'>Remarks</th>";

            fullHtml += "</tr>";
            fullHtml += "</thead>";

            fullHtml += "<tbody>";

            if (data != null && data.GuestList.Count > 0)
            {
                for (int i = 0; i < data.GuestList.Count; i++)
                {
                    RoomWiseGuestVm objBooking = data.GuestList[i];

                    string rowRoomNo = !vm.IsPirnt ? $"<a target='_blank' href='../RoomInfo/Details/{objBooking.BookingRoomId}'>{objBooking.BookingRoomNo}</a>" : $"{objBooking.BookingRoomNo}";

                    string complementary = objBooking.ComplementaryId > 0 ? "CBF" : "WCBF";

                    fullHtml += "<tr>";

                    fullHtml += $@"<td style='text-align:left;'>{i + 1}</td>
                                <td style='text-align:center;padding:5px;'><b>{rowRoomNo}</b></td>
                                <td style='text-align:left;'>{objBooking.GuestName}</td>
                                <td style='text-align:center;'>{objBooking.TotalGuest}</td>
                                <td style='text-align:center;'>{DU.Utility.ConvertDateToStr(objBooking.CheckInTime)}</td>
                                <td style='text-align:center;'>{DU.Utility.ConvertDateToStr(objBooking.CheckOutTime)}</td>
                                <td style='text-align:center;'>{objBooking.GuestMobile}</td>
                                <td style='text-align:center;'>{complementary}</td>";

                    fullHtml += "</tr>";
                }

                fullHtml += $@"<tr>
                            <td colspan='3'></td>
                            <td style='text-align:center;'>{data.GuestList.Sum(x => x.TotalGuest)}</td>
                            <td colspan='4'></td>
                        </tr>";

                var complementaryTotal = data.GuestList.Where(x => x.ComplementaryId > 0).Sum(x => x.TotalGuest);

                string expectedCbfText = "";

                if (!vm.IsPirnt)
                {
                    expectedCbfText = $"/Expected = ({complementaryTotal})";
                }

                fullHtml += $@"<tr>
                            <td colspan='5' style='text-align:left;'><b>CBF = {todayCbfText} {expectedCbfText}</b></td>
                            <td colspan='2' style='text-align:right;'><b>Total Occupied</b></td>
                            <td><b>{data.GuestList.Count()}</b></td>
                        </tr>";

            }


            fullHtml += $@"<tr>
                            <td colspan='5' style='text-align:left;'><b>Half Day = {data.HalfDayRoomNo}</b></td>
                            <td colspan='2' style='text-align:right;'><b>Half Day</b></td>
                            <td><b>{data.HalfDayUse}</b></td>
                        </tr>";

            fullHtml += $@"<tr>
                            <td colspan='5' style='text-align:left;'><b>Day Use = {data.DayUseRoomNo}</b></td>
                            <td colspan='2' style='text-align:right;'><b>Day Use</b></td>
                            <td><b>{data.DayUse}</b></td>
                        </tr>";

            fullHtml += $@"<tr>
                            <td colspan='5' style='text-align:left;'><b>Full Day = {data.FullDayUseRoomNo}</b></td>
                            <td colspan='2' style='text-align:right;'><b>Full Day</b></td>
                            <td><b>{data.FullDayUse}</b></td>
                        </tr>";

            if (data.NoShowRoom > 0)
            {
                fullHtml += $@"<tr>
                            <td colspan='5' style='text-align:left;'><b>No Show = {data.NoShowRoomNo}</b></td>
                            <td colspan='2' style='text-align:right;'><b>No Show</b></td>
                            <td><b>{data.NoShowRoom}</b></td>
                        </tr>";
            }

            fullHtml += $@"<tr>
                            <td colspan='7' style='text-align:center;'><b>Total</b></td>
                            <td><b>{(data.GuestList.Count() + data.HalfDayUse + data.DayUse + data.FullDayUse + data.NoShowRoom)}</b></td>
                        </tr>";

            fullHtml += "</tbody>";

            fullHtml += "</table>";
            // === Footer Section ===
            fullHtml += @"<div class='report-footer'>
                            <hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
                            <p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>";

            fullHtml += @"</div>";

            if (!vm.IsPirnt)
            {
                if (todayCbf == null)
                {
                    fullHtml += "<a class='btn btn-info' href='#' data-bs-toggle='modal' data-bs-target='#cbfAmountModal'> Entry CBF </a>";
                }
            }

            return fullHtml;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    #endregion

    #region GuestDueReportHtml
    public async Task<List<GuestDueReportVm>> GetGuestDueReport(GuestDueReportVm vm)
    {
        vm.StrQueryDate = (string.IsNullOrEmpty(vm.StrQueryDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrQueryDate;
        var reportDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrQueryDate)).ToString("dd/MMM/yyyy");
        string dateQuery = (!string.IsNullOrEmpty(vm.StrFromDate)) && (!string.IsNullOrEmpty(vm.StrFromDate))
                    ? $" AND CONVERT(date, CASE WHEN bs.BookingType = 'R' THEN rr.CheckOutTime ELSE hr.CheckOutTime END ) BETWEEN '{Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)):dd/MMM/yyyy}' And '{Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)):dd/MMM/yyyy}'"
                    : "";

        string guestIdFilter = (vm.GuestId > 0) ? $" and g.Id = {vm.GuestId}" : "";
        string guestNameFilter = (!string.IsNullOrEmpty(vm.GuestName)) ? $"and ISNULL(g.Salutation,'') + ' ' + ISNULL(g.FirstName,'') + ' ' + ISNULL(g.LastName,'') like '%{vm.GuestName}%'" : "";
        string companyIdFilter = (vm.CompanyId > 0) ? $" and g.CompanyId = {vm.CompanyId}" : "";

        string isComplementary = $"AND ISNULL(bc.IsComplimentary,0) = 0";

        #region Masuk Sir Query
        //string query = $@"select g.BookingType,g.BookingNo,g.BookingId,convert(date,g.BookingDate)BookingDate,g.GuestId,g.GuestName,g.GuestMobile,max(G.TotalGuest) TotalGuest,sum(g.Rent) Rent,Sum(ExtraBedCharge) ExtraBedCharge
        //,max(Discount) Discount,max(FoodBill) FoodBill,max(Vat) Vat,max(Tax) Tax,max(ServiceCharge) ServiceCharge,max(ConfBill) ConfBill,max(g.ExtraCharge) ExtraCharge
        //,sum(isnull(g.NetRent,0)) + max(isnull(g.ExtraCharge,0)) NetRent,sum(isnull(g.NetRent,0)) + max(isnull(FoodBill,0)) + max(isnull(ConfBill,0)) - max(isnull(Discount,0)) +  max(isnull(ExtraCharge,0)) TotalBill
        //,max(isnull(g.NetReceive,0)) TotalReceive, sum(isnull(g.NetRent,0)) +  max(isnull(g.ExtraCharge,0)) + max(isnull(FoodBill,0)) + max(isnull(ConfBill,0)) - max(isnull(Discount,0)) - max(isnull(g.NetReceive,0)) DueAmount
        //from (

        //select 'Room' BookingType,bs.BookingNo,bs.Id BookingId,bs.BookingDate, g.Id GuestId,g.FirstName+' '+isnull(g.LastName,'') GuestName,rc.Id RoomCategoryId,rc.CategoryName,r.Id RoomId,r.RoomNo,hr.CheckInTime
        //,hr.ActualCheckOutTime CheckOutTime,hr.TotalGuest,hr.Rent RoomRent,(hr.Rent+hr.ServiceCharge+hr.Vat+hr.Tax) Rent,hr.ExtraBedCharge,bs.Discount,hr.NetRent
        //,ab.Amount AdvanceAmount,b.BillNumber,
        //case when datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) = 0 then 
        //case when hr.BookingDayStatus = 1 then 0.5  else 1 end 
        //else 
        //case when BookingDayStatus = 1 then datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) + 0.5 
        //when BookingDayStatus = 2 then datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) + 1 
        //else datediff(day,convert(date,hr.CheckInTime),convert(date,hr.ActualCheckOutTime)) end
        //end TotalStay,b.FoodBill,isnull(af.RefundAmount,0) AdvanceRefund,isnull(p.NetReceive,0) NetReceive,bs.Vat,bs.Tax
        //,bs.ServiceCharge,e.ExtraCharge,0 PreReceive ,af.RefundTranNo,b.IsComplimentary,b.CmpRemarks,e.ConfBill ,g.Mobile GuestMobile 
        //FROM HtBookingServices bs
        //inner join HtBookingRooms hr on hr.BookingId = bs.Id
        //inner join HtRoomInfos r on r.Id = hr.RoomId
        //inner join HtRoomCategories rc on rc.Id = r.RoomCategoryId
        //left join (
        //select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000001' then d.NetAmount else 0 end) RoomBill
        //,sum(case when s.ServiceCode = 'SR000002' then d.NetAmount else 0 end) FoodBill,d.IsComplimentary,b.CmpRemarks
        //from HtBillingDetails d
        //inner join HtBillings b on b.Id = d.BillId
        //inner join HtServices s on s.Id = d.ServiceId
        //where b.BookingId > 0 and s.ServiceCode in ('SR000001','SR000002') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId,d.IsComplimentary,b.CmpRemarks
        //) b on b.BookingId = bs.Id and b.BookingRoomid = hr.Id
        //left join (
        //select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000003' then d.NetAmount else 0 end) ExtraCharge
        //,sum(case when s.ServiceCode in ('SR000005','SR000006','SR000007')  then d.NetAmount else 0 end) ConfBill
        //from HtBillingDetails d
        //inner join HtBillings b on b.Id = d.BillId
        //inner join HtServices s on s.Id = d.ServiceId
        //where b.BookingId > 0 and s.ServiceCode in ( 'SR000003','SR000005','SR000006','SR000007') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        //) e on e.BookingId = bs.Id 
        //left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
        //left join HtGuestInfos g on g.Id= bg.GuestId
        //left join (select sum(PaidAmount) Amount,BookingId from HtBookingPayments where BillingId is null group by BookingId) ab on ab.BookingId = bs.Id
        //left join (select BookingId,sum(RefundAmount) RefundAmount,max(TransactionNo) RefundTranNo from HtAdvanceRefunds group by BookingId) af on af.BookingId = bs.Id
        //left join (select p.BookingId,sum(isnull(ReceiveAmount,0) - isnull(RefundAmount,0)) NetReceive
        //from (
        //select BookingId,sum(PaidAmount) ReceiveAmount,0 RefundAmount from HtBookingPayments group by BookingId
        //)p group by p.BookingId
        //)p on p.BookingId = bs.Id
        //where bs.IsDeleted = 0 and (select convert(date,max(ActualCheckOutTime)) from HtBookingRooms r where r.BookingId = bs.Id and r.IsDeleted = 0) >= '{reportDate}' -- >= '01/Sep/2024'
        //and bs.BookingStatus = 3 and bs.BookingType = 'R' 


        //union all

        //select 'Hall' BookingTypee,bs.BookingNo,bs.Id BookingId,bs.BookingDate, g.Id GuestId,g.FirstName+' '+isnull(g.LastName,'') GuestName,null RoomCategoryId,null CategoryName,h.Id RoomId,h.HallName RoomNo,hr.BookingDate CheckInTime
        //,hr.BookingDate CheckOutTime,bs.TotalGuest,hr.Rent RoomRent,(hr.Rent+hr.ServiceCharge+hr.Vat+hr.Tax) Rent,0 ExtraBedCharge,bs.Discount,hr.NetRent,ab.Amount AdvanceAmount,b.BillNumber,
        //0 TotalStay,b.FoodBill,isnull(af.RefundAmount,0) AdvanceRefund,isnull(p.NetReceive,0) NetReceive,bs.Vat,bs.Tax
        //,bs.ServiceCharge,0 ExtraCharge,0 PreReceive ,af.RefundTranNo,0 IsComplimentary,null CmpRemarks,0 ConfBill,g.Mobile GuestMobile
        //FROM HtBookingServices bs
        //inner join HtBookingHalls hr on hr.BookingId = bs.Id
        //inner join HtHallInfos h on h.Id = hr.HallId
        //left join (
        //select b.BookingId,b.id BillId,b.BillNumber,d.BookingRoomId,sum(case when s.ServiceCode = 'SR000004' then d.NetAmount else 0 end) RoomBill
        //,sum(case when s.ServiceCode = 'SR000002' then d.NetAmount else 0 end) FoodBill
        //from HtBillingDetails d
        //inner join HtBillings b on b.Id = d.BillId
        //inner join HtServices s on s.Id = d.ServiceId
        //where b.BookingId > 0 and s.ServiceCode in ('SR000004','SR000002') group by b.BookingId,b.id,b.BillNumber,d.BookingRoomId
        //) b on b.BookingId = bs.Id and b.BookingRoomid = hr.Id
        //left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
        //left join HtGuestInfos g on g.Id= bg.GuestId
        //left join (select sum(PaidAmount) Amount,BookingId from HtBookingPayments where BillingId is null group by BookingId) ab on ab.BookingId = bs.Id
        //left join (select BookingId,sum(RefundAmount) RefundAmount,max(TransactionNo) RefundTranNo from HtAdvanceRefunds group by BookingId) af on af.BookingId = bs.Id
        //left join (select p.BookingId,sum(isnull(ReceiveAmount,0) - isnull(RefundAmount,0)) NetReceive
        //from (
        //select BookingId,sum(PaidAmount) ReceiveAmount,0 RefundAmount from HtBookingPayments  group by BookingId
        //)p group by p.BookingId
        //)p on p.BookingId = bs.Id
        //where bs.IsDeleted = 0 and bs.BookingType = 'H' and convert(date,bs.CheckOutTime) >= '{reportDate}' and bs.BookingStatus = 3
        //) g where 1 = 1 {guestIdFilter} {guestNameFilter}
        //group by g.BookingId,convert(date,g.BookingDate),g.GuestId,g.GuestName,g.BookingType,g.BookingNo,g.GuestMobile
        //having sum(isnull(g.NetRent,0)) + max(isnull(FoodBill,0)) + max(isnull(ConfBill,0)) - max(isnull(Discount,0)) - max(isnull(g.NetReceive,0)) > 0
        //order by convert(date,g.BookingDate) desc";
        #endregion

        #region Tawkir Query Update V3

        //string query = $@"
        //                SELECT
        //                    g.BookingType,
        //                    g.BookingNo,
        //                    g.BookingId,
        //                    MAX(g.BillDate) AS BillDate,
        //                    CONVERT(date, g.BookingDate) AS BookingDate,
        //                    CONVERT(date, g.CheckInTime) AS CheckInTime,
        //                    CONVERT(date, g.CheckOutTime) AS CheckOutTime,
        //                    g.GuestId,
        //                    g.GuestName,
        //                    g.GuestMobile,
        //                    g.CompanyId,
        //                    CASE 
        //                        WHEN g.CompanyId IS NULL THEN 'General Guest'
        //                        ELSE g.CompanyName 
        //                    END AS CompanyName,

        //                    MAX(g.TotalGuest) AS TotalGuest,
        //                    SUM(g.Rent) AS Rent,
        //                    SUM(g.ExtraBedCharge) AS ExtraBedCharge,
        //                    MAX(Discount) AS Discount,
        //                    MAX(SpecialDiscount) AS SpecialDiscount,
        //                    MAX(FoodBill) AS FoodBill,
        //                    MAX(Vat) AS Vat,
        //                    MAX(Tax) AS Tax,
        //                    MAX(ServiceCharge) AS ServiceCharge,
        //                    MAX(ConfBill) AS ConfBill,
        //                    MAX(g.ExtraCharge) AS ExtraCharge,

        //                    SUM(ISNULL(g.NetRent, 0))
        //                        + MAX(ISNULL(g.ExtraCharge, 0)) AS NetRent,

        //                    SUM(ISNULL(g.Rent, 0))
        //                        + MAX(ISNULL(FoodBill, 0))
        //                        + MAX(ISNULL(ConfBill, 0))
        //                        + MAX(ISNULL(ExtraBedCharge, 0))
        //                        + MAX(ISNULL(ExtraCharge, 0))
        //                        - MAX(ISNULL(Discount, 0))
        //                        - MAX(ISNULL(SpecialDiscount, 0)) AS TotalBill,

        //                    MAX(ISNULL(g.NetReceive, 0)) AS TotalReceive,

        //                    SUM(ISNULL(g.Rent, 0))
        //                  + MAX(ISNULL(g.ExtraBedCharge, 0))
        //                        + MAX(ISNULL(g.ExtraCharge, 0))
        //                        + MAX(ISNULL(FoodBill, 0))
        //                        + MAX(ISNULL(ConfBill, 0))
        //                        - MAX(ISNULL(Discount, 0))
        //                        - MAX(ISNULL(SpecialDiscount, 0))
        //                        - MAX(ISNULL(g.NetReceive, 0)) AS DueAmount
        //                FROM
        //                (
        //                    SELECT
        //                        'Room' AS BookingType,
        //                        bs.BookingNo,
        //                        bs.Id AS BookingId,
        //                        bs.BookingDate,
        //                        g.Id AS GuestId,
        //                        ISNULL(g.Salutation,'') + ' '
        //                            + ISNULL(g.FirstName,'') + ' '
        //                            + ISNULL(g.LastName,'') AS GuestName,
        //                        cc.Id AS CompanyId,
        //                        cc.Name AS CompanyName,
        //                        rc.Id AS RoomCategoryId,
        //                        rc.CategoryName,
        //                        r.Id AS RoomId,
        //                        r.RoomNo,
        //                        bs.CheckInTime,
        //                        bs.CheckOutTime AS CheckOutTime,
        //                        hr.TotalGuest,
        //                        hr.Rent AS RoomRent,
        //                        (hr.Rent + hr.ServiceCharge + hr.Vat + hr.Tax) AS Rent,
        //                        (hr.ExtraBedCharge + eb.ExtraBedBill) AS ExtraBedCharge,
        //                        bs.Discount,
        //                        hr.NetRent,
        //                        ab.Amount AS AdvanceAmount,
        //                        b.BillNumber,
        //                        b.BillDate,
        //                        ISNULL(b.SpecialDiscount, 0) AS SpecialDiscount,

        //                        CASE 
        //                            WHEN DATEDIFF(day, CONVERT(date, hr.CheckInTime), CONVERT(date, hr.ActualCheckOutTime)) = 0 THEN
        //                                CASE WHEN hr.BookingDayStatus = 1 THEN 0.5 ELSE 1 END
        //                            ELSE
        //                                CASE 
        //                                    WHEN BookingDayStatus = 1 THEN DATEDIFF(day, CONVERT(date, hr.CheckInTime), CONVERT(date, hr.ActualCheckOutTime)) + 0.5
        //                                    WHEN BookingDayStatus = 2 THEN DATEDIFF(day, CONVERT(date, hr.CheckInTime), CONVERT(date, hr.ActualCheckOutTime)) + 1
        //                                    ELSE DATEDIFF(day, CONVERT(date, hr.CheckInTime), CONVERT(date, hr.ActualCheckOutTime))
        //                                END
        //                        END AS TotalStay,

        //                        b.FoodBill,
        //                        ISNULL(af.RefundAmount, 0) AS AdvanceRefund,
        //                        ISNULL(p.NetReceive, 0) AS NetReceive,
        //                        bs.Vat,
        //                        bs.Tax,
        //                        bs.ServiceCharge,
        //                        e.ExtraCharge,
        //                        0 AS PreReceive,
        //                        af.RefundTranNo,
        //                        b.IsComplimentary,
        //                        b.CmpRemarks,
        //                        e.ConfBill,
        //                        g.Mobile AS GuestMobile
        //                    FROM HtBookingServices bs
        //                    INNER JOIN HtBookingRooms hr ON hr.BookingId = bs.Id
        //                    INNER JOIN HtRoomInfos r ON r.Id = hr.RoomId
        //                    INNER JOIN HtRoomCategories rc ON rc.Id = r.RoomCategoryId

        //                    LEFT JOIN (
        //                        SELECT
        //                            b.BookingId,
        //                            b.Id AS BillId,
        //                            b.BillNumber,
        //                            MAX(b.BillDate) AS BillDate,
        //                            d.BookingRoomId,
        //                            SUM(CASE WHEN s.ServiceCode = 'SR000001' THEN d.NetAmount ELSE 0 END) AS RoomBill,
        //                            SUM(CASE WHEN s.ServiceCode = 'SR000002' THEN d.NetAmount ELSE 0 END) AS FoodBill,
        //                            MAX(ISNULL(b.SpecialDiscount, 0)) AS SpecialDiscount,
        //                            d.IsComplimentary,
        //                            b.CmpRemarks
        //                        FROM HtBillingDetails d
        //                        INNER JOIN HtBillings b ON b.Id = d.BillId
        //                        INNER JOIN HtServices s ON s.Id = d.ServiceId
        //                        WHERE b.BookingId > 0
        //                          AND s.ServiceCode IN ('SR000001','SR000002')
        //                        GROUP BY
        //                            b.BookingId,
        //                            b.Id,
        //                            b.BillNumber,
        //                            d.BookingRoomId,
        //                            d.IsComplimentary,
        //                            b.CmpRemarks
        //                    ) b ON b.BookingId = bs.Id
        //                       AND b.BookingRoomId = hr.Id

        //                    LEFT JOIN (
        //                        SELECT
        //                            b.BookingId,
        //                            b.Id AS BillId,
        //                            b.BillNumber,
        //                            MAX(b.BillDate) AS BillDate,
        //                            d.BookingRoomId,
        //                            SUM(CASE WHEN s.ServiceCode = 'SR000003' THEN d.NetAmount ELSE 0 END) AS ExtraCharge,
        //                            SUM(CASE WHEN s.ServiceCode IN ('SR000005','SR000006','SR000007') THEN d.NetAmount ELSE 0 END) AS ConfBill
        //                        FROM HtBillingDetails d
        //                        INNER JOIN HtBillings b ON b.Id = d.BillId
        //                        INNER JOIN HtServices s ON s.Id = d.ServiceId
        //                        WHERE b.BookingId > 0
        //                          AND s.ServiceCode IN ('SR000003','SR000005','SR000006','SR000007')
        //                        GROUP BY
        //                            b.BookingId,
        //                            b.Id,
        //                            b.BillNumber,
        //                            d.BookingRoomId
        //                    ) e ON e.BookingId = bs.Id

        //                 LEFT JOIN (
        //                  SELECT
        //                   b.BookingId,
        //                   d.BookingRoomId,
        //                   SUM(d.NetAmount) AS ExtraBedBill
        //                  FROM HtBillingDetails d
        //                  INNER JOIN HtBillings b ON b.Id = d.BillId
        //                  INNER JOIN HtServices s ON s.Id = d.ServiceId
        //                  WHERE
        //                   b.BookingId > 0
        //                   AND s.ServiceCode = 'SR000009'   -- ✅ Extra Bed Service
        //                  GROUP BY
        //                   b.BookingId,
        //                   d.BookingRoomId
        //                 ) eb ON eb.BookingId = bs.Id
        //                   AND eb.BookingRoomId = hr.Id


        //                    LEFT JOIN (
        //                        SELECT MAX(GuestId) GuestId, BookingId
        //                        FROM HtBookingGuests
        //                        WHERE IsMain = 1
        //                        GROUP BY BookingId
        //                    ) bg ON bg.BookingId = bs.Id

        //                    LEFT JOIN HtGuestInfos g ON g.Id = bg.GuestId
        //                    LEFT JOIN ClientCompanies cc ON cc.Id = g.CompanyId

        //                    LEFT JOIN (
        //                        SELECT SUM(PaidAmount) Amount, BookingId
        //                        FROM HtBookingPayments
        //                        WHERE BillingId IS NULL
        //                        GROUP BY BookingId
        //                    ) ab ON ab.BookingId = bs.Id

        //                    LEFT JOIN (
        //                        SELECT BookingId, SUM(RefundAmount) RefundAmount, MAX(TransactionNo) RefundTranNo
        //                        FROM HtAdvanceRefunds
        //                        GROUP BY BookingId
        //                    ) af ON af.BookingId = bs.Id

        //                    LEFT JOIN (
        //                        SELECT p.BookingId,
        //                               SUM(ISNULL(ReceiveAmount,0) - ISNULL(RefundAmount,0)) NetReceive
        //                        FROM (
        //                            SELECT BookingId, SUM(PaidAmount) ReceiveAmount, 0 RefundAmount
        //                            FROM HtBookingPayments
        //                            GROUP BY BookingId
        //                        ) p
        //                        GROUP BY p.BookingId
        //                    ) p ON p.BookingId = bs.Id

        //                    WHERE
        //                        bs.IsDeleted = 0
        //                        AND CONVERT(date, bs.BookingDate) >= '05/Nov/2024'
        //                        AND (
        //                            SELECT CONVERT(date, MAX(ActualCheckOutTime))
        //                            FROM HtBookingRooms r
        //                            WHERE r.BookingId = bs.Id
        //                              AND r.IsDeleted = 0
        //                        ) >= '01/Sep/2024'
        //                        AND bs.BookingStatus = 3
        //                        AND bs.BookingType = 'R'

        //                    /* ================= HALL BOOKINGS ================= */
        //                    UNION ALL

        //                    SELECT
        //                        'Hall' AS BookingTypee,
        //                        bs.BookingNo,
        //                        bs.Id AS BookingId,
        //                        bs.BookingDate,
        //                        g.Id AS GuestId,
        //                        ISNULL(g.Salutation,'') + ' '
        //                            + ISNULL(g.FirstName,'') + ' '
        //                            + ISNULL(g.LastName,'') AS GuestName,
        //                        cc.Id AS CompanyId,
        //                        cc.Name AS CompanyName,
        //                        NULL AS RoomCategoryId,
        //                        NULL AS CategoryName,
        //                        h.Id AS RoomId,
        //                        h.HallName AS RoomNo,
        //                        bs.CheckInTime,
        //                        bs.CheckOutTime,
        //                        bs.TotalGuest,
        //                        hr.Rent AS RoomRent,
        //                        (hr.Rent + hr.ServiceCharge + hr.Vat + hr.Tax) AS Rent,
        //                        0 AS ExtraBedCharge,
        //                        bs.Discount,
        //                        hr.NetRent,
        //                        ab.Amount AS AdvanceAmount,
        //                        b.BillNumber,
        //                        b.BillDate,
        //                        ISNULL(b.SpecialDiscount, 0) AS SpecialDiscount,
        //                        0 AS TotalStay,
        //                        b.FoodBill,
        //                        ISNULL(af.RefundAmount, 0) AS AdvanceRefund,
        //                        ISNULL(p.NetReceive, 0) AS NetReceive,
        //                        bs.Vat,
        //                        bs.Tax,
        //                        bs.ServiceCharge,
        //                        0 AS ExtraCharge,
        //                        0 AS PreReceive,
        //                        af.RefundTranNo,
        //                        0 AS IsComplimentary,
        //                        NULL AS CmpRemarks,
        //                        0 AS ConfBill,
        //                        g.Mobile AS GuestMobile
        //                    FROM HtBookingServices bs
        //                    INNER JOIN HtBookingHalls hr ON hr.BookingId = bs.Id
        //                    INNER JOIN HtHallInfos h     ON h.Id = hr.HallId

        //                    LEFT JOIN (
        //                        SELECT
        //                            b.BookingId,
        //                            b.Id AS BillId,
        //                            b.BillNumber,
        //                            MAX(b.BillDate) AS BillDate,
        //                            d.BookingRoomId,
        //                            MAX(ISNULL(b.SpecialDiscount,0)) AS SpecialDiscount,
        //                            SUM(CASE WHEN s.ServiceCode = 'SR000004' THEN d.NetAmount ELSE 0 END) AS RoomBill,
        //                            SUM(CASE WHEN s.ServiceCode = 'SR000002' THEN d.NetAmount ELSE 0 END) AS FoodBill
        //                        FROM HtBillingDetails d
        //                        INNER JOIN HtBillings b ON b.Id = d.BillId
        //                        INNER JOIN HtServices s ON s.Id = d.ServiceId
        //                        WHERE b.BookingId > 0
        //                          AND s.ServiceCode IN ('SR000004','SR000002')
        //                        GROUP BY
        //                            b.BookingId,
        //                            b.Id,
        //                            b.BillNumber,
        //                            d.BookingRoomId
        //                    ) b ON b.BookingId = bs.Id
        //                       AND b.BookingRoomId = hr.Id

        //                    LEFT JOIN (
        //                        SELECT MAX(GuestId) GuestId, BookingId
        //                        FROM HtBookingGuests
        //                        WHERE IsMain = 1
        //                        GROUP BY BookingId
        //                    ) bg ON bg.BookingId = bs.Id

        //                    LEFT JOIN HtGuestInfos g ON g.Id = bg.GuestId
        //                    LEFT JOIN ClientCompanies cc ON cc.Id = g.CompanyId

        //                    LEFT JOIN (
        //                        SELECT SUM(PaidAmount) Amount, BookingId
        //                        FROM HtBookingPayments
        //                        WHERE BillingId IS NULL
        //                        GROUP BY BookingId
        //                    ) ab ON ab.BookingId = bs.Id

        //                    LEFT JOIN (
        //                        SELECT BookingId, SUM(RefundAmount) RefundAmount, MAX(TransactionNo) RefundTranNo
        //                        FROM HtAdvanceRefunds
        //                        GROUP BY BookingId
        //                    ) af ON af.BookingId = bs.Id

        //                    LEFT JOIN (
        //                        SELECT p.BookingId,
        //                               SUM(ISNULL(ReceiveAmount,0) - ISNULL(RefundAmount,0)) NetReceive
        //                        FROM (
        //                            SELECT BookingId, SUM(PaidAmount) ReceiveAmount, 0 RefundAmount
        //                            FROM HtBookingPayments
        //                            GROUP BY BookingId
        //                        ) p
        //                        GROUP BY p.BookingId
        //                    ) p ON p.BookingId = bs.Id

        //                    WHERE
        //                        bs.IsDeleted = 0
        //                        AND bs.BookingType = 'H'
        //                        AND CONVERT(date, bs.CheckOutTime) >= '05/Nov/2024'
        //                        AND bs.BookingStatus = 3
        //                ) g
        //                WHERE 1 = 1 {guestIdFilter} {guestNameFilter} {companyIdFilter} {fromDateQuery} {toDateQuery} {isComplementary}
        //                GROUP BY
        //                    g.BookingId,
        //                    CONVERT(date, g.BookingDate),
        //                    CONVERT(date, g.CheckInTime),
        //                    CONVERT(date, g.CheckOutTime),
        //                    g.GuestId,
        //                    g.GuestName,
        //                    g.CompanyId,
        //                    g.CompanyName,
        //                    g.BookingType,
        //                    g.BookingNo,
        //                    g.GuestMobile
        //                HAVING
        //                    SUM(ISNULL(g.Rent, 0))
        //                        + MAX(ISNULL(ExtraBedCharge, 0))
        //                        + MAX(ISNULL(ExtraCharge, 0))
        //                        + MAX(ISNULL(FoodBill, 0))
        //                        + MAX(ISNULL(ConfBill, 0))
        //                        - MAX(ISNULL(Discount, 0))
        //                        - MAX(ISNULL(SpecialDiscount, 0))
        //                        - MAX(ISNULL(g.NetReceive, 0)) > 0
        //                ORDER BY
        //                    CONVERT(date, g.BookingDate) DESC;";

        #endregion

        #region Tawkir Query Update V4
        string query = $@";WITH
                                RoomRentCTE AS (
                                    SELECT
                                        hr.BookingId,
                                        SUM(hr.Rent + hr.ServiceCharge + hr.Vat + hr.Tax) AS RoomRent,
                                        SUM(ISNULL(hr.ExtraBedCharge,0)) AS ExtraBedCharge,
                                        SUM(ISNULL(hr.NetRent,0)) AS NetRent,
                                        MAX(hr.TotalGuest) AS TotalGuest,
                                        MAX(hr.CheckInTime) AS CheckInTime,
                                        MAX(hr.ActualCheckOutTime) AS CheckOutTime
                                    FROM HtBookingRooms hr
                                    WHERE hr.IsDeleted = 0
                                    GROUP BY hr.BookingId
                                ),
                                HallRentCTE AS (
                                    SELECT
                                        hh.BookingId,
                                        SUM(hh.Rent + hh.ServiceCharge + hh.Vat + hh.Tax) AS HallRent,
                                        MAX(bs.TotalGuest) AS TotalGuest,
                                        MAX(bs.CheckInTime) AS CheckInTime,
                                        MAX(bs.CheckOutTime) AS CheckOutTime
                                    FROM HtBookingHalls hh
                                    INNER JOIN HtBookingServices bs ON bs.Id = hh.BookingId
                                    GROUP BY hh.BookingId
                                ),
                                BillingCTE AS (
                                    SELECT
                                        b.BookingId,
                                        MAX(b.BillDate) AS BillDate,
                                        SUM(CASE WHEN s.ServiceCode = 'SR000001' THEN d.NetAmount ELSE 0 END) AS RoomBill,
                                        SUM(CASE WHEN s.ServiceCode = 'SR000004' THEN d.NetAmount ELSE 0 END) AS HallBill,
                                        SUM(CASE WHEN s.ServiceCode = 'SR000002' THEN d.NetAmount ELSE 0 END) AS FoodBill,
                                        SUM(CASE WHEN s.ServiceCode = 'SR000003' THEN d.NetAmount ELSE 0 END) AS ExtraCharge,
                                        SUM(CASE WHEN s.ServiceCode IN ('SR000005','SR000006','SR000007') THEN d.NetAmount ELSE 0 END) AS ConfBill,
                                        SUM(CASE WHEN s.ServiceCode = 'SR000009' THEN d.NetAmount ELSE 0 END) AS ExtraBedBill,
                                        MAX(ISNULL(b.SpecialDiscount,0)) AS SpecialDiscount,
                                        MAX(CAST(d.IsComplimentary AS INT)) AS IsComplimentary
                                    FROM HtBillingDetails d
                                    INNER JOIN HtBillings b ON b.Id = d.BillId
                                    INNER JOIN HtServices s ON s.Id = d.ServiceId
                                    WHERE b.BookingId > 0
                                    GROUP BY b.BookingId
                                ),
                                PaymentCTE AS (
                                    SELECT BookingId, SUM(PaidAmount) AS NetReceive
                                    FROM HtBookingPayments
                                    GROUP BY BookingId
                                ),
                                MainGuestCTE AS (
                                    SELECT BookingId, MAX(GuestId) AS GuestId
                                    FROM HtBookingGuests
                                    WHERE IsMain = 1
                                    GROUP BY BookingId
                                )

                                SELECT *
                                FROM (
                                    SELECT
                                        bs.BookingType,
                                        bs.BookingNo,
                                        bs.Id AS BookingId,
                                        bs.BookingDate,

                                        CASE WHEN bs.BookingType = 'R' THEN rr.CheckInTime ELSE hr.CheckInTime END AS CheckInTime,
                                        CASE WHEN bs.BookingType = 'R' THEN rr.CheckOutTime ELSE hr.CheckOutTime END AS CheckOutTime,

                                        g.Id AS GuestId,
                                        g.Mobile AS GuestMobile,
                                        ISNULL(g.Salutation,'') + ' ' +
                                        ISNULL(g.FirstName,'') + ' ' +
                                        ISNULL(g.LastName,'') AS GuestName,

                                        cc.Id AS CompanyId,
                                        ISNULL(cc.Name,'General Guest') AS CompanyName,

                                        CASE WHEN bs.BookingType = 'R' THEN rr.TotalGuest ELSE hr.TotalGuest END AS TotalGuest,

                                        CASE WHEN bs.BookingType = 'R' THEN rr.RoomRent ELSE hr.HallRent END AS Rent,

                                        ISNULL(rr.ExtraBedCharge,0) + ISNULL(bc.ExtraBedBill,0) AS ExtraBedCharge,

                                        bs.Discount,
                                        bc.SpecialDiscount,
                                        bc.FoodBill,
                                        bc.ConfBill,
                                        bc.ExtraCharge,

                                        ISNULL(rr.NetRent,0) AS NetRent,

                                        (
                                            CASE WHEN bs.BookingType = 'R' THEN rr.RoomRent ELSE hr.HallRent END
                                            + ISNULL(bc.FoodBill,0)
                                            + ISNULL(bc.ConfBill,0)
                                            + ISNULL(bc.ExtraCharge,0)
                                            + ISNULL(bc.ExtraBedBill,0)
                                            - ISNULL(bs.Discount,0)
                                            - ISNULL(bc.SpecialDiscount,0)
                                        ) AS TotalBill,

                                        ISNULL(p.NetReceive,0) AS TotalReceive,

                                        (
                                            CASE WHEN bs.BookingType = 'R' THEN rr.RoomRent ELSE hr.HallRent END
                                            + ISNULL(bc.FoodBill,0)
                                            + ISNULL(bc.ConfBill,0)
                                            + ISNULL(bc.ExtraCharge,0)
                                            + ISNULL(bc.ExtraBedBill,0)
                                            - ISNULL(bs.Discount,0)
                                            - ISNULL(bc.SpecialDiscount,0)
                                            - ISNULL(p.NetReceive,0)
                                        ) AS DueAmount

                                    FROM HtBookingServices bs
                                    LEFT JOIN RoomRentCTE rr ON rr.BookingId = bs.Id
                                    LEFT JOIN HallRentCTE hr ON hr.BookingId = bs.Id
                                    LEFT JOIN BillingCTE bc ON bc.BookingId = bs.Id
                                    LEFT JOIN PaymentCTE p ON p.BookingId = bs.Id
                                    LEFT JOIN MainGuestCTE mg ON mg.BookingId = bs.Id
                                    LEFT JOIN HtGuestInfos g ON g.Id = mg.GuestId
                                    LEFT JOIN ClientCompanies cc ON cc.Id = g.CompanyId

                                    WHERE
                                        bs.IsDeleted = 0
                                        AND bs.BookingStatus = 3
                                        AND CONVERT(date, bs.BookingDate) >= '05/Nov/2024'
                                        AND ISNULL(bc.IsComplimentary,0) = 0
                                        {dateQuery} {guestIdFilter} {guestNameFilter} {companyIdFilter} {isComplementary}
                                ) x
                                WHERE x.DueAmount > 0
                                ORDER BY x.BookingDate DESC;";
        #endregion

        var data = await _iReadDbConnection.QueryAsync<GuestDueReportVm>(query);
        return data.ToList();
    }
    public async Task<string> GuestDueReportHtml(GuestDueReportVm vm, bool isPrint)
    {
        try
        {
            string fullHtml = "";
            List<GuestDueReportVm> objDataList = await GetGuestDueReport(vm);

            if (objDataList.Count > 0)
            {
                if (vm.ReporetGroupBy == "G")//GUEST WISE
                {
                    fullHtml += "<table class='table table-bordered report-table' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
                    fullHtml += "<thead>";

                    //fullHtml += $@"<tr style='height:30px;'><td colspan='17' class='text-center'>Date: {DateTime.Today:dd/MMM/yyyy}</td></tr>";
                    if (vm.StrFromDate != null && vm.StrToDate != null)
                    {
                        fullHtml += $@"<tr style='height:30px;'><td colspan='17' class='text-center'><b>Report Date: </b> {vm.StrFromDate} <b> to </b> {vm.StrToDate} </td></tr>";
                    }
                    else
                    {
                        fullHtml += $@"<tr style='height:30px;'><td colspan='17' class='text-center'> <b>Report Date: </b> {DateTime.Today.ToString("dd/MM/yyyy")} </td></tr>";
                    }

                    fullHtml += "<tr style='height:30px;' class='text-center'>";

                    fullHtml += $@"<th style='width:50px;' >SL</th>
                            <th style='width:50px;'> Type</th>
                            <th style='width:100px;'> Booking No</th>
                            <th style='width:100px;'> Check-In Date</th>
                            <th style='width:100px;'> Check-Out Date</th>
                            <th style='width:140px;'> Guest Name</th>
                            <th style='width:90px;'> Guest Mobile</th>
                            <th style='width:100px;'> Rent</th>
                            <th style='width:90px;'> Ex. Bed Charge</th>
                            <th style='width:90px;'> Ex. Charge</th>
                            <th style='width:90px;'> Food Bill</th>
                            <th style='width:100px;'> Conf. Bill</th>
                            <th style='width:100px;'> Net Bill</th>
                            <th style='width:90px;'> Discount</th>
                            <th style='width:100px;'> Total Bill</th>
                            <th style='width:100px;'> Paid Amount</th>
                            <th style='width:100px;'> Due Amount</th>";

                    fullHtml += "</tr>";

                    fullHtml += "</thead>";
                    fullHtml += "<tbody>";

                    for (int i = 0; i < objDataList.Count; i++)
                    {
                        GuestDueReportVm objDue = objDataList[i];
                        string bookingNo = !vm.IsPirnt ? $"<a target='_blank' href='../BookingService/Details/{objDue.BookingId}'>{objDue.BookingNo}</a>" : $"{objDue.BookingNo}";

                        fullHtml += "<tr>";

                        var totalDiscount = objDue.Discount + objDue.SpecialDiscount;

                        fullHtml += $@"<td style='text-align:left;'>{i + 1}</td>";
                        fullHtml += $@"<td style='text-align:center;'><b>{objDue.BookingType}</b></td>                              
                                <td style='text-align:center;'>{bookingNo}</td>
                                <td style='text-align:center;'>{objDue.CheckInTime:dd/MMM/yy}</td>
                                <td style='text-align:center;'>{objDue.CheckOutTime:dd/MMM/yy}</td>
                                <td style='text-align:left;'>{objDue.GuestName}</td>
                                <td style='text-align:center;'>{objDue.GuestMobile}</td>
                                <td style='text-align:center;'>{objDue.Rent:F2}</td>
                                <td style='text-align:center;'>{objDue.ExtraBedCharge:F2}</td>
                                <td style='text-align:center;'>{objDue.ExtraCharge:F2}</td>
                                <td style='text-align:center;'>{objDue.FoodBill:F2}</td>
                                <td style='text-align:center;'>{objDue.ConfBill:F2}</td>
                                <td style='text-align:center;'>{objDue.NetRent:F2}</td>
                                <td style='text-align:center;'>{totalDiscount:F2}</td>
                                <td style='text-align:center;'>{objDue.TotalBill:F2}</td>
                                <td style='text-align:center;'>{objDue.TotalReceive:F2}</td>
                                <td style='text-align:center;'>{objDue.DueAmount:F2}</td>";
                        fullHtml += "</tr>";

                    }

                    fullHtml += "</tbody>";

                    fullHtml += "<tfoot>";
                    fullHtml += $@"<tr><td colspan='7' style='text-align:right;'><b>TOTAL</b></td>                              
                                
                                <td style='text-align:center;'>{objDataList.Sum(o => o.Rent):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.ExtraBedCharge):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.ExtraCharge):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.FoodBill):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.ConfBill):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.NetRent):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.Discount + o.SpecialDiscount):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.TotalBill):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.TotalReceive):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.DueAmount):F2}</td>";
                    fullHtml += "</tr>";
                    fullHtml += "</tfoot>";


                    fullHtml += "</table>";
                }
                else
                {
                    //COMPANY WISE

                    #region Header Part
                    fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
                    fullHtml += "<thead>";

                    fullHtml += $@"<tr style='height:30px;'><td colspan='16' class='text-center'>Date: {DateTime.Today:dd/MMM/yyyy}</td></tr>";

                    fullHtml += "<tr style='height:30px;' class='text-center'>";

                    fullHtml += $@"<th style='width:50px;' >SL</th>
                            <th style='width:50px;'> Type</th>
                            <th style='width:100px;'> Booking No</th>
                            <th style='width:80px;'> Booking Date</th>
                            <th style='width:140px;'> Guest Name</th>
                            <th style='width:90px;'> Guest Mobile</th>
                            
                            <th style='width:100px;'> Rent</th>
                            <th style='width:90px;'> Ex. Bed Charge</th>
                            <th style='width:90px;'> Ex. Charge</th>
                            <th style='width:90px;'> Food Bill</th>
                            <th style='width:100px;'> Conf. Bill</th>
                            <th style='width:100px;'> Net Bill</th>
                            <th style='width:90px;'> Discount</th>
                            <th style='width:100px;'> Total Bill</th>
                            <th style='width:100px;'> Paid Amount</th>
                            <th style='width:100px;'> Due Amount</th>";

                    fullHtml += "</tr>";
                    fullHtml += "</thead>";
                    #endregion
                    fullHtml += "<tbody>";

                    var companyList = objDataList.Select(o => new { o.CompanyId, o.CompanyName }).Distinct().ToList();
                    int sl = 1;
                    foreach (var company in companyList)
                    {
                        List<GuestDueReportVm> companyWiseDataList = objDataList.Where(o => o.CompanyId == company.CompanyId).ToList();

                        fullHtml += $@"<tr style='height:25px;font-weight:bold;text-align:center;font-size:14px;'><td colspan='16'>{company.CompanyName}</td></tr>";

                        for (int i = 0; i < companyWiseDataList.Count; i++)
                        {
                            GuestDueReportVm objDue = companyWiseDataList[i];
                            string bookingNo = !vm.IsPirnt ? $"<a target='_blank' href='../BookingService/Details/{objDue.BookingId}'>{objDue.BookingNo}</a>" : $"{objDue.BookingNo}";

                            var totalDiscount = objDue.Discount + objDue.SpecialDiscount;

                            fullHtml += "<tr>";
                            fullHtml += $@"<td style='text-align:left;'>{i + 1}</td>";
                            fullHtml += $@"<td style='text-align:center;'><b>{objDue.BookingType}</b></td>                              
                                <td style='text-align:center;'>{bookingNo}</td>
                                <td style='text-align:center;'>{objDue.BookingDate:dd/MMM/yy}</td>
                                <td style='text-align:center;'>{objDue.GuestName}</td>
                                <td style='text-align:center;'>{objDue.GuestMobile}</td>
                                
                                <td style='text-align:center;'>{objDue.Rent:F2}</td>
                                <td style='text-align:center;'>{objDue.ExtraBedCharge:F2}</td>
                                <td style='text-align:center;'>{objDue.ExtraCharge:F2}</td>
                                <td style='text-align:center;'>{objDue.FoodBill:F2}</td>
                                <td style='text-align:center;'>{objDue.ConfBill:F2}</td>
                                <td style='text-align:center;'>{objDue.NetRent:F2}</td>
                                <td style='text-align:center;'>{totalDiscount:F2}</td>
                                <td style='text-align:center;'>{objDue.TotalBill:F2}</td>
                                <td style='text-align:center;'>{objDue.TotalReceive:F2}</td>
                                <td style='text-align:center;'>{objDue.DueAmount:F2}</td>";
                            fullHtml += "</tr>";

                        }

                        fullHtml += $@"<tr style='height:25px;font-weight:bold;text-align:center;font-size:14px;'><td colspan='6' style='text-align:right;'><b>{company.CompanyName} TOTAL</b></td>                              
                                
                                <td style='text-align:center;'>{companyWiseDataList.Sum(o => o.Rent):F2}</td>
                                <td style='text-align:center;'>{companyWiseDataList.Sum(o => o.ExtraBedCharge):F2}</td>
                                <td style='text-align:center;'>{companyWiseDataList.Sum(o => o.ExtraCharge):F2}</td>
                                <td style='text-align:center;'>{companyWiseDataList.Sum(o => o.FoodBill):F2}</td>
                                <td style='text-align:center;'>{companyWiseDataList.Sum(o => o.ConfBill):F2}</td>
                                <td style='text-align:center;'>{companyWiseDataList.Sum(o => o.NetRent):F2}</td>
                                <td style='text-align:center;'>{companyWiseDataList.Sum(o => o.Discount + o.SpecialDiscount):F2}</td>
                                <td style='text-align:center;'>{companyWiseDataList.Sum(o => o.TotalBill):F2}</td>
                                <td style='text-align:center;'>{companyWiseDataList.Sum(o => o.TotalReceive):F2}</td>
                                <td style='text-align:center;'>{companyWiseDataList.Sum(o => o.DueAmount):F2}</td>";
                        fullHtml += "</tr>";
                    }

                    fullHtml += "</tbody>";

                    fullHtml += "<tfoot>";
                    fullHtml += $@"<tr style='font-weight:bold;'><td colspan='6' style='text-align:right;font-weight:bold;'><b>GRAND TOTAL</b></td>                              
                                
                                <td style='text-align:center;'>{objDataList.Sum(o => o.Rent):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.ExtraBedCharge):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.ExtraCharge):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.FoodBill):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.ConfBill):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.NetRent):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.Discount + o.SpecialDiscount):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.TotalBill):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.TotalReceive):F2}</td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.DueAmount):F2}</td>";
                    fullHtml += "</tr>";
                    fullHtml += "</tfoot>";
                    fullHtml += "</table>";

                }
                // === Footer Section ===
                fullHtml += @"<div class='report-footer'>
    		                    <hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
    		                    <p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>";
                fullHtml += @"</div>";
            }


            return fullHtml;

        }
        catch (Exception ex)
        {
            throw;
        }
    }
    #endregion

    #region AdvanceReportHtml
    public async Task<List<AdvanceReportVm>> GetAdvanceReport(AdvanceReportVm vm)
    {
        string guestIdFilter = (vm.GuestId > 0) ? $" and g.GuestId = {vm.GuestId}" : "";
        string guestNameFilter = (!string.IsNullOrEmpty(vm.GuestName)) ? $" and to_lower(g.GuestName) like '%{vm.GuestName.ToLower()}%'" : "";
        string companyIdFilter = (vm.CompanyId > 0) ? $" and g.CompanyId = {vm.CompanyId}" : "";

        string query = $@"select 'RESERVATION' BookingType, bs.Id BookingId,bs.BookingNo,bs.BookingDate, g.Id GuestId,g.FirstName+' '+isnull(g.LastName,'') GuestName,cp.Id CompanyId,cp.Name CompanyName
            ,min(hr.CheckInTime) CheckInTime,min(hr.ActualCheckInTime) ActualCheckInTime,max(hr.CheckOutTime) CheckOutTime,min(hr.ActualCheckOutTime) ActualCheckOutTime
            ,(string_agg(cast(r.RoomNo as nvarchar(max)), ', ') within group (order by r.RoomNo)) RoomNoList,ab.Amount,bs.NetRent,ab.MrNoList
            from HtBookingServices bs
            inner join HtBookingRooms hr on hr.BookingId = bs.Id
            inner join HtRoomInfos r on r.Id = hr.RoomId
            inner join HtRoomCategories rc on rc.Id = r.RoomCategoryId
            left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
            left join HtGuestInfos g on g.Id= bg.GuestId
            left join ClientCompanies cp on cp.Id = g.CompanyId
            inner join (select sum(PaidAmount) Amount,BookingId,(string_agg(cast(TransactionNo as nvarchar(max)), ', ') within group (order by TransactionNo)) MrNoList
            from HtBookingPayments where BillingId is null group by BookingId) ab on ab.BookingId = bs.Id
            where bs.BookingStatus = 1 and cast(bs.BookingDate as Date) >= '01/Dec/2024' and ab.Amount > 0 and bs.IsDeleted = 0
            group by bs.Id,bs.BookingNo,bs.BookingDate, g.Id,g.FirstName+' '+isnull(g.LastName,''),ab.Amount,cp.id,cp.Name,bs.NetRent,ab.MrNoList
            --order by bs.BookingDate
            union all
            select 'ROOM ADVANCE' BookingType, bs.Id BookingId,bs.BookingNo,bs.BookingDate, g.Id GuestId,g.FirstName+' '+isnull(g.LastName,'') GuestName,cp.Id CompanyId,cp.Name CompanyName
            ,min(hr.CheckInTime) CheckInTime,min(hr.ActualCheckInTime) ActualCheckInTime,max(hr.CheckOutTime) CheckOutTime,min(hr.ActualCheckOutTime) ActualCheckOutTime
            ,(string_agg(cast(r.RoomNo as nvarchar(max)), ', ') within group (order by r.RoomNo)) RoomNoList,ab.Amount,bs.NetRent,ab.MrNoList
            from HtBookingServices bs
            inner join HtBookingRooms hr on hr.BookingId = bs.Id
            inner join HtRoomInfos r on r.Id = hr.RoomId
            inner join HtRoomCategories rc on rc.Id = r.RoomCategoryId
            left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
            left join HtGuestInfos g on g.Id= bg.GuestId
            left join ClientCompanies cp on cp.Id = g.CompanyId
            inner join (select sum(PaidAmount) Amount,BookingId,(string_agg(cast(TransactionNo as nvarchar(max)), ', ') within group (order by TransactionNo)) MrNoList
            from HtBookingPayments where BillingId is null group by BookingId) ab on ab.BookingId = bs.Id
            where bs.BookingStatus = 2 and cast(bs.BookingDate as Date) >= '01/Dec/2024' and ab.Amount > 0 and bs.IsDeleted = 0
            group by bs.Id,bs.BookingNo,bs.BookingDate, g.Id,g.FirstName+' '+isnull(g.LastName,''),ab.Amount,cp.id,cp.Name,bs.NetRent,ab.MrNoList
            union all
            select 'HALL RESERVATION' BookingType, bs.Id BookingId,bs.BookingNo,bs.BookingDate, g.Id GuestId,g.FirstName+' '+isnull(g.LastName,'') GuestName,cp.Id CompanyId,cp.Name CompanyName
            ,min(bs.CheckInTime) CheckInTime,null ActualCheckInTime,max(bs.CheckOutTime) CheckOutTime,null ActualCheckOutTime
            ,(string_agg(cast(r.HallName as nvarchar(max)), ', ') within group (order by r.HallName)) RoomNoList,ab.Amount,bs.NetRent,ab.MrNoList
            from HtBookingServices bs
            inner join HtBookingHalls hr on hr.BookingId = bs.Id
            inner join HtHallInfos r on r.Id = hr.HallId
            left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
            left join HtGuestInfos g on g.Id= bg.GuestId
            left join ClientCompanies cp on cp.Id = g.CompanyId
            inner join (select sum(PaidAmount) Amount,BookingId,(string_agg(cast(TransactionNo as nvarchar(max)), ', ') within group (order by TransactionNo)) MrNoList
            from HtBookingPayments where BillingId is null group by BookingId) ab on ab.BookingId = bs.Id
            where bs.BookingStatus = 1 and cast(bs.CheckInTime as date) < '18/Jan/2025' and ab.Amount > 0 and bs.IsDeleted = 0 and cast(bs.BookingDate as Date) >= '01/Dec/2024'
            group by bs.Id,bs.BookingNo,bs.BookingDate, g.Id,g.FirstName+' '+isnull(g.LastName,''),ab.Amount,cp.id,cp.Name,bs.NetRent,ab.MrNoList
            union all
            select 'HALL ADVANCE' BookingType, bs.Id BookingId,bs.BookingNo,bs.BookingDate, g.Id GuestId,g.FirstName+' '+isnull(g.LastName,'') GuestName,cp.Id CompanyId,cp.Name CompanyName
            ,min(bs.CheckInTime) CheckInTime,null ActualCheckInTime,max(bs.CheckOutTime) CheckOutTime,null ActualCheckOutTime
            ,(string_agg(cast(r.HallName as nvarchar(max)), ', ') within group (order by r.HallName)) RoomNoList,ab.Amount,bs.NetRent,ab.MrNoList
            from HtBookingServices bs
            inner join HtBookingHalls hr on hr.BookingId = bs.Id
            inner join HtHallInfos r on r.Id = hr.HallId
            left join (select max(GuestId)GuestId,BookingId from HtBookingGuests where IsMain = 1 group by BookingId ) bg on bg.BookingId = bs.Id
            left join HtGuestInfos g on g.Id= bg.GuestId
            left join ClientCompanies cp on cp.Id = g.CompanyId
            inner join (select sum(PaidAmount) Amount,BookingId,(string_agg(cast(TransactionNo as nvarchar(max)), ', ') within group (order by TransactionNo)) MrNoList
            from HtBookingPayments where BillingId is null group by BookingId) ab on ab.BookingId = bs.Id
            where bs.BookingStatus = 1 and cast(bs.CheckInTime as date) >= '18/Jan/2025' and ab.Amount > 0 and bs.IsDeleted = 0 and cast(bs.BookingDate as Date) >= '01/Dec/2024'
            group by bs.Id,bs.BookingNo,bs.BookingDate, g.Id,g.FirstName+' '+isnull(g.LastName,''),ab.Amount,cp.id,cp.Name,bs.NetRent,ab.MrNoList";

        var data = await _iReadDbConnection.QueryAsync<AdvanceReportVm>(query);
        return data.ToList();
    }
    public async Task<string> AdvanceReportHtml(AdvanceReportVm vm)
    {
        try
        {
            string fullHtml = "";
            List<AdvanceReportVm> objDataList = await GetAdvanceReport(vm);

            if (!string.IsNullOrEmpty(vm.QType))
            {
                if (vm.QType == "RR")
                    objDataList = objDataList.Where(x => x.BookingType.Equals(AdvanceReportType.Reservation)).ToList();
                else if (vm.QType == "RA")
                    objDataList = objDataList.Where(x => x.BookingType.Equals(AdvanceReportType.RoomAdvance)).ToList();
                else if (vm.QType == "HR")
                    objDataList = objDataList.Where(x => x.BookingType.Equals(AdvanceReportType.HallReservation)).ToList();
                else if (vm.QType == "HA")
                    objDataList = objDataList.Where(x => x.BookingType.Equals(AdvanceReportType.HallAdvance)).ToList();
                else
                    objDataList = objDataList.ToList();
            }

            if (objDataList.Count > 0)
            {
                fullHtml += "<table class='table report-table table-bordered' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
                fullHtml += "<thead>";

                fullHtml += $@"<tr style='height:30px;'><td colspan='11' class='text-center'>Date: {DateTime.Today:dd/MMM/yyyy}</td></tr>";

                fullHtml += "<tr style='height:30px;' class='text-center'>";

                fullHtml += $@"<th style='width:30px;' >SL</th>
                            <th style='width:150px;'> Type </th>
                            <th style='width:100px;'> Booking No</th>
                            <th style='width:80px;'> Booking Date</th>
                            <th style='width:140px;'> Guest Name</th>
                            <th style='width:90px;'> Booking Check In</th>
                            
                            <th style='width:100px;'> Booking Check Out</th>
                            <th style='width:90px;'> Booking Amount</th>
                            <th style='width:250px;'> Room No List</th>
                            <th style='width:90px;'> Advance Amount</th>
                            <th style='width:100px;'> Mr No List</th>";

                fullHtml += "</tr>";

                fullHtml += "</thead>";
                fullHtml += "<tbody>";

                for (int i = 0; i < objDataList.Count; i++)
                {
                    AdvanceReportVm objDue = objDataList[i];
                    string bookingNo = !vm.IsPirnt ? $"<a target='_blank' href='../BookingService/Details/{objDue.BookingId}'>{objDue.BookingNo}</a>" : $"{objDue.BookingNo}";

                    fullHtml += "<tr>";

                    fullHtml += $@"<td style='text-align:left;'>{i + 1}</td>";
                    fullHtml += $@"<td style='text-align:center;'><b>{objDue.BookingType}</b></td>                              
                                <td style='text-align:center;'>{bookingNo}</td>
                                <td style='text-align:center;'>{objDue.BookingDate:dd/MMM/yy}</td>
                                <td style='text-align:center;'>{objDue.GuestName}</td>
                                <td style='text-align:center;'>{objDue.CheckInTime:dd/MMM/yy}</td>
                                <td style='text-align:center;'>{objDue.CheckOutTime:dd/MMM/yy}</td>
                                <td style='text-align:center;'>{objDue.NetRent:F2}</td>
                                <td style='text-align:center;'>{objDue.RoomNoList}</td>
                                <td style='text-align:center;'>{objDue.Amount:F2}</td>
                                <td style='text-align:center;'>{objDue.MrNoList}</td>";
                    fullHtml += "</tr>";

                }

                fullHtml += "</tbody>";

                fullHtml += "<tfoot>";
                fullHtml += $@"<tr><td colspan='7' style='text-align:right;'><b>TOTAL</b></td>                              
                                <td style='text-align:center;'>{objDataList.Sum(o => o.NetRent):F2}</td>
                                <td style='text-align:center;'></td>
                                <td style='text-align:center;'>{objDataList.Sum(o => o.Amount):F2}</td>
                                <td style='text-align:center;'></td>";
                fullHtml += "</tr>";
                fullHtml += "</tfoot>";

                fullHtml += "</table>";
            }

            return fullHtml;

        }
        catch (Exception ex)
        {
            throw;
        }
    }
    #endregion

    #region PaymentTransactionReportDataAsync

    public async Task<List<PaymentTransactionReportVm>> PaymentTransactionReportDataAsync(PaymentTransactionReportVm vm)
    {
        string payModeFilter = vm.PayMode.HasValue ? $@"and bp.PayMode = {vm.PayMode.Value}" : "";
        //string payTypeFilter = vm.PayType.HasValue && vm.PayType.Value == 0 ? $@"and bp.BillingId is null" : vm.PayType.Value == 1 ? $@"and ar.BillingId is not null" : vm.PayType.Value == 2 ? $@"and ar.BookingId is not null" :"";
        var fromDate = (DateTime)(!string.IsNullOrEmpty(vm.StrFromDate) ? Utility.ConvertStrToDate(vm.StrFromDate) : DateTime.Today);
        var toDate = (DateTime)(!string.IsNullOrEmpty(vm.StrToDate) ? Utility.ConvertStrToDate(vm.StrToDate) : DateTime.Today);

        string query = $@" 
                            DECLARE @FromDate DATE = @pFromDate;
                            DECLARE @ToDate   DATE = @pToDate;

                            SELECT
                                bp.Id AS PaymentId,
                                bp.TransactionNo,
	                            bp.PaidDate,
                                bp.PayMode,
                                bp.Description,
                                bp.PaidAmount,
	
                                CONVERT(date,bp.AuditDate) AuditDate,
	                            bp.AuditRemarks,
	                            ab.FullName AS AuditBy,

                                u.FullName AS ReceivedBy,
    
                                bs.Id AS BookingId,
                                bs.BookingDate,
                                bs.BookingNo,
                                bs.PaymentStatus,
    
                                b.Id AS BillId,
                                b.BillNumber,
    
                                gi.Salutation + ' ' + gi.FirstName + ' ' + ISNULL(gi.LastName, '') AS GuestName,
                                gi.Mobile AS GuestMobile,
    
                                STRING_AGG(ri.RoomNo, ', ') AS RoomNoList,
                                CASE
                                  WHEN bp.PaidDate IS NULL OR bs.CheckOutTime IS NULL THEN 0
                                  WHEN CAST(bp.PaidDate AS DATE) > CAST(bs.CheckOutTime AS DATE) THEN 1
                                  ELSE 0
                                END AS IsDueCollection
                            FROM HtBookingPayments bp
                            INNER JOIN HtBookingServices bs ON bp.BookingId = bs.Id
                            LEFT JOIN HtBillings b ON bp.BillingId = b.Id
                            LEFT JOIN AspNetUsers u ON u.Id = bp.ActionById
                            LEFT JOIN AspNetUsers ab ON ab.Id = bp.AuditById
                            LEFT JOIN HtBookingRooms br ON br.BookingId = bs.Id
                            LEFT JOIN HtRoomInfos ri ON ri.Id = br.RoomId
                            LEFT JOIN HtBookingGuests bg ON bg.BookingId = bs.Id
                            LEFT JOIN HtGuestInfos gi ON gi.Id = bg.GuestId
                            WHERE 
                                   (CAST(bp.PaidDate AS DATE) BETWEEN @FromDate AND @ToDate) {payModeFilter}
                            GROUP BY 
                                bp.Id, bp.TransactionNo, bp.PayMode, bp.Description, bp.PaidAmount, bp.PaidDate,
                                u.FullName, bs.Id, bs.BookingNo, bs.BookingDate, bs.CheckOutTime, bs.PaymentStatus, b.Id, b.BillNumber,
                                gi.Salutation, gi.FirstName, gi.LastName, gi.Mobile,bp.AuditDate,bp.AuditRemarks,ab.FullName;";

        var data = await _iReadDbConnection.QueryAsync<PaymentTransactionReportVm>(query, new { pFromDate = vm.StrFromDate, pToDate = vm.StrToDate });
        return data.ToList();
    }

    #endregion

    #region MoneyReceiptDataAsync

    public async Task<List<MoneyReceiptPrintVm>> MoneyReceiptDataAsync(long id)
    {
        //string payModeFilter = vm.PayMode.HasValue ? $@"and bp.PayMode = {vm.PayMode.Value}" : "";
        //string payTypeFilter = vm.PayType.HasValue && vm.PayType.Value == 0 ? $@"and bp.BillingId is null" : vm.PayType.Value == 1 ? $@"and ar.BillingId is not null" : vm.PayType.Value == 2 ? $@"and ar.BookingId is not null" : "";
        //var fromDate = (DateTime)(!string.IsNullOrEmpty(vm.StrFromDate) ? Utility.ConvertStrToDate(vm.StrFromDate) : DateTime.Today);
        //var toDate = (DateTime)(!string.IsNullOrEmpty(vm.StrToDate) ? Utility.ConvertStrToDate(vm.StrToDate) : DateTime.Today);

        string query = $@"  SELECT 
                            bp.Id,
                            bs.BookingNo,
                            bp.PaidDate,
                            hb.BillNumber,
                            bp.TransactionNo,
                            bp.PaidAmount,
                            gi.Salutation + ' ' + gi.FirstName + ' ' + ISNULL(gi.LastName, '') AS GuestName,
                            gi.Mobile,
                            STRING_AGG(ri.RoomNo, ',') AS RoomNoList

                            FROM HtBookingPayments bp
                            LEFT JOIN HtBillings hb ON bp.BillingId = hb.Id
                            LEFT JOIN HtBookingServices bs ON bp.BookingId = bs.Id
                            LEFT JOIN HtBookingGuests bg ON bp.BookingId = bg.BookingId
                            LEFT JOIN HtGuestInfos gi ON gi.Id = bg.GuestId
                            LEFT JOIN HtBookingRooms br ON br.BookingId = bs.Id
                            LEFT JOIN HtRoomInfos ri ON ri.Id = br.RoomId
                            WHERE bp.Id = {id}

                            GROUP BY 
                            bp.Id,
                            bs.BookingNo,
                            bp.PaidDate,
                            hb.BillNumber,
                            bp.TransactionNo,
                            bp.PaidAmount,
                            gi.Salutation,
                            gi.FirstName,
                            gi.LastName,
                            gi.Mobile;";

        var data = await _iReadDbConnection.QueryAsync<MoneyReceiptPrintVm>(query);
        return data.ToList();
    }

    #endregion

    #region ExtraServiceReportDataAsync

    public async Task<List<ExtraServiceReportVm>> ExtraServiceReportDataAsync(ExtraServiceReportVm vm)
    {
        DateTime fromDate = string.IsNullOrEmpty(vm.StrFromDate) ? DateTime.Today : Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate));

        DateTime toDate = string.IsNullOrEmpty(vm.StrToDate) ? DateTime.Today : Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate));

        string serviceFilter = vm.ServiceId > 0 ? @$"and s.Id = {vm.ServiceId}" : "";

        #region OLD
        //string query = $@" 
        //                    DECLARE @FromDate DATE = @pFromDate;
        //                    DECLARE @ToDate   DATE = @pToDate;

        //                    SELECT 
        //                    CONVERT(date, bd.ServiceDate) AS ServiceDate,
        //                    ri.RoomNo,
        //                    bs.BookingNo,
        //                    bs.Id BookingId,
        //                    b.Id BillId,
        //                    bd.Id BillDtlId,
        //                 b.BillNumber BillNo,
        //                    s.Id ServiceId,
        //                    s.ServiceName,
        //                    s.ServiceCode,

        //                    CONVERT(date,bd.AuditDate) AuditDate,
        //                    bd.AuditRemarks,
        //                    ab.FullName AS AuditBy,

        //                    SUM(bd.Amount) AS Amount,
        //                 SUM(bd.Rate) AS Rate,
        //                 SUM(bd.Discount) AS Discount,
        //                 SUM(bd.ServiceCharge) AS ServiceCharge,
        //                 SUM(bd.VAT) AS VAT,
        //                    SUM(bd.Quantity) AS Quantity,
        //                    SUM(bd.NetAmount) AS TotalAmount

        //                    FROM HtBillings b
        //                    LEFT JOIN HtBillingDetails bd ON b.Id = bd.BillId
        //                    LEFT JOIN AspNetUsers u ON u.Id = bd.ActionById
        //                    LEFT JOIN AspNetUsers ab ON ab.Id = bd.AuditById
        //                    LEFT JOIN HtServices s ON s.Id = bd.ServiceId
        //                    LEFT JOIN HtBookingServices bs ON bs.Id = b.BookingId
        //                    LEFT JOIN (
        //                    SELECT BookingId, MIN(RoomId) AS RoomId
        //                    FROM HtBookingRooms
        //                    GROUP BY BookingId
        //                    ) br ON br.BookingId = bs.Id
        //                    LEFT JOIN HtRoomInfos ri ON ri.Id = br.RoomId
        //                    WHERE s.ServiceCode <> 'SR000001' and CONVERT(date, bd.ServiceDate) between @FromDate and @ToDate {serviceFilter}

        //                    GROUP BY CONVERT(date, bd.ServiceDate), ri.RoomNo, bs.BookingNo, bs.Id, s.ServiceName, s.ServiceCode,b.Id, b.BillNumber, s.Id,bd.Id,
        //                    bd.AuditDate,bd.AuditRemarks,ab.FullName
        //                    ORDER BY CONVERT(date, bd.ServiceDate) DESC, bs.BookingNo DESC;";
        #endregion

        var query = $@"
            DECLARE @FromDate DATE = @pFromDate;
            DECLARE @ToDate   DATE = @pToDate;

            SELECT 
            CONVERT(date, bd.ServiceDate) AS ServiceDate,
            ri.RoomNo,
            bs.BookingNo,
            bs.Id BookingId,
            b.Id BillId,
            bd.Id BillDtlId,
            b.BillNumber BillNo,
            s.Id ServiceId,
            s.ServiceName,
            s.ServiceCode,
            CONVERT(date,bd.AuditDate) AuditDate,
            bd.AuditRemarks,
            ab.FullName AS AuditBy,
            SUM(bd.Amount) AS Amount,
            SUM(bd.Rate) AS Rate,
            SUM(bd.Discount) AS Discount,
            SUM(bd.ServiceCharge) AS ServiceCharge,
            SUM(bd.VAT) AS VAT,
            SUM(bd.Quantity) AS Quantity,
            SUM(bd.NetAmount) AS TotalAmount

            FROM HtBillings b
            LEFT JOIN HtBillingDetails bd ON b.Id = bd.BillId
            LEFT JOIN AspNetUsers u ON u.Id = bd.ActionById
            LEFT JOIN AspNetUsers ab ON ab.Id = bd.AuditById
            LEFT JOIN HtServices s ON s.Id = bd.ServiceId
            LEFT JOIN HtBookingServices bs ON bs.Id = b.BookingId
            -- Join through BookingRooms to get the correct room
            LEFT JOIN HtBookingRooms br ON br.Id = bd.BookingRoomId  -- This is the key change
            LEFT JOIN HtRoomInfos ri ON ri.Id = br.RoomId
            WHERE s.ServiceCode <> 'SR000001' 
            AND CONVERT(date, bd.ServiceDate) between @FromDate and @ToDate {serviceFilter}
            GROUP BY CONVERT(date, bd.ServiceDate), ri.RoomNo, bs.BookingNo, bs.Id, s.ServiceName, 
            s.ServiceCode, b.Id, b.BillNumber, s.Id, bd.Id, bd.AuditDate, bd.AuditRemarks, ab.FullName
            ORDER BY CONVERT(date, bd.ServiceDate) DESC, bs.BookingNo DESC;";

        //var data = await _iReadDbConnection.QueryAsync<DepartureReportVm>(query);
        var data = await _iReadDbConnection.QueryAsync<ExtraServiceReportVm>(query, new { pFromDate = vm.StrFromDate, pToDate = vm.StrToDate });

        data = !string.IsNullOrEmpty(vm.ServiceCode) ? data.Where(x => x.ServiceCode.ToLower() == vm.ServiceCode.ToLower()).ToList() : data;

        return data.ToList();
    }

    #endregion

    #region HKSalesReportDataAsync

    public async Task<List<HkSalesReportVm>> HKSalesReportDataAsync(HkSalesReportVm vm)
    {
        DateTime fromDate = string.IsNullOrEmpty(vm.StrFromDate) ? DateTime.Today : Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate));

        DateTime toDate = string.IsNullOrEmpty(vm.StrToDate) ? DateTime.Today : Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate));

        //string serviceFilter = vm.ServiceId > 0 ? @$"and s.Id = {vm.ServiceId}" : "";

        string query = $@" 
                            DECLARE @FromDate DATE = @pFromDate;
                            DECLARE @ToDate   DATE = @pToDate;
                            SELECT 
                            tm.TranNo,
                            b.BillNumber,
                            b.Id as BillId,
                            ri.RoomNo,
                            CONVERT(date, tm.TranDate) AS TransDate,
                            tm.TotalAmount AS Amount,
                            tm.Discount,
                            (tm.TotalAmount - tm.Discount) AS DiscountedAmount,
                            tm.ServiceCharge,
                            tm.VAT,
                            ((tm.TotalAmount - tm.Discount) + tm.ServiceCharge + tm.VAT) AS NetTotal,

                            bs.BookingNo,
                            ri.RoomNo,

                            SUM(CASE WHEN bp.PayMode = 0 THEN bp.BillAmount ELSE 0 END) AS CashReceived,
                            SUM(CASE WHEN bp.PayMode = 3 THEN bp.BillAmount ELSE 0 END) AS CardReceived,
                            SUM(CASE WHEN bp.PayMode = 4 THEN bp.BillAmount ELSE 0 END) AS CompanyCredit,
                            CASE WHEN b.BillStatus != 2 THEN ((tm.TotalAmount - tm.Discount) + tm.ServiceCharge + tm.VAT) ELSE 0 END AS RoomCredit,
                            --((tm.TotalAmount - tm.Discount) + tm.ServiceCharge + tm.VAT) RoomCredit,

                            u.FullName AS TranBy

                            FROM TranMsts tm
                            LEFT JOIN TranDtls td ON tm.Id = td.TranMstId
                            LEFT JOIN HtRoomInfos ri ON ri.Id = tm.IssueRoomId
                            LEFT JOIN HtBookingServices bs ON bs.Id = tm.BookingId
                            LEFT JOIN HtBillings b ON b.BookingId = bs.Id
                            LEFT JOIN InventoryBillPayments bp ON bp.SaleId = tm.Id AND bp.PaymentType = 'P'
                            LEFT JOIN AspNetUsers u ON u.Id = tm.TranById

                            where tm.TranDate between @FromDate and @ToDate

                            GROUP BY 
                            tm.TranNo,
                            b.BillNumber,
                            b.Id,
                            CONVERT(date, tm.TranDate),
                            tm.TotalAmount,
                            tm.Discount,
                            tm.ServiceCharge,
                            tm.VAT,
                            bs.BookingNo,
                            ri.RoomNo,
                            u.FullName,
                            b.BillStatus,
                            ri.RoomNo;";

        //var data = await _iReadDbConnection.QueryAsync<DepartureReportVm>(query);
        var data = await _iReadDbConnection.QueryAsync<HkSalesReportVm>(query, new { pFromDate = vm.StrFromDate, pToDate = vm.StrToDate });
        return data.ToList();
    }

    #endregion

    #region RoomChangeReportDataAsync

    public async Task<List<RoomChangeReportVm>> RoomChangeReportDataAsync(RoomChangeReportVm vm)
    {
        DateTime fromDate = string.IsNullOrEmpty(vm.StrFromDate) ? DateTime.Today : Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate));

        DateTime toDate = string.IsNullOrEmpty(vm.StrToDate) ? DateTime.Today : Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate));

        string query = $@" 
                            DECLARE @FromDate DATE = @pFromDate;
                            DECLARE @ToDate   DATE = @pToDate;

                            SELECT 
                            CAST(cl.ChangeDate AS DATE) AS ChangeDate,
                            bs.BookingNo,
                            bs.Id as BookingId,
                            (g.Salutation + ' ' + g.FirstName + ' ' + isnull(g.LastName,'')) AS GuestName,
                            g.Mobile AS GuestMobile,
                            ori.RoomNo as OldRoomNo,
                            orc.CategoryName as OldCategoryName,
                            nri.RoomNo as NewRoomNo,
                            nrc.CategoryName as NewCategoryName,
                            u.FullName as SubmittedBy,
                            cl.Reason,
                            cl.Remarks
                            from HtRoomChangeLogs cl 
                            left JOIN HtBookingRooms br ON br.Id = cl.BookingRoomId
                            left JOIN HtBookingServices bs ON bs.Id = br.BookingId AND bs.BookingType = 'R'
                            left join HtBookingGuests bg on bg.BookingId = bs.Id
                            left JOIN HtGuestInfos g ON g.Id = bg.GuestId
                            left JOIN HtRoomInfos ori ON ori.Id = cl.OldRoomId
                            left JOIN HtRoomInfos nri ON nri.Id = cl.NewRoomId
                            left JOIN HtRoomCategories orc ON orc.Id = ori.RoomCategoryId
                            left JOIN HtRoomCategories nrc ON nrc.Id = nri.RoomCategoryId
                            left JOIN AspNetUsers u ON u.Id = cl.ActionById
                            WHERE 
                            CAST(cl.ChangeDate AS DATE) BETWEEN @FromDate AND @ToDate
                            ORDER BY cl.ChangeDate DESC;";

        //var data = await _iReadDbConnection.QueryAsync<DepartureReportVm>(query);
        var data = await _iReadDbConnection.QueryAsync<RoomChangeReportVm>(query, new { pFromDate = vm.StrFromDate, pToDate = vm.StrToDate });
        return data.ToList();
    }

    #endregion

    #region InHouseGuestLedgerDueReportDataAsync

    public async Task<List<InHouseGuestLedgerReportVm>> InHouseGuestLedgerDueReportDataAsync(InHouseGuestLedgerReportVm vm)
    {
        DateTime queryDate = string.IsNullOrEmpty(vm.StrQueryDate) ? DateTime.Today : Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrQueryDate));

        string query = $@"DECLARE @BusinessDate DATE = @pFromDate;

                     WITH RoomTariff AS
                        (SELECT
                            br.BookingId,
                            br.Id AS BookingRoomId,
                            br.RoomId,
		                    br.RoomRent,
                            r.RoomNo,
		                    rc.CategoryName,
                            SUM(rda.Rate) AS RoomRentCharged
                        FROM HtBookingRooms br
	                    INNER JOIN HtRoomCategories rc
                            ON rc.Id = br.RoomCategoryId
                        INNER JOIN HtRoomInfos r
                            ON r.Id = br.RoomId
                        LEFT JOIN HtRoomDayAudits rda
                            ON rda.BookingRoomId = br.Id
		                    AND rda.IsCharged = 1
		                    AND CONVERT(date, rda.BusinessDate) <= @BusinessDate
                        WHERE
                            br.IsDeleted = 0
                            AND br.ActualCheckInTime IS NOT NULL
                            AND CONVERT(date, br.ActualCheckInTime) <= @BusinessDate
		                    AND (
				                    br.ActualCheckOutTime IS NULL
			                        OR CONVERT(date, br.ActualCheckOutTime) > @BusinessDate
			                    )
                        GROUP BY
                            br.BookingId,
                            br.Id,
                            br.RoomId,
		                    br.RoomRent,
                            r.RoomNo,
		                    rc.CategoryName
                        ),
                        FnbRoom AS
                        (SELECT
                            fo.RoomId,
                            fo.BookingId,     -- if exists, keep it; else remove
                            SUM(fo.NetAmount) AS FnbCharged
                        FROM RsFoodOrders fo
                        WHERE
                            fo.AuditById IS NOT NULL
                            AND fo.AuditDate IS NOT NULL
                        GROUP BY
                            fo.RoomId,
                            fo.BookingId
                    ),
                    PaymentAgg AS
                    (
                        SELECT
                            BookingId,
                            SUM(PaidAmount) AS TotalPaid
                        FROM HtBookingPayments
                        WHERE
                            AuditDate IS NOT NULL
                        GROUP BY
                            BookingId
                    ),
                    BookingChargeAgg AS
                    (
                        SELECT
                            rt.BookingId,
                            SUM(rt.RoomRentCharged) AS TotalRoomCharge,
                            SUM(ISNULL(fr.FnbCharged,0)) AS TotalFnbCharge,
                            SUM(rt.RoomRentCharged + ISNULL(fr.FnbCharged,0)) AS BookingTotalCharged
                        FROM RoomTariff rt
                        LEFT JOIN FnbRoom fr
                            ON fr.RoomId = rt.RoomId
                            AND fr.BookingId = rt.BookingId
                        GROUP BY
                            rt.BookingId
                    )

                    SELECT
                        bs.Id AS BookingId,
                        bs.BookingNo,

                        gi.Salutation + ' ' + gi.FirstName + ' ' + ISNULL(gi.LastName,'') AS GuestName,
                        gi.Mobile AS GuestMobile,

                        rt.RoomNo,
                        br.CheckInTime,
	                    br.CheckOutTime,
	                    rt.CategoryName,
	                    cc.Name AS CompanyName,
	                    br.RoomRent AS RackRate,
                        rt.RoomRentCharged,
                        ISNULL(fr.FnbCharged, 0) AS FnbCharged,
                        (rt.RoomRentCharged + ISNULL(fr.FnbCharged,0)) AS RoomTotalTariff,
                        bca.BookingTotalCharged,
                        ISNULL(p.TotalPaid, 0) AS BookingPaid,
                        (bca.BookingTotalCharged - ISNULL(p.TotalPaid,0)) AS BookingDue

                    FROM RoomTariff rt
                    INNER JOIN HtBookingServices bs
                        ON bs.Id = rt.BookingId
                    INNER JOIN HtBookingRooms br
                        ON br.Id = rt.BookingRoomId
                    INNER JOIN HtBookingGuests bg
                        ON bg.BookingId = bs.Id
                    INNER JOIN HtGuestInfos gi
                        ON gi.Id = bg.GuestId
                    LEFT JOIN ClientCompanies cc
                        ON cc.Id = gi.CompanyId
                    LEFT JOIN FnbRoom fr
                        ON fr.RoomId = rt.RoomId
                        AND fr.BookingId = rt.BookingId  
                    LEFT JOIN BookingChargeAgg bca
                        ON bca.BookingId = bs.Id
                    LEFT JOIN PaymentAgg p
                        ON p.BookingId = bs.Id

                    ORDER BY
                        rt.RoomNo;";

        try
        {
            var data = await _iReadDbConnection.QueryAsync<InHouseGuestLedgerReportVm>(query, new { pFromDate = queryDate });
            return data.ToList();
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    #endregion

    private string BuildInHouseGuestLedgerHtml(List<InHouseGuestLedgerReportVm> data, InHouseGuestLedgerReportVm vm)
    {
        var sb = new System.Text.StringBuilder();

        data = data ?? new List<InHouseGuestLedgerReportVm>();

        var bookingTotals = data
            .GroupBy(d => d.BookingId)
            .ToDictionary(g => g.Key, g => new
            {
                InHouseTotalAmount = g.Sum(x => x.RoomTotalTariff),
                TotalPaid = g.Max(x => x.BookingPaid),
                TotalBookingCharged = g.Max(x => x.BookingTotalCharged),
                TotalDue = g.Max(x => x.BookingDue)
            });

        // Optional header
        if (vm.IsPirnt)
        {
            sb.Append($@"<div style='text-align:center;margin-bottom:20px;'>
                        <p style='margin:4px 0;color:#555;font-size:14px;'><b>Date: </b> {System.Net.WebUtility.HtmlEncode(vm.StrQueryDate)}</p>
                        <hr style='border:1px solid #ddd;width:80%;margin:10px auto;' />
                     </div>");
        }

        sb.Append("<table class='table table-bordered report-table mb-3 w-100' id='print_table' style='width:100%; padding-bottom:10px; repeat-header:yes;' border='1'>");

        sb.Append("<thead>");
        sb.Append("<tr style='height:30px;'>");
        sb.Append(@"<th style='width:3%;'>SL No</th>
                <th style='width:15%; text-align:center;'>Room Info</th>
                <th style='width:8%; text-align:center;'>Booking No</th>
                <th style='width:15%; text-align:center;'>Name Of Guest</th>
                <th style='width:8%; text-align:center;'>C/In Date</th>
                <th style='width:8%; text-align:center;'>C/Out Date</th>
                <th style='width:5%; text-align:center;'>Night's</th>
                <th style='width:8%; text-align:center;'>Room Tarrif</th>
                <th style='width:8%; text-align:center;'>Food Bill</th>
                <th style='width:10%; text-align:center;'>In-House Amount</th>
                <th style='width:10%; text-align:center;'>Total Charged Amount</th>
                <th style='width:10%; text-align:center;'>Paid</th>
                <th style='width:10%; text-align:center;'>Balance</th>");
        sb.Append("</tr>");
        sb.Append("</thead>");

        sb.Append("<tbody>");

        // Ensure deterministic order: group by booking then by room
        var ordered = data.OrderBy(d => d.BookingId).ThenBy(d => d.RoomNo).ToList();

        for (int i = 0; i < ordered.Count; i++)
        {
            var r = ordered[i];

            string bookingNo = vm.IsPirnt
                ? System.Net.WebUtility.HtmlEncode(r.BookingNo)
                : $"<a target='_blank' href='../BookingService/Details/{r.BookingId}'>{System.Net.WebUtility.HtmlEncode(r.BookingNo)}</a>";

            var nights = 0;
            try
            {
                nights = Domain.Utility.AppUtility.DaysDiffernce(r.CheckOutTime.Date, r.CheckInTime.Date);
            }
            catch
            {
                // ignore; fallback to 0
            }

            bool isFirstForBooking = (i == 0) || (ordered[i].BookingId != ordered[i - 1].BookingId);

            sb.Append("<tr>");
            sb.Append($"<td style='text-align:center;'>{i + 1}</td>");
            sb.Append($"<td style='text-align:center;padding:5px;'><b>{System.Net.WebUtility.HtmlEncode(r.RoomNo)} - {System.Net.WebUtility.HtmlEncode(r.CategoryName)}</b></td>");
            sb.Append($"<td style='text-align:center;'>{bookingNo}</td>");
            sb.Append($"<td style='text-align:center;'><b>{System.Net.WebUtility.HtmlEncode(r.GuestName)}</b><br/>{System.Net.WebUtility.HtmlEncode(r.GuestMobile)}</td>");
            sb.Append($"<td style='text-align:center;'>{DU.Utility.ConvertDateToStr(r.CheckInTime)}</td>");
            sb.Append($"<td style='text-align:center;'>{DU.Utility.ConvertDateToStr(r.CheckOutTime)}</td>");
            sb.Append($"<td style='text-align:center;'>{nights}</td>");
            sb.Append($"<td style='text-align:right;'>{r.RackRate:F2}</td>");
            sb.Append($"<td style='text-align:right;'>{r.FnbCharged:F2}</td>");

            if (isFirstForBooking && bookingTotals.TryGetValue(r.BookingId, out var totals))
            {
                var dueAmount = totals.TotalDue > 0 ? totals.TotalDue.ToString("F2") : $"({Math.Abs(totals.TotalDue):F2})";

                sb.Append($"<td style='text-align:right;'>{totals.InHouseTotalAmount:F2}</td>");
                sb.Append($"<td style='text-align:right;'>{totals.TotalBookingCharged:F2}</td>");
                sb.Append($"<td style='text-align:right;'>{totals.TotalPaid:F2}</td>");
                sb.Append($"<td style='text-align:right;'>{dueAmount}</td>");
            }
            else
            {
                sb.Append("<td></td><td></td><td></td><td></td>");
            }

            sb.Append("</tr>");
        }

        // Grand totals (bottom)
        if (ordered.Any())
        {
            var totalDueAmount = ordered.DistinctBy(x => x.BookingId).Sum(x => x.BookingDue);
            var totalDue = totalDueAmount > 0 ? totalDueAmount.ToString("F2") : $"({Math.Abs(totalDueAmount):F2})";

            sb.Append($@"<tr>
                        <td colspan='7' style='text-align:right;'><b>Total</b></td>
                        <td style='text-align:right;'><b>{ordered.Sum(x => x.RackRate):F2}</b></td>
                        <td style='text-align:right;'><b>{ordered.Sum(x => x.FnbCharged):F2}</b></td>
                        <td style='text-align:right;'><b>{ordered.Sum(x => x.RoomTotalTariff):F2}</b></td>
                        <td style='text-align:right;'><b>{ordered.DistinctBy(x => x.BookingId).Sum(x => x.BookingTotalCharged):F2}</b></td>
                        <td style='text-align:right;'><b>{ordered.DistinctBy(x => x.BookingId).Sum(x => x.BookingPaid):F2}</b></td>
                        <td style='text-align:right;'><b>{totalDue}</b></td>
                     </tr>");
        }

        sb.Append("</tbody>");
        sb.Append("</table>");

        // footer
        sb.Append(@"<div class='report-footer'>
                    <hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
                    <p>Generated on: " + System.DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>");
        sb.Append("</div>");

        return sb.ToString();
    }

    public async Task<string> InHouseGuestLedgerReportUpdateHtml(InHouseGuestLedgerReportVm vm)
    {
        try
        {
            var data = await InHouseGuestLedgerDueReportDataAsync(vm);
            return BuildInHouseGuestLedgerHtml(data, vm);
        }
        catch (Exception)
        {
            throw;
        }
    }

}