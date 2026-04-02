using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccLedger;
using Interface.Base;

namespace Interface.Repository.Accounts
{
    public interface IAccLedgerRepository : IRepository<AccLedger>
    {
        Task<DataTablePagination<AccLedgerSearchVm, AccLedgerSearchVm>>
                                                     SearchAsync(DataTablePagination<AccLedgerSearchVm, AccLedgerSearchVm> model);

        Task<long> GetTotalLedgerByHeadId(long headId);
    }
}
