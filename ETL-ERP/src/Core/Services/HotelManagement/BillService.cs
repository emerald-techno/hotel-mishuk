using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Billing;
using Domain.ViewModel.HotelManagement.RoomBooking;
using Domain.ViewModel.Restaurant.FoodOrder;
using Interface.Repository.Accounts;
using Interface.Repository.Common;
using Interface.Repository.HotelManagement;
using Interface.Repository.Restaurant;
using Interface.Services.Accounts;
using Interface.Services.Admin;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Base;
using System.Transactions;
using DU = Domain.Utility;

namespace Services.HotelManagement;

public class BillService : BaseService<HtBilling>, IBillService
{
    #region Config
    private IBillingRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IBookingGuestRepository _iBookingGuestRepository;
    private readonly IBookingPaymentRepository _iBookingPaymentRepository;
    private readonly IBookingServiceRepository _iBookingServiceRepository;
    private readonly IBookingRoomRepository _iBookingRoomRepository;
    private readonly IAccTranMstRepository _iAccTranMstRepository;
    private readonly IAutoCodeRepository _iAutoCodeRepository;
    private readonly IBillingDetailRepository _iBillingDetailRepository;
    private readonly IServiceRepository _iServiceRepository;
    private readonly IRsOrderPaymentRepository _iOrderPaymentRepository;

    private readonly IFoodOrderRepository _iFoodOrderRepository;
    private readonly IRsOrderPaymentRepository _iRsOrderPaymentRepository;

    private readonly ISetCurrencyService _iSetCurrencyService;
    private readonly ISetFincYearService _iSetFincYearService;
    private readonly IAccLedgerService _iAccLedgerService;

    public BillService(IBillingRepository iRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork,
        IBookingGuestRepository iBookingGuestRepository,
        IBookingPaymentRepository iBookingPaymentRepository,
        IBookingServiceRepository iBookingServiceRepository,
        ISetCurrencyService iSetCurrencyService,
        ISetFincYearService iSetFincYearService,
        IAccLedgerService iAccLedgerService,
        IAccTranMstRepository iAccTranMstRepository,
        IAutoCodeRepository iAutoCodeRepository,
        IBillingDetailRepository iBillingDetailRepository,
        IServiceRepository iServiceRepository,
        IFoodOrderRepository iFoodOrderRepository,
        IRsOrderPaymentRepository iRsOrderPaymentRepository,
        IBookingRoomRepository iBookingRoomRepository,
        IRsOrderPaymentRepository iOrderPaymentRepository) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iBookingGuestRepository = iBookingGuestRepository;
        _iBookingPaymentRepository = iBookingPaymentRepository;
        _iBookingServiceRepository = iBookingServiceRepository;
        _iAccTranMstRepository = iAccTranMstRepository;
        _iAutoCodeRepository = iAutoCodeRepository;
        _iServiceRepository = iServiceRepository;
        _iFoodOrderRepository = iFoodOrderRepository;
        _iRsOrderPaymentRepository = iRsOrderPaymentRepository;
        _iBookingRoomRepository = iBookingRoomRepository;

