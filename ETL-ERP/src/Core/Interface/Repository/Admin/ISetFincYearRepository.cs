using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.FinancialYear;
using Interface.Base;

namespace Interface.Repository.Admin;

public interface ISetFincYearRepository : IRepository<SetFincYear>
{
    Task<DataTablePagination<SetFincYearSearchVm, SetFincYearSearchVm>>
            SearchAsync(DataTablePagination<SetFincYearSearchVm, SetFincYearSearchVm> vm);
}
