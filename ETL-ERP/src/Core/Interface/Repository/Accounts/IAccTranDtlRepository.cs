using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccTranDtl;
using Interface.Base;

namespace Interface.Repository.Accounts
{
    public interface IAccTranDtlRepository : IRepository<AccTranDtl>
    {
        Task<DataTablePagination<AccTranDtlSearchVm, AccTranDtlSearchVm>>
                                             SearchAsync(DataTablePagination<AccTranDtlSearchVm, AccTranDtlSearchVm> model);
    }
}
