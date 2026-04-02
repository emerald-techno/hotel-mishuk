using AutoMapper;
using Domain.Entities.Pf;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.PfSettlement;
using Interface.Repository.Pf;
using Interface.Services.Pf;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Pf;

public class PfSettlementService : BaseService<PfSettlement>, IPfSettlementService
{
    #region Config
    private IPfSettlementRepository Repository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public PfSettlementService(IPfSettlementRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork)
        : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }
    #endregion

    #region Search

    public async Task<DataTablePagination<PfSettlementSearchVm, PfSettlementSearchVm>> SearchAsync(DataTablePagination<PfSettlementSearchVm, PfSettlementSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    #endregion
}
