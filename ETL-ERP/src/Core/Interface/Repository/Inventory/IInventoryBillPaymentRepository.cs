using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.BillPayment;
using Interface.Base;

namespace Interface.Repository.Inventory;

public interface IInventoryBillPaymentRepository : IRepository<InventoryBillPayment>
{
    Task<InventoryBillPaymentDetails> GetBillPaymentByIdAsync(long id);

    Task<DataTablePagination<InventoryBillPaymentSearchVm, InventoryBillPaymentSearchVm>>
            SearchAsync(DataTablePagination<InventoryBillPaymentSearchVm, InventoryBillPaymentSearchVm> vm);
}
