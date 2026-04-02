using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Entities.HotelManagement;
using Domain.Entities.Restaurant;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Report;
using Domain.ViewModel.Restaurant.FoodOrder;
using Domain.ViewModel.Restaurant.OrderPayment;
using Interface.Repository.Accounts;
using Interface.Repository.Common;
using Interface.Repository.HotelManagement;
using Interface.Repository.Inventory;
using Interface.Repository.Restaurant;
using Interface.Services.Accounts;
using Interface.Services.Admin;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;
using DU = Domain.Utility;

namespace Services.Restaurant;

public class FoodOrderService : BaseService<RsFoodOrder>, IFoodOrderService
{
    #region Config
    private IFoodOrderRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IAutoCodeRepository _iAutoCodeRepository;
    private readonly IFoodItemRepository _iFoodItemRepository;
    private readonly IRsOrderPaymentRepository _iOrderPaymentRepository;
    private readonly IFoodOrderItemRepository _iFoodOrderItemRepository;
    private readonly IBookingRoomRepository _iBookingRoomRepository;

    private readonly ISetCurrencyService _iSetCurrencyService;
    private readonly ISetFincYearService _iSetFincYearService;
    private readonly IAccLedgerService _iAccLedgerService;
    private readonly IAccTranMstRepository _iAccTranMstRepository;
    private readonly IAccTranDtlRepository _iAccTranDtlRepository;
    private readonly IFoodIngredientRepository _iFoodIngredientRepository;
    private readonly IItemConvertionRepository _iItemConvertionRepository;

    private readonly IInventoryReportRepository _iInvReportRepository;
    private readonly ICustomerTypeRepository _iCustomerTypeRepository;
    private readonly ITableRepository _iTableRepository;
    private readonly ICustomerRepository _iCustomerRepository;
    private readonly IBillingRepository _iBillingRepository;

