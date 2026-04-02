using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.BillPayment;
using Interface.Base;

namespace Interface.Services.Inventory;

public interface IInventoryBillPaymentService : IService<InventoryBillPayment>
{
    Task<InventoryBillPaymentDetails> GetBillPaymentDataAsync(long id);

    Task<DataTablePagination<InventoryBillPaymentSearchVm, InventoryBillPaymentSearchVm>>
            SearchAsync(DataTablePagination<InventoryBillPaymentSearchVm, InventoryBillPaymentSearchVm> model);
}
