using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccTranDtl;
using Interface.Base;

namespace Interface.Services.Accounts
{
    public interface IAccTranDtlService : IService<AccTranDtl>
    {
        Task<DataTablePagination<AccTranDtlSearchVm, AccTranDtlSearchVm>> SearchAsync(DataTablePagination<AccTranDtlSearchVm, AccTranDtlSearchVm> model);
    }
}
