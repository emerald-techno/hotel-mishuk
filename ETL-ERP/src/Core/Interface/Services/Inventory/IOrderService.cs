using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.BillPayment;
using Domain.ViewModel.Inventory.Order;
using Domain.ViewModel.Inventory.Receive;
using Interface.Base;

namespace Interface.Services.Inventory;

public interface IOrderService : IService<OrderMst>
{
    Task<bool> AddAsync(OrderVm vm);
    Task<DataTablePagination<OrderSearchVm, OrderSearchVm>>
        SearchAsync(DataTablePagination<OrderSearchVm, OrderSearchVm> model);
    Task<OrderVm> GetOrderDataAsync(long id);
    dynamic GetOrderDynamicDataBySupplier(long? supplierId);
    dynamic GetReceivedOrderDynamicDataBySupplier(long? supplierId);
    double GetReceiveItemTotalRateByOrder(long orderId);
    bool MakeOrderComplete(OrderCompletionVm vm);
    Task<bool> ReviewUpdate(OrderApprovalVm vm, string ntfLink);
    Task<string> GetOrderCode();
    Task<List<ReceiveDtlVm>> GetOrderItemForReceive(long orderId);
    Task<InvBillSaveVm> GetBillDataAsync(long orderId);
    Task<bool> OrderPayBill(InventoryBillPaymentVm vm);
    Task<string> GetOrderByIdAsyncHtml(long id);
}
