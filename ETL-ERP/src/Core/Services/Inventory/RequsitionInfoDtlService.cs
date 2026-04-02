using AutoMapper;
using Domain.Entities.Inventory;
using Interface.Repository.Inventory;
using Interface.Services.Inventory;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Inventory
{
    public class RequsitionInfoDtlService : BaseService<RequsitionInfoDtl>, IRequsitionInfoDtlService
    {
        #region CONFIG

        private IRequsitionInfoDtlRepository Repository { get; }
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public RequsitionInfoDtlService(IRequsitionInfoDtlRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }

        #endregion
    }
}