        _iSetCurrencyService = iSetCurrencyService;
        _iSetFincYearService = iSetFincYearService;
        _iAccLedgerService = iAccLedgerService;
        _iBillingDetailRepository = iBillingDetailRepository;
        _iOrderPaymentRepository = iOrderPaymentRepository;
    }

    #endregion

    #region Search
    public async Task<DataTablePagination<BillingSearchVm, BillingSearchVm>> SearchAsync(DataTablePagination<BillingSearchVm, BillingSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }
    #endregion

    #region GetBillById

    public async Task<BillingVm> GetBillByIdAsync(long id)
    {
        var data = await _iRepository.GetBillByIdAsync(id);
        var model = _iMapper.Map<BillingVm>(data);
        model.BookingNo = data.Booking.BookingNo;
        model.BookingDate = data.Booking.BookingDate;
        model.BookingType = data.Booking.BookingType;
        model.BookingStatus = data.Booking.BookingStatus;
        model.BillByName = data.BillBy.FullName;

        var bookingGuest = await _iBookingGuestRepository.GetFirstOrDefaultAsync(x => x.BookingId == model.BookingId && x.IsMain && !x.IsDeleted, g => g.Guest);
        if (bookingGuest == null)
            throw new Exception("Guest Not Found For Billing...!!");

        model.BookingGuestId = bookingGuest.GuestId;
        model.BookingGuestName = $"{bookingGuest.Guest.Salutation} {bookingGuest.Guest.FirstName} {bookingGuest.Guest.LastName}";
        model.BookingGuestMobile = bookingGuest.Guest.Mobile;
        model.BookingGuestAddress = bookingGuest.Guest.Address;
        model.TotalGuest = data.Booking.TotalGuest;


        if (model.BillingDetails.Count > 0)
        {
            foreach (var dtl in model.BillingDetails)
            {
                var filterData = data.BillingDetails.FirstOrDefault(c => c.Id == dtl.Id);

                dtl.ServiceName = filterData?.Service.ServiceName;
                dtl.ServiceCode = filterData?.Service.ServiceCode;
                dtl.LedgerId = filterData?.Service?.LedgerId;
                dtl.LedgerName = filterData?.Service?.Ledger?.LedgerName;
                dtl.LedgerCode = filterData?.Service?.Ledger?.LedgerCode;
                dtl.BookingRoomNo = filterData?.BookingRoom?.Room?.RoomNo;
                dtl.BookingRoomCheckInTime = filterData?.BookingRoom?.CheckInTime;
                dtl.BookingRoomCheckOutTime = filterData?.BookingRoom?.ActualCheckOutTime;

                if (data.Booking.BookingStatus == BookingServiceStatusEnum.NoShow)
                {
                    dtl.BookingRoomCheckOutTime = filterData?.BookingRoom?.CheckOutTime;
                }

                //dtl.Days = dtl.BookingRoomCheckInTime != null ? AppUtility.DaysDiffernce((DateTime)dtl.BookingRoomCheckOutTime, (DateTime)dtl.BookingRoomCheckInTime) : 0;
                dtl.Days = dtl.BookingRoomCheckInTime != null ? AppUtility.DaysDiffernceOnlyDate((DateTime)dtl.BookingRoomCheckOutTime, (DateTime)dtl.BookingRoomCheckInTime) : 0;

                //dtl.Days = dtl.Days == 0 ? 1 : dtl.Days;

                if (filterData?.BookingRoom?.BookingDayStatus == (int)BookingDayStatusEnum.HalfDay)
                {
                    dtl.Days = dtl.Days + 0.5;
                }
                else if (filterData?.BookingRoom?.BookingDayStatus == (int)BookingDayStatusEnum.DayUse)
                {
                    dtl.Days = dtl.Days + 1;
                }

                dtl.BookingRoomNo = filterData?.BookingRoom?.Room.RoomNo;
                dtl.BookingRoomRent = filterData?.BookingRoom?.Rent;
                dtl.BookingRoomDiscount = filterData?.BookingRoom?.Discount;
                dtl.BookingRoomServiceCharge = filterData?.BookingRoom?.ServiceCharge;
                dtl.BookingRoomExtraBedCharge = filterData?.BookingRoom?.ExtraBedCharge;
                dtl.BookingRoomExtraBed = filterData?.BookingRoom?.ExtraBed;



                dtl.BookingHallName = filterData?.BookingHall?.Hall.HallName;
                dtl.BookingHallBookDate = filterData?.BookingHall?.BookingDate;
                dtl.BookingHallBookShift = filterData.BookingHall != null ? (int)filterData?.BookingHall?.HallShift : 0;
                dtl.BookingHallRent = filterData?.BookingHall?.Rent;

                if (data.Booking.BookingType == BookingType.Hall)
                {
                    dtl.Days = dtl.BookingHallBookShift == 3 ? 2 : 1;
                }


                dtl.BookingRoomCategoryId = filterData?.BookingRoom?.Room.RoomCategoryId;
                dtl.BookingRoomCategoryName = filterData?.BookingRoom?.Room.RoomCategory.CategoryName;
                dtl.BookingDayStatus = (filterData?.BookingRoom?.BookingDayStatus != null) ? Convert.ToInt32(filterData?.BookingRoom?.BookingDayStatus) : 0;

            }
        }

        model.BillServiceLookUp = model.BillingDetails.DistinctBy(d => d.ServiceId).Select(x => new SelectListItem { Text = $"{x.ServiceName}", Value = $"{x.ServiceId}" }).ToList();
        //model.SpecialDiscount = model.Discount - (model.BillingDetails.Sum(s => s.Discount));

        model.SetBillStatusInfo();

        #region Payments

        var paidList = await _iBookingPaymentRepository.GetAsync(x => x.BookingId == model.BookingId && !x.IsDeleted);
        model.BillingPayments = _iMapper.Map<List<HtBookingPaymentVm>>(paidList);

        //Advance
        var advanceList = paidList.Where(x => x.BillingId == null).ToList();

        model.AdvanceAmount = advanceList.Sum(x => x.PaidAmount);
        model.MrList = advanceList.Any()
            ? string.Join(",", advanceList.Select(x => x.TransactionNo))
            : "";

        //Voucher
        var paymentIds = paidList.Select(x => x.Id);
        var voucherList = await _iAccTranMstRepository.GetAsync(x => paymentIds.Contains(x.PaymentId.Value));

        if (voucherList.Count > 0)
        {
            foreach (var payment in model.BillingPayments)
            {
                var voucher = voucherList.FirstOrDefault(v => v.PaymentId == payment.Id);
                if (voucher != null)
                {
                    payment.VoucherNo = voucher.VcNo;
                    payment.VoucherId = voucher.Id;
                }
            }

        }

        #endregion

        return model;
    }

    #endregion

    #region PayBill

    public async Task<bool> PayBill(PayBillVm vm)
    {
        if (vm == null && !(vm.BillId > 0) && !(vm.PaidAmount > 0))
            throw new Exception("Information is not correct..!!");

        var paymentModel = _iMapper.Map<HtBookingPayment>(vm);
        paymentModel.PaidDate = (DateTime)(!string.IsNullOrEmpty(vm.PaidDateStr)
            ? Utility.ConvertStrToDate(vm.PaidDateStr)
            : DU.Utility.GetBdDateTimeNow());
        paymentModel.ActionById = CurrentUserId;
        paymentModel.ActionDate = Utility.GetBdDateTimeNow();

        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        var reportDate = Utility.GenerateReportDate(paymentModel.PaidDate.Add(currentTime));
        paymentModel.ReportDate = reportDate;

        var bill = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == vm.BillId && x.BillStatus != BillStatusEnum.FullPaid && !x.IsDeleted, d => d.BillingDetails);

        if (bill == null)
            throw new Exception("Bill Info Is Not Correct Of Bill Is Paid Fully...!!");

        paymentModel.BillingId = bill.Id;

        var bookingService = await _iBookingServiceRepository.GetFirstOrDefaultAsync(x => x.Id == bill.BookingId && !x.IsDeleted);
        if (bookingService == null)
            throw new Exception("Booking Service Not Found...!!");

        paymentModel.BookingId = bookingService.Id;

        var paidList = _iBookingPaymentRepository.Get(c => c.BookingId == bookingService.Id && !c.IsDeleted).ToList();
        var alreadyPaidAmount = paidList.Sum(x => x.PaidAmount);

        if (alreadyPaidAmount != bill.PaidAmount)
            throw new Exception("Already Paid Amount Is Not Matched...!!");

        var totalAmount = alreadyPaidAmount + paymentModel.PaidAmount;

        if (totalAmount > bill.NetAmount)
            throw new Exception("Given Amount Is Higher Than Net Amount...!!");

        if (totalAmount == bill.NetAmount)
            bookingService.PaymentStatus = PaymentStatusEnum.FullPayment;
        else
            bookingService.PaymentStatus = PaymentStatusEnum.PartialPayment;

        bill.PaidAmount = totalAmount;

        if (bill.NetAmount == bill.PaidAmount)
            bill.BillStatus = BillStatusEnum.FullPaid;
        else
            bill.BillStatus = BillStatusEnum.PartialPaid;

        #region FoodServicePayment

        List<RsFoodOrder> updateRsFoodOrders = new List<RsFoodOrder>();
        List<RsOrderPayments> rsFoodPayments = new List<RsOrderPayments>();

        var foodService = _iServiceRepository.GetFirstOrDefault(x => x.ServiceCode == HtServiceCode.FoodService);
        if (foodService == null)
            throw new Exception("Food Service Not Found In DB..!!");

        var unpaidFoodOrders = await _iFoodOrderRepository.GetAsync(x => x.BookingId == bill.BookingId && x.PaymentStatus != RsOrderPaymentStatusEnum.FullPayment);

        if (bill.BillingDetails != null && bill.BillingDetails.Count > 0 && bill.BillStatus == BillStatusEnum.FullPaid)
        {
            var billDtlBookingIds = bill.BillingDetails.Select(x => x.BookingRoomId).ToList();
            var billingBookedRooms = await _iBookingRoomRepository.GetAsync(x => billDtlBookingIds.Contains(x.Id) && !x.IsDeleted, r => r.Room);
            var bookingRoomList = billingBookedRooms.Select(x => x.Room).ToList();

            foreach (var billDtl in bill.BillingDetails)
            {
                if (billDtl.ServiceId == foodService.Id)
                {
                    if (unpaidFoodOrders.Count > 0)
                    {
                        var bookingRoom = billingBookedRooms.FirstOrDefault(x => x.Id == billDtl.BookingRoomId);
                        if (bookingRoom == null)
                            throw new Exception("Booking Room Info Not Found..!!");

                        var unpaidRoomFoodOrders = unpaidFoodOrders.Where(x => x.RoomId == bookingRoom.RoomId).ToList();

                        foreach (var unpaidOrder in unpaidRoomFoodOrders)
                        {
                            var paymentExist = _iRsOrderPaymentRepository.GetFirstOrDefault(x => x.OrderId == unpaidOrder.Id && x.BillDtlId == billDtl.Id && !x.IsDeleted);
                            if (paymentExist != null)
                                continue;

                            var foodPaidList = _iOrderPaymentRepository.Get(c => c.OrderId == unpaidOrder.Id && c.PaymentType == RsOrderPaymentTypeEnum.Receive && !c.IsDeleted).ToList();
                            var alreadyFoodPaidAmount = foodPaidList.Sum(x => x.PaidAmount);

                            var dueAmount = unpaidOrder.NetAmount > alreadyFoodPaidAmount ? unpaidOrder.NetAmount - alreadyFoodPaidAmount : 0;

                            var rsPaymentModel = new RsOrderPayments();
                            rsPaymentModel.OrderId = unpaidOrder.Id;
                            rsPaymentModel.PaidDate = paymentModel.PaidDate;
                            rsPaymentModel.PayMode = vm.PayMode;
                            //rsPaymentModel.PaidAmount = unpaidOrder.NetAmount;
                            rsPaymentModel.PaidAmount = dueAmount;
                            rsPaymentModel.ActionById = CurrentUserId;
                            rsPaymentModel.ActionDate = Utility.GetBdDateTimeNow();
                            rsPaymentModel.BillDtlId = billDtl.Id;
                            rsPaymentModel.Remarks = $"Amount Is Paid From Hotel Bill No. {bill.BillNumber}";


                            var rsReportDate = Utility.GenerateReportDate(rsPaymentModel.PaidDate.Add(currentTime));
                            rsPaymentModel.ReportDate = reportDate;

                            if (rsPaymentModel.PaidAmount > 0)
                            {
                                unpaidOrder.PaymentStatus = RsOrderPaymentStatusEnum.FullPayment;

                                rsFoodPayments.Add(rsPaymentModel);
                                updateRsFoodOrders.Add(unpaidOrder);
                            }

                        }
                    }

                }
            }
        }

        #endregion

        ////standard accounting system
        //var paymentVoucher = await GetPaymentVoucher(paymentModel, bookingService.BookingNo, bill.BillNumber, alreadyPaidAmount: alreadyPaidAmount);
        //quick voucher
        //var paymentVoucher = await GetPaymentQuickVoucher(paymentModel, bookingService.BookingNo, bill.BillNumber, alreadyPaidAmount: alreadyPaidAmount);
        //if (paymentVoucher == null)
        //    throw new Exception("Somthing Went Wrong Creating Voucher..!!");

        var billPaymentExist = paidList.Any(x => x.BillingId == bill.Id);

        AccTranMst paymentVoucher = null;

        var novDate = new DateTime(2024, 11, 29);
        if (paymentModel.PaidDate.Date > novDate.Date && !billPaymentExist)
        {
            //paymentVoucher = await GetPaymentQuickVoucher(paymentModel, bookingService.BookingNo, bill.BillNumber, alreadyPaidAmount: alreadyPaidAmount);
            //if (paymentVoucher == null)
            //    throw new Exception("Somthing Went Wrong Creating Voucher..!!");
            var dueAmount = bill.BillStatus == BillStatusEnum.PartialPaid ? (bill.NetAmount - bill.PaidAmount) : 0;

            paymentVoucher = await GetBillVoucher(bill.Id, refundAmount: alreadyPaidAmount, dueAmount: dueAmount);
            //updated by tawkir: 26/12/2025
            //paymentVoucher = await GetBillVoucherUpdate(bill.Id, refundAmount: alreadyPaidAmount, dueAmount: dueAmount);
            if (paymentVoucher == null)
                throw new Exception("Somthing Went Wrong Creating Voucher..!!");
            paymentVoucher.VcDate = paymentModel.PaidDate;
        }
        else
        {
            paymentVoucher = await GetDueBillReceiveVoucher(paymentModel);
            if (paymentVoucher == null)
                throw new Exception("Somthing Went Wrong Creating Voucher..!!");
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _iBookingPaymentRepository.AddAsync(paymentModel);
        await _iUnitOfWork.CompleteAsync();

        await _iRepository.UpdateAsync(bill);

        await _iBookingServiceRepository.UpdateAsync(bookingService);

        if (paymentVoucher != null)
        {
            paymentVoucher.PaymentId = paymentModel.Id;
            await _iAccTranMstRepository.AddAsync(paymentVoucher);
        }

        if (updateRsFoodOrders.Count > 0 && rsFoodPayments.Count > 0)
        {
            _iFoodOrderRepository.UpdateRange(updateRsFoodOrders);
            _iRsOrderPaymentRepository.AddRange(rsFoodPayments);
        }

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) return false;
        ts.Complete();
        return true;
    }

    #endregion

    #region FullPaidBillClose

    public async Task<bool> FullPaymentBillClose(long billId)
    {
        if (billId > 0 is false)
            throw new Exception("Information is not correct..!!");

        var bill = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == billId && x.BillStatus != BillStatusEnum.FullPaid && !x.IsDeleted, d => d.BillingDetails);

        if (bill == null)
            throw new Exception("Bill Info Is Not Correct Of Bill Is Paid Fully...!!");

        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        var reportDate = Utility.GenerateReportDate(bill.BillDate.Add(currentTime));

        var bookingService = await _iBookingServiceRepository.GetFirstOrDefaultAsync(x => x.Id == bill.BookingId && !x.IsDeleted);
        if (bookingService == null)
            throw new Exception("Booking Service Not Found...!!");

        var paidList = _iBookingPaymentRepository.Get(c => c.BookingId == bookingService.Id && !c.IsDeleted).ToList();
        var alreadyPaidAmount = paidList.Sum(x => x.PaidAmount);

        var lastPayment = paidList.OrderByDescending(c => c.PaidDate).FirstOrDefault();

        if (alreadyPaidAmount != bill.PaidAmount)
            throw new Exception("Already Paid Amount Is Not Matched...!!");

        var totalAmount = alreadyPaidAmount;

        if (totalAmount > bill.NetAmount)
            throw new Exception("Given Amount Is Higher Than Net Amount...!!");

        if (totalAmount == bill.NetAmount)
            bookingService.PaymentStatus = PaymentStatusEnum.FullPayment;
        else
            bookingService.PaymentStatus = PaymentStatusEnum.PartialPayment;

        bill.PaidAmount = totalAmount;

        if (bill.NetAmount == bill.PaidAmount)
            bill.BillStatus = BillStatusEnum.FullPaid;
        else
            bill.BillStatus = BillStatusEnum.PartialPaid;

        #region FoodServicePayment

        List<RsFoodOrder> updateRsFoodOrders = new List<RsFoodOrder>();
        List<RsOrderPayments> rsFoodPayments = new List<RsOrderPayments>();

        var foodService = _iServiceRepository.GetFirstOrDefault(x => x.ServiceCode == HtServiceCode.FoodService);
        if (foodService == null)
            throw new Exception("Food Service Not Found In DB..!!");

        var unpaidFoodOrders = await _iFoodOrderRepository.GetAsync(x => x.BookingId == bill.BookingId && x.PaymentStatus != RsOrderPaymentStatusEnum.FullPayment);

        if (bill.BillingDetails != null && bill.BillingDetails.Count > 0 && bill.BillStatus == BillStatusEnum.FullPaid)
        {
            var billDtlBookingIds = bill.BillingDetails.Select(x => x.BookingRoomId).ToList();
            var billingBookedRooms = await _iBookingRoomRepository.GetAsync(x => billDtlBookingIds.Contains(x.Id) && !x.IsDeleted, r => r.Room);
            var bookingRoomList = billingBookedRooms.Select(x => x.Room).ToList();

            foreach (var billDtl in bill.BillingDetails)
            {
                if (billDtl.ServiceId == foodService.Id)
                {
                    if (unpaidFoodOrders.Count > 0)
                    {
                        var bookingRoom = billingBookedRooms.FirstOrDefault(x => x.Id == billDtl.BookingRoomId);
                        if (bookingRoom == null)
                            throw new Exception("Booking Room Info Not Found..!!");

                        var unpaidRoomFoodOrders = unpaidFoodOrders.Where(x => x.RoomId == bookingRoom.RoomId).ToList();

                        foreach (var unpaidOrder in unpaidRoomFoodOrders)
                        {
                            var paymentExist = _iRsOrderPaymentRepository.GetFirstOrDefault(x => x.OrderId == unpaidOrder.Id && x.BillDtlId == billDtl.Id && !x.IsDeleted);
                            if (paymentExist != null)
                                continue;

                            var foodPaidList = _iOrderPaymentRepository.Get(c => c.OrderId == unpaidOrder.Id && c.PaymentType == RsOrderPaymentTypeEnum.Receive && !c.IsDeleted).ToList();
                            var alreadyFoodPaidAmount = foodPaidList.Sum(x => x.PaidAmount);

                            var dueAmount = unpaidOrder.NetAmount > alreadyFoodPaidAmount ? unpaidOrder.NetAmount - alreadyFoodPaidAmount : 0;

                            var rsPaymentModel = new RsOrderPayments();
                            rsPaymentModel.OrderId = unpaidOrder.Id;
                            rsPaymentModel.PaidDate = bill.BillDate;
                            rsPaymentModel.PayMode = lastPayment.PayMode;
                            //rsPaymentModel.PaidAmount = unpaidOrder.NetAmount;
                            rsPaymentModel.PaidAmount = dueAmount;
                            rsPaymentModel.ActionById = CurrentUserId;
                            rsPaymentModel.ActionDate = Utility.GetBdDateTimeNow();
                            rsPaymentModel.BillDtlId = billDtl.Id;
                            rsPaymentModel.Remarks = $"Amount Is Paid From Hotel Bill No. {bill.BillNumber}";


                            var rsReportDate = Utility.GenerateReportDate(rsPaymentModel.PaidDate.Add(currentTime));
                            rsPaymentModel.ReportDate = reportDate;

                            if (rsPaymentModel.PaidAmount > 0)
                            {
                                unpaidOrder.PaymentStatus = RsOrderPaymentStatusEnum.FullPayment;

                                rsFoodPayments.Add(rsPaymentModel);
                                updateRsFoodOrders.Add(unpaidOrder);
                            }

                        }
                    }

                }
            }
        }

        #endregion

        var billPaymentExist = paidList.Any(x => x.BillingId == bill.Id);

        AccTranMst paymentVoucher = null;

        var novDate = new DateTime(2024, 11, 29);
        if (lastPayment.PaidDate.Date > novDate.Date && !billPaymentExist)
        {
            var dueAmount = bill.BillStatus == BillStatusEnum.PartialPaid ? (bill.NetAmount - bill.PaidAmount) : 0;

            paymentVoucher = await GetBillVoucher(bill.Id, refundAmount: alreadyPaidAmount, dueAmount: dueAmount);

            //updated by tawkir: 26/12/2025
            //paymentVoucher = await GetBillVoucherUpdate(bill.Id, refundAmount: alreadyPaidAmount, dueAmount: dueAmount);
            if (paymentVoucher == null)
                throw new Exception("Somthing Went Wrong Creating Voucher..!!");
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _iRepository.UpdateAsync(bill);

        await _iBookingServiceRepository.UpdateAsync(bookingService);

        if (paymentVoucher != null)
        {
            paymentVoucher.PaymentId = lastPayment.Id;
            await _iAccTranMstRepository.AddAsync(paymentVoucher);
        }

        if (updateRsFoodOrders.Count > 0 && rsFoodPayments.Count > 0)
        {
            _iFoodOrderRepository.UpdateRange(updateRsFoodOrders);
            _iRsOrderPaymentRepository.AddRange(rsFoodPayments);
        }

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) return false;
        ts.Complete();
        return true;
    }

    #endregion

    #region BillHtml

    public async Task<string> GetBillByIdAsyncHtml(long id)
    {
        var data = await GetBillByIdAsync(id);

        var fullHtml = "";
        fullHtml += "<div><p><u>Bill To :</u></p></div>";
        fullHtml += "<div style='padding-top:5px'>";
        fullHtml += $@"<table class='master-table'>
                                <tbody>
                                    <tr>
                                        <td style='width:12%;'><b>Guest Name</b></td>
                                        <td style='width:38%'>: {data.BookingGuestName}</td>
                                        <td style='width:12%;'><b>Bill No</b></td>
                                        <td style='width:38%'>: {data.BillNumber}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Guest Mobile</b></td>
                                        <td style='width:38%'>: {data.BookingGuestMobile}</td>
                                        <td style='width:12%;'><b>Bill Date</b></td>
                                        <td style='width:38%'>: {data.BillDate.ToString("dd-MMM-yyyy")}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Address</b></td>
                                        <td style='width:38%'>: {data.BookingGuestAddress}</td>
                                        <td style='width:12%;'><b>Booking No</b></td>
                                        <td style='width:38%'>: {data.BookingNo}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Note</b></td>
                                        <td style='width:38%;'>: {data.Remarks}</td>
                                        <td style='width:12%;'><b>Booking Date</b></td>
                                        <td style='width:38%'>: {data.BookingDate.ToString("dd-MMM-yyyy")}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Bill Status</b></td>
                                        <td style='width:38%;'>: {(data.BillStatus == BillStatusEnum.FullPaid ? "PAID" : "DUE")}</td>
                                        <td style='width:12%;'><b>Bill By</b></td>
                                        <td style='width:38%'>: {data.BillByName}</td>
                                    </tr>
                                </tbody>
                            </table>";

        fullHtml += "</div>";

        //Order Item
        fullHtml += "<div style='padding-top:15px;'><p>Room Info :</p></div>";
        fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center;margin-top:5px;font-size:11px'>";

        fullHtml += "<thead>";
        fullHtml += "<tr>";

        if (data.BookingType == BookingType.Hall)
        {
            fullHtml += "<th style='width:15%'>Hall</th>";
            fullHtml += "<th style='width:15%'>Service Name</th>";
            fullHtml += "<th style='width:10%'>Check In</th>";
            fullHtml += "<th style='width:10%'>Check Out</th>";
            fullHtml += "<th style='width:8%'>No Of Shift</th>";
            fullHtml += "<th style='width:15%'>Rate</th>";
            fullHtml += "<th style='width:10%'>Total</th>";
        }
        else
        {
            fullHtml += "<th style='width:9%'>Room No</th>";
            fullHtml += "<th style='width:15%'>Service Name</th>";
            fullHtml += "<th style='width:15%'>Check In</th>";
            fullHtml += "<th style='width:15%'>Check Out</th>";
            fullHtml += "<th style='width:8%'>No Of Days</th>";
            fullHtml += "<th style='width:15%'>Rate</th>";
            fullHtml += "<th style='width:8%'>Total</th>";
        }

        fullHtml += "</tr>";

        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        if (data.BillingDetails.Count > 0)
        {
            foreach (var (item, i) in data.BillingDetails.GetItemWithIndex())
            {
                if (item.ServiceCode == HtServiceCode.RoomRent)
                {
                    fullHtml += "<tr>";

                    fullHtml += $@"<td class='text-center'>{item.BookingRoomNo} <br/>";

                    if (item.BookingRoomExtraBed > 0)
                    {
                        fullHtml += $@"<p class='f-9'>Extra Bed: {item.BookingRoomExtraBed}</p>";
                    }

                    fullHtml += "</td>";

                    fullHtml += $@"<td class='text-start'>{item.ServiceName}</td>";
                    fullHtml += $@"<td class='text-center'>{Utility.ConvertDateTimeToStr((DateTime)item.BookingRoomCheckInTime)}</td>";
                    fullHtml += $@"<td class='text-center'>{Utility.ConvertDateTimeToStr((DateTime)item.BookingRoomCheckOutTime)}</td>";
                    fullHtml += $@"<td class='text-center'>{item.Days}</td>";
                    fullHtml += $@"<td class='text-start f-9'>
                                    <b>Rent:</b> {item.Rate} <br/>
                                    <b>Service Charge:</b> {item.BookingRoomServiceCharge} <br/>";

                    if (item.BookingRoomExtraBed > 0)
                    {
                        fullHtml += $@"<b>Extra Bed Charge:</b> {item.BookingRoomExtraBedCharge} <br/>";
                    }

                    fullHtml += "</td>";
                    fullHtml += $@"<td class='text-end'>{item.Amount + item.ExtraBedCharge + item.ServiceCharge}</td>";

                    fullHtml += "</tr>";
                }
                else if (item.ServiceCode == HtServiceCode.HallRent)
                {
                    fullHtml += "<tr>";

                    fullHtml += $@"<td class='text-center'>{item.BookingHallName} <br/>";
                    fullHtml += $@"<p class='f-9'>Date: {item.BookingHallBookDate?.ToString("dd/MM/yyyy")}</p>";
                    fullHtml += $@"<p class='f-9'>Shift: {item.HallShiftText}</p>";
                    fullHtml += "</td>";

                    fullHtml += $@"<td class='text-start'>{item.ServiceName}</td>";
                    fullHtml += $@"<td class='text-center'></td>";
                    fullHtml += $@"<td class='text-center'></td>";
                    fullHtml += $@"<td class='text-center'>{item.Days}</td>";
                    fullHtml += $@"<td class='text-start f-9'>
                                    <b>Rent:</b> {item.Rate} <br/>
                                    <b>Service Charge:</b> {item.BookingRoomServiceCharge} <br/>";
                    fullHtml += "</td>";
                    fullHtml += $@"<td class='text-end'>{item.Amount + item.ServiceCharge}</td>";

                    fullHtml += "</tr>";
                }
                else if (item.ServiceCode == HtServiceCode.FoodService)
                {
                    fullHtml += "<tr>";

                    fullHtml += $@"<td class='text-center'>{item.BookingRoomNo} <br/>";

                    fullHtml += "</td>";

                    fullHtml += $@"<td class='text-start'>{item.ServiceName}</td>";
                    fullHtml += $@"<td class='text-center'></td>";
                    fullHtml += $@"<td class='text-center'></td>";
                    fullHtml += $@"<td class='text-center'></td>";
                    fullHtml += $@"<td class='text-start f-9'>";
                    fullHtml += "</td>";
                    fullHtml += $@"<td class='text-end'>{item.Amount + item.ServiceCharge}</td>";

                    fullHtml += "</tr>";
                }
                else
                {
                    fullHtml += "<tr>";

                    fullHtml += $@"<td class='text-center'> N/A </td>";

                    fullHtml += $@"<td class='text-start'>{item.ServiceName}</td>";
                    fullHtml += $@"<td class='text-center'></td>";
                    fullHtml += $@"<td class='text-center'></td>";
                    fullHtml += $@"<td class='text-center'></td>";
                    fullHtml += $@"<td class='text-start f-9'>
                                    <b>Rate:</b> {item.Rate} x {item.Quantity} = {item.Amount} <br/>";
                    fullHtml += "</td>";
                    fullHtml += $@"<td class='text-end'>{item.Amount}</td>";

                    fullHtml += "</tr>";
                }

            }
        }

        var subTotal = data.BillingDetails.Sum(x => x.Amount + x.ExtraBedCharge + x.ServiceCharge);

        fullHtml += "<tr>";
        fullHtml += $@"<td colspan='6' class='text-end'><b>Grand Total</b></td>";
        fullHtml += $@"<td class='text-end'><b>{subTotal.ToString("N2")}</b></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $@"<td colspan='6' class='text-end'><b>VAT</b></td>";
        fullHtml += $@"<td class='text-end'><b>{data.Vat.ToString("N2")}</b></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $@"<td colspan='6' class='text-end'><b>Tax</b></td>";
        fullHtml += $@"<td class='text-end'><b>{data.Tax.ToString("N2")}</b></td>";
        fullHtml += "</tr>";

        var totalDiscount = data.BillingDetails.Sum(x => x.Discount) + data.Discount;
        double discountPercent = AppUtility.CalculatePercentage(totalDiscount, subTotal);

        string discountPercentText = totalDiscount > 0 ? $"({discountPercent.ToString("F2")} %)" : string.Empty;

        fullHtml += "<tr>";
        fullHtml += $@"<td colspan='6' class='text-end'>{discountPercentText} <b>Discount</b></td>";
        fullHtml += $@"<td class='text-end'><b>{totalDiscount.ToString("N2")}</b></td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $@"<td colspan='6' class='text-end'><b>Net Payable Amount</b></td>";
        fullHtml += $@"<td class='text-end'><b>{data.NetAmount.ToString("N2")}</b></td>";
        fullHtml += "</tr>";

        string mrListText = !string.IsNullOrEmpty(data.MrList) ? $"({data.MrList})" : string.Empty;

        fullHtml += "<tr>";
        fullHtml += $@"<td colspan='6' class='text-end'>{mrListText} <b>Deposit Paid</b></td>";
        fullHtml += $@"<td class='text-end'><b>{data.AdvanceAmount.ToString("N2")}</b></td>";
        fullHtml += "</tr>";

        var dueAmount = data.NetAmount > data.PaidAmount ? (data.NetAmount - data.PaidAmount) : 0;
        var netPayableAmount = data.NetAmount > data.AdvanceAmount ? (data.NetAmount - data.AdvanceAmount) : 0;

        //fullHtml += "<tr>";
        //fullHtml += $@"<td colspan='6' class='text-end'><b>Net Pay Amount</b></td>";
        //fullHtml += $@"<td class='text-end'><b>{netPayableAmount.ToString("N2")}</b></td>";
        //fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $@"<td colspan='6' class='text-end'><b>Balance to be paid in full</b></td>";
        fullHtml += $@"<td class='text-end'><b>{netPayableAmount.ToString("N2")}</b></td>";
        fullHtml += "</tr>";

        //fullHtml += "<tr>";
        //fullHtml += $@"<td colspan='6' class='text-end'><b>Total Paid Amount</b></td>";
        //fullHtml += $@"<td class='text-end'><b>{data.PaidAmount.ToString("N2")}</b></td>";
        //fullHtml += "</tr>";

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        return fullHtml;
    }

    #endregion

    #region GetPaymentVoucher

    private async Task<AccTranMst> GetPaymentVoucher(HtBookingPayment payment, string bookingNo, string billNo, double alreadyPaidAmount = 0)
    {
        var model = new AccTranMst();

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

        model.VcDate = payment.PaidDate;
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.FinYearId = (await _iSetFincYearService.GetFincYearByDate(model.VcDate)).Id;
        model.Narration = $"Room rent receive. Booking No:{bookingNo}, Bill No:{billNo}";
        model.ActionById = CurrentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();

        var accTranList = new List<AccTranDtl>();

        if (payment != null)
        {
            var modelDtl = new AccTranDtl();

            var ladger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.RoomRentReceive);
            if (ladger == null)
                throw new Exception("No Ledger Found Against Room Rent Receive..!!");

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
                var cashLadger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.CashInHand);
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

            if (alreadyPaidAmount > 0)
            {
                var advanceLadger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.RoomAdvanceRentReceive);
                var modelDtl2 = new AccTranDtl();
                modelDtl2.AmountDr = alreadyPaidAmount;
                modelDtl2.AmountCr = alreadyPaidAmount;
                modelDtl2.LedgerDrId = advanceLadger.Id;
                modelDtl2.LedgerCrId = ladger.Id;
                modelDtl2.ActionById = CurrentUserId;
                modelDtl2.ActionDate = Utility.GetBdDateTimeNow();
                accTranList.Add(modelDtl2);
            }


            model.AccTranDtls = accTranList;
        }

        model.TotalAmount = model.AccTranDtls.Sum(c => c.AmountDr);

        return model;
    }

    #endregion

    #region GetPaymentQuickVoucher

    private async Task<AccTranMst> GetPaymentQuickVoucher(HtBookingPayment payment, string bookingNo, string billNo,
        double alreadyPaidAmount = 0)
    {
        var model = new AccTranMst();
        model.VcDate = payment.PaidDate;
        model.VcType = VoucherType.JournalVoucher;
        model.SubVacType = VoucherType.JournalVoucher;
        model.VcNo = await _iAutoCodeRepository.GetVoucherAutoNo(VoucherTypeCode.JournalVoucher, model.VcDate);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.FinYearId = (await _iSetFincYearService.GetFincYearByDate(model.VcDate)).Id;
        model.Narration = $"Room rent receive. Booking No:{bookingNo}, Bill No:{billNo}";
        model.ActionById = CurrentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();
        model.IsAuto = true;

        var mishukLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.HotelMisuk && !c.IsDeleted);
        model.AccAccountId = mishukLedger.Id;

        var accTranList = new List<AccTranDtl>();

        if (payment != null)
        {
            var billModel = await GetBillByIdAsync(Convert.ToInt64(payment.BillingId));
            if (billModel != null)
            {
                var groupedList = billModel.BillingDetails
                .GroupBy(s => new { s.LedgerCode, s.ServiceCode, s.ServiceName, s.ServiceId })
                .Select(g => new BillingDetailVm
                {
                    ServiceCode = g.Key.ServiceCode,
                    ServiceName = g.Key.ServiceName,
                    LedgerCode = g.Key.LedgerCode,
                    ServiceId = g.Key.ServiceId,
                    Amount = g.Sum(x => x.Amount),
                    NetAmount = g.Sum(x => x.NetAmount),
                    ExtraBedCharge = g.Sum(x => x.ExtraBedCharge),
                    Discount = g.Sum(x => x.Discount)
                })
                .ToList();

                var roomRentReceiveLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.RoomRentReceive && !c.IsDeleted);

                foreach (var item in groupedList)
                {
                    var voucherDtl = new AccTranDtl();
                    //var voucherCrLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.HotelMisuk && !c.IsDeleted);
                    string drLedgerCode = (!string.IsNullOrEmpty(item.LedgerCode)) ? item.LedgerCode : AccLadgerCode.RoomRentReceive;
                    var voucherDrLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == drLedgerCode && !c.IsDeleted);

                    //voucherDtl.AmountDr = (item.LedgerCode == AccLadgerCode.RoomRentReceive)? item.NetAmount - item.ExtraBedCharge - item.Discount - billModel.MasterDiscount: item.NetAmount  - item.Discount ;
                    voucherDtl.AmountDr = (item.LedgerCode == AccLadgerCode.RoomRentReceive) ? item.NetAmount - alreadyPaidAmount - item.ExtraBedCharge - billModel.Discount : item.NetAmount;
                    voucherDtl.AmountCr = voucherDtl.AmountDr;
                    voucherDtl.LedgerDrId = mishukLedger.Id;
                    voucherDtl.LedgerCrId = voucherDrLedger.Id;
                    voucherDtl.ActionById = CurrentUserId;
                    voucherDtl.ActionDate = Utility.GetBdDateTimeNow();
                    accTranList.Add(voucherDtl);


                    //extra bed charge
                    if (item.ExtraBedCharge > 0)
                    {
                        var extraBedDtl = new AccTranDtl();
                        var extraBedDtlCrLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.HotelMisuk && !c.IsDeleted);
                        var extraBedDtlDrLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.ExtraBedChargeLedger && !c.IsDeleted);

                        extraBedDtl.AmountDr = item.ExtraBedCharge;
                        extraBedDtl.AmountCr = item.ExtraBedCharge;
                        extraBedDtl.LedgerDrId = extraBedDtlCrLedger.Id;
                        extraBedDtl.LedgerCrId = extraBedDtlDrLedger.Id;
                        extraBedDtl.ActionById = CurrentUserId;
                        extraBedDtl.ActionDate = Utility.GetBdDateTimeNow();
                        accTranList.Add(extraBedDtl);
                    }
                }
                //bill master discount
                if (billModel.Discount > 0)
                {
                    var extraBedDtl = new AccTranDtl();
                    var extraBedDtlCrLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.DiscountBillPaidLedger && !c.IsDeleted);
                    //var extraBedDtlDrLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.RoomRentReceive && !c.IsDeleted);

                    extraBedDtl.AmountDr = billModel.Discount;
                    extraBedDtl.AmountCr = billModel.Discount;
                    extraBedDtl.LedgerDrId = extraBedDtlCrLedger.Id;
                    extraBedDtl.LedgerCrId = roomRentReceiveLedger.Id;
                    extraBedDtl.ActionById = CurrentUserId;
                    extraBedDtl.ActionDate = Utility.GetBdDateTimeNow();
                    accTranList.Add(extraBedDtl);
                }
                //already paid 
                if (alreadyPaidAmount > 0)
                {
                    var advanceLadger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.RoomAdvanceRentReceive);

                    var modelDtl2 = new AccTranDtl();
                    modelDtl2.AmountDr = alreadyPaidAmount;
                    modelDtl2.AmountCr = alreadyPaidAmount;
                    modelDtl2.LedgerDrId = advanceLadger.Id;
                    modelDtl2.LedgerCrId = roomRentReceiveLedger.Id;
                    modelDtl2.ActionById = CurrentUserId;
                    modelDtl2.ActionDate = Utility.GetBdDateTimeNow();
                    accTranList.Add(modelDtl2);
                }


                model.AccTranDtls = accTranList;
            }


            #region Single Voucher

            //var modelDtl = new AccTranDtl();

            //var crLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.HotelMisuk);
            //var drLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.RoomRentReceive);


            //if (crLedger == null)
            //    throw new Exception("No Ledger Found Against Room Rent Receive..!!");
            //if (drLedger == null)
            //    throw new Exception("No Bank Ledger Found Against Hotel Mishuk..!!");


            //modelDtl.AmountDr = payment.PaidAmount;
            //modelDtl.AmountCr = payment.PaidAmount;
            //modelDtl.LedgerDrId = mishukLedger.Id; 
            //modelDtl.LedgerCrId = drLedger.Id;
            //modelDtl.ActionById = CurrentUserId;
            //modelDtl.ActionDate = Utility.GetBdDateTimeNow();
            //accTranList.Add(modelDtl);

            //if (alreadyPaidAmount > 0)
            //{
            //    model.Narration += $" , Advance Refund : Tk. {alreadyPaidAmount}";
            //}
            //model.AccTranDtls = accTranList;

            #endregion
        }

        model.TotalAmount = model.AccTranDtls.Sum(c => c.AmountDr);

        return model;
    }

    #endregion

    #region BillDetailAdd

    public async Task<bool> BillDetailAddAsync(SaveBillingDetailVm vm)
    {
        if (vm == null && !(vm.BillId > 0))
            throw new Exception("Billing Information Is Not Correct...!!");

        var bill = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == vm.BillId && !x.IsDeleted);

        if (bill == null)
            throw new Exception("Bill Not Found...!!");

        var detailModel = _iMapper.Map<HtBillingDetail>(vm);

        #region Extra Bed
        var extraBedDetailList = new List<HtBillingDetail>();
        var extraBedService = _iServiceRepository.GetFirstOrDefault(x => x.ServiceCode == HtServiceCode.ExtraBed);

        if ((vm.FromDate != null && vm.ToDate != null))
        {
            if (vm.FromDate == null || vm.ToDate == null)
                throw new Exception("Extra Bed dates are missing...!!");

            var nights = (vm.ToDate.Value - vm.FromDate.Value).Days;

            for (var date = vm.FromDate.Value; date < vm.ToDate.Value; date = date.AddDays(1))
            {
                bool isExist = bill.BillingDetails?.Any(x => x.ServiceId == extraBedService.Id && x.BookingRoomId == vm.BookingRoomId && x.ServiceDate != null && x.ServiceDate.Value.Date == date.Date) ?? false;

                if (isExist)
                    throw new Exception(@$"Extra Bed is Already Added on Date:{date.ToString("dd/MMM/yyyy")}...!!");

                var detail = _iMapper.Map<HtBillingDetail>(vm);
                detail.ActionById = CurrentUserId;
                detail.ActionDate = Utility.GetBdDateTimeNow();

                detail.Amount = vm.Rate * vm.Quantity;
                detail.Discount = 0;
                detail.ServiceCharge = vm.ServiceCharge / nights;
                detail.VAT = vm.VAT / nights;
                detail.NetAmount = vm.NetAmount / nights;
                detail.ServiceDate = date;
                detail.ServiceId = extraBedService.Id;
                extraBedDetailList.Add(detail);
            }

            bill.TotalAmount += extraBedDetailList.Sum(x => x.Amount);
            bill.ServiceCharge += extraBedDetailList.Sum(x => x.ServiceCharge);
            bill.Vat += extraBedDetailList.Sum(x => x.VAT);
            bill.Tax += extraBedDetailList.Sum(x => x.Tax);
            bill.Discount += extraBedDetailList.Sum(x => x.Discount);
            bill.NetAmount += extraBedDetailList.Sum(x => x.NetAmount);

        }
        #endregion
        #region Bill Details
        else
        {
            detailModel.ActionById = CurrentUserId;
            detailModel.ActionDate = Utility.GetBdDateTimeNow();

            detailModel.Amount = vm.Rate * vm.Quantity;
            detailModel.Discount = vm.Discount;
            detailModel.ServiceCharge = vm.ServiceCharge;
            detailModel.VAT = vm.VAT;
            detailModel.NetAmount = vm.NetAmount;
            detailModel.ServiceDate = vm.ServiceDate;
            detailModel.ServiceId = vm.ServiceId;

            // Bill
            bill.TotalAmount += detailModel.Amount;
            bill.ServiceCharge += detailModel.ServiceCharge;
            bill.Vat += detailModel.VAT;
            bill.Tax += detailModel.Tax;
            bill.Discount += detailModel.Discount;
            bill.NetAmount += detailModel.NetAmount;
        }

        #endregion

        //var detailModel = _iMapper.Map<HtBillingDetail>(vm);
        //detailModel.ActionById = CurrentUserId;
        //detailModel.ActionDate = Utility.GetBdDateTimeNow();

        //detailModel.Amount = detailModel.Rate * detailModel.Quantity;
        //detailModel.NetAmount = (detailModel.Amount + detailModel.VAT + detailModel.Tax) - (detailModel.Discount);


        //bill.TotalAmount += detailModel.Amount;
        //bill.Vat += detailModel.VAT;
        //bill.Tax += detailModel.Tax;
        //bill.Discount += detailModel.Discount;
        //bill.NetAmount += detailModel.NetAmount;

        var paidList = _iBookingPaymentRepository.Get(c => c.BookingId == bill.BookingId).ToList();
        var alreadyPaidAmount = paidList.Sum(x => x.PaidAmount);

        if (alreadyPaidAmount != bill.PaidAmount)
            throw new Exception("Already Paid Amount Is Not Matched...!!");

        if (bill.PaidAmount > 0)
        {
            if (bill.NetAmount == bill.PaidAmount)
                bill.BillStatus = BillStatusEnum.FullPaid;
            else
                bill.BillStatus = BillStatusEnum.PartialPaid;
        }
        else
        {
            bill.BillStatus = BillStatusEnum.Fresh;
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (extraBedDetailList.Count > 0)
        {
            await _iBillingDetailRepository.AddRangeAsync(extraBedDetailList);
        }
        else
        {
            await _iBillingDetailRepository.AddAsync(detailModel);
        }

        await _iRepository.UpdateAsync(bill);
        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }

    #endregion

    #region MakeComplimentary

    public async Task<bool> MakeComplimentary(MakeComplimentaryVm vm)
    {
        if (vm == null && !(vm.BillId > 0) && vm.AllService == true)
            throw new Exception("Information is not correct..!!");

        var bill = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == vm.BillId && x.BillStatus != BillStatusEnum.FullPaid && !x.IsDeleted, d => d.BillingDetails);
        if (bill == null)
            throw new Exception("Bill Info Is Not Correct Of Bill Is Paid Fully...!!");

        bill.CmpRemarks = vm.Remarks;

        var bookingService = await _iBookingServiceRepository.GetFirstOrDefaultAsync(x => x.Id == bill.BookingId && !x.IsDeleted);
        if (bookingService == null)
            throw new Exception("Booking Service Not Found...!!");

        //var paidList = _iBookingPaymentRepository.Get(c => c.BookingId == bookingService.Id).ToList();
        //var alreadyPaidAmount = paidList.Sum(x => x.PaidAmount);

        //if (alreadyPaidAmount != bill.PaidAmount)
        //    throw new Exception("Already Paid Amount Is Not Matched...!!");

        double complimentaryAmount = 0;

        if (vm.AllService == true)
        {
            vm.ServiceIds = bill.BillingDetails.DistinctBy(x => x.ServiceId).Select(x => x.ServiceId).ToList();
        }

        var updateBillDetailList = new List<HtBillingDetail>();

        if (vm.ServiceIds.Count > 0)
        {
            foreach (var serviceId in vm.ServiceIds)
            {
                var service = _iServiceRepository.GetFirstOrDefault(x => x.Id == serviceId && !x.IsDeleted);
                if (service == null)
                    throw new Exception("Service Not Found...!!");

                var bookingServiceList = bill.BillingDetails.Where(x => x.ServiceId == service.Id && !x.IsDeleted).ToList();

                foreach (var billDtl in bookingServiceList)
                {
                    billDtl.IsComplimentary = true;

                    updateBillDetailList.Add(billDtl);
                }

            }
        }

        complimentaryAmount = updateBillDetailList.Where(x => x.IsComplimentary == true).Sum(x => x.NetAmount);

        if (complimentaryAmount == bill.NetAmount)
            bookingService.PaymentStatus = PaymentStatusEnum.FullPayment;
        else
            bookingService.PaymentStatus = PaymentStatusEnum.PartialPayment;

        if (complimentaryAmount == bill.NetAmount)
            bill.BillStatus = BillStatusEnum.FullPaid;
        else
            bill.BillStatus = BillStatusEnum.PartialPaid;


        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _iRepository.UpdateAsync(bill);

        await _iBookingServiceRepository.UpdateAsync(bookingService);

        if (updateBillDetailList.Count > 0)
        {
            await _iBillingDetailRepository.UpdateRangeAsync(updateBillDetailList);
        }

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) return false;
        ts.Complete();
        return true;
    }

    #endregion

    #region BillDetailNew
    private List<DateTime> GetDateList(DateTime startDate, DateTime endDate)
    {
        List<DateTime> dateList = new List<DateTime>();

        if (startDate <= endDate)
        {
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                dateList.Add(date);
            }
        }
        return dateList;
    }
    public async Task<string> GetBillDetailHtmlById(long id)
    {
        var model = await GetBillByIdAsync(id);
        string fullHtml = await _iRepository.GetBillDetailHtmlById(model);
        return fullHtml;
    }
    #endregion

    #region DiscountUpdate

    public async Task<bool> DiscountUpdateAsync(BillDiscountUpdateDto dto)
    {
        if (dto == null || !(dto.BillId > 0))
            throw new Exception("Billing Information Is Not Correct...!!");

        var bill = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == dto.BillId && !x.IsDeleted, d => d.BillingDetails);
        var booking = _iBookingServiceRepository.GetFirstOrDefault(x => x.Id == bill.BookingId && !x.IsDeleted);

        if (bill == null)
            throw new Exception("Bill Not Found...!!");

        if (booking == null)
            throw new Exception("Booking Not Found...!!");

        if (bill.BillStatus == BillStatusEnum.FullPaid)
            throw new Exception("Bill Already Full Paid...!!");

        if (booking.PaymentStatus == PaymentStatusEnum.FullPayment)
            throw new Exception("Room Rent Already Full Paid...!!");

        bill.SpecialDiscount = dto.Discount;
        bill.NetAmount = (bill.TotalAmount + bill.Vat + bill.Tax) - (bill.Discount + bill.SpecialDiscount);

        //booking.Discount = dto.Discount;
        //booking.NetRent = (booking.Rent + booking.Vat + booking.Tax + booking.ServiceCharge) - (dto.Discount);

        if (bill.PaidAmount > bill.NetAmount)
            throw new Exception("Pay Amount Is Higher Than Payable Amount");

        if (bill.PaidAmount == bill.NetAmount)
        {
            bill.BillStatus = BillStatusEnum.FullPaid;
            booking.PaymentStatus = PaymentStatusEnum.FullPayment;
        }
        else if (bill.PaidAmount < bill.NetAmount && bill.PaidAmount > 0)
        {
            bill.BillStatus = BillStatusEnum.PartialPaid;
            booking.PaymentStatus = PaymentStatusEnum.PartialPayment;
        }
        else
        {
            bill.BillStatus = BillStatusEnum.Fresh;
            booking.PaymentStatus = PaymentStatusEnum.Pending;
        }

        #region FoodServicePayment

        List<RsFoodOrder> updateRsFoodOrders = new List<RsFoodOrder>();
        List<RsOrderPayments> rsFoodPayments = new List<RsOrderPayments>();

        var foodService = _iServiceRepository.GetFirstOrDefault(x => x.ServiceCode == HtServiceCode.FoodService);
        if (foodService == null)
            throw new Exception("Food Service Not Found In DB..!!");

        var unpaidFoodOrders = await _iFoodOrderRepository.GetAsync(x => x.BookingId == bill.BookingId && x.PaymentStatus != RsOrderPaymentStatusEnum.FullPayment);

        if (bill.BillingDetails != null && bill.BillingDetails.Count > 0 && bill.BillStatus == BillStatusEnum.FullPaid)
        {
            var billDtlBookingIds = bill.BillingDetails.Select(x => x.BookingRoomId).ToList();
            var billingBookedRooms = await _iBookingRoomRepository.GetAsync(x => billDtlBookingIds.Contains(x.Id) && !x.IsDeleted, r => r.Room);
            var bookingRoomList = billingBookedRooms.Select(x => x.Room).ToList();

            foreach (var billDtl in bill.BillingDetails)
            {
                if (billDtl.ServiceId == foodService.Id)
                {
                    if (unpaidFoodOrders.Count > 0)
                    {
                        var bookingRoom = billingBookedRooms.FirstOrDefault(x => x.Id == billDtl.BookingRoomId);
                        if (bookingRoom == null)
                            throw new Exception("Booking Room Info Not Found..!!");

                        var unpaidRoomFoodOrders = unpaidFoodOrders.Where(x => x.RoomId == bookingRoom.RoomId).ToList();

                        foreach (var unpaidOrder in unpaidRoomFoodOrders)
                        {
                            var paymentExist = _iRsOrderPaymentRepository.GetFirstOrDefault(x => x.OrderId == unpaidOrder.Id && x.BillDtlId == billDtl.Id && !x.IsDeleted);
                            if (paymentExist != null)
                                continue;

                            var foodPaidList = _iOrderPaymentRepository.Get(c => c.OrderId == unpaidOrder.Id && c.PaymentType == RsOrderPaymentTypeEnum.Receive && !c.IsDeleted).ToList();
                            var alreadyFoodPaidAmount = foodPaidList.Sum(x => x.PaidAmount);

                            var dueAmount = unpaidOrder.NetAmount > alreadyFoodPaidAmount ? unpaidOrder.NetAmount - alreadyFoodPaidAmount : 0;

                            var rsPaymentModel = new RsOrderPayments();
                            rsPaymentModel.OrderId = unpaidOrder.Id;
                            rsPaymentModel.PaidDate = Utility.GetBdDateTimeNow();
                            rsPaymentModel.PayMode = PayModeEnum.Cash;
                            rsPaymentModel.PaidAmount = unpaidOrder.NetAmount;
                            rsPaymentModel.PaidAmount = dueAmount;
                            rsPaymentModel.ActionById = CurrentUserId;
                            rsPaymentModel.ActionDate = Utility.GetBdDateTimeNow();
                            rsPaymentModel.BillDtlId = billDtl.Id;
                            rsPaymentModel.Remarks = $"Amount Is Paid From Hotel Bill No. {bill.BillNumber}";


                            var rsReportDate = Utility.GenerateReportDate(rsPaymentModel.PaidDate.Add(DateTime.Now.TimeOfDay));
                            rsPaymentModel.ReportDate = rsReportDate;

                            if (rsPaymentModel.PaidAmount > 0)
                            {
                                unpaidOrder.PaymentStatus = RsOrderPaymentStatusEnum.FullPayment;

                                rsFoodPayments.Add(rsPaymentModel);
                                updateRsFoodOrders.Add(unpaidOrder);
                            }

                        }
                    }

                }
            }
        }

        #endregion

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _iRepository.UpdateAsync(bill);

        await _iBookingServiceRepository.UpdateAsync(booking);

        if (updateRsFoodOrders.Count > 0 && rsFoodPayments.Count > 0)
        {
            _iFoodOrderRepository.UpdateRange(updateRsFoodOrders);
            _iRsOrderPaymentRepository.AddRange(rsFoodPayments);
        }
        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }

    #endregion

    #region BillVoucher

    private async Task<AccTranMst> GetBillVoucher(long billId, double refundAmount = 0, double dueAmount = 0)
    {
        var billModel = await GetBillByIdAsync(billId);
        if (billModel == null)
            throw new Exception("Bill information not found..!!");

        var model = new AccTranMst();
        model.VcDate = billModel.BillDate;
        model.VcType = VoucherType.JournalVoucher;
        model.SubVacType = VoucherType.JournalVoucher;
        model.VcNo = await _iAutoCodeRepository.GetVoucherAutoNo(VoucherTypeCode.JournalVoucher, model.VcDate);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.FinYearId = (await _iSetFincYearService.GetFincYearByDate(model.VcDate)).Id;
        model.Narration = $"Room rent receive. Booking No:{billModel.BookingNo}, Bill No:{billModel.BillNumber}";
        model.ActionById = CurrentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();
        model.IsAuto = true;

        var mishukLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.HotelMisuk && !c.IsDeleted);
        model.AccAccountId = mishukLedger.Id;

        var accTranList = new List<AccTranDtl>();

        if (billModel != null)
        {
            var groupedList = billModel.BillingDetails
            .GroupBy(s => new { s.LedgerCode, s.ServiceCode, s.ServiceName, s.ServiceId })
            .Select(g => new BillingDetailVm
            {
                ServiceCode = g.Key.ServiceCode,
                ServiceName = g.Key.ServiceName,
                LedgerCode = g.Key.LedgerCode,
                ServiceId = g.Key.ServiceId,
                Amount = g.Sum(x => x.Amount),
                NetAmount = g.Sum(x => x.NetAmount),
                ExtraBedCharge = g.Sum(x => x.ExtraBedCharge),
                Discount = g.Sum(x => x.Discount)
            })
            .ToList();

            var roomRentReceiveLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.RoomRentReceive && !c.IsDeleted);

            foreach (var item in groupedList)
            {
                var voucherDtl = new AccTranDtl();
                string drLedgerCode = (!string.IsNullOrEmpty(item.LedgerCode)) ? item.LedgerCode : AccLadgerCode.RoomRentReceive;
                var voucherDrLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == drLedgerCode && !c.IsDeleted);

                //voucherDtl.AmountDr = (item.LedgerCode == AccLadgerCode.RoomRentReceive) ? item.NetAmount - refundAmount - item.ExtraBedCharge - billModel.Discount : item.NetAmount;
                // Updated (Discount amount removed) by Rifat
                voucherDtl.AmountDr = (item.LedgerCode == AccLadgerCode.RoomRentReceive)
                    ? item.NetAmount - refundAmount - item.ExtraBedCharge - billModel.SpecialDiscount
                    : item.NetAmount;

                if (dueAmount > 0)
                {
                    //voucherDtl.AmountDr = (item.LedgerCode == AccLadgerCode.RoomRentReceive) ? item.NetAmount - refundAmount - item.ExtraBedCharge - billModel.Discount - dueAmount : item.NetAmount;
                    // Updated (Discount amount removed) by Rifat

                    if (billModel.SpecialDiscount > 0)
                    {
                        voucherDtl.AmountDr = (item.LedgerCode == AccLadgerCode.RoomRentReceive)
                            ? (item.NetAmount - refundAmount - item.ExtraBedCharge - billModel.SpecialDiscount) - dueAmount
                            : item.NetAmount;
                    }
                    else
                    {
                        voucherDtl.AmountDr = (item.LedgerCode == AccLadgerCode.RoomRentReceive || item.LedgerCode == AccLadgerCode.HallRentReceive)
                            ? (item.NetAmount - refundAmount - item.ExtraBedCharge) - dueAmount
                            : item.NetAmount;
                    }
                }

                if (voucherDtl.AmountDr > 0 is false)
                    continue;

                voucherDtl.AmountCr = voucherDtl.AmountDr;
                voucherDtl.LedgerDrId = mishukLedger.Id;
                voucherDtl.LedgerCrId = voucherDrLedger.Id;
                voucherDtl.ActionById = CurrentUserId;
                voucherDtl.ActionDate = Utility.GetBdDateTimeNow();
                accTranList.Add(voucherDtl);


                //extra bed charge
                //if (item.ExtraBedCharge > 0)
                //{
                //    var extraBedDtl = new AccTranDtl();

                //    var extraBedDtlDrLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.ExtraBedChargeLedger && !c.IsDeleted);

                //    extraBedDtl.AmountDr = item.ExtraBedCharge;
                //    extraBedDtl.AmountCr = item.ExtraBedCharge;
                //    extraBedDtl.LedgerDrId = mishukLedger.Id;
                //    extraBedDtl.LedgerCrId = extraBedDtlDrLedger.Id;
                //    extraBedDtl.ActionById = CurrentUserId;
                //    extraBedDtl.ActionDate = Utility.GetBdDateTimeNow();
                //    accTranList.Add(extraBedDtl);
                //}
            }

            //bill master discount
            if (billModel.Discount > 0 || billModel.SpecialDiscount > 0)
            {
                var extraBedDtl = new AccTranDtl();
                var extraBedDtlCrLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.DiscountBillPaidLedger && !c.IsDeleted);
                //var extraBedDtlDrLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.RoomRentReceive && !c.IsDeleted);

                extraBedDtl.AmountDr = (billModel.SpecialDiscount + billModel.Discount);
                extraBedDtl.AmountCr = (billModel.SpecialDiscount + billModel.Discount);
                extraBedDtl.LedgerDrId = extraBedDtlCrLedger.Id;
                extraBedDtl.LedgerCrId = roomRentReceiveLedger.Id;
                extraBedDtl.ActionById = CurrentUserId;
                extraBedDtl.ActionDate = Utility.GetBdDateTimeNow();
                accTranList.Add(extraBedDtl);
            }

            //already paid 
            if (refundAmount > 0)
            {
                var advanceLadger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.RoomAdvanceRentReceive);

                var modelDtl2 = new AccTranDtl();
                modelDtl2.AmountDr = refundAmount;
                modelDtl2.AmountCr = refundAmount;
                modelDtl2.LedgerDrId = advanceLadger.Id;
                modelDtl2.LedgerCrId = roomRentReceiveLedger.Id;
                modelDtl2.ActionById = CurrentUserId;
                modelDtl2.ActionDate = Utility.GetBdDateTimeNow();
                accTranList.Add(modelDtl2);
            }

            //accounts receiveable 
            if (dueAmount > 0)
            {
                var crLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.RoomRentReceive);

                if (billModel.BookingType == BookingType.Hall)
                {
                    crLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.HallRentReceive);
                }

                _iAccLedgerService.CurrentUserId = CurrentUserId;
                var drLedgerId = await _iAccLedgerService.GetGuestLedgerId(billModel.BookingGuestId ?? 0);

                if (crLedger == null)
                    throw new Exception("No Ledger Found Against Room Rent Receive..!!");
                if (!(drLedgerId > 0))
                    throw new Exception("No Debit Ledger Found !!");

                var dueDtl = new AccTranDtl();
                dueDtl.AmountDr = dueAmount;
                dueDtl.AmountCr = dueAmount;
                dueDtl.LedgerDrId = drLedgerId;
                dueDtl.LedgerCrId = crLedger.Id;
                dueDtl.ActionById = CurrentUserId;
                dueDtl.ActionDate = Utility.GetBdDateTimeNow();
                accTranList.Add(dueDtl);
            }

            model.AccTranDtls = accTranList;
        }

        model.TotalAmount = model.AccTranDtls.Sum(c => c.AmountDr);

        return model;
    }

    #endregion

    #region DueBillReceiveVoucher

    private async Task<AccTranMst> GetDueBillReceiveVoucher(HtBookingPayment payment)
    {
        var billModel = await GetBillByIdAsync(Convert.ToInt64(payment.BillingId));
        if (billModel == null)
            throw new Exception("Bill information not found..!!");

        var model = new AccTranMst();
        model.VcDate = payment.PaidDate;
        model.VcType = VoucherType.JournalVoucher;
        model.SubVacType = VoucherType.JournalVoucher;
        model.VcNo = await _iAutoCodeRepository.GetVoucherAutoNo(VoucherTypeCode.JournalVoucher, model.VcDate);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.FinYearId = (await _iSetFincYearService.GetFincYearByDate(model.VcDate)).Id;
        model.Narration = $"Bill Due Receive. Bill No:{billModel.BillNumber}";
        model.ActionById = CurrentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();
        model.IsAuto = true;

        var mishukLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.HotelMisuk && !c.IsDeleted);
        model.AccAccountId = mishukLedger.Id;

        var accTranList = new List<AccTranDtl>();

        if (payment != null)
        {
            var modelDtl = new AccTranDtl();

            //var drLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.CashInHand);
            var drLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.HotelMisuk);

            _iAccLedgerService.CurrentUserId = CurrentUserId;
            var crLedgerId = await _iAccLedgerService.GetGuestLedgerId(billModel.BookingGuestId ?? 0);

            if (crLedgerId > 0 is false)
                throw new Exception("No Ledger Found Against Guest Due..!!");
            if (drLedger == null)
                throw new Exception("No Debit Ledger Found For Hotel Mishuk..!!");

            modelDtl.AmountDr = payment.PaidAmount;
            modelDtl.AmountCr = payment.PaidAmount;
            modelDtl.LedgerDrId = drLedger.Id;
            modelDtl.LedgerCrId = crLedgerId;
            modelDtl.ActionById = CurrentUserId;
            modelDtl.ActionDate = Utility.GetBdDateTimeNow();

            accTranList.Add(modelDtl);

            model.AccTranDtls = accTranList;
        }

        model.TotalAmount = model.AccTranDtls.Sum(c => c.AmountDr);

        return model;
    }

    #endregion

    #region InititalBill

    public async Task<bool> GenerateInitialBill(long bookingId)
    {
        var booking = await _iBookingServiceRepository.GetFirstOrDefaultAsync(x => x.Id == bookingId && !x.IsDeleted);
        if (booking == null)
            throw new Exception("Booking Service Not Found...!!");

        var roomList = await _iBookingRoomRepository.GetAsync(x => x.BookingId == booking.Id && !x.IsDeleted, r => r.Room);
        if (!(roomList.Count > 0))
            throw new Exception("No room found in this booking!!");

        var paidList = await _iBookingPaymentRepository.GetAsync(x => x.BookingId == booking.Id && !x.IsDeleted);
        var paidAmount = paidList.Sum(x => x.PaidAmount);

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

        model.BillingDetails = modelDetails;

        var totalAmount = modelDetails.Sum(x => x.Amount);
        var totalServiceCharge = modelDetails.Sum(x => x.ServiceCharge);
        var totalVat = modelDetails.Sum(x => x.VAT);
        var netAmount = modelDetails.Sum(x => x.NetAmount);

        model.TotalAmount = totalAmount;
        model.Vat = totalVat;
        model.PaidAmount = paidAmount;
        model.NetAmount = netAmount;

        await _iRepository.AddAsync(model);
        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return false; }
        return true;
    }

    #endregion

    #region BillNumber
    private async Task<string> GetBillNumber()
    {
        var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.HtBillings.ToString(), "BillNumber", "INV", 5);
        return data;
    }
    #endregion

    #region Bill Service Remove
    public async Task<bool> BillServiceRemoveAsync(long id)
    {
        if (!(id > 0))
            throw new Exception("Service Information Is Not Correct...!!");

        var service = await _iBillingDetailRepository.GetFirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, b => b.Bill);
        if (service == null)
            throw new Exception("Service Not Found...!!");

        var bill = service.Bill;
        if (bill == null)
            throw new Exception("Bill Not Found...!!");

        bill.TotalAmount -= service.Amount;
        bill.ServiceCharge -= service.ServiceCharge;
        bill.Vat -= service.VAT;
        bill.Tax -= service.Tax;
        bill.Discount -= service.Discount;
        bill.NetAmount -= service.NetAmount;

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        _iBillingDetailRepository.Remove(service);

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }

        ts.Complete();
        return true;
    }
    #endregion

    #region NewBillVoucher

    private async Task<AccTranMst> GetBillVoucherUpdate(long billId, double refundAmount = 0,
    double dueAmount = 0)
    {
        var bill = await GetBillByIdAsync(billId)
            ?? throw new Exception("Bill information not found..!!");

        var voucher = await BuildVoucherHeader(bill);

        // 🔹 Service-wise receive + refund adjustment (NO CASH REFUND)
        BuildServiceReceiveEntries(voucher, bill, refundAmount, dueAmount);

        // 🔹 Bill level discount (revenue reduction)
        AddBillDiscountEntry(voucher, bill);

        // 🔹 Accounts receivable (due)
        await AddDueEntry(voucher, bill, dueAmount);

        // 🔹 Final total
        CalculateTotal(voucher);

        // 🔒 Safety check
        if (voucher.AccTranDtls.Sum(x => x.AmountDr)
            != voucher.AccTranDtls.Sum(x => x.AmountCr))
        {
            throw new Exception("Voucher is not balanced");
        }

        return voucher;
    }


    private async Task<AccTranMst> BuildVoucherHeader(BillingVm bill)
    {
        return new AccTranMst
        {
            VcDate = bill.BillDate,
            VcType = VoucherType.JournalVoucher,
            SubVacType = VoucherType.JournalVoucher,
            VcNo = await _iAutoCodeRepository
                .GetVoucherAutoNo(VoucherTypeCode.JournalVoucher, bill.BillDate),
            CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id,
            FinYearId = (await _iSetFincYearService.GetFincYearByDate(bill.BillDate)).Id,
            Narration = $"Room rent receive. Booking No:{bill.BookingNo}, Bill No:{bill.BillNumber}",
            ActionById = CurrentUserId,
            ActionDate = Utility.GetBdDateTimeNow(),
            IsAuto = true,
            AccAccountId = GetCashLedger().Id,
            AccTranDtls = new List<AccTranDtl>()
        };
    }

    private void BuildServiceReceiveEntries(
    AccTranMst voucher,
    BillingVm bill,
    double refundAmount,
    double dueAmount)
    {
        var groupedServices = bill.BillingDetails
            .GroupBy(x => new { x.LedgerCode })
            .Select(g => new
            {
                LedgerCode = string.IsNullOrEmpty(g.Key.LedgerCode)
                    ? AccLadgerCode.RoomRentReceive
                    : g.Key.LedgerCode,
                NetAmount = g.Sum(x => x.NetAmount),
                ExtraBed = g.Sum(x => x.ExtraBedCharge)
            })
            .ToList();

        double remainingRefund = refundAmount;

        foreach (var item in groupedServices)
        {
            var serviceLedger = GetLedger(item.LedgerCode);

            double serviceReceivable =
                item.NetAmount
                - item.ExtraBed
                - bill.SpecialDiscount
                - bill.Discount;

            if (serviceReceivable <= 0)
                continue;

            // Apply refund proportionally / sequentially
            double adjustedAmount = serviceReceivable;

            if (remainingRefund > 0)
            {
                double refundApplied = Math.Min(serviceReceivable, remainingRefund);
                adjustedAmount -= refundApplied;
                remainingRefund -= refundApplied;

                // 🔁 Reverse service revenue (NO CASH)
                voucher.AccTranDtls.Add(CreateDtl(
                    refundApplied,
                    serviceLedger.Id,
                    GetLedger(AccLadgerCode.RoomAdvanceRentReceive).Id));
            }

            // Normal revenue recognition (if any)
            if (adjustedAmount > 0)
            {
                voucher.AccTranDtls.Add(CreateDtl(
                    adjustedAmount,
                    GetCashLedger().Id,
                    serviceLedger.Id));
            }
        }
    }

    private void AddBillDiscountEntry(AccTranMst voucher, BillingVm bill)
    {
        double discount = bill.Discount + bill.SpecialDiscount;
        if (discount <= 0) return;

        voucher.AccTranDtls.Add(CreateDtl(
            discount,
            GetLedger(AccLadgerCode.DiscountBillPaidLedger).Id,
            GetLedger(AccLadgerCode.RoomRentReceive).Id));
    }

    private async Task AddDueEntry(
    AccTranMst voucher,
    BillingVm bill,
    double dueAmount)
    {
        if (dueAmount <= 0) return;

        var guestLedgerId = await _iAccLedgerService
            .GetGuestLedgerId(bill.BookingGuestId ?? 0);

        voucher.AccTranDtls.Add(CreateDtl(
            dueAmount,
            guestLedgerId,
            GetLedger(AccLadgerCode.RoomRentReceive).Id));
    }

    private AccTranDtl CreateDtl(double amount, long drId, long crId)
    {
        return new AccTranDtl
        {
            AmountDr = amount,
            AmountCr = amount,
            LedgerDrId = drId,
            LedgerCrId = crId,
            ActionById = CurrentUserId,
            ActionDate = Utility.GetBdDateTimeNow()
        };
    }

    private AccLedger GetCashLedger() =>
        GetLedger(AccLadgerCode.HotelMisuk);

    private AccLedger GetLedger(string code) =>
        _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == code && !c.IsDeleted)
        ?? throw new Exception($"Ledger not found: {code}");

    private void CalculateTotal(AccTranMst voucher) =>
        voucher.TotalAmount = voucher.AccTranDtls.Sum(x => x.AmountDr);

    #endregion
}