    public FoodOrderService(IFoodOrderRepository iRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork,
        IAutoCodeRepository iAutoCodeRepository,
        IFoodItemRepository iFoodItemRepository,
        IRsOrderPaymentRepository iOrderPaymentRepository,
        IBookingRoomRepository iBookingRoomRepository,
        IFoodOrderItemRepository iFoodOrderItemRepository,
        ISetCurrencyService iSetCurrencyService,
        ISetFincYearService iSetFincYearService,
        IAccLedgerService iAccLedgerService,
        IAccTranMstRepository iAccTranMstRepository,
        IFoodIngredientRepository iFoodIngredientRepository,
        IItemConvertionRepository iItemConvertionRepository,
        IInventoryReportRepository iInvReportRepository,
        ICustomerTypeRepository iCustomerTypeRepository,
        ITableRepository iTableRepository,
        ICustomerRepository iCustomerRepository,
        IAccTranDtlRepository iAccTranDtlRepository,
        IBillingRepository iBillingRepository) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iAutoCodeRepository = iAutoCodeRepository;
        _iFoodItemRepository = iFoodItemRepository;
        _iOrderPaymentRepository = iOrderPaymentRepository;
        _iBookingRoomRepository = iBookingRoomRepository;
        _iFoodOrderItemRepository = iFoodOrderItemRepository;
        _iSetCurrencyService = iSetCurrencyService;
        _iSetFincYearService = iSetFincYearService;
        _iAccLedgerService = iAccLedgerService;
        _iAccTranMstRepository = iAccTranMstRepository;
        _iFoodIngredientRepository = iFoodIngredientRepository;
        _iItemConvertionRepository = iItemConvertionRepository;
        _iInvReportRepository = iInvReportRepository;
        _iCustomerTypeRepository = iCustomerTypeRepository;
        _iTableRepository = iTableRepository;
        _iCustomerRepository = iCustomerRepository;
        _iAccTranDtlRepository = iAccTranDtlRepository;
        _iBillingRepository = iBillingRepository;
    }

    #endregion

    #region OrderEntry

    public async Task<(bool, long)> FoodOrderEntry(FoodOrderVm vm)
    {
        var orderModel = _iMapper.Map<RsFoodOrder>(vm);

        orderModel.OrderNo = await GetFoodOrderCode();
        orderModel.OrderDate = (DateTime)(!string.IsNullOrEmpty(vm.OrderDateStr) ? Utility.ConvertStrToDate(vm.OrderDateStr) : DU.Utility.GetBdDateTimeNow());
        orderModel.OrderStatus = RsOrderStatusEnum.Pending;
        orderModel.ActionById = CurrentUserId;
        orderModel.ActionDate = Utility.GetBdDateTimeNow();

        var reportDate = Utility.GenerateReportDate(orderModel.OrderDate);
        orderModel.ReportDate = reportDate;

        if (orderModel.OrderType == RsOrderTypeEnum.Reservation)
        {
            orderModel.OrderDate = (DateTime)(!string.IsNullOrEmpty(vm.ReservationDateStr) ? Utility.ConvertStrToDate(vm.ReservationDateStr) : DU.Utility.GetBdDateTimeNow());
            orderModel.ReservationDate = !string.IsNullOrEmpty(vm.OrderDateStr) ? Utility.ConvertStrToDate(vm.OrderDateStr) : DU.Utility.GetBdDateTimeNow();
        }
        else
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            orderModel.OrderDate = orderModel.OrderDate.Add(currentTime);
        }



        if (vm.RsFoodOrderItems.Count > 0 is false)
            throw new Exception("No Food Item Found...!!");

        var foodItems = _iMapper.Map<List<RsFoodOrderItem>>(vm.RsFoodOrderItems);

        HtBilling billInfo = null;

        if (foodItems.Count > 0)
        {
            foreach (var food in foodItems)
            {
                var filterData = vm.RsFoodOrderItems.FirstOrDefault(x => x.FoodId == food.FoodId);

                if (filterData == null)
                    throw new Exception("Food Not Found...!");

                food.ActionDate = Utility.GetBdDateTimeNow();
                food.ActionById = CurrentUserId;

                if (food.FoodId > 0)
                {
                    var foodInfo = _iFoodItemRepository.GetFirstOrDefault(x => x.Id == food.FoodId && x.IsActive && !x.IsDeleted);
                    if (foodInfo == null)
                        throw new Exception("Food Item Not Found...!!");

                    food.Rate = foodInfo.Rate;
                    food.TotalAmount = food.Rate * food.Quantity;
                }
            }

            orderModel.RsFoodOrderItems = foodItems;

            if (vm.RoomId != null && vm.RoomId > 0)
            {
                var bookingRoomInfo = await _iBookingRoomRepository.GetBookedRoomInfoByRoomIdAsync((long)vm.RoomId);
                if (bookingRoomInfo == null)
                    throw new Exception("No Booking Room Information Found...!!");

                orderModel.BookingId = bookingRoomInfo.BookingId;

                //if room service is auto
                if (vm.IsRoomService)
                {
                    var orderAmount = foodItems.Sum(x => x.TotalAmount);
                    var serviceCharge = Utility.PercentCalculation(10, orderAmount);
                    orderModel.ServiceCharge = serviceCharge;
                }
                else
                {
                    orderModel.ServiceCharge = 0;
                }
            }

            var netTotalAmount = foodItems.Sum(x => x.TotalAmount);
            orderModel.NetAmount = (orderModel.VAT + orderModel.TAX + orderModel.ServiceCharge + netTotalAmount) - (orderModel.Discount);
        }

        RsOrderPayments paymentModel = null;
        AccTranMst paymentVoucher = null;

        if (vm.PaidAmount > 0 && vm.PayMode != null)
        {
            paymentModel = new RsOrderPayments();
            paymentModel.PaidAmount = vm.PaidAmount;
            paymentModel.PayMode = (PayModeEnum)vm.PayMode;
            paymentModel.PaidDate = orderModel.OrderType == RsOrderTypeEnum.Reservation
                ? (DateTime)orderModel.ReservationDate : orderModel.OrderDate;
            paymentModel.Remarks = $"Amount {vm.PaidAmount} is paid against Order: {orderModel.OrderNo}.";
            paymentModel.ActionById = CurrentUserId;
            paymentModel.ActionDate = Utility.GetBdDateTimeNow();
            paymentModel.PaymentType = RsOrderPaymentTypeEnum.Receive;

            var pReportDate = Utility.GenerateReportDate(paymentModel.PaidDate);
            paymentModel.ReportDate = pReportDate;

            if (orderModel.NetAmount == paymentModel.PaidAmount)
                orderModel.PaymentStatus = RsOrderPaymentStatusEnum.FullPayment;
            else if (paymentModel.PaidAmount < orderModel.NetAmount)
                orderModel.PaymentStatus = RsOrderPaymentStatusEnum.PartialPayment;

            paymentVoucher = await GetFoodOrderPaymentQuickVoucher(paymentModel, orderModel.OrderNo);//for quick voucher system
            if (paymentVoucher == null)
                throw new Exception("Somthing Went Wrong Creating Voucher..!!");
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iRepository.AddAsync(orderModel);
        var orderAdded = await _iUnitOfWork.CompleteAsync();

        if (paymentModel != null)
        {
            paymentModel.OrderId = orderModel.Id;
            await _iOrderPaymentRepository.AddAsync(paymentModel);
            var isPaymentAdded = await _iUnitOfWork.CompleteAsync();

            // have to add food order payment id
            paymentVoucher.RsPaymentId = paymentModel.Id;
            await _iAccTranMstRepository.AddAsync(paymentVoucher);
            var isVoucherAdded = await _iUnitOfWork.CompleteAsync();

            if (!isPaymentAdded && !isVoucherAdded) { return (false, 0); }
        }

        if (!orderAdded) { return (false, 0); }
        ts.Complete();
        return (true, orderModel.Id);
    }

    #endregion

    #region Search
    public async Task<DataTablePagination<FoodOrderSearchVm, FoodOrderSearchVm>> SearchAsync(DataTablePagination<FoodOrderSearchVm, FoodOrderSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }
    #endregion

    #region GetFoodOrderCode

    public async Task<string> GetFoodOrderCode()
    {
        var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.RsFoodOrders.ToString(), "OrderNo", "ORD", 6);
        return data;
    }

    #endregion

    #region GetFoodOrderById

    public async Task<FoodOrderVm> GetFoodOrderByIdAsync(long id)
    {
        var data = await _iRepository.GetFoodOrderByIdAsync(id);

        var model = _iMapper.Map<FoodOrderVm>(data);

        model.CustomerName = $"{data.Customer.Salutation} {data.Customer.FirstName} {data.Customer.LastName}";
        model.CustomerMobile = data.Customer.Mobile;
        model.CustomerAddress = data.Customer.Address;
        model.CustomerTypeName = data.CustomerType.TypeName;
        model.RoomNo = data.Room?.RoomNo;
        model.BookingNo = data.Booking?.BookingNo;
        model.BookingId = data.Booking?.Id;
        model.TableNo = data.Table?.TableNo;
        model.WaiterName = data.Waiter?.Name;
        model.WaiterCode = data.Waiter?.Code;
        model.ActionByName = data.ActionBy?.FullName;
        model.CustomerTypeCode = data.CustomerType.TypeCode;
        var paidList = await _iOrderPaymentRepository.GetAsync(x => x.OrderId == model.Id && !x.IsDeleted);

        model.PaidAmount = paidList.Where(c => c.PaymentType == RsOrderPaymentTypeEnum.Receive).Sum(x => x.PaidAmount);

        model.AlreadyRefundAmount = paidList.Where(c => c.PaymentType == RsOrderPaymentTypeEnum.Refund).Sum(x => x.PaidAmount);

        var receiveList = paidList.Where(c => c.PaymentType == RsOrderPaymentTypeEnum.Receive).ToList();
        model.OrderPayments = _iMapper.Map<ICollection<RsOrderPaymentVm>>(receiveList);

        if (model.RsFoodOrderItems.Count > 0)
        {
            foreach (var dtl in model.RsFoodOrderItems)
            {
                var filterData = data.RsFoodOrderItems.FirstOrDefault(c => c.Id == dtl.Id);

                dtl.FoodName = filterData?.Food.ItemName;
                dtl.FoodDescription = filterData?.Food.Description;
            }
        }

        return model;
    }

    #endregion

    #region PayFoodOrder

    public async Task<bool> PayFoodOrder(PayRsOrderVm vm)
    {
        if (vm == null && !(vm.OrderId > 0) && !(vm.PaidAmount > 0))
            throw new Exception("Information is not correct..!!");

        var paymentModel = _iMapper.Map<RsOrderPayments>(vm);
        //paymentModel.PaidDate = Utility.GetBdDateTimeNow();
        paymentModel.PaidDate = (DateTime)(!string.IsNullOrEmpty(vm.PaidDateStr) ? Utility.ConvertStrToDate(vm.PaidDateStr) : DU.Utility.GetBdDateTimeNow());
        paymentModel.ActionById = CurrentUserId;
        paymentModel.ActionDate = Utility.GetBdDateTimeNow();
        paymentModel.PaymentType = RsOrderPaymentTypeEnum.Receive;

        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        var reportDate = Utility.GenerateReportDate(paymentModel.PaidDate.Add(currentTime));
        paymentModel.ReportDate = reportDate;

        var order = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == vm.OrderId && x.PaymentStatus != RsOrderPaymentStatusEnum.FullPayment && !x.IsDeleted);
        if (order == null)
            throw new Exception("Order Info Is Not Correct Of Order Is Paid Fully...!!");

        paymentModel.OrderId = order.Id;

        var paidList = _iOrderPaymentRepository.Get(c => c.OrderId == paymentModel.OrderId && c.PaymentType == RsOrderPaymentTypeEnum.Receive).ToList();
        var alreadyPaidAmount = paidList.Sum(x => x.PaidAmount);

        var totalAmount = alreadyPaidAmount + paymentModel.PaidAmount;

        if (totalAmount > order.NetAmount)
            throw new Exception("Given Amount Is Higher Than Net Amount...!!");

        if (totalAmount == order.NetAmount)
            order.PaymentStatus = RsOrderPaymentStatusEnum.FullPayment;
        else
            order.PaymentStatus = RsOrderPaymentStatusEnum.PartialPayment;

        //var paymentVoucher = await GetPaymentVoucher(paymentModel, bookingService.BookingNo, bill.BillNumber, alreadyPaidAmount: alreadyPaidAmount);
        //if (paymentVoucher == null)
        //    throw new Exception("Somthing Went Wrong Creating Voucher..!!");

        var paymentVoucher = await GetFoodOrderPaymentQuickVoucher(paymentModel, order.OrderNo);
        if (paymentVoucher == null)
            throw new Exception("Somthing Went Wrong Creating Voucher..!!");

        RsOrderPayments refundModel = null;

        var advanceRefunded = _iOrderPaymentRepository.GetFirstOrDefault(c => c.OrderId == paymentModel.OrderId && c.PaymentType == RsOrderPaymentTypeEnum.Refund);

        var advanceAmount = paidList.Where(x => x.PaidDate.Date < order.OrderDate.Date).Sum(x => x.PaidAmount);

        if (advanceRefunded == null && advanceAmount > 0 && paymentModel.PaidDate.Date >= order.OrderDate.Date)
        {
            var todayPaidAmount = paidList.Where(c => c.PaidDate.Date == paymentModel.PaidDate.Date).Sum(x => x.PaidAmount);

            refundModel = new RsOrderPayments();
            refundModel.OrderId = order.Id;
            refundModel.PaidAmount = advanceAmount;
            refundModel.PaidDate = paymentModel.PaidDate;
            refundModel.ActionById = CurrentUserId;
            refundModel.ActionDate = Utility.GetBdDateTimeNow();
            refundModel.PaymentType = RsOrderPaymentTypeEnum.Refund;
            refundModel.Remarks = $"Advance Payment Adjusted..Amount: {refundModel.PaidAmount}";

            var rReportDate = Utility.GenerateReportDate(refundModel.PaidDate.Add(currentTime));
            refundModel.ReportDate = reportDate;
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _iOrderPaymentRepository.AddAsync(paymentModel);
        await _iUnitOfWork.CompleteAsync();

        paymentVoucher.RsPaymentId = paymentModel.Id;
        await _iAccTranMstRepository.AddAsync(paymentVoucher);

        if (refundModel != null && refundModel.PaidAmount > 0)
        {
            await _iOrderPaymentRepository.AddAsync(refundModel);
        }

        await _iUnitOfWork.CompleteAsync();
        await _iRepository.UpdateAsync(order);

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) return false;
        ts.Complete();
        return true;
    }

    #endregion

    #region OrderBillHtml

    public async Task<(string, float)> GetOrderBillByIdAsyncHtml(long id)
    {
        var data = await GetFoodOrderByIdAsync(id);

        float pageHeight = 135;

        var roomHtml = "";
        if (!string.IsNullOrEmpty(data.RoomNo))
        {
            string serviceHtml = data.IsRoomService ? "Room Service" : "Table Service";

            roomHtml = $@"<tr>
                            <td style='width:40%;'><b>Room No</b></td>
                            <td style='width:60%'>{data.RoomNo} - {serviceHtml}</td>
                        </tr>";
        }

        var fullHtml = "";
        fullHtml += $@"<div style='text-align:center;'><b>Bill No : {data.OrderNo}</b></div>";
        fullHtml += "<div style='padding-top:5px'>";
        fullHtml += $@"<table class='receipt-table receipt-border'>
                                <tbody>
                                    <tr>
                                        <td style='width:40%;'><b>Customer Type</b></td>
                                        <td style='width:60%'>{data.CustomerTypeName}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:40%;'><b>Bill Date</b></td>
                                        <td style='width:60%'>{data.OrderDate.ToString("dd-MMM-yyyy")}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:40%;'><b>Bill Time</b></td>
                                        <td style='width:60%'>{data.OrderDate.ToString("hh:mm tt")}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:40%;'><b>Customer Name</b></td>
                                        <td style='width:60%'>{data.CustomerName}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:40%;'><b>Table No</b></td>
                                        <td style='width:60%'>{data.TableNo}</td>
                                    </tr>
                                    {roomHtml}
                                    <tr>
                                        <td style='width:40%;'><b>Bill Prepared By</b></td>
                                        <td style='width:60%'>{data.ActionByName}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:40%;'><b>Waiter</b></td>
                                        <td style='width:60%'>{data.WaiterCode}-({data.WaiterName})</td>
                                    </tr>
                                </tbody>
                            </table>";

        fullHtml += "</div>";

        //Order Item
        fullHtml += "<table class='receipt-table-dtl receipt-border' style='width:100%;text-align:center;margin-top:5px;font-size:10px'>";

        fullHtml += "<tbody>";

        if (data.RsFoodOrderItems.Count > 0)
        {
            foreach (var (item, i) in data.RsFoodOrderItems.GetItemWithIndex())
            {
                fullHtml += "<tr>";

                fullHtml += $@"<td class='text-start' style='width:75%;'><b>{item.FoodName}</b><br/>";

                if (!string.IsNullOrEmpty(item.FoodDescription))
                {
                    fullHtml += $@"<p class='f-8'>{item.FoodDescription}</p>";
                }

                fullHtml += $@"{item.Quantity}x{item.Rate}</td>";

                fullHtml += $@"<td class='text-end' style='width:25%;'>{item.TotalAmount}</td>";

                fullHtml += "</tr>";

                pageHeight += 8;
            }
        }

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        fullHtml += "<table class='receipt-table-dtl' style='width:100%;text-align:center;margin-top:5px;font-size:10px'>";

        fullHtml += "<tbody>";

        fullHtml += "<tr>";
        fullHtml += $@"<td class='text-start'><b>Sub Total</b></td>";
        fullHtml += $@"<td class='text-end'>{data.OrderAmount.ToString("N2")}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $@"<td class='text-start'><b>VAT</b></td>";
        fullHtml += $@"<td class='text-end'>{data.VAT.ToString("N2")}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $@"<td class='text-start'><b>Service Charge</b></td>";
        fullHtml += $@"<td class='text-end'>{data.ServiceCharge.ToString("N2")}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $@"<td class='text-start'><b>Discount</b></td>";
        fullHtml += $@"<td class='text-end'>{data.Discount.ToString("N2")}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $@"<td class='text-start'><b>Total Amount</b></td>";
        fullHtml += $@"<td class='text-end'>{data.NetAmount.ToString("N2")}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $@"<td class='text-start'><b>Paid Amount</b></td>";
        fullHtml += $@"<td class='text-end'>{data.PaidAmount.ToString("N2")}</td>";
        fullHtml += "</tr>";

        var dueAmount = data.PaidAmount > 0 && data.NetAmount > data.PaidAmount ? (data.NetAmount - data.PaidAmount) : 0;

        fullHtml += "<tr>";
        fullHtml += $@"<td class='text-start'><b>Due</b></td>";
        fullHtml += $@"<td class='text-end'>{dueAmount.ToString("N2")}</td>";
        fullHtml += "</tr>";

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        return (fullHtml, pageHeight);
    }

    #endregion

    #region kotHtml

    private async Task<(string, float)> GetKotByIdAsyncHtml(long id, List<RsFoodOrderItem> kotItems)
    {
        var data = await GetFoodOrderByIdAsync(id);

        var kotItemsIds = kotItems.Select(x => x.Id).ToList();
        var printOrderItems = data.RsFoodOrderItems.Where(x => kotItemsIds.Contains(x.Id)).ToList();

        float pageHeight = 120;

        var roomHtml = "";
        if (!string.IsNullOrEmpty(data.RoomNo))
        {
            string serviceHtml = data.IsRoomService ? "Room Service" : "Table Service";

            roomHtml = $@"<tr>
                            <td style='width:40%;'><b>Room No</b></td>
                            <td style='width:60%'>{data.RoomNo} - {serviceHtml}</td>
                        </tr>";
        }

        var fullHtml = "";
        fullHtml += "<div class='text-center'><p><u>KOT</u></p></div>";
        fullHtml += "<div style='padding-top:5px'>";
        fullHtml += $@"<div style='text-align:center;'><b>KOT NO : {printOrderItems.FirstOrDefault()?.KotNo}/{DateTime.Today.ToString("dd.MM.yy")}</b></div>";
        fullHtml += $@"<table class='receipt-table receipt-border'>
                                <tbody>
                                    <tr>
                                        <td style='width:40%;'><b>Order No</b></td>
                                        <td style='width:60%'>{data.OrderNo}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:40%;'><b>Sold To</b></td>
                                        <td style='width:60%'>{data.CustomerTypeName}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:40%;'><b>Order Date</b></td>
                                        <td style='width:60%'>{data.OrderDate.ToString("dd/MMM/yyyy hh:mm tt")}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:40%;'><b>Table No</b></td>
                                        <td style='width:60%'>{data.TableNo}</td>
                                    </tr>
                                    {roomHtml}
                                    <tr>
                                        <td style='width:40%;'><b>KOT Generate Time</b></td>
                                        <td style='width:60%'>{DateTime.Now.ToString("dd/MMM/yyyy HH:mm tt")}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:40%;'><b>KOT Prepared By</b></td>
                                        <td style='width:60%'>{data.ActionByName}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:40%;'><b>Waiter</b></td>
                                        <td style='width:60%'>{data.WaiterCode}-({data.WaiterName})</td>
                                    </tr>
                                </tbody>
                            </table>";

        fullHtml += "</div>";

        //Order Item
        fullHtml += "<table class='receipt-table-dtl receipt-border' style='width:100%;text-align:center;margin-top:5px;font-size:10px'>";

        fullHtml += "<tbody>";

        if (data.RsFoodOrderItems.Count > 0)
        {
            foreach (var (item, i) in printOrderItems.GetItemWithIndex())
            {
                fullHtml += "<tr>";

                fullHtml += $@"<td class='text-start' style='width:75%;'><b>{item.FoodName}</b>";

                if (!string.IsNullOrEmpty(item.FoodDescription))
                {
                    fullHtml += $@"<br/><p class='f-9'>{item.FoodDescription}</p>";
                }

                fullHtml += $@"</td>";

                fullHtml += $@"<td class='text-end' style='width:25%;'>x <b>{item.Quantity}</b></td>";

                fullHtml += "</tr>";

                pageHeight += 6;
            }
        }

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        return (fullHtml, pageHeight);
    }

    #endregion

    #region RsDailySalesReportHtml

    public async Task<string> RsMonthlySalesReportHtml(RsDailySalesReportVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        var data = await _iRepository.RsMonthlySalesDataAsync(vm);
        data = data.OrderBy(x => x.OrderNo).ToList();

        vm.StrFromDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrFromDate;
        vm.StrToDate = (string.IsNullOrEmpty(vm.StrToDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrToDate;
        var queryFromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate));
        var queryToDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate));

        if (data != null && data.Count > 0)
        {
            if (isPrint)
            {
                fullHtml += $@"<h6 style='text-align:center;padding-bottom:5px;'>Date: {queryFromDate.ToString("dd/MM/yyyy")} To {queryToDate.ToString("dd/MM/yyyy")}</h6>";
            }

            fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
            fullHtml += "<thead>";
            fullHtml += $@"<tr style='height:30px;'><td colspan='13' class='text-center'><b>Date: {queryFromDate.ToString("dd/MM/yyyy")} To {queryToDate.ToString("dd/MM/yyyy")}</b></td></tr>";
            fullHtml += "<tr style='height:30px;font-size: 12px;'>";

            fullHtml += $@"<th style='width:50px;text-align:center;'>SL No</th>
                            <th style='width:150px;text-align:center;'>Order</th>
                            <th class='text-center' style='width:200px;'>Customer</th>
                            <th class='text-center' style='width:100px;'>Customer Type</th>
                            <th class='text-center' style='width:100px;'>Room</th>
                            <th class='text-center' style='width:120px;'>Booking</th>
                            <th class='text-center' style='width:80px;'>Order Amount</th>
                            <th class='text-center' style='width:80px;'>Service Charge</th>
                            <th class='text-center' style='width:80px;'>Discount</th>
                            <th class='text-center' style='width:80px;'>Net Amount</th>
                            <th class='text-center' style='width:80px;'>Paid Amount</th>
                            <th class='text-center' style='width:80px;'>Due Amount</th>
                            <th class='text-center' style='width:120px;'>Order Description</th>";

            fullHtml += "</tr>";
            fullHtml += "</thead>";
            fullHtml += "<tbody>";

            for (int i = 0; i < data.Count; i++)
            {
                RsDailySalesReportVm objBooking = data[i];

                string orderNo = !isPrint ? $"<a target='_blank' href='../FoodOrder/Details/{objBooking.OrderId}'>{objBooking.OrderNo}</a>" : $"{objBooking.OrderNo}";
                string bookingNo = !isPrint ? $"<a target='_blank' href='../BookingService/Details/{objBooking.BookingId}'>{objBooking.BookingNo}</a>" : $"{objBooking.BookingNo}";

                fullHtml += "<tr>";

                fullHtml += $@"<td style='text-align:left;text-align:center;'>{i + 1}</td>
                                <td style='text-align:left;'><b>{orderNo}</b><br/>{objBooking.OrderDate.ToString("dd/MMM/yyyy")}</td>
                                <td style='text-align:left;'><b>{objBooking.CustomerName}</b><br/>{objBooking.Mobile}</td>
                                <td style='text-align:center;'>{objBooking.CustomerType}</td>
                                <td style='text-align:center;'>{objBooking.RoomNo}</td>
                                <td style='text-align:center;'><b>{bookingNo}</b><br/>{objBooking.BookingDate?.ToString("dd/MMM/yyyy")}</td>
                                <td style='text-align:right;'>{objBooking.OrderAmount:N2}</td>
                                <td style='text-align:right;'>{objBooking.ServiceCharge:N2}</td>
                                <td style='text-align:right;'>{objBooking.Discount:N2}</td>
                                <td style='text-align:right;'>{objBooking.NetAmount:N2}</td>
                                <td style='text-align:right;'>{objBooking.PaidAmount:N2}</td>
                                <td style='text-align:right;'>{objBooking.DueAmount:N2}</td>
                                <td style='text-align:left;'>{objBooking.OrderDesc}</td>";

                fullHtml += " </tr>";
            }

            fullHtml += $@"<tr>
                            <td colspan='6' style='text-align:right;'><b>Total</b></td>
                            <td style='text-align:right;'><b>{data.Sum(x => x.OrderAmount):N2}</b></td>
                            <td style='text-align:right;'><b>{data.Sum(x => x.ServiceCharge):N0}</b></td>
                            <td style='text-align:right;'><b>{data.Sum(x => x.Discount):N0}</b></td>
                            <td style='text-align:right;'><b>{data.Sum(x => x.NetAmount):N0}</b></td>
                            <td style='text-align:right;'><b>{data.Sum(x => x.PaidAmount):N0}</b></td>
                            <td style='text-align:right;'><b>{data.Sum(x => x.DueAmount):N0}</b></td>
                        </tr>";
        }
        fullHtml += "</tbody>";

        fullHtml += "</table>";

        return fullHtml;
    }
    #endregion

    #region RsCustomerWiseDueReportHtml

    public async Task<string> RsCustomerWiseDueReportHtml(RsCustomerWiseDueReportVm vm, bool isPrint = false)
    {
        string fullHtml = await _iRepository.RsCustomerWiseDueReportHtml(vm);
        return fullHtml;
    }
    #endregion

    #region RsSalesReportHtml

    public async Task<string> RsSalesReportHtml(RsSalesReportVm vm, bool isPrint = false)
    {
        string fullHtml = await _iRepository.RsSalesReportHtml(vm);
        return fullHtml;
    }
    #endregion

    #region RsDailySalesSummaryReportHtml

    public async Task<string> RsDailySalesSummaryReportHtml(RsDailySalesSummaryVm vm, bool isPrint = false)
    {
        string fullHtml = await _iRepository.RsDailySalesSummaryReportHtml(vm);
        return fullHtml;
    }
    #endregion

    #region UpdateOrder

    public async Task<bool> UpdateOrder(FoodOrderVm vm)
    {
        if (vm == null && !(vm.Id > 0))
            throw new Exception("Booking Service Information Is Not Correct...!!");

        var order = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == vm.Id && !x.IsDeleted);

        if (order == null)
            throw new Exception("Order/Reservation Not Found...!!");

        if (order.OrderStatus == RsOrderStatusEnum.Served)
            throw new Exception("Order Already Served...!!");

        order.VAT = vm.VAT;
        order.ServiceCharge = vm.ServiceCharge;
        order.Discount = vm.Discount;
        order.UpdatedById = CurrentUserId;
        order.UpdateDate = Utility.GetBdDateTimeNow();

        if (order.OrderType == RsOrderTypeEnum.Reservation)
        {
            order.ReservationDate = (DateTime)(!string.IsNullOrEmpty(vm.ReservationDateStr) ? Utility.ConvertStrToDate(vm.ReservationDateStr) : order.ReservationDate);
            order.OrderDate = (DateTime)(!string.IsNullOrEmpty(vm.OrderDateStr) ? Utility.ConvertStrToDate(vm.OrderDateStr) : order.OrderDate);

            order.OrderAmount = 0;
            order.NetAmount = 0;
        }

        #region FoodItem

        List<RsFoodOrderItem> addableItemList = null;
        List<RsFoodOrderItem> updateableItemList = null;
        List<RsFoodOrderItem> deletableItemList = null;

        var existOrderItems = await _iFoodOrderItemRepository.GetAsync(x => x.OrderId == vm.Id && !x.IsDeleted);

        if (vm?.RsFoodOrderItems?.Count > 0)
        {
            var dataListForAdd = vm?.RsFoodOrderItems?.Where(c => c.Id == 0).ToList();

            var updatableItemIds = vm?.RsFoodOrderItems.Where(c => c.Id > 0).Select(c => c.Id).ToList();
            updateableItemList = (existOrderItems.Where(x => updatableItemIds.Contains(x.Id))).ToList();

            if (updateableItemList?.Count > 0)
            {
                foreach (var foodItem in updateableItemList)
                {
                    var filterData = vm.RsFoodOrderItems.Where(c => c.Id == foodItem.Id).FirstOrDefault();

                    foodItem.UpdateDate = Utility.GetBdDateTimeNow();
                    foodItem.UpdatedById = CurrentUserId;

                    foodItem.Rate = filterData.Rate;
                    foodItem.Quantity = filterData.Quantity;
                    foodItem.TotalAmount = (foodItem.Rate * foodItem.Quantity);
                }

                order.OrderAmount = updateableItemList.Sum(x => x.TotalAmount);
                order.NetAmount = (order.OrderAmount + order.VAT + order.TAX + order.ServiceCharge) - (order.Discount);
            }

            var oldIds = updateableItemList?.Select(c => c.Id).ToList();
            deletableItemList = (existOrderItems.Where(x => !oldIds.Contains(x.Id))).ToList();

            if (dataListForAdd?.Count > 0)
            {
                addableItemList = _iMapper.Map<List<RsFoodOrderItem>>(dataListForAdd);

                foreach (var (foodItem, i) in addableItemList.GetItemWithIndex())
                {
                    var filterData = dataListForAdd.FirstOrDefault(c => c.FoodId == foodItem.FoodId);

                    if (filterData == null)
                        throw new Exception("Item Not Found...!");

                    foodItem.OrderId = order.Id;

                    foodItem.ActionDate = Utility.GetBdDateTimeNow();
                    foodItem.ActionById = CurrentUserId;

                    if (foodItem.FoodId > 0)
                    {
                        var foodInfo = _iFoodItemRepository.GetFirstOrDefault(x => x.Id == foodItem.FoodId && x.IsActive && !x.IsDeleted);
                        if (foodInfo == null)
                            throw new Exception("Food Item Not Found...!!");

                        foodItem.Rate = foodInfo.NetRate;
                        foodItem.Quantity = filterData.Quantity;
                        foodItem.TotalAmount = (foodItem.Rate * foodItem.Quantity);
                    }
                }

                order.OrderAmount += addableItemList.Sum(x => x.TotalAmount);
                order.NetAmount = (order.OrderAmount + order.VAT + order.TAX + order.ServiceCharge) - (order.Discount);
            }

        }

        var paidList = _iOrderPaymentRepository.Get(c => c.OrderId == order.Id).ToList();
        var alreadyPaidAmount = paidList.Sum(x => x.PaidAmount);

        if (alreadyPaidAmount == order.NetAmount)
            order.PaymentStatus = RsOrderPaymentStatusEnum.FullPayment;
        else if (alreadyPaidAmount < order.NetAmount)
            order.PaymentStatus = RsOrderPaymentStatusEnum.PartialPayment;

        #endregion

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        _iRepository.Update(order);

        if (addableItemList?.Count > 0)
        {
            _iFoodOrderItemRepository.AddRange(addableItemList);
        }

        if (updateableItemList?.Count > 0)
        {
            _iFoodOrderItemRepository.UpdateRange(updateableItemList);
        }

        if (deletableItemList?.Count > 0)
        {
            _iFoodOrderItemRepository.RemoveRange(deletableItemList);
        }

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }

    #endregion

    #region RefundReservation

    public async Task<bool> RefundReservation(PayRsOrderVm vm)
    {
        if (vm == null && !(vm.OrderId > 0) && !(vm.PaidAmount > 0))
            throw new Exception("Information is not correct..!!");

        var paymentModel = _iMapper.Map<RsOrderPayments>(vm);
        paymentModel.PaidDate = Utility.GetBdDateTimeNow();
        paymentModel.ActionById = CurrentUserId;
        paymentModel.ActionDate = Utility.GetBdDateTimeNow();
        paymentModel.PaymentType = RsOrderPaymentTypeEnum.Refund;

        TimeSpan currentTime = DateTime.Now.TimeOfDay;
        var reportDate = Utility.GenerateReportDate(paymentModel.PaidDate.Add(currentTime));
        paymentModel.ReportDate = reportDate;

        var order = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == vm.OrderId && !x.IsDeleted);

        if (order.OrderType != RsOrderTypeEnum.Reservation)
            throw new Exception("This Is Not Reservation Order..!!");

        if (order.OrderStatus != RsOrderStatusEnum.Canceled)
            throw new Exception("Only Cancel Order Can Be Refunded..!!");

        paymentModel.OrderId = order.Id;

        var paidList = _iOrderPaymentRepository.Get(c => c.OrderId == paymentModel.OrderId).ToList();

        var alreadyPaidAmount = paidList.Where(c => c.PaymentType == RsOrderPaymentTypeEnum.Receive).Sum(x => x.PaidAmount);

        var alreadyRefundAmount = paidList.Where(c => c.PaymentType == RsOrderPaymentTypeEnum.Refund).Sum(x => x.PaidAmount);

        var dueRefundAmount = alreadyPaidAmount - alreadyRefundAmount;

        if (paymentModel.PaidAmount > dueRefundAmount)
            throw new Exception("Refund Amount Is Higher Than Due Refund...!!");

        var totalAmount = alreadyRefundAmount + paymentModel.PaidAmount;

        if (totalAmount == alreadyPaidAmount)
            order.PaymentStatus = RsOrderPaymentStatusEnum.Refund;

        //var paymentVoucher = await GetPaymentVoucher(paymentModel, bookingService.BookingNo, bill.BillNumber, alreadyPaidAmount: alreadyPaidAmount);
        //if (paymentVoucher == null)
        //    throw new Exception("Somthing Went Wrong Creating Voucher..!!");

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _iOrderPaymentRepository.AddAsync(paymentModel);
        await _iUnitOfWork.CompleteAsync();

        await _iRepository.UpdateAsync(order);

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) return false;
        ts.Complete();
        return true;
    }

    #endregion

    #region ReservationBillHtml

    public async Task<(string, float)> GetReservationBillByIdAsyncHtml(long id)
    {
        var data = await GetFoodOrderByIdAsync(id);

        float pageHeight = 135;

        var fullHtml = "";
        fullHtml += "<div><p><u>Bill To :</u></p></div>";
        fullHtml += "<div style='padding-top:5px'>";
        fullHtml += $@"<table class='receipt-table receipt-border'>
                                <tbody>
                                    <tr>
                                        <td style='width:40%;'><b>Type</b></td>
                                        <td style='width:60%'>Reservation</td>
                                    </tr>
                                    <tr>
                                        <td style='width:40%;'><b>Bill No</b></td>
                                        <td style='width:60%'>{data.OrderNo}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:40%;'><b>Sold To</b></td>
                                        <td style='width:60%'>{data.CustomerTypeName}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:40%;'><b>Entry Date</b></td>
                                        <td style='width:60%'>{data.OrderDate.ToString("dd-MMM-yyyy")}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:40%;'><b>Customer Name</b></td>
                                        <td style='width:60%'>{data.CustomerName}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:40%;'><b>Reservation Date</b></td>
                                        <td style='width:60%'>{data.ReservationDate?.ToString("dd-MMM-yyyy")}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:40%;'><b>Sales Person</b></td>
                                        <td style='width:60%'>{data.ActionByName}</td>
                                    </tr>
                                </tbody>
                            </table>";

        fullHtml += "</div>";

        //Order Item
        fullHtml += "<table class='receipt-table-dtl receipt-border' style='width:100%;text-align:center;margin-top:5px;font-size:10px'>";

        fullHtml += "<tbody>";

        if (data.RsFoodOrderItems.Count > 0)
        {
            foreach (var (item, i) in data.RsFoodOrderItems.GetItemWithIndex())
            {
                fullHtml += "<tr>";

                fullHtml += $@"<td class='text-start' style='width:75%;'><b>{item.FoodName}</b><br/>";

                if (!string.IsNullOrEmpty(item.FoodDescription))
                {
                    fullHtml += $@"<p class='f-8'>{item.FoodDescription}</p>";
                }

                fullHtml += $@"{item.Quantity}x{item.Rate}</td>";

                fullHtml += $@"<td class='text-end' style='width:25%;'>{item.TotalAmount}</td>";

                fullHtml += "</tr>";

                pageHeight += 8;
            }
        }

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        fullHtml += "<table class='receipt-table-dtl' style='width:100%;text-align:center;margin-top:5px;font-size:10px'>";

        fullHtml += "<tbody>";

        fullHtml += "<tr>";
        fullHtml += $@"<td class='text-start'><b>Sub Total</b></td>";
        fullHtml += $@"<td class='text-end'>{data.OrderAmount.ToString("N2")}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $@"<td class='text-start'><b>VAT</b></td>";
        fullHtml += $@"<td class='text-end'>{data.VAT.ToString("N2")}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $@"<td class='text-start'><b>Service Charge</b></td>";
        fullHtml += $@"<td class='text-end'>{data.ServiceCharge.ToString("N2")}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $@"<td class='text-start'><b>Discount</b></td>";
        fullHtml += $@"<td class='text-end'>{data.Discount.ToString("N2")}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $@"<td class='text-start'><b>Total Amount</b></td>";
        fullHtml += $@"<td class='text-end'>{data.NetAmount.ToString("N2")}</td>";
        fullHtml += "</tr>";

        fullHtml += "<tr>";
        fullHtml += $@"<td class='text-start'><b>Paid Amount</b></td>";
        fullHtml += $@"<td class='text-end'>{data.PaidAmount.ToString("N2")}</td>";
        fullHtml += "</tr>";

        //var changeAmount = data.PaidAmount > data.NetAmount ? (data.PaidAmount - data.NetAmount) : 0;

        //fullHtml += "<tr>";
        //fullHtml += $@"<td class='text-start'><b>Change</b></td>";
        //fullHtml += $@"<td class='text-end'>{changeAmount.ToString("N2")}</td>";
        //fullHtml += "</tr>";

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        return (fullHtml, pageHeight);
    }

    #endregion

    #region GetFoodOrderPaymentQuickVoucher

    private async Task<AccTranMst> GetFoodOrderPaymentQuickVoucher(RsOrderPayments payment, string orderNo)
    {
        var model = new AccTranMst();
        model.VcDate = payment.PaidDate;
        model.VcType = VoucherType.JournalVoucher;
        model.SubVacType = VoucherType.JournalVoucher;
        model.VcNo = await _iAutoCodeRepository.GetVoucherAutoNo(VoucherTypeCode.JournalVoucher, model.VcDate);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.FinYearId = (await _iSetFincYearService.GetFincYearByDate(model.VcDate)).Id;
        model.Narration = $"Food bill receive. Order No:{orderNo}";
        model.ActionById = CurrentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();
        model.IsAuto = true;

        var accTranList = new List<AccTranDtl>();

        if (payment != null)
        {
            var modelDtl = new AccTranDtl();

            var crLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.RestaurantLedger);
            var drLedger = _iAccLedgerService.GetFirstOrDefault(c => c.LedgerCode == AccLadgerCode.FoodPaymentReceive);


            if (crLedger == null)
                throw new Exception("No Ledger Found Against Food Bill Receive..!!");
            if (drLedger == null)
                throw new Exception("No Bank Ledger Found Against Restaurant..!!");

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

    #region GenerateKot

    public async Task<(string, float)> GenerateKot(long id)
    {
        //var todayOrderItems = await _iFoodOrderItemRepository.GetAsync(x => !x.IsDeleted && x.ActionDate.Date == DateTime.Today.Date);
        var todayOrderItems = await _iFoodOrderItemRepository.GetAsync(x => !x.IsDeleted);
        if (!(todayOrderItems.Count > 0))
            throw new Exception("No Order Items Not Found For Today..!!");

        var orderItems = todayOrderItems.Where(x => x.OrderId == id).ToList();
        if (!(orderItems.Count > 0))
            throw new Exception("Order Items Not Found For Generate KOT..!!");

        var itemWithoutKot = orderItems.Where(x => string.IsNullOrEmpty(x.KotNo)).ToList();
        //if (!(itemWithoutKot.Count > 0))
        //    throw new Exception("KOT Already Generated..!!");
        if (!(itemWithoutKot.Count > 0))
        {
            var lastOrderKotNumber = orderItems.Where(x => !string.IsNullOrEmpty(x.KotNo)).LastOrDefault()?.KotNo;
            var lastKotItems = orderItems.Where(x => x.KotNo.Equals(lastOrderKotNumber)).ToList();

            var (phtml, ppageHeight) = await GetKotByIdAsyncHtml(id, lastKotItems);
            return (phtml, ppageHeight);
        }

        var lastKot = todayOrderItems.Where(x => !string.IsNullOrEmpty(x.KotNo)).LastOrDefault();
        int kotNumber = lastKot != null ? Convert.ToInt32(lastKot.KotNo) + 1 : 1;

        itemWithoutKot.ForEach(x => x.KotNo = $"{kotNumber}");

        await _iFoodOrderItemRepository.UpdateRangeAsync(itemWithoutKot);
        var kotAdded = await _iUnitOfWork.CompleteAsync();

        if (!kotAdded)
            throw new Exception("KOT Generated Failed...!!");

        var (html, pageHeight) = await GetKotByIdAsyncHtml(id, itemWithoutKot);
        return (html, pageHeight);
    }

    #endregion

    #region FoodItemsAdd

    public async Task<bool> FoodItemsAddAsync(SaveFoodOrderItemDto dto)
    {
        if (dto == null || !(dto.OrderId > 0) || !(dto.FoodId > 0))
            throw new Exception("Item Information Is Not Correct...!!");

        var order = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == dto.OrderId && !x.IsDeleted);

        if (order == null)
            throw new Exception("Food Order Not Found...!!");

        //if (order.PaymentStatus == RsOrderPaymentStatusEnum.FullPayment)
        //    throw new Exception("Food Bill Already Full Paid...!!");

        #region Bill Details

        var detailModel = _iMapper.Map<RsFoodOrderItem>(dto);
        detailModel.ActionById = CurrentUserId;
        detailModel.ActionDate = Utility.GetBdDateTimeNow();

        detailModel.TotalAmount = detailModel.Rate * detailModel.Quantity;

        #endregion

        order.OrderAmount += detailModel.TotalAmount;

        if (order.RoomId > 0 && order.BookingId > 0)
        {
            var serviceCharge = Utility.PercentCalculation(10, detailModel.TotalAmount);
            order.ServiceCharge += serviceCharge;
            order.NetAmount += (detailModel.TotalAmount + serviceCharge);
        }
        else
        {
            order.NetAmount += detailModel.TotalAmount;
        }

        order.PaymentStatus = order.PaymentStatus == RsOrderPaymentStatusEnum.FullPayment
            ? RsOrderPaymentStatusEnum.PartialPayment
            : RsOrderPaymentStatusEnum.Pending;

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iFoodOrderItemRepository.AddAsync(detailModel);
        await _iRepository.UpdateAsync(order);
        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }

    #endregion

    #region FoodMultipleItemsAdd

    public async Task<bool> FoodMultipleItemsAddAsync(List<SaveFoodOrderItemDto> dtos)
    {
        if (dtos == null || !(dtos.Count > 0))
            throw new Exception("Item Information Is Not Correct...!!");

        var orderId = dtos.FirstOrDefault()?.OrderId;

        if (orderId == null || !(orderId > 0))
            throw new Exception("Order Information Is Not Correct...!!");

        var order = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == orderId && !x.IsDeleted);

        if (order == null)
            throw new Exception("Food Order Not Found...!!");

        var customerType = await _iCustomerTypeRepository.GetFirstOrDefaultAsync(x => x.Id == order.CustomerTypeId && !x.IsDeleted);

        #region Bill Details

        var addableItemList = _iMapper.Map<List<RsFoodOrderItem>>(dtos);

        if (addableItemList != null && addableItemList.Count > 0)
        {
            foreach (var detailModel in addableItemList)
            {
                detailModel.ActionById = CurrentUserId;
                detailModel.ActionDate = Utility.GetBdDateTimeNow();
                detailModel.TotalAmount = detailModel.Rate * detailModel.Quantity;
            }
        }


        #endregion

        var addableItemTotalAmount = addableItemList.Sum(x => x.TotalAmount);

        order.OrderAmount += addableItemTotalAmount;

        if (order.RoomId > 0 && order.BookingId > 0 && order.IsRoomService == true)
        {
            var serviceCharge = Utility.PercentCalculation(10, addableItemTotalAmount);
            order.ServiceCharge += serviceCharge;
            order.NetAmount += (addableItemTotalAmount + serviceCharge);
        }
        else
        {
            order.NetAmount += addableItemTotalAmount;
        }
        if (customerType.DiscountPercent > 0)
        {
            var discount = Utility.PercentCalculation(customerType.DiscountPercent, order.OrderAmount);
            order.Discount = discount;
            order.NetAmount = (order.OrderAmount + order.ServiceCharge) - discount;
        }

        order.PaymentStatus = order.PaymentStatus == RsOrderPaymentStatusEnum.FullPayment
            ? RsOrderPaymentStatusEnum.PartialPayment
            : RsOrderPaymentStatusEnum.Pending;

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iFoodOrderItemRepository.AddRangeAsync(addableItemList);
        await _iRepository.UpdateAsync(order);
        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }

    #endregion

    #region DiscountUpdate

    public async Task<bool> DiscountUpdateAsync(DiscountUpdateDto dto)
    {
        if (dto == null || !(dto.OrderId > 0))
            throw new Exception("Billing Information Is Not Correct...!!");

        var bill = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == dto.OrderId && !x.IsDeleted);

        if (bill == null)
            throw new Exception("Bill Not Found...!!");

        if (bill.PaymentStatus == RsOrderPaymentStatusEnum.FullPayment)
            throw new Exception("Food Bill Already Full Paid...!!");

        bill.Discount = dto.Discount;

        bill.NetAmount = (bill.OrderAmount + bill.VAT + bill.TAX + bill.ServiceCharge) - (bill.Discount);

        var paidList = _iOrderPaymentRepository.Get(c => c.OrderId == bill.Id).ToList();
        var paidAmount = paidList.Sum(x => x.PaidAmount);

        if (paidAmount > bill.NetAmount)
            throw new Exception("Pay Amount Is Higher Than Payable Amount");

        if (paidAmount == bill.NetAmount)
            bill.PaymentStatus = RsOrderPaymentStatusEnum.FullPayment;
        else
            bill.PaymentStatus = bill.PaymentStatus;

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iRepository.UpdateAsync(bill);
        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }

    #endregion

    #region Food Item Remove

    public async Task<bool> FoodItemRemoveAsync(long id)
    {
        var data = await _iFoodOrderItemRepository.GetFirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, s => s.Food);

        if (data == null)
            throw new Exception("Item not found..");

        if (!(data.OrderId > 0))
            throw new Exception("Food Order Information Is Not Correct...!!");

        var bill = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == data.OrderId && !x.IsDeleted, d => d.RsFoodOrderItems);

        if (bill == null)
            throw new Exception("Bill Not Found...!!");

        if (!(bill.RsFoodOrderItems.Count > 1))
            throw new Exception("Can't Remove The Item..Cause this is last order item...!!");

        #region Bill Details

        var detailModel = _iMapper.Map<RsFoodOrderItem>(data);
        detailModel.IsDeleted = true;
        detailModel.TotalAmount = detailModel.Rate * detailModel.Quantity;

        #endregion

        bill.OrderAmount -= (detailModel.Rate * detailModel.Quantity);
        //bill.NetAmount -= (detailModel.Rate * detailModel.Quantity);

        if (bill.RoomId > 0 && bill.BookingId > 0 && bill.IsRoomService == true)
        {
            var serviceCharge = Utility.PercentCalculation(10, detailModel.TotalAmount);
            bill.ServiceCharge -= serviceCharge;
            bill.NetAmount -= (detailModel.TotalAmount + serviceCharge);
        }
        else
        {
            bill.NetAmount -= (detailModel.Rate * detailModel.Quantity);
        }

        var customerType = await _iCustomerTypeRepository.GetFirstOrDefaultAsync(x => x.Id == bill.CustomerTypeId && !x.IsDeleted);

        if (customerType.DiscountPercent > 0)
        {
            var discount = Utility.PercentCalculation(customerType.DiscountPercent, bill.OrderAmount);
            bill.Discount = discount;
            bill.NetAmount = (bill.OrderAmount + bill.ServiceCharge) - discount;
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        _iFoodOrderItemRepository.Remove(detailModel);

        await _iRepository.UpdateAsync(bill);

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }

    #endregion

    #region SingleKotHtml

    public async Task<(string, float)> SingleKotHtml(long id, string kotNo)
    {
        var orderItems = await _iFoodOrderItemRepository.GetAsync(x => x.OrderId == id && !x.IsDeleted);
        if (!(orderItems.Count > 0))
            throw new Exception("No Order Items Not Found..!!");

        var kotItems = orderItems.Where(x => x.KotNo.Equals(kotNo)).ToList();
        if (!(kotItems.Count > 0))
            throw new Exception("No Order Items Not Found..!!");

        var (html, pageHeight) = await GetKotByIdAsyncHtml(id, kotItems);
        return (html, pageHeight);
    }

    #endregion

    #region Food Item Served

    public async Task<bool> FoodItemServedAsync(long id)
    {
        var data = await _iFoodOrderItemRepository.GetFirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

        if (data == null)
            throw new Exception("Food Order Item not found...!!");

        if (data.IsServed)
            throw new Exception("Food Order Item Already Served...!!");

        if (!(data.OrderId > 0))
            throw new Exception("Food Order Information Is Not Correct...!!");

        var bill = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == data.OrderId && !x.IsDeleted, d => d.RsFoodOrderItems);

        if (bill == null)
            throw new Exception("Bill Not Found...!!");

        #region Bill Details

        var detailModel = _iMapper.Map<RsFoodOrderItem>(data);
        detailModel.IsServed = true;
        detailModel.ServedTime = DU.Utility.GetBdDateTimeNow();
        detailModel.ServedById = CurrentUserId;
        #endregion

        if (bill.RsFoodOrderItems.Count > 0)
        {
            var isFullServed = bill.RsFoodOrderItems.All(x => x.IsServed);
            bill.OrderStatus = isFullServed ? RsOrderStatusEnum.Served : RsOrderStatusEnum.Pending;
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _iFoodOrderItemRepository.UpdateAsync(detailModel);

        await _iRepository.UpdateAsync(bill);

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }

    #endregion

    #region Food Order Status Change

    public async Task<bool> FoodOrderStatusChangeAsync(long orderId, int orderStatus)
    {
        var orderInfo = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == orderId && !x.IsDeleted, d => d.RsFoodOrderItems);
        if (orderInfo == null)
            throw new Exception("No Order Found...!!");

        orderInfo.OrderStatus = (RsOrderStatusEnum)orderStatus;

        if (orderInfo.OrderStatus == RsOrderStatusEnum.Canceled)
        {
            orderInfo.CancelDate = DU.Utility.GetBdDateTimeNow();
            orderInfo.CancelById = CurrentUserId;
        }
        else if (orderInfo.OrderStatus == RsOrderStatusEnum.Served)
        {
            orderInfo.RsFoodOrderItems.ForEach(x =>
            {
                x.IsServed = true;
                x.ServedTime = DU.Utility.GetBdDateTimeNow();
                x.ServedById = CurrentUserId;
            });
        }

        await _iRepository.UpdateAsync(orderInfo);
        var isUpdate = await _iUnitOfWork.CompleteAsync();
        if (!isUpdate) { return false; }
        return true;
    }

    #endregion

    #region OrderFoodItemWiseIngredientHst

    private async Task<List<RsFoodIngredientsHst>> FoodItemWiseIngredientHst(long itemId, long orderItemId, double qty)
    {
        var data = await _iFoodItemRepository.GetFirstOrDefaultAsync(x => x.Id == itemId && !x.IsDeleted);

        if (data == null)
            throw new Exception("Food Item not found...!!");

        var historyList = new List<RsFoodIngredientsHst>();

        var foodIngredientList = await _iFoodIngredientRepository.GetAsync(x => x.FoodItemId == itemId && !x.IsDeleted, i => i.Item);

        if (foodIngredientList.Count > 0)
        {
            var ingredientItemIds = foodIngredientList.Select(x => x.ItemId).ToList();
            var itemConvertionList = await _iItemConvertionRepository.GetAsync(x => ingredientItemIds.Contains(x.Id) && x.IsActive && !x.IsDeleted);
            var stockList = await _iInvReportRepository.GetInventoryStockInfo(new StockVm());

            foreach (var ingredient in foodIngredientList)
            {
                var model = new RsFoodIngredientsHst();

                model.OrderItemId = orderItemId;
                model.FoodItemId = data.Id;
                model.ItemId = ingredient.Id;
                model.UnitId = ingredient.UnitId;
                model.Quantity = ingredient.Quantity * qty;
                model.Price = ingredient.Price;
                model.Amount = model.Quantity * model.Price;
                model.ActionById = CurrentUserId;
                model.ActionDate = DateTime.Now;
                model.IsActive = true;

                if (ingredient.Item.UnitId != model.UnitId)
                {
                    var itemConvertion = itemConvertionList.FirstOrDefault(x => x.ItemId == model.ItemId
                    && x.UnitId == ingredient.Item.UnitId && x.ConvertedUnitId == model.UnitId);

                    var itemUnitPrice = stockList.FirstOrDefault(x => x.ItemId == model.ItemId)?.UnitPrice ?? 0;

                    model.SysPrice = itemUnitPrice > 0 && itemConvertion != null
                        ? itemUnitPrice / itemConvertion.ConvertedQuantity : 0;

                    model.SysAmount = model.Quantity * model.SysPrice;
                }

                historyList.Add(model);
            }
        }

        return historyList;
    }

    #endregion

    #region RoomAssign

    public async Task<bool> RoomAssign(AssignRoomVm dto)
    {
        if (dto == null || !(dto.OrderId > 0))
            throw new Exception("Billing Information Is Not Correct...!!");

        var bill = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == dto.OrderId && !x.IsDeleted);

        if (bill == null)
            throw new Exception("Bill Not Found...!!");

        if (bill.PaymentStatus == RsOrderPaymentStatusEnum.FullPayment)
            throw new Exception("Food Bill Already Full Paid...!!");

        var hotelCustomerType = await _iCustomerTypeRepository.GetFirstOrDefaultAsync(x => x.TypeCode == RsCustomerTypeCode.Hotel && !x.IsDeleted);
        if (hotelCustomerType == null)
            throw new Exception("Hotel Type Not Found...!!");

        if (dto.RoomId > 0)
        {
            var bookingRoomInfo = await _iBookingRoomRepository.GetBookedRoomInfoByRoomIdAsync(dto.RoomId);
            if (bookingRoomInfo == null)
                throw new Exception("No Booking Room Information Found...!!");

            bill.RoomId = dto.RoomId;
            bill.CustomerId = dto.CustomerId;
            bill.CustomerTypeId = hotelCustomerType.Id;
            bill.IsRoomService = dto.IsRoomService;
            bill.BookingId = bookingRoomInfo.BookingId;
        }

        if (dto.IsRoomService)
        {
            var serviceCharge = Utility.PercentCalculation(10, bill.OrderAmount);
            bill.ServiceCharge = serviceCharge;
        }
        else
        {
            bill.ServiceCharge = 0;
        }

        bill.NetAmount = (bill.OrderAmount + bill.VAT + bill.TAX + bill.ServiceCharge) - (bill.Discount);

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iRepository.UpdateAsync(bill);
        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }

    #endregion#region RoomAssign

    #region Customer Type Update

    public async Task<bool> CusomerTypeUpdateAsync(CustomerTypeUpdateDto dto)
    {
        if (dto == null || !(dto.OrderId > 0))
            throw new Exception("Order Information Is Not Correct...!!");

        var order = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == dto.OrderId && !x.IsDeleted);

        if (order == null)
            throw new Exception("Bill Not Found...!!");

        if (order.PaymentStatus == RsOrderPaymentStatusEnum.FullPayment)
            throw new Exception("Food Bill Already Full Paid...!!");


        if (dto.CustomerTypeId > 0)
        {
            var customerType = await _iCustomerTypeRepository.GetFirstOrDefaultAsync(x => x.Id == dto.CustomerTypeId && !x.IsDeleted);
            if (customerType == null)
                throw new Exception("Customer Type Not Found...!!");

            order.CustomerTypeId = dto.CustomerTypeId;

            if (customerType.DiscountPercent > -1)
            {
                var discount = Utility.PercentCalculation(customerType.DiscountPercent, order.OrderAmount);
                order.Discount = discount;
                order.NetAmount = (order.OrderAmount + order.ServiceCharge) - discount;
            }
        }


        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iRepository.UpdateAsync(order);
        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }

    #endregion

    #region Customer Name Update

    public async Task<bool> CustomerNameUpdateAsync(CustomerNameUpdateDto dto)
    {
        if (dto == null || !(dto.OrderId > 0))
            throw new Exception("Order Information Is Not Correct...!!");

        var order = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == dto.OrderId && !x.IsDeleted);

        if (order == null)
            throw new Exception("Order Not Found...!!");

        if (dto.CustomerId > 0 && dto.CustomerTypeId > 0)
        {
            var customerType = await _iCustomerTypeRepository.GetFirstOrDefaultAsync(x => x.Id == dto.CustomerTypeId && !x.IsDeleted);
            if (customerType == null)
                throw new Exception("Customer Type Not Found...!!");

            var customer = await _iCustomerRepository.GetFirstOrDefaultAsync(x => x.Id == dto.CustomerId && !x.IsDeleted);
            if (customer == null)
                throw new Exception("Customer Type Not Found...!!");

            if (customerType.TypeCode == RsCustomerTypeCode.Employee)
            {
                if (!(customer.EmployeeId > 0))
                    throw new Exception("Selected Customer Is Not Employee Type... But Customer Type Is Employee...!!");
            }

            order.CustomerId = dto.CustomerId;
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iRepository.UpdateAsync(order);
        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }

    #endregion

    #region PaymentRemove

    public async Task<bool> PaymentRemoveAsync(long paymentId)
    {
        var paymentData = await _iOrderPaymentRepository.GetFirstOrDefaultAsync(x => x.Id == paymentId && !x.IsDeleted, o => o.Order);

        if (paymentData == null)
            throw new Exception("No Payment Information Found");
        var paymentVoucher = await _iAccTranMstRepository.GetFirstOrDefaultAsync(x => x.RsPaymentId == paymentData.Id && !x.IsDeleted, o => o.AccTranDtls);

        var orderPaidList = await _iOrderPaymentRepository.GetAsync(x => x.OrderId == paymentData.OrderId && !x.IsDeleted);

        var order = paymentData.Order;

        var remainPayment = orderPaidList.Where(x => x.Id != paymentData.Id).Sum(x => x.PaidAmount);

        if (order.NetAmount == remainPayment)
        {
            order.PaymentStatus = RsOrderPaymentStatusEnum.FullPayment;
        }
        if (order.NetAmount > remainPayment && remainPayment > 0)
        {
            order.PaymentStatus = RsOrderPaymentStatusEnum.PartialPayment;
        }
        if (remainPayment == 0)
        {
            order.PaymentStatus = RsOrderPaymentStatusEnum.Pending;
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (paymentVoucher != null)
        {
            _iAccTranDtlRepository.RemoveRange(paymentVoucher.AccTranDtls);
            _iAccTranMstRepository.Remove(paymentVoucher);
        }

        //order = null;

        _iOrderPaymentRepository.Remove(paymentData);



        await _iRepository.UpdateAsync(order);

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }


    #endregion

    #region OrderRemove

    public async Task<bool> OrderRemoveAsync(long orderId)
    {
        var order = await _iRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new Exception("No Payment Information Found");

        var orderItems = await _iFoodOrderItemRepository.GetAsync(x => x.OrderId == order.Id);

        var orderPaidList = await _iOrderPaymentRepository.GetAsync(x => x.OrderId == order.Id);
        var paymentIds = orderPaidList.Select(x => x.Id).ToList();

        var vouchers = await _iAccTranMstRepository.GetAsync(x => paymentIds.Contains((long)x.RsPaymentId), o => o.AccTranDtls);

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

        if (orderPaidList != null && orderPaidList.Count > 0)
        {
            _iOrderPaymentRepository.RemoveRange(orderPaidList);
        }

        if (orderItems != null && orderItems.Count > 0)
        {
            _iFoodOrderItemRepository.RemoveRange(orderItems);
        }

        _iRepository.Remove(order);

        var isExecuted = await _iUnitOfWork.CompleteAsync();
        if (!isExecuted) { return false; }
        ts.Complete();
        return true;
    }


    #endregion

    #region RsPaymentTransectionReport
    public async Task<string> RsPaymentTransectionReportHtml(RsTransectionReportVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        var data = await _iRepository.RsPaymentTransectionReportAsync(vm);
        data = data.OrderBy(x => x.OrderNo).ToList();

        vm.StrFromDate = string.IsNullOrEmpty(vm.StrFromDate)
            ? DateTime.Today.ToString("dd/MM/yyyy")
            : vm.StrFromDate;

        vm.StrToDate = string.IsNullOrEmpty(vm.StrToDate)
            ? DateTime.Today.ToString("dd/MM/yyyy")
            : vm.StrToDate;

        var queryFromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate));
        var queryToDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate));

        if (data == null || data.Count == 0)
            return "<h5 style='text-align:center;color:red;'>No Records Found</h5>";

        if (isPrint)
        {
            fullHtml += $@"<h6 style='text-align:center;padding-bottom:5px;'>
                        Date: {queryFromDate:dd/MM/yyyy} To {queryToDate:dd/MM/yyyy}
                      </h6>";
        }
        string style = @"<style>
                            @media print {
                                .print-flex {
                                    display: block !important;
                                    text-align: center !important;
                                }
                                .print-flex table {
                                    display: inline-block !important;
                                    vertical-align: top !important;
                                    margin-right: 50px !important;
                                }
                            }
                        </style>";


        // ========= GROUPS =========
        var restaurantGroup = data.Where(x => x.BillDtlId == null).ToList();
        var frontdeskGroup = data.Where(x => x.BillDtlId != null).ToList();

        double restaurantTotal = 0;
        double frontdeskTotal = 0;

        // ========= MAIN TABLE =========
        fullHtml += @"<table class='table report-table table-hover' border='1' style='width:100%;'>";

        fullHtml += $@"<thead>
                    <tr><th colspan='14' style='text-align:center;'>
                        <b>Date: {queryFromDate:dd/MM/yyyy} To {queryToDate:dd/MM/yyyy}</b>
                    </th></tr>

                    <tr style='font-size:10px;'>
                        <th>SL</th>
                        <th>Order No</th>
                        <th >Customer</th>
                        <th >Customer Type</th>
                        <th >Waiter</th>
                        <th>Room</th>
                        <th>Table</th>
                        <th>Pay Mode</th>
                        <th>Paid Date</th>
                        <th>Order Amount</th>
                        <th>Net Amount</th>
                        <th>Paid Amount</th>
                        <th>Collection</th>
                        <th>Remarks</th>
                    </tr>
                  </thead>";

        fullHtml += "<tbody>";

        // ========== RESTAURANT SECTION ==========
        if (restaurantGroup.Count > 0)
        {
            fullHtml += @"<tr>
                        <td colspan='14' style='background:#dfe6e9;font-weight:600;text-align:center;'>
                            RESTAURANT COLLECTION
                        </td>
                      </tr>";

            int i = 1;
            foreach (var obj in restaurantGroup)
            {
                restaurantTotal += obj.PaidAmount;

                string orderLink = isPrint ? obj.OrderNo :
                    $"<a target='_blank' href='../FoodOrder/Details/{obj.OrderId}'>{obj.OrderNo}</a>";

                fullHtml += $@"<tr>
                            <td style='text-align:center;'>{i++}</td>
                            <td>{orderLink}<br/>{obj.OrderDate:dd/MMM/yyyy}</td>
                            <td style='text-align:center;'><b>{obj.CustomerName}</b><br/>{obj.Mobile}</td>
                            <td style='text-align:center;'>{obj.CustomerType}</td>
                            <td style='text-align:center;'>{obj.WaiterName}</td>
                            <td>{obj.RoomNo}</td>
                            <td>{obj.TableNo}</td>
                            <td>{((PayModeEnum)obj.PayMode).GetDescription()}</td>
                            <td>{obj.PaidDate:dd/MMM/yyyy}</td>
                            <td style='text-align:right;'>{obj.OrderAmount:N2}</td>
                            <td style='text-align:right;'>{obj.NetAmount:N2}</td>
                            <td style='text-align:right;'>{obj.PaidAmount:N2}</td>
                            <td style='text-align:center;'>Restaurant</td>
                            <td>{obj.Remarks}</td>
                          </tr>";
            }

            fullHtml += $@"<tr style='font-weight:600;background:#f8f9fa;'>
                        <td colspan='11' style='text-align:right;'>Subtotal (Restaurant)</td>
                        <td style='text-align:right;'>{restaurantTotal:N2}</td>
                        <td colspan='2'></td>
                       </tr>";
        }


        // ========== FRONT DESK SECTION ==========
        if (frontdeskGroup.Count > 0)
        {
            fullHtml += @"<tr>
                        <td colspan='14' style='background:#dfe6e9;font-weight:600;text-align:center;'>
                            FRONT DESK COLLECTION
                        </td>
                      </tr>";

            int i = 1;
            foreach (var obj in frontdeskGroup)
            {
                frontdeskTotal += obj.PaidAmount;

                string orderLink = isPrint ? obj.OrderNo :
                    $"<a target='_blank' href='../FoodOrder/Details/{obj.OrderId}'>{obj.OrderNo}</a>";

                fullHtml += $@"<tr>
                            <td style='text-align:center;'>{i++}</td>
                            <td>{orderLink}<br/>{obj.OrderDate:dd/MMM/yyyy}</td>
                            <td><b>{obj.CustomerName}</b><br/>{obj.Mobile}</td>
                            <td>{obj.CustomerType}</td>
                            <td>{obj.WaiterName}</td>
                            <td>{obj.RoomNo}</td>
                            <td>{obj.TableNo}</td>
                            <td>{((PayModeEnum)obj.PayMode).GetDescription()}</td>
                            <td>{obj.PaidDate:dd/MMM/yyyy}</td>
                            <td style='text-align:right;'>{obj.OrderAmount:N2}</td>
                            <td style='text-align:right;'>{obj.NetAmount:N2}</td>
                            <td style='text-align:right;'>{obj.PaidAmount:N2}</td>
                            <td style='text-align:center;'>Front-Desk</td>
                            <td>{obj.Remarks}</td>
                          </tr>";
            }

            fullHtml += $@"<tr style='font-weight:600;background:#f8f9fa;'>
                        <td colspan='11' style='text-align:right;'>Subtotal (Front-Desk)</td>
                        <td style='text-align:right;'>{frontdeskTotal:N2}</td>
                        <td colspan='2'></td>
                       </tr>";
        }

        fullHtml += "</tbody></table>";

        // Restaurant
        double resCash = restaurantGroup.Where(x => x.PayMode == (int)PayModeEnum.Cash).Sum(x => x.PaidAmount);
        double resCard = restaurantGroup.Where(x => x.PayMode == (int)PayModeEnum.Card).Sum(x => x.PaidAmount);
        double resBank = restaurantGroup.Where(x => x.PayMode == (int)PayModeEnum.Bank).Sum(x => x.PaidAmount);
        double resBkash = restaurantGroup.Where(x => x.PayMode == (int)PayModeEnum.Bkash).Sum(x => x.PaidAmount);

        // Front Desk
        double fdCash = frontdeskGroup.Where(x => x.PayMode == (int)PayModeEnum.Cash).Sum(x => x.PaidAmount);
        double fdCard = frontdeskGroup.Where(x => x.PayMode == (int)PayModeEnum.Card).Sum(x => x.PaidAmount);
        double fdBank = frontdeskGroup.Where(x => x.PayMode == (int)PayModeEnum.Bank).Sum(x => x.PaidAmount);
        double fdBkash = frontdeskGroup.Where(x => x.PayMode == (int)PayModeEnum.Bkash).Sum(x => x.PaidAmount);

        fullHtml += style;
        fullHtml += @"<div class='print-flex' style='display:flex;gap:50px;justify-content:center;margin-top:20px;'>";


        // ========== RESTAURANT ==============
        fullHtml += @"<table border='1' style='width:250px;' class='report-table'>
                            <thead>
                                <tr><th colspan='2' style='text-align:center;'>Restaurant Summary</th></tr>
                            </thead>
                       <tbody>";

        fullHtml += $@"<tr><td>Cash</td><td style='text-align:right;'>{resCash:N2}</td></tr>
                        <tr><td>Card</td><td style='text-align:right;'>{resCard:N2}</td></tr>
                        <tr><td>Bank</td><td style='text-align:right;'>{resBank:N2}</td></tr>
                        <tr><td>M-Banking</td><td style='text-align:right;'>{resBkash:N2}</td></tr>

                        <tr style='font-weight:600;background:#f8f9fa;'>
                            <td>Total Restaurant</td>
                            <td style='text-align:right;'>{restaurantTotal:N2}</td>
                        </tr>";

        fullHtml += @"</tbody></table>";



        // ========== FRONT DESK ==============
        fullHtml += @"<table border='1' style='width:250px;' class='report-table'>
                            <thead>
                                <tr><th colspan='2' style='text-align:center;'>Front Desk Summary</th></tr>
                            </thead>
                        <tbody>";

        fullHtml += $@"<tr><td>Cash</td><td style='text-align:right;'>{fdCash:N2}</td></tr>
                        <tr><td>Card</td><td style='text-align:right;'>{fdCard:N2}</td></tr>
                        <tr><td>Bank</td><td style='text-align:right;'>{fdBank:N2}</td></tr>
                        <tr><td>M-Banking</td><td style='text-align:right;'>{fdBkash:N2}</td></tr>

                        <tr style='font-weight:600;background:#f8f9fa;'>
                            <td>Total Front Desk</td>
                            <td style='text-align:right;'>{frontdeskTotal:N2}</td>
                        </tr>";

        fullHtml += @"</tbody></table>";

        fullHtml += @"</div>";

        return fullHtml;
    }

    #endregion
}
