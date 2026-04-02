using Domain.Entities.Pf;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.PfFundOpenning;
using Interface.Base;

namespace Interface.Services.Pf
{
    public interface IPfFundOpenningService : IService<PfFundOpenning>
    {
        Task<DataTablePagination<PfFundOpenningSearchVm, PfFundOpenningSearchVm>>
                  SearchAsync(DataTablePagination<PfFundOpenningSearchVm, PfFundOpenningSearchVm> model);
    }
}
