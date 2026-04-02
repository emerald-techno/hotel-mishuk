using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccHead;
using Interface.Base;

namespace Interface.Repository.Accounts
{
    public interface IAccHeadRepository : IRepository<AccHead>
    {
        Task<DataTablePagination<AccHeadSearchVm, AccHeadSearchVm>>
                                                     SearchAsync(DataTablePagination<AccHeadSearchVm, AccHeadSearchVm> model);
    }
}
