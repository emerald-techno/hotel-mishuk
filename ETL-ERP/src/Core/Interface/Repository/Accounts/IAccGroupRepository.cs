using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccGroup;
using Interface.Base;

namespace Interface.Repository.Accounts
{
    public interface IAccGroupRepository : IRepository<AccGroup>
    {
        Task<DataTablePagination<AccGroupSearchVm, AccGroupSearchVm>>
                                                  SearchAsync(DataTablePagination<AccGroupSearchVm, AccGroupSearchVm> model);
    }
}
