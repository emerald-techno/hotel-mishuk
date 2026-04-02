using AutoMapper;
using Domain.Entities.HR;
using Interface.Repository.Hr;
using Interface.Services.Hr;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Hr
{
    public class HrSettingService : BaseService<HrSetting>, IHrSettingService
    {
        #region Config
        private readonly IHrSettingRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public HrSettingService(IHrSettingRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }
        #endregion
    }
}
