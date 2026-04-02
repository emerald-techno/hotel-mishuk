using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.BillPayment;
using Domain.ViewModel.Inventory.Order;
using Domain.ViewModel.Inventory.Receive;
using Interface.Repository.Common;
using Interface.Repository.Inventory;
using Interface.Services.Inventory;
using Interface.Services.Notification;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;
using DU = Domain.Utility;

namespace Services.Inventory;

public class OrderService : BaseService<OrderMst>, IOrderService
{
    private IOrderRepository Repository { get; }
    private readonly IMapper _iMapper;
    private readonly ITranService _tranService;
    private readonly IOrderDtlService _iOrderDtlService;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IAutoCodeRepository _iAutoCodeRepository;
    private readonly IInventoryBillPaymentRepository _iInventoryBillPaymentRepository;

    private readonly INtfNotificationMsgService _iNtfMsgService;

    public OrderService(IOrderRepository iRepository, IMapper iMapper,
                        ITranService tranService, 
                        IOrderDtlService orderDtlService, 
                        IUnitOfWork iUnitOfWork, 
                        IAutoCodeRepository iAutoCodeRepository, 
                        IInventoryBillPaymentRepository iInventoryBillPaymentRepository,
                        INtfNotificationMsgService iNtfMsgService) : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _tranService = tranService;
        _iOrderDtlService = orderDtlService;
        _iUnitOfWork = iUnitOfWork;
        _iAutoCodeRepository = iAutoCodeRepository;
        _iInventoryBillPaymentRepository = iInventoryBillPaymentRepository;
        _iNtfMsgService = iNtfMsgService;
    }

    public async Task<bool> AddAsync(OrderVm vm)
    {
        var orderModel = _iMapper.Map<OrderMst>(vm);
        var orderModelDtls = _iMapper.Map<List<OrderDtl>>(vm.OrderDtls);

        orderModel.OrderDate = (DateTime)(!string.IsNullOrEmpty(vm.OrderDateStr) ? DU.Utility.ConvertStrToDate(vm.OrderDateStr) : vm.OrderDate);
        orderModel.DeliveryDeadline = !string.IsNullOrEmpty(vm.DeliveryDeadlineStr) ? DU.Utility.ConvertStrToDate(vm.DeliveryDeadlineStr) : null;

        orderModel.Status = (short)OrderStatusEnum.FRESH;
        orderModel.ReceiveStatus = (short)OrderReceiveStatusEnum.NOT;

        //Note here: 'OrderById' and 'SubmitById' not present in OrderMst Entity Model

        orderModel.ActionById = CurrentUserId;
        orderModel.ActionDate = DU.Utility.GetBdDateTimeNow();

        if (orderModelDtls != null && orderModelDtls.Count() > 0)
        {
            var i = 0;

            foreach (var item in orderModelDtls)
            {
                item.ActionById = CurrentUserId;
                item.ActionDate = DU.Utility.GetBdDateTimeNow();
                item.SlNo = i++;
            }
        }

        orderModel.OrderDtls = null;

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await Repository.AddAsync(orderModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();

        orderModelDtls.ForEach(x => x.OrderId = orderModel.Id);

        var isDtlAdded = await _iOrderDtlService.AddRangeAsync(orderModelDtls);

        if (!isAdded || !isDtlAdded)
        {
            return false;
        }

        ts.Complete();
        return true;
    }


    public async Task<DataTablePagination<OrderSearchVm, OrderSearchVm>>
        SearchAsync(DataTablePagination<OrderSearchVm, OrderSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    public async Task<OrderVm> GetOrderDataAsync(long id)
    {
        var data = await Repository.GetOrderByIdAsync(id);
        return data;
    }

    public dynamic GetOrderDynamicDataBySupplier(long? supplierId)
    {
        var dataList = Repository.Get(c => c.SupplierId == supplierId && !c.IsDeleted);
        var dynamicData = dataList.Select(c => new { c.Id, Name = $"{c.OrderNo}-({DU.Utility.GetDate(c.OrderDate)})" });
        return dynamicData;
    }

    public dynamic GetReceivedOrderDynamicDataBySupplier(long? supplierId)
    {
        var receivedOrderIds = _tranService.Get(c => c.SupplierId == supplierId && !c.IsDeleted && c.TranType == TransType.Receive).Select(c => c.OrderId);
        var dataList = Repository.Get(c => c.SupplierId == supplierId && !c.IsDeleted && receivedOrderIds.Contains(c.Id));
        var dynamicData = dataList.Select(c => new { c.Id, Name = $"{c.OrderNo}-({DU.Utility.GetDate(c.OrderDate)})" });
        return dynamicData;
    }

    public double GetReceiveItemTotalRateByOrder(long orderId)
    {
        var order = Repository.GetFirstOrDefault(c => c.Id == orderId, i => i.OrderDtls);
        var receiveOrders = _tranService.Get(c => c.OrderId == orderId && c.TranType == TransType.Receive && !c.IsDeleted, i => i.TranDtls);
        var receivedOrderDetails = receiveOrders.SelectMany(c => c.TranDtls).ToList();

        double result = 0;

        foreach (var item in receivedOrderDetails)
        {
            var orderDtlData = order.OrderDtls.FirstOrDefault(c => c.ItemId == item.ItemId);

            if (orderDtlData != null)
            {
                result += item.ItemQty * orderDtlData.Rate;
            }
        }

        return result;
    }

    public bool MakeOrderComplete(OrderCompletionVm vm)
    {
        if (vm.Id == 0 || vm.OrderCompletionDtls == null || vm.OrderCompletionDtls.Count <= 0)
        {
            throw new Exception("Sorry! No Item detail found! ");
        }

        var orderModel = Repository.GetById(vm.Id);
        orderModel.CompleteDate = vm.CompleteDate;
        orderModel.CompleteRemarks = vm.CompleteRemarks;

        var isOrderDtlUpdated = false;

        var orderDtls = orderModel.OrderDtls;

        List<OrderDtl> updatableItems = null;

        var existingDetails = _iOrderDtlService.Get(c => c.OrderId == orderModel.Id).ToList();

        if (existingDetails?.Count > 0)
        {
            foreach (var item in existingDetails)
            {
                var filterExistData = vm.OrderCompletionDtls.FirstOrDefault(c => c.Id == item.Id);
                item.ActualAmount = filterExistData.ActualAmount;
            }
            updatableItems = _iMapper.Map<List<OrderDtl>>(existingDetails);
        }

        using (var ts = new TransactionScope())
        {
            var isUpdated = Update(orderModel);

            if (updatableItems?.Count > 0)
            {
                _iOrderDtlService.UpdateRange(updatableItems);
                isOrderDtlUpdated = _iUnitOfWork.Complete();
            }

            if (isUpdated && isOrderDtlUpdated)
            {
                ts.Complete();
                return true;
            }
        }
        return false;
    }

    public async Task<string> GetOrderCode()
    {
        var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.OrderMsts.ToString(), "OrderNo", "PO", 6);
        return data;
    }

    public async Task<bool> ReviewUpdate(OrderApprovalVm vm, string ntfLink)
    {
        if (vm.Id == 0 || vm.ApprovalDtls == null || vm.ApprovalDtls.Count <= 0)
        {
            throw new Exception("Sorry! No Item details Found !");
        }

        var orderModel = Repository.GetById(vm.Id);
        orderModel.Status = vm.Status;
        if (orderModel.Status == (short)OrderStatusEnum.APPROVED)
        {
            orderModel.ActionById = CurrentUserId;
            orderModel.ApprovedDate = DU.Utility.GetBdDateTimeNow();
        }
        var isDetailUpdated = false;

        //var reqDetails = orderModel.OrderDtls;

        List<OrderDtl> updateableItems = null;
        var existingDetails = _iOrderDtlService.Get(c => c.OrderId == orderModel.Id).ToList();

        if (existingDetails?.Count > 0)
        {
            foreach (var item in existingDetails)
            {
                var filterExistingData = vm.ApprovalDtls.FirstOrDefault(c => c.Id == item.Id);
                item.AprOrderQty = filterExistingData.AprOrderQty;
            }

            updateableItems = _iMapper.Map<List<OrderDtl>>(existingDetails);
        }
        using (var ts = new TransactionScope())
        {
            var isUpdated = Update(orderModel);
            if (updateableItems?.Count > 0)
            {
                _iOrderDtlService.UpdateRange(updateableItems);
                isDetailUpdated = _iUnitOfWork.Complete();
            }
            if (isUpdated && isDetailUpdated)
            {
                string ntfMsgHtml = @$"<a href='{ntfLink}' target='_blank'><b>Order {orderModel.OrderNo}-({orderModel.OrderDate.ToString("dd/MM/yyyy")}) Review Approved</b></a>";
                string emailMsg = $@"Order {orderModel.OrderNo}-({orderModel.OrderDate.ToString("dd/MM/yyyy")}) Review Approved";

                var ntfGenerated = await _iNtfMsgService.GenerateNtf(NotificationEventCode.OrderApproveNtf, ntfMsgHtml, emailMsg);

                ts.Complete();
                return true;
            }
        }
        return false;
    }

    public async Task<List<ReceiveDtlVm>> GetOrderItemForReceive(long orderId)
    {
        var dataList = await Repository.GetOrderItemForReceive(orderId);
        return dataList;
    }

    public async Task<InvBillSaveVm> GetBillDataAsync(long orderId)
    {
        var data = await Repository.GenerateBill(orderId);
        return data;
    }

    #region OrderPayBill

    public async Task<bool> OrderPayBill(InventoryBillPaymentVm vm)
    {
        try
        {
            if (vm == null && !(vm.OrderMstId > 0) && !(vm.BillAmount > 0))
                throw new Exception("Information is not correct..!!");

            var paymentModel = _iMapper.Map<InventoryBillPayment>(vm);
            paymentModel.BillNo = await GetBillNo();
            paymentModel.BillDate = Utility.GetBdDateTimeNow();
            paymentModel.BillById = CurrentUserId;
            paymentModel.ActionById = CurrentUserId;
            paymentModel.ActionDate = Utility.GetBdDateTimeNow();

            //var paidList = _iInventoryBillPaymentRepository.Get(c => c.OrderMstId == vm.OrderMstId).ToList();
            //var alreadyPaidAmount = paidList.Sum(x => x.BillAmount);

            //if (vm.BillAmount > alreadyPaidAmount)
            //    throw new Exception("Given Amount Is Higher Than Net Amount...!!");

            //var paymentVoucher = await GetPaymentVoucher(paymentModel, bookingService.BookingNo, bill.BillNumber, alreadyPaidAmount: alreadyPaidAmount);
            //if (paymentVoucher == null)
            //    throw new Exception("Somthing Went Wrong Creating Voucher..!!");

            using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await _iInventoryBillPaymentRepository.AddAsync(paymentModel);
            //await _iUnitOfWork.CompleteAsync();

            //paymentVoucher.PaymentId = paymentModel.Id;
            //await _iAccTranMstRepository.AddAsync(paymentVoucher);

            var isExecuted = await _iUnitOfWork.CompleteAsync();
            if (!isExecuted) return false;
            ts.Complete();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
        
    }

    public async Task<string> GetBillNo()
    {
        var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.InventoryBillPayments.ToString(), "BillNo", "INV", 6);
        return data;
    }

    #endregion

    #region OrderHtml

    public async Task<string> GetOrderByIdAsyncHtml(long id)
    {
        try
        {
            var data = await GetOrderDataAsync(id);

            var fullHtml = "";
            fullHtml += "<div style='padding-top:5px'>";
            fullHtml += $@"<table class='master-table'>
                                <tbody>
                                    <tr>
                                        <td style='width:12%;'><b>Order By</b></td>
                                        <td style='width:38%'>: {data.ActionBy}</td>
                                        <td style='width:12%;'><b>Order No</b></td>
                                        <td style='width:38%'>: {data.OrderNo}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Supplier</b></td>
                                        <td style='width:38%'>: {data.SupplierName}<br/>
                                            {data.SupplierMobile}
                                        </td>
                                        <td style='width:12%;'><b>Order Date</b></td>
                                        <td style='width:38%'>: {data.OrderDate.ToString("dd-MMM-yyyy")}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Remarks</b></td>
                                        <td style='width:38%;'>: {data.Remarks}</td>
                                        <td style='width:12%;'><b>Status</b></td>
                                        <td style='width:38%'>: {data.StatusText}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Complitation Remarks</b></td>
                                        <td style='width:38%;'>: {data.CompleteRemarks}</td>
                                        <td style='width:12%;'><b>Receive Status</b></td>
                                        <td style='width:38%'>: {data.ReceiveStatusText}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:12%;'><b>Requsition No.</b></td>
                                        <td style='width:38%'>: {data.RequsitionNo}</td>
                                        <td style='width:12%;'><b>Complete Date</b></td>
                                        <td style='width:38%'>: {data.CompleteDate?.ToString("dd-MMM-yyyy")}</td>
                                    </tr>
                                </tbody>
                            </table>";

            fullHtml += "</div>";

            //Order Item
            fullHtml += "<div style='padding-top:15px;'><p>Item List :</p></div>";
            fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center;margin-top:5px;font-size:11px'>";

            fullHtml += "<thead>";
            fullHtml += "<tr>";
            fullHtml += "<th style='width:5%'>Sl No</th>";
            fullHtml += "<th style='width:15%'>Item Name</th>";
            fullHtml += "<th style='width:15%'>Unit</th>";
            fullHtml += "<th style='width:10%'>Order Qty</th>";
            fullHtml += "<th style='width:10%'>Approve Qty</th>";
            fullHtml += "<th style='width:10%'>Rate</th>";
            fullHtml += "<th style='width:10%'>Cost</th>";
            fullHtml += "<th style='width:15%'>Approval Cost</th>";
            fullHtml += "</tr>";

            fullHtml += "</thead>";

            fullHtml += "<tbody>";

            if (data.OrderDtls.Count > 0)
            {
                double totalCost = 0;
                double totalActualCost = 0;
                double totalAprCost = 0;

                foreach (var (item, i) in data.OrderDtls.GetItemWithIndex())
                {
                    double itemCost = item.OrderQty * item.Rate;
                    totalCost += itemCost;
                    totalActualCost += item.ActualAmount;

                    double actualCost = item.AprOrderQty * item.Rate;
                    totalAprCost += actualCost;

                    fullHtml += "<tr>";

                    fullHtml += $@"<td class='text-center'>{i + 1} </td>";
                    fullHtml += $@"<td class='text-start'>{item.ItemName}</td>";
                    fullHtml += $@"<td class='text-start'>{item.ItemUnitName}</td>";
                    fullHtml += $@"<td class='text-center'>{item.OrderQty}</td>";
                    fullHtml += $@"<td class='text-center'>{item.AprOrderQty}</td>";
                    fullHtml += $@"<td class='text-end'>{item.Rate}</td>";
                    fullHtml += $@"<td class='text-end'>{itemCost}</td>";
                    fullHtml += $@"<td class='text-end'>{actualCost}</td>";

                    fullHtml += "</tr>";
                }

                fullHtml += "<tr>";
                fullHtml += $@"<td colspan='6' class='text-end'><b>Total</b></td>";
                fullHtml += $@"<td class='text-end'><b>{totalCost.ToString("N2")}</b></td>";
                fullHtml += $@"<td class='text-end'><b>{totalAprCost.ToString("N2")}</b></td>";
                fullHtml += "</tr>";
            }

            fullHtml += "</tbody>";
            fullHtml += "</table>";

            return fullHtml;
        }
        catch (Exception ex)        {

            throw;
        }
    }

    #endregion
}
