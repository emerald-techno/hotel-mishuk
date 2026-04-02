using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Billing;
using Domain.ViewModel.HotelManagement.Booking;
using Domain.ViewModel.HotelManagement.FD_PaymentTranReport;
using Domain.ViewModel.HotelManagement.HotelReport;
using Domain.ViewModel.HotelManagement.MoneyReceipt;
using Domain.ViewModel.HotelManagement.RoomBooking;
using Domain.ViewModel.Report;
using Domain.ViewModel.Website;
using Interface.Repository.Accounts;
using Interface.Repository.Common;
using Interface.Repository.HotelManagement;
using Interface.Repository.Restaurant;
using Interface.Services;
using Interface.Services.Accounts;
using Interface.Services.Admin;
using Interface.Services.HotelManagement;
using Interface.Services.Notification;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Services.Base;
using System.Transactions;
using DU = Domain.Utility;

namespace Services.HotelManagement;

public class BookingServiceService : BaseService<HtBookingService>, IBookingServiceService
{
    #region Config
    private IBookingServiceRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IBookingRoomRepository _iBookingRoomRepository;
    private readonly IBookingGuestRepository _iBookingGuestRepository;
    private readonly IBookingPaymentRepository _iBookingPaymentRepository;
    private readonly IRoomCategoryRepository _iRoomCategoryRepository;
    private readonly IRoomInfoRepository _iRoomInfoRepository;
    private readonly IAutoCodeRepository _iAutoCodeRepository;
    private readonly IServiceRepository _iHtServiceRepository;
    private readonly IBillingRepository _iBillRepository;
    private readonly IBillingDetailRepository _iBillingDetailRepository;
    private readonly IAccTranMstRepository _iAccTranMstRepository;
    private readonly IAccTranDtlRepository _iAccTranDtlRepository;
    private readonly IHotelManagementRepository _iHotelManagementRepository;
    private readonly IFoodOrderRepository _iFoodOrderRepository;
    private readonly IRsOrderPaymentRepository _iOrderPaymentRepository;
    private readonly IRoomStatusHistoryRepository _iRoomStatusHistoryRepository;
    private readonly IHallInfoRepository _iHallInfoRepository;
    private readonly IBookingHallRepository _iBookingHallRepository;
    private readonly IAdvanceRefundRepository _iAdvanceRefundRepository;
    private readonly ISetCurrencyService _iSetCurrencyService;
    private readonly ISetFincYearService _iSetFincYearService;
    private readonly IAccLedgerService _iAccLedgerService;
    private readonly IRoomInfoService _iRoomInfoService;
    private readonly IGuestInfoRepository _iGuestInfoRepository;
    private readonly INtfNotificationMsgService _iNtfMsgService;
    private readonly IUrlHelperService _iUrlHelperService;
    private readonly IHttpContextAccessor _iHttpContextAccessor;
    private readonly IRoomDayAuditRepository _iRoomDayAuditRepository;

    private readonly IBillService _iBillService;

    public BookingServiceService(IBookingServiceRepository repository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork,
        IBookingRoomRepository iBookingRoomRepository,
        IBookingGuestRepository iBookingGuestRepository,
        IBookingPaymentRepository iBookingPaymentRepository,
        IRoomCategoryRepository iRoomCategoryRepository,
        IRoomInfoRepository iRoomInfoRepository,
        IAutoCodeRepository iAutoCodeRepository,
        IServiceRepository iHtServiceRepository,
        IBillingRepository iBillRepository,
        ISetCurrencyService iSetCurrencyService,
        ISetFincYearService iSetFincYearService,
        IAccTranMstRepository iAccTranMstRepository,
        IAccLedgerService iAccLedgerService,
        IHotelManagementRepository iHotelManagementRepository,
        IFoodOrderRepository iFoodOrderRepository,
        IAdvanceRefundRepository iAdvanceRefundRepository,
        IRoomInfoService iRoomInfoService,
        IHallInfoRepository iHallInfoRepository,
        IBookingHallRepository iBookingHallRepository,
        IRsOrderPaymentRepository iOrderPaymentRepository,
        INtfNotificationMsgService iNtfMsgService,
        IUrlHelperService iUrlHelperService,
        IHttpContextAccessor iHttpContextAccessor,
        IGuestInfoRepository iGuestInfoRepository,
        IRoomStatusHistoryRepository iRoomStatusHistoryRepository,
        IAccTranDtlRepository iAccTranDtlRepository,
        IBillingDetailRepository iBillingDetailRepository,
        IBillService iBillService,
        IRoomDayAuditRepository iRoomDayAuditRepository) : base(repository, iUnitOfWork)
    {
        _iRepository = repository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iBookingRoomRepository = iBookingRoomRepository;
        _iBookingGuestRepository = iBookingGuestRepository;
        _iBookingPaymentRepository = iBookingPaymentRepository;
        _iRoomCategoryRepository = iRoomCategoryRepository;
        _iRoomInfoRepository = iRoomInfoRepository;
        _iAutoCodeRepository = iAutoCodeRepository;
        _iHtServiceRepository = iHtServiceRepository;
        _iBillRepository = iBillRepository;
        _iAccTranMstRepository = iAccTranMstRepository;
        _iAccLedgerService = iAccLedgerService;
        _iSetCurrencyService = iSetCurrencyService;
        _iSetFincYearService = iSetFincYearService;
        _iHotelManagementRepository = iHotelManagementRepository;
        _iFoodOrderRepository = iFoodOrderRepository;
        _iAdvanceRefundRepository = iAdvanceRefundRepository;
        _iRoomInfoService = iRoomInfoService;
        _iHallInfoRepository = iHallInfoRepository;
        _iBookingHallRepository = iBookingHallRepository;
        _iOrderPaymentRepository = iOrderPaymentRepository;
        _iNtfMsgService = iNtfMsgService;
        _iUrlHelperService = iUrlHelperService;
        _iHttpContextAccessor = iHttpContextAccessor;
        _iGuestInfoRepository = iGuestInfoRepository;
        _iRoomStatusHistoryRepository = iRoomStatusHistoryRepository;
        _iAccTranDtlRepository = iAccTranDtlRepository;
        _iBillingDetailRepository = iBillingDetailRepository;
        _iBillService = iBillService;
        _iRoomDayAuditRepository = iRoomDayAuditRepository;
    }

    #endregion

    #region BookingEntry

    public async Task<(bool, long)> RoomBookingEntry(HtBookingServiceVm vm)
    {
        var bookingModel = _iMapper.Map<HtBookingService>(vm);

        bookingModel.BookingNo = await GetBookingCode();
        bookingModel.BookingDate = (DateTime)(!string.IsNullOrEmpty(vm.BookingDateStr) ? Utility.ConvertStrToDate(vm.BookingDateStr) : DU.Utility.GetBdDateTimeNow());
        bookingModel.CheckInTime = (DateTime)(!string.IsNullOrEmpty(vm.CheckInTimeStr) ? Utility.ConvertStrToDate(vm.CheckInTimeStr) : DU.Utility.GetBdDateTimeNow());
        bookingModel.CheckOutTime = (DateTime)(!string.IsNullOrEmpty(vm.CheckOutTimeStr) ? Utility.ConvertStrToDate(vm.CheckOutTimeStr) : DU.Utility.GetBdDateTimeNow());
        bookingModel.BookingType = BookingType.Room;
        bookingModel.BookingStatus = BookingServiceStatusEnum.Booked;
        bookingModel.ActionById = CurrentUserId;
        bookingModel.ActionDate = Utility.GetBdDateTimeNow();

        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        var bookingDateTime = bookingModel.BookingDate.Add(currentTime);

        var reportDate = Utility.GenerateReportDate(bookingDateTime);
        bookingModel.ReportDate = reportDate;

        if (bookingModel.BookingDate.Date > bookingModel.CheckInTime.Date)
            throw new Exception("Check-In Date Is Previous Date Than Booking Date..!!");

        if (bookingModel.CheckInTime.Date > bookingModel.CheckOutTime.Date)
            throw new Exception("CheakIn & CheckOut Date Is Not Correct..!!");

        if (vm.BookingRoomVms?.Count > 0 is false)
            throw new Exception("No Room Information Found...!!");

        var bookingRooms = _iMapper.Map<List<HtBookingRoom>>(vm.BookingRoomVms);

        List<HtRoomInfo> bookingRoomList = new List<HtRoomInfo>();
        HtGuestInfo mainGuestInfo = null;

        var roomCategories = await _iRoomCategoryRepository.GetAsync(x => x.IsActive && !x.IsDeleted);

        if (bookingRooms.Count > 0)
        {
            foreach (var room in bookingRooms)
            {
                var filterData = vm.BookingRoomVms.FirstOrDefault(x => x.RoomId == room.RoomId);
                if (filterData == null)
                    throw new Exception("Room Not Found...!");

                var category = roomCategories.FirstOrDefault(x => x.Id == room.RoomCategoryId);
                if (category == null)
                    throw new Exception("Category Not Found...!");

                room.CheckInTime = (DateTime)(!string.IsNullOrEmpty(filterData.CheckInTimeStr) ? Utility.ConvertStrToDate(filterData.CheckInTimeStr) : bookingModel.CheckInTime);
                room.CheckOutTime = (DateTime)(!string.IsNullOrEmpty(filterData.CheckOutTimeStr) ? Utility.ConvertStrToDate(filterData.CheckOutTimeStr) : bookingModel.CheckOutTime);

                if (room.CheckInTime.Date > room.CheckOutTime.Date)
                    throw new Exception("Room CheakIn & CheckOut Date Is Not Correct..!!");

                room.ActionDate = Utility.GetBdDateTimeNow();
                room.ActionById = CurrentUserId;

                var days = AppUtility.DaysDiffernceOnlyDate(room.CheckOutTime, room.CheckInTime);

                if (days > 0)
                {
                    if (room.RoomId != null && room.RoomId > 0)
                    {
                        var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == room.RoomId && x.IsActive && !x.IsDeleted);
                        if (roomInfo == null)
                            throw new Exception("Room Not Found...!!");

                        var checkAvalibility = await _iRoomInfoService.CheckRoomIsAvaliable(roomInfo.Id, room.CheckInTime, room.CheckOutTime);
                        if (!checkAvalibility)
                            throw new Exception("Room Is Not Available...!!");

                        room.Rent = (roomInfo.Rent * days);
                        room.ServiceCharge = (roomInfo.ServiceCharge * days);
                        room.NetRent = (room.Rent + room.ServiceCharge) - (room.Discount);
                        room.TotalGuest = roomInfo.Person;

                        roomInfo.BookingStatus = BookingStatusEnum.Booked;

                        bookingRoomList.Add(roomInfo);
                    }
                }
                else
                {
                    throw new Exception("Booking Days Have To More Than Zero....!!");
                }
            }

            bookingModel.Rent = bookingRooms.Sum(x => x.Rent);
            bookingModel.ServiceCharge = bookingRooms.Sum(x => x.ServiceCharge);
            bookingModel.Discount = bookingRooms.Sum(x => x.Discount);
            bookingModel.NetRent = bookingRooms.Sum(x => x.NetRent);
            bookingModel.TotalGuest = bookingRooms.Sum(x => x.TotalGuest);

            var minCheckInDate = bookingRooms.Select(x => x.CheckInTime).Min();
            var maxCheckOutDate = bookingRooms.Select(x => x.CheckOutTime).Max();

            if (minCheckInDate.Date != bookingModel.CheckInTime.Date)
                bookingModel.CheckInTime = minCheckInDate;

            if (maxCheckOutDate.Date != bookingModel.CheckOutTime.Date)
                bookingModel.CheckOutTime = maxCheckOutDate;
        }

        if (vm.BookingGuestVms?.Count > 0 is false)
            throw new Exception("No Guest Information Found...!!");

        var mainGuest = vm.BookingGuestVms.Any(x => x.IsMain == true);
        if (mainGuest == false)
            throw new Exception("No Bill Guest Information Found...!!");

        var bookingGuests = _iMapper.Map<List<HtBookingGuest>>(vm.BookingGuestVms);

        if (bookingGuests.Count > 0)
        {
            foreach (var guest in bookingGuests)
            {
                var guestInfo = _iGuestInfoRepository.GetFirstOrDefault(x => x.Id == guest.GuestId && !x.IsDeleted);
                if (guestInfo == null)
                    throw new Exception("Guest Not Found...!!");

                mainGuestInfo = guest.IsMain ? guestInfo : mainGuestInfo;

                guest.ActionDate = Utility.GetBdDateTimeNow();
                guest.ActionById = CurrentUserId;
            }
        }

        HtBookingPayment paymentModel = null;
        AccTranMst paymentVoucher = null;

        if (vm.PaymentVm != null && vm.PaymentVm.PaidAmount > 0)
        {
            paymentModel = _iMapper.Map<HtBookingPayment>(vm.PaymentVm);

            paymentModel.PaidDate = (DateTime)(!string.IsNullOrEmpty(vm.PaidDateStr) ? Utility.ConvertStrToDate(vm.PaidDateStr) : DU.Utility.GetBdDateTimeNow());
            paymentModel.Description = $"Advance Amount {vm.PaymentVm.PaidAmount} is paid when room is booked..";
            paymentModel.ActionById = CurrentUserId;
            paymentModel.ActionDate = Utility.GetBdDateTimeNow();
            paymentModel.IsAdvance = true;
            paymentModel.ReportDate = reportDate;

            if (bookingModel.NetRent < paymentModel.PaidAmount)
                throw new Exception("Paid Amount Is Higher Than Net Amount...!!");

            //if (bookingModel.NetRent == paymentModel.PaidAmount)
            //    bookingModel.PaymentStatus = PaymentStatusEnum.FullPayment;
            //else if (bookingModel.NetRent > paymentModel.PaidAmount)
            //    bookingModel.PaymentStatus = PaymentStatusEnum.PartialPayment;

            bookingModel.PaymentStatus = PaymentStatusEnum.PartialPayment;

            //paymentVoucher = await GetAdvancePaymentVoucher(paymentModel, bookingModel.BookingNo);//for standard accounting system
            var novDate = new DateTime(2024, 11, 29);
            if (paymentModel.PaidDate.Date > novDate.Date)
            {
                paymentVoucher = await GetAdvancePaymentQuickVoucher(paymentModel, bookingModel.BookingNo);//for quick voucher system
                if (paymentVoucher == null)
                    throw new Exception("Somthing Went Wrong Creating Voucher..!!");
            }
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iRepository.AddAsync(bookingModel);
        await _iUnitOfWork.CompleteAsync();

        bookingRooms.ForEach(x => x.BookingId = bookingModel.Id);
        bookingGuests.ForEach(x => x.BookingId = bookingModel.Id);

        await _iBookingRoomRepository.AddRangeAsync(bookingRooms);
        await _iBookingGuestRepository.AddRangeAsync(bookingGuests);

        if (paymentModel != null)
        {
            paymentModel.BookingId = bookingModel.Id;
            await _iBookingPaymentRepository.AddAsync(paymentModel);
            await _iUnitOfWork.CompleteAsync();
        }

        if (paymentVoucher != null)
        {
            paymentVoucher.PaymentId = paymentModel.Id;
            await _iAccTranMstRepository.AddAsync(paymentVoucher);
        }

        if (bookingRoomList?.Count > 0)
        {
            _iRoomInfoRepository.UpdateRange(bookingRoomList);
        }

        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return (false, 0); }

        string notificationMsg = BookingNotificationMsg(bookingModel, bookingRoomList, $"{mainGuestInfo?.FirstName} {mainGuestInfo?.LastName}", paymentModel?.PaidAmount ?? 0);

        var ntfGenerated = await _iNtfMsgService.GenerateNtf(NotificationEventCode.BookingNtf, notificationMsg, notificationMsg);

