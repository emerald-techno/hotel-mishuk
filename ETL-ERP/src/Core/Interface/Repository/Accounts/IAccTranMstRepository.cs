using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccTranMst;
using Interface.Base;

namespace Interface.Repository.Accounts
{
    public interface IAccTranMstRepository : IRepository<AccTranMst>
    {
        Task<AccTranMstVm?> GetOpeningDetails(long finYearId);
        Task<AccTranMstVm?> GetClosingDetails(long finYearId);
        Task<AccTranMstVm> GetJournalDetails(long id);
        Task<DataTablePagination<AccTranMstSearchVm, AccTranMstSearchVm>>
                                                     SearchAsync(DataTablePagination<AccTranMstSearchVm, AccTranMstSearchVm> model);
    }
}
