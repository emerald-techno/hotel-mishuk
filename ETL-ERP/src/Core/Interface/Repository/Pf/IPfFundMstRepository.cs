using Domain.Entities.Pf;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.PfFund;
using Interface.Base;

namespace Interface.Repository.Pf
{
    public interface IPfFundMstRepository : IRepository<PfFundMst>
    {
        Task<PfFundMstVm> GetPfFundData(PfFundMstVm vm);
        Task<PfFundMst> GetPfByIdAsync(long id);
        Task<DataTablePagination<PfFundMstSearchVm, PfFundMstSearchVm>>
                                               SearchAsync(DataTablePagination<PfFundMstSearchVm, PfFundMstSearchVm> model);

    }
}
