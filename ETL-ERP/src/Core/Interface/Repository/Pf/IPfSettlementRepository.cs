using Domain.Entities.Pf;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.PfSettlement;
using Interface.Base;

namespace Interface.Repository.Pf
{
    public interface IPfSettlementRepository : IRepository<PfSettlement>
    {
        Task<DataTablePagination<PfSettlementSearchVm, PfSettlementSearchVm>>
            SearchAsync(DataTablePagination<PfSettlementSearchVm, PfSettlementSearchVm> model);
    }
}
