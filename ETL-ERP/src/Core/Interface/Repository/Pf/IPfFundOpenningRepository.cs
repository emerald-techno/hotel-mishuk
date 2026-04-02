using Domain.Entities.Pf;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.PfFundOpenning;
using Interface.Base;

namespace Interface.Repository.Pf
{
    public interface IPfFundOpenningRepository : IRepository<PfFundOpenning>
    {
        Task<DataTablePagination<PfFundOpenningSearchVm, PfFundOpenningSearchVm>>
            SearchAsync(DataTablePagination<PfFundOpenningSearchVm, PfFundOpenningSearchVm> model);
    }
}
