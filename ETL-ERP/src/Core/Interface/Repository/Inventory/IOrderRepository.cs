using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.BillPayment;
using Domain.ViewModel.Inventory.Order;
using Domain.ViewModel.Inventory.Receive;
using Interface.Base;

namespace Interface.Repository.Inventory;

public interface IOrderRepository : IRepository<OrderMst>
{
    Task<DataTablePagination<OrderSearchVm, OrderSearchVm>>
        SearchAsync(DataTablePagination<OrderSearchVm, OrderSearchVm> model);
    Task<OrderVm> GetOrderByIdAsync(long id);
    Task<List<ReceiveDtlVm>> GetOrderItemForReceive(long orderId);
    Task<InvBillSaveVm> GenerateBill(long orderId);
}