        ts.Complete();
        return (true, bookingModel.Id);
    }

    #endregion

    #region Search

    public async Task<DataTablePagination<BookingServiceSearchVm, BookingServiceSearchVm>>
        SearchAsync(DataTablePagination<BookingServiceSearchVm, BookingServiceSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }
    #endregion

    #region ReservationBillHtml

    public async Task<string> ReservationBillHtml(long bookingId)
    {
        var data = await _iRepository.GetBookingByIdAsync(bookingId);
        var reservedRooms = await GetBookedRoomListByBookingId(bookingId);
        var guestList = await GetBookingGuestListByBookingId(bookingId);

        var guestInfo = guestList.FirstOrDefault(x => x.IsMain);
        var guestName = guestInfo != null && guestInfo.Gender == "M" ? $@"Sir, {guestInfo.GuestName}" : guestInfo != null && guestInfo.Gender == "F" ? $@"Madam, {guestInfo.GuestName}" : $"{guestInfo?.GuestName}";

        var nights = (data.CheckOutTime.Date - data.CheckInTime.Date).Days;

        var fullHtml = "";
        fullHtml += @$"<div>";
        fullHtml += @$"<div style='padding-top:0px;font-size:12px;'><b>Reservation Letter</b></div>";
        fullHtml += @$"<hr style='border: 1px solid gray; color: gray; margin-bottom:0px;'/>";

        fullHtml += "<table  style='width:100%;text-align:center;font-size:11px'>";
        fullHtml += "<tbody>";

        fullHtml += "<tr style='height: 20px'>";
        fullHtml += $@"<td class='text-start' style='width:60%;'><b>Reservation No. : {data.BookingNo} </b></td>";
        fullHtml += $@"<td class='text-start' style='width:40%;'>Print Date : {DateTime.Today.Date.ToString("dd/MM/yyyy")}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 20px'>";
        fullHtml += $@"<td class='text-start' style='width:60%;'>Kind Attention : {guestName}</td>";
        fullHtml += $@"<td class='text-start' style='width:40%;'>Organization : {guestInfo.CompanyName ?? ""}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 20px'>";
        fullHtml += $@"<td class='text-start' style='width:60%;'>Email : {guestInfo.Email ?? ""}</td>";
        fullHtml += $@"<td class='text-start' style='width:40%;'>Address : {guestInfo.Address ?? ""}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 20px'>";
        fullHtml += $@"<td class='text-start' style='width:60%;'>Mobile : {guestInfo.GuestMobile ?? ""}</td>";
        fullHtml += $@"<td class='text-start' style='width:40%;'>Reservation Creator : {data.ActionByName ?? ""}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 20px'>";
        fullHtml += $@"<td class='text-start' style='width:60%;'></td>";
        fullHtml += $@"<td class='text-start' style='width:40%;'>Reservation Date : {data.BookingDate.ToString("dd/MMM/yyyy")}</td>";
        fullHtml += "</tr>";

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        //Subject Section
        fullHtml += @$"<div style='font-size:10px;'><b>Subject: Room Reservation Letter</b></div>";
        fullHtml += @$"<div style='padding-top:2px;font-size:10px;'>We truely appritiate your kind patronage in choosing <b>Hotel Mishuk</b>.Please refer to the details of your reservation outlined below.</div>";

        //Reservation & guest Info Section
        fullHtml += @$"<div style='padding-top:10px;font-size:10px;'><b>Reservation Details :-</b></div>";
        fullHtml += @$"<hr style='border: 1px solid gray; color: gray; margin-bottom:0px;'/>";

        fullHtml += "<table  style='width:100%;text-align:center;font-size:11px'>";
        fullHtml += "<tbody>";

        fullHtml += "<tr style='height: 20px'>";
        fullHtml += $@"<td class='text-start' style='width:60%;'><b>Guest Name : {guestInfo.GuestName} </b></td>";
        fullHtml += $@"<td class='text-start' style='width:40%;'><b>Phone : {guestInfo.GuestMobile ?? ""} </b></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 20px'>";
        fullHtml += $@"<td class='text-start' style='width:60%;'><b>Room Night : {nights} </b></td>";
        fullHtml += $@"<td class='text-start' style='width:40%;'><b>Adult + Child : {data.Adult} + {data.Child} = {data.TotalGuest} </b></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 20px'>";
        fullHtml += $@"<td class='text-start' style='width:60%;'><b>Arrival Date : {data.CheckInTime.ToString("dd/MMM/yyyy")} </b></td>";
        fullHtml += $@"<td class='text-start' style='width:40%;'><b>Departure Date : {data.CheckOutTime.ToString("dd/MMM/yyyy")} </b></td>";
        fullHtml += "</tr>";

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center;font-size:11px'>";
        fullHtml += "<thead>";
        fullHtml += "<tr>";
        fullHtml += "<th style='width:25%; font-size: 10px'>Type Of Room</th>";
        fullHtml += "<th style='width:15%; font-size: 10px'>Arrival Date</th>";
        fullHtml += "<th style='width:15%; font-size: 10px'>Departure Date</th>";
        fullHtml += "<th style='width:20%; font-size: 10px'>Room Rent (After Dis.)</th>";
        fullHtml += "<th style='width:10%; font-size: 10px'>No Of Room</th>";
        fullHtml += "<th style='width:10%; font-size: 10px'>Room Night(s)</th>";
        fullHtml += "<th style='width:10%; font-size: 10px'>Total Room Rent</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        if (reservedRooms.Count > 0)
        {
            // Group by Category, CheckIn, CheckOut
            var groupedRooms = reservedRooms
                .GroupBy(x => new { x.RoomCategoryName, x.CheckInTime, x.CheckOutTime })
                .Select(g => new
                {
                    RoomCategoryName = g.Key.RoomCategoryName,
                    CheckInTime = g.Key.CheckInTime,
                    CheckOutTime = g.Key.CheckOutTime,
                    RoomRentAfterDiscount = (g.FirstOrDefault().RoomRent - g.FirstOrDefault().RoomDiscount),
                    Quantity = g.Count(),
                    Days = g.FirstOrDefault().Days,
                    NetRent = g.Sum(x => x.NetRent)
                })
                .ToList();

            foreach (var item in groupedRooms.Select((val, i) => new { val, i }))
            {
                fullHtml += "<tr>";

                fullHtml += $@"<td class='text-start'>{item.val.RoomCategoryName} <br/></td>";
                fullHtml += $@"<td class='text-center'>{item.val.CheckInTime:dd/MMM/yyyy}</td>";
                fullHtml += $@"<td class='text-center'>{item.val.CheckOutTime:dd/MMM/yyyy}</td>";
                fullHtml += $@"<td class='text-center'>{item.val.RoomRentAfterDiscount:F2}</td>";
                fullHtml += $@"<td class='text-center'>{item.val.Quantity}</td>";
                fullHtml += $@"<td class='text-center'>{item.val.Days}</td>";
                fullHtml += $@"<td class='text-end'>{item.val.NetRent:N2}</td>";

                fullHtml += "</tr>";
            }

            // Totals
            fullHtml += "<tr>";
            fullHtml += $@"<td colspan='6' class='text-end'><b>Total</b></td>";
            fullHtml += $@"<td class='text-end'>{data.NetRent:N2}</td>";
            fullHtml += "</tr>";

            fullHtml += "<tr>";
            fullHtml += $@"<td colspan='6' class='text-end'><b>Advance</b></td>";
            fullHtml += $@"<td class='text-end'>{data.PaidAmount:N2}</td>";
            fullHtml += "</tr>";

            var dueAmount = data.NetRent > data.PaidAmount ? (data.NetRent - data.PaidAmount) : 0;

            fullHtml += "<tr>";
            fullHtml += $@"<td colspan='6' class='text-end'><b>Due</b></td>";
            fullHtml += $@"<td class='text-end'>{dueAmount:N2}</td>";
            fullHtml += "</tr>";
        }


        fullHtml += "</tbody>";
        fullHtml += "</table>";

        fullHtml += "<div class='row' style='padding-top:10px'>";

        //fullHtml += "<div style='font-size:10px; font-weight:bold; text-decoration:underline;'><h6>Hotel Policy:</h6></div>";

        fullHtml += "<table style='font-size:10px;width:100%;border-collapse:collapse;margin-top:3px;'>";

        fullHtml += "<tr><td colspan='2' valign='top' style='font-size:12px;font-weight:bold;text-decoration:underline;padding-bottom:12px;'> Hotel Policy: </td></tr>";

        fullHtml += "<tr><td valign='top' width='3%' style='padding-bottom:7px;'>1.</td><td style='padding-bottom:7px;'>Complimentary Services: Buffet breakfast, High-speed Wi-Fi etc.</td></tr>";
        fullHtml += "<tr><td valign='top' style='padding-bottom:7px;'>2.</td><td style='padding-bottom:7px;'>To confirm the booking, 50% advance deposit is required within 24 hours. All bills must be settled upon departure.</td></tr>";
        fullHtml += "<tr><td valign='top' style='padding-bottom:7px;'>3.</td><td style='padding-bottom:7px;'>This is mandatory for all guests to provide National ID/ Passport during Check-in time.</td></tr>";
        fullHtml += "<tr><td valign='top' style='padding-bottom:7px;'>4.</td><td style='padding-bottom:7px;'>Check-in time after 12.00 pm and check-out time at 11.00 am.</td></tr>";
        fullHtml += "<tr><td valign='top' style='padding-bottom:7px;'>5.</td><td style='padding-bottom:7px;'>Cancellation and amendments must be made at least 3 days before of your original arrival date.</td></tr>";
        fullHtml += "<tr><td valign='top' style='padding-bottom:7px;'>6.</td><td style='padding-bottom:7px;'>We do not offer refund, but you can amend your reservation to any date within next three months, for your new reservation date, discount rates for that particular month will apply.</td></tr>";
        fullHtml += "<tr><td valign='top' style='padding-bottom:7px;'>7.</td><td style='padding-bottom:7px;'>Children aged below 5 Years are complimentary. For 5-10 years 500 BDT (Breakfast + Swimming pool) will be charged. For above 10 years extra bed is mandatory, extra bed charge will be 500 BDT (Including Breakfast + Swimming pool).</td></tr>";


        fullHtml += "</table>";

        fullHtml += "</div>";
        fullHtml += "</div>";





        return fullHtml;
    }

    #endregion

    #region ReservationCardHtml

    public async Task<string> ReservationCardHtml(long bookingId)
    {
        var data = await _iRepository.GetBookingByIdAsync(bookingId);
        var reservedRooms = await GetBookedRoomListByBookingId(bookingId);
        var guestList = await GetBookingGuestListByBookingId(bookingId);

        var guestInfo = guestList.FirstOrDefault(x => x.IsMain);
        //var guestName = guestInfo != null && guestInfo.Gender == "M" ? $@"Sir, {guestInfo.GuestName}" : guestInfo != null && guestInfo.Gender == "F" ? $@"Madam, {guestInfo.GuestName}" : $"{guestInfo?.GuestName}";
        var genderText = guestInfo != null && guestInfo.Gender == "M" ? $@"Male" : guestInfo != null && guestInfo.Gender == "F" ? $@"Female" : "";

        var nights = (data.CheckOutTime.Date - data.CheckInTime.Date).Days;

        var fullHtml = "";
        fullHtml += @$"<div style='border: 1px solid black;'>";
        fullHtml += @$"<div style='width:100%;padding-top:0px;font-size:12px'>";
        fullHtml += "<table style='font-size:10px;width:100%;border-collapse:collapse;margin-top:3px;'>";

        fullHtml += @$"<tr><td valign='top' width='60%' style='padding-bottom:7px;font-size:15px;text-align:right;'> <b>Registration Card</b> </td><td width='40%' style='padding-bottom:7px; text-align:right;'><b> Print Date : {DateTime.Now.ToString("dd/MMM/yyyy HH:mm")} </b></td></tr>";

        fullHtml += "</table>";
        fullHtml += @$"</div>";

        fullHtml += "<table  style='width:100%;text-align:center;font-size:11px'>";
        fullHtml += "<tbody>";

        fullHtml += "<tr style='height: 30px'>";
        fullHtml += $@"<td class='text-start' style='width:50%;display:flex;align-self:end;font-size:10px;'> <b>Registration No:</b> {data.BookingNo}</td>";
        fullHtml += $@"<td class='text-end' style='width:50%;font-size:10px; padding-left:30px;'><b>Arrival Date : </b> {data.CheckInTime.Date.ToString("dd-MMM-yyyy")}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px'>";
        fullHtml += $@"<td class='text-start' style='width:50%;display:flex;align-self:end;font-size:10px;'></td>";
        fullHtml += $@"<td class='text-end' style='width:50%;font-size:10px; padding-left:30px;'><b>Departure Date : </b> {data.CheckOutTime.Date.ToString("dd-MMM-yyyy")}</td>";
        fullHtml += "</tr>";

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        //Reservation & guest Info Section
        fullHtml += @$"<div style='padding:10px 0px 10px;font-size:12px;'><b>Personal Information:-</b></div>";

        fullHtml += "<table  style='width:100%;text-align:center;font-size:11px;'>";
        fullHtml += "<tbody>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left; padding-right:10px;'><b>Guest Name :</b></td>";
        fullHtml += $@"<td style='width:35%;text-align:left;'>{guestInfo?.GuestName}<hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left; padding-left:20px;'><b>Address :</b></td>";
        fullHtml += $@"<td style='width:35%;text-align:left;'>{guestInfo?.Address}<hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left;padding-right:10px;'><b>Phone No :</b></td>";
        fullHtml += $@"<td style='width:35%;text-align:left;'>{guestInfo?.GuestMobile}<hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left; padding-left:20px;'><b>Email :</b></td>";
        fullHtml += $@"<td style='width:35%;text-align:left;'>{guestInfo?.Email}<hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left;padding-right:10px;'><b>Date of Birth :</b></td>";
        fullHtml += $@"<td style='width:35%;text-align:left;'>{guestInfo?.Dob?.ToString("dd-MMM-yyyy")}<hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left; padding-left:20px;'><b>Gender :</b></td>";
        fullHtml += $@"<td style='width:35%;text-align:left;'>{genderText}<hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left; padding-right:10px;'><b>Country :</b></td>";
        fullHtml += $@"<td style='width:35%;text-align:left;'>{guestInfo?.Country}<hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left;padding-left:20px;'><b>City :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left; padding-right:10px;'><b>Zip Code :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left;padding-left:20px;'><b>Nationality :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left;padding-right:10px;'><b>Profession :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left; padding-left:20px;'><b>Visit Purpose :</b></td>";
        fullHtml += $@"<td style='width:35%;text-align:left;'>{data.VisitPurpose}<hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left;padding-right:10px;'><b>NID/ Diriving License :</b></td>";
        fullHtml += $@"<td style='width:35%;text-align:left;'>{guestInfo.IdentityNo + "-" + (guestInfo.IdentityType)}<hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left; padding-left:20px;'><b>Pass. Number :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left;padding-right:10px;'><b>Place Of Issue :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left; padding-left:20px;'><b>Pass Issue Date :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left;padding-right:10px;'><b>Visa Number :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left; padding-left:20px;'><b>Visa Issue Date :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left;padding-right:10px;'><b>Visa Date of Expiry :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        //Room Info section

        fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center;font-size:11px;margin-top:10px'>";
        fullHtml += "<thead>";

        fullHtml += $@"<tr><th class='text-start' colspan='7' style='font-size:14px;border-top:none !important;'><b>Accommodation</b></th></tr>";

        fullHtml += "<tr>";
        fullHtml += "<th style='width:25%; font-size: 10px'>Type Of Room</th>";
        fullHtml += "<th style='width:15%; font-size: 10px'>Arrival Date</th>";
        fullHtml += "<th style='width:15%; font-size: 10px'>Departure Date</th>";
        fullHtml += "<th style='width:20%; font-size: 10px'>Room Rent (After Dis.)</th>";
        fullHtml += "<th style='width:10%; font-size: 10px'>No Of Room</th>";
        fullHtml += "<th style='width:10%; font-size: 10px'>Room Night(s)</th>";
        fullHtml += "<th style='width:10%; font-size: 10px'>Total Room Rent</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        if (reservedRooms.Count > 0)
        {
            var groupedRooms = reservedRooms
                .GroupBy(x => new { x.RoomCategoryName, CheckInDate = x.CheckInTime.Date, CheckOutDate = x.CheckOutTime.Date })
                .Select(g => new
                {
                    RoomCategoryName = $"{g.Key.RoomCategoryName} <br/> Room#{string.Join(", ", g.Where(x => x.ActualCheckInTime != null).Select(x => x.RoomNo))}",
                    CheckInTime = g.Key.CheckInDate,
                    CheckOutTime = g.Key.CheckOutDate,
                    RentAfterDiscount = (g.First().RoomRent - g.First().RoomDiscount),
                    Quantity = g.Count(),
                    Days = g.First().Days,
                    NetRent = g.Sum(x => x.NetRent)
                }).ToList();

            foreach (var item in groupedRooms.Select((val, i) => new { val, i }))
            {
                fullHtml += "<tr>";

                fullHtml += $@"<td class='text-start'> {item.val.RoomCategoryName} <br/></td>";
                fullHtml += $@"<td class='text-center'>{item.val.CheckInTime:dd/MMM/yyyy}</td>";
                fullHtml += $@"<td class='text-center'>{item.val.CheckOutTime:dd/MMM/yyyy}</td>";
                fullHtml += $@"<td class='text-center'>{item.val.RentAfterDiscount:F2}</td>";
                fullHtml += $@"<td class='text-center'>{item.val.Quantity}</td>";
                fullHtml += $@"<td class='text-center'>{item.val.Days}</td>";
                fullHtml += $@"<td class='text-end'>{item.val.NetRent:N2}</td>";

                fullHtml += "</tr>";
            }

            // Totals
            fullHtml += "<tr>";
            fullHtml += $@"<td colspan='6' class='text-end'><b>Total</b></td>";
            fullHtml += $@"<td class='text-end'>{data.NetRent:N2}</td>";
            fullHtml += "</tr>";

            fullHtml += "<tr>";
            fullHtml += $@"<td colspan='6' class='text-end'><b>Advance</b></td>";
            fullHtml += $@"<td class='text-end'>{data.PaidAmount:N2}</td>";
            fullHtml += "</tr>";

            var dueAmount = data.NetRent > data.PaidAmount ? (data.NetRent - data.PaidAmount) : 0;

            fullHtml += "<tr>";
            fullHtml += $@"<td colspan='6' class='text-end'><b>Due</b></td>";
            fullHtml += $@"<td class='text-end'>{dueAmount:N2}</td>";
            fullHtml += "</tr>";
        }
        fullHtml += "</tbody>";
        fullHtml += "</table>";

        fullHtml += "<div class='row' style='padding-top:10px'>";


        fullHtml += "<table class='table table-bordered' style='font-size:13px;width:50%;border-collapse:collapse;margin-bottom:10px;'>";

        fullHtml += "<tr>";
        fullHtml += $@"<td class='text-start' style='width:7%;'><b>Adult</b></td>";
        fullHtml += $@"<td class='text-start' style='width:18%;color:white;'>0</td>";
        fullHtml += $@"<td class='text-start' style='width:7%;'><b>Child</b></td>";
        fullHtml += $@"<td class='text-start' style='width:18%;color:white;border-top:none;'>0</td>";
        fullHtml += "</tr>";

        fullHtml += "</table>";
        fullHtml += "<table style='font-size:13px;width:100%;border-collapse:collapse;margin-top:3px;'>";

        fullHtml += "<tr><td colspan='2' valign='top' style='font-size:14px;font-weight:bold;text-decoration:underline;padding-bottom:12px;'> Terms and Conditions: </td></tr>";
        fullHtml += "<tr><td colspan='2' valign='top' style='font-size:14px;padding-bottom:12px;'> PLEASE READ CAREFULLY & SIGN - Terms & Conditions of Your Stay </td></tr>";

        fullHtml += "<tr><td valign='top' width='3%' style='padding-bottom:7px;'>1.</td><td style='padding-bottom:7px;'> Hotel Can not be held responsible for loss of items & maluables left in the room or public places, please use the sage deposit facilities at the reception. For future information on terms and condition please ask reception for details</td></tr>";
        fullHtml += "<tr><td valign='top' style='padding-bottom:7px;'>2.</td><td style='padding-bottom:7px;'> Lost or damaged KEY card or KEY will incur a replacement fee of 500 (BDT) or equivalent at current exchange rate.</td></tr>";

        fullHtml += "</table>";

        fullHtml += "</div>";
        fullHtml += "</div>";

        return fullHtml;
    }

    #endregion

    #region EmptyReservationCard
    public async Task<string> ReservationCardHtmlEmpty()
    {
        var fullHtml = "";
        fullHtml += @$"<div style='border: 1px solid black;'>";
        fullHtml += @$"<div style='width:100%;padding-top:0px;font-size:12px'>";
        fullHtml += "<table style='font-size:10px;width:100%;border-collapse:collapse;margin-top:3px;'>";

        fullHtml += "<tr>";

        fullHtml += "<td width='40%' style='padding-bottom:7px;'></td>";
        fullHtml += "<td valign='top' width='26%' style='padding-bottom:7px;font-size:15px;text-align:center;'> <b>Registration Card</b> </td>";

        fullHtml += $@"<td class='text-end' style='width:20%;font-size:10px; padding-left:30px; text-align: right;'><b>Print Date : </b></td>";
        fullHtml += $@"<td class='text-end' style='width:14%;font-size:10px; text-align: left;'>{DateTime.Today.Date.ToString("dd/MMM/yy")}</td>";

        fullHtml += "</tr>";

        fullHtml += "</table>";
        fullHtml += @$"</div>";

        fullHtml += "<table style='width:100%;text-align:center;font-size:11px; border-collapse: collapse;'>";
        fullHtml += "<tbody>";

        fullHtml += "<tr style='height: 30px'>";
        fullHtml += $@"<td class='text-start' style='width:50%;font-size:10px; text-align: left;'> <b>Registration No:</b></td>";

        fullHtml += $@"<td class='text-end' style='width:36%;font-size:10px; padding-left:30px; text-align: right;'><b>Arrival Date : </b></td>";

        fullHtml += $@"<td class='text-end' style='width:14%;font-size:10px; text-align: left;'> ........./........../............</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px'>";
        fullHtml += $@"<td class='text-start' style='width:50%;font-size:10px;'></td>";

        fullHtml += $@"<td class='text-end' style='width:36%;font-size:10px; padding-left:30px; text-align: right;'><b>Departure Date : </b></td>";

        fullHtml += $@"<td class='text-end' style='width:14%;font-size:10px; text-align: left;'> ........./........../............</td>";
        fullHtml += "</tr>";

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        fullHtml += @$"<div style='padding:10px 0px 10px;font-size:12px;'><b>Personal Information:-</b></div>";

        fullHtml += "<table  style='width:100%;text-align:center;font-size:11px;'>";
        fullHtml += "<tbody>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left; padding-right:10px;'><b>Guest Name :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left; padding-left:20px;'><b>Address :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left;padding-right:10px;'><b>Phone No :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left; padding-left:20px;'><b>Email :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left;padding-right:10px;'><b>Date of Birth :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left; padding-left:20px;'><b>Gender :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left; padding-right:10px;'><b>Country :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left;padding-left:20px;'><b>City :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left; padding-right:10px;'><b>Zip Code :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left;padding-left:20px;'><b>Nationality :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:13px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left;padding-right:10px;'><b>Profession :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left; padding-left:20px;'><b>Visit Purpose :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left;padding-right:10px;'><b>NID/ Diriving License :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left; padding-left:20px;'><b>Pass. Number :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left;padding-right:10px;'><b>Place Of Issue :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left; padding-left:20px;'><b>Pass Issue Date :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left;padding-right:10px;'><b>Visa Number :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";

        fullHtml += "<td style='width:15%; text-align:left; padding-left:20px;'><b>Visa Issue Date :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr style='height: 30px;'>";
        fullHtml += "<td style='width:15%; text-align:left;padding-right:10px;'><b>Visa Date of Expiry :</b></td>";
        fullHtml += "<td style='width:35%;'><hr style='border:none; border-top:1px solid black; margin:5px 0 0 0;'/></td>";
        fullHtml += "</tr>";

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center;font-size:11px;margin-top:10px'>";
        fullHtml += "<thead>";

        fullHtml += $@"<tr><th class='text-start' colspan='5' style='font-size:14px;border-top:none !important;'><b>Accommodation</b></th></tr>";

        fullHtml += "<tr>";
        fullHtml += "<th style='width:25%; font-size: 10px'>Type Of Room</th>";
        fullHtml += "<th style='width:20%; font-size: 10px'>Room Rent (After Dis.)</th>";
        fullHtml += "<th style='width:15%; font-size: 10px'>No Of Room</th>";
        fullHtml += "<th style='width:15%; font-size: 10px'>Room Night(s)</th>";
        fullHtml += "<th style='width:25%; font-size: 10px'>Total Room Rent</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        for (int i = 0; i < 5; i++)
        {
            fullHtml += "<tr>";
            fullHtml += "<td style='width:25%; font-size: 10px'></td>";
            fullHtml += "<td style='width:20%; font-size: 10px'></td>";
            fullHtml += "<td style='width:15%; font-size: 10px'></td>";
            fullHtml += "<td style='width:15%; font-size: 10px'></td>";
            fullHtml += "<td style='width:25%; font-size: 10px'></td>";
            fullHtml += "</tr>";
        }

        fullHtml += "<tr>";
        fullHtml += $@"<td colspan='4' class='text-end'><b>Total</b></td>";
        fullHtml += $@"<td class='text-end'></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $@"<td colspan='4' class='text-end'><b>Advance</b></td>";
        fullHtml += $@"<td class='text-end'></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $@"<td colspan='4' class='text-end'><b>Due</b></td>";
        fullHtml += $@"<td class='text-end'></td>";
        fullHtml += "</tr>";

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        fullHtml += "<div class='row' style='padding-top:10px'>";

        fullHtml += "<table class='table table-bordered' style='font-size:13px;width:50%;border-collapse:collapse;margin-bottom:10px;'>";

        fullHtml += "<tr>";
        fullHtml += $@"<td class='text-start' style='width:7%;'><b>Adult</b></td>";
        fullHtml += $@"<td class='text-start' style='width:18%;font-size: 10px''></td>";
        fullHtml += $@"<td class='text-start' style='width:7%;'><b>Child</b></td>";
        fullHtml += $@"<td class='text-start' style='width:18%;font-size: 10px';border-top:none;'></td>";
        fullHtml += "</tr>";

        fullHtml += "</table>";
        fullHtml += "<table style='font-size:13px;width:100%;border-collapse:collapse;margin-top:3px;'>";

        fullHtml += "<tr><td colspan='2' valign='top' style='font-size:14px;font-weight:bold;text-decoration:underline;padding-bottom:12px;'> Terms and Conditions: </td></tr>";
        fullHtml += "<tr><td colspan='2' valign='top' style='font-size:14px;padding-bottom:12px;'> PLEASE READ CAREFULLY & SIGN - Terms & Conditions of Your Stay </td></tr>";

        fullHtml += "<tr><td valign='top' width='3%' style='padding-bottom:7px;'>1.</td><td style='padding-bottom:7px;'> Hotel Can not be held responsible for loss of items & maluables left in the room or public places, please use the sage deposit facilities at the reception. For future information on terms and condition please ask reception for details</td></tr>";
        fullHtml += "<tr><td valign='top' style='padding-bottom:7px;'>2.</td><td style='padding-bottom:7px;'> Lost or damaged KEY card or KEY will incur a replacement fee of 500 (BDT) or equivalent at current exchange rate.</td></tr>";

        fullHtml += "</table>";

        fullHtml += "</div>";
        fullHtml += "</div>";

        return fullHtml;
    }
    #endregion

    #region GetBookingInfo

    public async Task<HtBookingServiceVm> GetBookingInfoById(long bookingId)
    {
        var data = await _iRepository.GetBookingByIdAsync(bookingId);

        // for mishuk old already checked in guest
        if (data.BookingStatus == BookingServiceStatusEnum.CheckIn && !data.IsBillGenerated)
        {
            _iBillService.CurrentUserId = CurrentUserId;
            await _iBillService.GenerateInitialBill(data.Id);
        }

        return data;
    }

    #endregion

    #region GetBookingCode

    public async Task<string> GetBookingCode()
    {
        var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.HtBookingServices.ToString(), "BookingNo", "MSK", 6);
        return data;
    }

    #endregion

    #region BookedRoomList

    public async Task<List<HtBookingRoomVm>> GetBookedRoomListByBookingId(long bookingId)
    {
        if (bookingId < 0)
            throw new Exception("booking info not found...!");

        var bookingRooms = await _iBookingRoomRepository.GetBookedRoom(bookingId);
        var result = _iMapper.Map<List<HtBookingRoomVm>>(bookingRooms);

        if (result.Count > 0)
        {
            var bookingRoomIds = result.Where(x => x.ActualCheckInTime != null).Select(c => c.Id).ToList();
            var billRoomServiceList = await _iBillingDetailRepository.GetAsync(x => bookingRoomIds.Contains((long)x.BookingRoomId) && x.ServiceDate != null, s => s.Service);

            foreach (var room in result)
            {
                //var days = AppUtility.DaysDiffernce(room.CheckOutTime, room.CheckInTime);
                double days = AppUtility.DaysDiffernceOnlyDate(room.CheckOutTime, room.CheckInTime);

                if (room.BookingDayStatus == 1)
                {
                    days = days + 0.5;
                    room.RoomDiscount = room.Discount / days;
                }
                else if (room.BookingDayStatus == 2)
                {
                    days = days + 1;
                    room.RoomDiscount = room.Discount / days;
                }
                else
                {
                    room.RoomDiscount = days > 0 ? room.Discount / days : 0;
                }
                room.Days = days;
                room.Adult = room.Adult > 0 ? room.Adult : room.TotalGuest;

                var roomExtraBedAsServiceList = billRoomServiceList.Where(x => x.Service.ServiceCode == HtServiceCode.ExtraBed && x.BookingRoomId == room.Id).ToList();
                room.ExtraBedAsServiceCharge = roomExtraBedAsServiceList.Sum(x => x.NetAmount);
            }
        }

        result = result.OrderBy(x => x.RoomNo).ToList();

        return result;
    }

    #endregion

    #region BookingGuestList

    public async Task<List<HtBookingGuestVm>> GetBookingGuestListByBookingId(long bookingId)
    {
        if (bookingId < 0)
            throw new Exception("booking info not found...!");

        var bookingGuests = await _iBookingGuestRepository.GetAsync(x => x.BookingId == bookingId && !x.IsDeleted, g => g.Guest.Company, g => g.Guest.Country);
        var dataList = _iMapper.Map<List<HtBookingGuestVm>>(bookingGuests);

        if (dataList.Count > 0)
        {
            foreach (var item in dataList)
            {
                var filterData = bookingGuests.FirstOrDefault(x => x.Id == item.Id);

                item.GuestName = $"{filterData.Guest.Salutation} {filterData.Guest.FirstName} {filterData.Guest.LastName}";
                item.GuestMobile = filterData.Guest.Mobile;
                item.GuestId = filterData.Guest.Id;
                item.Email = filterData.Guest.Email;
                item.Address = filterData.Guest.Address;
                item.CompanyName = filterData.Guest?.Company?.Name;
                item.CompanyId = filterData.Guest?.Company?.Id;
                item.DistrictId = filterData.Guest?.DistrictId;
                item.Gender = filterData.Guest?.Gender;
                item.Dob = filterData.Guest?.Dob;
                item.Country = filterData.Guest?.Country?.Name;
                item.CountryId = filterData.Guest?.Country?.Id;
                item.IdentityNo = filterData.Guest?.IdentityNo;
                item.IdentityType = filterData.Guest?.IdentityType;
                item.IsVip = filterData.Guest.IsVip;
            }
        }

        return dataList;
    }

    #endregion

    #region UpdateBooking

    public async Task<bool> UpdateBooking(HtBookingServiceVm vm)
    {
        if (vm == null && !(vm.Id > 0))
            throw new Exception("Booking Service Information Is Not Correct...!!");

        var bookingService = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == vm.Id && x.BookingType == BookingType.Room && !x.IsDeleted);

        if (bookingService == null)
            throw new Exception("Booking Service Not Found...!!");

        if (bookingService.BookingStatus == BookingServiceStatusEnum.CheckOut)
            throw new Exception("Booking Service Already Checked Out...!!");

        bookingService.CheckInTime = (DateTime)(!string.IsNullOrEmpty(vm.CheckInTimeStr) ? Utility.ConvertStrToDate(vm.CheckInTimeStr) : DU.Utility.GetBdDateTimeNow());
        bookingService.CheckOutTime = (DateTime)(!string.IsNullOrEmpty(vm.CheckOutTimeStr) ? Utility.ConvertStrToDate(vm.CheckOutTimeStr) : DU.Utility.GetBdDateTimeNow());
        bookingService.VisitPurpose = vm.VisitPurpose;
        bookingService.UpdatedById = CurrentUserId;
        bookingService.UpdateDate = Utility.GetBdDateTimeNow();

        if (bookingService.BookingDate.Date > bookingService.CheckInTime.Date)
            throw new Exception("Check-In Date Is Previous Date Than Booking Date..!!");

        if (bookingService.CheckInTime.Date > bookingService.CheckOutTime.Date)
            throw new Exception("CheakIn & CheckOut Date Is Not Correct..!!");

        #region Room

        List<HtBookingRoom> addableRoomList = null;
        List<HtBookingRoom> updateableRoomList = null;
        List<HtBookingRoom> deletableRoomList = null;

        bookingService.Rent = 0;
        bookingService.ServiceCharge = 0;
        bookingService.Discount = 0;
        bookingService.NetRent = 0;

        var checkInDateList = new List<DateTime>();
        var checkOutDateList = new List<DateTime>();

        var existBookingRooms = await _iBookingRoomRepository.GetAsync(x => x.BookingId == vm.Id && !x.IsDeleted);

        if (vm?.BookingRoomVms?.Count > 0)
        {
            var dataListForAdd = vm?.BookingRoomVms?.Where(c => c.Id == 0).ToList();

            var updatableItemIds = vm?.BookingRoomVms.Where(c => c.Id > 0).Select(c => c.Id).ToList();
            updateableRoomList = (existBookingRooms.Where(x => updatableItemIds.Contains(x.Id))).ToList();

            if (updateableRoomList?.Count > 0)
            {
                foreach (var updateRoom in updateableRoomList)
                {
                    var filterData = vm.BookingRoomVms.Where(c => c.Id == updateRoom.Id).FirstOrDefault();

                    updateRoom.CheckInTime = (DateTime)(!string.IsNullOrEmpty(filterData.CheckInTimeStr) ? Utility.ConvertStrToDate(filterData.CheckInTimeStr) : bookingService.CheckInTime);
                    updateRoom.CheckOutTime = (DateTime)(!string.IsNullOrEmpty(filterData.CheckOutTimeStr) ? Utility.ConvertStrToDate(filterData.CheckOutTimeStr) : bookingService.CheckOutTime);
                    updateRoom.Adult = filterData.Adult ?? 0;
                    updateRoom.Child = filterData.Child ?? 0;
                    updateRoom.Discount = filterData.Discount;

                    if (updateRoom.CheckInTime.Date > updateRoom.CheckOutTime.Date)
                        throw new Exception("Room CheakIn & CheckOut Date Is Not Correct..!!");

                    updateRoom.UpdateDate = Utility.GetBdDateTimeNow();
                    updateRoom.UpdatedById = CurrentUserId;

                    //var days = AppUtility.DaysDiffernce(updateRoom.CheckOutTime, updateRoom.CheckInTime);

                    var days = AppUtility.DaysDiffernceOnlyDate(updateRoom.CheckOutTime, updateRoom.CheckInTime);

                    if (days > 0)
                    {
                        if (updateRoom.RoomId != null && updateRoom.RoomId > 0)
                        {
                            var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == updateRoom.RoomId && x.IsActive && !x.IsDeleted);
                            if (roomInfo == null)
                                throw new Exception("Room Not Found...!!");

                            var checkAvalibility = await _iRoomInfoService.CheckRoomIsAvaliable(roomInfo.Id, updateRoom.CheckInTime, updateRoom.CheckOutTime, updateRoom.BookingId);
                            if (!checkAvalibility)
                                throw new Exception("Room Is Not Available...!!");

                            updateRoom.Rent = (roomInfo.Rent * days);
                            updateRoom.ServiceCharge = (roomInfo.ServiceCharge * days);
                            updateRoom.NetRent = (updateRoom.Rent + updateRoom.ServiceCharge) - (updateRoom.Discount);
                            updateRoom.TotalGuest = roomInfo.Person;

                            roomInfo.BookingStatus = BookingStatusEnum.Booked;
                        }
                    }
                    else
                    {
                        throw new Exception("Booking Days Have To More Than Zero....!!");
                    }

                    checkInDateList.Add(updateRoom.CheckInTime);
                    checkOutDateList.Add(updateRoom.CheckOutTime);
                }

                bookingService.Rent = updateableRoomList.Sum(x => x.Rent);
                bookingService.ServiceCharge = updateableRoomList.Sum(x => x.ServiceCharge);
                bookingService.Discount = updateableRoomList.Sum(x => x.Discount);
                bookingService.NetRent = bookingService.Rent + bookingService.ServiceCharge - (bookingService.Discount);
            }

            var oldIds = updateableRoomList?.Select(c => c.Id).ToList();
            deletableRoomList = (existBookingRooms.Where(x => !oldIds.Contains(x.Id))).ToList();

            if (dataListForAdd?.Count > 0)
            {
                addableRoomList = _iMapper.Map<List<HtBookingRoom>>(dataListForAdd);

                foreach (var (room, i) in addableRoomList.GetItemWithIndex())
                {
                    var filterData = dataListForAdd.FirstOrDefault(c => c.RoomId == room.RoomId);

                    if (filterData == null)
                        throw new Exception("Room Not Found...!");

                    room.BookingId = bookingService.Id;
                    room.CheckInTime = (DateTime)(!string.IsNullOrEmpty(filterData.CheckInTimeStr) ? Utility.ConvertStrToDate(filterData.CheckInTimeStr) : bookingService.CheckInTime);
                    room.CheckOutTime = (DateTime)(!string.IsNullOrEmpty(filterData.CheckOutTimeStr) ? Utility.ConvertStrToDate(filterData.CheckOutTimeStr) : bookingService.CheckOutTime);
                    room.Discount = filterData.Discount;

                    if (room.CheckInTime.Date > room.CheckOutTime.Date)
                        throw new Exception("Room CheakIn & CheckOut Date Is Not Correct..!!");

                    room.ActionDate = Utility.GetBdDateTimeNow();
                    room.ActionById = CurrentUserId;
                    room.UpdateDate = Utility.GetBdDateTimeNow();
                    room.UpdatedById = CurrentUserId;

                    if (room.RoomId != null && room.RoomId > 0)
                    {
                        var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == room.RoomId && x.IsActive && !x.IsDeleted);
                        if (roomInfo == null)
                            throw new Exception("Room Not Found...!!");

                        var checkAvalibility = await _iRoomInfoService.CheckRoomIsAvaliable(roomInfo.Id, room.CheckInTime, room.CheckOutTime);
                        if (!checkAvalibility)
                            throw new Exception("Room Is Not Available...!!");

                        //var days = AppUtility.DaysDiffernce(room.CheckOutTime, (DateTime)room.CheckInTime);

                        var days = AppUtility.DaysDiffernceOnlyDate(room.CheckOutTime, room.CheckInTime);
                        if (days > 0)
                        {
                            room.Rent = (roomInfo.Rent * days);
                            room.ServiceCharge = (roomInfo.ServiceCharge * days);
                            room.NetRent = (room.Rent + room.ServiceCharge) - (room.Discount);
                            room.TotalGuest = roomInfo.Person;
                        }
                        else
                        {
                            throw new Exception("Booking Days Have To More Than Zero....!!");
                        }
                    }

                    checkInDateList.Add(room.CheckInTime);
                    checkOutDateList.Add(room.CheckOutTime);
                }

                bookingService.Rent += addableRoomList.Sum(x => x.Rent);
                bookingService.ServiceCharge += addableRoomList.Sum(x => x.ServiceCharge);
                bookingService.Discount += addableRoomList.Sum(x => x.Discount);
                bookingService.NetRent = bookingService.Rent + bookingService.ServiceCharge - (bookingService.Discount);
            }

            var minCheckInDate = checkInDateList.Min();
            var maxCheckOutDate = checkOutDateList.Max();

            if (minCheckInDate.Date != bookingService.CheckInTime.Date)
                bookingService.CheckInTime = minCheckInDate;

            if (maxCheckOutDate.Date != bookingService.CheckOutTime.Date)
                bookingService.CheckOutTime = maxCheckOutDate;

        }

        #endregion

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        _iRepository.Update(bookingService);

        if (addableRoomList?.Count > 0)
        {
            _iBookingRoomRepository.AddRange(addableRoomList);
        }

        if (updateableRoomList?.Count > 0)
        {
            _iBookingRoomRepository.UpdateRange(updateableRoomList);
        }

        if (deletableRoomList?.Count > 0)
        {
            _iBookingRoomRepository.RemoveRange(deletableRoomList);
        }

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }

    #endregion

    #region CheckIn

    public async Task<bool> RoomCheckIn(HtBookingServiceVm vm)
    {
        if (vm == null && !(vm.Id > 0))
            throw new Exception("Booking Service Information Is Not Correct...!!");

        var bookingService = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == vm.Id && x.BookingType == BookingType.Room && !x.IsDeleted);

        if (bookingService == null)
            throw new Exception("Booking Service Not Found...!!");

        if (bookingService.BookingStatus == BookingServiceStatusEnum.CheckIn)
            throw new Exception("Booking Service Already Checked In...!!");

        bookingService.VisitPurpose = vm.VisitPurpose;
        bookingService.BookingStatus = BookingServiceStatusEnum.CheckIn;
        bookingService.BookingConfirmStatus = BookingConfirmEnum.Confirm;

        bookingService.Rent = 0;
        bookingService.ServiceCharge = 0;
        bookingService.Vat = 0;
        bookingService.Discount = 0;
        bookingService.NetRent = 0;

        #region Room

        List<HtBookingRoom> addableRoomList = null;
        List<HtBookingRoom> updateableRoomList = null;
        List<HtBookingRoom> deletableRoomList = null;

        var checkInDateList = new List<DateTime>();
        var checkOutDateList = new List<DateTime>();

        var actualCheckInDateList = new List<DateTime>();
        var checkInRoomList = new List<HtRoomInfo>();

        var existBookingRooms = await _iBookingRoomRepository.GetAsync(x => x.BookingId == vm.Id && !x.IsDeleted);

        if (vm?.BookingRoomVms?.Count > 0)
        {
            var dataListForAdd = vm?.BookingRoomVms?.Where(c => c.Id == 0).ToList();

            var updatableItemIds = vm?.BookingRoomVms.Where(c => c.Id > 0).Select(c => c.Id).ToList();
            updateableRoomList = (existBookingRooms.Where(x => updatableItemIds.Contains(x.Id))).ToList();

            if (updateableRoomList?.Count > 0)
            {
                foreach (var updateRoom in updateableRoomList)
                {
                    var filterData = vm.BookingRoomVms.Where(c => c.Id == updateRoom.Id).FirstOrDefault();

                    //if (string.IsNullOrEmpty(filterData.ActualCheckInTimeStr))
                    //    throw new Exception("Actual CheakIn Date Is Mendatory..!!");

                    var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == updateRoom.RoomId && x.IsActive && !x.IsDeleted);
                    if (roomInfo == null)
                        throw new Exception("Room Not Found...!!");

                    var isVc = IsVcRoom(roomInfo);
                    if (!isVc)
                    {
                        vm.VDRooms.Add(roomInfo.RoomNo);
                        continue;
                    }

                    if (string.IsNullOrEmpty(filterData.ActualCheckInTimeStr))
                        continue;

                    updateRoom.ActualCheckInTime = (DateTime)(!string.IsNullOrEmpty(filterData.ActualCheckInTimeStr) ? Utility.ConvertStrToDate(filterData.ActualCheckInTimeStr) : filterData.CheckInTime);

                    if (updateRoom.ActualCheckInTime?.Date != updateRoom.CheckInTime.Date)
                        throw new Exception("Actual CheakIn Date Not Matched..!!");

                    updateRoom.Adult = filterData.Adult ?? 0;
                    updateRoom.Child = filterData.Child ?? 0;
                    updateRoom.ExtraBed = filterData.ExtraBed ?? 0;
                    updateRoom.Discount = filterData.Discount;

                    //var days = AppUtility.DaysDiffernce(updateRoom.CheckOutTime, (DateTime)updateRoom.ActualCheckInTime);
                    var days = AppUtility.DaysDiffernceOnlyDate(updateRoom.CheckOutTime, (DateTime)updateRoom.ActualCheckInTime);
                    if (days > 0)
                    {
                        if (updateRoom.RoomId != null && updateRoom.RoomId > 0)
                        {
                            //updateRoom.Rent = (roomInfo.Rent * days);
                            //updateRoom.ServiceCharge = (roomInfo.ServiceCharge * days);

                            updateRoom.Rent = (updateRoom.RoomRent * days);
                            updateRoom.ServiceCharge = (updateRoom.RoomServiceCharge * days);

                            var bedCharge = (updateRoom.ExtraBed * ExtraBedAmount.ExtraBedSingleCharge) * days;
                            updateRoom.ExtraBedCharge = bedCharge;
                            updateRoom.NetRent = (updateRoom.Rent + updateRoom.ServiceCharge + updateRoom.ExtraBedCharge) - (updateRoom.Discount);

                            updateRoom.TotalGuest = roomInfo.Person;

                            roomInfo.BookingStatus = BookingStatusEnum.Booked;
                        }
                    }

                    actualCheckInDateList.Add(updateRoom.CheckOutTime);
                }

                bookingService.Rent = updateableRoomList.Sum(x => x.Rent);
                bookingService.ServiceCharge = updateableRoomList.Sum(x => x.ServiceCharge);
                bookingService.Vat = updateableRoomList.Sum(x => x.Vat);
                bookingService.Discount = updateableRoomList.Sum(x => x.Discount);

                var extraBedChargeUpdate = updateableRoomList.Sum(x => x.ExtraBedCharge);

                bookingService.NetRent = (bookingService.Rent + bookingService.ServiceCharge + bookingService.Vat + extraBedChargeUpdate) - (bookingService.Discount);
            }

            var oldIds = updateableRoomList?.Select(c => c.Id).ToList();
            deletableRoomList = (existBookingRooms.Where(x => !oldIds.Contains(x.Id))).ToList();

            if (dataListForAdd?.Count > 0)
            {
                addableRoomList = _iMapper.Map<List<HtBookingRoom>>(dataListForAdd);

                foreach (var (room, i) in addableRoomList.GetItemWithIndex())
                {
                    var filterData = dataListForAdd.FirstOrDefault(c => c.RoomId == room.RoomId);

                    if (filterData == null)
                        throw new Exception("Room Not Found...!");

                    var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == room.RoomId && x.IsActive && !x.IsDeleted);
                    if (roomInfo == null)
                        throw new Exception("Room Not Found...!!");

                    var isVc = IsVcRoom(roomInfo);
                    if (!isVc)
                    {
                        vm.VDRooms.Add(roomInfo.RoomNo);
                        continue;
                    }

                    room.BookingId = bookingService.Id;
                    room.CheckInTime = (DateTime)(!string.IsNullOrEmpty(filterData.CheckInTimeStr) ? Utility.ConvertStrToDate(filterData.CheckInTimeStr) : bookingService.CheckInTime);
                    room.CheckOutTime = (DateTime)(!string.IsNullOrEmpty(filterData.CheckOutTimeStr) ? Utility.ConvertStrToDate(filterData.CheckOutTimeStr) : bookingService.CheckOutTime);
                    room.Discount = filterData.Discount;

                    if (room.CheckInTime.Date > room.CheckOutTime.Date)
                        throw new Exception("Room CheakIn & CheckOut Date Is Not Correct..!!");

                    room.ActualCheckInTime = (DateTime)(!string.IsNullOrEmpty(filterData.ActualCheckInTimeStr) ? Utility.ConvertStrToDate(filterData.ActualCheckInTimeStr) : filterData.CheckInTime);

                    if (room.ActualCheckInTime?.Date != room.CheckInTime.Date)
                        throw new Exception("Actual CheakIn Date Not Matched..!!");

                    room.ActionDate = Utility.GetBdDateTimeNow();
                    room.ActionById = CurrentUserId;

                    if (room.RoomId != null && room.RoomId > 0)
                    {
                        var checkAvalibility = await _iRoomInfoService.CheckRoomIsAvaliable(roomInfo.Id, room.CheckInTime, room.CheckOutTime);
                        if (!checkAvalibility)
                            throw new Exception("Room Is Not Available...!!");

                        //var days = AppUtility.DaysDiffernce(room.CheckOutTime, (DateTime)room.ActualCheckInTime);
                        var days = AppUtility.DaysDiffernceOnlyDate(room.CheckOutTime, (DateTime)room.ActualCheckInTime);
                        if (days > 0)
                        {
                            //room.Rent = (roomInfo.Rent * days);
                            //room.ServiceCharge = (roomInfo.ServiceCharge * days);

                            room.Rent = (room.RoomRent * days);
                            room.ServiceCharge = (room.RoomServiceCharge * days);

                            var bedCharge = (room.ExtraBed * ExtraBedAmount.ExtraBedSingleCharge) * days;
                            room.ExtraBedCharge = bedCharge;

                            room.NetRent = (room.Rent + room.ServiceCharge + room.ExtraBedCharge) - (room.Discount);
                            room.TotalGuest = roomInfo.Person;
                        }
                        else
                        {
                            throw new Exception("Booking Days Have To More Than Zero....!!");
                        }

                        actualCheckInDateList.Add(room.CheckOutTime);
                    }
                }

                bookingService.Rent += addableRoomList.Sum(x => x.Rent);
                bookingService.ServiceCharge += addableRoomList.Sum(x => x.ServiceCharge);
                bookingService.Vat += addableRoomList.Sum(x => x.Vat);
                bookingService.Discount += addableRoomList.Sum(x => x.Discount);

                var extraBedChargeAdd = addableRoomList.Sum(x => x.ExtraBedCharge);

                bookingService.NetRent = (bookingService.Rent + bookingService.ServiceCharge + bookingService.Vat + extraBedChargeAdd) - (bookingService.Discount);
            }

            if (vm?.BookingRoomVms?.Count == vm.VDRooms.Count)
                throw new Exception($"{string.Join(',', vm.VDRooms)} Room(s) is/are not Vacant and Clean...!!!");
        }

        if (!(actualCheckInDateList.Count > 0))
            throw new Exception("No Room Wise Check-In Date Not Found...!!!");

        #region RoomStatusUpdate

        List<HtRoomInfo> changeStatusRoomList = new List<HtRoomInfo>();

        var roomList = await _iRoomInfoRepository.GetAsync(x => !x.IsDeleted);

        if (addableRoomList?.Count > 0)
        {
            foreach (var item in addableRoomList)
            {
                var roomInfo = roomList.FirstOrDefault(x => x.Id == item.RoomId);
                if (roomInfo == null)
                    throw new Exception("Room Not Found To Book..!!");

                if (vm.VDRooms.Contains(roomInfo.RoomNo))
                {
                    continue;
                }

                if (item.ActualCheckInTime != null)
                {
                    roomInfo.CleaningStatus = CleaningStatusEnum.O;
                    roomInfo.HouseKeeperAvailabilityStatus = AvailabilityStatusEnum.Occupied;

                    checkInRoomList.Add(roomInfo);
                }

                changeStatusRoomList.Add(roomInfo);
            }
        }

        if (updateableRoomList?.Count > 0)
        {
            foreach (var item in updateableRoomList)
            {
                var roomInfo = roomList.FirstOrDefault(x => x.Id == item.RoomId);
                if (roomInfo == null)
                    throw new Exception("Room Not Found To Book..!!");

                if (vm.VDRooms.Contains(roomInfo.RoomNo))
                {
                    continue;
                }

                if (item.ActualCheckInTime != null)
                {
                    roomInfo.CleaningStatus = CleaningStatusEnum.O;
                    roomInfo.HouseKeeperAvailabilityStatus = AvailabilityStatusEnum.Occupied;

                    checkInRoomList.Add(roomInfo);
                }

                changeStatusRoomList.Add(roomInfo);
            }
        }

        if (deletableRoomList?.Count > 0)
        {
            foreach (var item in deletableRoomList)
            {
                var roomInfo = roomList.FirstOrDefault(x => x.Id == item.RoomId);
                if (roomInfo == null)
                    throw new Exception("Room Not Found To Book..!!");

                roomInfo.BookingStatus = BookingStatusEnum.Available;
                roomInfo.HouseKeeperAvailabilityStatus = AvailabilityStatusEnum.Available;

                changeStatusRoomList.Add(roomInfo);
            }
        }

        #region RoomStatusHst

        var (changeHstList, addedHstList) = await GetRoomStatusHistory(changeStatusRoomList);

        #endregion

        #endregion

        #endregion

        #region Guest

        List<HtBookingGuest> addableGuestList = null;
        List<HtBookingGuest> updateableGuestList = null;
        List<HtBookingGuest> deletableGuestList = null;

        var existBookingGuests = await _iBookingGuestRepository.GetAsync(x => x.BookingId == vm.Id && !x.IsDeleted, g => g.Guest);

        var mainGuest = existBookingGuests.FirstOrDefault(x => x.IsMain);

        if (vm?.BookingGuestVms?.Count > 0)
        {
            var guestListForAdd = vm?.BookingGuestVms?.Where(c => c.Id == 0).ToList();

            var updatableItemIds = vm?.BookingGuestVms.Where(c => c.Id > 0).Select(c => c.Id).ToList();
            updateableGuestList = (existBookingGuests.Where(x => updatableItemIds.Contains(x.Id))).ToList();

            var oldIds = updateableGuestList?.Select(c => c.Id).ToList();

            deletableGuestList = (existBookingGuests.Where(x => !oldIds.Contains(x.Id))).ToList();

            if (guestListForAdd?.Count > 0)
            {
                addableGuestList = _iMapper.Map<List<HtBookingGuest>>(guestListForAdd);

                foreach (var (v, i) in addableGuestList.GetItemWithIndex())
                {
                    var filterData = guestListForAdd.FirstOrDefault(c => c.GuestId == v.GuestId);

                    v.BookingId = bookingService.Id;
                    v.ActionDate = DateTime.Now;
                    v.ActionById = CurrentUserId;
                }
            }
        }

        #endregion

        #region Payment

        HtBookingPayment paymentModel = null;
        AccTranMst paymentVoucher = null;

        if (vm.PaymentVm != null && vm.PaymentVm.PaidAmount > 0)
        {
            paymentModel = _iMapper.Map<HtBookingPayment>(vm.PaymentVm);
            //paymentModel.PaidDate = Utility.GetBdDateTimeNow();
            paymentModel.PaidDate = (DateTime)(!string.IsNullOrEmpty(vm.PaidDateStr) ? Utility.ConvertStrToDate(vm.PaidDateStr) : DU.Utility.GetBdDateTimeNow());
            paymentModel.Description = $"Amount {vm.PaymentVm.PaidAmount} is paid when check in into room.";
            paymentModel.BookingId = bookingService.Id;
            paymentModel.ActionById = CurrentUserId;
            paymentModel.ActionDate = Utility.GetBdDateTimeNow();
            paymentModel.IsAdvance = true;

            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            var reportDate = Utility.GenerateReportDate(paymentModel.PaidDate.Add(currentTime));
            paymentModel.ReportDate = reportDate;

            if (bookingService.NetRent == paymentModel.PaidAmount)
                bookingService.PaymentStatus = PaymentStatusEnum.FullPayment;
            else if (bookingService.NetRent > paymentModel.PaidAmount)
                bookingService.PaymentStatus = PaymentStatusEnum.PartialPayment;

            //paymentVoucher = await GetAdvancePaymentVoucher(paymentModel, bookingService.BookingNo);//For standard account system
            var novDate = new DateTime(2024, 11, 29);
            if (paymentModel.PaidDate.Date > novDate.Date)
            {
                paymentVoucher = await GetAdvancePaymentQuickVoucher(paymentModel, bookingService.BookingNo);//For quick voucher system
                if (paymentVoucher == null)
                    throw new Exception("Somthing Went Wrong Creating Voucher..!!");
            }
        }

        #endregion

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        _iRepository.Update(bookingService);

        if (addableRoomList?.Count > 0)
        {
            _iBookingRoomRepository.AddRange(addableRoomList);
        }

        if (updateableRoomList?.Count > 0)
        {
            _iBookingRoomRepository.UpdateRange(updateableRoomList);
        }

        if (deletableRoomList?.Count > 0)
        {
            _iBookingRoomRepository.RemoveRange(deletableRoomList);
        }

        if (changeStatusRoomList?.Count > 0)
        {
            _iRoomInfoRepository.UpdateRange(changeStatusRoomList);
        }

        if (changeHstList?.Count > 0)
        {
            _iRoomStatusHistoryRepository.UpdateRange(changeHstList);
        }

        if (addedHstList?.Count > 0)
        {
            await _iRoomStatusHistoryRepository.AddRangeAsync(addedHstList);
        }

        if (addableGuestList?.Count > 0)
        {
            _iBookingGuestRepository.AddRange(addableGuestList);
        }

        if (deletableGuestList?.Count > 0)
        {
            _iBookingGuestRepository.RemoveRange(deletableGuestList);
        }

        if (paymentModel != null && paymentVoucher != null)
        {
            await _iBookingPaymentRepository.AddAsync(paymentModel);
            await _iUnitOfWork.CompleteAsync();

            paymentVoucher.PaymentId = paymentModel.Id;
            await _iAccTranMstRepository.AddAsync(paymentVoucher);
        }
        else if (paymentModel != null)
        {
            await _iBookingPaymentRepository.AddAsync(paymentModel);
        }

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }

        string ntfMsgHtml = CheckInNotificationMsg(bookingService, checkInRoomList, $"{mainGuest?.Guest.FirstName} {mainGuest?.Guest.LastName}", paymentModel?.PaidAmount ?? 0);

        var ntfGenerated = await _iNtfMsgService.GenerateNtf(NotificationEventCode.CheckInNtf, ntfMsgHtml, ntfMsgHtml);

        ts.Complete();
        return true;
    }

    #endregion

    #region CheckInUpdate

    public async Task<bool> RoomCheckInUpdate(HtBookingServiceVm vm)
    {
        if (vm == null && !(vm.Id > 0))
            throw new Exception("Booking Service Information Is Not Correct...!!");

        var bookingService = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == vm.Id && x.BookingType == BookingType.Room && !x.IsDeleted);

        if (bookingService == null)
            throw new Exception("Booking Service Not Found...!!");

        if (bookingService.BookingStatus != BookingServiceStatusEnum.CheckIn)
            throw new Exception("Booking Service In Not Checked In...!!");

        bookingService.VisitPurpose = vm.VisitPurpose;
        bookingService.BookingStatus = BookingServiceStatusEnum.CheckIn;

        var bill = await _iBillRepository.GetFirstOrDefaultAsync(x => x.BookingId == bookingService.Id, d => d.Include(x => x.BillingDetails)
                                                                                                                   .ThenInclude(x => x.Service));
        if (bill is null)
            throw new Exception("Bill Not Found...!!");

        var detailModel = bill.BillingDetails;

        #region Room

        List<HtBookingRoom> addableRoomList = null;
        List<HtBookingRoom> updateableRoomList = null;
        List<HtBookingRoom> deletableRoomList = null;
        List<HtRoomDayAudit> deletableAuditRoomList = null;

        bookingService.Rent = 0;
        bookingService.ServiceCharge = 0;
        bookingService.Vat = 0;
        bookingService.Discount = 0;
        bookingService.NetRent = 0;

        var checkOutDates = new List<DateTime>();

        var existBookingRooms = await _iBookingRoomRepository.GetAsync(x => x.BookingId == vm.Id && !x.IsDeleted);

        var alreadyCheckOutRooms = existBookingRooms.Where(x => x.ActualCheckInTime != null && x.ActualCheckOutTime != null).ToList();

        if (vm?.BookingRoomVms?.Count > 0)
        {
            var dataListForAdd = vm?.BookingRoomVms?.Where(c => c.Id == 0).ToList();

            var updatableItemIds = vm?.BookingRoomVms.Where(c => c.Id > 0).Select(c => c.Id).ToList();
            updateableRoomList = (existBookingRooms.Where(x => updatableItemIds.Contains(x.Id))).ToList();



            if (updateableRoomList?.Count > 0)
            {
                foreach (var updateRoom in updateableRoomList)
                {
                    var filterData = vm.BookingRoomVms.Where(c => c.Id == updateRoom.Id).FirstOrDefault();

                    //if (string.IsNullOrEmpty(filterData.ActualCheckInTimeStr))
                    //    throw new Exception("Actual CheakIn Date Is Mendatory..!!");

                    var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == updateRoom.RoomId && x.IsActive && !x.IsDeleted);
                    if (roomInfo == null)
                        throw new Exception("Room Not Found...!!");

                    var isVc = IsVcRoom(roomInfo);
                    if (!isVc && updateRoom.ActualCheckInTime == null)
                    {
                        vm.VDRooms.Add(roomInfo.RoomNo);
                        continue;
                    }

                    updateRoom.CheckOutTime = (DateTime)(!string.IsNullOrEmpty(filterData.CheckOutTimeStr) ? Utility.ConvertStrToDate(filterData.CheckOutTimeStr) : updateRoom.CheckOutTime);
                    updateRoom.ActualCheckInTime = !string.IsNullOrEmpty(filterData.ActualCheckInTimeStr) ? Utility.ConvertStrToDate(filterData.ActualCheckInTimeStr) : null;
                    updateRoom.ActualCheckOutTime = !string.IsNullOrEmpty(filterData.ActualCheckOutTimeStr) ? Utility.ConvertStrToDate(filterData.ActualCheckOutTimeStr) : null;

                    updateRoom.Adult = filterData.Adult ?? 0;
                    updateRoom.Child = filterData.Child ?? 0;
                    updateRoom.ExtraBed = filterData.ExtraBed ?? 0;
                    updateRoom.Discount = filterData.Discount;

                    var checkInTime = updateRoom.ActualCheckInTime != null ? updateRoom.ActualCheckInTime : updateRoom.CheckInTime;
                    var checkOutTime = updateRoom.ActualCheckOutTime != null ? updateRoom.ActualCheckOutTime : updateRoom.CheckOutTime;

                    if (updateRoom.ActualCheckOutTime != null)
                    {

                        if (updateRoom.ActualCheckOutTime < checkInTime)
                            throw new Exception("Actual Check-Out Time Have To Greater Than Check-In Time...!!");

                        updateRoom.CheckOutTime = (DateTime)updateRoom.ActualCheckOutTime;
                    }

                    double days = AppUtility.DaysDiffernceOnlyDate((DateTime)checkOutTime, (DateTime)checkInTime);

                    if (filterData.IsHalfDay == true && filterData.IsDayUse)
                        throw new Exception("Can't Both Half Day And Day Use...Added..!!");

                    if (updateRoom.BookingDayStatus > 0)
                    {
                        if (updateRoom.BookingDayStatus == (int)BookingDayStatusEnum.HalfDay)
                        {
                            days = (days + 0.5);
                        }
                        else if (updateRoom.BookingDayStatus == (int)BookingDayStatusEnum.DayUse)
                        {
                            days = (days + 1);
                        }
                    }
                    else
                    {
                        if (filterData.IsHalfDay)
                        {
                            days = (days + 0.5);
                            updateRoom.BookingDayStatus = (int)BookingDayStatusEnum.HalfDay;
                        }
                        else if (filterData.IsDayUse)
                        {
                            days = (days + 1);
                            updateRoom.BookingDayStatus = (int)BookingDayStatusEnum.DayUse;
                        }
                    }

                    if (!(days > 0))
                        throw new Exception($"Days can't be zero...!!");

                    if (days >= 0)
                    {
                        if (updateRoom.RoomId != null && updateRoom.RoomId > 0)
                        {
                            //var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == updateRoom.RoomId && x.IsActive && !x.IsDeleted);
                            //if (roomInfo == null)
                            //    throw new Exception("Room Not Found...!!");

                            var checkAvalibility = await _iRoomInfoService.CheckRoomIsAvaliable(roomInfo.Id, updateRoom.CheckInTime, updateRoom.CheckOutTime, updateRoom.BookingId);
                            if (!checkAvalibility)
                                throw new Exception("Room Is Not Available...!!");

                            updateRoom.Rent = (updateRoom.RoomRent * days);
                            updateRoom.ServiceCharge = (updateRoom.RoomServiceCharge * days);

                            var bedCharge = (updateRoom.ExtraBed * ExtraBedAmount.ExtraBedSingleCharge) * days;
                            updateRoom.ExtraBedCharge = filterData.ExtraBedCharge ?? 0;

                            updateRoom.NetRent = (updateRoom.Rent + updateRoom.ServiceCharge + updateRoom.Vat + updateRoom.ExtraBedCharge) - (updateRoom.Discount);
                            updateRoom.TotalGuest = roomInfo.Person;

                            roomInfo.BookingStatus = BookingStatusEnum.Booked;
                        }
                    }

                    checkOutDates.Add(updateRoom.CheckOutTime);
                }

                bookingService.Rent = updateableRoomList.Sum(x => x.Rent);
                bookingService.ServiceCharge = updateableRoomList.Sum(x => x.ServiceCharge);
                bookingService.Vat = updateableRoomList.Sum(x => x.Vat);
                bookingService.Discount = updateableRoomList.Sum(x => x.Discount);

                var extraBedChargeUpdate = updateableRoomList.Sum(x => x.ExtraBedCharge);

                bookingService.NetRent = (bookingService.Rent + bookingService.ServiceCharge + extraBedChargeUpdate) - (bookingService.Discount);
            }

            var oldIds = updateableRoomList?.Select(c => c.Id).ToList();
            //deletableRoomList = (existBookingRooms.Where(x => !oldIds.Contains(x.Id))).ToList();
            //updated by Rifat 7-Jan-2026: to prevent service contained room remove
            var alldeletableRoomList = (existBookingRooms.Where(room => !oldIds.Contains(room.Id))).ToList();

            vm.RoomsWithExtraService = alldeletableRoomList
                                        .Where(room => detailModel
                                        .Any(d => d.BookingRoomId == room.Id
                                                && d.Service != null
                                                && d.Service.IsExtra))
                                        .Select(x => x.Room?.RoomNo)
                                        .ToList();

            deletableRoomList = alldeletableRoomList
                                .Where(room => !detailModel
                                .Any(d => d.BookingRoomId == room.Id
                                        && d.Service != null
                                        && d.Service.IsExtra))
                                .ToList();

            if (deletableRoomList.Count > 0)
            {
                var deletableIds = deletableRoomList.Select(x => x.Id).ToList();
                var existingAudits = await _iRoomDayAuditRepository.GetAsync(c => deletableIds.Contains(c.BookingRoomId));

                deletableAuditRoomList = existingAudits.ToList();
            }

            if (dataListForAdd?.Count > 0)
            {
                addableRoomList = _iMapper.Map<List<HtBookingRoom>>(dataListForAdd);

                foreach (var (room, i) in addableRoomList.GetItemWithIndex())
                {
                    var filterData = dataListForAdd.FirstOrDefault(c => c.RoomId == room.RoomId);

                    if (filterData == null)
                        throw new Exception("Room Not Found...!");

                    var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == room.RoomId && x.IsActive && !x.IsDeleted);
                    if (roomInfo == null)
                        throw new Exception("Room Not Found...!!");

                    var isVc = IsVcRoom(roomInfo);
                    if (!isVc)
                    {
                        vm.VDRooms.Add(roomInfo.RoomNo);
                        continue;
                    }

                    room.BookingId = bookingService.Id;
                    room.CheckInTime = (DateTime)(!string.IsNullOrEmpty(filterData.CheckInTimeStr) ? Utility.ConvertStrToDate(filterData.CheckInTimeStr) : bookingService.CheckInTime);
                    room.CheckOutTime = (DateTime)(!string.IsNullOrEmpty(filterData.CheckOutTimeStr) ? Utility.ConvertStrToDate(filterData.CheckOutTimeStr) : bookingService.CheckOutTime);
                    room.Discount = filterData.Discount;

                    if (room.CheckInTime.Date > room.CheckOutTime.Date)
                        throw new Exception("Room CheakIn & CheckOut Date Is Not Correct..!!");

                    room.ActualCheckInTime = (DateTime)(!string.IsNullOrEmpty(filterData.ActualCheckInTimeStr) ? Utility.ConvertStrToDate(filterData.ActualCheckInTimeStr) : filterData.CheckInTime);
                    room.ActionDate = Utility.GetBdDateTimeNow();
                    room.ActionById = CurrentUserId;

                    if (room.RoomId != null && room.RoomId > 0)
                    {
                        //var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == room.RoomId && x.IsActive && !x.IsDeleted);
                        //if (roomInfo == null)
                        //    throw new Exception("Room Not Found...!!");

                        var days = AppUtility.DaysDiffernceOnlyDate(room.CheckOutTime, (DateTime)room.ActualCheckInTime);
                        if (days > 0)
                        {
                            room.Rent = (room.RoomRent * days);
                            room.ServiceCharge = (room.RoomServiceCharge * days);

                            var bedCharge = (room.ExtraBed * ExtraBedAmount.ExtraBedSingleCharge) * days;
                            room.ExtraBedCharge = bedCharge;

                            room.NetRent = (room.Rent + room.ServiceCharge + room.ExtraBedCharge) - (room.Discount);
                            room.TotalGuest = roomInfo.Person;
                        }
                    }

                    checkOutDates.Add(room.CheckOutTime);
                }

                bookingService.Rent += addableRoomList.Sum(x => x.Rent);
                bookingService.ServiceCharge += addableRoomList.Sum(x => x.ServiceCharge);
                bookingService.Vat += addableRoomList.Sum(x => x.Vat);
                bookingService.Discount += addableRoomList.Sum(x => x.Discount);

                var extraBedChargeAdd = addableRoomList.Sum(x => x.ExtraBedCharge);

                bookingService.NetRent = (bookingService.Rent + bookingService.ServiceCharge + bookingService.Vat + extraBedChargeAdd) - (bookingService.Discount);
            }
        }

        var maxCheckOutDate = checkOutDates.Count > 0 ? checkOutDates.Max() : bookingService.CheckOutTime;
        bookingService.CheckOutTime = maxCheckOutDate;

        #region RoomStatusUpdate

        List<HtRoomInfo> changeStatusRoomList = new List<HtRoomInfo>();

        var roomList = await _iRoomInfoRepository.GetAsync(x => !x.IsDeleted);

        if (addableRoomList?.Count > 0)
        {
            foreach (var item in addableRoomList)
            {
                var roomInfo = roomList.FirstOrDefault(x => x.Id == item.RoomId);
                if (roomInfo == null)
                    throw new Exception("Room Not Found To Book..!!");

                if (vm.VDRooms.Contains(roomInfo.RoomNo))
                {
                    continue;
                }

                if (item.ActualCheckInTime != null)
                {
                    roomInfo.BookingStatus = BookingStatusEnum.Booked;
                    roomInfo.CleaningStatus = CleaningStatusEnum.O;
                    roomInfo.HouseKeeperAvailabilityStatus = AvailabilityStatusEnum.Occupied;
                }

                changeStatusRoomList.Add(roomInfo);
            }
        }

        if (updateableRoomList?.Count > 0)
        {
            foreach (var item in updateableRoomList)
            {
                var existInAlreadyCheckout = alreadyCheckOutRooms.Any(x => x.Id == item.Id);
                if (existInAlreadyCheckout)
                    continue;

                var roomInfo = roomList.FirstOrDefault(x => x.Id == item.RoomId);
                if (roomInfo == null)
                    throw new Exception("Room Not Found To Book..!!");

                if (vm.VDRooms.Contains(roomInfo.RoomNo))
                {
                    continue;
                }

                if (item.ActualCheckInTime != null)
                {
                    roomInfo.BookingStatus = BookingStatusEnum.Booked;
                    roomInfo.CleaningStatus = CleaningStatusEnum.O;
                    roomInfo.HouseKeeperAvailabilityStatus = AvailabilityStatusEnum.Occupied;
                }

                if (item.ActualCheckInTime != null && item.ActualCheckOutTime != null)
                {
                    roomInfo.BookingStatus = BookingStatusEnum.Available;
                    roomInfo.CleaningStatus = CleaningStatusEnum.VD;
                    roomInfo.HouseKeeperAvailabilityStatus = AvailabilityStatusEnum.CheckedOut;
                }

                changeStatusRoomList.Add(roomInfo);
            }
        }

        if (deletableRoomList?.Count > 0)
        {
            foreach (var item in deletableRoomList)
            {
                var roomInfo = roomList.FirstOrDefault(x => x.Id == item.RoomId);
                if (roomInfo == null)
                    throw new Exception("Room Not Found To Book..!!");

                roomInfo.BookingStatus = BookingStatusEnum.Available;
                roomInfo.CleaningStatus = item.ActualCheckInTime != null ? CleaningStatusEnum.VD : CleaningStatusEnum.VC;
                roomInfo.HouseKeeperAvailabilityStatus = item.ActualCheckInTime != null ? AvailabilityStatusEnum.CheckedOut : AvailabilityStatusEnum.Available;

                changeStatusRoomList.Add(roomInfo);
            }
        }

        #region RoomStatusHst

        var (changeHstList, addedHstList) = await GetRoomStatusHistory(changeStatusRoomList);

        #endregion

        #endregion

        #endregion

        #region Bill

        var billCheckOutTime = !string.IsNullOrEmpty(vm.ActualCheckOutTimeStr) ? Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.ActualCheckOutTimeStr))
            : bookingService.CheckOutTime;

        var paidList = await _iBookingPaymentRepository.GetAsync(x => x.BookingId == bookingService.Id && !x.IsDeleted && x.PaidDate.Date <= billCheckOutTime.Date);

        var alreadyPaidAmount = paidList.Sum(x => x.PaidAmount);

        var totalRoomList = addableRoomList != null && addableRoomList.Count > 0 ? updateableRoomList.Concat(addableRoomList).ToList() : updateableRoomList;

        var updateBill = await CheckInUpdateBill(bookingService, totalRoomList);
        if (updateBill == null)
            throw new Exception("Can't Update Bill..!!");

        updateBill.TotalAmount = updateBill.BillingDetails.Sum(x => x.Amount);
        updateBill.Discount = updateBill.BillingDetails.Sum(x => x.Discount);
        updateBill.ServiceCharge = updateBill.BillingDetails.Sum(x => x.ServiceCharge);
        updateBill.Vat = updateBill.BillingDetails.Sum(x => x.VAT);

        updateBill.SpecialDiscount = vm.SpecialDiscount;

        var netAmount = (updateBill.TotalAmount + updateBill.ServiceCharge + updateBill.Vat) - (updateBill.Discount + updateBill.SpecialDiscount);
        updateBill.NetAmount = Math.Round(netAmount, 2);

        updateBill.PaidAmount = alreadyPaidAmount;

        var occupiedRoomNetRent = updateableRoomList.Where(x => x.ActualCheckOutTime == null).Sum(x => x.NetRent);
        var billAmountWithRent = updateBill.NetAmount + occupiedRoomNetRent;

        if (updateBill.PaidAmount == billAmountWithRent && updateBill.PaidAmount > 0)
            updateBill.BillStatus = BillStatusEnum.FullPaid;
        else if (updateBill.PaidAmount > billAmountWithRent)
            updateBill.BillStatus = BillStatusEnum.PartialPaid;

        if (updateBill.PaidAmount == billAmountWithRent && updateBill.PaidAmount > 0)
            bookingService.PaymentStatus = PaymentStatusEnum.FullPayment;
        else if (updateBill.PaidAmount > billAmountWithRent)
            bookingService.PaymentStatus = PaymentStatusEnum.PartialPayment;

        #endregion

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        _iRepository.Update(bookingService);

        if (addableRoomList?.Count > 0)
        {
            _iBookingRoomRepository.AddRange(addableRoomList);
        }

        if (updateableRoomList?.Count > 0)
        {
            _iBookingRoomRepository.UpdateRange(updateableRoomList);
        }

        if (deletableAuditRoomList?.Count > 0)
        {
            _iRoomDayAuditRepository.RemoveRange(deletableAuditRoomList);
        }

        if (deletableRoomList?.Count > 0)
        {
            _iBookingRoomRepository.RemoveRange(deletableRoomList);
        }

        if (changeStatusRoomList?.Count > 0)
        {
            _iRoomInfoRepository.UpdateRange(changeStatusRoomList);
        }

        if (changeHstList?.Count > 0)
        {
            _iRoomStatusHistoryRepository.UpdateRange(changeHstList);
        }

        if (addedHstList?.Count > 0)
        {
            await _iRoomStatusHistoryRepository.AddRangeAsync(addedHstList);
        }

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }

    #endregion

    #region CheckOut

    public async Task<(bool, long)> RoomCheckOut(HtBookingServiceVm vm)
    {
        if (vm == null && !(vm.Id > 0))
            throw new Exception("Booking Service Information Is Not Correct...!!");

        var bookingService = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == vm.Id && x.BookingType == BookingType.Room && !x.IsDeleted);

        if (bookingService == null)
            throw new Exception("Booking Service Not Found...!!");

        if (bookingService.BookingStatus == BookingServiceStatusEnum.CheckOut)
            throw new Exception("Booking Service Already Checked Out...!!");

        if (string.IsNullOrEmpty(vm.ActualCheckOutTimeStr))
            throw new Exception("Actual Check-Out Time Not Found");

        bookingService.BookingStatus = BookingServiceStatusEnum.CheckOut;

        #region Room

        List<HtBookingRoom> updateableRoomList = null;

        bookingService.Discount = 0;

        var checkOutDates = new List<DateTime>();

        var existBookingRooms = await _iBookingRoomRepository.GetAsync(x => x.BookingId == vm.Id && !x.IsDeleted);

        var alreadyCheckOutRooms = existBookingRooms.Where(x => x.ActualCheckInTime != null && x.ActualCheckOutTime != null).ToList();

        if (vm?.BookingRoomVms?.Count > 0)
        {
            var updatableItemIds = vm?.BookingRoomVms.Where(c => c.Id > 0).Select(c => c.Id).ToList();
            updateableRoomList = (existBookingRooms.Where(x => updatableItemIds.Contains(x.Id))).ToList();

            if (updateableRoomList?.Count > 0)
            {
                foreach (var updateRoom in updateableRoomList)
                {
                    var filterData = vm.BookingRoomVms.Where(c => c.Id == updateRoom.Id).FirstOrDefault();

                    updateRoom.ActualCheckOutTime = !string.IsNullOrEmpty(filterData.ActualCheckOutTimeStr) ? Utility.ConvertStrToDate(filterData.ActualCheckOutTimeStr) : null;

                    updateRoom.CheckOutTime = (DateTime)updateRoom.ActualCheckOutTime;

                    if (updateRoom.RoomId != null && updateRoom.RoomId > 0)
                    {
                        var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == updateRoom.RoomId && x.IsActive && !x.IsDeleted);
                        if (roomInfo == null)
                            throw new Exception("Room Not Found...!!");

                        if (updateRoom.ActualCheckInTime == null)
                            throw new Exception($"Room: {roomInfo.RoomNo} Actual Check-In Time Is Mandatory...!");

                        if (updateRoom.ActualCheckOutTime == null)
                            throw new Exception($"Room: {roomInfo.RoomNo} Actual Check-Out Time Is Mandatory...!");

                        //double days = AppUtility.DaysDiffernce((DateTime)updateRoom.ActualCheckOutTime, updateRoom.CheckInTime);
                        double days = AppUtility.DaysDiffernceOnlyDate((DateTime)updateRoom.ActualCheckOutTime, updateRoom.CheckInTime);

                        if (filterData.IsHalfDay == true && filterData.IsDayUse)
                            throw new Exception("Can't Both Half Day And Day Use...Added..!!");

                        if (updateRoom.BookingDayStatus > 0)
                        {
                            if (updateRoom.BookingDayStatus == (int)BookingDayStatusEnum.HalfDay)
                            {
                                days = (days + 0.5);
                            }
                            else if (updateRoom.BookingDayStatus == (int)BookingDayStatusEnum.DayUse)
                            {
                                days = (days + 1);
                            }
                        }
                        else
                        {
                            if (filterData.IsHalfDay)
                            {
                                days = (days + 0.5);
                                updateRoom.BookingDayStatus = (int)BookingDayStatusEnum.HalfDay;
                            }
                            else if (filterData.IsDayUse)
                            {
                                days = (days + 1);
                                updateRoom.BookingDayStatus = (int)BookingDayStatusEnum.DayUse;
                            }
                        }

                        //if (filterData.IsHalfDay)
                        //{
                        //    days = (days + 0.5);
                        //    updateRoom.BookingDayStatus = (int)BookingDayStatusEnum.HalfDay;
                        //}
                        //else if (filterData.IsDayUse)
                        //{
                        //    days = (days + 1);
                        //    updateRoom.BookingDayStatus = (int)BookingDayStatusEnum.DayUse;
                        //}

                        if (!(days > 0))
                            throw new Exception("Days can't be zero...!!");

                        updateRoom.Discount = filterData.Discount > 0 ? filterData.Discount : updateRoom.Discount;

                        if (days > 0)
                        {
                            //var rent = roomInfo.Rent * days;

                            var rent = updateRoom.RoomRent * days;
                            var serviceCharge = updateRoom.RoomServiceCharge * days;

                            //var bedCharge = (updateRoom.ExtraBed * ExtraBedAmount.ExtraBedSingleCharge) * days;
                            updateRoom.Rent = rent;
                            updateRoom.ServiceCharge = serviceCharge;
                            updateRoom.ExtraBedCharge = filterData.ExtraBedCharge ?? 0;
                            updateRoom.NetRent = updateRoom.Rent + updateRoom.ServiceCharge + updateRoom.ExtraBedCharge - updateRoom.Discount;
                        }
                    }

                    checkOutDates.Add(updateRoom.CheckOutTime);
                }
            }
        }

        var maxCheckOutDate = checkOutDates.Max();
        bookingService.CheckOutTime = maxCheckOutDate;

        #region RoomStatusUpdate

        List<HtRoomInfo> changeStatusRoomList = new List<HtRoomInfo>();
        List<HtRoomInfo> checkOutRoomList = new List<HtRoomInfo>();

        var roomList = await _iRoomInfoRepository.GetAsync(x => !x.IsDeleted);

        if (updateableRoomList?.Count > 0)
        {
            foreach (var item in updateableRoomList)
            {
                var existInAlreadyCheckout = alreadyCheckOutRooms.Any(x => x.Id == item.Id);
                if (existInAlreadyCheckout)
                    continue;

                var roomInfo = roomList.FirstOrDefault(x => x.Id == item.RoomId);
                if (roomInfo == null)
                    throw new Exception("Room Not Found To Book..!!");

                roomInfo.BookingStatus = BookingStatusEnum.Available;
                roomInfo.CleaningStatus = CleaningStatusEnum.VD;
                roomInfo.HouseKeeperAvailabilityStatus = AvailabilityStatusEnum.CheckedOut;

                changeStatusRoomList.Add(roomInfo);

                if (item.ActualCheckOutTime != null)
                {
                    checkOutRoomList.Add(roomInfo);
                }
            }
        }

        #region RoomStatusHst

        var (changeHstList, addedHstList) = await GetRoomStatusHistory(changeStatusRoomList);

        #endregion

        #endregion

        #endregion

        bookingService.Rent = updateableRoomList.Sum(x => x.Rent);
        bookingService.ServiceCharge = updateableRoomList.Sum(x => x.ServiceCharge);
        bookingService.Vat = vm.Vat;
        bookingService.Tax = vm.Tax;

        var roomDiscount = updateableRoomList.Sum(x => x.Discount);
        bookingService.Discount = roomDiscount + vm.Discount;

        var extraBedCharge = updateableRoomList.Sum(x => x.ExtraBedCharge);
        var subRent = bookingService.Rent + bookingService.ServiceCharge + extraBedCharge;

        bookingService.NetRent = (subRent + bookingService.Vat + bookingService.Tax) - (bookingService.Discount);

        //Updated By Tawkir.. :: 17/03/2024
        var paidList = await _iBookingPaymentRepository.GetAsync(x => x.BookingId == bookingService.Id && !x.IsDeleted && x.PaidDate.Date <= Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.ActualCheckOutTimeStr)).Date);
        var alreadyPaidAmount = paidList.Sum(x => x.PaidAmount);

        #region Refund Amount

        HtAdvanceRefund refundModel = null;

        if (vm.IsRefund && alreadyPaidAmount > 0)
        {
            var refundAmount = alreadyPaidAmount;

            refundModel = new HtAdvanceRefund();
            refundModel.BookingId = bookingService.Id;
            refundModel.RefundAmount = refundAmount;
            refundModel.RefundMode = PayModeEnum.Cash;
            refundModel.RefundDate = Utility.GetBdDateTimeNow();
            refundModel.ActionById = CurrentUserId;
            refundModel.ActionDate = Utility.GetBdDateTimeNow();
            refundModel.TransactionNo = paidList != null && paidList.Count > 0 ? string.Join(", ", paidList.Select(x => x.TransactionNo)) : "";
            refundModel.Description = $"Advance Amount {refundModel.RefundAmount} is Adjusted.";

        }

        #endregion

        #region Bill_OLD

        //var generatedBill = await GenerateBill(bookingService, updateableRoomList);
        //if (generatedBill == null)
        //    throw new Exception("Can't Generated Bill..!!");

        //generatedBill.TotalAmount = generatedBill.BillingDetails.Sum(x => x.NetAmount);
        //generatedBill.Vat = vm.Vat;
        //generatedBill.Tax = vm.Tax;
        //generatedBill.Discount = vm.Discount;

        //generatedBill.PaidAmount = alreadyPaidAmount;

        //generatedBill.NetAmount = (generatedBill.TotalAmount + generatedBill.Vat + generatedBill.Tax) - (generatedBill.Discount);

        //if (generatedBill.PaidAmount == generatedBill.NetAmount)
        //{
        //    generatedBill.BillStatus = BillStatusEnum.PartialPaid;
        //}
        //else if (generatedBill.PaidAmount < generatedBill.NetAmount && generatedBill.PaidAmount != 0)
        //{
        //    generatedBill.BillStatus = BillStatusEnum.PartialPaid;
        //}
        //else if (generatedBill.PaidAmount == 0)
        //{
        //    generatedBill.BillStatus = BillStatusEnum.Fresh;
        //}

        //if (generatedBill.PaidAmount == generatedBill.NetAmount)
        //    bookingService.PaymentStatus = PaymentStatusEnum.FullPayment;
        //else if (generatedBill.PaidAmount < generatedBill.NetAmount)
        //    bookingService.PaymentStatus = PaymentStatusEnum.PartialPayment;

        #endregion

        #region Bill

        var updateBill = await UpdateBill(bookingService, updateableRoomList);
        if (updateBill == null)
            throw new Exception("Can't Update Bill..!!");

        updateBill.TotalAmount = updateBill.BillingDetails.Sum(x => Math.Round(x.Amount, 2));
        updateBill.Discount = updateBill.BillingDetails.Sum(x => Math.Round(x.Discount, 2));
        updateBill.ServiceCharge = updateBill.BillingDetails.Sum(x => Math.Round(x.ServiceCharge, 2));
        updateBill.Vat = updateBill.BillingDetails.Sum(x => Math.Round(x.VAT, 2));

        updateBill.SpecialDiscount = Math.Round(vm.SpecialDiscount, 2);

        var billNetAmount = (updateBill.TotalAmount + updateBill.ServiceCharge + updateBill.Vat) - (updateBill.Discount + updateBill.SpecialDiscount);

        updateBill.NetAmount = Math.Round(billNetAmount, 0);

        updateBill.PaidAmount = alreadyPaidAmount;
        updateBill.BillById = CurrentUserId;

        //comment by tawkir: 11/12/2025
        //if (updateBill.PaidAmount == updateBill.NetAmount)
        //    updateBill.BillStatus = BillStatusEnum.FullPaid;
        //else if (updateBill.PaidAmount < updateBill.NetAmount)
        //    updateBill.BillStatus = BillStatusEnum.PartialPaid;

        if (updateBill.PaidAmount == updateBill.NetAmount)
            updateBill.BillStatus = BillStatusEnum.PartialPaid;
        else if (updateBill.PaidAmount < updateBill.NetAmount && updateBill.PaidAmount > 0)
            updateBill.BillStatus = BillStatusEnum.PartialPaid;
        else if (updateBill.PaidAmount == 0)
            updateBill.BillStatus = BillStatusEnum.Fresh;

        if (updateBill.PaidAmount == updateBill.NetAmount)
            bookingService.PaymentStatus = PaymentStatusEnum.FullPayment;
        else if (updateBill.PaidAmount < updateBill.NetAmount)
            bookingService.PaymentStatus = PaymentStatusEnum.PartialPayment;

        #endregion

        var existBookingGuests = await _iBookingGuestRepository.GetAsync(x => x.BookingId == bookingService.Id && !x.IsDeleted, g => g.Guest);

        var mainGuest = existBookingGuests.FirstOrDefault(x => x.IsMain);

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        _iRepository.Update(bookingService);

        if (updateableRoomList?.Count > 0)
        {
            _iBookingRoomRepository.UpdateRange(updateableRoomList);
        }

        if (changeStatusRoomList?.Count > 0)
        {
            _iRoomInfoRepository.UpdateRange(changeStatusRoomList);
        }

        if (changeHstList?.Count > 0)
        {
            _iRoomStatusHistoryRepository.UpdateRange(changeHstList);
        }

        if (addedHstList?.Count > 0)
        {
            await _iRoomStatusHistoryRepository.AddRangeAsync(addedHstList);
        }

        if (refundModel != null)
        {
            await _iAdvanceRefundRepository.AddAsync(refundModel);
        }

        if (updateBill != null)
        {
            await _iBillRepository.UpdateAsync(updateBill);
        }

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return (false, 0); }

        string ntfMsgHtml = CheckOutNotificationMsg(bookingService, checkOutRoomList, $"{mainGuest.Guest.FirstName} {mainGuest?.Guest.LastName}", updateBill?.PaidAmount ?? 0);

        var ntfGenerated = await _iNtfMsgService.GenerateNtf(NotificationEventCode.CheckOutNtf, ntfMsgHtml, ntfMsgHtml);

        ts.Complete();
        return (true, updateBill.Id);
    }

    #endregion

    #region NoShow

    public async Task<(bool, long)> BookingNoShow(long bookingId)
    {
        if (!(bookingId > 0))
            throw new Exception("Booking Service Information Is Not Correct...!!");

        var bookingService = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == bookingId && x.BookingType == BookingType.Room && !x.IsDeleted);

        if (bookingService == null)
            throw new Exception("Booking Service Not Found...!!");

        if (bookingService.BookingStatus != BookingServiceStatusEnum.Booked)
            throw new Exception("Booking Service Is Not Booked...!!");

        bookingService.BookingStatus = BookingServiceStatusEnum.NoShow;

        #region Room

        List<HtBookingRoom> updateableRoomList = null;

        var existBookingRooms = await _iBookingRoomRepository.GetAsync(x => x.BookingId == bookingService.Id && !x.IsDeleted);

        if (existBookingRooms.Count > 0)
        {
            updateableRoomList = existBookingRooms.ToList();

            if (updateableRoomList?.Count > 0)
            {
                foreach (var updateRoom in updateableRoomList)
                {
                    updateRoom.CheckOutTime = updateRoom.CheckInTime;
                    //updateRoom.ActualCheckInTime = updateRoom.CheckInTime;
                    //updateRoom.ActualCheckOutTime = updateRoom.CheckInTime;

                    if (updateRoom.RoomId != null && updateRoom.RoomId > 0)
                    {
                        var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == updateRoom.RoomId && x.IsActive && !x.IsDeleted);
                        if (roomInfo == null)
                            throw new Exception("Room Not Found...!!");
                    }
                }
            }
        }

        #region RoomStatusUpdate

        List<HtRoomInfo> changeStatusRoomList = new List<HtRoomInfo>();

        var roomList = await _iRoomInfoRepository.GetAsync(x => !x.IsDeleted);

        if (updateableRoomList?.Count > 0)
        {
            foreach (var item in updateableRoomList)
            {
                var roomInfo = roomList.FirstOrDefault(x => x.Id == item.RoomId);
                if (roomInfo == null)
                    throw new Exception("Room Not Found To Book..!!");

                roomInfo.BookingStatus = BookingStatusEnum.Available;
                roomInfo.CleaningStatus = CleaningStatusEnum.VC;
                roomInfo.HouseKeeperAvailabilityStatus = AvailabilityStatusEnum.Available;

                changeStatusRoomList.Add(roomInfo);
            }
        }

        #endregion

        #endregion

        bookingService.Rent = updateableRoomList.Sum(x => x.Rent);
        bookingService.ServiceCharge = updateableRoomList.Sum(x => x.ServiceCharge);
        bookingService.Vat = bookingService.Vat;
        bookingService.Tax = bookingService.Tax;
        bookingService.Discount = bookingService.Discount;

        var subRent = bookingService.Rent + bookingService.ServiceCharge;

        bookingService.NetRent = (subRent + bookingService.Vat + bookingService.Tax) - (bookingService.Discount);

        //Updated By Tawkir.. :: 17/03/2024
        var paidList = await _iBookingPaymentRepository.GetAsync(x => x.BookingId == bookingService.Id && !x.IsDeleted && x.PaidDate.Date <= bookingService.CheckInTime.Date);
        var alreadyPaidAmount = paidList.Sum(x => x.PaidAmount);

        #region Refund Amount

        HtAdvanceRefund refundModel = null;

        if (alreadyPaidAmount > 0)
        {
            refundModel = new HtAdvanceRefund();
            refundModel.BookingId = bookingService.Id;
            refundModel.RefundAmount = alreadyPaidAmount;
            refundModel.RefundMode = PayModeEnum.Cash;
            refundModel.RefundDate = Utility.GetBdDateTimeNow();
            refundModel.ActionById = CurrentUserId;
            refundModel.ActionDate = Utility.GetBdDateTimeNow();
            refundModel.TransactionNo = paidList != null && paidList.Count > 0 ? string.Join(", ", paidList.Select(x => x.TransactionNo)) : "";
            refundModel.Description = $"No Show Advance Amount {refundModel.RefundAmount} is Adjusted.";
        }

        #endregion

        #region Bill

        var generatedBill = await GenerateNoShowBill(bookingService, updateableRoomList);
        if (generatedBill == null)
            throw new Exception("Can't Generated Bill..!!");

        //generatedBill.TotalAmount = generatedBill.BillingDetails.Sum(x => x.NetAmount);
        //generatedBill.Vat = vm.Vat;
        //generatedBill.Tax = vm.Tax;
        //generatedBill.Discount = vm.Discount;

        generatedBill.TotalAmount = alreadyPaidAmount;
        generatedBill.Vat = 0;
        generatedBill.Tax = 0;
        generatedBill.Discount = 0;

        generatedBill.PaidAmount = alreadyPaidAmount;

        generatedBill.NetAmount = (generatedBill.TotalAmount + generatedBill.Vat + generatedBill.Tax) - (generatedBill.Discount);

        if (generatedBill.PaidAmount == generatedBill.NetAmount)
            generatedBill.BillStatus = BillStatusEnum.FullPaid;
        else if (generatedBill.PaidAmount > generatedBill.NetAmount)
            generatedBill.BillStatus = BillStatusEnum.PartialPaid;

        #endregion

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        _iRepository.Update(bookingService);

        if (updateableRoomList?.Count > 0)
        {
            _iBookingRoomRepository.UpdateRange(updateableRoomList);
        }

        if (changeStatusRoomList?.Count > 0)
        {
            _iRoomInfoRepository.UpdateRange(changeStatusRoomList);
        }

        if (refundModel != null)
        {
            await _iAdvanceRefundRepository.AddAsync(refundModel);
        }

        if (generatedBill != null)
        {
            await _iBillRepository.AddAsync(generatedBill);
        }

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return (false, 0); }
        ts.Complete();
        return (true, generatedBill.Id);
    }

    #endregion

    #region BillGenerate
    private async Task<HtBilling> GenerateBill(HtBookingService booking, List<HtBookingRoom> roomList)
    {
        if (booking == null && roomList == null && !(roomList.Count > 0))
            return null;

        var model = new HtBilling();
        model.BillNumber = await GetBillNumber();
        model.BillDate = Utility.GetBdDateTimeNow();
        model.BillStatus = BillStatusEnum.Fresh;
        model.Remarks = $"Bill Generated For Booking No. {booking.BookingNo}";
        model.BookingId = booking.Id;
        model.BillById = CurrentUserId;
        model.ActionById = CurrentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();

        List<HtBillingDetail> modelDetails = new List<HtBillingDetail>();

        foreach (var bookingRoom in roomList)
        {
            var detailModel = new HtBillingDetail();

            var roomRentalService = await _iHtServiceRepository.GetFirstOrDefaultAsync(x => x.ServiceCode == HtServiceCode.RoomRent);
            if (roomRentalService == null)
                throw new Exception("Room Rent Service Not Found...!!");

            var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == bookingRoom.RoomId && x.IsActive && !x.IsDeleted);
            if (roomInfo == null)
                throw new Exception("Room Not Found...!!");

            double days = AppUtility.DaysDiffernceOnlyDate((DateTime)bookingRoom.ActualCheckOutTime, bookingRoom.CheckInTime);

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
            detailModel.ActionById = CurrentUserId;
            detailModel.ActionDate = Utility.GetBdDateTimeNow();
            //detailModel.Quantity = 1;
            detailModel.Rate = roomInfo.Rent;
            detailModel.Amount = bookingRoom.Rent;
            detailModel.VAT = bookingRoom.Vat;
            detailModel.Tax = bookingRoom.Tax;
            detailModel.Discount = bookingRoom.Discount;
            detailModel.ServiceCharge = bookingRoom.ServiceCharge;
            detailModel.ExtraBedCharge = bookingRoom.ExtraBedCharge;
            detailModel.NetAmount = bookingRoom.NetRent;
            detailModel.Remarks = $"Bill For Room No. {roomInfo.RoomNo}";

            modelDetails.Add(detailModel);
        }

        #region FoodService

        foreach (var bookingRoom in roomList)
        {
            var detailModel = new HtBillingDetail();

            var foodService = await _iHtServiceRepository.GetFirstOrDefaultAsync(x => x.ServiceCode == HtServiceCode.FoodService);
            if (foodService == null)
                throw new Exception("Food Service Not Found...!!");

            var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == bookingRoom.RoomId && x.IsActive && !x.IsDeleted);
            if (roomInfo == null)
                throw new Exception("Room Not Found...!!");

            var anyUnpaidFoodBillList = await _iFoodOrderRepository.GetAsync(x => x.RoomId == roomInfo.Id && x.BookingId == model.BookingId && x.PaymentStatus != RsOrderPaymentStatusEnum.FullPayment && x.OrderStatus != RsOrderStatusEnum.Canceled, c => c.CustomerType);

            anyUnpaidFoodBillList = anyUnpaidFoodBillList.Where(x => x.CustomerType.TypeCode == RsCustomerTypeCode.Hotel || x.CustomerType.TypeCode == RsCustomerTypeCode.WalkIn).ToList();

            if (anyUnpaidFoodBillList == null || !(anyUnpaidFoodBillList.Count > 0))
            {
                continue;
            }

            detailModel.BookingRoomId = bookingRoom.Id;
            detailModel.ServiceId = foodService.Id;
            detailModel.ActionById = CurrentUserId;
            detailModel.ActionDate = Utility.GetBdDateTimeNow();
            detailModel.Quantity = anyUnpaidFoodBillList.Count();

            detailModel.Amount = anyUnpaidFoodBillList.Sum(x => x.OrderAmount);
            detailModel.VAT = anyUnpaidFoodBillList.Sum(x => x.VAT);
            detailModel.Tax = anyUnpaidFoodBillList.Sum(x => x.TAX);
            detailModel.Discount = anyUnpaidFoodBillList.Sum(x => x.Discount);
            detailModel.ServiceCharge = anyUnpaidFoodBillList.Sum(x => x.ServiceCharge);

            var unpaidIds = anyUnpaidFoodBillList.Select(x => x.Id).ToList();
            var paidList = _iOrderPaymentRepository.Get(c => unpaidIds.Contains(c.OrderId) && c.PaymentType == RsOrderPaymentTypeEnum.Receive && !c.IsDeleted).ToList();
            var alreadyPaidAmount = paidList.Sum(x => x.PaidAmount);

            detailModel.NetAmount = anyUnpaidFoodBillList.Sum(x => x.NetAmount) - (alreadyPaidAmount);

            detailModel.Remarks = $"Food Service Bill For Room No. {roomInfo.RoomNo}";

            modelDetails.Add(detailModel);
        }

        #endregion

        model.BillingDetails = modelDetails;

        return model;
    }

    private async Task<HtBilling> GenerateNoShowBill(HtBookingService booking, List<HtBookingRoom> roomList)
    {
        if (booking == null && roomList == null && !(roomList.Count > 0))
            return null;

        var model = new HtBilling();
        model.BillNumber = await GetBillNumber();
        model.BillDate = Utility.GetBdDateTimeNow();
        model.BillStatus = BillStatusEnum.FullPaid;
        model.Remarks = $"Bill For No Show Booking No. {booking.BookingNo}";
        model.BookingId = booking.Id;
        model.BillById = CurrentUserId;
        model.ActionById = CurrentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();

        List<HtBillingDetail> modelDetails = new List<HtBillingDetail>();

        foreach (var bookingRoom in roomList)
        {
            var detailModel = new HtBillingDetail();

            var roomRentalService = await _iHtServiceRepository.GetFirstOrDefaultAsync(x => x.ServiceCode == HtServiceCode.RoomRent);
            if (roomRentalService == null)
                throw new Exception("Room Rent Service Not Found...!!");

            var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == bookingRoom.RoomId && x.IsActive && !x.IsDeleted);
            if (roomInfo == null)
                throw new Exception("Room Not Found...!!");

            detailModel.BookingRoomId = bookingRoom.Id;
            detailModel.ServiceId = roomRentalService.Id;
            detailModel.ActionById = CurrentUserId;
            detailModel.ActionDate = Utility.GetBdDateTimeNow();
            detailModel.Quantity = 1;
            detailModel.Rate = roomInfo.Rent;
            detailModel.Amount = bookingRoom.Rent;
            detailModel.VAT = bookingRoom.Vat;
            detailModel.Tax = bookingRoom.Tax;
            detailModel.Discount = bookingRoom.Discount;
            detailModel.ServiceCharge = bookingRoom.ServiceCharge;
            detailModel.ExtraBedCharge = bookingRoom.ExtraBedCharge;
            detailModel.NetAmount = bookingRoom.NetRent;
            detailModel.Remarks = $"Bill For No Show Room No. {roomInfo.RoomNo}";

            modelDetails.Add(detailModel);
        }

        model.BillingDetails = modelDetails;

        return model;
    }

    private async Task<HtBilling> GenerateHallBill(HtBookingService booking, List<HtBookingHall> hallList)
    {
        if (booking == null && hallList == null && !(hallList.Count > 0))
            return null;

        var model = new HtBilling();
        model.BillNumber = await GetBillNumber();
        model.BillDate = Utility.GetBdDateTimeNow();
        model.BillStatus = BillStatusEnum.Fresh;
        model.Remarks = $"Bill For Hall Booking No. {booking.BookingNo}";
        model.BookingId = booking.Id;
        model.BillById = CurrentUserId;
        model.ActionById = CurrentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();

        List<HtBillingDetail> modelDetails = new List<HtBillingDetail>();

        foreach (var bookingHall in hallList)
        {
            var detailModel = new HtBillingDetail();

            var hallRentalService = await _iHtServiceRepository.GetFirstOrDefaultAsync(x => x.ServiceCode == HtServiceCode.HallRent);
            if (hallRentalService == null)
                throw new Exception("Hall Rent Service Not Found...!!");

            var hallInfo = _iHallInfoRepository.GetFirstOrDefault(x => x.Id == bookingHall.HallId && x.IsActive && !x.IsDeleted);
            if (hallInfo == null)
                throw new Exception("Hall Not Found...!!");

            detailModel.BookingHallId = bookingHall.Id;
            detailModel.ServiceId = hallRentalService.Id;
            detailModel.ActionById = CurrentUserId;
            detailModel.ActionDate = Utility.GetBdDateTimeNow();
            detailModel.Quantity = 1;
            detailModel.Rate = bookingHall.Rent;
            detailModel.Amount = bookingHall.Rent;
            detailModel.VAT = bookingHall.Vat;
            detailModel.Tax = bookingHall.Tax;
            detailModel.Discount = bookingHall.Discount;
            detailModel.NetAmount = bookingHall.NetRent;
            detailModel.Remarks = $"Bill For Hall Booking, Hall Name: {hallInfo.HallName}";

            modelDetails.Add(detailModel);
        }

        model.BillingDetails = modelDetails;

        return model;
    }

    public async Task<string> GetBillNumber()
    {
        var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.HtBillings.ToString(), "BillNumber", "INV", 5);
        return data;
    }

    #endregion

    #region DemoBill

    private async Task<BillingVm> GenerateDemoBill(HtBookingService booking, List<HtBookingRoom> roomList)
    {
        if (booking == null && roomList == null && !(roomList.Count > 0))
            return null;

        var model = new BillingVm();
        model.BillNumber = await GetBillNumber();
        model.BillDate = Utility.GetBdDateTimeNow();
        model.BillStatus = BillStatusEnum.Fresh;
        model.Remarks = $"Bill Generated For Booking No. {booking.BookingNo}";
        model.BookingId = booking.Id;
        model.BillById = CurrentUserId;

        List<BillingDetailVm> modelDetails = new List<BillingDetailVm>();

        foreach (var bookingRoom in roomList)
        {
            var detailModel = new BillingDetailVm();

            var roomRentalService = await _iHtServiceRepository.GetFirstOrDefaultAsync(x => x.ServiceCode == HtServiceCode.RoomRent);
            if (roomRentalService == null)
                throw new Exception("Room Rent Service Not Found...!!");

            var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == bookingRoom.RoomId && x.IsActive && !x.IsDeleted);
            if (roomInfo == null)
                throw new Exception("Room Not Found...!!");

            double days = AppUtility.DaysDiffernceOnlyDate((DateTime)bookingRoom.CheckOutTime, bookingRoom.CheckInTime);

            if (bookingRoom.BookingDayStatus == (int)BookingDayStatusEnum.HalfDay)
            {
                days = (days + 0.5);
            }
            else if (bookingRoom.BookingDayStatus == (int)BookingDayStatusEnum.DayUse)
            {
                days = (days + 1);
            }

            detailModel.Quantity = days;

            var perDayDiscount = bookingRoom.Discount > 0 ? (bookingRoom.Discount / days) : 0;

            detailModel.BookingRoomId = bookingRoom.Id;
            detailModel.ServiceId = roomRentalService.Id;
            detailModel.ServiceName = roomRentalService.ServiceName;
            detailModel.Rate = roomInfo.Rent;
            //detailModel.Rate = (bookingRoom.RoomRent - perDayDiscount);
            detailModel.Amount = bookingRoom.Rent;
            detailModel.VAT = bookingRoom.Vat;
            detailModel.Tax = bookingRoom.Tax;
            detailModel.Discount = bookingRoom.Discount;
            detailModel.ServiceCharge = bookingRoom.ServiceCharge;
            detailModel.ExtraBedCharge = bookingRoom.ExtraBedCharge;
            detailModel.NetAmount = bookingRoom.NetRent;
            detailModel.Remarks = $"Bill For Room No. {roomInfo.RoomNo}";

            modelDetails.Add(detailModel);
        }

        #region FoodService

        foreach (var bookingRoom in roomList)
        {
            var detailModel = new BillingDetailVm();

            var foodService = await _iHtServiceRepository.GetFirstOrDefaultAsync(x => x.ServiceCode == HtServiceCode.FoodService);
            if (foodService == null)
                throw new Exception("Food Service Not Found...!!");

            var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == bookingRoom.RoomId && x.IsActive && !x.IsDeleted);
            if (roomInfo == null)
                throw new Exception("Room Not Found...!!");

            var anyUnpaidFoodBillList = await _iFoodOrderRepository.GetAsync(x => x.RoomId == roomInfo.Id && x.BookingId == model.BookingId && x.PaymentStatus != RsOrderPaymentStatusEnum.FullPayment && x.OrderStatus != RsOrderStatusEnum.Canceled, c => c.CustomerType);

            anyUnpaidFoodBillList = anyUnpaidFoodBillList.Where(x => x.CustomerType.TypeCode == RsCustomerTypeCode.Hotel || x.CustomerType.TypeCode == RsCustomerTypeCode.WalkIn).ToList();

            if (anyUnpaidFoodBillList == null || !(anyUnpaidFoodBillList.Count > 0))
            {
                continue;
            }

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
            var paidList = _iOrderPaymentRepository.Get(c => unpaidIds.Contains(c.OrderId) && c.PaymentType == RsOrderPaymentTypeEnum.Receive && !c.IsDeleted).ToList();
            var alreadyPaidAmount = paidList.Sum(x => x.PaidAmount);

            detailModel.NetAmount = anyUnpaidFoodBillList.Sum(x => x.NetAmount) - (alreadyPaidAmount);

            detailModel.Remarks = $"Food Service Bill For Room No. {roomInfo.RoomNo}";

            modelDetails.Add(detailModel);
        }

        #endregion

        model.BillingDetails = modelDetails;

        model.TotalAmount = model.BillingDetails.Sum(x => x.Amount);
        model.Discount = model.BillingDetails.Sum(x => x.Discount);
        model.NetAmount = model.BillingDetails.Sum(x => x.NetAmount);

        return model;
    }

    private async Task<BillingVm> GenerateCurrentBill(long bookingId)
    {
        if (!(bookingId > 0))
            throw new Exception("Booking Information is not correct..!!");

        var booking = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == bookingId && !x.IsDeleted);
        if (booking == null)
            throw new Exception("Booking Service Not Found...!!");

        var bookingRoomList = await _iBookingRoomRepository.GetAsync(x => x.BookingId == booking.Id && !x.IsDeleted, r => r.Room);
        if (!(bookingRoomList.Count > 0))
            throw new Exception("No room found in this booking!!");

        var bill = await _iBillRepository.GetFirstOrDefaultAsync(x => x.BookingId == booking.Id && !x.IsDeleted,
                                                                        query => query.Include(u => u.ActionBy)
                                                                                      .Include(d => d.BillingDetails)
                                                                                           .ThenInclude(bd => bd.Service));
        if (bill == null)
            throw new Exception("Bill Not Found For This Booking...!!");

        var allServiceList = await _iHtServiceRepository.GetAsync(x => !x.IsDeleted);

        var model = _iMapper.Map<BillingVm>(bill);

        //model.BillNumber = await GetBillNumber();
        //model.BillDate = Utility.GetBdDateTimeNow();
        //model.BillStatus = BillStatusEnum.Fresh;
        //model.Remarks = $"Bill Generated For Booking No. {booking.BookingNo}";
        //model.BookingId = booking.Id;
        //model.BillById = CurrentUserId;

        model.BookingNo = booking.BookingNo;
        model.BookingDate = booking.BookingDate;
        model.BookingType = booking.BookingType;
        model.BillByName = bill?.ActionBy?.FullName;

        var extraServiceDetails = bill.BillingDetails.Select(x => x.Service).Where(x => x.ServiceCode != HtServiceCode.RoomRent && x.ServiceCode != HtServiceCode.FoodService && x.ServiceCode != HtServiceCode.HallRent).ToList();
        if (extraServiceDetails.Count > 0)
        {
            foreach (var service in extraServiceDetails)
            {
                var matchedService = allServiceList.FirstOrDefault(x => x.Id == service.Id);
                var billingDetail = model.BillingDetails.FirstOrDefault(x => x.ServiceId == service.Id);

                if (matchedService != null && billingDetail != null)
                {
                    billingDetail.ServiceName = matchedService.ServiceName;
                    billingDetail.ServiceCode = matchedService.ServiceCode;
                }
            }
        }

        #region RoomService

        if (bookingRoomList.Count > 0)
        {
            var roomIds = bookingRoomList.Select(x => x.RoomId).ToList();
            var roomList = await _iRoomInfoRepository.GetAsync(x => roomIds.Contains(x.Id) && x.IsActive && !x.IsDeleted);

            foreach (var bookingRoom in bookingRoomList)
            {
                var detailModel = new BillingDetailVm();

                if (bookingRoom.ActualCheckOutTime != null)
                    continue;

                var roomRentalService = allServiceList.FirstOrDefault(x => x.ServiceCode == HtServiceCode.RoomRent);
                if (roomRentalService == null)
                    throw new Exception("Room Rent Service Not Found...!!");

                var roomInfo = roomList.FirstOrDefault(x => x.Id == bookingRoom.RoomId);
                if (roomInfo == null)
                    throw new Exception("Room Not Found...!!");

                DateTime checkInTime = bookingRoom.ActualCheckInTime != null ? (DateTime)bookingRoom.ActualCheckInTime : bookingRoom.CheckInTime;
                DateTime checkOutTime = bookingRoom.ActualCheckOutTime != null ? (DateTime)bookingRoom.ActualCheckOutTime : DateTime.Now.Date;

                if (checkOutTime.Date == checkInTime.Date)
                    checkOutTime = checkInTime.Date.AddDays(1);

                double actualDays = AppUtility.DaysDiffernceOnlyDate(bookingRoom.CheckOutTime, bookingRoom.CheckInTime);
                double days = AppUtility.DaysDiffernceOnlyDate(checkOutTime, checkInTime);

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
                detailModel.ServiceName = roomRentalService.ServiceName;
                detailModel.ServiceCode = roomRentalService.ServiceCode;
                detailModel.Rate = roomInfo.Rent;
                detailModel.Amount = bookingRoom.Rent;
                detailModel.VAT = bookingRoom.Vat;
                detailModel.Tax = bookingRoom.Tax;
                detailModel.Discount = bookingRoom.Discount;
                detailModel.ServiceCharge = bookingRoom.ServiceCharge;
                detailModel.ExtraBedCharge = bookingRoom.ExtraBedCharge;
                detailModel.NetAmount = bookingRoom.NetRent;
                detailModel.Remarks = $"Bill For Room No. {roomInfo.RoomNo}";

                if (booking.BookingStatus == BookingServiceStatusEnum.NoShow)
                {
                    detailModel.BookingRoomCheckOutTime = bookingRoom?.CheckOutTime;
                }

                model.BillingDetails.Add(detailModel);
            }
        }

        #endregion

        #region FoodService

        foreach (var bookingRoom in bookingRoomList)
        {
            var detailModel = new BillingDetailVm();

            var foodService = await _iHtServiceRepository.GetFirstOrDefaultAsync(x => x.ServiceCode == HtServiceCode.FoodService);
            if (foodService == null)
                throw new Exception("Food Service Not Found...!!");

            var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == bookingRoom.RoomId && x.IsActive && !x.IsDeleted);
            if (roomInfo == null)
                throw new Exception("Room Not Found...!!");

            var anyUnpaidFoodBillList = await _iFoodOrderRepository.GetAsync(x => x.RoomId == roomInfo.Id && x.BookingId == model.BookingId && x.PaymentStatus != RsOrderPaymentStatusEnum.FullPayment && x.OrderStatus != RsOrderStatusEnum.Canceled, c => c.CustomerType);

            anyUnpaidFoodBillList = anyUnpaidFoodBillList.Where(x => x.CustomerType.TypeCode == RsCustomerTypeCode.Hotel || x.CustomerType.TypeCode == RsCustomerTypeCode.WalkIn).ToList();

            if (anyUnpaidFoodBillList == null || !(anyUnpaidFoodBillList.Count > 0))
            {
                continue;
            }

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
            var paidList = _iOrderPaymentRepository.Get(c => unpaidIds.Contains(c.OrderId) && c.PaymentType == RsOrderPaymentTypeEnum.Receive && !c.IsDeleted).ToList();
            var alreadyPaidAmount = paidList.Sum(x => x.PaidAmount);

            detailModel.NetAmount = anyUnpaidFoodBillList.Sum(x => x.NetAmount) - (alreadyPaidAmount);

            detailModel.Remarks = $"Food Service Bill For Room No. {roomInfo.RoomNo}";

            model.BillingDetails.Add(detailModel);
        }

        #endregion

        model.TotalAmount = model.BillingDetails.Sum(x => x.Amount);
        model.Discount = model.BillingDetails.Sum(x => x.Discount);
        model.NetAmount = model.BillingDetails.Sum(x => x.NetAmount);
        model.TotalGuest = bookingRoomList.Sum(x => x.TotalGuest);
        return model;
    }

    public async Task<string> GetBillDetailHtmlById(long bookingId)
    {
        if (!(bookingId > 0))
            throw new Exception("Booking Service Information Is Not Correct...!!");

        var bookingService = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == bookingId && x.BookingType == BookingType.Room && !x.IsDeleted);
        var existBookingRooms = await _iBookingRoomRepository.GetAsync(x => x.BookingId == bookingId && !x.IsDeleted, r => r.Room.RoomCategory);

        //var model = await GenerateDemoBill(bookingService, existBookingRooms.ToList());

        var model = await GenerateCurrentBill(bookingId);

        //model.BookingNo = bookingService.BookingNo;
        //model.BookingDate = bookingService.BookingDate;
        //model.BookingType = bookingService.BookingType;
        model.BillByName = "";

        var bookingGuest = await _iBookingGuestRepository.GetFirstOrDefaultAsync(x => x.BookingId == model.BookingId && x.IsMain && !x.IsDeleted, g => g.Guest);
        if (bookingGuest == null)
            throw new Exception("Guest Not Found For Billing...!!");

        model.BookingGuestName = $"{bookingGuest.Guest.Salutation} {bookingGuest.Guest.FirstName} {bookingGuest.Guest.LastName}";
        model.BookingGuestMobile = bookingGuest.Guest.Mobile;
        model.BookingGuestAddress = bookingGuest.Guest.Address;
        //model.TotalGuest = bookingService.TotalGuest;

        if (model.BillingDetails.Count > 0)
        {
            foreach (var dtl in model.BillingDetails)
            {
                var filterData = existBookingRooms.FirstOrDefault(c => c.Id == dtl.BookingRoomId);

                dtl.ServiceName = dtl.ServiceName;
                dtl.ServiceCode = dtl.ServiceCode;
                dtl.BookingRoomCheckInTime = filterData?.CheckInTime;
                dtl.BookingRoomCheckOutTime = filterData?.CheckOutTime;

                if (bookingService.BookingStatus == BookingServiceStatusEnum.NoShow)
                {
                    dtl.BookingRoomCheckOutTime = filterData?.CheckOutTime;
                }

                dtl.Days = dtl.BookingRoomCheckInTime != null ? AppUtility.DaysDiffernceOnlyDate((DateTime)dtl.BookingRoomCheckOutTime, (DateTime)dtl.BookingRoomCheckInTime) : 0;


                if (dtl.BookingDayStatus == (int)BookingDayStatusEnum.HalfDay)
                {
                    dtl.Days = dtl.Days + 0.5;
                }
                else if (dtl.BookingDayStatus == (int)BookingDayStatusEnum.DayUse)
                {
                    dtl.Days = dtl.Days + 1;
                }

                dtl.BookingRoomNo = filterData.Room.RoomNo;
                dtl.BookingRoomRent = filterData.Rent;
                dtl.BookingRoomDiscount = filterData?.Discount;
                dtl.BookingRoomServiceCharge = filterData?.ServiceCharge;
                dtl.BookingRoomExtraBedCharge = filterData?.ExtraBedCharge;
                dtl.BookingRoomExtraBed = filterData?.ExtraBed;

                if (bookingService.BookingType == BookingType.Hall)
                {
                    dtl.Days = dtl.BookingHallBookShift == 3 ? 2 : 1;
                }

                dtl.BookingRoomCategoryId = filterData?.Room.RoomCategoryId;
                dtl.BookingRoomCategoryName = filterData?.Room.RoomCategory.CategoryName;
                dtl.BookingDayStatus = (filterData?.BookingDayStatus != null) ? Convert.ToInt32(filterData?.BookingDayStatus) : 0;
            }
        }

        model.SetBillStatusInfo();

        #region MrList

        var paidList = await _iBookingPaymentRepository.GetAsync(x => x.BookingId == model.BookingId && !x.IsDeleted);
        model.BillingPayments = _iMapper.Map<List<HtBookingPaymentVm>>(paidList);
        var advanceList = paidList.Where(x => x.BillingId == null).ToList();

        model.AdvanceAmount = advanceList.Sum(x => x.PaidAmount);
        model.MrList = advanceList.Count > 0 ? string.Join(",", advanceList.Select(x => x.TransactionNo)) : "";

        #endregion

        string fullHtml = await _iBillRepository.GetBillDetailHtmlById(model);
        return fullHtml;
    }
    #endregion

    #region BookingPayment

    public async Task<bool> PaymentEntry(BookingPaymentDto payment)
    {
        if (payment == null && payment.BookingId > 0)
            throw new Exception("Payemnt Information Is Not Correct...!!");

        var bookingService = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == payment.BookingId && !x.IsDeleted);

        if (bookingService == null)
            throw new Exception("Booking Service Not Found...!!");

        if (bookingService.BookingStatus == BookingServiceStatusEnum.CheckOut && bookingService.PaymentStatus == PaymentStatusEnum.FullPayment)
            throw new Exception("Can't pay the amount cause payment is fully paid...!!");

        var model = _iMapper.Map<HtBookingPayment>(payment);
        model.PaidDate = (DateTime)(!string.IsNullOrEmpty(payment.PaidDateStr) ? Utility.ConvertStrToDate(payment.PaidDateStr) : DU.Utility.GetBdDateTimeNow());
        model.ActionById = CurrentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();
        model.Description = $"Amount {payment.PaidAmount} is paid partialy.";
        model.IsAdvance = true;

        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        var reportDate = Utility.GenerateReportDate(model.PaidDate.Add(currentTime));
        model.ReportDate = reportDate;

        bookingService.PaymentStatus = PaymentStatusEnum.PartialPayment;

        var paidList = _iBookingPaymentRepository.Get(c => c.BookingId == bookingService.Id).ToList();
        var alreadyPaidAmount = paidList.Sum(x => x.PaidAmount);

        var totalAmount = alreadyPaidAmount + model.PaidAmount;

        var billInfo = await _iBillRepository.GetFirstOrDefaultAsync(x => x.BookingId == bookingService.Id);
        if (billInfo != null)
            billInfo.PaidAmount = totalAmount;

        AccTranMst paymentVoucher = null;

        var novDate = new DateTime(2024, 11, 29);
        if (model.PaidDate.Date > novDate.Date)
        {

            paymentVoucher = await GetAdvancePaymentQuickVoucher(model, bookingService.BookingNo);//For quick voucher system

            if (bookingService.BookingType == BookingType.Hall)
            {
                paymentVoucher = await GetAdvancePaymentQuickVoucher(model, bookingService.BookingNo, isHallBooking: true);//For quick voucher system
            }

            if (paymentVoucher == null)
                throw new Exception("Somthing Went Wrong Creating Voucher..!!");
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _iBookingPaymentRepository.AddAsync(model);
        await _iUnitOfWork.CompleteAsync();

        if (paymentVoucher != null)
        {
            paymentVoucher.PaymentId = model.Id;
            await _iAccTranMstRepository.AddAsync(paymentVoucher);
        }

        if (billInfo != null)
        {
            await _iBillRepository.UpdateAsync(billInfo);
        }

        _iRepository.Update(bookingService);

        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return false; }

        ts.Complete();
        return true;
    }

    #endregion

    #region GetAdvancePaymentVoucher

    private async Task<AccTranMst> GetAdvancePaymentVoucher(HtBookingPayment payment, string bookingNo)
    {
        var model = new AccTranMst();
        model.VcDate = payment.PaidDate;
        if (payment.PayMode == PayModeEnum.Bank)
        {
            model.VcType = VoucherType.BankDebitVoucher;
            model.SubVacType = VoucherType.BankDebitVoucher;
            model.VcNo = await _iAutoCodeRepository.GetVoucherAutoNo(VoucherTypeCode.BankDebitVoucher, model.VcDate);
        }
        else if (payment.PayMode == PayModeEnum.Cash)
        {
            model.VcType = VoucherType.CashDebitVoucher;
            model.SubVacType = VoucherType.CashDebitVoucher;
            model.VcNo = await _iAutoCodeRepository.GetVoucherAutoNo(VoucherTypeCode.CashDebitVoucher, model.VcDate);
        }



        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.FinYearId = (await _iSetFincYearService.GetFincYearByDate(model.VcDate)).Id;
        model.Narration = $"Room advance rent receive. Booking No:{bookingNo}";
        model.ActionById = CurrentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();

        var accTranList = new List<AccTranDtl>();

        if (payment != null)
        {
            var modelDtl = new AccTranDtl();

            var ladger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.HallAdvanceRentReceive);
            if (ladger == null)
                throw new Exception("No Ledger Found Against Room Advance Receive..!!");

            AccLedger drLadger = null;

            if (payment.PayMode == PayModeEnum.Bank)
            {
                var bankLadger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.CityBankLtd);
                if (bankLadger == null)
                    throw new Exception("No Bank Ledger Found Against Bank Account..!!");

                drLadger = bankLadger;
            }
            else if (payment.PayMode == PayModeEnum.Cash)
            {
                var cashLadger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.HotelMisuk);
                if (cashLadger == null)
                    throw new Exception("No Cash In Hand Ledger Found..!!");

                drLadger = cashLadger;
            }

            modelDtl.AmountDr = payment.PaidAmount;
            modelDtl.AmountCr = payment.PaidAmount;
            modelDtl.LedgerDrId = drLadger.Id;
            modelDtl.LedgerCrId = ladger.Id;
            modelDtl.ActionById = CurrentUserId;
            modelDtl.ActionDate = Utility.GetBdDateTimeNow();

            accTranList.Add(modelDtl);

            model.AccTranDtls = accTranList;
        }

        model.TotalAmount = model.AccTranDtls.Sum(c => c.AmountDr);

        return model;
    }

    #endregion

    #region GetAdvancePaymentQuickVoucher

    private async Task<AccTranMst> GetAdvancePaymentQuickVoucher(HtBookingPayment payment, string bookingNo, bool isHallBooking = false)
    {
        var model = new AccTranMst();
        model.VcDate = payment.PaidDate;
        model.VcType = VoucherType.JournalVoucher;
        model.SubVacType = VoucherType.JournalVoucher;
        model.VcNo = await _iAutoCodeRepository.GetVoucherAutoNo(VoucherTypeCode.JournalVoucher, model.VcDate);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.FinYearId = (await _iSetFincYearService.GetFincYearByDate(model.VcDate)).Id;
        model.Narration = $"Room rent advance receive. Booking No:{bookingNo}";
        model.ActionById = CurrentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();
        model.IsAuto = true;

        var accTranList = new List<AccTranDtl>();

        if (payment != null)
        {
            var modelDtl = new AccTranDtl();

            var crLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.HotelMisuk);
            var drLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.RoomAdvanceRentReceive);

            if (isHallBooking)
            {
                drLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.HallAdvanceRentReceive);
            }

            if (crLedger == null)
                throw new Exception("No Ledger Found Against Room Advance Receive..!!");
            if (drLedger == null)
                throw new Exception("No Debit Ledger Found !!");

            model.AccAccountId = crLedger.Id;

            modelDtl.AmountDr = payment.PaidAmount;
            modelDtl.AmountCr = payment.PaidAmount;
            //modelDtl.LedgerDrId = drLedger.Id;
            //modelDtl.LedgerCrId = crLedger.Id;
            modelDtl.LedgerDrId = crLedger.Id;
            modelDtl.LedgerCrId = drLedger.Id;
            modelDtl.ActionById = CurrentUserId;
            modelDtl.ActionDate = Utility.GetBdDateTimeNow();

            accTranList.Add(modelDtl);

            model.AccTranDtls = accTranList;
        }

        model.TotalAmount = model.AccTranDtls.Sum(c => c.AmountDr);

        return model;
    }

    #endregion

    #region GetBookingServiceReportHtml

    public async Task<string> GetBookingServiceReportHtml(BookingServiceReportVm vm, bool isPrint)
    {
        string fullHtml = await _iHotelManagementRepository.GetBookingReportHtml(vm, isPrint);
        return fullHtml;
    }

    #endregion

    #region GetDashBoardData

    public async Task<IndexRoomVm> GetDashBoardData()
    {
        var result = await _iRepository.GetDashboardDataAsync();
        return result;
    }

    #endregion

    #region GetRoomAvailabilityByDate
    public async Task<List<HtRoomInfo>> GetAvailableRoomByDate(DateTime selectedDate)
    {
        // All booking  
        var bookingList = await _iRepository.GetAsync(x => x.CheckInTime.Date == selectedDate.Date || x.CheckOutTime.Date == selectedDate.Date && x.BookingType == BookingType.Room && !x.IsDeleted);

        var checkInList = bookingList.Where(x => x.BookingStatus == BookingServiceStatusEnum.CheckIn).ToList();
        var checkOutList = bookingList.Where(x => x.BookingStatus == BookingServiceStatusEnum.CheckOut).ToList();

        var checkInListIds = checkInList.Select(x => x.Id);
        var checkOutListIds = checkOutList.Select(x => x.Id);

        var unavailableRoomIds = (await _iBookingRoomRepository.GetAsync(c => checkInListIds.Contains(c.BookingId) && !c.IsDeleted)).Select(x => x.RoomId).ToList();
        var availableRoomIds = (await _iBookingRoomRepository.GetAsync(c => checkOutListIds.Contains(c.BookingId) && !c.IsDeleted)).Select(x => x.RoomId).ToList();

        var totalAvailableRoomList = await _iRoomInfoRepository.GetAsync(x => !unavailableRoomIds.Contains(x.Id) && availableRoomIds.Contains(x.Id));

        return totalAvailableRoomList.OrderByDescending(x => x.RoomNo).ToList();
    }
    #endregion

    #region GetRoomAvailabilityByDate
    public async Task<List<RoomReportVm>> GetRoomAvailabilityByDate(DateTime selectedDate, int roomStatus = 0, int cleanStatus = 0)
    {
        var dataList = new List<RoomReportVm>();

        // All booking  
        var bookingList = await _iRepository.GetAsync(x => x.CheckInTime.Date <= selectedDate.Date && x.CheckOutTime.Date >= selectedDate.Date && !x.IsDeleted
        && x.BookingType == BookingType.Room && !x.IsDeleted);

        //remove cancel booking and no show booking
        bookingList = bookingList.Where(x => x.BookingStatus != BookingServiceStatusEnum.Canceled && x.BookingStatus != BookingServiceStatusEnum.NoShow).ToList();

        var bookingListIds = bookingList.Select(x => x.Id);
        var bookingRoomList = await _iBookingRoomRepository.GetAsync(c => bookingListIds.Contains(c.BookingId) && !c.IsDeleted, x => x.Booking, r => r.Room);

        bookingRoomList = bookingRoomList.Where(x => x.CheckInTime.Date <= selectedDate.Date && x.CheckOutTime.Date >= selectedDate.Date && x.ActualCheckOutTime is null).ToList();

        //Added By Tawkir: 18/02/2025: For same room booking & occupied issue.
        bookingRoomList = bookingRoomList.OrderBy(x => x.CheckInTime).ToList();

        var bookedRoomList = bookingRoomList.Select(x => x.Room.RoomNo).ToList();

        var totalRooms = await _iRoomInfoRepository.GetAsync(x => x.IsActive && !x.IsDeleted);


        if (totalRooms.Count > 0)
        {
            foreach (var room in totalRooms)
            {
                var filterData = bookingRoomList.FirstOrDefault(x => x.RoomId == room.Id);

                var model = new RoomReportVm();
                model.RoomId = room.Id;
                model.RoomNo = room.RoomNo;
                model.CleaningStatus = (int)room.CleaningStatus;

                /*
                 Status: 
                    1 = Booked
                    2 = Occupied
                    3 = Available
                    4 = Out Of Order
                    5 = Vacant & Dirty
                */

                if (filterData != null)
                {
                    var bookingStatus = filterData.Booking.BookingStatus;

                    if (bookingStatus == BookingServiceStatusEnum.Booked)
                    {
                        model.Status = 1;
                    }
                    else if (bookingStatus == BookingServiceStatusEnum.CheckIn)
                    {
                        if (filterData.ActualCheckInTime != null && filterData.ActualCheckOutTime == null)
                        {
                            model.Status = 2;

                            if (filterData.CheckOutTime.Date == selectedDate.Date)
                                model.IsTodayCheckout = true;
                            else
                                model.IsTodayCheckout = false;
                        }
                        else if (filterData.ActualCheckInTime != null && filterData.ActualCheckOutTime != null)
                        {
                            if (room.CleaningStatus == CleaningStatusEnum.OOO)
                            {
                                model.Status = 4;
                            }
                            else if (room.CleaningStatus == CleaningStatusEnum.VD)
                            {
                                model.Status = 5;
                            }
                            else if (room.CleaningStatus == CleaningStatusEnum.VC)
                            {
                                model.Status = 3;
                            }
                        }
                        else if (filterData.ActualCheckInTime == null && filterData.ActualCheckOutTime == null)
                        {
                            model.Status = 1;
                        }
                    }
                    else if (bookingStatus == BookingServiceStatusEnum.CheckOut)
                    {
                        if (room.CleaningStatus == CleaningStatusEnum.OOO)
                        {
                            model.Status = 4;
                        }
                        else if (room.CleaningStatus == CleaningStatusEnum.VD)
                        {
                            model.Status = 5;
                        }
                        else if (room.CleaningStatus == CleaningStatusEnum.VC)
                        {
                            model.Status = 3;
                        }
                    }
                }
                else
                {
                    if (room.CleaningStatus == CleaningStatusEnum.OOO)
                    {
                        model.Status = 4;
                    }
                    else if (room.CleaningStatus == CleaningStatusEnum.VD)
                    {
                        model.Status = 5;
                    }
                    //else if (room.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.Occupied)
                    //{
                    //    model.Status = 2;
                    //}
                    else if (room.CleaningStatus == CleaningStatusEnum.VC)
                    {
                        model.Status = 3;
                    }
                    else
                    {
                        model.Status = 3;
                    }
                }

                dataList.Add(model);
            }
        }

        if (roomStatus > 0 && cleanStatus > 0)
            dataList = dataList.Where(o => o.Status == roomStatus || o.CleaningStatus == cleanStatus).ToList();
        else if (roomStatus > 0)
            dataList = dataList.Where(o => o.Status == roomStatus).ToList();

        return dataList.OrderBy(x => x.RoomNo).ToList();
    }
    #endregion

    #region GetOccupiedRoomList
    public async Task<List<HtRoomInfo>> GetOccupiedRoomList()
    {

        // All booking  
        var bookingList = await _iRepository.GetAsync(x => x.BookingStatus == BookingServiceStatusEnum.CheckIn
        && x.BookingStatus != BookingServiceStatusEnum.CheckOut && x.BookingType == BookingType.Room && !x.IsDeleted);

        var bookingIds = bookingList.Select(x => x.Id);

        var occupiedRoomIds = (await _iBookingRoomRepository.GetAsync(c => bookingIds.Contains(c.BookingId) && c.ActualCheckInTime != null && c.ActualCheckOutTime == null && !c.IsDeleted))
            .Select(x => x.RoomId).ToList();

        var totalOccupiedRoomList = await _iRoomInfoRepository.GetAsync(x => occupiedRoomIds.Contains(x.Id));

        return totalOccupiedRoomList.OrderByDescending(x => x.RoomNo).ToList();
    }

    public async Task<IEnumerable<SelectListItem>> GetOccupiedRoomSelectListItems(bool isDefaultSelectAdd = true)
    {
        var items = new List<SelectListItem>();
        if (isDefaultSelectAdd) items.Add(new SelectListItem { Value = "", Text = "---Select---" });
        var dataList = await GetOccupiedRoomList();
        items.AddRange(dataList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.RoomNo }));
        return items;
    }
    #endregion

    #region GetArrivalReportHtml

    public async Task<string> GetArrivalReportHtml(BookingArrivalVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        var data = await _iRepository.TodayArrivalDataAsync(vm);

        var queryDate = (DateTime)(!string.IsNullOrEmpty(vm.StrQueryDate) ? Utility.ConvertStrToDate(vm.StrQueryDate) : DateTime.Today);

        if (data != null && data.Count > 0)
        {
            // === Header Section ===
            if (isPrint)
            {
                fullHtml += $@"
                <div style='text-align:center;margin-bottom:20px;'>
                    
                    <p style='margin:4px 0;color:#555;font-size:14px;'>
                        <b>Date: </b> {queryDate.ToString("dd/MM/yyyy")} {vm.StrFromDate}
                    </p>
                    <hr style='border:1px solid #ddd;width:80%;margin:10px auto;' />
                </div>";
            }

            fullHtml += "<table class='table table-bordered report-table' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
            fullHtml += "<thead>";

            fullHtml += "<tr style='height:30px;'>";

            fullHtml += $@" <th style='width:5%;text-align:center;'>SL No</th>
                            <th style='width:15%;text-align:center;'>Booking No</th>
                            <th style='width:20%;text-align:center;'>Guest Info</th>
                            <th style='width:30%;text-align:center;'>Room No</th>
                            <th style='width:15%;text-align:center;'>C/Out Date</th>
                            <th style='width:15%;text-align:center;'>Remarks</th>";

            fullHtml += "</tr>";
            fullHtml += "</thead>";
            fullHtml += "<tbody>";

            for (int i = 0; i < data.Count; i++)
            {
                BookingArrivalVm objBooking = data[i];

                if (objBooking.AdvanceAmount > 0 && objBooking.BookingStatus == BookingServiceStatusEnum.Canceled)
                {
                    objBooking.Remarks = $@"Advance - {objBooking.AdvanceAmount}/- (Booking Canceled)";
                }
                else if (objBooking.AdvanceAmount > 0)
                {
                    objBooking.Remarks = $@"Advance - {objBooking.AdvanceAmount}/-";
                }

                string rowBookingNo = !isPrint ? $"<a target='_blank' href='../BookingService/Details/{objBooking.BookingId}'>{objBooking.BookingNo}</a>" : $"{objBooking.BookingNo}";
                var fontSize = objBooking.RoomList.Length > 150 ? "font-size: 10px;" : "";

                fullHtml += "<tr>";

                fullHtml += $@"<td >{i + 1}</td>
                                <td style='padding:5px;'><b>{rowBookingNo}</b><br />{DU.Utility.ConvertDateToStr(objBooking.BookingDate)}</td>
                                <td >{objBooking.GuestName} <br/>{objBooking.GuestMobile}</td>
                                <td style='word-break: break-word; {fontSize} text-align:left;'>{objBooking.RoomList}</td>
                                <td >{DU.Utility.ConvertDateToStr(objBooking.CheckOutTime)}</td>
                                <td ><b>{objBooking.Remarks}</b></td>";

                fullHtml += "</tr>";

            }

            fullHtml += $@"<tr>
                            <td colspan='2' style='text-align:center;'><b>Total Booked Room</b></td>
                            <td colspan='4' style='text-align:left;'><b>{data.Sum(x => x.RoomCount)}</b></td>
                        </tr>";
        }
        fullHtml += "</tbody>";

        fullHtml += "</table>";

        // === Footer Section ===
        fullHtml += @"<div class='report-footer'>
                        <hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
                        <p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>";

        fullHtml += @"</div>";

        return fullHtml;
    }

    #endregion

    #region GetTodayExpectedCheckOutReportHtml

    public async Task<string> GetTodayCheckOutReportHtml(BookingArrivalVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        var data = await _iRepository.TodayExpectedCheckOutDataAsync(vm);

        if (data == null || !data.Any() && isPrint)
            return "<p class='text-danger text-center'>No data found for the selected criteria.</p>";

        if (data != null && data.Count > 0)
        {
            // === Header Section ===
            if (isPrint)
            {
                fullHtml += $@"
                <div style='text-align:center;margin-bottom:20px;'>
                    
                    <p style='margin:4px 0;color:#555;font-size:14px;'>
                        <b>From:</b> {vm.StrFromDate}  |  <b>To:</b> {vm.StrToDate}
                    </p>
                    <hr style='border:1px solid #ddd;width:80%;margin:10px auto;' />
                </div>";
            }

            // === Table Section ===
            fullHtml += "<table class='table table-striped table-bordered table-md report-table' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
            fullHtml += "<thead>";

            fullHtml += "<tr style='height:30px;'>";

            fullHtml += $@"<th style='width:50px;'>SL.</th>
                            <th class='text-center' style='width:140px;'>Expected Departure</th>
                            <th class='text-center' style='width:140px;'>Suite No.</th>
                            <th style='width:250px;text-align:center;'>Guest</th>
                            <th class='text-center' style='width:140px;'>C/In Date</th>
                            <th class='text-center' style='width:150px;'>Net Rent</th>
                            <th style='width:100px;'>Booking No</th>
                            <th class='text-center' style='width:150px;'>Payment Status</th>
                            <th class='text-center' style='width:140px;'>Remarks</th>";

            fullHtml += "</tr>";
            fullHtml += "</thead>";
            fullHtml += "<tbody>";

            for (int i = 0; i < data.Count; i++)
            {
                BookingArrivalVm objBooking = data[i];

                string rowBookingNo = !isPrint ? $"<a target='_blank' href='../BookingService/Details/{objBooking.BookingId}'>{objBooking.BookingNo}</a>" : $"{objBooking.BookingNo}";

                fullHtml += "<tr>";

                fullHtml += $@"<td style='text-align:center;'>{i + 1}</td>
                                <td style='text-align:center;'>{objBooking.CheckOutTime:dd/MM/yyyy}</td>
                                <td style='text-align:center;'>{objBooking.RoomList}</td>
                                <td style='text-align:center;'>{objBooking.GuestName}<br />{objBooking.GuestMobile}</td>
                                <td style='text-align:center;'>{DU.Utility.ConvertDateToStr(objBooking.CheckInTime)}</td>
                                <td style='text-align:center;'>{objBooking.NetRent}</td>
                                <td style='text-align:center;padding:5px;'><b>{rowBookingNo}</b><br />{DU.Utility.ConvertDateToStr(objBooking.BookingDate)}</td>
                                <td style='text-align:center;'>{objBooking.PaymentStatus}</td>
                                <td style='text-align:left;'>{objBooking.Remarks} </td>";

                fullHtml += "</tr>";

            }
            fullHtml += "</tbody>";

            fullHtml += "</table>";
        }
        else
        {
            fullHtml += "<h5 style='text-align:center;color:red'>No Expected Check Out Data Found<h5>";
        }
        // === Footer Section ===
        fullHtml += @"
            <div class='report-footer'>
                <hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
                <p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + @"</p>
            </div>";

        return fullHtml;
    }

    #endregion

    #region GetCheckOutReportHtml

    public async Task<string> GetCheckOutReportHtml(DepartureReportVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        var data = await _iRepository.TodayDepartureDataAsync(vm);

        if ((data == null || !data.Any()) && isPrint)
            return "<p class='text-danger text-center'>No data found for the selected criteria.</p>";

        if (data != null && data.Count > 0)
        {
            // === Header Section ===
            if (isPrint)
            {
                fullHtml += $@"
                <div style='text-align:center;margin-bottom:20px;'>
                    
                    <p style='margin:4px 0;color:#555;font-size:14px;'>
                        <b>From:</b> {vm.StrFromDate}  |  <b>To:</b> {vm.StrToDate}
                    </p>
                    <hr style='border:1px solid #ddd;width:80%;margin:10px auto;' />
                </div>";
            }

            // === Table Section ===
            fullHtml += @"<table class='report-table' id='print_table'>
            <thead>
                <tr>
                    <th style='width:3%;'>SL.</th>
                    <th style='width:8%;'>Checked Out</th>
                    <th style='width:6%;'>Time</th>
                    <th style='width:10%;'>Checked In</th>
                    <th style='width:9%;'>Suite No</th>
                    <th style='width:10%;'>Guest</th>
                    <th style='width:10%;'>Nationality</th>
                    <th style='width:10%;'>Company</th>
                    <th style='width:8%;'>Reg./Bill No.</th>
                    <th style='width:5%;'>Adult</th>
                    <th style='width:5%;'>Child</th>
                    <th style='width:6%;'>Room Rent</th>
                    <th style='width:10%;'>Remarks</th>
                </tr>
            </thead>
            <tbody>";

            // === Data Rows ===
            foreach (var (obj, i) in data.GetItemWithIndex())
            {
                string bookingLink = string.IsNullOrEmpty(obj.BookingNo)
                    ? ""
                    : @$"<a href='../../BookingService/Details/{obj.BookingId}' target='_blank' style='color:#2980b9;text-decoration:none;font-weight:600;'>{obj.BookingNo}</a>";

                string billLink = string.IsNullOrEmpty(obj.BillNumber)
                    ? ""
                    : @$"<br/><a href='../../Bill/Details/{obj.BillId}' target='_blank' style='color:#27ae60;text-decoration:none;font-size:12px;'>{obj.BillNumber}</a>";

                fullHtml += $@"
                <tr>
                    <td style='text-align:center;'>{i + 1}</td>
                    <td style='text-align:center;'>{obj.CheckedOutDateTime:dd/MM/yyyy}</td>
                    <td style='text-align:center;'>{obj.CheckedOutTime}</td>
                    <td style='text-align:center;'>{obj.CheckInDateTime:dd/MM/yyyy HH:mm}</td>
                    <td style='text-align:center;font-weight:600;'>{obj.RoomNo}-{obj.RoomCategory}</td>
                    <td style='text-align:center;'>{obj.GuestName}<br/><small style='color:#555;'>{obj.GuestMobile}</small></td>
                    <td style='text-align:center;'>{obj.CountryName}</td>
                    <td style='text-align:center;'>{obj.CompanyName ?? "N/A"}</td>
                    <td style='text-align:center;'>{bookingLink}{billLink}</td>
                    <td style='text-align:center;'>{obj.Adult}</td>
                    <td style='text-align:center;'>{obj.Child}</td>
                    <td style='text-align:right;font-weight:600;color:#2c3e50;'>{obj.RoomRent:N2}</td>
                    <td style='text-align:left;'>{obj.Remarks}</td>
                </tr>";
            }

            fullHtml += "</tbody></table>";

            // === Footer Section ===
            fullHtml += @"
            <div class='report-footer'>
                <hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
                <p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>";

            fullHtml += @"
            </div>";
        }
        else
        {
            fullHtml += "<h5 style='text-align:center;color:red;'>No Check Out Data Found</h5>";
        }

        return fullHtml;
    }

    #endregion

    #region InHouseGuestReportHtml

    public async Task<string> InHouseGuestReportHtml(bool isPrint = false)
    {
        try
        {
            string fullHtml = "";
            var data = await _iRepository.InHouseGuestDataAsync();
            data = data.OrderBy(x => x.BookingRoomNo).ToList();

            var todayCheckOutRoomList = await _iBookingRoomRepository.GetAsync(x => x.ActualCheckOutTime != null && x.ActualCheckOutTime.Value.Date == DateTime.Today.Date, r => r.Room);

            var isHalfDayList = todayCheckOutRoomList.Where(x => x.BookingDayStatus == (int)BookingDayStatusEnum.HalfDay).ToList();

            var isDayUseList = todayCheckOutRoomList.Where(x => x.BookingDayStatus == (int)BookingDayStatusEnum.DayUse && x.ActualCheckInTime != null && x.ActualCheckOutTime != null && x.ActualCheckInTime.Value.Date == x.ActualCheckOutTime.Value.Date).ToList();

            var isFullDayList = todayCheckOutRoomList.Where(x => x.BookingDayStatus == (int)BookingDayStatusEnum.DayUse && x.ActualCheckInTime != null && x.ActualCheckOutTime != null && x.ActualCheckInTime.Value.Date != x.ActualCheckOutTime.Value.Date).ToList();

            var halfDayRoomJoin = string.Join(",", isHalfDayList.Select(s => s.Room.RoomNo).ToList());
            var dayUseRoomJoin = string.Join(",", isDayUseList.Select(s => s.Room.RoomNo).ToList());
            var fullDayRoomJoin = string.Join(",", isFullDayList.Select(s => s.Room.RoomNo).ToList());

            var todayCheckInRoomList = await _iBookingRoomRepository.GetAsync(x => x.CheckInTime.Date == DateTime.Today.Date, r => r.Room, b => b.Booking);

            var todayNoShowRoomList = todayCheckInRoomList.Where(x => x.Booking.BookingStatus == BookingServiceStatusEnum.NoShow).ToList();

            if (isPrint)
            {
                fullHtml += $@"<h6 style='text-align:center;padding-bottom:5px;'>Date: {DateTime.Today.ToString("dd/MM/yyyy")}</h6>";
            }

            fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
            fullHtml += "<thead>";

            fullHtml += "<tr style='height:30px;'>";

            fullHtml += $@"<th style='width:50px;'>SL No</th>
                            <th style='width:100px;text-align:center;'>Room No</th>
                            <th style='width:250px;text-align:center;'>Name Of Guest</th>
                            <th class='text-center' style='width:140px;'>Pax</th>
                            <th class='text-center' style='width:140px;'>C/In Date</th>
                            <th class='text-center' style='width:140px;'>C/Out Date</th>
                            <th class='text-center' style='width:150px;'>Mobile</th>
                            <th class='text-center' style='width:140px;'>Remarks</th>";

            fullHtml += "</tr>";
            fullHtml += "</thead>";
            fullHtml += "<tbody>";

            if (data != null && data.Count > 0)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    RoomWiseGuestVm objBooking = data[i];

                    string rowRoomNo = !isPrint ? $"<a target='_blank' href='../RoomInfo/Details/{objBooking.BookingRoomId}'>{objBooking.BookingRoomNo}</a>" : $"{objBooking.BookingRoomNo}";

                    string complementary = objBooking.ComplementaryId > 0 ? "CBF" : "WCBF";

                    fullHtml += "<tr>";

                    fullHtml += $@"<td style='text-align:left;'>{i + 1}</td>
                                <td style='text-align:center;padding:5px;'><b>{rowRoomNo}</b></td>
                                <td style='text-align:center;'>{objBooking.GuestName}</td>
                                <td style='text-align:center;'>{objBooking.TotalGuest}</td>
                                <td style='text-align:center;'>{DU.Utility.ConvertDateToStr(objBooking.CheckInTime)}</td>
                                <td style='text-align:center;'>{DU.Utility.ConvertDateToStr(objBooking.CheckOutTime)}</td>
                                <td style='text-align:center;'>{objBooking.GuestMobile}</td>
                                <td style='text-align:center;'>{complementary}</td>";

                    fullHtml += "</tr>";
                }

                fullHtml += $@"<tr>
                            <td colspan='3'></td>
                            <td style='text-align:center;'>{data.Sum(x => x.TotalGuest)}</td>
                            <td colspan='4'></td>
                        </tr>";

                var complementaryTotal = data.Where(x => x.ComplementaryId > 0).Sum(x => x.TotalGuest);

                fullHtml += $@"<tr>
                            <td colspan='5' style='text-align:left;'><b>CBF = {complementaryTotal}</b></td>
                            <td colspan='2' style='text-align:right;'><b>Total Occupied</b></td>
                            <td><b>{data.Count()}</b></td>
                        </tr>";

            }


            fullHtml += $@"<tr>
                            <td colspan='5' style='text-align:left;'><b>Half Day = {halfDayRoomJoin}</b></td>
                            <td colspan='2' style='text-align:right;'><b>Half Day</b></td>
                            <td><b>{isHalfDayList.Count()}</b></td>
                        </tr>";

            fullHtml += $@"<tr>
                            <td colspan='5' style='text-align:left;'><b>Day Use = {dayUseRoomJoin}</b></td>
                            <td colspan='2' style='text-align:right;'><b>Day Use</b></td>
                            <td><b>{isDayUseList.Count()}</b></td>
                        </tr>";

            fullHtml += $@"<tr>
                            <td colspan='5' style='text-align:left;'><b>Full Day = {fullDayRoomJoin}</b></td>
                            <td colspan='2' style='text-align:right;'><b>Full Day</b></td>
                            <td><b>{isFullDayList.Count()}</b></td>
                        </tr>";

            if (todayNoShowRoomList.Count > 0)
            {
                var noShowRoomJoin = string.Join(",", todayNoShowRoomList.Select(s => s.Room.RoomNo).ToList());

                fullHtml += $@"<tr>
                            <td colspan='5' style='text-align:left;'><b>No Show = {noShowRoomJoin}</b></td>
                            <td colspan='2' style='text-align:right;'><b>No Show</b></td>
                            <td><b>{todayNoShowRoomList.Count()}</b></td>
                        </tr>";
            }

            fullHtml += $@"<tr>
                            <td colspan='7' style='text-align:center;'><b>Total</b></td>
                            <td><b>{(data.Count() + isHalfDayList.Count() + isDayUseList.Count() + isFullDayList.Count() + todayNoShowRoomList.Count())}</b></td>
                        </tr>";

            fullHtml += "</tbody>";

            fullHtml += "</table>";

            return fullHtml;
        }
        catch (Exception ex)
        {
            throw;
        }

    }
    #endregion

    #region DailyInHouseGuestList

    public async Task<string> DailyInHouseGuestReportHtml(DailyInHouseGuestVm vm)
    {
        string fullHtml = await _iRepository.DailyInHouseGuestReportHtml(vm);
        return fullHtml;
    }
    #endregion

    #region GuestDueReportHtml
    public async Task<string> GuestDueReportHtml(GuestDueReportVm vm, bool isPrint)
    {
        string fullHtml = await _iRepository.GuestDueReportHtml(vm, isPrint);
        return fullHtml;
    }
    #endregion

    #region RoomDailySalesReportHtml

    public async Task<string> RoomDailySalesReportHtml(bool isPrint = false)
    {
        string fullHtml = "";
        var data = await _iRepository.RoomDailySalesDataAsync();
        data = data.OrderBy(x => x.BookingRoomNo).ToList();

        if (data != null && data.Count > 0)
        {
            // === Header Section ===
            if (isPrint)
            {
                fullHtml += $@"<div style='text-align:center;margin-bottom:20px;'>
        
                                    <p style='margin:4px 0;color:#555;font-size:14px;'>
                                        <b>Date: </b> {DateTime.Today.ToString("dd/MM/yyyy")}
                                    </p>
                                    <hr style='border:1px solid #ddd;width:80%;margin:10px auto;' />
                                </div>";
            }

            fullHtml += "<table class='table table-bordered report-table' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
            fullHtml += "<thead>";

            fullHtml += "<tr style='height:30px;'>";

            fullHtml += $@"<th style='width:50px;text-align:center;'>SL No</th>
                            <th style='width:250px;text-align:center;'>Name Of Guest</th>
                            <th class='text-center' style='width:150px;'>Mobile</th>
                            <th class='text-center' style='width:140px;'>C/In Date</th>
                            <th class='text-center' style='width:140px;'>C/Out Date</th>
                            <th style='width:100px;text-align:center;'>Room No</th>
                            <th class='text-center' style='width:150px;'>Room Rate</th>
                            <th class='text-center' style='width:140px;'>Discount</th>
                            <th class='text-center' style='width:140px;'>Net Sale</th>";

            fullHtml += "</tr>";
            fullHtml += "</thead>";
            fullHtml += "<tbody>";

            for (int i = 0; i < data.Count; i++)
            {
                RoomDailySalesReportVm objBooking = data[i];

                string rowRoomNo = !isPrint ? $"<a target='_blank' href='../RoomInfo/Details/{objBooking.BookingRoomId}'>{objBooking.BookingRoomNo}</a>" : $"{objBooking.BookingRoomNo}";

                fullHtml += "<tr>";

                fullHtml += $@"<td style='text-align:left;text-align:center;'>{i + 1}</td>
                                <td style='text-align:center;'>{objBooking.GuestName}</td>
                                <td style='text-align:center;'>{objBooking.GuestMobile}</td>
                                <td style='text-align:center;'>{DU.Utility.ConvertDateToStr(objBooking.CheckInTime)}</td>
                                <td style='text-align:center;'>{DU.Utility.ConvertDateToStr(objBooking.CheckOutTime)}</td>
                                <td style='text-align:center;padding:5px;'><b>{objBooking.BookingRoomNo}</b></td>
                                <td style='text-align:right;'>{objBooking.RoomRate}</td>
                                <td style='text-align:right;'>{objBooking.Discount}</td>
                                <td style='text-align:right;'>{objBooking.NetSale}</td>";

                fullHtml += "</tr>";
            }

            fullHtml += $@"<tr>
                            <td colspan='6' style='text-align:right;'><b>Total</b></td>
                            <td style='text-align:right;'><b>{data.Sum(x => x.RoomRate)}</b></td>
                            <td style='text-align:right;'><b>{data.Sum(x => x.Discount)}</b></td>
                            <td style='text-align:right;'><b>{data.Sum(x => x.NetSale)}</b></td>
                        </tr>";
        }
        fullHtml += "</tbody>";

        fullHtml += "</table>";

        // === Footer Section ===
        fullHtml += @"<div class='report-footer'>
    		<hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
    		<p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>";
        fullHtml += @"</div>";

        return fullHtml;
    }
    #endregion

    #region GetBookingDailySalesReportHtml

    public async Task<string> GetBookingDailySalesReportHtml(BookingDailySalesReportVm vm, bool isPrint)
    {
        string fullHtml = await _iHotelManagementRepository.GetBookingDailySalesReportHtml(vm);
        return fullHtml;
    }

    #endregion

    #region RoomOccupancyReportHtml
    public async Task<string> RoomOccupancyReportHtml(RoomOccupancyReportVm vm)
    {
        string fullHtml = await _iHotelManagementRepository.RoomOccupancyReportHtml(vm);
        return fullHtml;
    }
    #endregion

    #region NotificationMsg

    private string BookingNotificationMsg(HtBookingService booking, List<HtRoomInfo> rooms, string guestName = "", double advanceAmount = 0)
    {
        var fullName = "";
        var user = _iHttpContextAccessor.HttpContext?.User;
        if (user != null && user.Identity.IsAuthenticated)
        {
            fullName = user.FindFirst("FullName")?.Value;
        }

        var actionUrl = _iUrlHelperService.BookingDetailUrl(booking.Id);
        string bookingNoHtml = @$"<a href='{actionUrl}' target='_blank'><b>{booking.BookingNo}</b></a>";

        string msgHtml = $@"<div class='ntf'>
                                <p>A new booking has just been saved by {fullName}.</p>
                                <p><b>Room No : </b>{string.Join(",", rooms.Select(x => x.RoomNo))}</p>
                                <p><b>Guest Name :</b> {guestName}.</p>
                                <p><b>Duration :</b> From {booking.CheckInTime.ToString("dd/MMM/yyyy")} to {booking.CheckOutTime.ToString("dd/MMM/yyyy")}.</p>
                                <p><b>Amount :</b> {booking.NetRent}, <b>Advance Paid :</b> {advanceAmount}.</p>
                                <p><b>Booking No :</b> {bookingNoHtml}.</p>
                            </div>";

        return msgHtml;
    }

    private string CheckInNotificationMsg(HtBookingService booking, List<HtRoomInfo> rooms, string guestName = "", double payAmount = 0)
    {
        var fullName = "";
        var user = _iHttpContextAccessor.HttpContext?.User;
        if (user != null && user.Identity.IsAuthenticated)
        {
            fullName = user.FindFirst("FullName")?.Value;
        }

        var actionUrl = _iUrlHelperService.BookingDetailUrl(booking.Id);
        string bookingNoHtml = @$"<a href='{actionUrl}' target='_blank'><b>{booking.BookingNo}</b></a>";

        string msgHtml = $@"<div class='ntf'>
                                <p>Room Check-In has been saved by {fullName}.</p>
                                <p><b>Room No : </b>{string.Join(",", rooms.Select(x => x.RoomNo))}</p>
                                <p><b>Guest Name :</b> {guestName}.</p>
                                <p><b>Duration :</b> From {booking.CheckInTime.ToString("dd/MMM/yyyy")} to {booking.CheckOutTime.ToString("dd/MMM/yyyy")}.</p>
                                <p><b>Amount :</b> {booking.NetRent}, <b>Check-In Pay Amount :</b> {payAmount}.</p>
                                <p><b>Booking No :</b> {bookingNoHtml}.</p>
                            </div>";

        return msgHtml;
    }

    private string CheckOutNotificationMsg(HtBookingService booking, List<HtRoomInfo> rooms, string guestName = "", double payAmount = 0)
    {
        var fullName = "";
        var user = _iHttpContextAccessor.HttpContext?.User;
        if (user != null && user.Identity.IsAuthenticated)
        {
            fullName = user.FindFirst("FullName")?.Value;
        }

        var actionUrl = _iUrlHelperService.BookingDetailUrl(booking.Id);
        string bookingNoHtml = @$"<a href='{actionUrl}' target='_blank'><b>{booking.BookingNo}</b></a>";

        string msgHtml = $@"<div class='ntf'>
                                <p>Room Check-Out has been saved by {fullName} and bill enerated.</p>
                                <p><b>Room No : </b>{string.Join(",", rooms.Select(x => x.RoomNo))}</p>
                                <p><b>Guest Name :</b> {guestName}.</p>
                                <p><b>Duration :</b> From {booking.CheckInTime.ToString("dd/MMM/yyyy")} to {booking.CheckOutTime.ToString("dd/MMM/yyyy")}.</p>
                                <p><b>Bill Amount :</b> {booking.NetRent}, <b>Already Paid :</b> {payAmount}.</p>
                                <p><b>Booking No :</b> {bookingNoHtml}.</p>
                            </div>";

        return msgHtml;
    }

    #endregion

    #region PaymentRemove

    public async Task<bool> PaymentRemoveAsync(long paymentId)
    {
        var paymentData = await _iBookingPaymentRepository.GetFirstOrDefaultAsync(x => x.Id == paymentId && !x.IsDeleted, o => o.Booking, b => b.Billing);

        if (paymentData == null)
            throw new Exception("No Payment Information Found");
        var paymentVoucher = await _iAccTranMstRepository.GetFirstOrDefaultAsync(x => x.PaymentId == paymentData.Id && !x.IsDeleted, o => o.AccTranDtls);

        var orderPaidList = await _iBookingPaymentRepository.GetAsync(x => x.BookingId == paymentData.BookingId && !x.IsDeleted);
        var remainPayment = orderPaidList.Where(x => x.Id != paymentData.Id).Sum(x => x.PaidAmount);

        HtBilling bill = null;

        if (paymentData.BillingId > 0 && paymentData.Billing != null)
        {
            bill = paymentData.Billing;

            if (bill.NetAmount == remainPayment)
            {
                bill.BillStatus = BillStatusEnum.FullPaid;
            }
            if (bill.NetAmount > remainPayment)
            {
                bill.BillStatus = BillStatusEnum.PartialPaid;
            }
            if (remainPayment == 0)
            {
                bill.BillStatus = BillStatusEnum.Fresh;
            }
            bill.PaidAmount = remainPayment;
        }

        var booking = paymentData.Booking;


        if (booking.NetRent == remainPayment)
        {
            booking.PaymentStatus = PaymentStatusEnum.FullPayment;
        }
        if (booking.NetRent > remainPayment && remainPayment > 0)
        {
            booking.PaymentStatus = PaymentStatusEnum.PartialPayment;
        }
        if (remainPayment == 0)
        {
            booking.PaymentStatus = PaymentStatusEnum.Pending;
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (paymentVoucher != null)
        {
            _iAccTranDtlRepository.RemoveRange(paymentVoucher.AccTranDtls);
            _iAccTranMstRepository.Remove(paymentVoucher);
        }

        _iBookingPaymentRepository.Remove(paymentData);

        if (bill != null)
        {
            await _iBillRepository.UpdateAsync(bill);
        }

        await _iRepository.UpdateAsync(booking);

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }


    #endregion

    #region BookingRemove

    public async Task<bool> BookingRemoveAsync(long bookingId)
    {
        var booking = await _iRepository.GetByIdAsync(bookingId);
        if (booking == null)
            throw new Exception("No Booking Information Found");

        var bookingGuestList = await _iBookingGuestRepository.GetAsync(x => x.BookingId == booking.Id);
        if (bookingGuestList == null)
            throw new Exception("No Guest Information Found");

        var bookingRoomList = await _iBookingRoomRepository.GetAsync(x => x.BookingId == booking.Id);
        if (bookingRoomList == null)
            throw new Exception("No Room Information Found");

        var bookingPaidList = await _iBookingPaymentRepository.GetAsync(x => x.BookingId == booking.Id);

        var bookingBill = await _iBillRepository.GetAsync(x => x.BookingId == booking.Id, d => d.BillingDetails);
        var billingDetails = bookingBill.FirstOrDefault()?.BillingDetails;

        var advancerefund = await _iAdvanceRefundRepository.GetAsync(r => r.BookingId == booking.Id);

        var paymentIds = bookingPaidList.Select(x => x.Id).ToList();
        var vouchers = await _iAccTranMstRepository.GetAsync(x => paymentIds.Contains((long)x.PaymentId), o => o.AccTranDtls);


        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (vouchers != null && vouchers.Count > 0)
        {
            var allAccTranDtls = vouchers.SelectMany(v => v.AccTranDtls).ToList();

            if (allAccTranDtls.Count > 0)
            {
                _iAccTranDtlRepository.RemoveRange(allAccTranDtls);
            }
            _iAccTranMstRepository.RemoveRange(vouchers);
        }

        if (bookingPaidList != null && bookingPaidList.Count > 0)
        {
            _iBookingPaymentRepository.RemoveRange(bookingPaidList);
        }

        if (bookingBill != null && bookingBill.Count > 0)
        {
            if (billingDetails.Count > 0)
            {
                _iBillingDetailRepository.RemoveRange(billingDetails);
            }
            _iBillRepository.RemoveRange(bookingBill);
        }

        if (advancerefund != null && advancerefund.Count > 0)
        {
            _iAdvanceRefundRepository.RemoveRange(advancerefund);
        }

        if (bookingRoomList != null && bookingRoomList.Count > 0)
        {
            _iBookingRoomRepository.RemoveRange(bookingRoomList);
        }

        if (bookingGuestList != null && bookingGuestList.Count > 0)
        {
            _iBookingGuestRepository.RemoveRange(bookingGuestList);
        }

        _iRepository.Remove(booking);

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }


    #endregion

    /// <summary>
    ///  Hall booking all method start from here
    /// </summary>

    #region HallBookingEntry

    public async Task<bool> HallBookingEntry(HtBookingServiceVm vm)
    {
        var bookingModel = _iMapper.Map<HtBookingService>(vm);

        bookingModel.BookingNo = await GetBookingCode();
        bookingModel.BookingDate = (DateTime)(!string.IsNullOrEmpty(vm.BookingDateStr) ? Utility.ConvertStrToDate(vm.BookingDateStr) : DU.Utility.GetBdDateTimeNow());
        bookingModel.BookingType = BookingType.Hall;
        bookingModel.BookingStatus = BookingServiceStatusEnum.Booked;
        bookingModel.ActionById = CurrentUserId;
        bookingModel.ActionDate = Utility.GetBdDateTimeNow();

        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        var reportDate = Utility.GenerateReportDate(bookingModel.BookingDate.Add(currentTime));
        bookingModel.ReportDate = reportDate;

        if (vm.BookingHallVms?.Count > 0 is false)
            throw new Exception("No Hall Information Found...!!");

        foreach (var hallVm in vm.BookingHallVms)
        {
            if (string.IsNullOrEmpty(hallVm.BookingDateStr))
                throw new Exception("Hall Booking Date Not Found..!!");

            hallVm.BookingDate = (DateTime)(!string.IsNullOrEmpty(hallVm.BookingDateStr) ? Utility.ConvertStrToDate(hallVm.BookingDateStr) : bookingModel.CheckInTime);
        }

        var bookingHalls = _iMapper.Map<List<HtBookingHall>>(vm.BookingHallVms);

        if (bookingHalls.Count > 0)
        {
            foreach (var hall in bookingHalls)
            {
                var filterData = vm.BookingHallVms.FirstOrDefault(x => x.HallId == hall.HallId && x.HallShift == hall.HallShift && x.BookingDate.Date == hall.BookingDate.Date);
                if (filterData == null)
                    throw new Exception("Hall Not Found...!");

                if (string.IsNullOrEmpty(filterData.BookingDateStr))
                    throw new Exception("Hall Booking Date Is Not Correct..!!");

                if (!(filterData.HallShift > 0))
                    throw new Exception("Hall Booking Shift Is Not Correct..!!");

                hall.BookingDate = (DateTime)(!string.IsNullOrEmpty(filterData.BookingDateStr) ? Utility.ConvertStrToDate(filterData.BookingDateStr) : bookingModel.CheckInTime);
                hall.ActionDate = Utility.GetBdDateTimeNow();
                hall.ActionById = CurrentUserId;

                if (hall.HallId > 0)
                {
                    var hallInfo = _iHallInfoRepository.GetFirstOrDefault(x => x.Id == hall.HallId && x.IsActive && !x.IsDeleted);
                    if (hallInfo == null)
                        throw new Exception("Hall Not Found...!!");

                    //var checkAvalibility = await _iRoomInfoService.CheckRoomIsAvaliable(roomInfo.Id, room.CheckInTime, room.CheckOutTime);
                    //if (!checkAvalibility)
                    //    throw new Exception("Room Is Not Available...!!");

                    var shiftCount = hall.HallShift == HallBookingShiftEnum.Both ? 2 : 1;

                    //hall.Rent = (hallInfo.Rent * shiftCount);
                    hall.HallRent = (filterData.Rent / shiftCount);
                    hall.Rent = filterData.Rent;
                    hall.ServiceCharge = (hallInfo.ServiceCharge * shiftCount);
                    hall.NetRent = (hall.Rent + hall.ServiceCharge);
                }
            }

            bookingModel.Rent = bookingHalls.Sum(x => x.Rent);
            bookingModel.ServiceCharge = bookingHalls.Sum(x => x.ServiceCharge);
            bookingModel.NetRent = bookingHalls.Sum(x => x.NetRent);

            var minCheckInDate = bookingHalls.Select(x => x.BookingDate).Min();
            var maxCheckOutDate = bookingHalls.Select(x => x.BookingDate).Max();

            if (minCheckInDate.Date != bookingModel.CheckInTime.Date)
                bookingModel.CheckInTime = minCheckInDate;

            if (maxCheckOutDate.Date != bookingModel.CheckOutTime.Date)
                bookingModel.CheckOutTime = maxCheckOutDate;
        }

        if (bookingModel.BookingDate.Date > bookingModel.CheckInTime.Date)
            throw new Exception("Hall Booking Date Is Previous Date Than Booking Entry Date..!!");

        if (bookingModel.CheckInTime.Date > bookingModel.CheckOutTime.Date)
            throw new Exception("Date Is Not Correct..!!");

        if (vm.BookingGuestVms?.Count > 0 is false)
            throw new Exception("No Guest Information Found...!!");

        var mainGuest = vm.BookingGuestVms.Any(x => x.IsMain == true);
        if (mainGuest == false)
            throw new Exception("No Bill Guest Information Found...!!");

        var bookingGuests = _iMapper.Map<List<HtBookingGuest>>(vm.BookingGuestVms);

        if (bookingGuests.Count > 0)
        {
            foreach (var guest in bookingGuests)
            {
                guest.ActionDate = Utility.GetBdDateTimeNow();
                guest.ActionById = CurrentUserId;
            }
        }

        HtBookingPayment paymentModel = null;
        AccTranMst paymentVoucher = null;

        if (vm.PaymentVm != null && vm.PaymentVm.PaidAmount > 0)
        {
            paymentModel = _iMapper.Map<HtBookingPayment>(vm.PaymentVm);
            paymentModel.PaidDate = (DateTime)(!string.IsNullOrEmpty(vm.PaidDateStr) ? Utility.ConvertStrToDate(vm.PaidDateStr) : DU.Utility.GetBdDateTimeNow());
            paymentModel.Description = $"Advance Amount {vm.PaymentVm.PaidAmount} is paid when hall is booked..";
            paymentModel.ActionById = CurrentUserId;
            paymentModel.ActionDate = Utility.GetBdDateTimeNow();
            paymentModel.IsAdvance = true;

            var paymentReportDate = Utility.GenerateReportDate(paymentModel.PaidDate.Add(currentTime));
            paymentModel.ReportDate = paymentReportDate;

            if (bookingModel.NetRent < paymentModel.PaidAmount)
                throw new Exception("Paid Amount Is Higher Than Net Amount...!!");

            if (bookingModel.NetRent == paymentModel.PaidAmount)
                bookingModel.PaymentStatus = PaymentStatusEnum.FullPayment;
            else if (bookingModel.NetRent > paymentModel.PaidAmount)
                bookingModel.PaymentStatus = PaymentStatusEnum.PartialPayment;



            //paymentVoucher = await GetAdvancePaymentVoucher(paymentModel, bookingModel.BookingNo);
            paymentVoucher = await GetAdvancePaymentQuickVoucher(paymentModel, bookingModel.BookingNo, isHallBooking: true);
            if (paymentVoucher == null)
                throw new Exception("Somthing Went Wrong Creating Voucher..!!");
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iRepository.AddAsync(bookingModel);
        await _iUnitOfWork.CompleteAsync();

        bookingHalls.ForEach(x => x.BookingId = bookingModel.Id);
        bookingGuests.ForEach(x => x.BookingId = bookingModel.Id);

        await _iBookingHallRepository.AddRangeAsync(bookingHalls);
        await _iBookingGuestRepository.AddRangeAsync(bookingGuests);

        if (paymentModel != null && paymentVoucher != null)
        {
            paymentModel.BookingId = bookingModel.Id;
            await _iBookingPaymentRepository.AddAsync(paymentModel);
            await _iUnitOfWork.CompleteAsync();

            paymentVoucher.PaymentId = paymentModel.Id;
            await _iAccTranMstRepository.AddAsync(paymentVoucher);
        }

        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return false; }
        ts.Complete();
        return true;
    }

    #endregion

    #region HallBookingEdit

    public async Task<bool> HallBookingUpdate(HtBookingServiceVm vm)
    {
        if (vm == null && !(vm.Id > 0))
            throw new Exception("Hall Booking Service Information Is Not Correct...!!");

        var bookingService = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == vm.Id && x.BookingType == BookingType.Hall && !x.IsDeleted);

        if (bookingService == null)
            throw new Exception("Hall Booking Service Not Found...!!");

        if (bookingService.BookingStatus != BookingServiceStatusEnum.Booked)
            throw new Exception("Booking Service Can't Be Edit...!!");

        bookingService.VisitPurpose = vm.VisitPurpose;

        #region Room

        List<HtBookingHall> addableHallList = null;
        List<HtBookingHall> updateableHallList = null;
        List<HtBookingHall> deletableHallList = null;

        bookingService.Rent = 0;
        bookingService.ServiceCharge = 0;
        bookingService.NetRent = 0;

        var hallBookDates = new List<DateTime>();

        var existBookingHalls = await _iBookingHallRepository.GetAsync(x => x.BookingId == vm.Id && !x.IsDeleted);

        if (vm?.BookingHallVms?.Count > 0)
        {
            var dataListForAdd = vm?.BookingHallVms?.Where(c => c.Id == 0).ToList();

            var updatableItemIds = vm?.BookingHallVms.Where(c => c.Id > 0).Select(c => c.Id).ToList();
            updateableHallList = (existBookingHalls.Where(x => updatableItemIds.Contains(x.Id))).ToList();

            if (updateableHallList?.Count > 0)
            {
                foreach (var updateHall in updateableHallList)
                {
                    var filterData = vm.BookingHallVms.Where(c => c.Id == updateHall.Id).FirstOrDefault();

                    if (string.IsNullOrEmpty(filterData.BookingDateStr))
                        throw new Exception("Hall Booking Date Is Mendatory..!!");

                    updateHall.BookingDate = (DateTime)(!string.IsNullOrEmpty(filterData.BookingDateStr) ? Utility.ConvertStrToDate(filterData.BookingDateStr) : updateHall.BookingDate);

                    hallBookDates.Add(updateHall.BookingDate);
                }

                bookingService.Rent = updateableHallList.Sum(x => x.Rent);
                bookingService.ServiceCharge = updateableHallList.Sum(x => x.ServiceCharge);
                bookingService.NetRent = bookingService.Rent + bookingService.ServiceCharge;
            }

            var oldIds = updateableHallList?.Select(c => c.Id).ToList();
            deletableHallList = (existBookingHalls.Where(x => !oldIds.Contains(x.Id))).ToList();

            if (dataListForAdd != null && dataListForAdd.Count > 0)
            {
                foreach (var hallVm in dataListForAdd)
                {
                    if (string.IsNullOrEmpty(hallVm.BookingDateStr))
                        throw new Exception("Hall Booking Date Not Found..!!");

                    hallVm.BookingDate = (DateTime)(!string.IsNullOrEmpty(hallVm.BookingDateStr) ? Utility.ConvertStrToDate(hallVm.BookingDateStr) : hallVm.BookingDate);
                }
            }

            if (dataListForAdd?.Count > 0)
            {
                addableHallList = _iMapper.Map<List<HtBookingHall>>(dataListForAdd);

                foreach (var (hall, i) in addableHallList.GetItemWithIndex())
                {
                    var filterData = dataListForAdd.FirstOrDefault(c => c.HallId == hall.HallId && c.HallShift == hall.HallShift && c.BookingDate.Date == hall.BookingDate.Date);

                    if (filterData == null)
                        throw new Exception("Hall Not Found...!");

                    hall.BookingId = bookingService.Id;

                    if (string.IsNullOrEmpty(filterData.BookingDateStr))
                        throw new Exception("Hall Booking Date Not Found...!");

                    hall.BookingDate = (DateTime)(!string.IsNullOrEmpty(filterData.BookingDateStr) ? Utility.ConvertStrToDate(filterData.BookingDateStr) : hall.BookingDate);
                    hall.ActionDate = Utility.GetBdDateTimeNow();
                    hall.ActionById = CurrentUserId;

                    if (hall.HallId > 0)
                    {
                        var hallInfo = _iHallInfoRepository.GetFirstOrDefault(x => x.Id == hall.HallId && x.IsActive && !x.IsDeleted);
                        if (hallInfo == null)
                            throw new Exception("Hall Not Found...!!");

                        var shiftCount = hall.HallShift == HallBookingShiftEnum.Both ? 2 : 1;

                        //hall.Rent = (hallInfo.Rent * shiftCount);
                        hall.HallRent = (filterData.Rent / shiftCount);
                        hall.Rent = filterData.Rent;
                        hall.ServiceCharge = (hallInfo.ServiceCharge * shiftCount);
                        hall.NetRent = (hall.Rent + hall.ServiceCharge);
                    }

                    hallBookDates.Add(hall.BookingDate);
                }

                bookingService.Rent += addableHallList.Sum(x => x.Rent);
                bookingService.ServiceCharge += addableHallList.Sum(x => x.ServiceCharge);
                bookingService.NetRent = bookingService.Rent + bookingService.ServiceCharge;
            }
        }

        var minBookingDate = hallBookDates.Min();
        var maxBookingDate = hallBookDates.Max();

        bookingService.CheckInTime = minBookingDate;
        bookingService.CheckOutTime = maxBookingDate;

        #endregion

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        _iRepository.Update(bookingService);

        if (addableHallList?.Count > 0)
        {
            _iBookingHallRepository.AddRange(addableHallList);
        }

        if (updateableHallList?.Count > 0)
        {
            _iBookingHallRepository.UpdateRange(updateableHallList);
        }

        if (deletableHallList?.Count > 0)
        {
            _iBookingHallRepository.RemoveRange(deletableHallList);
        }

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }

    #endregion

    #region HallBookingComplete

    public async Task<(bool, long)> HallBookingComplete(HtBookingServiceVm vm)
    {
        if (vm == null && !(vm.Id > 0))
            throw new Exception("Hall Booking Service Information Is Not Correct...!!");

        var bookingService = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == vm.Id && x.BookingType == BookingType.Hall && !x.IsDeleted);

        if (bookingService == null)
            throw new Exception("Hall Booking Service Not Found...!!");

        if (bookingService.BookingStatus == BookingServiceStatusEnum.CheckOut)
            throw new Exception("Hall Booking Service Already Completed...!!");

        bookingService.BookingStatus = BookingServiceStatusEnum.CheckOut;

        bookingService.Vat = vm.Vat;
        bookingService.Tax = vm.Tax;
        bookingService.Discount = vm.Discount;

        var subRent = bookingService.Rent + bookingService.ServiceCharge;

        bookingService.NetRent = (subRent + bookingService.Vat + bookingService.Tax) - (bookingService.Discount);

        var paidList = await _iBookingPaymentRepository.GetAsync(x => x.BookingId == bookingService.Id && !x.IsDeleted && x.PaidDate.Date <= bookingService.CheckOutTime.Date);
        var alreadyPaidAmount = paidList.Sum(x => x.PaidAmount);

        #region Refund Amount

        HtAdvanceRefund refundModel = null;

        if (vm.IsRefund && alreadyPaidAmount > 0)
        {
            var refundAmount = alreadyPaidAmount;

            refundModel = new HtAdvanceRefund();
            refundModel.BookingId = bookingService.Id;
            refundModel.RefundAmount = refundAmount;
            refundModel.RefundMode = PayModeEnum.Cash;
            refundModel.RefundDate = Utility.GetBdDateTimeNow();
            refundModel.ActionById = CurrentUserId;
            refundModel.ActionDate = Utility.GetBdDateTimeNow();
            refundModel.TransactionNo = paidList != null && paidList.Count > 0 ? string.Join(", ", paidList.Select(x => x.TransactionNo)) : "";
            refundModel.Description = $"Advance Amount {refundModel.RefundAmount} is Adjusted.";
        }

        #endregion

        #region Bill

        var existBookingHalls = await _iBookingHallRepository.GetAsync(x => x.BookingId == vm.Id && !x.IsDeleted);

        var generatedBill = await GenerateHallBill(bookingService, existBookingHalls.ToList());
        if (generatedBill == null)
            throw new Exception("Can't Generated Bill..!!");

        //generatedBill.TotalAmount = updateableRoomList.Sum(x => x.NetRent);
        generatedBill.TotalAmount = generatedBill.BillingDetails.Sum(x => x.NetAmount);
        generatedBill.Vat = vm.Vat;
        generatedBill.Tax = vm.Tax;
        generatedBill.Discount = vm.Discount;

        generatedBill.PaidAmount = alreadyPaidAmount;

        generatedBill.NetAmount = (generatedBill.TotalAmount + generatedBill.Vat + generatedBill.Tax) - (generatedBill.Discount);

        if (generatedBill.PaidAmount == generatedBill.NetAmount)
            generatedBill.BillStatus = BillStatusEnum.FullPaid;
        else if (generatedBill.PaidAmount > generatedBill.NetAmount)
            generatedBill.BillStatus = BillStatusEnum.PartialPaid;

        #endregion

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        _iRepository.Update(bookingService);

        if (refundModel != null)
        {
            await _iAdvanceRefundRepository.AddAsync(refundModel);
        }

        if (generatedBill != null)
        {
            await _iBillRepository.AddAsync(generatedBill);
        }

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return (false, 0); }
        ts.Complete();
        return (true, generatedBill.Id);
    }

    #endregion

    #region GetHallAvailabilityByDate
    public async Task<List<HtHallInfo>> GetAvailableHallByDate(DateTime selectedDate, int? shift = null)
    {
        var hallBookingList = await _iRepository.GetAsync(x => x.CheckInTime.Date <= selectedDate.Date && x.CheckOutTime.Date >= selectedDate.Date && x.BookingType == BookingType.Hall && !x.IsDeleted);
        var unavailableList = hallBookingList.Where(x => x.BookingStatus == BookingServiceStatusEnum.CheckIn || x.BookingStatus == BookingServiceStatusEnum.Booked).ToList();
        var unavailableListIds = unavailableList.Select(x => x.Id);

        var unavailableHallList = await _iBookingHallRepository.GetAsync(c => unavailableListIds.Contains(c.BookingId) && c.BookingDate.Date == selectedDate.Date && !c.IsDeleted);

        if (shift != null && shift > 0 && shift != (int)HallBookingShiftEnum.Both)
        {
            unavailableHallList = unavailableHallList.Where(x => x.HallShift == (HallBookingShiftEnum)shift || x.HallShift == HallBookingShiftEnum.Both).ToList();
        }
        else if (shift != null && shift > 0 && shift == (int)HallBookingShiftEnum.Both)
        {
            unavailableHallList = unavailableHallList.Where(x => (x.HallShift == HallBookingShiftEnum.DayShift || x.HallShift == HallBookingShiftEnum.NightShift || x.HallShift == HallBookingShiftEnum.Both)).ToList();
        }

        var unavailableHallIds = unavailableHallList.Select(x => x.HallId).ToList();

        var totalAvailableHallList = await _iHallInfoRepository.GetAsync(x => !unavailableHallIds.Contains(x.Id));

        return totalAvailableHallList.OrderByDescending(x => x.HallName).ToList();
    }

    public async Task<dynamic> GetDynamicAvailableHallByDateAndShift(DateTime selectedDate, int? shift = null)
    {
        var dataList = await GetAvailableHallByDate(selectedDate, shift);

        var availableHallList = dataList.Select(c => new
        {
            c.Id,
            Name = c.HallName,
            c.Rent,
            c.ServiceCharge,
            c.TotalRent,
        }).ToList().OrderBy(o => o.Name);

        return availableHallList;
    }
    #endregion

    #region GetAvailableHallByDateRanege

    public async Task<List<HtHallInfo>> GetAvailableHallByDateRange(DateTime fromDate, DateTime toDate, int? shift = null)
    {
        var hallBookingList = await _iRepository.GetAsync(x => (fromDate.Date >= x.CheckInTime.Date && fromDate.Date < x.CheckOutTime.Date)
        || (toDate.Date > x.CheckInTime.Date && toDate.Date <= x.CheckOutTime.Date)
        || (fromDate.Date <= x.CheckInTime.Date && x.CheckOutTime.Date <= toDate.Date) && x.BookingType == BookingType.Hall && !x.IsDeleted);

        var unavailableList = hallBookingList.Where(x => x.BookingStatus == BookingServiceStatusEnum.CheckIn || x.BookingStatus == BookingServiceStatusEnum.Booked).ToList();
        var unavailableListIds = unavailableList.Select(x => x.Id);

        var unavailableHallList = await _iBookingHallRepository.GetAsync(c => unavailableListIds.Contains(c.BookingId) && !c.IsDeleted);

        var selectedUnAvailableHallList = new List<HtBookingHall>();

        var dateList = DU.AppUtility.DateRangeList((DateTime)fromDate, (DateTime)toDate);

        if (dateList.Count() > 0)
        {
            foreach (var date in dateList)
            {
                if (shift != null && shift > 0 && shift != (int)HallBookingShiftEnum.Both)
                {
                    var dateUnavailableHallList = unavailableHallList.Where(x => x.BookingDate.Date == date.Date && (x.HallShift == (HallBookingShiftEnum)shift || x.HallShift == HallBookingShiftEnum.Both)).ToList();

                    if (dateUnavailableHallList.Count > 0)
                    {
                        foreach (var item in dateUnavailableHallList)
                        {
                            selectedUnAvailableHallList.Add(item);
                        }
                    }
                }
                else if (shift != null && shift > 0 && shift == (int)HallBookingShiftEnum.Both)
                {
                    var dateUnavailableHallList = unavailableHallList.Where(x => x.BookingDate.Date == date.Date && (x.HallShift == HallBookingShiftEnum.DayShift || x.HallShift == HallBookingShiftEnum.NightShift || x.HallShift == HallBookingShiftEnum.Both)).ToList();

                    if (dateUnavailableHallList.Count > 0)
                    {
                        foreach (var item in dateUnavailableHallList)
                        {
                            selectedUnAvailableHallList.Add(item);
                        }
                    }
                }
            }
        }

        var unavailableHallIds = selectedUnAvailableHallList.Select(x => x.HallId).ToList();

        var totalAvailableHallList = await _iHallInfoRepository.GetAsync(x => !unavailableHallIds.Contains(x.Id));

        return totalAvailableHallList.OrderByDescending(x => x.HallName).ToList();
    }

    public async Task<dynamic> GetDynamicAvailableHallByDateRange(DateTime fromDate, DateTime toDate, int? shift = null)
    {
        var dataList = await GetAvailableHallByDateRange(fromDate, toDate, shift);

        var availableHallList = dataList.Select(c => new
        {
            c.Id,
            Name = c.HallName,
            c.Rent,
            c.ServiceCharge,
            c.TotalRent,
        }).ToList().OrderBy(o => o.Name);

        return availableHallList;
    }

    #endregion

    #region GetHallBookingInfo

    public async Task<HtBookingServiceVm> GetHallBookingInfoById(long bookingId)
    {
        var data = await _iRepository.GetHallBookingByIdAsync(bookingId);
        return data;
    }

    #endregion

    #region BookedHallList

    public async Task<List<HtBookingHallVm>> GetBookedHallListByBookingId(long bookingId)
    {
        if (bookingId < 0)
            throw new Exception("booking info not found...!");

        var bookingRooms = await _iBookingHallRepository.GetBookedHall(bookingId);

        return bookingRooms;
    }

    #endregion

    #region HallAvailableReport

    public async Task<List<HallAvaliableReportVm>> GetHallAvailabilityByDateRange(long hallId, string formDateStr, string toDateStr)
    {
        if (hallId < 0)
            throw new Exception("hall info not found...!");

        var hallInfo = await _iHallInfoRepository.GetFirstOrDefaultAsync(x => x.Id == hallId && !x.IsDeleted);

        if (hallInfo == null)
            throw new Exception("hall info not found...!");

        var fromDate = !string.IsNullOrEmpty(formDateStr) ? Utility.ConvertStrToDate(formDateStr) : null;
        var toDate = !string.IsNullOrEmpty(toDateStr) ? Utility.ConvertStrToDate(toDateStr) : null;

        if (fromDate == null && toDate == null)
        {
            return new List<HallAvaliableReportVm>();
        }

        var dateList = DU.AppUtility.DateRangeList((DateTime)fromDate, (DateTime)toDate);

        var dataList = new List<HallAvaliableReportVm>();

        if (dateList.Count() > 0)
        {
            foreach (var date in dateList)
            {
                var hallBookingList = await _iRepository.GetAsync(x => x.CheckInTime.Date <= date.Date && x.CheckOutTime.Date >= date.Date && x.BookingType == BookingType.Hall && !x.IsDeleted);
                var unavailableList = hallBookingList.Where(x => x.BookingStatus == BookingServiceStatusEnum.CheckIn || x.BookingStatus == BookingServiceStatusEnum.Booked).ToList();
                var unavailableListIds = unavailableList.Select(x => x.Id);

                var bookingHallList = await _iBookingHallRepository.GetAsync(c => unavailableListIds.Contains(c.BookingId) && c.HallId == hallId && c.BookingDate.Date == date.Date && !c.IsDeleted, b => b.Booking);

                var model = new HallAvaliableReportVm();

                model.BookingDate = date;
                model.Day = date.Date.DayOfWeek.ToString();
                model.HallId = hallInfo.Id;
                model.HallName = hallInfo.HallName;

                if (bookingHallList.Count > 0)
                {
                    model.IsBothAvailable = true;
                    model.IsDayAvailable = true;
                    model.IsNightAvailable = true;

                    foreach (HallBookingShiftEnum shift in Enum.GetValues(typeof(HallBookingShiftEnum)))
                    {
                        if (shift == HallBookingShiftEnum.Both)
                        {
                            var bookedHall = bookingHallList.FirstOrDefault(c => c.HallId == hallId && c.BookingDate.Date == date.Date && c.HallShift == HallBookingShiftEnum.Both);

                            if (bookedHall != null)
                            {
                                model.IsBothAvailable = false;
                                model.IsDayAvailable = false;
                                model.IsNightAvailable = false;

                                model.BookingId = bookedHall.BookingId;
                                model.BookingNo = bookedHall.Booking?.BookingNo;
                            }
                        }
                        else if (shift == HallBookingShiftEnum.DayShift)
                        {
                            var bookedHall = bookingHallList.FirstOrDefault(c => c.HallId == hallId && c.BookingDate.Date == date.Date && c.HallShift == HallBookingShiftEnum.DayShift);

                            if (bookedHall != null)
                            {
                                model.IsDayAvailable = false;
                                model.IsBothAvailable = false;

                                model.BookingId = bookedHall.BookingId;
                                model.BookingNo = bookedHall.Booking?.BookingNo;
                            }
                        }
                        else if (shift == HallBookingShiftEnum.NightShift)
                        {
                            var bookedHall = bookingHallList.FirstOrDefault(c => c.HallId == hallId && c.BookingDate.Date == date.Date && c.HallShift == HallBookingShiftEnum.NightShift);

                            if (bookedHall != null)
                            {
                                model.IsNightAvailable = false;
                                model.IsBothAvailable = false;

                                model.BookingId = bookedHall.BookingId;
                                model.BookingNo = bookedHall.Booking?.BookingNo;
                            }
                        }
                    }
                }
                else
                {
                    model.IsBothAvailable = true;
                    model.IsDayAvailable = true;
                    model.IsNightAvailable = true;
                }

                dataList.Add(model);
            }
        }

        return dataList.OrderBy(x => x.BookingDate).ToList();
    }

    #endregion

    #region GetHallAvailableReportHtml

    public async Task<string> GetHallAvailableReportHtml(HallAvaliableReportVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        var data = await GetHallAvailabilityByDateRange(vm.HallId, vm.FromDateStr, vm.ToDateStr);

        if (data != null && data.Count > 0)
        {
            fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
            fullHtml += "<thead>";

            fullHtml += $@"<tr><th class='text-center' colspan='5'> <h2> {data.FirstOrDefault().HallName} </h2> </th></tr>";

            fullHtml += "<tr style='height:30px;'>";

            fullHtml += $@"<th class='text-center' style='width:200px;'>Date</th>
                            <th class='text-center' style='width:200px;'>Day</th>
                            <th class='text-center' style='width:200px;'>Day Shift</th>
                            <th class='text-center' style='width:200px;'>Night Shift</th>
                            <th class='text-center' style='width:200px;'>Both Shift</th>";

            fullHtml += "</tr>";
            fullHtml += "</thead>";
            fullHtml += "<tbody>";

            for (int i = 0; i < data.Count; i++)
            {
                HallAvaliableReportVm objBooking = data[i];

                var bookingNoHtml = "";
                if (!isPrint)
                {
                    bookingNoHtml += $@"<a target='_blank' href='../BookingService/HallDetails/{objBooking.BookingId}'>{objBooking.BookingNo}</a>";
                }

                fullHtml += "<tr>";

                var dayAvailableText = objBooking.IsDayAvailable ? "Available" : $"Booked";
                var nightAvailableText = objBooking.IsNightAvailable ? "Available" : $"Booked";
                var bothAvailableText = objBooking.IsBothAvailable ? "Available" : $"Booked";

                fullHtml += $@"<td style='text-align:left;'>{DU.Utility.ConvertDateToStr(objBooking.BookingDate)}</td>
                                <td style='text-align:left;padding:5px;'><b>{objBooking.Day}</b></td>
                                <td style='text-align:center;'>{dayAvailableText}</td>
                                <td style='text-align:center;'>{nightAvailableText}</td>
                                <td style='text-align:center;'>{bothAvailableText}</td>";

                fullHtml += "</tr>";

            }
        }
        fullHtml += "</tbody>";

        fullHtml += "</table>";

        return fullHtml;
    }

    #endregion

    #region RoomStatusHistory

    private async Task<(List<HtRoomStatusHistory>, List<HtRoomStatusHistory>)> GetRoomStatusHistory(List<HtRoomInfo> changeStatusRoomList)
    {
        var currentDate = DateTime.Now.Date;
        var currentTimestamp = DU.Utility.GetBdDateTimeNow();

        List<HtRoomStatusHistory> roomStatusHstList = new();
        List<HtRoomStatusHistory> isCurrentChangeHstList = new();

        if (changeStatusRoomList.Count > 0)
        {
            // Fetch all histories for the given rooms in a single query
            var roomIds = changeStatusRoomList.Select(room => room.Id).ToList();
            var allRoomHistories = await _iRoomStatusHistoryRepository.GetAsync(
                x => roomIds.Contains(x.RoomId)
                     && x.StatusDate.Date == currentDate
                     && x.IsCurrent
                     && !x.IsDeleted);

            foreach (var room in changeStatusRoomList)
            {
                var roomHstList = allRoomHistories.Where(h => h.RoomId == room.Id).ToList();

                var roomStatus = room.HouseKeeperAvailabilityStatus switch
                {
                    AvailabilityStatusEnum.Available when room.CleaningStatus == CleaningStatusEnum.VD
                        => RoomStatusHstEnum.VD,
                    AvailabilityStatusEnum.Available => RoomStatusHstEnum.Available,
                    AvailabilityStatusEnum.Occupied => RoomStatusHstEnum.Occupied,
                    AvailabilityStatusEnum.CheckedOut => RoomStatusHstEnum.Available,
                    _ => throw new InvalidOperationException("Unknown RoomStatus")
                };

                // Update current history if room status changes
                foreach (var history in roomHstList)
                {
                    if (history.RoomStatus != roomStatus)
                    {
                        history.IsCurrent = false;
                        isCurrentChangeHstList.Add(history);
                    }
                }

                // Add new history if no existing match
                if (!roomHstList.Any(h => h.RoomStatus == roomStatus))
                {
                    roomStatusHstList.Add(new HtRoomStatusHistory
                    {
                        RoomId = room.Id,
                        RoomStatusById = CurrentUserId,
                        StatusDate = currentDate,
                        IsCurrent = true,
                        ActionDate = currentTimestamp,
                        ActionById = CurrentUserId,
                        RoomStatus = roomStatus
                    });
                }
            }
        }

        return (isCurrentChangeHstList, roomStatusHstList);
    }


    #endregion

    #region AdvanceReportHtml
    public async Task<string> AdvanceReportHtml(AdvanceReportVm vm)
    {
        string fullHtml = await _iRepository.AdvanceReportHtml(vm);
        return fullHtml;
    }
    #endregion

    #region CategoryWiseRoomAvailableReportHtml
    public async Task<string> CategoryWiseRoomAvailableReportHtml(CategoryWiseRoomAvailableReportVm vm, bool isPrint = false)
    {
        string fullHtml = await _iHotelManagementRepository.CategoryWiseRoomAvailableReportHtml(vm, isPrint);
        return fullHtml;
    }
    #endregion

    #region GetPaymentTransactionReportHtml

    public async Task<string> GetPaymentTransactionReportHtml(PaymentTransactionReportVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        var data = await _iRepository.PaymentTransactionReportDataAsync(vm);

        if (vm.ShowOnlyDue)
        {
            data = data.Where(x => x.IsDueCollection).ToList();
        }

        if (data != null && data.Count > 0)
        {
            // === Header Section ===
            if (isPrint)
            {
                fullHtml += $@"<div style='text-align:center;margin-bottom:20px;'>
                                    <p style='margin:4px 0;color:#555;font-size:14px;'>
                                        <b>From:</b> {vm.StrFromDate}  |  <b>To:</b> {vm.StrToDate}
                                    </p>
                                    <hr style='border:1px solid #ddd;width:80%;margin:10px auto;' />
                                </div>";
            }

            var groupedData = data.GroupBy(x => x.PayMode).ToList();
            string actionHeader = isPrint ? "" : "<th style='width:5%;'>Action</th>";

            // === Table Section ===
            fullHtml += $@"<table class='table report-table' id='print_table'>
                            <thead>
                                <tr>
                                    <th style='width:5%;'>SL.</th>
                                    <th style='width:10%;'>Guest</th>
                                    <th style='width:7%;'>Booking Date</th>
                                    <th style='width:7%;'>Pay Mode</th>
                                    <th style='width:10%;'>Particulars</th>
                                    <th style='width:8%;'>Bill No./ Trans. No.</th>
                                    <th style='width:8%;'>Paid Date</th>
                                    <th style='width:5%;'>Paid Amount</th>
                                    <th style='width:20%;'>Remarks</th>
                                    <th style='width:15%;'>Received By</th>
                                    {actionHeader}
                                </tr>
                            </thead>
                            <tbody>";

            double grandTotal = 0;
            var summaryList = new List<(string PayModeName, double TotalAmount)>();

            foreach (var group in groupedData)
            {
                var payModeName = (((PayModeEnum)group.Key).GetDescription());
                fullHtml += isPrint ? $@"<tr><td colspan='10' style='background:#dfe6e9;font-weight:600;color:#2c3e50;text-align:center;'>{payModeName}</td></tr>" : $@"<tr><td colspan='11' style='background:#dfe6e9;font-weight:600;color:#2c3e50;text-align:center;'>{payModeName}</td></tr>";

                int i = 0;
                double subTotal = 0;

                foreach (var obj in group)
                {
                    string bookingLink = string.IsNullOrEmpty(obj.BookingNo) ? "" : @$"<a href='../../BookingService/Details/{obj.BookingId}' target='_blank' style='color:#2980b9;text-decoration:none;font-weight:600;'>{obj.BookingNo}</a>";
                    string billLink = string.IsNullOrEmpty(obj.BillNumber) ? "" : @$"<br/><a href='../../Bill/Details/{obj.BillId}' target='_blank' style='color:#27ae60;text-decoration:none;font-size:12px;'>{obj.BillNumber}</a>";
                    string receiptPrintBtn = !(obj.PaymentId > 0) ? "" : @$"<a href='../../BookingService/MoneyreceiptPrint/{obj.PaymentId}' target='_blank' style='color:#27ae60;text-decoration:none;font-size:12px;'>Receipt</a>";
                    string receiptBtn = isPrint ? "" : $@"<td style='text-align:center;'>{receiptPrintBtn}</td>";
                    string remarks = obj.IsDueCollection ? "Due Collection" : $@"{obj.Description}";

                    fullHtml += $@"<tr>
                                        <td style='text-align:center;'>{++i}</td>
                                        <td style='text-align:center;'>{obj.GuestName}<br/><small style='color:#555;'>{obj.GuestMobile}</small></td>
                                        <td style='text-align:center;'>{obj.BookingDate:dd/MM/yyyy}</td>
                                        <td style='text-align:center;'>{payModeName}</td>
                                        <td style='text-align:center;'>{bookingLink}<br />(Room#{obj.RoomNoList})</td>
                                        <td style='text-align:center;'>{obj.TransactionNo}{billLink}</td>
                                        <td style='text-align:center;'>{obj.PaidDate:dd/MM/yyyy}</td>
                                        <td style='text-align:right;font-weight:600;color:#2c3e50;'>{obj.PaidAmount:N2}</td>
                                        <td style='text-align:left;'>{remarks}</td>
                                        <td style='text-align:left;'>{obj.ReceivedBy}</td>
                                        {receiptBtn}
                                    </tr>";

                    subTotal += obj.PaidAmount;
                }

                // === Subtotal Row ===
                fullHtml += $@"<tr class='subtotal-row'>
                                    <td colspan='7' style='text-align:right;'>{payModeName} Received:</td>
                                    <td style='text-align:right;'>{subTotal:N2}</td>
                                    <td colspan='3'></td>
                                </tr>";

                grandTotal += subTotal;
                summaryList.Add((payModeName, subTotal));
            }

            fullHtml += "</tbody></table>";

            // === Payment Summary Table ===
            fullHtml += @"<div style='display:flex;justify-content:center; '>";
            fullHtml += @"<table class='report-table' style='width:50%; margin-top:20px'>
                                        <thead><tr><th colspan='2' style='text-align:center;'>Payment Summary</th></tr></thead>
                                        <tbody>";

            foreach (var item in summaryList)
            {
                fullHtml += $@"<tr>
                                <td style='text-align:left;'>{item.PayModeName} Received</td>
                                <td style='text-align:right;'>{item.TotalAmount:N2}</td>
                            </tr>";
            }

            fullHtml += $@"<tr class='subtotal-row'>
                            <td style='text-align:right;'>Total Received:</td>
                            <td style='text-align:right;'>{grandTotal:N2}</td>
                        </tr>";

            fullHtml += "</tbody></table>";
            fullHtml += "</div>";

            // === Footer Section ===
            fullHtml += @"<div class='report-footer'>
                    <hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
                    <p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>";
            fullHtml += @"</div>";
        }
        else
        {
            fullHtml += "<h5 style='text-align:center;color:red;'>No Data Found</h5>";
        }


        return fullHtml;
    }

    #endregion

    #region MoneyReceiptHtmlAsync

    public async Task<string> MoneyReceiptHtmlAsync(long id)
    {
        string fullHtml = "";
        var data = await _iRepository.MoneyReceiptDataAsync(id);

        if (data == null || data.Count == 0)
            return "<p class='text-danger text-center'>No data found for this receipt.</p>";

        MoneyReceiptPrintVm vm = data.First();
        vm.AmountInWords = Utility.ConvertToWord(vm.PaidAmount);

        var copies = new[] { "Guest Copy", "Merchant Copy" };


        foreach (var copy in copies)
        {
            var copySeperator = "";
            string logoHtml = "";

            string imageURL = DU.Utility.ServerPath + "\\misuk_logo_sm.jpg";
            string logoUrlForHtml = imageURL.Replace("\\", "/");


            if (copy == "Guest Copy")
            {
                copySeperator = "<hr style='border-top:1px dotted #999;margin:35px 0 20px 0;' />";
                logoHtml = "<div style='text-align:center; '>" +
                               "<img src='" + logoUrlForHtml + "' style='width:100px;height:auto;' />" +
                               "<br/>" +
                               "<p style='font-family: Times-Roman; font-size:12px; margin-bottom:15px;'>Hotel Motel Zone Sea Beach Road, Cox's Bazar, Bangladesh</p>" +
                          "</div>";

            }

            var transactionNoHtml = "<td style='width:80%;padding:4px 0;'>: " +
           (string.IsNullOrEmpty(vm.TransactionNo) ? vm.BillNumber : vm.TransactionNo) +
           (!string.IsNullOrEmpty(vm.BookingNo) ? " | " + vm.BookingNo : "") +
           "</td>";


            fullHtml += $@"<div style='font-family:Arial,Helvetica,sans-serif;color:#222;font-size:13px;margin-bottom:25px;'>

                                <div style='text-align:center;margin-bottom:5px;'>
                                    <h2 style='margin:0;color:#333;text-transform:uppercase;font-weight:bold;'>Money Receipt</h2>
                                    <hr style='border:1px solid #333;width:60%;margin:6px auto;' />
                                </div>

                               <table style='width:100%; border-collapse:collapse; margin-bottom:10px;'>
                                    <tr>
                                        <td style='text-align:left;'>
                                            <div style='display:inline-block; background:#f3f3f3; border:1px solid #ccc; border-radius:4px; padding:5px 10px; font-size:10px;'>
                                                {copy}
                                            </div>
                                        </td>
                                        <td style='text-align:right; font-size:13px; color:#555;'>
                                            {vm.PaidDate:dd MMM yyyy}
                                        </td>
                                    </tr>
                                </table>

                                <table style='width:100%;border-collapse:collapse;margin-bottom:18px;font-size:12px;'>
                                    <tr>
                                        <td style='width:20%;padding:4px 0;font-weight:bold;color:#444;'>Receipt No.</td>
                                        {transactionNoHtml}
                                    </tr>
                                    <tr>
                                        <td style='padding:4px 0;font-weight:bold;color:#444;'>Guest Name</td>
                                        <td>: {vm.GuestName}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding:4px 0;font-weight:bold;color:#444;'>Guest Mobile</td>
                                        <td>: {vm.Mobile}</td>
                                    </tr>
                                    <tr>
                                        <td style='padding:4px 0;font-weight:bold;color:#444;'>Room No.</td>
                                        <td>: {vm.RoomNoList}</td>
                                    </tr>
                                </table>

                                <table cellspacing='0' cellpadding='8' style='width:100%;border-collapse:collapse;font-size:13px;border:1px solid #F2F2F2;'>
                                    <thead style='background:#f0f0f0;color:#222;'>
                                        <tr>
                                            <th style='width:18%;text-align:center;'>Date</th>
                                            <th style='width:25%;text-align:center;'>Invoice/ Tran No.</th>
                                            <th style='width:37%;text-align:center;'>Payment Mode</th>
                                            <th style='width:20%;text-align:right;'>Amount (BDT)</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td style='text-align:center;'>{vm.PaidDate:dd/MM/yyyy}</td>
                                            <td style='text-align:center;'>{(string.IsNullOrEmpty(vm.TransactionNo) ? vm.BillNumber : vm.TransactionNo)}</td>
                                            <td style='text-align:center;'>{(PayModeEnum)vm.PayMode}</td>
                                            <td style='text-align:right;font-weight:bold;'>৳ {vm.PaidAmount:N2}</td>
                                        </tr>
                                    </tbody>
                                </table>

                                <p style='font-size:13px;margin-top:30px;margin-bottom:60px;color:#333;'>
                                    <b>In Words:{vm.AmountInWords} Tk. Only </b> 
                                </p>

                                <div style='position:relative;min-height:80px;margin-top:40px;'>
                                    <table style='width:100%;position:absolute;bottom:0;'>
                                        <tr>
                                            <td style='width:50%;text-align:center;'>
                                                <div style='border-top:1px solid #000;width:60%;margin:0 auto 5px auto;'></div>
                                                <span style='font-size:12px;'>Guest’s Signature</span>
                                            </td>
                                            <td style='width:50%;text-align:center;'>
                                                <div style='border-top:1px solid #000;width:60%;margin:0 auto 5px auto;'></div>
                                                <span style='font-size:12px;'>Authorized Signature</span>
                                            </td>
                                        </tr>
                                    </table>
                                </div>

                                <p style='text-align:center;font-size:12px;margin-top:80px;color:#555;'>
                                    Thank you for choosing to stay with us at Hotel Mishuk, Sea Beach Road, Cox’s Bazar.
                                </p>
                            </div>
                            {copySeperator}
                            {logoHtml}";

        }

        return fullHtml;
    }

    #endregion

    #region CancelReportHtml

    public async Task<string> CancelReportHtml(CancelReportVm vm, bool isPrint = false)
    {
        string fullHtml = await _iHotelManagementRepository.CancelReportHtml(vm, isPrint);
        return fullHtml;
    }
    #endregion

    #region ReviceCancelBooking
    public async Task<bool> ReviceCancelBooking(HtBookingServiceVm vm)
    {
        if (vm == null && !(vm.Id > 0))
            throw new Exception("Booking Service Information Is Not Correct...!!");

        var bookingService = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == vm.Id && x.BookingType == BookingType.Room && !x.IsDeleted && x.BookingStatus == BookingServiceStatusEnum.Canceled);

        if (bookingService == null)
            throw new Exception("Booking Service Not Found...!!");

        if (bookingService.BookingStatus == BookingServiceStatusEnum.CheckOut)
            throw new Exception("Booking Service Already Checked Out...!!");

        if (bookingService.CancelDate == null)
            throw new Exception("Cancel Date Not Found..!!");

        var dayDifference = (DateTime.Today.Date - bookingService.CancelDate?.Date)?.TotalDays;
        if (dayDifference > 90)
            throw new Exception("Booking Cancelled Before 90 Days..!!");

        bookingService.CheckInTime = (DateTime)(!string.IsNullOrEmpty(vm.CheckInTimeStr) ? Utility.ConvertStrToDate(vm.CheckInTimeStr) : DU.Utility.GetBdDateTimeNow());
        bookingService.CheckOutTime = (DateTime)(!string.IsNullOrEmpty(vm.CheckOutTimeStr) ? Utility.ConvertStrToDate(vm.CheckOutTimeStr) : DU.Utility.GetBdDateTimeNow());
        bookingService.VisitPurpose = vm.VisitPurpose;
        bookingService.UpdatedById = CurrentUserId;
        bookingService.UpdateDate = Utility.GetBdDateTimeNow();
        bookingService.BookingStatus = BookingServiceStatusEnum.Booked;
        bookingService.Remarks = @$"Cancelled Date: {bookingService.CancelDate?.Date.ToString("dd/MMM/yyyy")}, Reviced Date: {DateTime.Today.Date.ToString("dd/MMM/yyyy")}, Reviced After {dayDifference}";

        bookingService.CancelDate = null;
        bookingService.CancelBy = null;
        bookingService.CancelReason = null;

        if (bookingService.BookingDate.Date > bookingService.CheckInTime.Date)
            throw new Exception("Check-In Date Is Previous Date Than Booking Date..!!");

        if (bookingService.CheckInTime.Date > bookingService.CheckOutTime.Date)
            throw new Exception("CheakIn & CheckOut Date Is Not Correct..!!");

        #region Room

        List<HtBookingRoom> addableRoomList = null;
        List<HtBookingRoom> deletableRoomList = null;

        bookingService.Rent = 0;
        bookingService.ServiceCharge = 0;
        bookingService.Discount = 0;
        bookingService.NetRent = 0;

        var checkInDateList = new List<DateTime>();
        var checkOutDateList = new List<DateTime>();

        var existBookingRooms = await _iBookingRoomRepository.GetAsync(x => x.BookingId == vm.Id && !x.IsDeleted);

        if (vm?.BookingRoomVms?.Count > 0)
        {
            var dataListForAdd = vm?.BookingRoomVms?.Where(c => c.Id == 0).ToList();

            deletableRoomList = _iMapper.Map<List<HtBookingRoom>>(existBookingRooms);

            if (dataListForAdd?.Count > 0)
            {
                addableRoomList = _iMapper.Map<List<HtBookingRoom>>(dataListForAdd);

                foreach (var (room, i) in addableRoomList.GetItemWithIndex())
                {
                    var filterData = dataListForAdd.FirstOrDefault(c => c.RoomId == room.RoomId);

                    if (filterData == null)
                        throw new Exception("Room Not Found...!");

                    room.BookingId = bookingService.Id;
                    room.CheckInTime = (DateTime)(!string.IsNullOrEmpty(filterData.CheckInTimeStr) ? Utility.ConvertStrToDate(filterData.CheckInTimeStr) : bookingService.CheckInTime);
                    room.CheckOutTime = (DateTime)(!string.IsNullOrEmpty(filterData.CheckOutTimeStr) ? Utility.ConvertStrToDate(filterData.CheckOutTimeStr) : bookingService.CheckOutTime);
                    room.Discount = filterData.Discount;

                    if (room.CheckInTime.Date > room.CheckOutTime.Date)
                        throw new Exception("Room CheakIn & CheckOut Date Is Not Correct..!!");

                    room.ActionDate = Utility.GetBdDateTimeNow();
                    room.ActionById = CurrentUserId;
                    room.UpdateDate = Utility.GetBdDateTimeNow();
                    room.UpdatedById = CurrentUserId;

                    if (room.RoomId != null && room.RoomId > 0)
                    {
                        var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == room.RoomId && x.IsActive && !x.IsDeleted);
                        if (roomInfo == null)
                            throw new Exception("Room Not Found...!!");

                        var checkAvalibility = await _iRoomInfoService.CheckRoomIsAvaliable(roomInfo.Id, room.CheckInTime, room.CheckOutTime);
                        if (!checkAvalibility)
                            throw new Exception("Room Is Not Available...!!");

                        //var days = AppUtility.DaysDiffernce(room.CheckOutTime, (DateTime)room.CheckInTime);

                        var days = AppUtility.DaysDiffernceOnlyDate(room.CheckOutTime, room.CheckInTime);
                        if (days > 0)
                        {
                            room.Rent = (roomInfo.Rent * days);
                            room.ServiceCharge = (roomInfo.ServiceCharge * days);
                            room.NetRent = (room.Rent + room.ServiceCharge) - (room.Discount);
                            room.TotalGuest = roomInfo.Person;
                        }
                        else
                        {
                            throw new Exception("Booking Days Have To More Than Zero....!!");
                        }
                    }

                    checkInDateList.Add(room.CheckInTime);
                    checkOutDateList.Add(room.CheckOutTime);
                }

                bookingService.Rent += addableRoomList.Sum(x => x.Rent);
                bookingService.ServiceCharge += addableRoomList.Sum(x => x.ServiceCharge);
                bookingService.Discount += addableRoomList.Sum(x => x.Discount);
                bookingService.NetRent = bookingService.Rent + bookingService.ServiceCharge - (bookingService.Discount);
            }

            var minCheckInDate = checkInDateList.Min();
            var maxCheckOutDate = checkOutDateList.Max();

            if (minCheckInDate.Date != bookingService.CheckInTime.Date)
                bookingService.CheckInTime = minCheckInDate;

            if (maxCheckOutDate.Date != bookingService.CheckOutTime.Date)
                bookingService.CheckOutTime = maxCheckOutDate;

        }

        #endregion

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        _iRepository.Update(bookingService);

        if (deletableRoomList?.Count > 0)
        {
            _iBookingRoomRepository.RemoveRange(deletableRoomList);
        }

        if (addableRoomList?.Count > 0)
        {
            _iBookingRoomRepository.AddRange(addableRoomList);
        }

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }

    #endregion

    #region UpdateBill

    private async Task<HtBilling> UpdateBill(HtBookingService booking, List<HtBookingRoom> roomList)
    {
        if (booking == null && roomList == null && !(roomList.Count > 0))
            return null;

        var bill = await _iBillRepository.GetFirstOrDefaultAsync(x => x.BookingId == booking.Id && !x.IsDeleted,
            include: q => q
                .Include(b => b.BillingDetails)
                    .ThenInclude(d => d.Service)
                        .ThenInclude(s => s.Ledger)
                );
        if (bill == null)
            throw new Exception("Bill Not Found For Update...!!");

        bill.BillDate = Utility.GetBdDateTimeNow();
        bill.UpdatedById = CurrentUserId;
        bill.UpdateDate = Utility.GetBdDateTimeNow();

        #region Room
        foreach (var bookingRoom in roomList)
        {
            var detailModel = new HtBillingDetail();

            var roomRentalService = await _iHtServiceRepository.GetFirstOrDefaultAsync(x => x.ServiceCode == HtServiceCode.RoomRent, l => l.Ledger);
            if (roomRentalService == null)
                throw new Exception("Room Rent Service Not Found...!!");

            var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == bookingRoom.RoomId && x.IsActive && !x.IsDeleted);
            if (roomInfo == null)
                throw new Exception("Room Not Found...!!");

            double days = AppUtility.DaysDiffernceOnlyDate((DateTime)bookingRoom.ActualCheckOutTime, bookingRoom.CheckInTime);

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
            detailModel.ActionById = CurrentUserId;
            detailModel.ActionDate = Utility.GetBdDateTimeNow();
            detailModel.Rate = roomInfo.Rent;
            detailModel.Amount = bookingRoom.Rent;
            detailModel.VAT = bookingRoom.Vat;
            detailModel.Tax = bookingRoom.Tax;
            detailModel.Discount = bookingRoom.Discount;
            detailModel.ServiceCharge = bookingRoom.ServiceCharge;
            detailModel.ExtraBedCharge = bookingRoom.ExtraBedCharge;
            detailModel.NetAmount = bookingRoom.NetRent;
            detailModel.Remarks = $"Bill For Room No. {roomInfo.RoomNo}";

            detailModel.Service = roomRentalService;

            var roomServiceExist = bill.BillingDetails.FirstOrDefault(x => x.BookingRoomId == bookingRoom.Id && x.ServiceId == roomRentalService.Id);
            if (roomServiceExist != null)
                continue;

            bill.BillingDetails.Add(detailModel);
        }
        #endregion

        #region FoodService

        var bookingFoodBillList = await _iFoodOrderRepository.GetAsync(x => x.BookingId == bill.BookingId && x.OrderStatus != RsOrderStatusEnum.Canceled,
            c => c.CustomerType);

        foreach (var bookingRoom in roomList)
        {
            var detailModel = new HtBillingDetail();

            var foodService = await _iHtServiceRepository.GetFirstOrDefaultAsync(x => x.ServiceCode == HtServiceCode.FoodService);
            if (foodService == null)
                throw new Exception("Food Service Not Found...!!");

            var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == bookingRoom.RoomId && x.IsActive && !x.IsDeleted);
            if (roomInfo == null)
                throw new Exception("Room Not Found...!!");

            var anyUnpaidFoodBillList = await _iFoodOrderRepository.GetAsync(x => x.RoomId == roomInfo.Id && x.BookingId == bill.BookingId && x.PaymentStatus != RsOrderPaymentStatusEnum.FullPayment && x.OrderStatus != RsOrderStatusEnum.Canceled, c => c.CustomerType);

            anyUnpaidFoodBillList = anyUnpaidFoodBillList.Where(x => x.CustomerType.TypeCode == RsCustomerTypeCode.Hotel || x.CustomerType.TypeCode == RsCustomerTypeCode.WalkIn).ToList();

            if (anyUnpaidFoodBillList == null || !(anyUnpaidFoodBillList.Count > 0))
            {
                continue;
            }

            detailModel.BookingRoomId = bookingRoom.Id;
            detailModel.ServiceId = foodService.Id;
            detailModel.ActionById = CurrentUserId;
            detailModel.ActionDate = Utility.GetBdDateTimeNow();
            detailModel.Quantity = anyUnpaidFoodBillList.Count();

            detailModel.Amount = anyUnpaidFoodBillList.Sum(x => x.OrderAmount);
            detailModel.VAT = anyUnpaidFoodBillList.Sum(x => x.VAT);
            detailModel.Tax = anyUnpaidFoodBillList.Sum(x => x.TAX);
            detailModel.Discount = anyUnpaidFoodBillList.Sum(x => x.Discount);
            detailModel.ServiceCharge = anyUnpaidFoodBillList.Sum(x => x.ServiceCharge);

            var unpaidIds = anyUnpaidFoodBillList.Select(x => x.Id).ToList();
            var paidList = _iOrderPaymentRepository.Get(c => unpaidIds.Contains(c.OrderId) && c.PaymentType == RsOrderPaymentTypeEnum.Receive && !c.IsDeleted).ToList();
            var alreadyPaidAmount = paidList.Sum(x => x.PaidAmount);

            detailModel.NetAmount = anyUnpaidFoodBillList.Sum(x => x.NetAmount) - (alreadyPaidAmount);

            detailModel.Remarks = $"Food Service Bill For Room No. {roomInfo.RoomNo}";

            bill.BillingDetails.Add(detailModel);
        }

        #endregion

        return bill;
    }

    #endregion

    #region CheckInUpdateBill

    private async Task<HtBilling> CheckInUpdateBill(HtBookingService booking, List<HtBookingRoom> roomList)
    {
        if (booking == null && roomList == null && !(roomList.Count > 0))
            return null;

        var bill = await _iBillRepository.GetFirstOrDefaultAsync(x => x.BookingId == booking.Id && !x.IsDeleted, d => d.BillingDetails);
        if (bill == null)
            throw new Exception("Bill Not Found For Update...!!");

        bill.UpdatedById = CurrentUserId;
        bill.UpdateDate = Utility.GetBdDateTimeNow();

        List<HtBillingDetail> modelDetails = new List<HtBillingDetail>();

        var checkOutRoomList = roomList.Where(x => x.ActualCheckOutTime != null).ToList();

        var roomRentalService = await _iHtServiceRepository.GetFirstOrDefaultAsync(x => x.ServiceCode == HtServiceCode.RoomRent);
        if (roomRentalService == null)
            throw new Exception("Room Rent Service Not Found...!!");

        #region RoomList

        foreach (var bookingRoom in checkOutRoomList)
        {
            var detailModel = new HtBillingDetail();

            var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == bookingRoom.RoomId && x.IsActive && !x.IsDeleted);
            if (roomInfo == null)
                throw new Exception("Room Not Found...!!");

            double days = AppUtility.DaysDiffernceOnlyDate((DateTime)bookingRoom.ActualCheckOutTime, (DateTime)bookingRoom.ActualCheckInTime);

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
            detailModel.ActionById = CurrentUserId;
            detailModel.ActionDate = Utility.GetBdDateTimeNow();
            detailModel.Rate = roomInfo.Rent;
            detailModel.Amount = bookingRoom.Rent;
            detailModel.VAT = bookingRoom.Vat;
            detailModel.Tax = bookingRoom.Tax;
            detailModel.Discount = bookingRoom.Discount;
            detailModel.ServiceCharge = bookingRoom.ServiceCharge;
            detailModel.ExtraBedCharge = bookingRoom.ExtraBedCharge;
            detailModel.NetAmount = bookingRoom.NetRent;
            detailModel.Remarks = $"Bill For Room No. {roomInfo.RoomNo}";

            var roomServiceExist = bill.BillingDetails.FirstOrDefault(x => x.BookingRoomId == bookingRoom.Id && x.ServiceId == roomRentalService.Id);
            if (roomServiceExist != null)
                continue;

            bill.BillingDetails.Add(detailModel);
        }

        #endregion

        return bill;
    }

    #endregion

    #region BookingOccupiedRoom
    public async Task<IEnumerable<SelectListItem>> GetBookingOccupiedRoom(long bookingId, bool isDefaultSelectAdd = true)
    {
        var items = new List<SelectListItem>();
        if (isDefaultSelectAdd) items.Add(new SelectListItem { Value = "", Text = "---Select---" });
        var roomList = await _iBookingRoomRepository.GetBookedRoom(bookingId);
        items.AddRange(roomList.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.RoomNo }));
        return items;
    }
    #endregion

    #region ExtraServiceReportHtml

    public async Task<string> ExtraServiceReportHtml(ExtraServiceReportVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        var data = await _iRepository.ExtraServiceReportDataAsync(vm);

        if ((data == null || !data.Any()) && isPrint)
            return "<p class='text-danger text-center'>No data found for the selected criteria.</p>";

        if (data != null && data.Count > 0)
        {
            // === Header Section ===
            if (isPrint)
            {
                fullHtml += $@"<div style='text-align:center;margin-bottom:20px;'>
                            <p style='margin:4px 0;color:#555;font-size:14px;'>
                                <b>From:</b> {vm.StrFromDate} | <b>To:</b> {vm.StrToDate}
                            </p>
                            <hr style='border:1px solid #ddd;width:80%;margin:10px auto;' />
                       </div>";
            }

            double grandTotal = 0;
            double grandDiscount = 0;
            double grandVat = 0;
            double grandServiceCharge = 0;
            double totalDiscountedRate = 0;

            fullHtml += @"<table class='report-table' id='print_table' width='100%' style='border-collapse:collapse;font-size:12px;'>
                        <thead>
                            <tr style='background-color:#f2f2f2;'>
                                <th style='width:5%;text-align:center;'>SL.</th>
                                <th style='width:15%;text-align:center;'>Service</th>
                                <th style='width:10%;text-align:center;'>Service Date</th>
                                <th style='width:12%;text-align:center;'>Booking/Bill No.</th>
                                <th style='width:8%;text-align:center;'>Room No</th>
                                <th style='width:8%;text-align:center;'>Rate</th>
                                <th style='width:10%;text-align:center;'>Discounted Rate (Rate - Discount)</th>
                                <th style='width:6%;text-align:center;'>Qty</th>
                                <th style='width:6%;text-align:center;'>Amount</th>
                                <th style='width:8%;text-align:center;'>Service Charge</th>
                                <th style='width:6%;text-align:center;'>VAT</th>
                                <th style='width:8%;text-align:center;'>Discount</th>
                                <th style='width:8%;text-align:center;'>Total Amount</th>
                            </tr>
                        </thead>
                        <tbody>";

            foreach (var (obj, i) in data.GetItemWithIndex())
            {
                string bookingLink = string.IsNullOrEmpty(obj.BookingNo)
                    ? ""
                    : $"<a href='../../BookingService/Details/{obj.BookingId}' target='_blank' " +
                      $"style='color:#2980b9;text-decoration:none;font-weight:600;'>{obj.BookingNo}</a>";

                string billLink = string.IsNullOrEmpty(obj.BillNo)
                    ? ""
                    : $"<a href='../../Bill/Details/{obj.BillId}' target='_blank' " +
                      $"style='color:#27ae60;text-decoration:none;font-size:12px;'>{obj.BillNo}</a>";

                double discountedAmount = obj.Rate - (obj.Discount / (obj.Quantity == 0 ? 1 : obj.Quantity));

                // accumulate totals
                grandTotal += obj.TotalAmount;
                grandDiscount += obj.Discount;
                grandVat += obj.VAT;
                grandServiceCharge += obj.ServiceCharge;
                totalDiscountedRate += discountedAmount;

                fullHtml +=
                            $@"<tr>
                            <td style='text-align:center;'>{i + 1}</td>
                            <td style='text-align:left;'>{System.Net.WebUtility.HtmlEncode(obj.ServiceName)}</td>
                            <td style='text-align:center;'>{obj.ServiceDate:dd/MM/yyyy}</td>
                            <td style='text-align:center;'>{bookingLink}<br />{billLink}</td>
                            <td style='text-align:center;font-weight:600;'>{obj.RoomNo}</td>

                            <td style='text-align:right;'>{obj.Rate:N2}</td>
                            <td style='text-align:right;'>{discountedAmount}</td>
                            <td style='text-align:center;'>{obj.Quantity}</td>
                            <td style='text-align:right;'>{obj.Amount:N2}</td>
                            <td style='text-align:right;'>{obj.ServiceCharge:N2}</td>
                            <td style='text-align:right;'>{obj.VAT:N2}</td>
                            <td style='text-align:right;'>{obj.Discount:N2}</td>
                            <td style='text-align:right;font-weight:600;'>{obj.TotalAmount:N2}</td>
                        </tr>";
            }

            // Footer row for totals
            fullHtml += $@"<tr style='font-weight:700;background-color:#f8f9fa;'>
                        <td colspan='6' style='text-align:right;'>Grand Totals:</td>
                        <td style='text-align:right;'>{totalDiscountedRate:N2}</td>
                        <td style='text-align:center;'>{data.Sum(x => x.Quantity)}</td>
                        <td style='text-align:right;'>{data.Sum(x => x.Amount):N2}</td>
                        <td style='text-align:right;'>{grandServiceCharge:N2}</td>
                        <td style='text-align:right;'>{grandVat:N2}</td>
                        <td style='text-align:right;'>{grandDiscount:N2}</td>
                        <td style='text-align:right;'>{grandTotal:N2}</td>
                    </tr>";

            fullHtml += @"</tbody></table>";

            // Footer section
            fullHtml += $@"<div style='text-align:center;margin-top:20px;font-size:11px;color:#666;'>
                    <hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
                    <p>Generated on: {DateTime.Now:dd MMM yyyy hh:mm tt}</p>
                   </div>";
        }
        else
        {
            fullHtml += "<h5 style='text-align:center;color:red;'>No Data Found</h5>";
        }

        return fullHtml;
    }


    #endregion

    #region BookingHallReportHtml

    public async Task<string> BookingHallReportHtml(BookingHallReportVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        var data = await _iBookingHallRepository.BookingHallReportDataAsync(vm);

        if ((data == null || !data.Any()) && isPrint)
            return "<p class='text-danger text-center'>No data found for the selected criteria.</p>";

        if (data != null && data.Count > 0)
        {
            // === Header Section ===
            if (isPrint)
            {
                fullHtml += $@"<div style='text-align:center;margin-bottom:20px;'>
                        <p style='margin:4px 0;color:#555;font-size:14px;'>
                            <b>From:</b> {vm.StrFromDate} | <b>To:</b> {vm.StrToDate}
                        </p>
                        <hr style='border:1px solid #ddd;width:80%;margin:10px auto;' />
                   </div>";
            }

            // Totals
            double totalHallRent = 0;
            double totalNetRent = 0;
            double totalServiceCharge = 0;
            double totalVat = 0;
            double totalDiscount = 0;

            fullHtml += @"<table class='report-table' id='print_table' width='100%' 
                        style='border-collapse:collapse;font-size:12px;'>
                    <thead>
                        <tr style='background-color:#f2f2f2;'>
                            <th style='width:5%;text-align:center;'>SL.</th>
                            <th style='width:10%;text-align:center;'>Booking No</th>
                            <th style='width:10%;text-align:center;'>Booking Date</th>
                            <th style='width:10%;text-align:center;'>Party Date</th>
                            <th style='width:10%;text-align:center;'>Hall Name</th>
                            <th style='width:8%;text-align:center;'>Hall Shift</th>
                            <th style='width:8%;text-align:right;'>Hall Rent</th>
                            <th style='width:8%;text-align:right;'>Service Charge</th>
                            <th style='width:8%;text-align:right;'>VAT</th>
                            <th style='width:8%;text-align:right;'>Discount</th>
                            <th style='width:8%;text-align:right;'>Net Rent</th>
                            <th style='width:10%;text-align:center;'>Guest</th>
                            <th style='width:10%;text-align:center;'>Mobile</th>
                        </tr>
                    </thead>
                    <tbody>";

            foreach (var (obj, i) in data.GetItemWithIndex())
            {
                // Totals accumulate
                totalHallRent += obj.HallRent;
                totalServiceCharge += obj.ServiceCharge;
                totalVat += obj.Vat;
                totalDiscount += obj.Discount;
                totalNetRent += obj.NetRent;

                fullHtml +=
                    $@"<tr>
                    <td style='text-align:center;'>{i + 1}</td>

                    <td style='text-align:center;font-weight:600;'>{obj.BookingNo}</td>

                    <td style='text-align:center;'>{obj.BookingDate:dd/MM/yyyy}</td>
                    <td style='text-align:center;'>{obj.PartyDate:dd/MM/yyyy}</td>

                    <td style='text-align:left;'>{System.Net.WebUtility.HtmlEncode(obj.HallName)}</td>
                    <td style='text-align:center;'>{obj.HallShift}</td>

                    <td style='text-align:right;'>{obj.HallRent:N2}</td>
                    <td style='text-align:right;'>{obj.ServiceCharge:N2}</td>
                    <td style='text-align:right;'>{obj.Vat:N2}</td>
                    <td style='text-align:right;'>{obj.Discount:N2}</td>

                    <td style='text-align:right;font-weight:600;'>{obj.NetRent:N2}</td>

                    <td style='text-align:left;'>{obj.GuestName}</td>
                    <td style='text-align:center;'>{obj.Mobile}</td>
                </tr>";
            }

            // === Footer Totals Row ===
            fullHtml += $@"<tr style='font-weight:700;background-color:#f8f9fa;'>
                    <td colspan='6' style='text-align:right;'>Grand Totals:</td>

                    <td style='text-align:right;'>{totalHallRent:N2}</td>
                    <td style='text-align:right;'>{totalServiceCharge:N2}</td>
                    <td style='text-align:right;'>{totalVat:N2}</td>
                    <td style='text-align:right;'>{totalDiscount:N2}</td>

                    <td style='text-align:right;'>{totalNetRent:N2}</td>

                    <td colspan='2'></td>
                </tr>";

            fullHtml += @"</tbody></table>";

            // Footer section
            fullHtml += $@"<div style='text-align:center;margin-top:20px;font-size:11px;color:#666;'>
                <hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
                <p>Generated on: {DateTime.Now:dd MMM yyyy hh:mm tt}</p>
               </div>";
        }
        else
        {
            fullHtml += "<h5 style='text-align:center;color:red;'>No Data Found</h5>";
        }

        return fullHtml;
    }

    #endregion

    #region IsVcRoom
    public bool IsVcRoom(HtRoomInfo room)
    {
        if (room == null) return false;
        if (room.CleaningStatus == CleaningStatusEnum.VC)
            return true;
        else
            return false;
    }
    #endregion

    #region ReservationReportHtml

    public async Task<string> ReservationReportHtml(ReservationReportVm vm, bool isPrint = false)
    {
        string fullHtml = await _iHotelManagementRepository.ReservationReportHtml(vm, isPrint);
        return fullHtml;
    }

    #endregion

    #region GetHtPaymentAutoCode

    public async Task<string> GetHtPaymentAutoCode()
    {
        var data = await _iAutoCodeRepository.GetHtPaymentsAutoCode("HtBookingPayments", "TransactionNo", "MR-", 6);
        return data;
    }

    #endregion

    #region RemoveComplementary
    public async Task<bool> RemoveComplementary(long bookingRoomId)
    {
        var bookingroom = await _iBookingRoomRepository.GetByIdAsync(bookingRoomId);

        if (bookingroom == null)
            throw new Exception("No Booked Room Found..!!");

        bookingroom.ComplementaryId = null;

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iBookingRoomRepository.UpdateAsync(bookingroom);

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();

        return true;
    }
    #endregion

    #region ComplementaryEntry
    public async Task<bool> ComplementaryEntryAsync(long bookingRoomId, long complementaryId)
    {
        var bookingroom = await _iBookingRoomRepository.GetByIdAsync(bookingRoomId);

        if (bookingroom == null)
            throw new Exception("No Booked Room Found..!!");

        if (bookingroom.ComplementaryId > 0)
            throw new Exception("Complementary Alrady Added");

        bookingroom.ComplementaryId = complementaryId;

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iBookingRoomRepository.UpdateAsync(bookingroom);

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();

        return true;
    }
    #endregion

    #region GetRoomChangeReportHtml

    public async Task<string> GetRoomChangeReportHtml(RoomChangeReportVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        var data = await _iRepository.RoomChangeReportDataAsync(vm);

        if ((data == null || !data.Any()) && isPrint)
            return "<p class='text-danger text-center'>No data found for the selected criteria.</p>";

        if (data != null && data.Count > 0)
        {
            // === Header Section ===            
            if (isPrint)
            {
                fullHtml += $@"<div style='text-align:center;margin-bottom:20px;'>
                                    <p style='margin:4px 0;color:#555;font-size:14px;'>
                                        <b>From:</b> {vm.StrFromDate}  |  <b>To:</b> {vm.StrToDate}
                                    </p>
                                    <hr style='border:1px solid #ddd;width:80%;margin:10px auto;' />
                                </div>";
            }

            // === Table Section ===
            fullHtml += @"<table class='report-table' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;font-size:xx-small' border='1'>>
                            <thead>
                                <tr>
                                    <th style='width:5%;'>SL.</th>
                                    <th style='width:10%;'>Change Date</th>
                                    <th style='width:10%;'>Booking/Bill No.</th>
                                    <th style='width:10%;'>Guest</th>
                                    <th style='width:12%;'>Old Room</th>
                                    <th style='width:12%;'>New Room</th>
                                    <th style='width:10%;'>Shift By</th>
                                    <th style='width:16%;'>Reason</th>
                                    <th style='width:15%;'>Remarks</th>
                                </tr>
                            </thead>
                            <tbody>";

            foreach (var (obj, i) in data.GetItemWithIndex())
            {
                string bookingLink = string.IsNullOrEmpty(obj.BookingNo)
                    ? ""
                    : @$"<a href='../../BookingService/Details/{obj.BookingId}' target='_blank' style='color:#2980b9;text-decoration:none;font-weight:600;'>{obj.BookingNo}</a>";

                //string billLink = string.IsNullOrEmpty(obj.BillNumber)
                //    ? ""
                //    : @$"<br/><a href='../../Bill/Details/{obj.BillId}' target='_blank' style='color:#27ae60;text-decoration:none;font-size:12px;'>{obj.BillNumber}</a>";

                fullHtml += $@"<tr>
                                    <td style='text-align:center;'>{i + 1}</td>
                                    <td style='text-align:center;'>{obj.ChangeDate:dd/MM/yyyy}</td>
                                    <td style='text-align:center;'>{bookingLink}</td>
                                    <td style='text-align:center;'>{obj.GuestName}<br/><small style='color:#555;'>{obj.GuestMobile}</small></td>
                                    <td style='text-align:center;font-weight:600;'>{obj.OldRoomNo}<br/><small>{obj.OldCategoryName}</small></td>
                                    <td style='text-align:center;font-weight:600;'>{obj.NewRoomNo}<br/><small>{obj.NewCategoryName}</small></td>
                                    <td style='text-align:center;'>{obj.SubmittedBy}</td>
                                    <td style='text-align:left;'>{obj.Reason}</td>
                                    <td style='text-align:left;'>{obj.Remarks}</td>
                                </tr>";
            }

            fullHtml += "</tbody></table>";

            fullHtml += @"<div class='report-footer'>
                            <hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
                            <p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + @"</p>
                        </div>";

        }
        else
        {
            fullHtml += "<h5 style='text-align:center;color:red;'>No Data Found</h5>";
        }

        return fullHtml;
    }

    #endregion

    #region InHouseGuestLedgerReportHtml
    public async Task<string> InHouseGuestLedgerReportHtml(InHouseGuestLedgerReportVm vm)
    {
        string fullHtml = await _iRepository.InHouseGuestLedgerReportUpdateHtml(vm);
        return fullHtml;
    }
    #endregion

    #region
    public async Task<double> TodayExtraServiceReportCount(ExtraServiceReportVm vm)
    {
        var data = await _iRepository.ExtraServiceReportDataAsync(vm);
        var quantity = data.Sum(x => x?.Quantity) ?? 0;
        return quantity;
    }
    #endregion
}
