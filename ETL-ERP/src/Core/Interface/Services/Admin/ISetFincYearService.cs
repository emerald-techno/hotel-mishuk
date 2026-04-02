using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.FinancialYear;
using Interface.Base;

namespace Interface.Services.Admin
{
    public interface ISetFincYearService : IService<SetFincYear>
    {
        Task<SetFincYear> GetFincYearByDate(DateTime queryDate);
        SetFincYear GetFinancialYearByDate(DateTime queryDate);
        Task<SetFincYear> GetFinancialYearById(long fincYearId);

        Task<DataTablePagination<SetFincYearSearchVm, SetFincYearSearchVm>>
            SearchAsync(DataTablePagination<SetFincYearSearchVm, SetFincYearSearchVm> model);
    }
}
