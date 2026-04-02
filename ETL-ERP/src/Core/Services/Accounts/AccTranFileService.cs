using AutoMapper;
using Domain.Entities.Accounting;
using Interface.Repository.Accounts;
using Interface.Services.Accounts;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Accounts
{
    public class AccTranFileService : BaseService<AccTranFile>, IAccTranFileService
    {
        #region Config
        private IAccTranFileRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public AccTranFileService(IAccTranFileRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork)
            : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }
        #endregion  
    }
}
