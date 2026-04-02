using AutoMapper;
using Domain.Entities.Pf;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.PfFundOpenning;
using Interface.Repository.Pf;
using Interface.Services.Pf;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Pf;

public class PfFundOpenningService : BaseService<PfFundOpenning>, IPfFundOpenningService
{
    #region Config
    private IPfFundOpenningRepository Repository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public PfFundOpenningService(IPfFundOpenningRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork)
        : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }
    #endregion

    #region Search

    public async Task<DataTablePagination<PfFundOpenningSearchVm, PfFundOpenningSearchVm>>
        SearchAsync(DataTablePagination<PfFundOpenningSearchVm, PfFundOpenningSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    #endregion
}
