using AutoMapper;
using Domain.Entities.Pf;
using Interface.Repository.Pf;
using Interface.Services.Pf;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Pf;

public class PfSettingService : BaseService<PfSetting>, IPfSettingService
{
    #region Config
    private readonly IPfSettingRepository Repository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public PfSettingService(IPfSettingRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }
    #endregion
}
