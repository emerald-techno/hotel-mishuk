using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.Transaction;
using Interface.Base;

namespace Interface.Services.Inventory;

public interface ITranService : IService<TranMst>
{
    Task<bool> AddAsync(TransactionVm vm);

    Task<DataTablePagination<TransactionSearchVm, TransactionSearchVm>>
        SearchAsync(DataTablePagination<TransactionSearchVm, TransactionSearchVm> model);

    Task<DataTablePagination<TransactionSearchVm, TransactionSearchVm>>
        SearchLOAsync(DataTablePagination<TransactionSearchVm, TransactionSearchVm> model);
    List<TransactionDtlVm> GetReceiveDetailsByOrderIdAsync(long id);
    Task<string> GetTransAutoCode(string tranType);
    Task<TransactionVm> GetTransInfoDataAsync(long id);
    Task<bool> IssueEntryAsync(TransactionVm vm);
    Task<long> AddOpening(TransactionVm vm);
    Task<long> UpdateOpening(TransactionVm vm);
    Task<TransactionVm?> GetInventoryOpeningDataAsync();
    Task<TransactionVm?> GetInventoryOpeningDataByDeptIdAsync(long id);
    Task<string> GetReceiveByIdAsyncHtml(long id);
    Task<string> GetIssueByIdAsyncHtml(long id);
    Task<(bool, long)> DirectReceiveEntryAsync(TransactionVm vm);
    Task<string> DirectReceiveReportHtml(DirectReceiveReportVm vm, bool isPrint = false);
    Task<bool> AddOrUpdate(TransactionVm vm);
}
