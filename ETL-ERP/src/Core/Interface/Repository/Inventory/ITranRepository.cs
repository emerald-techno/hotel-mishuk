using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.Transaction;
using Interface.Base;

namespace Interface.Repository.Inventory
{
    public interface ITranRepository : IRepository<TranMst>
    {
        Task<DataTablePagination<TransactionSearchVm, TransactionSearchVm>>
            SearchAsync(DataTablePagination<TransactionSearchVm, TransactionSearchVm> model);

        Task<DataTablePagination<TransactionSearchVm, TransactionSearchVm>>
            SearchLOAsync(DataTablePagination<TransactionSearchVm, TransactionSearchVm> model);

        Task<TransactionVm> GetTransByIdAsync(long id);
        Task<TransactionVm?> GetOpeningData();
        Task<TransactionVm> GetOpeningDataByDept(long id);
        Task<List<DirectReceiveReportVm>> GetDirectReceiveReportData(DirectReceiveReportVm vm);
    }
}
