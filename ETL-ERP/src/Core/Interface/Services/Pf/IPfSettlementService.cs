using Domain.Entities.Pf;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.PfSettlement;
using Interface.Base;

namespace Interface.Services.Pf
{
    public interface IPfSettlementService : IService<PfSettlement>
    {
        Task<DataTablePagination<PfSettlementSearchVm, PfSettlementSearchVm>>
                           SearchAsync(DataTablePagination<PfSettlementSearchVm, PfSettlementSearchVm> model);
    }
}
